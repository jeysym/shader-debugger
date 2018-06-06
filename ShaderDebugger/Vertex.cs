using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDebugger
{
    public class VertexCollection
    {
        IList<Vertex> Vertices { get; set; }
        IList<AttributeInfo> AttributeInfos { get; set; }
    }

    public class AttributeInfo
    {
        String Name { get; set; }
        GLType Type { get; set; }
        int? Location { get; set; }
    }

    public class Vertex
    {
        Dictionary<string, GLVariable> Attributes { get; set; }
    }
}
