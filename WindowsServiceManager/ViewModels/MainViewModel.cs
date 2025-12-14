using System.Collections.ObjectModel;
using System.ServiceProcess;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using WindowsServiceManager.Commands;
using WindowsServiceManager.Models;
using WindowsServiceManager.Services;

namespace WindowsServiceManager.ViewModels;

/// <summary>
/// Main ViewModel for the Windows Service Manager application.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<MainViewModel> _logger;
    
    private ObservableCollection<WindowsServiceModel> _services;
    private ObservableCollection<WindowsServiceModel> _filteredServices;
    private WindowsServiceModel? _selectedService;
    private string _searchText;
    private bool _isLoading;
    private string _statusMessage;

    public MainViewModel(IServiceManager serviceManager, ILogger<MainViewModel> logger)
    {
        _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _services = new ObservableCollection<WindowsServiceModel>();
        _filteredServices = new ObservableCollection<WindowsServiceModel>();
        _searchText = string.Empty;
        _statusMessage = "Ready";

        // Initialize commands
        RefreshCommand = new AsyncRelayCommand(LoadServicesAsync);
        StartServiceCommand = new AsyncRelayCommand(StartServiceAsync, () => CanExecuteServiceCommand(ServiceControllerStatus.Stopped));
        StopServiceCommand = new AsyncRelayCommand(StopServiceAsync, () => CanExecuteServiceCommand(ServiceControllerStatus.Running));
        RestartServiceCommand = new AsyncRelayCommand(RestartServiceAsync, () => CanExecuteServiceCommand(ServiceControllerStatus.Running));
        PauseServiceCommand = new AsyncRelayCommand(PauseServiceAsync, () => CanPauseService());
        ResumeServiceCommand = new AsyncRelayCommand(ResumeServiceAsync, () => CanResumeService());
    }

    /// <summary>
    /// Initialize the ViewModel. Call this after the ViewModel is created.
    /// </summary>
    public async Task InitializeAsync()
    {
        await LoadServicesAsync();
    }

    public ObservableCollection<WindowsServiceModel> FilteredServices
    {
        get => _filteredServices;
        set => SetProperty(ref _filteredServices, value);
    }

    public WindowsServiceModel? SelectedService
    {
        get => _selectedService;
        set
        {
            if (SetProperty(ref _selectedService, value))
            {
                RaiseCommandsCanExecuteChanged();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsAdministrator => _serviceManager.IsRunningAsAdministrator();

    public ICommand RefreshCommand { get; }
    public ICommand StartServiceCommand { get; }
    public ICommand StopServiceCommand { get; }
    public ICommand RestartServiceCommand { get; }
    public ICommand PauseServiceCommand { get; }
    public ICommand ResumeServiceCommand { get; }

    private async Task LoadServicesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading services...";
            _logger.LogInformation("Loading services");

            IEnumerable<WindowsServiceModel> services = await _serviceManager.LoadServicesAsync();
            
            _services.Clear();
            foreach (WindowsServiceModel service in services)
            {
                _services.Add(service);
            }

            ApplyFilter();
            StatusMessage = $"Loaded {_services.Count} services";
            _logger.LogInformation("Loaded {Count} services", _services.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load services");
            StatusMessage = "Failed to load services";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task StartServiceAsync()
    {
        if (SelectedService == null) return;

        try
        {
            StatusMessage = $"Starting {SelectedService.DisplayName}...";
            ServiceOperationResult result = await _serviceManager.StartServiceAsync(SelectedService);
            
            if (result.Success)
            {
                await RefreshSelectedServiceAsync();
            }

            StatusMessage = result.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start service");
            StatusMessage = "Failed to start service";
        }
    }

    private async Task StopServiceAsync()
    {
        if (SelectedService == null) return;

        try
        {
            StatusMessage = $"Stopping {SelectedService.DisplayName}...";
            ServiceOperationResult result = await _serviceManager.StopServiceAsync(SelectedService);
            
            if (result.Success)
            {
                await RefreshSelectedServiceAsync();
            }

            StatusMessage = result.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop service");
            StatusMessage = "Failed to stop service";
        }
    }

    private async Task RestartServiceAsync()
    {
        if (SelectedService == null) return;

        try
        {
            StatusMessage = $"Restarting {SelectedService.DisplayName}...";
            ServiceOperationResult result = await _serviceManager.RestartServiceAsync(SelectedService);
            
            if (result.Success)
            {
                await RefreshSelectedServiceAsync();
            }

            StatusMessage = result.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restart service");
            StatusMessage = "Failed to restart service";
        }
    }

    private async Task PauseServiceAsync()
    {
        if (SelectedService == null) return;

        try
        {
            StatusMessage = $"Pausing {SelectedService.DisplayName}...";
            ServiceOperationResult result = await _serviceManager.PauseServiceAsync(SelectedService);
            
            if (result.Success)
            {
                await RefreshSelectedServiceAsync();
            }

            StatusMessage = result.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pause service");
            StatusMessage = "Failed to pause service";
        }
    }

    private async Task ResumeServiceAsync()
    {
        if (SelectedService == null) return;

        try
        {
            StatusMessage = $"Resuming {SelectedService.DisplayName}...";
            ServiceOperationResult result = await _serviceManager.ResumeServiceAsync(SelectedService);
            
            if (result.Success)
            {
                await RefreshSelectedServiceAsync();
            }

            StatusMessage = result.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume service");
            StatusMessage = "Failed to resume service";
        }
    }

    public async Task ChangeStartupTypeAsync(string newStartupType)
    {
        if (SelectedService == null || string.IsNullOrWhiteSpace(newStartupType)) return;

        try
        {
            StatusMessage = $"Changing startup type to {newStartupType}...";
            ServiceOperationResult result = await _serviceManager.ChangeStartupTypeAsync(SelectedService, newStartupType);
            
            if (result.Success)
            {
                await RefreshSelectedServiceAsync();
            }

            StatusMessage = result.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change startup type");
            StatusMessage = "Failed to change startup type";
        }
    }

    private async Task RefreshSelectedServiceAsync()
    {
        if (SelectedService == null) return;

        WindowsServiceModel? updated = await _serviceManager.RefreshServiceAsync(SelectedService.ServiceName);
        if (updated != null)
        {
            // Update the service in the collection
            int index = _services.IndexOf(SelectedService);
            if (index >= 0)
            {
                _services[index] = updated;
                SelectedService = updated;
                ApplyFilter();
            }
        }
    }

    private void ApplyFilter()
    {
        FilteredServices.Clear();

        IEnumerable<WindowsServiceModel> filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _services
            : _services.Where(s =>
                s.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                s.ServiceName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (s.Description != null && s.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

        foreach (WindowsServiceModel service in filtered)
        {
            FilteredServices.Add(service);
        }
    }

    private bool CanExecuteServiceCommand(ServiceControllerStatus requiredStatus)
    {
        return SelectedService != null && SelectedService.Status == requiredStatus;
    }

    private bool CanPauseService()
    {
        return SelectedService != null &&
               SelectedService.Status == ServiceControllerStatus.Running &&
               SelectedService.CanPauseAndContinue;
    }

    private bool CanResumeService()
    {
        return SelectedService != null && SelectedService.Status == ServiceControllerStatus.Paused;
    }

    private void RaiseCommandsCanExecuteChanged()
    {
        (StartServiceCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (StopServiceCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (RestartServiceCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (PauseServiceCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ResumeServiceCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
    }
}
