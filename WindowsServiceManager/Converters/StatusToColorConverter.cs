using System.Globalization;
using System.ServiceProcess;
using System.Windows.Data;
using System.Windows.Media;

namespace WindowsServiceManager.Converters;

/// <summary>
/// Converts ServiceControllerStatus to a color brush for visual indication.
/// </summary>
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ServiceControllerStatus status)
        {
            return status switch
            {
                ServiceControllerStatus.Running => new SolidColorBrush(Colors.Green),
                ServiceControllerStatus.Stopped => new SolidColorBrush(Colors.Red),
                ServiceControllerStatus.Paused => new SolidColorBrush(Colors.Orange),
                ServiceControllerStatus.StartPending => new SolidColorBrush(Colors.Yellow),
                ServiceControllerStatus.StopPending => new SolidColorBrush(Colors.Yellow),
                ServiceControllerStatus.ContinuePending => new SolidColorBrush(Colors.Yellow),
                ServiceControllerStatus.PausePending => new SolidColorBrush(Colors.Yellow),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
