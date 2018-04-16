using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDebugger
{
    public class GLType : NotifyPropertyChangedBase
    {
        
    }

    public class Float : GLType
    {
        private float _Value;

        public float Value {
            get { return _Value; }
            set { _Value = value; NotifyPropertyChanged(); }
        }
    }

    public class Vec2f : GLType
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
    }

    public class Vec3f : GLType
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
    }

    public class Vec4f : GLType
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
    }
}
