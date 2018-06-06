using System;
using System.Collections.Generic;
using SharpGL;

namespace ShaderDebugger
{
    public abstract class Uniform : NotifyPropertyChangedBase
    {
        private string _Name;
        private int? _Location;

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
        /// Gets string description of the uniform type. (float, vec2, vec3 etc.)
        /// </summary>
        public string TypeDescription
        {
            get { return GetTypeDescription(); }
        }

        /// <summary>
        /// Can be used to check, whether this uniform is valid in the current shader-program context.
        /// </summary>
        public bool HasValidLocation
        {
            get { return _Location != null; }
        }

        /// <summary>
        /// Gets the actual variable stored in this uniform.
        /// </summary>
        public abstract GLVariable Variable { get; }

        protected Uniform(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Sets this uniform in given OpenGL context.
        /// </summary>
        /// <param name="gl">Represents the OpenGL context to use.</param>
        /// <param name="location">Location that this uniform value will be bound to.</param>
        public abstract void Set(OpenGL gl, int location);

        public abstract string GetTypeDescription();
    }

    public class UniformMaker
    {
        delegate Uniform UniformCreator(string name);

        private static IDictionary<GLType, UniformCreator> creatorsDictionary;

        static UniformMaker()
        {
            creatorsDictionary = new Dictionary<GLType, UniformCreator>();
            creatorsDictionary.Add(GLType.Float, (name) => { return new FloatUniform(name); });
            creatorsDictionary.Add(GLType.Vec2, (name) => { return new Vec2Uniform(name); });
            creatorsDictionary.Add(GLType.Vec3, (name) => { return new Vec3Uniform(name); });
            creatorsDictionary.Add(GLType.Vec4, (name) => { return new Vec4Uniform(name); });
        }
        
        public static ICollection<GLType> GetSupportedTypes()
        {
            return creatorsDictionary.Keys;
        }

        public static Uniform Make(GLType type, string name)
        {
            UniformCreator creator;
            if (creatorsDictionary.TryGetValue(type, out creator))
            {
                var result = creator(name);
                return result;
            }

            return null;
        }
    }

    public class FloatUniform : Uniform
    {
        Float _Value;

        public override GLVariable Variable => _Value;

        public Float Value {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public FloatUniform(string name) : base(name)
        {
            Value = new Float();
        }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform1(location, Value.Value);
        }

        public override string GetTypeDescription()
        {
            return "float";
        }
    }

    public class Vec2Uniform : Uniform
    {
        Vec2f _Value;

        public override GLVariable Variable => _Value;

        public Vec2f Value
        {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public Vec2Uniform(string name) : base(name)
        {
            Value = new Vec2f();
        }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform2(location, Value.X, Value.Y);
        }

        public override string GetTypeDescription()
        {
            return "vec2";
        }
    }

    public class Vec3Uniform : Uniform
    {
        Vec3f _Value;

        public override GLVariable Variable => _Value;

        public Vec3f Value
        {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public Vec3Uniform(string name) : base(name)
        {
            Value = new Vec3f();
        }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform3(location, Value.X, Value.Y, Value.Z);
        }

        public override string GetTypeDescription()
        {
            return "vec3";
        }
    }

    public class Vec4Uniform : Uniform
    {
        Vec4f _Value;

        public override GLVariable Variable => _Value;

        public Vec4f Value
        {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public Vec4Uniform(string name) : base(name)
        {
            Value = new Vec4f();
        }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform4(location, Value.X, Value.Y, Value.Z, Value.W);
        }

        public override string GetTypeDescription()
        {
            return "vec4";
        }
    }

}
