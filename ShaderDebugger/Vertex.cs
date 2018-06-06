using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDebugger
{
    public class AttributeInfo : NotifyPropertyChangedBase
    {
        private string _Id;
        private string _Name;
        private GLType _Type;
        private int? _Location;

        public string Id {
            get { return _Id; }
            set { _Id = value; NotifyPropertyChanged(); }
        }
        
        public string Name {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

        public GLType Type {
            get { return _Type; }
            set { _Type = value; NotifyPropertyChanged(); }
        }

        public int? Location {
            get { return _Location; }
            set { _Location = value; NotifyPropertyChanged(); }
        }

        public bool HasValidLocation
        {
            get { return _Location != null; }
        }

        public GLVariable CreateNewVariable()
        {
            return VariableMaker.Make(Type);
        }
    }

    public class Vertex : NotifyPropertyChangedBase
    {
        private Dictionary<string, GLVariable> _Attributes;

        public Dictionary<string, GLVariable> Attributes {
            get { return _Attributes; }
            set { _Attributes = value; NotifyPropertyChanged(); }
        }

        public Vertex()
        {
            Attributes = new Dictionary<string, GLVariable>();
        }
    }
}
