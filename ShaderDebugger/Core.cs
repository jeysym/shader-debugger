using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.Shaders;
using System.Collections.ObjectModel;

namespace ShaderDebugger
{
    interface IInitable<T>
    {
        void Init(T initInfo);
        bool IsInitialized();
    }

    [Flags]
    enum ShaderProgramState
    {
        Valid = 0,
        ShadersChanged = 1,
        UniformsChanged = 2,
        VerticesChanged = 8
    }

    public class Core : NotifyPropertyChangedBase, IInitable<OpenGL>
    {
        // ====================================================================
        // PRIVATE FIELDS
        // ====================================================================

        // PROPERTY FIELDS
        
        private string _vertexShaderCode;
        private string _fragmentShaderCode;
        private ObservableCollection<Uniform> _uniforms;
        private ObservableCollection<Vertex> _vertices;
        private Dictionary<string, AttributeInfo> _attributeInfos;
        private string _errorOutput;

        // OTHER FIELDS

        private OpenGL gl;
        private ShaderProgram program;
        private ShaderProgramState state;
        private int nextAttributeId = 0;

        // ====================================================================
        // PROPERTIES
        // ====================================================================

        public string VertexShaderCode {
            get { return _vertexShaderCode; }
            set { _vertexShaderCode = value; state |= ShaderProgramState.ShadersChanged; NotifyPropertyChanged(); }
        }

        public string FragmentShaderCode {
            get { return _fragmentShaderCode; }
            set { _fragmentShaderCode = value; state |= ShaderProgramState.ShadersChanged; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Uniform> Uniforms {
            get { return _uniforms; }
            set { _uniforms = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Vertex> Vertices {
            get { return _vertices; }
            set { _vertices = value; NotifyPropertyChanged(); }
        }

        public Dictionary<string, AttributeInfo> AttributeInfos
        {
            get { return _attributeInfos; }
            set { _attributeInfos = value; NotifyPropertyChanged(); }
        }

        public string ErrorOutput
        {
            get { return _errorOutput; }
            set { _errorOutput = value; NotifyPropertyChanged(); }
        }

        public bool WasCompiledWithoutError
        {
            get { return _errorOutput == null; }
        }


        // ====================================================================
        // UNIFORM / VERTEX MANIPULATION
        // ====================================================================

        public void AddUniform(Uniform newUniform)
        {
            Uniforms.Add(newUniform);
            newUniform.Variable.PropertyChanged += OnUniformChange;
            newUniform.PropertyChanged += OnUniformChange;
        }

        public void AddNewVertex()
        {
            Vertex vertex = new Vertex();
            
            foreach (var pair in AttributeInfos)
            {
                var attributeInfo = pair.Value;
                vertex.Attributes.Add(attributeInfo.Id, attributeInfo.CreateNewVariable());
            }

            Vertices.Add(vertex);
        }

        public void AddNewAttribute(AttributeInfo attributeInfo)
        {
            string id = nextAttributeId.ToString();
            attributeInfo.Id = id;
            nextAttributeId++;

            AttributeInfos.Add(id, attributeInfo);
            foreach (Vertex vertex in Vertices) {
                var newVariable = attributeInfo.CreateNewVariable();
                newVariable.PropertyChanged += OnAttributeChange;
                vertex.Attributes.Add(id, newVariable);
            }
        }

        public void RemoveAttribute(string id)
        {
            AttributeInfos.Remove(id);

            foreach (Vertex vertex in Vertices)
            {
                vertex.Attributes.Remove(id);
            }
        }

        
        // ====================================================================
        // HANDLING CHANGES
        // ====================================================================

        // Is called when single uniform changes in some way.
        private void OnUniformChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            state |= ShaderProgramState.UniformsChanged;
        }

        // Is called when uniform collection changes (uniform added, removed, etc.)
        private void OnUniformCollectionChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            state |= ShaderProgramState.UniformsChanged;
        }

        private void OnVertexCollectionChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            state |= ShaderProgramState.VerticesChanged;
        }

        private void OnAttributeChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            state |= ShaderProgramState.VerticesChanged;
        }


        public Core()
        {
            state = ShaderProgramState.Valid;
            Uniforms = new ObservableCollection<Uniform>();
            Vertices = new ObservableCollection<Vertex>();
            AttributeInfos = new Dictionary<string, AttributeInfo>();

            Uniforms.CollectionChanged += OnUniformCollectionChange;
            Vertices.CollectionChanged += OnVertexCollectionChange;
        }

        public void Init(OpenGL gl)
        {
            this.gl = gl;
            var attributeLocations = new Dictionary<uint, string>();

            program = new ShaderProgram();
            program.Create(gl, VertexShaderCode, FragmentShaderCode, attributeLocations);
            program.Bind(gl);

            DebugCreateArrays();    // TODO : only for debug purposes
        }

        public bool IsInitialized()
        {
            return (gl != null);
        }

        private int GetUniformLocation(string uniformName)
        {
            return program.GetUniformLocation(gl, uniformName);
        }

        private void RecompileShaders()
        {
            program.Delete(gl);

            try
            {
                var attributeLocations = new Dictionary<uint, string>();
                program = new ShaderProgram();
                program.Create(gl, VertexShaderCode, FragmentShaderCode, attributeLocations);
                program.Bind(gl);
            }
            catch (ShaderCompilationException e)
            {
                ErrorOutput = e.CompilerOutput;
                program.Delete(gl);
                return;
            }

            ErrorOutput = null;
        }

        private void ResetUniforms()
        {
            if (Uniforms == null)
                return;

            foreach (Uniform uniform in Uniforms)
            {
                int location = GetUniformLocation(uniform.Name);

                if (location != -1)
                {
                    uniform.Location = location;
                    uniform.Set(gl, location);
                }
                else
                {
                    uniform.Location = null;
                }
            }
        }

        private void RebufferVertices()
        {
            
        }

        private uint vbo = 0;
        private uint vao = 0;

        private float[] data = {
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.0f,  0.5f, 0.0f
        };

        private void DebugCreateArrays()
        {
            uint[] temp = new uint[1];

            // create VBO
            gl.GenBuffers(1, temp);
            vbo = temp[0];

            // create VAO
            gl.GenVertexArrays(1, temp);
            vao = temp[0];

            gl.BindVertexArray(vao);
            {
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vbo);
                gl.BufferData(OpenGL.GL_ARRAY_BUFFER, data, OpenGL.GL_STATIC_DRAW);
                gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 3 * sizeof(float), (IntPtr)0);
                gl.EnableVertexAttribArray(0);
            }
            gl.BindVertexArray(0);
        }

        private void CheckValidity()
        {
            if (state.HasFlag(ShaderProgramState.ShadersChanged))
            {
                // we need to recompile shaders
                RecompileShaders();
                ResetUniforms();
                RebufferVertices();
            }
            else
            {
                if (state.HasFlag(ShaderProgramState.UniformsChanged))
                {
                    // reset uniforms
                    ResetUniforms();
                }

                if (state.HasFlag(ShaderProgramState.VerticesChanged))
                {
                    // reset vertex buffers
                    RebufferVertices();
                }
            }

            state = ShaderProgramState.Valid;
        }

        public void Render(int width, int height)
        {
            gl.Viewport(0, 0, width, height);
            gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            CheckValidity();

            gl.BindVertexArray(vao);
            {
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                gl.Finish();
            }
            gl.BindVertexArray(0);
        }

    }
}
