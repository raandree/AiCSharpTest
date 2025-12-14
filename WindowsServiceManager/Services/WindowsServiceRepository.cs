using System.Management;
using System.ServiceProcess;
using Microsoft.Extensions.Logging;
using WindowsServiceManager.Models;

namespace WindowsServiceManager.Services;

/// <summary>
/// Implements Windows service data access using System.ServiceProcess.ServiceController.
/// Provides async wrappers around the ServiceController API.
/// </summary>
public class WindowsServiceRepository : IServiceRepository
{
    private readonly ILogger<WindowsServiceRepository> _logger;
    private const int ServiceOperationTimeoutSeconds = 30;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsServiceRepository"/> class.
    /// </summary>
    /// <param name="logger">Logger for diagnostic information.</param>
    public WindowsServiceRepository(ILogger<WindowsServiceRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<WindowsServiceModel>> GetAllServicesAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Retrieving all Windows services");
                
                ServiceController[] services = ServiceController.GetServices();
                List<WindowsServiceModel> serviceModels = new List<WindowsServiceModel>();

                foreach (ServiceController service in services)
                {
                    try
                    {
                        WindowsServiceModel model = MapToServiceModel(service);
                        serviceModels.Add(model);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to map service {ServiceName}", service.ServiceName);
                    }
                    finally
                    {
                        service.Dispose();
                    }
                }

                _logger.LogInformation("Retrieved {Count} services", serviceModels.Count);
                return serviceModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve services");
                return Enumerable.Empty<WindowsServiceModel>();
            }
        });
    }

    /// <inheritdoc/>
    public async Task<WindowsServiceModel?> GetServiceAsync(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name cannot be null or empty", nameof(serviceName));
        }

        return await Task.Run(() =>
        {
            try
            {
                _logger.LogDebug("Retrieving service {ServiceName}", serviceName);
                
                using ServiceController service = new ServiceController(serviceName);
                service.Refresh();
                
                return MapToServiceModel(service);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Service {ServiceName} not found", serviceName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve service {ServiceName}", serviceName);
                return null;
            }
        });
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> StartServiceAsync(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return ServiceOperationResult.FailureResult("Service name cannot be empty");
        }

        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Starting service {ServiceName}", serviceName);
                
                using ServiceController service = new ServiceController(serviceName);
                
                if (service.Status == ServiceControllerStatus.Running)
                {
                    return ServiceOperationResult.SuccessResult($"Service '{serviceName}' is already running");
                }

                service.Start();
                service.WaitForStatus(
                    ServiceControllerStatus.Running, 
                    TimeSpan.FromSeconds(ServiceOperationTimeoutSeconds)
                );

                _logger.LogInformation("Service {ServiceName} started successfully", serviceName);
                return ServiceOperationResult.SuccessResult($"Service '{serviceName}' started successfully");
            }
            catch (System.TimeoutException ex)
            {
                string message = $"Service '{serviceName}' did not start within {ServiceOperationTimeoutSeconds} seconds";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (InvalidOperationException ex)
            {
                string message = $"Cannot start service '{serviceName}': {ex.Message}";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start service {ServiceName}", serviceName);
                return ServiceOperationResult.FromException(ex, $"Failed to start service '{serviceName}'");
            }
        });
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> StopServiceAsync(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return ServiceOperationResult.FailureResult("Service name cannot be empty");
        }

        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Stopping service {ServiceName}", serviceName);
                
                using ServiceController service = new ServiceController(serviceName);
                
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    return ServiceOperationResult.SuccessResult($"Service '{serviceName}' is already stopped");
                }

                if (!service.CanStop)
                {
                    return ServiceOperationResult.FailureResult($"Service '{serviceName}' cannot be stopped");
                }

                service.Stop();
                service.WaitForStatus(
                    ServiceControllerStatus.Stopped, 
                    TimeSpan.FromSeconds(ServiceOperationTimeoutSeconds)
                );

                _logger.LogInformation("Service {ServiceName} stopped successfully", serviceName);
                return ServiceOperationResult.SuccessResult($"Service '{serviceName}' stopped successfully");
            }
            catch (System.TimeoutException ex)
            {
                string message = $"Service '{serviceName}' did not stop within {ServiceOperationTimeoutSeconds} seconds";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (InvalidOperationException ex)
            {
                string message = $"Cannot stop service '{serviceName}': {ex.Message}";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop service {ServiceName}", serviceName);
                return ServiceOperationResult.FromException(ex, $"Failed to stop service '{serviceName}'");
            }
        });
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> RestartServiceAsync(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return ServiceOperationResult.FailureResult("Service name cannot be empty");
        }

        _logger.LogInformation("Restarting service {ServiceName}", serviceName);

        // Stop the service
        ServiceOperationResult stopResult = await StopServiceAsync(serviceName);
        if (!stopResult.Success)
        {
            return stopResult;
        }

        // Wait a moment for cleanup
        await Task.Delay(1000);

        // Start the service
        ServiceOperationResult startResult = await StartServiceAsync(serviceName);
        if (!startResult.Success)
        {
            return startResult;
        }

        return ServiceOperationResult.SuccessResult($"Service '{serviceName}' restarted successfully");
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> PauseServiceAsync(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return ServiceOperationResult.FailureResult("Service name cannot be empty");
        }

        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Pausing service {ServiceName}", serviceName);
                
                using ServiceController service = new ServiceController(serviceName);
                
                if (service.Status == ServiceControllerStatus.Paused)
                {
                    return ServiceOperationResult.SuccessResult($"Service '{serviceName}' is already paused");
                }

                if (!service.CanPauseAndContinue)
                {
                    return ServiceOperationResult.FailureResult($"Service '{serviceName}' does not support pause/continue");
                }

                service.Pause();
                service.WaitForStatus(
                    ServiceControllerStatus.Paused, 
                    TimeSpan.FromSeconds(ServiceOperationTimeoutSeconds)
                );

                _logger.LogInformation("Service {ServiceName} paused successfully", serviceName);
                return ServiceOperationResult.SuccessResult($"Service '{serviceName}' paused successfully");
            }
            catch (System.TimeoutException ex)
            {
                string message = $"Service '{serviceName}' did not pause within {ServiceOperationTimeoutSeconds} seconds";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (InvalidOperationException ex)
            {
                string message = $"Cannot pause service '{serviceName}': {ex.Message}";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pause service {ServiceName}", serviceName);
                return ServiceOperationResult.FromException(ex, $"Failed to pause service '{serviceName}'");
            }
        });
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> ResumeServiceAsync(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return ServiceOperationResult.FailureResult("Service name cannot be empty");
        }

        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Resuming service {ServiceName}", serviceName);
                
                using ServiceController service = new ServiceController(serviceName);
                
                if (service.Status != ServiceControllerStatus.Paused)
                {
                    return ServiceOperationResult.FailureResult($"Service '{serviceName}' is not paused");
                }

                service.Continue();
                service.WaitForStatus(
                    ServiceControllerStatus.Running, 
                    TimeSpan.FromSeconds(ServiceOperationTimeoutSeconds)
                );

                _logger.LogInformation("Service {ServiceName} resumed successfully", serviceName);
                return ServiceOperationResult.SuccessResult($"Service '{serviceName}' resumed successfully");
            }
            catch (System.TimeoutException ex)
            {
                string message = $"Service '{serviceName}' did not resume within {ServiceOperationTimeoutSeconds} seconds";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (InvalidOperationException ex)
            {
                string message = $"Cannot resume service '{serviceName}': {ex.Message}";
                _logger.LogError(ex, message);
                return ServiceOperationResult.FailureResult(message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resume service {ServiceName}", serviceName);
                return ServiceOperationResult.FromException(ex, $"Failed to resume service '{serviceName}'");
            }
        });
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> ChangeStartupTypeAsync(string serviceName, string startupType)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return ServiceOperationResult.FailureResult("Service name cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(startupType))
        {
            return ServiceOperationResult.FailureResult("Startup type cannot be empty");
        }

        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Changing startup type for service {ServiceName} to {StartupType}", serviceName, startupType);

                // Map friendly names to WMI values
                string wmiStartupType = startupType.ToLowerInvariant() switch
                {
                    "automatic" => "Automatic",
                    "manual" => "Manual",
                    "disabled" => "Disabled",
                    _ => throw new ArgumentException($"Invalid startup type: {startupType}")
                };

                // Use WMI to change startup type (ServiceController doesn't support this)
                string query = $"SELECT * FROM Win32_Service WHERE Name = '{serviceName}'";
                using ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                using ManagementObjectCollection collection = searcher.Get();

                if (collection.Count == 0)
                {
                    return ServiceOperationResult.FailureResult($"Service '{serviceName}' not found");
                }

                foreach (ManagementObject service in collection)
                {
                    try
                    {
                        object[] parameters = new object[] { wmiStartupType };
                        object result = service.InvokeMethod("ChangeStartMode", parameters);
                        
                        uint returnValue = Convert.ToUInt32(result);
                        if (returnValue == 0)
                        {
                            _logger.LogInformation("Startup type for service {ServiceName} changed to {StartupType}", serviceName, startupType);
                            return ServiceOperationResult.SuccessResult($"Startup type changed to '{startupType}' successfully");
                        }
                        else
                        {
                            string error = GetWmiErrorMessage(returnValue);
                            _logger.LogError("Failed to change startup type for service {ServiceName}: {Error}", serviceName, error);
                            return ServiceOperationResult.FailureResult($"Failed to change startup type: {error}");
                        }
                    }
                    finally
                    {
                        service.Dispose();
                    }
                }

                return ServiceOperationResult.FailureResult("Failed to change startup type");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change startup type for service {ServiceName}", serviceName);
                return ServiceOperationResult.FromException(ex, $"Failed to change startup type for service '{serviceName}'");
            }
        });
    }

    /// <inheritdoc/>
    public async Task<WindowsServiceModel?> RefreshServiceAsync(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Service name cannot be null or empty", nameof(serviceName));
        }

        return await GetServiceAsync(serviceName);
    }

    /// <summary>
    /// Maps a ServiceController to a WindowsServiceModel.
    /// </summary>
    private WindowsServiceModel MapToServiceModel(ServiceController service)
    {
        string description = string.Empty;
        string? executablePath = null;

        try
        {
            // Get additional service information via WMI
            string query = $"SELECT * FROM Win32_Service WHERE Name = '{service.ServiceName}'";
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            using ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject wmiService in collection)
            {
                try
                {
                    description = wmiService["Description"]?.ToString() ?? string.Empty;
                    executablePath = wmiService["PathName"]?.ToString();
                }
                finally
                {
                    wmiService.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get additional info for service {ServiceName}", service.ServiceName);
        }

        return new WindowsServiceModel
        {
            ServiceName = service.ServiceName,
            DisplayName = service.DisplayName,
            Description = description,
            Status = service.Status,
            StartupType = service.StartType,
            Dependencies = service.ServicesDependedOn.Select(s => s.ServiceName).ToArray(),
            CanStop = service.CanStop,
            CanPauseAndContinue = service.CanPauseAndContinue,
            ExecutablePath = executablePath,
            IsSystemCritical = IsSystemCriticalService(service.ServiceName)
        };
    }

    /// <summary>
    /// Determines if a service is system-critical and should show warnings before modification.
    /// </summary>
    private bool IsSystemCriticalService(string serviceName)
    {
        string[] criticalServices = new[]
        {
            "WinDefend",      // Windows Defender
            "BITS",           // Background Intelligent Transfer Service
            "Dhcp",           // DHCP Client
            "Dnscache",       // DNS Client
            "EventLog",       // Windows Event Log
            "RpcSs",          // Remote Procedure Call
            "LanmanServer",   // Server
            "LanmanWorkstation", // Workstation
            "PlugPlay",       // Plug and Play
            "SENS",           // System Event Notification Service
            "ShellHWDetection", // Shell Hardware Detection
            "Themes",         // Themes
            "W32Time",        // Windows Time
            "Winmgmt"         // Windows Management Instrumentation
        };

        return criticalServices.Contains(serviceName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Converts WMI error codes to user-friendly messages.
    /// </summary>
    private string GetWmiErrorMessage(uint returnValue)
    {
        return returnValue switch
        {
            0 => "Success",
            1 => "Not Supported",
            2 => "Access Denied",
            3 => "Dependent Services Running",
            4 => "Invalid Service Control",
            5 => "Service Cannot Accept Control",
            6 => "Service Not Active",
            7 => "Service Request Timeout",
            8 => "Unknown Failure",
            9 => "Path Not Found",
            10 => "Service Already Running",
            11 => "Service Database Locked",
            12 => "Service Dependency Deleted",
            13 => "Service Dependency Failure",
            14 => "Service Disabled",
            15 => "Service Logon Failure",
            16 => "Service Marked For Deletion",
            17 => "Service No Thread",
            18 => "Status Circular Dependency",
            19 => "Status Duplicate Name",
            20 => "Status Invalid Name",
            21 => "Status Invalid Parameter",
            22 => "Status Invalid Service Account",
            23 => "Status Service Exists",
            24 => "Service Already Paused",
            _ => $"Unknown error code: {returnValue}"
        };
    }
}
