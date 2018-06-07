using System;
using System.Collections.Generic;
using SharpGL;

namespace ShaderDebugger
{
    public class Uniform : NotifyPropertyChangedBase
    {
        private string _Name;
        private int? _Location;
        private GLVariable _Variable;

        /// <summary>
        /// Name of this uniform.
        /// </summary>
        public string Name {
            get { return _Name; }
            set { _Name = value;  NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Location of this uniform. Null value indicates that the location could not be retrieved.
        /// </summary>
        public int? Location
        {
            get { return _Location; }
            set { _Location = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets the actual variable stored in this uniform.
        /// </summary>
        public GLVariable Variable {
            get { return _Variable; }
            set { _Variable = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets string description of the uniform type. (float, vec2, vec3 etc.)
        /// </summary>
        public string TypeDescription
        {
            get { return Variable.GetGLType().ToString(); }
        }

        /// <summary>
        /// Can be used to check, whether this uniform is valid in the current shader-program context.
        /// </summary>
        public bool HasValidLocation
        {
            get { return _Location != null; }
        }


        public Uniform(string name, GLVariable variable)
        {
            Name = name;
            Variable = variable;
        }

        /// <summary>
        /// Sets this uniform in given OpenGL context.
        /// </summary>
        /// <param name="gl">Represents the OpenGL context to use.</param>
        /// <param name="location">Location that this uniform value will be bound to.</param>
        public void Set(OpenGL gl, int location)
        {
            Variable.SetAsUniform(gl, location);
        }
    }

    public static class UniformMaker
    {   
        public static ICollection<GLType> GetSupportedTypes()
        {
            return VariableMaker.GetSupportedTypes();
        }

        public static Uniform Make(GLType type, string name)
        {
            GLVariable variable = VariableMaker.Make(type);

            return new Uniform(name, variable);
        }
    }
}
