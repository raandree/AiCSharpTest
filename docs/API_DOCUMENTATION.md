# API Documentation

## Namespace: WindowsServiceManager.Services

### IServiceManager Interface

High-level service management operations with permission checks and user confirmations.

#### Methods

##### LoadServicesAsync()
```csharp
Task<IEnumerable<WindowsServiceModel>> LoadServicesAsync()
```
Loads all Windows services from the system.

**Returns:** Collection of all services.

---

##### RefreshServiceAsync(string serviceName)
```csharp
Task<WindowsServiceModel?> RefreshServiceAsync(string serviceName)
```
Refreshes a specific service's state.

**Parameters:**
- `serviceName`: The service to refresh.

**Returns:** The updated service model, or null if not found.

---

##### StartServiceAsync(WindowsServiceModel service)
```csharp
Task<ServiceOperationResult> StartServiceAsync(WindowsServiceModel service)
```
Starts a service with permission checks and user confirmation for critical services.

**Parameters:**
- `service`: The service to start.

**Returns:** Operation result.

**Throws:**
- `ArgumentNullException`: When service is null.

---

##### StopServiceAsync(WindowsServiceModel service)
```csharp
Task<ServiceOperationResult> StopServiceAsync(WindowsServiceModel service)
```
Stops a service with permission checks and user confirmation for critical services.

**Parameters:**
- `service`: The service to stop.

**Returns:** Operation result.

**Throws:**
- `ArgumentNullException`: When service is null.

---

##### RestartServiceAsync(WindowsServiceModel service)
```csharp
Task<ServiceOperationResult> RestartServiceAsync(WindowsServiceModel service)
```
Restarts a service with permission checks and user confirmation for critical services.

**Parameters:**
- `service`: The service to restart.

**Returns:** Operation result.

**Throws:**
- `ArgumentNullException`: When service is null.

---

##### PauseServiceAsync(WindowsServiceModel service)
```csharp
Task<ServiceOperationResult> PauseServiceAsync(WindowsServiceModel service)
```
Pauses a service with permission checks.

**Parameters:**
- `service`: The service to pause.

**Returns:** Operation result.

**Throws:**
- `ArgumentNullException`: When service is null.

---

##### ResumeServiceAsync(WindowsServiceModel service)
```csharp
Task<ServiceOperationResult> ResumeServiceAsync(WindowsServiceModel service)
```
Resumes a paused service with permission checks.

**Parameters:**
- `service`: The service to resume.

**Returns:** Operation result.

**Throws:**
- `ArgumentNullException`: When service is null.

---

##### IsRunningAsAdministrator()
```csharp
bool IsRunningAsAdministrator()
```
Checks if the application is running with administrator privileges.

**Returns:** True if running as administrator; otherwise, false.

---

### IServiceRepository Interface

Data access interface for Windows services.

#### Methods

##### GetAllServicesAsync()
```csharp
Task<IEnumerable<WindowsServiceModel>> GetAllServicesAsync()
```
Retrieves all Windows services from the system.

**Returns:** Collection of all services.

---

##### RefreshServiceAsync(string serviceName)
```csharp
Task<WindowsServiceModel?> RefreshServiceAsync(string serviceName)
```
Refreshes a single service's information.

**Parameters:**
- `serviceName`: The service name to refresh.

**Returns:** Updated service model, or null if not found.

---

##### StartServiceAsync(string serviceName)
```csharp
Task<ServiceOperationResult> StartServiceAsync(string serviceName)
```
Starts a Windows service.

**Parameters:**
- `serviceName`: The service to start.

**Returns:** Operation result.

---

##### StopServiceAsync(string serviceName)
```csharp
Task<ServiceOperationResult> StopServiceAsync(string serviceName)
```
Stops a Windows service.

**Parameters:**
- `serviceName`: The service to stop.

**Returns:** Operation result.

---

##### PauseServiceAsync(string serviceName)
```csharp
Task<ServiceOperationResult> PauseServiceAsync(string serviceName)
```
Pauses a Windows service.

**Parameters:**
- `serviceName`: The service to pause.

**Returns:** Operation result.

---

##### ResumeServiceAsync(string serviceName)
```csharp
Task<ServiceOperationResult> ResumeServiceAsync(string serviceName)
```
Resumes a paused Windows service.

