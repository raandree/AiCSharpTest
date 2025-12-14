# System Patterns: Windows Service Manager UI

## Architecture Overview

### High-Level Architecture
```
┌─────────────────────────────────────────────────────┐
│                  Presentation Layer                  │
│                    (WPF Views)                       │
├─────────────────────────────────────────────────────┤
│                  ViewModel Layer                     │
│              (MVVM Pattern, Commands)                │
├─────────────────────────────────────────────────────┤
│                   Service Layer                      │
│           (Business Logic, Orchestration)            │
├─────────────────────────────────────────────────────┤
│                   Data Access Layer                  │
│         (ServiceController Wrapper, Models)          │
├─────────────────────────────────────────────────────┤
│                Windows Service API                   │
│           (System.ServiceProcess.dll)                │
└─────────────────────────────────────────────────────┘
```

## MVVM Pattern Implementation

### Model Layer
**Responsibility**: Represent domain entities and data structures

```csharp
// Models/WindowsServiceModel.cs
public class WindowsServiceModel
{
    public string ServiceName { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public ServiceControllerStatus Status { get; set; }
    public ServiceStartMode StartupType { get; set; }
    public string[] Dependencies { get; set; }
    public bool CanStop { get; set; }
    public bool CanPauseAndContinue { get; set; }
    public bool IsSystemCritical { get; set; }
}
```

### View Layer
**Responsibility**: UI presentation and user interaction

```xaml
<!-- Views/MainWindow.xaml -->
- DataGrid for service list
- Details panel for selected service
- Command toolbar (Start, Stop, Restart buttons)
- Search/filter controls
- Status bar
```

**Key Principles**:
- No business logic in code-behind
- Data binding for all UI updates
- Commands for all user actions
- Converters for value transformations

### ViewModel Layer
**Responsibility**: Presentation logic, state management, commands

```csharp
// ViewModels/MainViewModel.cs
public class MainViewModel : ViewModelBase
{
    private IServiceManager _serviceManager;
    private ObservableCollection<WindowsServiceViewModel> _services;
    private WindowsServiceViewModel _selectedService;
    private string _searchText;
    
    public ICommand RefreshCommand { get; }
    public ICommand StartServiceCommand { get; }
    public ICommand StopServiceCommand { get; }
    public ICommand RestartServiceCommand { get; }
    public ICommand PauseServiceCommand { get; }
    public ICommand ResumeServiceCommand { get; }
    
    // Data binding properties
    public ObservableCollection<WindowsServiceViewModel> Services { get; }
    public WindowsServiceViewModel SelectedService { get; set; }
    public string SearchText { get; set; }
}
```

## Core Design Patterns

### 1. Repository Pattern
**Purpose**: Abstract data access layer from business logic

```csharp
// Services/IServiceRepository.cs
public interface IServiceRepository
{
    Task<IEnumerable<WindowsServiceModel>> GetAllServicesAsync();
    Task<WindowsServiceModel> GetServiceAsync(string serviceName);
    Task<ServiceOperationResult> StartServiceAsync(string serviceName);
    Task<ServiceOperationResult> StopServiceAsync(string serviceName);
    Task<ServiceOperationResult> RestartServiceAsync(string serviceName);
    Task<ServiceOperationResult> PauseServiceAsync(string serviceName);
    Task<ServiceOperationResult> ResumeServiceAsync(string serviceName);
    Task<ServiceOperationResult> ChangeStartupTypeAsync(string serviceName, ServiceStartMode startupType);
}

// Services/WindowsServiceRepository.cs
public class WindowsServiceRepository : IServiceRepository
{
    // Wraps System.ServiceProcess.ServiceController
    // Handles exceptions and permission checks
    // Converts ServiceController objects to domain models
}
```

### 2. Command Pattern (ICommand)
**Purpose**: Encapsulate UI actions as executable objects

```csharp
// Commands/RelayCommand.cs
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;
    
    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object parameter) => _execute(parameter);
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

// Commands/AsyncRelayCommand.cs
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;
    private bool _isExecuting;
    
    // Async execution with cancellation support
}
```

### 3. Observer Pattern (INotifyPropertyChanged)
**Purpose**: Enable automatic UI updates when data changes

```csharp
// ViewModels/ViewModelBase.cs
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
            
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

### 4. Dependency Injection
**Purpose**: Loose coupling, testability, maintainability

```csharp
// App.xaml.cs
public partial class App : Application
{
    private IServiceProvider _serviceProvider;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        // Register services
        services.AddSingleton<IServiceRepository, WindowsServiceRepository>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ILogger, FileLogger>();
        
        // Register ViewModels
        services.AddTransient<MainViewModel>();
        
        // Register Views
        services.AddTransient<MainWindow>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
```

### 5. Factory Pattern
**Purpose**: Create service operation results with consistent error handling

```csharp
// Services/ServiceOperationResult.cs
public class ServiceOperationResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public Exception Exception { get; private set; }
    
    private ServiceOperationResult() { }
    
    public static ServiceOperationResult SuccessResult(string message = null)
    {
        return new ServiceOperationResult 
        { 
            Success = true, 
            Message = message ?? "Operation completed successfully" 
        };
    }
    
    public static ServiceOperationResult FailureResult(string message, Exception ex = null)
    {
        return new ServiceOperationResult 
        { 
            Success = false, 
            Message = message, 
            Exception = ex 
        };
    }
}
```

## Component Architecture

### Service Manager Component
**Responsibility**: Orchestrate service operations with proper error handling

```csharp
// Services/ServiceManager.cs
public class ServiceManager : IServiceManager
{
    private readonly IServiceRepository _repository;
    private readonly ILogger _logger;
    private readonly IDialogService _dialogService;
    
