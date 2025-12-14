using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsServiceManager.ViewModels;

/// <summary>
/// Base class for all ViewModels, providing INotifyPropertyChanged implementation.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed. Auto-filled by compiler.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the property value and raises PropertyChanged if the value has changed.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="field">Reference to the backing field.</param>
    /// <param name="value">New value to set.</param>
    /// <param name="propertyName">Name of the property. Auto-filled by compiler.</param>
    /// <returns>True if the value was changed; otherwise, false.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises PropertyChanged for multiple properties.
    /// Useful when one property change affects multiple computed properties.
    /// </summary>
    /// <param name="propertyNames">Names of the properties that changed.</param>
    protected void OnPropertiesChanged(params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            OnPropertyChanged(propertyName);
        }
    }
}