**Parameters:**
- `serviceName`: The service to resume.

**Returns:** Operation result.

---

### IDialogService Interface

UI dialog abstraction for user interactions.

#### Methods

##### ShowError(string message, string title = "Error")
```csharp
void ShowError(string message, string title = "Error")
```
Displays an error message dialog.

**Parameters:**
- `message`: The error message to display.
- `title`: Optional dialog title (default: "Error").

---

##### ShowSuccess(string message, string title = "Success")
```csharp
void ShowSuccess(string message, string title = "Success")
```
Displays a success message dialog.

**Parameters:**
- `message`: The success message to display.
- `title`: Optional dialog title (default: "Success").

---

##### ShowWarning(string message, string title = "Warning")
```csharp
void ShowWarning(string message, string title = "Warning")
```
Displays a warning message dialog.

**Parameters:**
- `message`: The warning message to display.
- `title`: Optional dialog title (default: "Warning").

---

##### ShowInformation(string message, string title = "Information")
```csharp
void ShowInformation(string message, string title = "Information")
```
Displays an information message dialog.

**Parameters:**
- `message`: The information message to display.
- `title`: Optional dialog title (default: "Information").

---

##### ShowConfirmation(string message, string title = "Confirm")
```csharp
bool ShowConfirmation(string message, string title = "Confirm")
```
Displays a confirmation dialog.

**Parameters:**
- `message`: The confirmation message to display.
- `title`: Optional dialog title (default: "Confirm").

**Returns:** True if user confirmed; otherwise, false.

---

## Namespace: WindowsServiceManager.Models

### WindowsServiceModel Class

Represents a Windows service with all its properties and state information.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `ServiceName` | `string` | Internal service name used by the system |
| `DisplayName` | `string` | Friendly display name of the service |
| `Description` | `string` | Description of what the service does |
| `Status` | `ServiceControllerStatus` | Current status (Running, Stopped, etc.) |
| `StartupType` | `ServiceStartMode` | How the service starts (Automatic, Manual, Disabled) |
| `Dependencies` | `string[]` | List of services this service depends on |
| `CanStop` | `bool` | Whether the service can be stopped |
| `CanPauseAndContinue` | `bool` | Whether the service can be paused and continued |
| `IsSystemCritical` | `bool` | Whether this is a system-critical service |
| `ProcessId` | `int?` | Process ID if running |
| `PathName` | `string` | Executable path of the service |

---

### ServiceOperationResult Class

Encapsulates the result of a service operation.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Success` | `bool` | Whether the operation succeeded |
| `Message` | `string` | User-friendly result message |
| `Exception` | `Exception?` | Exception if operation failed |

#### Methods

##### SuccessResult(string message)
```csharp
static ServiceOperationResult SuccessResult(string message)
```
Creates a success result.

**Parameters:**
- `message`: Success message.

**Returns:** Success operation result.

---

##### FailureResult(string message, Exception? exception = null)
```csharp
static ServiceOperationResult FailureResult(string message, Exception? exception = null)
```
Creates a failure result.

**Parameters:**
- `message`: Failure message.
- `exception`: Optional exception that caused the failure.

**Returns:** Failure operation result.

---

## Namespace: WindowsServiceManager.ViewModels

### MainViewModel Class

Main ViewModel for the Windows Service Manager application.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `FilteredServices` | `ObservableCollection<WindowsServiceModel>` | Filtered list of services |
| `SelectedService` | `WindowsServiceModel?` | Currently selected service |
| `SearchText` | `string` | Search filter text |
| `IsLoading` | `bool` | Whether services are being loaded |
| `StatusMessage` | `string` | Current status message |
| `IsAdministrator` | `bool` | Whether running as administrator |

#### Commands

| Command | Description |
|---------|-------------|
| `RefreshCommand` | Refreshes the service list |
| `StartServiceCommand` | Starts the selected service |
| `StopServiceCommand` | Stops the selected service |
| `RestartServiceCommand` | Restarts the selected service |
| `PauseServiceCommand` | Pauses the selected service |
| `ResumeServiceCommand` | Resumes the selected service |

#### Methods

##### InitializeAsync()
```csharp
Task InitializeAsync()
```
Initializes the ViewModel. Call this after the ViewModel is created.

**Returns:** Task representing the async operation.

---

### ViewModelBase Class

Base class for all ViewModels, providing INotifyPropertyChanged implementation.

#### Methods

##### OnPropertyChanged([CallerMemberName] string? propertyName = null)
```csharp
protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
```
Raises the PropertyChanged event for the specified property.

**Parameters:**
- `propertyName`: Name of the property that changed (auto-filled by compiler).

---

##### SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
```csharp
protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
```
Sets the property value and raises PropertyChanged if the value has changed.

**Type Parameters:**
- `T`: Type of the property.

**Parameters:**
- `field`: Reference to the backing field.
- `value`: New value to set.
- `propertyName`: Name of the property (auto-filled by compiler).

**Returns:** True if the value was changed; otherwise, false.

---

## Namespace: WindowsServiceManager.Commands

### AsyncRelayCommand Class

An asynchronous command implementation that relays its execution to async delegates.

#### Constructor

```csharp
AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
```

**Parameters:**
- `execute`: The async execution logic.
- `canExecute`: Optional execution status logic.

**Throws:**
- `ArgumentNullException`: When execute is null.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsExecuting` | `bool` | Whether the command is currently executing |

