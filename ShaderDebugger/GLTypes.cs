using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDebugger
{
    public enum GLType
    {
        Float, Vec2, Vec3, Vec4
    }

    public abstract class GLVariable : NotifyPropertyChangedBase
    {
        public abstract GLType GetGLType();
    }

    public class Float : GLVariable
    {
        private float _Value;

        public float Value {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }

        public override GLType GetGLType()
        {
            return GLType.Float;
        }
    }

    public class Vec2f : GLVariable
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

        public override GLType GetGLType()
        {
            return GLType.Vec2;
        }
    }

    public class Vec3f : GLVariable
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

        public override GLType GetGLType()
        {
            return GLType.Vec3;
        }
    }

    public class Vec4f : GLVariable
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

        public override GLType GetGLType()
        {
            return GLType.Vec4;
        }
    }

    public class VariableMaker
    {
        delegate GLVariable VariableCreator();

        private static IDictionary<GLType, VariableCreator> creatorsDictionary;

        static VariableMaker()
        {
            creatorsDictionary = new Dictionary<GLType, VariableCreator>();
            creatorsDictionary.Add(GLType.Float, () => { return new Float(); });
            creatorsDictionary.Add(GLType.Vec2, () => { return new Vec2f(); });
            creatorsDictionary.Add(GLType.Vec3, () => { return new Vec3f(); });
            creatorsDictionary.Add(GLType.Vec4, () => { return new Vec4f(); });
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
