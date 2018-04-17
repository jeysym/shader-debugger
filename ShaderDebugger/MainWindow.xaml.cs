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

            FloatUniform k = new FloatUniform("k");
            k.Value = new Float { Value = 0.5f };

            Vec3Uniform v = new Vec3Uniform("v");
            v.Value = new Vec3f { X = 1.0f, Y = 0.0f, Z = 1.0f };

            core.Uniforms.Add(k);
            core.Uniforms.Add(v);
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

            int width = (int)openGLControl.RenderSize.Width;
            int height = (int)openGLControl.RenderSize.Height;

            core.Render(width, height);
        }

        private void deleteUniformButton_Click(object sender, RoutedEventArgs e)
        {
            var core = GetCore();

            int selectedIndex = uniformsDataGrid.SelectedIndex;
            if (selectedIndex != -1)
            {
                core.Uniforms.RemoveAt(selectedIndex);
            }
        }

        private void newUniformButton_Click(object sender, RoutedEventArgs e)
        {
            var core = GetCore();

            AddUniformWindow window = new AddUniformWindow();
            window.ShowDialog();

            if (window.Result)
            {
                core.Uniforms.Add(window.NewUniform);
            }
        }
    }
}
