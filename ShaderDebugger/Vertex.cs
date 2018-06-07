using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDebugger
{
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
