using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;           // Debug.Assert

namespace ShaderDebugger
{
    /// <summary>
    /// Contains some utility functions that can be used in application.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Gets a minimum of two input values
        /// </summary>
        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// Gets a maximum of two input values
        /// </summary>
        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Clamps a value to a specified interval [min, max].
        /// </summary>
        /// <param name="value">Value to be clamped.</param>
        /// <param name="min">Minimum of the clamped value.</param>
        /// <param name="max">Maximum of the clamped value.</param>
        /// <returns>Clamped value.</returns>
        static int Clamp(int value, int min, int max)
        {
            Debug.Assert(min <= max);

            value = Min(value, max);
            value = Max(value, min);

            return value;
        }
    }
}
