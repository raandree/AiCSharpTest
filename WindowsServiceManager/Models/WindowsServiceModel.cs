using System.ServiceProcess;

namespace WindowsServiceManager.Models;

/// <summary>
/// Represents a Windows service with all its properties and state information.
/// </summary>
public class WindowsServiceModel
{
    /// <summary>
    /// Gets or sets the internal service name used by the system.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the friendly display name of the service.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of what the service does.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the service (Running, Stopped, etc.).
    /// </summary>
    public ServiceControllerStatus Status { get; set; }

    /// <summary>
    /// Gets or sets how the service starts (Automatic, Manual, Disabled).
    /// </summary>
    public ServiceStartMode StartupType { get; set; }

    /// <summary>
    /// Gets or sets the list of services this service depends on.
    /// </summary>
    public string[] Dependencies { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the service can be stopped.
    /// </summary>
    public bool CanStop { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the service can be paused and continued.
    /// </summary>
    public bool CanPauseAndContinue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a system-critical service.
    /// System-critical services should show warnings before modification.
    /// </summary>
    public bool IsSystemCritical { get; set; }

    /// <summary>
    /// Gets or sets the path to the service executable.
    /// </summary>
    public string? ExecutablePath { get; set; }

    /// <summary>
    /// Gets a user-friendly string representation of the service status.
    /// </summary>
    public string StatusText => Status switch
    {
        ServiceControllerStatus.Running => "Running",
        ServiceControllerStatus.Stopped => "Stopped",
        ServiceControllerStatus.Paused => "Paused",
        ServiceControllerStatus.StartPending => "Starting...",
        ServiceControllerStatus.StopPending => "Stopping...",
        ServiceControllerStatus.PausePending => "Pausing...",
        ServiceControllerStatus.ContinuePending => "Resuming...",
        _ => "Unknown"
    };

    /// <summary>
    /// Gets a user-friendly string representation of the startup type.
    /// </summary>
    public string StartupTypeText => StartupType switch
    {
        ServiceStartMode.Automatic => "Automatic",
        ServiceStartMode.Manual => "Manual",
        ServiceStartMode.Disabled => "Disabled",
        ServiceStartMode.Boot => "Boot",
        ServiceStartMode.System => "System",
        _ => "Unknown"
    };
}
