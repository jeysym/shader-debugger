using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class AddUniformWindow : Window, INotifyPropertyChanged
    {
        // ==================================================================================================
        // INOTIFYPROPERTYCHANGED STUFF
        // ==================================================================================================

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        // ==================================================================================================
        // PROPERTY PRIVATE FIELDS
        // ==================================================================================================

        private string _NewUniformName;
        private bool _IsNewUniformNameValid;


        // ==================================================================================================
        // PROPERTIES
        // ==================================================================================================

        public OpenGLIdentifierValidationRule NameValidationRule { get; private set; }
        public Uniform NewUniform { get; set; }

        public string NewUniformName {
            get { return _NewUniformName; }
            set {
                _NewUniformName = value;
                IsNewUniformNameValid = NameValidationRule.Validate(_NewUniformName, System.Globalization.CultureInfo.CurrentUICulture).IsValid;
                NotifyPropertyChanged();
            }
        }

        public bool IsNewUniformNameValid {
            get { return _IsNewUniformNameValid; }
            set { _IsNewUniformNameValid = value; NotifyPropertyChanged(); }
        }


        // ==================================================================================================
        // OTHER
        // ==================================================================================================

        public AddUniformWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            NewUniform = null;
            NameValidationRule = new OpenGLIdentifierValidationRule();

            ICollection<GLType> supportedTypes = UniformMaker.GetSupportedTypes();
            typeComboBox.ItemsSource = supportedTypes;
            typeComboBox.SelectedIndex = 0;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NewUniformName;
            GLType type = (GLType)typeComboBox.SelectedItem;

            NewUniform = UniformMaker.Make(type, name);
            Close();
        }
    }
}
