using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ShaderDebugger
{
    /// <summary>
    /// Interaction logic for AddUniformWindow.xaml
    /// </summary>
    public partial class AddAttributeWindow : Window, INotifyPropertyChanged
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

        private string _NewAttributeName;
        private bool _IsNewAttributeNameValid;


        // ==================================================================================================
        // PROPERTIES
        // ==================================================================================================

        public OpenGLIdentifierValidationRule NameValidationRule { get; private set; }
        public AttributeInfo NewAttribute { get; set; }

        public string NewAttributeName
        {
            get { return _NewAttributeName; }
            set
            {
                _NewAttributeName = value;
                IsNewAttributeNameValid = NameValidationRule.Validate(_NewAttributeName, System.Globalization.CultureInfo.CurrentUICulture).IsValid;
                NotifyPropertyChanged();
            }
        }

        public bool IsNewAttributeNameValid
        {
            get { return _IsNewAttributeNameValid; }
            set { _IsNewAttributeNameValid = value; NotifyPropertyChanged(); }
        }


        // ==================================================================================================
        // OTHER
        // ==================================================================================================

        public AddAttributeWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            NewAttribute = null;
            NameValidationRule = new OpenGLIdentifierValidationRule();

            ICollection<GLType> supportedTypes = AttributeMaker.GetSupportedTypes();
            typeComboBox.ItemsSource = supportedTypes;
            typeComboBox.SelectedIndex = 0;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NewAttributeName;
            GLType type = (GLType)typeComboBox.SelectedItem;

            NewAttribute = AttributeMaker.Make(type, name);
            Close();
        }
    }
}
