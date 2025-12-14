# Windows Service Manager - Technical Documentation

## Overview

Windows Service Manager is a WPF-based application for managing Windows services. It provides a modern, safe interface for viewing and controlling system services with built-in safety features for critical operations.

## Architecture

### Design Patterns

The application follows several well-established design patterns:

#### 1. Model-View-ViewModel (MVVM)

- **Models** (`Models/`): Data structures representing Windows services and operation results
- **Views** (`Views/`): XAML-based UI components
- **ViewModels** (`ViewModels/`): Presentation logic and data binding

#### 2. Repository Pattern

- **IServiceRepository**: Abstraction for data access
- **WindowsServiceRepository**: Concrete implementation using Windows APIs

#### 3. Service Pattern

- **IServiceManager**: High-level business operations
- **ServiceManager**: Orchestrates repository calls, permission checks, and user interactions

#### 4. Command Pattern

- **ICommand** implementations for user actions
- **AsyncRelayCommand**: Asynchronous operations with cancellation
- **RelayCommand**: Synchronous operations

#### 5. Dependency Injection

Uses `Microsoft.Extensions.DependencyInjection` for:
- Service lifetime management
- Loose coupling between components
- Testability

### Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                          Views                              │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              MainWindow.xaml                        │   │
│  └─────────────────┬───────────────────────────────────┘   │
└────────────────────┼───────────────────────────────────────┘
                     │ Data Binding
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                       ViewModels                            │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              MainViewModel                          │   │
│  │  - FilteredServices                                 │   │
│  │  - SelectedService                                  │   │
│  │  - Commands (Start/Stop/Restart/etc.)              │   │
│  └─────────────────┬───────────────────────────────────┘   │
└────────────────────┼───────────────────────────────────────┘
                     │ Uses
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                        Services                             │
│  ┌──────────────────────────┐  ┌──────────────────────┐    │
│  │    IServiceManager       │  │   IDialogService     │    │
│  │  ┌──────────────────┐    │  │  ┌──────────────┐   │    │
│  │  │  ServiceManager  │────┼──┼──│DialogService │   │    │
│  │  └────────┬─────────┘    │  │  └──────────────┘   │    │
│  └───────────┼──────────────┘  └──────────────────────┘    │
│              │ Uses                                         │
│              ▼                                              │
│  ┌──────────────────────────┐                              │
│  │   IServiceRepository     │                              │
│  │  ┌────────────────────┐  │                              │
│  │  │WindowsServiceRepo  │  │                              │
│  │  └────────────────────┘  │                              │
│  └──────────────────────────┘                              │
└─────────────────────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                    Windows APIs                             │
│  - System.ServiceProcess.ServiceController                 │
│  - System.Management (WMI)                                 │
└─────────────────────────────────────────────────────────────┘
```

## Key Components

### Models

#### WindowsServiceModel
Represents a Windows service with properties:
- `ServiceName`: Internal system name
- `DisplayName`: User-friendly name
- `Status`: Current service status
- `StartupType`: How the service starts
- `CanStop`, `CanPauseAndContinue`: Capability flags
- `IsSystemCritical`: Safety flag for critical services
- `ProcessId`, `PathName`: Runtime information

#### ServiceOperationResult
Encapsulates operation outcomes:
- `Success`: Operation status
- `Message`: User-friendly result message
- `Exception`: Error details if failed

### ViewModels

#### MainViewModel
Central ViewModel managing:
- Service collection and filtering
- Selected service state
- Command execution
- Loading states and status messages

Key responsibilities:
- Load and refresh service list
- Filter services based on search text
- Execute service operations via commands
- Update UI state based on operations

#### ViewModelBase
Base class providing:
- `INotifyPropertyChanged` implementation
- `SetProperty<T>` helper for property changes
- `OnPropertyChanged` event raising
- Multi-property change notification

### Services

#### ServiceManager
High-level service operations:
- Permission checking (admin privileges)
- Critical service confirmation dialogs
- Operation orchestration
- Result handling and user feedback

#### WindowsServiceRepository
Low-level Windows service access:
- Service enumeration via `ServiceController`
- Extended information via WMI queries
- Service control operations (start/stop/pause/resume)
- Status polling and timeout handling

#### DialogService
User interaction abstraction:
- Message boxes (error/warning/success/info)
- Confirmation dialogs
- WPF MessageBox wrapper for testability

### Commands

#### AsyncRelayCommand
Async command execution with:
- Automatic execution state tracking
- Prevents concurrent executions
- Optional `CanExecute` predicate
- Cancellation token support

#### RelayCommand
Synchronous command execution with:
- Direct action invocation
- Optional `CanExecute` predicate
- Standard ICommand implementation

### Converters

#### StatusToColorConverter
Maps `ServiceControllerStatus` to colors:
- Running → Green
- Stopped → Red
- Paused → Orange
- Pending states → Yellow

#### InverseBooleanToVisibilityConverter
Inverted boolean to visibility mapping:
- `true` → `Visibility.Collapsed`
- `false` → `Visibility.Visible`

### Helpers

#### SecurityHelper
Security utilities:
- `IsAdministrator()`: Check admin privileges
- `GetCurrentUserName()`: Get current user identity

## Data Flow

### Service Loading Flow

```
User Action (Refresh)
    ↓
MainViewModel.RefreshCommand
    ↓
MainViewModel.LoadServicesAsync()
    ↓
IServiceManager.LoadServicesAsync()
    ↓
IServiceRepository.GetAllServicesAsync()
    ↓
