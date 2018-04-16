using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        /// Can be used to check, whether this uniform is valid in the current shader-program context.
        /// </summary>
        public bool HasValidLocation
        {
            get { return _Location != null; }
        }

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
    }

    public class FloatUniform : Uniform
    {
        Float _Value;

        public Float Value {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public FloatUniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform1(location, Value.Value);
        }
    }

    public class Vec2fUniform : Uniform
    {
        Vec2f _Value;

        public Vec2f Value
        {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public Vec2fUniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform2(location, Value.X, Value.Y);
        }
    }

    public class Vec3Uniform : Uniform
    {
        Vec3f _Value;

        public Vec3f Value
        {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public Vec3Uniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform3(location, Value.X, Value.Y, Value.Z);
        }
    }

    public class Vec4Uniform : Uniform
    {
        Vec4f _Value;

        public Vec4f Value
        {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public Vec4Uniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform4(location, Value.X, Value.Y, Value.Z, Value.W);
        }
    }

}
