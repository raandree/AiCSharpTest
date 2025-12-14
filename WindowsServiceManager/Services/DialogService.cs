using System.Windows;

namespace WindowsServiceManager.Services;

/// <summary>
/// Implements dialog message display using WPF MessageBox.
/// </summary>
public class DialogService : IDialogService
{
    /// <inheritdoc/>
    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }

    /// <inheritdoc/>
    public void ShowSuccess(string message, string title = "Success")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    /// <inheritdoc/>
    public void ShowWarning(string message, string title = "Warning")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Warning
        );
    }

    /// <inheritdoc/>
    public void ShowInformation(string message, string title = "Information")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    /// <inheritdoc/>
    public bool ShowConfirmation(string message, string title = "Confirm")
    {
        MessageBoxResult result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        return result == MessageBoxResult.Yes;
    }

    /// <inheritdoc/>
    public bool ShowConfirmation(string message, string title, string yesButtonText, string noButtonText)
    {
        // Note: WPF MessageBox doesn't support custom button text directly
        // For now, using standard Yes/No buttons
        // In a production app, you might create a custom dialog window
        MessageBoxResult result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        return result == MessageBoxResult.Yes;
    }
}
