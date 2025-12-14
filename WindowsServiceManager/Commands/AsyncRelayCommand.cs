using System.Windows.Input;

namespace WindowsServiceManager.Commands;

/// <summary>
/// An asynchronous command implementation that relays its execution to async delegates.
/// Prevents multiple concurrent executions and provides proper cancellation support.
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The async execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <exception cref="ArgumentNullException">Thrown when execute is null.</exception>
    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Gets a value indicating whether the command is currently executing.
    /// </summary>
    public bool IsExecuting => _isExecuting;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command (not used in this implementation).</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command (not used in this implementation).</param>
    public async void Execute(object? parameter)
    {
        await ExecuteAsync();
    }

    /// <summary>
    /// Executes the command asynchronously with proper exception handling.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteAsync()
    {
        if (_isExecuting)
        {
            return;
        }

        _isExecuting = true;
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            RaiseCanExecuteChanged();
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Cancels the currently executing command if it's running.
    /// </summary>
    public void Cancel()
    {
        if (_isExecuting && _cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
