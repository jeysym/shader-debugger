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

    public enum GLBaseType
    {
        Float,
    }

    public enum GLComponentCount
    {
        One = 1, Two, Three, Four
    }

    public class GLType : IEquatable<GLType>
    {
        public GLBaseType BaseType { get; private set; }
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
        public abstract GLBaseType GetBaseGLType();
        public abstract GLComponentCount GetGLComponentCount();

        public GLType GetGLType()
        {
            return new GLType(GetBaseGLType(), GetGLComponentCount());
        }

        public abstract void SetAsUniform(OpenGL gl, int location);
    }

    public abstract class GLFloatVariable : GLVariable
    {
        public override GLBaseType GetBaseGLType()
        {
            return GLBaseType.Float;
        }
    }

    public class Float : GLFloatVariable
    {
        private float _Value;

        public float Value {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public override GLComponentCount GetGLComponentCount()
        {
            return GLComponentCount.One;
        }

        public override void SetAsUniform(OpenGL gl, int location)
        {
            gl.Uniform1(location, Value);
        }
    }

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

        public override GLComponentCount GetGLComponentCount()
        {
            return GLComponentCount.Two;
        }

        public override void SetAsUniform(OpenGL gl, int location)
        {
            gl.Uniform2(location, X, Y);
        }
    }

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

        public override GLComponentCount GetGLComponentCount()
        {
            return GLComponentCount.Three;
        }

        public override void SetAsUniform(OpenGL gl, int location)
        {
            gl.Uniform3(location, X, Y, Z);
        }
    }

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

        public override GLComponentCount GetGLComponentCount()
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

        public static ICollection<GLType> GetSupportedTypes()
        {
            return creatorsDictionary.Keys;
        }

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
