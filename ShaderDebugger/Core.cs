using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.Shaders;

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

    public class Core : IInitable<OpenGL>
    {
        private string _vertexShaderCode;
        private string _fragmentShaderCode;

        public string VertexShaderCode {
            get { return _vertexShaderCode; }
            set { _vertexShaderCode = value; state |= ShaderProgramState.ShadersChanged; }
        }

        public string FragmentShaderCode {
            get { return _fragmentShaderCode; }
            set { _fragmentShaderCode = value;  state |= ShaderProgramState.ShadersChanged; }
        }

        public IList<Uniform> uniforms { get; set; }
        public IList<Vertex> vertices { get; set; }

        private OpenGL gl;
        private ShaderProgram program;
        private ShaderProgramState state;

        public Core()
        {
            state = ShaderProgramState.Valid;
            uniforms = new List<Uniform>();
            vertices = new List<Vertex>();
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

            var attributeLocations = new Dictionary<uint, string>();
            program = new ShaderProgram();
            program.Create(gl, VertexShaderCode, FragmentShaderCode, attributeLocations);
            program.Bind(gl);
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

        public void Render()
        {
            if (state.HasFlag(ShaderProgramState.ShadersChanged))
            {
                // we need to recompile shaders
                RecompileShaders();
            }
            else {
                if (state.HasFlag(ShaderProgramState.UniformsChanged))
                {
                    // reset uniforms
                    foreach (Uniform uniform in uniforms)
                    {
                        var location = GetUniformLocation(uniform.Name);
                        uniform.Set(gl, location);
                    }
                }

                if (state.HasFlag(ShaderProgramState.VerticesChanged))
                {
                    // reset vertex buffers
                    // TODO: implement this!
                }
            }

            gl.BindVertexArray(vao);
            {
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                gl.Finish();
            }
            gl.BindVertexArray(0);
        }

    }
}
