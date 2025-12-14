namespace WindowsServiceManager.Services;

/// <summary>
/// Defines the contract for displaying dialog messages to the user.
/// Abstracts MessageBox functionality for testability.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an error message dialog.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Error".</param>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Shows a success message dialog.
    /// </summary>
    /// <param name="message">The success message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Success".</param>
    void ShowSuccess(string message, string title = "Success");

    /// <summary>
    /// Shows a warning message dialog.
    /// </summary>
    /// <param name="message">The warning message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Warning".</param>
    void ShowWarning(string message, string title = "Warning");

    /// <summary>
    /// Shows an informational message dialog.
    /// </summary>
    /// <param name="message">The informational message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Information".</param>
    void ShowInformation(string message, string title = "Information");

    /// <summary>
    /// Shows a confirmation dialog with Yes/No buttons.
    /// </summary>
    /// <param name="message">The confirmation message to display.</param>
    /// <param name="title">The dialog title. Defaults to "Confirm".</param>
    /// <returns>True if the user clicked Yes; otherwise, false.</returns>
    bool ShowConfirmation(string message, string title = "Confirm");

    /// <summary>
    /// Shows a confirmation dialog with custom button text.
    /// </summary>
    /// <param name="message">The confirmation message to display.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="yesButtonText">Text for the affirmative button.</param>
    /// <param name="noButtonText">Text for the negative button.</param>
    /// <returns>True if the user clicked the affirmative button; otherwise, false.</returns>
    bool ShowConfirmation(string message, string title, string yesButtonText, string noButtonText);
}
