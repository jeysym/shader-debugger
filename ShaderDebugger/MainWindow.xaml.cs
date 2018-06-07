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

            Uniform k = new Uniform("k", new Float() { Value = 4.2f });
            Uniform d = new Uniform("d", new Vec3f { X = 1.0f, Y = 0.0f, Z = 1.0f });

            core.AddUniform(k);
            core.AddUniform(d);

            core.AddNewVertex();
            core.AddNewVertex();

            AttributeInfo attInfo = new AttributeInfo();
            attInfo.Name = "Vektor ctyri";
            attInfo.Location = 42;
            attInfo.Type = GLType.VEC3;
            core.AddNewAttribute(attInfo);

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = attInfo.Name;
            nameColumn.Binding = new Binding($"Attributes[{attInfo.Id}]");
            verticesDataGrid.Columns.Add(nameColumn);


            //Vertex vert = new Vertex();
            //vert.Attributes.Add(attInfo.Id, attInfo.CreateNewVariable());

            //core.Vertices.Add(vert);
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

            bool manualRenderSize = (renderSizeManualCheckBox.IsChecked != null) ? renderSizeManualCheckBox.IsChecked.Value : false;
            int width, height;
            if (manualRenderSize)
            {
                if (int.TryParse(renderWidthTextBox.Text, out width) == false || width <= 0)
                {
                    width = (int)openGLControl.RenderSize.Width;
                }

                if (int.TryParse(renderHeightTextBox.Text, out height) == false || height <= 0)
                {
                    height = (int)openGLControl.RenderSize.Height;
                }
            }
            else
            {
                width = (int)openGLControl.RenderSize.Width;
                height = (int)openGLControl.RenderSize.Height;
            }

            renderWidthTextBox.Text = width.ToString();
            renderWidthTextBox.InvalidateMeasure();
            renderHeightTextBox.Text = height.ToString();
            renderHeightTextBox.InvalidateMeasure();

            core.Render(width, height);
        }

        private void deleteUniformButton_Click(object sender, RoutedEventArgs e)
        {
            var core = GetCore();

            while (uniformsDataGrid.SelectedItems.Count > 0)
            {
                int selectedIndex = uniformsDataGrid.SelectedIndex;
                if (selectedIndex != -1)
                {
                    core.Uniforms.RemoveAt(selectedIndex);
                }
            }
        }

        private void newUniformButton_Click(object sender, RoutedEventArgs e)
        {
            var core = GetCore();

            AddUniformWindow window = new AddUniformWindow();
            window.ShowDialog();

            if (window.Result)
            {
                core.AddUniform(window.NewUniform);
            }
        }

        private void renderSizeManualCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            renderWidthTextBox.IsEnabled = true;
            renderHeightTextBox.IsEnabled = true;
        }

        private void renderSizeManualCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            renderWidthTextBox.IsEnabled = false;
            renderHeightTextBox.IsEnabled = false;
        }

        private void deleteVerticesButton_Click(object sender, RoutedEventArgs e)
        {
            var core = GetCore();

            while (verticesDataGrid.SelectedItems.Count > 0)
            {
                int selectedIndex = verticesDataGrid.SelectedIndex;
                if (selectedIndex != -1)
                {
                    core.Vertices.RemoveAt(selectedIndex);
                }
            }
        }

        private void newVertexButton_Click(object sender, RoutedEventArgs e)
        {
            var core = GetCore();
            core.AddNewVertex();
        }

        private void newAttributeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
