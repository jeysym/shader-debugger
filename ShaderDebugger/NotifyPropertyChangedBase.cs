using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ShaderDebugger
{
    /// <summary>
    /// Abstract class that serves as a base for all classes that would like to implement INotifyPropertyChanged.
    /// </summary>
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
