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
using System.Windows.Markup;

namespace ShaderDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Cached core class
        private Core core;
        private bool renderManualDimensions;

        private string GetDefVShaderCode()
        {
            return @"#version 330 core
layout (location = 0) in vec3 position;
  
out vec4 vertexColor; // specify a color output to the fragment shader

void main()
{
    gl_Position = vec4(position, 1.0); 
    vertexColor = vec4(0.5, 0.0, 0.0, 1.0); // set the output variable to a dark-red color
}
";
        }

        private string GetDefFShaderCode()
        {
            return @"#version 330 core
out vec4 FragColor;

uniform float k;
  
in vec4 vertexColor; // the input variable from the vertex shader (same name and same type)  

void main()
{
    FragColor = k * vertexColor;
}
";
        }

        private Core GetCore()
        {
            return (Core)this.Resources["core"];
        }

        private void InitUniforms()
        {
            Uniform k = new Uniform("k", new Float() { Value = 1.0f });

            core.Uniforms.Add(k);
        }

        private void InitVertices()
        {
            
        }

        public MainWindow()
        {
            InitializeComponent();

            core = GetCore();
            renderManualDimensions = 
                (renderSizeManualCheckBox.IsChecked != null) && (renderSizeManualCheckBox.IsChecked.Value);

            // There is a two-way binding between shader strings an TextBoxes so it is sufficient to change
            // only the source strings.
            core.VertexShaderCode = GetDefVShaderCode();
            core.FragmentShaderCode = GetDefFShaderCode();

            InitUniforms();
            InitVertices();
        }


        // ==================================================================================================
        // HANDLING EVENTS
        // ==================================================================================================

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;

            core.Init(gl);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;

            int width, height;
            if (renderManualDimensions)
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
                core.Uniforms.Add(window.NewUniform);
            }
        }

        private void renderSizeManualCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            renderWidthTextBox.IsEnabled = true;
            renderHeightTextBox.IsEnabled = true;
            renderManualDimensions = true;
        }

        private void renderSizeManualCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            renderWidthTextBox.IsEnabled = false;
            renderHeightTextBox.IsEnabled = false;
            renderManualDimensions = false;
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
            core.Vertices.Add(new Vertex());
        }

        DataGridTemplateColumn MakeTemplateColumn(AttributeInfo attrInfo)
        {
            string xamlString = @"
                <DataGridTemplateColumn xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                    Header = """ + attrInfo.Name + @""">

                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <ContentPresenter 
                                Content = ""{ Binding Path = Attributes[" + attrInfo.Id + @"] }""
                            />
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>";

            object result = XamlReader.Parse(xamlString);
            DataGridTemplateColumn column = result as DataGridTemplateColumn;

            return column;       
        }

        private void newAttributeButton_Click(object sender, RoutedEventArgs e)
        {
            var core = GetCore();

            AddAttributeWindow window = new AddAttributeWindow();
            window.ShowDialog();

            if (window.Result)
            {
                AttributeInfo attributeInfo = window.NewAttribute;

                core.AddAttribute(attributeInfo);

                var column = MakeTemplateColumn(attributeInfo);
                verticesDataGrid.Columns.Add(column);
            }
        }
    }
}
