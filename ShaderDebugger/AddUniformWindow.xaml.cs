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
    /// This window is used for creation of new uniforms. It contains interaction logic for 
    /// AddUniformWindow.xaml
    /// </summary>
    public partial class AddUniformWindow : Window, INotifyPropertyChanged
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

        private string _NewUniformName;
        private bool _IsNewUniformNameValid;


        // ==================================================================================================
        // PROPERTIES
        // ==================================================================================================

        public OpenGLIdentifierValidationRule NameValidationRule { get; private set; }

        /// <summary>
        /// This property stores the new Uniform instance created by this window. If this property is
        /// null it means this window was canceled.
        /// </summary>
        public Uniform NewUniform { get; set; }

        /// <summary>
        /// Name of the uniform being created.
        /// </summary>
        public string NewUniformName {
            get { return _NewUniformName; }
            set {
                _NewUniformName = value;
                IsNewUniformNameValid = NameValidationRule.Validate(_NewUniformName, System.Globalization.CultureInfo.CurrentUICulture).IsValid;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Tells whether the new name is valid name for OpenGL uniform.
        /// </summary>
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
