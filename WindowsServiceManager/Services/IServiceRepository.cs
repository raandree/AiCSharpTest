using WindowsServiceManager.Models;

namespace WindowsServiceManager.Services;

/// <summary>
/// Defines the contract for Windows service data access operations.
/// Abstracts the System.ServiceProcess API for testability and maintainability.
/// </summary>
public interface IServiceRepository
{
    /// <summary>
    /// Retrieves all Windows services on the local machine.
    /// </summary>
    /// <returns>A collection of all services, or empty collection if none found.</returns>
    Task<IEnumerable<WindowsServiceModel>> GetAllServicesAsync();

    /// <summary>
    /// Retrieves a specific Windows service by its service name.
    /// </summary>
    /// <param name="serviceName">The internal service name (not display name).</param>
    /// <returns>The service model if found; otherwise, null.</returns>
    Task<WindowsServiceModel?> GetServiceAsync(string serviceName);

    /// <summary>
    /// Starts a stopped Windows service.
    /// </summary>
    /// <param name="serviceName">The internal service name to start.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    Task<ServiceOperationResult> StartServiceAsync(string serviceName);

    /// <summary>
    /// Stops a running Windows service.
    /// </summary>
    /// <param name="serviceName">The internal service name to stop.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    Task<ServiceOperationResult> StopServiceAsync(string serviceName);

    /// <summary>
    /// Restarts a Windows service (stops then starts).
    /// </summary>
    /// <param name="serviceName">The internal service name to restart.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    Task<ServiceOperationResult> RestartServiceAsync(string serviceName);

    /// <summary>
    /// Pauses a running Windows service that supports pausing.
    /// </summary>
    /// <param name="serviceName">The internal service name to pause.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    Task<ServiceOperationResult> PauseServiceAsync(string serviceName);

    /// <summary>
    /// Resumes a paused Windows service.
    /// </summary>
    /// <param name="serviceName">The internal service name to resume.</param>
    /// <returns>Result indicating success or failure with details.</returns>
    Task<ServiceOperationResult> ResumeServiceAsync(string serviceName);

    /// <summary>
    /// Changes the startup type of a Windows service.
    /// </summary>
    /// <param name="serviceName">The internal service name to configure.</param>
    /// <param name="startupType">The new startup type (Automatic, Manual, Disabled).</param>
    /// <returns>Result indicating success or failure with details.</returns>
    Task<ServiceOperationResult> ChangeStartupTypeAsync(string serviceName, string startupType);

    /// <summary>
    /// Refreshes the state of a specific service by requerying its current status.
    /// </summary>
    /// <param name="serviceName">The internal service name to refresh.</param>
    /// <returns>The updated service model if found; otherwise, null.</returns>
    Task<WindowsServiceModel?> RefreshServiceAsync(string serviceName);
}
