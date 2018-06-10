using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDebugger
{
    /// <summary>
    /// Class that represents one vertex, which is basically just a collection of attributes.
    /// </summary>
    public class Vertex : NotifyPropertyChangedBase
    {
        private Dictionary<string, GLVariable> _Attributes;

        /// <summary>
        /// Attributes of this vertex. They are indexed by string id, that is stored in AttributeInfo.
        /// </summary>
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
