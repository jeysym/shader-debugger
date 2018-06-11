using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;


namespace ShaderDebugger
{
    /// <summary>
    /// This window is used for creation of new attributes. It contains interaction logic for 
    /// AddAttributeWindow.xaml
    /// </summary>
    public partial class AddAttributeWindow : Window, INotifyPropertyChanged
    {
        // ==================================================================================================
        // INOTIFYPROPERTYCHANGED STUFF
        // ==================================================================================================

        /// <summary>
        /// Event that is invoked when a property of this class is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
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

        private OpenGLIdentifierValidationRule NameValidationRule { get; set; }

        /// <summary>
        /// This property stores the new AttributeInfo instance created by this window. If this property is
        /// null it means this window was canceled.
        /// </summary>
        public AttributeInfo NewAttribute { get; set; }

        /// <summary>
        /// Name of the attribute being created.
        /// </summary>
        public string NewAttributeName
        {
            get { return _NewAttributeName; }
            set
            {
                _NewAttributeName = value;
                IsNewAttributeNameValid = 
                    NameValidationRule.Validate(_NewAttributeName, CultureInfo.CurrentUICulture).IsValid;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Tells whether the new name is valid name for OpenGL attribute.
        /// </summary>
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
