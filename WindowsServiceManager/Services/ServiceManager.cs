using Microsoft.Extensions.Logging;
using WindowsServiceManager.Helpers;
using WindowsServiceManager.Models;

namespace WindowsServiceManager.Services;

/// <summary>
/// Implements high-level service management operations with permission checks and user confirmations.
/// </summary>
public class ServiceManager : IServiceManager
{
    private readonly IServiceRepository _repository;
    private readonly IDialogService _dialogService;
    private readonly ILogger<ServiceManager> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceManager"/> class.
    /// </summary>
    public ServiceManager(
        IServiceRepository repository,
        IDialogService dialogService,
        ILogger<ServiceManager> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<WindowsServiceModel>> LoadServicesAsync()
    {
        _logger.LogInformation("Loading all Windows services");
        return await _repository.GetAllServicesAsync();
    }

    /// <inheritdoc/>
    public async Task<WindowsServiceModel?> RefreshServiceAsync(string serviceName)
    {
        _logger.LogInformation("Refreshing service {ServiceName}", serviceName);
        return await _repository.RefreshServiceAsync(serviceName);
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> StartServiceAsync(WindowsServiceModel service)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (!CheckAdministratorPrivileges())
        {
            return ServiceOperationResult.FailureResult("Administrator privileges required to start services");
        }

        if (service.IsSystemCritical)
        {
            bool confirmed = _dialogService.ShowConfirmation(
                $"'{service.DisplayName}' is a system-critical service.\n\n" +
                "Starting this service may affect system stability.\n\n" +
                "Are you sure you want to continue?",
                "Confirm Start Critical Service"
            );

            if (!confirmed)
            {
                return ServiceOperationResult.FailureResult("Operation cancelled by user");
            }
        }

        ServiceOperationResult result = await _repository.StartServiceAsync(service.ServiceName);
        
        if (result.Success)
        {
            _dialogService.ShowSuccess(result.Message);
        }
        else
        {
            _dialogService.ShowError(result.Message);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> StopServiceAsync(WindowsServiceModel service)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (!CheckAdministratorPrivileges())
        {
            return ServiceOperationResult.FailureResult("Administrator privileges required to stop services");
        }

        if (service.IsSystemCritical)
        {
            bool confirmed = _dialogService.ShowConfirmation(
                $"'{service.DisplayName}' is a system-critical service.\n\n" +
                "Stopping this service may cause system instability or loss of functionality.\n\n" +
                "Are you sure you want to stop this service?",
                "Confirm Stop Critical Service"
            );

            if (!confirmed)
            {
                return ServiceOperationResult.FailureResult("Operation cancelled by user");
            }
        }

        ServiceOperationResult result = await _repository.StopServiceAsync(service.ServiceName);
        
        if (result.Success)
        {
            _dialogService.ShowSuccess(result.Message);
        }
        else
        {
            _dialogService.ShowError(result.Message);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> RestartServiceAsync(WindowsServiceModel service)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (!CheckAdministratorPrivileges())
        {
            return ServiceOperationResult.FailureResult("Administrator privileges required to restart services");
        }

        if (service.IsSystemCritical)
        {
            bool confirmed = _dialogService.ShowConfirmation(
                $"'{service.DisplayName}' is a system-critical service.\n\n" +
                "Restarting this service may temporarily affect system functionality.\n\n" +
                "Are you sure you want to restart this service?",
                "Confirm Restart Critical Service"
            );

            if (!confirmed)
            {
                return ServiceOperationResult.FailureResult("Operation cancelled by user");
            }
        }

        ServiceOperationResult result = await _repository.RestartServiceAsync(service.ServiceName);
        
        if (result.Success)
        {
            _dialogService.ShowSuccess(result.Message);
        }
        else
        {
            _dialogService.ShowError(result.Message);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> PauseServiceAsync(WindowsServiceModel service)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (!CheckAdministratorPrivileges())
        {
            return ServiceOperationResult.FailureResult("Administrator privileges required to pause services");
        }

        if (!service.CanPauseAndContinue)
        {
            return ServiceOperationResult.FailureResult($"Service '{service.DisplayName}' does not support pause/resume operations");
        }

        ServiceOperationResult result = await _repository.PauseServiceAsync(service.ServiceName);
        
        if (result.Success)
        {
            _dialogService.ShowSuccess(result.Message);
        }
        else
        {
            _dialogService.ShowError(result.Message);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> ResumeServiceAsync(WindowsServiceModel service)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (!CheckAdministratorPrivileges())
        {
            return ServiceOperationResult.FailureResult("Administrator privileges required to resume services");
        }

        ServiceOperationResult result = await _repository.ResumeServiceAsync(service.ServiceName);
        
        if (result.Success)
        {
            _dialogService.ShowSuccess(result.Message);
        }
        else
        {
            _dialogService.ShowError(result.Message);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<ServiceOperationResult> ChangeStartupTypeAsync(WindowsServiceModel service, string newStartupType)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (string.IsNullOrWhiteSpace(newStartupType))
        {
            return ServiceOperationResult.FailureResult("Startup type cannot be empty");
        }

        if (!CheckAdministratorPrivileges())
        {
            return ServiceOperationResult.FailureResult("Administrator privileges required to change service configuration");
        }

        if (service.IsSystemCritical && newStartupType.Equals("Disabled", StringComparison.OrdinalIgnoreCase))
        {
            bool confirmed = _dialogService.ShowConfirmation(
                $"'{service.DisplayName}' is a system-critical service.\n\n" +
                "Disabling this service may prevent Windows from starting properly or cause system instability.\n\n" +
                "Are you absolutely sure you want to disable this service?",
                "Confirm Disable Critical Service"
            );

            if (!confirmed)
            {
                return ServiceOperationResult.FailureResult("Operation cancelled by user");
            }
        }

        ServiceOperationResult result = await _repository.ChangeStartupTypeAsync(service.ServiceName, newStartupType);
        
        if (result.Success)
        {
            _dialogService.ShowSuccess(result.Message);
        }
        else
        {
            _dialogService.ShowError(result.Message);
        }

        return result;
    }

    /// <inheritdoc/>
    public bool IsRunningAsAdministrator()
    {
        return SecurityHelper.IsAdministrator();
    }

    /// <summary>
    /// Checks if the application is running with administrator privileges and shows error if not.
    /// </summary>
    private bool CheckAdministratorPrivileges()
    {
        if (!SecurityHelper.IsAdministrator())
        {
            _logger.LogWarning("Operation attempted without administrator privileges");
            _dialogService.ShowError(
                "This operation requires administrator privileges.\n\n" +
                "Please restart the application as administrator.",
                "Administrator Privileges Required"
            );
            return false;
        }

        return true;
    }
}
