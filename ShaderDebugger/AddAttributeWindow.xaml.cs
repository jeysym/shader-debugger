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
    public partial class AddAttributeWindow : Window
    {
        public bool Result { get; set; }
        public AttributeInfo NewAttribute { get; set; }

        private ICollection<GLType> supportedTypes;

        public AddAttributeWindow()
        {
            InitializeComponent();

            Result = false;

            supportedTypes = AttributeMaker.GetSupportedTypes();
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

            NewAttribute = AttributeMaker.Make(type, name);
            Result = true;
            Close();
        }
    }
}
