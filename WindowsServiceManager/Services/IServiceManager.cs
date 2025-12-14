using WindowsServiceManager.Models;

namespace WindowsServiceManager.Services;

/// <summary>
/// Defines the contract for high-level service management operations.
/// Orchestrates repository calls, permission checks, and user confirmations.
/// </summary>
public interface IServiceManager
{
    /// <summary>
    /// Loads all Windows services from the system.
    /// </summary>
    /// <returns>Collection of all services.</returns>
    Task<IEnumerable<WindowsServiceModel>> LoadServicesAsync();

    /// <summary>
    /// Refreshes a specific service's state.
    /// </summary>
    /// <param name="serviceName">The service to refresh.</param>
    /// <returns>The updated service model, or null if not found.</returns>
    Task<WindowsServiceModel?> RefreshServiceAsync(string serviceName);

    /// <summary>
    /// Starts a service with permission checks and user confirmation for critical services.
    /// </summary>
    /// <param name="service">The service to start.</param>
    /// <returns>Operation result.</returns>
    Task<ServiceOperationResult> StartServiceAsync(WindowsServiceModel service);

    /// <summary>
    /// Stops a service with permission checks and user confirmation for critical services.
    /// </summary>
    /// <param name="service">The service to stop.</param>
    /// <returns>Operation result.</returns>
    Task<ServiceOperationResult> StopServiceAsync(WindowsServiceModel service);

    /// <summary>
    /// Restarts a service with permission checks and user confirmation for critical services.
    /// </summary>
    /// <param name="service">The service to restart.</param>
    /// <returns>Operation result.</returns>
    Task<ServiceOperationResult> RestartServiceAsync(WindowsServiceModel service);

    /// <summary>
    /// Pauses a service with permission checks.
    /// </summary>
    /// <param name="service">The service to pause.</param>
    /// <returns>Operation result.</returns>
    Task<ServiceOperationResult> PauseServiceAsync(WindowsServiceModel service);

    /// <summary>
    /// Resumes a paused service with permission checks.
    /// </summary>
    /// <param name="service">The service to resume.</param>
    /// <returns>Operation result.</returns>
    Task<ServiceOperationResult> ResumeServiceAsync(WindowsServiceModel service);

    /// <summary>
    /// Changes a service's startup type with permission checks and user confirmation for critical services.
    /// </summary>
    /// <param name="service">The service to configure.</param>
    /// <param name="newStartupType">The new startup type (Automatic, Manual, Disabled).</param>
    /// <returns>Operation result.</returns>
    Task<ServiceOperationResult> ChangeStartupTypeAsync(WindowsServiceModel service, string newStartupType);

    /// <summary>
    /// Checks if the application is running with administrator privileges.
    /// </summary>
    /// <returns>True if administrator; otherwise, false.</returns>
    bool IsRunningAsAdministrator();
}
