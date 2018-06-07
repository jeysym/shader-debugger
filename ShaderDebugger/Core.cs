using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
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
        // ==================================================================================================
        // PRIVATE FIELDS
        // ==================================================================================================

        // PROPERTY FIELDS
        
        private string _VertexShaderCode;
        private string _FragmentShaderCode;
        private ObservableCollection<Uniform> _Uniforms;
        private ObservableCollection<Vertex> _Vertices;
        private Dictionary<string, AttributeInfo> _AttributeInfos;
        private string _ErrorOutput;

        // OTHER FIELDS

        private OpenGL gl;
        private ShaderProgram program;
        private ShaderProgramState state;

        private uint mainVAO = 0;
        private uint floatVBO = 0;


        // ==================================================================================================
        // PROPERTIES
        // ==================================================================================================

        /// <summary>
        /// Gets or sets the GLSL code for vertex shader.
        /// </summary>
        public string VertexShaderCode {
            get { return _VertexShaderCode; }
            set
            {
                _VertexShaderCode = value;
                state |= ShaderProgramState.ShadersChanged;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the GLSL code for fragment shader.
        /// </summary>
        public string FragmentShaderCode {
            get { return _FragmentShaderCode; }
            set
            {
                _FragmentShaderCode = value;
                state |= ShaderProgramState.ShadersChanged;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of shader program uniform variables.
        /// </summary>
        public ObservableCollection<Uniform> Uniforms {
            get { return _Uniforms; }
            set
            {
                _Uniforms = value;
                state |= ShaderProgramState.UniformsChanged;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of vertices (with their attributes) that serve as vertex shader input.
        /// </summary>
        public ObservableCollection<Vertex> Vertices {
            get { return _Vertices; }
        }

        /// <summary>
        /// Dictionary with information about attributes. Do not use this property to add or remove 
        /// attributes, use AddAttribute() or RemoveAttribute() instead.
        /// </summary>
        public Dictionary<string, AttributeInfo> AttributeInfos
        {
            get { return _AttributeInfos; }
        }

        /// <summary>
        /// Error output from GLSL compiler and linker.
        /// </summary>
        public string ErrorOutput
        {
            get { return _ErrorOutput; }
            set { _ErrorOutput = value; NotifyPropertyChanged(); }
        }

        public bool WasCompiledWithoutError
        {
            get { return _ErrorOutput == null; }
        }


        // ==================================================================================================
        // UNIFORM / VERTEX MANIPULATION
        // ==================================================================================================

        /// <summary>
        /// Adds new attribute for all vertices.
        /// </summary>
        /// <param name="attributeInfo">Structure describing the attribute</param>
        public void AddAttribute(AttributeInfo attributeInfo)
        {
            AttributeInfos.Add(attributeInfo.Id, attributeInfo);

            // Ensure that all vertices will have this attribute.
            foreach (Vertex vertex in Vertices) {
                var newVariable = attributeInfo.CreateNewVariable();
                newVariable.PropertyChanged += OnAttributeChange;
                vertex.Attributes.Add(attributeInfo.Id, newVariable);
            }
        }

        /// <summary>
        /// Removes attribute specified by its id from all the vertices.
        /// </summary>
        /// <param name="id">Specifies the attribute to be removed</param>
        public void RemoveAttribute(string id)
        {
            AttributeInfos.Remove(id);

            foreach (Vertex vertex in Vertices)
            {
                vertex.Attributes.Remove(id);
            }
        }


        // ==================================================================================================
        // HANDLING CHANGES
        // ==================================================================================================

        /// <summary>
        /// Is called when single uniform changes in some way.
        /// </summary>
        private void OnUniformChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            state |= ShaderProgramState.UniformsChanged;
        }

        /// <summary>
        /// Is called when single vertex changes in some way.
        /// </summary>
        private void OnVertexChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            state |= ShaderProgramState.VerticesChanged;
        }

        /// <summary>
        /// Is called when single attribute variable changes in some way.
        /// </summary>
        private void OnAttributeChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            state |= ShaderProgramState.VerticesChanged;
        }

        /// <summary>
        /// Is called when uniform collection changes (uniform added, removed, etc.).
        /// </summary>
        private void OnUniformCollectionChange(
            object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    // Have new elements in collection notify this class when they get changed.
                    foreach (var obj in e.NewItems)
                    {
                        Uniform uniform = (Uniform)obj;
                        uniform.PropertyChanged += OnUniformChange;
                        uniform.Variable.PropertyChanged += OnUniformChange;
                    }
                    break;
            }

            state |= ShaderProgramState.UniformsChanged;
        }

        /// <summary>
        /// Is called when vertex collection changes (uniform added, removed, etc.).
        /// </summary>
        private void OnVertexCollectionChange(
            object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    // Have new elements in collection notify this class when they get changed.
                    // Also ensure that new vertices will have all the already defined attributes.
                    foreach (var obj in e.NewItems)
                    {
                        Vertex vertex = (Vertex)obj;
                        vertex.PropertyChanged += OnVertexChange;
                        
                        foreach (AttributeInfo attributeInfo in AttributeInfos.Values)
                        {
                            if (vertex.Attributes.ContainsKey(attributeInfo.Id) == false)
                            {
                                vertex.Attributes.Add(attributeInfo.Id, attributeInfo.CreateNewVariable());
                            }

                            vertex.Attributes[attributeInfo.Id].PropertyChanged += OnVertexChange;
                        }
                    }
                    break;
            }

            state |= ShaderProgramState.VerticesChanged;
        }


        // ==================================================================================================
        // OTHER
        // ==================================================================================================

        public Core()
        {
            state |= ShaderProgramState.ShadersChanged;
            state |= ShaderProgramState.UniformsChanged;
            state |= ShaderProgramState.VerticesChanged;

            Uniforms = new ObservableCollection<Uniform>();
            _Vertices = new ObservableCollection<Vertex>();
            _AttributeInfos = new Dictionary<string, AttributeInfo>();

            Uniforms.CollectionChanged += OnUniformCollectionChange;
            Vertices.CollectionChanged += OnVertexCollectionChange;
        }

        public void GenerateBuffers()
        {
            uint[] temp = new uint[1];

            // create VBOs
            gl.GenBuffers(1, temp);
            floatVBO = temp[0];

            // create VAO
            gl.GenVertexArrays(1, temp);
            mainVAO = temp[0];
        }

        public void Init(OpenGL gl)
        {
            this.gl = gl;
            var attributeLocations = new Dictionary<uint, string>();

            program = new ShaderProgram();
            program.Create(gl, VertexShaderCode, FragmentShaderCode, attributeLocations);
            program.Bind(gl);

            GenerateBuffers();
        }

        public bool IsInitialized()
        {
            return (gl != null);
        }

        private int GetUniformLocation(string uniformName)
        {
            return program.GetUniformLocation(gl, uniformName);
        }

        private int GetAttributeLocation(string attributeName)
        {
            return gl.GetAttribLocation(program.ShaderProgramObject, attributeName);
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

        private void RebufferFloats()
        {
            var floatAttrs = from attrInfo in AttributeInfos.Values
                             where attrInfo.Type.BaseType == GLBaseType.Float
                             select attrInfo;

            // Update attribute location.
            foreach (var floatAttr in floatAttrs)
            {
                int location = GetAttributeLocation(floatAttr.Name);
                if (location != -1)
                {
                    floatAttr.Location = location;
                }
                else
                {
                    floatAttr.Location = null;
                }
            }

            var validFloatAttrs = from floatAttr in floatAttrs
                                  where floatAttr.HasValidLocation
                                  select floatAttr;

            int stride = 0;
            int bufferSize = 0;

            foreach (var floatAttr in validFloatAttrs)
            {
                stride += floatAttr.Type.GetComponentCount() * sizeof(float);
                bufferSize += floatAttr.Type.GetComponentCount() * Vertices.Count;
            }

            float[] bufferData = new float[bufferSize];
            int currentIndex = 0;
            for (int iVertex = 0; iVertex < Vertices.Count; ++iVertex)
            {
                Vertex vertex = Vertices[iVertex];
                foreach (var floatAttr in validFloatAttrs)
                {
                    floatAttr.WriteToFloatBuffer(vertex, bufferData, currentIndex);
                    currentIndex += floatAttr.Type.GetComponentCount();
                }
            }

            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, floatVBO);
            {
                gl.BufferData(OpenGL.GL_ARRAY_BUFFER, bufferData, OpenGL.GL_STATIC_DRAW);

                int startIndex = 0;
                foreach (var floatAttr in validFloatAttrs)
                {
                    uint location = (uint)floatAttr.Location.Value;
                    int size = floatAttr.Type.GetComponentCount();

                    gl.VertexAttribPointer(
                        location, 
                        size, 
                        OpenGL.GL_FLOAT, 
                        false, 
                        stride, 
                        (IntPtr)startIndex);

                    gl.EnableVertexAttribArray(location);

                    startIndex += size * sizeof(float);
                }
            }
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
        }

        private void RebufferVertices()
        {
            gl.BindVertexArray(mainVAO);
            {
                RebufferFloats();
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
            CheckValidity();

            gl.Viewport(0, 0, width, height);
            gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.BindVertexArray(mainVAO);
            {
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                gl.Finish();
            }
            gl.BindVertexArray(0);
        }

    }
}
