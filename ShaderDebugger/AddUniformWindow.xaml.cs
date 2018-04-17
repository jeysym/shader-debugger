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
using System.Windows.Shapes;

namespace ShaderDebugger
{
    /// <summary>
    /// Interaction logic for AddUniformWindow.xaml
    /// </summary>
    public partial class AddUniformWindow : Window
    {
        public bool Result { get; set; }
        public Uniform NewUniform { get; set; }

        private ICollection<GLType> supportedTypes;

        public AddUniformWindow()
        {
            InitializeComponent();

            Result = false;

            supportedTypes = UniformMaker.GetSupportedTypes();
            typeComboBox.ItemsSource = supportedTypes;
            typeComboBox.SelectedIndex = 0;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;
            GLType type = (GLType)typeComboBox.SelectedItem;

            NewUniform = UniformMaker.Make(type, name);
            Result = true;
            Close();
        }
    }
}
