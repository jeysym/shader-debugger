Shader Debugger
===============

Description
-----------
This is a WPF application that can be used for testing OpenGL vertex and fragment shaders.

Motivation
----------
Interface for compiling and testing OpenGL shaders is very user non-friendly (as the rest of OpenGL).
This application provides an easy user interface for shader code editing, creating vertices or 
uniforms to be used by the shaders. It shows the output message of GLSL compiler and linker,
so it can be used to debug errors in shaders.

User documentation
------------------
User documentation can be found [here](user_doc.md).

Used packages
-------------
This application uses these NuGet packages:
* **SharpGL** - This package is a C# wrapper for OpenGL API. It also provides WPF control for 
OpenGL viewport.
* **Extended.Wpf.Toolkit** - This package contains lots of useful WPF controls, e.g. color pickers
or spinners.

Author
------
Created by [Jeysym](https://github.com/jeysym/) in 2018.

License
-------
This project is licensed under MIT licnse (see LICENSE file).