#### Methods

##### CanExecute(object? parameter)
```csharp
bool CanExecute(object? parameter)
```
Determines whether the command can execute in its current state.

**Returns:** True if the command can be executed; otherwise, false.

---

##### Execute(object? parameter)
```csharp
void Execute(object? parameter)
```
Executes the command asynchronously.

---

##### Cancel()
```csharp
void Cancel()
```
Cancels the ongoing command execution if one is in progress.

---

### RelayCommand Class

A synchronous command implementation that relays its execution to delegates.

#### Constructor

```csharp
RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
```

**Parameters:**
- `execute`: The execution logic.
- `canExecute`: Optional execution status logic.

**Throws:**
- `ArgumentNullException`: When execute is null.

#### Methods

##### CanExecute(object? parameter)
```csharp
bool CanExecute(object? parameter)
```
Determines whether the command can execute in its current state.

**Returns:** True if the command can be executed; otherwise, false.

---

##### Execute(object? parameter)
```csharp
void Execute(object? parameter)
```
Executes the command.

---

## Namespace: WindowsServiceManager.Helpers

### SecurityHelper Class

Provides security-related utility methods.

#### Methods

##### IsAdministrator()
```csharp
static bool IsAdministrator()
```
Determines whether the current process is running with administrator privileges.

**Returns:** True if running as administrator; otherwise, false.

---

##### GetCurrentUserName()
```csharp
static string GetCurrentUserName()
```
Gets the current user's name.

**Returns:** The current user name, or "Unknown" if unable to determine.

---

## Usage Examples

### Example 1: Starting a Service

```csharp
var serviceManager = serviceProvider.GetRequiredService<IServiceManager>();
var service = new WindowsServiceModel { ServiceName = "Spooler" };

var result = await serviceManager.StartServiceAsync(service);
if (result.Success)
{
    Console.WriteLine($"Service started: {result.Message}");
}
else
{
    Console.WriteLine($"Failed to start service: {result.Message}");
}
```

### Example 2: Loading All Services

```csharp
var serviceManager = serviceProvider.GetRequiredService<IServiceManager>();
var services = await serviceManager.LoadServicesAsync();

foreach (var service in services)
{
    Console.WriteLine($"{service.DisplayName} - {service.Status}");
}
```

### Example 3: Checking Administrator Privileges

```csharp
var serviceManager = serviceProvider.GetRequiredService<IServiceManager>();
if (serviceManager.IsRunningAsAdministrator())
{
    Console.WriteLine("Running with administrator privileges");
}
else
{
    Console.WriteLine("Not running as administrator");
}
```

### Example 4: Creating a Custom Command

```csharp
public class CustomViewModel : ViewModelBase
{
    private readonly IServiceManager _serviceManager;
    
    public ICommand MyCustomCommand { get; }
    
    public CustomViewModel(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
        MyCustomCommand = new AsyncRelayCommand(ExecuteCustomAsync, CanExecuteCustom);
    }
    
    private async Task ExecuteCustomAsync()
    {
        var services = await _serviceManager.LoadServicesAsync();
        // Process services...
    }
    
    private bool CanExecuteCustom()
    {
        return _serviceManager.IsRunningAsAdministrator();
    }
}
```
