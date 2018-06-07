using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpGL;

namespace ShaderDebugger
{
    public class AttributeInfo : NotifyPropertyChangedBase
    {
        private string _Id;
        private string _Name;
        private GLType _Type;
        private int? _Location;


        // ==================================================================================================
        // PROPERTIES
        // ==================================================================================================

        /// <summary>
        /// Identifies this attribute.
        /// </summary>
        public string Id
        {
            get { return _Id; }
            set { _Id = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Name of this attribute.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// OpenGL type of this attribute.
        /// </summary>
        public GLType Type
        {
            get { return _Type; }
            set { _Type = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Location of this attribute in shader program context.
        /// </summary>
        public int? Location
        {
            get { return _Location; }
            set { _Location = value; NotifyPropertyChanged(); }
        }

        public bool HasValidLocation
        {
            get { return _Location != null; }
        }


        public AttributeInfo(string id, string name, GLType type)
        {
            Debug.Assert(VariableMaker.GetSupportedTypes().Contains(type));

            Id = id;
            Name = name;
            Type = type;
        }

        public GLVariable CreateNewVariable()
        {
            return VariableMaker.Make(Type);
        }

        public void WriteToFloatBuffer(Vertex vertex, float[] buffer, int where)
        {
            if (Type.BaseType != GLBaseType.Float)
                throw new Exception("This attribute is not float based!");

            GLVariable vertexVar = vertex.Attributes[Id];

            switch (Type.ComponentCount)
            {
                case GLComponentCount.One:
                    {
                        Float variable = (Float)vertexVar;
                        buffer[where] = variable.Value;
                    } break;
                case GLComponentCount.Two:
                    {
                        Vec2f variable = (Vec2f)vertexVar;
                        buffer[where    ] = variable.X;
                        buffer[where + 1] = variable.Y;
                    } break;
                case GLComponentCount.Three:
                    {
                        Vec3f variable = (Vec3f)vertexVar;
                        buffer[where    ] = variable.X;
                        buffer[where + 1] = variable.Y;
                        buffer[where + 2] = variable.Z;
                    } break;
                case GLComponentCount.Four:
                    {
                        Vec4f variable = (Vec4f)vertexVar;
                        buffer[where    ] = variable.X;
                        buffer[where + 1] = variable.Y;
                        buffer[where + 2] = variable.Z;
                        buffer[where + 3] = variable.W;
                    } break;
                default:
                    throw new Exception("This code should be unreachable!");
            }
        }
    }

    /// <summary>
    /// Creates AttributeInfo instances. Also makes sure that attributes will have different Ids.
    /// </summary>
    class AttributeMaker
    {
        static long nextId = 0;

        /// <summary>
        /// Returns all OpenGL types, for which AttributeInfo can be created.
        /// </summary>
        public static ICollection<GLType> GetSupportedTypes()
        {
            return VariableMaker.GetSupportedTypes();
        }

        /// <summary>
        /// Makes AttributeInfo of specified type and name.
        /// </summary>
        /// <param name="type">OpenGL type of the new attribute</param>
        /// <param name="name">Name of the new attribute</param>
        /// <returns></returns>
        public static AttributeInfo Make(GLType type, string name)
        {
            string id = (nextId++).ToString();
            return new AttributeInfo(id, name, type);
        }
    }
}
