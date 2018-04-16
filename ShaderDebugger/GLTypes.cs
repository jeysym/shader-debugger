using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDebugger
{
    public abstract class GLType
    {

    }

    public class Float : GLType
    {
        public float Value { get; set; }
    }

    public class Vec2f : GLType
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class Vec3f : GLType
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class Vec4f : GLType
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }
}
