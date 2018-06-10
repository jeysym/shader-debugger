using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace ShaderDebugger
{
    // ======================================================================================================
    // OPENGL TYPES INFO
    // ======================================================================================================

    /// <summary>
    /// Base type of OpenGL type.
    /// </summary>
    public enum GLBaseType
    {
        Float,
    }

    /// <summary>
    /// Number of components of OpenGL type (more accurately GLSL type).
    /// </summary>
    public enum GLComponentCount
    {
        One = 1, Two, Three, Four
    }

    /// <summary>
    /// Describes GLSL type.
    /// </summary>
    public class GLType : IEquatable<GLType>
    {
        /// <summary>
        /// Base type of GLSL type.
        /// </summary>
        public GLBaseType BaseType { get; private set; }

        /// <summary>
        /// Number of components of GLSL type.
        /// </summary>
        public GLComponentCount ComponentCount { get; private set; }

        public GLType(GLBaseType type, GLComponentCount count)
        {
            this.BaseType = type;
            this.ComponentCount = count;
        }

        public bool Equals(GLType other)
        {
            return (BaseType == other.BaseType) && (ComponentCount == other.ComponentCount);
        }

        /// <summary>
        /// Gets number of components of this GLSL type. Resulting value lies in interval [1-4].
        /// </summary>
        /// <returns>Number of components.</returns>
        public int GetComponentCount()
        {
            return (int)ComponentCount;
        }

        public override string ToString()
        {
            string typeName = "";
            string suffix = "";
            switch (BaseType)
            {
                case GLBaseType.Float:
                    typeName = "float";
                    suffix = "";
                    break;
            }

            int components = GetComponentCount();
            if (components == 1)
            {
                return $"{typeName}";
            }
            else
            {
                return $"vec{components}{suffix}";
            }
        }


        // ==================================================================================================
        // THE KNOWN GLSL TYPES
        // ==================================================================================================

        public static GLType FLOAT = new GLType(GLBaseType.Float, GLComponentCount.One);
        public static GLType VEC2 = new GLType(GLBaseType.Float, GLComponentCount.Two);
        public static GLType VEC3 = new GLType(GLBaseType.Float, GLComponentCount.Three);
        public static GLType VEC4 = new GLType(GLBaseType.Float, GLComponentCount.Four);
    }


    // ======================================================================================================
    // OPENGL VARIABLES
    // ======================================================================================================

    public abstract class GLVariable : NotifyPropertyChangedBase
    {
        protected abstract GLBaseType GetBaseGLType();
        protected abstract GLComponentCount GetGLComponentCount();

        /// <summary>
        /// Gets the GLSL type of this variable.
        /// </summary>
        public GLType GetGLType()
        {
            return new GLType(GetBaseGLType(), GetGLComponentCount());
        }

        /// <summary>
        /// Sets this GLSL variable as uniform.
        /// </summary>
        /// <param name="gl">OpenGL context.</param>
        /// <param name="location">Uniform location that the variable will be bound to.</param>
        public abstract void SetAsUniform(OpenGL gl, int location);
    }

    public abstract class GLFloatVariable : GLVariable
    {
        protected override GLBaseType GetBaseGLType()
        {
            return GLBaseType.Float;
        }
    }

    /// <summary>
    /// Float based GLSL variable.
    /// </summary>
    public class Float : GLFloatVariable
    {
        private float _Value;

        public float Value {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        protected override GLComponentCount GetGLComponentCount()
        {
            return GLComponentCount.One;
        }

        public override void SetAsUniform(OpenGL gl, int location)
        {
            gl.Uniform1(location, Value);
        }
    }

    /// <summary>
    /// GLSL variable vec2.
    /// </summary>
    public class Vec2f : GLFloatVariable
    {
        private float _X, _Y;

        public float X {
            get { return _X; }
            set { _X = value;  NotifyPropertyChanged(); }
        }

        public float Y {
            get { return _Y; }
            set { _Y = value; NotifyPropertyChanged(); }
        }

        protected override GLComponentCount GetGLComponentCount()
        {
            return GLComponentCount.Two;
        }

        public override void SetAsUniform(OpenGL gl, int location)
        {
            gl.Uniform2(location, X, Y);
        }
    }

    /// <summary>
    /// GLSL variable vec3.
    /// </summary>
    public class Vec3f : GLFloatVariable
    {
        private float _X, _Y, _Z;

        public float X
        {
            get { return _X; }
            set { _X = value; NotifyPropertyChanged(); }
        }

        public float Y
        {
            get { return _Y; }
            set { _Y = value; NotifyPropertyChanged(); }
        }

        public float Z
        {
            get { return _Z; }
            set { _Z = value; NotifyPropertyChanged(); }
        }

        protected override GLComponentCount GetGLComponentCount()
        {
            return GLComponentCount.Three;
        }

        public override void SetAsUniform(OpenGL gl, int location)
        {
            gl.Uniform3(location, X, Y, Z);
        }
    }

    /// <summary>
    /// GLSL variable vec4.
    /// </summary>
    public class Vec4f : GLFloatVariable
    {
        private float _X, _Y, _Z, _W;

        public float X
        {
            get { return _X; }
            set { _X = value; NotifyPropertyChanged(); }
        }

        public float Y
        {
            get { return _Y; }
            set { _Y = value; NotifyPropertyChanged(); }
        }

        public float Z
        {
            get { return _Z; }
            set { _Z = value; NotifyPropertyChanged(); }
        }

        public float W
        {
            get { return _W; }
            set { _W = value; NotifyPropertyChanged(); }
        }

        protected override GLComponentCount GetGLComponentCount()
        {
            return GLComponentCount.Four;
        }

        public override void SetAsUniform(OpenGL gl, int location)
        {
            gl.Uniform4(location, X, Y, Z, W);
        }
    }


    // ======================================================================================================
    // VARIABLE CREATION
    // ======================================================================================================

    /// <summary>
    /// Factory class that makes GLVariable.
    /// </summary>
    public static class VariableMaker
    {
        delegate GLVariable VariableCreator();

        private static IDictionary<GLType, VariableCreator> creatorsDictionary;

        static VariableMaker()
        {
            creatorsDictionary = new Dictionary<GLType, VariableCreator>();
            creatorsDictionary.Add(GLType.FLOAT, () => { return new Float(); });
            creatorsDictionary.Add(GLType.VEC2, () => { return new Vec2f(); });
            creatorsDictionary.Add(GLType.VEC3, () => { return new Vec3f(); });
            creatorsDictionary.Add(GLType.VEC4, () => { return new Vec4f(); });
        }

        /// <summary>
        /// Gets all GLSL types for which a variable can be created.
        /// </summary>
        public static ICollection<GLType> GetSupportedTypes()
        {
            return creatorsDictionary.Keys;
        }

        /// <summary>
        /// Makes variable of specified type.
        /// </summary>
        /// <param name="type">Type of variable to be created. It should be one of the supported types
        /// </param>
        public static GLVariable Make(GLType type)
        {
            VariableCreator creator;
            if (creatorsDictionary.TryGetValue(type, out creator))
            {
                var result = creator();
                return result;
            }

            return null;
        }
    }
}
