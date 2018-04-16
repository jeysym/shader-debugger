using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace ShaderDebugger
{
    public abstract class Uniform
    {
        /// <summary>
        /// Name of this uniform.
        /// </summary>
        public string Name { get; private set; }

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
        Float Value { get; set; }

        public FloatUniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform1(location, Value.Value);
        }
    }

    public class Vec2fUniform : Uniform
    {
        Vec2f Value { get; set; }

        public Vec2fUniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform2(location, Value.X, Value.Y);
        }
    }

    public class Vec3Uniform : Uniform
    {
        Vec3f Value { get; set; }

        public Vec3Uniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform3(location, Value.X, Value.Y, Value.Z);
        }
    }

    public class Vec4Uniform : Uniform
    {
        Vec4f Value { get; set; }

        public Vec4Uniform(string name) : base(name)
        { }

        public override void Set(OpenGL gl, int location)
        {
            gl.Uniform4(location, Value.X, Value.Y, Value.Z, Value.W);
        }
    }
}
