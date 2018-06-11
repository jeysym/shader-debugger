using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShaderDebugger
{
    /// \mainpage Shader Debugger
    /// 
    /// \section description Description
    /// This is a WPF application that can be used for testing OpenGL vertex and fragment shaders.
    /// 
    /// \section motivation Motivation
    /// Interface for compiling and testing OpenGL shaders is very user non-friendly (as the rest of OpenGL).
    /// This application provides an easy user interface for shader code editing, creating vertices or 
    /// uniforms to be used by the shaders. It shows the output message of GLSL compiler and linker,
    /// so it can be used to debug errors in shaders.
    /// 
    /// \section build How to build
    /// Just open the .sln file in Visual Studio and build it.
    /// 
    /// \section packages Used packages
    /// This application uses these NuGet packages:
    /// * <b>SharpGL</b> - This package is a C# wrapper for OpenGL API. It also provides WPF control for 
    ///                    OpenGL viewport.
    /// * <b>Extended.Wpf.Toolkit</b> - This package contains lots of useful WPF controls, e.g. color pickers
    ///                                 or spinners.
    /// 
    /// \section author Author
    /// Created by [Jeysym](https://github.com/jeysym/) in 2018.
    /// 
    /// \section license License
    /// This project is licensed under MIT licnse (see LICENSE file).


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
}
