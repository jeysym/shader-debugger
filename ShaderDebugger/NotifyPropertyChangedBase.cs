using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ShaderDebugger
{
    /// <summary>
    /// Abstract class that serves as a base for all classes that would like to implement 
    /// INotifyPropertyChanged. It makes implementing this interface easier.
    /// </summary>
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Derived classes should call this event when their properties are changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Call this method from derived class when a property was changed.
        /// </summary>
        /// <param name="propertyName">Name of a property.</param>
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
