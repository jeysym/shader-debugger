using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;

namespace ShaderDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string GetDefVShaderCode()
        {
            return (string)this.Resources ["defaultVertexShaderCode"];
        }

        private string GetDefFShaderCode()
        {
            return (string)this.Resources["defaultFragmentShaderCode"];
        }

        private Core GetCore()
        {
            return (Core)this.Resources["core"];
        }

        public MainWindow()
        {
            InitializeComponent();

            var core = GetCore();
            var vCode = GetDefVShaderCode();
            var fCode = GetDefFShaderCode();

            vertexShaderTextBox.Text = vCode;
            fragmentShaderTextBox.Text = fCode;

            core.VertexShaderCode = vCode;
            core.FragmentShaderCode = fCode;
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            var core = GetCore();

            core.Init(gl);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            var core = GetCore();

            gl.Viewport(0, 0, 800, 600);
            gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            core.Render();
        }
    }
}
