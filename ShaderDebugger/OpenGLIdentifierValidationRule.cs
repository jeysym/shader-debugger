﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShaderDebugger
{
    /// <summary>
    /// Validation rule that checks if a string is correct OpenGL variable name. It can be attached to 
    /// Binding.
    /// </summary>
    public class OpenGLIdentifierValidationRule : ValidationRule
    {
        private static Regex idRegex;

        static OpenGLIdentifierValidationRule() {
            idRegex = new Regex(@"^[a-zA-Z][a-zA-Z0-9_]*$");
        }

        /// <summary>
        /// Validates a string. Checks whether it is a correct OpenGL variable name.
        /// </summary>
        /// <param name="value">Pass string here.</param>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;

            if (str == null)
            {
                return new ValidationResult(false, "This is not even string!");
            }

            if (idRegex.IsMatch(str))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "This is not correct OpenGL identifier!");
            }
        }
    }
}
