using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;           // Debug.Assert

namespace ShaderDebugger
{
    class Utils
    {
        static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        static int Clamp(int value, int min, int max)
        {
            Debug.Assert(min <= max);

            value = Min(value, max);
            value = Max(value, min);

            return value;
        }
    }
}