    public async Task<bool> StartServiceAsync(string serviceName)
    {
        try
        {
            _logger.LogInfo($"Starting service: {serviceName}");
            
            // Check permissions
            if (!IsAdministrator())
            {
                _dialogService.ShowError("Administrator privileges required");
                return false;
            }
            
            // Check if system critical
            if (IsSystemCritical(serviceName))
            {
                var confirm = _dialogService.ShowConfirmation(
                    "This is a system-critical service. Continue?");
                if (!confirm) return false;
            }
            
            var result = await _repository.StartServiceAsync(serviceName);
            
            if (result.Success)
            {
                _logger.LogInfo($"Service started successfully: {serviceName}");
                _dialogService.ShowSuccess("Service started");
            }
            else
            {
                _logger.LogError($"Failed to start service: {result.Message}");
                _dialogService.ShowError(result.Message);
            }
            
            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception starting service: {ex.Message}", ex);
            _dialogService.ShowError($"Error: {ex.Message}");
            return false;
        }
    }
    
    // Similar methods for Stop, Restart, Pause, Resume
}
```

### Dialog Service
**Purpose**: Decouple UI dialogs from ViewModels for testability

```csharp
// Services/IDialogService.cs
public interface IDialogService
{
    void ShowError(string message);
    void ShowSuccess(string message);
    void ShowWarning(string message);
    void ShowInfo(string message);
    bool ShowConfirmation(string message);
}
```

## Data Flow Patterns

### Service List Loading Flow
```
User Action (Refresh) 
    ↓
RefreshCommand.Execute()
    ↓
MainViewModel.LoadServicesAsync()
    ↓
ServiceRepository.GetAllServicesAsync()
    ↓
ServiceController.GetServices()
    ↓
Map to WindowsServiceModel
    ↓
Update ObservableCollection<WindowsServiceViewModel>
    ↓
UI Auto-Updates (INotifyPropertyChanged)
```

### Service Control Operation Flow
```
User Action (Click Start Button)
    ↓
StartServiceCommand.CanExecute() → Check if service can be started
    ↓
StartServiceCommand.Execute()
    ↓
ServiceManager.StartServiceAsync()
    ↓
Check Permissions (IsAdministrator)
    ↓
Check System Critical (Confirmation Dialog)
    ↓
ServiceRepository.StartServiceAsync()
    ↓
ServiceController.Start()
    ↓
ServiceOperationResult
    ↓
DialogService.ShowSuccess/ShowError
    ↓
Refresh Service List
    ↓
UI Updates
```

## Error Handling Strategy

### Layered Exception Handling
```csharp
// Layer 1: Repository Layer - Catch specific exceptions
try
{
    serviceController.Start();
    return ServiceOperationResult.SuccessResult();
}
catch (InvalidOperationException ex)
{
    return ServiceOperationResult.FailureResult(
        "Service cannot be started in its current state", ex);
}
catch (UnauthorizedAccessException ex)
{
    return ServiceOperationResult.FailureResult(
        "Administrator privileges required", ex);
}

// Layer 2: Service Manager - Business logic error handling
var result = await _repository.StartServiceAsync(serviceName);
if (!result.Success)
{
    _logger.LogError(result.Message, result.Exception);
    _dialogService.ShowError(result.Message);
    return false;
}

// Layer 3: ViewModel - UI error handling
try
{
    await _serviceManager.StartServiceAsync(SelectedService.ServiceName);
}
catch (Exception ex)
{
    // Last resort - should rarely reach here
    _dialogService.ShowError($"Unexpected error: {ex.Message}");
}
```

## Performance Patterns

### Async/Await Throughout
- All I/O operations are async
- UI thread never blocks
- Cancellation token support for long-running operations

### Lazy Loading
- Load service details only when selected
- Virtualized DataGrid for large service lists

### Debouncing
- Search input debounced (300ms delay)
- Prevents excessive filtering operations

### Caching
- Service list cached with periodic refresh
- Configurable auto-refresh interval (default 5 seconds)

## Testing Strategy

### Unit Tests
- ViewModels (command logic, property changes)
- Service layer (business logic)
- Repository layer (data access, mocking ServiceController)

### Integration Tests
- Full MVVM stack with mocked dialogs
- Service operations with test service

### UI Tests
- Manual testing checklist
- Future: Automated UI tests with FlaUI

## Security Patterns

### Privilege Escalation
```csharp
public static bool IsAdministrator()
{
    using var identity = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(identity);
    return principal.IsInRole(WindowsBuiltInRole.Administrator);
}
```

### System-Critical Service Protection
```csharp
private static readonly HashSet<string> SystemCriticalServices = new()
{
    "RpcSs", "DCOM", "EventLog", "PlugPlay", "CryptSvc"
    // Extended list maintained in configuration
};
```

## Project Structure

```
WindowsServiceManager/
├── Models/
│   ├── WindowsServiceModel.cs
│   └── ServiceOperationResult.cs
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── MainViewModel.cs
│   └── WindowsServiceViewModel.cs
├── Views/
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── Services/
│   ├── IServiceRepository.cs
│   ├── WindowsServiceRepository.cs
│   ├── IServiceManager.cs
│   ├── ServiceManager.cs
│   ├── IDialogService.cs
│   └── DialogService.cs
├── Commands/
│   ├── RelayCommand.cs
│   └── AsyncRelayCommand.cs
├── Converters/
│   ├── StatusToColorConverter.cs
│   └── BoolToVisibilityConverter.cs
├── Helpers/
│   ├── SecurityHelper.cs
│   └── ServiceHelper.cs
└── App.xaml / App.xaml.cs
```

---
*Created: 2024-12-14*
*Version: 1.0*