ServiceController.GetServices() + WMI Queries
    ↓
WindowsServiceModel[] returned
    ↓
ObservableCollection updated
    ↓
UI updates via data binding
```

### Service Operation Flow

```
User Action (Start Service)
    ↓
MainViewModel.StartServiceCommand
    ↓
MainViewModel.StartServiceAsync()
    ↓
IServiceManager.StartServiceAsync(service)
    ↓
Check Administrator Privileges
    ↓
Check if Critical Service → Show Confirmation
    ↓
IServiceRepository.StartServiceAsync(serviceName)
    ↓
ServiceController.Start() + WaitForStatus()
    ↓
ServiceOperationResult returned
    ↓
DialogService shows result
    ↓
Service list refreshed
    ↓
UI updates
```

## Threading Model

- **UI Thread**: All ViewModels and Views
- **Background Tasks**: Service operations via `Task.Run`
- **Synchronization**: Automatic via WPF Dispatcher
- **Async/Await**: Throughout for responsive UI

## Error Handling

### Exception Handling Strategy

1. **Repository Level**: Catch API exceptions, log, return error results
2. **Service Manager Level**: Validate parameters, check permissions
3. **ViewModel Level**: Handle operation results, update UI state
4. **View Level**: Display error messages to user

### Logging

Uses `Microsoft.Extensions.Logging`:
- **Information**: Normal operations, service status changes
- **Warning**: Non-critical issues, permission checks
- **Error**: Exceptions, failed operations
- **Debug**: Detailed diagnostic information

## Security

### Permission Model

- **Read Operations**: Available to all users
- **Write Operations**: Require administrator privileges
- **Critical Services**: Require additional user confirmation

### Administrator Detection

Uses `WindowsIdentity` and `WindowsPrincipal` to check:
```csharp
WindowsIdentity.GetCurrent()
WindowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator)
```

### Critical Service Protection

Services marked as `IsSystemCritical`:
- Show warning dialogs before operations
- Provide detailed impact information
- Require explicit user confirmation

## Testing Strategy

### Unit Tests

Test coverage for:
- ViewModels (command execution, property changes)
- Service Manager (business logic, permission checks)
- Repository (service operations, error handling)
- Commands (execution, CanExecute logic)

### Mocking

Mock interfaces for testing:
- `IServiceRepository` → Mock service data access
- `IDialogService` → Mock user interactions
- `ILogger<T>` → Mock logging calls

### Test Structure

```csharp
public class ServiceManagerTests
{
    private readonly Mock<IServiceRepository> _mockRepository;
    private readonly Mock<IDialogService> _mockDialog;
    private readonly Mock<ILogger<ServiceManager>> _mockLogger;
    private readonly ServiceManager _sut; // System Under Test

    [Fact]
    public async Task StartServiceAsync_ValidService_ReturnsSuccess()
    {
        // Arrange
        var service = new WindowsServiceModel { ServiceName = "TestService" };
        _mockRepository
            .Setup(r => r.StartServiceAsync(service.ServiceName))
            .ReturnsAsync(ServiceOperationResult.SuccessResult("Started"));

        // Act
        var result = await _sut.StartServiceAsync(service);

        // Assert
        Assert.True(result.Success);
    }
}
```

## Performance Considerations

### Optimization Techniques

1. **Async Operations**: All I/O operations are asynchronous
2. **Virtual Scrolling**: ListView virtualizes items for large lists
3. **Lazy Loading**: Service details loaded on-demand
4. **Debounced Search**: Search filter updates with slight delay
5. **Cached Results**: Service information cached between refreshes

### Memory Management

- Dispose of `ServiceController` instances
- Use `using` statements for IDisposable resources
- Avoid memory leaks in event handlers
- Weak event patterns where appropriate

## Extension Points

### Adding New Features

#### New Service Operation

1. Add method to `IServiceRepository`
2. Implement in `WindowsServiceRepository`
3. Add orchestration method to `IServiceManager`
4. Implement in `ServiceManager`
5. Add command to `MainViewModel`
6. Bind command in `MainWindow.xaml`

#### New Service Property

1. Add property to `WindowsServiceModel`
2. Update `MapServiceToModel` in repository
3. Add UI binding in `MainWindow.xaml`

#### Custom Dialog

1. Create new dialog interface
2. Implement WPF dialog window
3. Inject and use in ViewModels

## Build and Deployment

### Build Configuration

- **Debug**: Full symbols, no optimization
- **Release**: Optimized, trimmed

### Platform Target

- **x64**: 64-bit Windows systems only

### Dependencies

NuGet packages:
- `System.ServiceProcess.ServiceController` 8.0.0
- `System.Management` 8.0.0
- `Microsoft.Extensions.DependencyInjection` 8.0.0
- `Microsoft.Extensions.Logging` 8.0.0
- `Microsoft.Extensions.Logging.Console` 8.0.0
- `Microsoft.Extensions.Logging.Debug` 8.0.0

### Deployment Options

1. **Standalone**: Framework-dependent deployment
2. **Self-Contained**: Includes .NET runtime
3. **Single File**: All dependencies in one executable

## Future Enhancements

Potential improvements:
- Service event log viewing
- Scheduled service operations
- Service configuration editing
- Remote service management
- Service dependency graph visualization
- Export service list to CSV/Excel
- Service performance monitoring
- Custom service groups/favorites

## References

- [.NET WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [ServiceController Class](https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller)
- [WMI Win32_Service](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-service)
- [MVVM Pattern](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm)
