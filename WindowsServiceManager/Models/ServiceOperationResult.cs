namespace WindowsServiceManager.Models;

/// <summary>
/// Represents the result of a service operation (start, stop, restart, etc.).
/// Implements the Result pattern for consistent error handling.
/// </summary>
public class ServiceOperationResult
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Success { get; private init; }

    /// <summary>
    /// Gets the message describing the result (success message or error details).
    /// </summary>
    public string Message { get; private init; } = string.Empty;

    /// <summary>
    /// Gets the exception that occurred, if any.
    /// </summary>
    public Exception? Exception { get; private init; }

    /// <summary>
    /// Private constructor to enforce use of factory methods.
    /// </summary>
    private ServiceOperationResult()
    {
    }

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <param name="message">Optional success message. Defaults to generic success message.</param>
    /// <returns>A ServiceOperationResult indicating success.</returns>
    public static ServiceOperationResult SuccessResult(string? message = null)
    {
        return new ServiceOperationResult
        {
            Success = true,
            Message = message ?? "Operation completed successfully"
        };
    }

    /// <summary>
    /// Creates a failed operation result.
    /// </summary>
    /// <param name="message">Error message describing what went wrong.</param>
    /// <param name="exception">Optional exception that caused the failure.</param>
    /// <returns>A ServiceOperationResult indicating failure.</returns>
    public static ServiceOperationResult FailureResult(string message, Exception? exception = null)
    {
        return new ServiceOperationResult
        {
            Success = false,
            Message = message,
            Exception = exception
        };
    }

    /// <summary>
    /// Creates a failed operation result from an exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <param name="customMessage">Optional custom message. If null, uses exception message.</param>
    /// <returns>A ServiceOperationResult indicating failure.</returns>
    public static ServiceOperationResult FromException(Exception exception, string? customMessage = null)
    {
        return new ServiceOperationResult
        {
            Success = false,
            Message = customMessage ?? exception.Message,
            Exception = exception
        };
    }
}
