# Windows Service Manager

A modern, user-friendly WPF application for managing Windows services with administrative privileges and safety features.

## 🌟 Features

- **Service Management**: View, start, stop, restart, pause, and resume Windows services
- **Real-time Status**: Monitor service status with automatic refresh capabilities
- **Search & Filter**: Quickly find services using the built-in search functionality
- **Safety Features**: 
  - Critical service warnings before making changes
  - Administrator privilege detection and enforcement
  - Confirmation dialogs for system-critical operations
- **Service Information**: View detailed service information including:
  - Service name and display name
  - Current status and startup type
  - Dependencies
  - Process ID and path
  - Service description
- **Modern UI**: Clean, responsive WPF interface with status indicators

## 📋 Prerequisites

- **Operating System**: Windows 10 or later
- **.NET Runtime**: .NET 8.0 or later
- **Permissions**: Administrator rights required for service operations

## 🚀 Installation

### From Release

1. Download the latest release from the [Releases](../../releases) page
2. Extract the ZIP file to your desired location
3. Run `WindowsServiceManager.exe` as Administrator

### From Source

1. Clone the repository:
   ```powershell
   git clone https://github.com/raandree/AiCSharpTest.git
   cd AiCSharpTest
   ```

2. Build the solution:
   ```powershell
   dotnet build WindowsServiceManager.sln --configuration Release
   ```

3. Run the application:
   ```powershell
   .\WindowsServiceManager\bin\Release\net8.0-windows\WindowsServiceManager.exe
   ```

## 💡 Usage

### Starting the Application

1. **Run as Administrator**: Right-click the application and select "Run as administrator"
2. The application will display all Windows services on startup

### Managing Services

- **Start a Service**: Select a stopped service and click the "Start" button
- **Stop a Service**: Select a running service and click the "Stop" button
- **Restart a Service**: Select a running service and click the "Restart" button
- **Pause/Resume**: Use the Pause/Resume buttons for services that support these operations

### Searching for Services

- Use the search box at the top to filter services by name or display name
- The list updates in real-time as you type

### Refreshing Service Status

- Click the "Refresh" button to update the status of all services
- Service status is also automatically updated after operations

## 🏗️ Architecture

The application follows the **MVVM (Model-View-ViewModel)** pattern with dependency injection:

### Project Structure

```
WindowsServiceManager/
├── Commands/              # ICommand implementations
│   ├── AsyncRelayCommand.cs      # Async command with cancellation support
│   └── RelayCommand.cs           # Synchronous relay command
├── Converters/            # WPF value converters
│   ├── InverseBooleanToVisibilityConverter.cs
│   └── StatusToColorConverter.cs
├── Helpers/               # Utility classes
│   └── SecurityHelper.cs         # Admin privilege checking
├── Models/                # Data models
│   ├── ServiceOperationResult.cs # Operation result wrapper
│   └── WindowsServiceModel.cs    # Service data model
├── Services/              # Business logic layer
│   ├── IServiceManager.cs        # High-level service operations interface
│   ├── ServiceManager.cs         # Service management orchestration
│   ├── IServiceRepository.cs     # Data access interface
│   ├── WindowsServiceRepository.cs # Windows service data access
│   ├── IDialogService.cs         # UI dialog interface
│   └── DialogService.cs          # WPF dialog implementation
├── ViewModels/            # View models
│   ├── ViewModelBase.cs          # Base class with INotifyPropertyChanged
│   └── MainViewModel.cs          # Main window view model
└── Views/                 # WPF views
    ├── MainWindow.xaml
    └── MainWindow.xaml.cs
```

### Key Design Patterns

1. **MVVM Pattern**: Separation of UI, business logic, and data
2. **Repository Pattern**: Abstraction of data access through `IServiceRepository`
3. **Service Pattern**: High-level operations orchestration via `IServiceManager`
4. **Dependency Injection**: Uses `Microsoft.Extensions.DependencyInjection`
5. **Command Pattern**: Async and synchronous relay commands for UI actions
6. **Observer Pattern**: `INotifyPropertyChanged` for data binding

### Dependencies

- **System.ServiceProcess.ServiceController**: Windows service control
- **System.Management**: WMI queries for extended service information
- **Microsoft.Extensions.DependencyInjection**: Dependency injection container
- **Microsoft.Extensions.Logging**: Structured logging framework

## 🔒 Security Considerations

- The application requires administrator privileges for service operations
- System-critical services display warning dialogs before operations
- All operations are logged for audit purposes
- Administrator status is checked before each operation

## 🧪 Testing

The project includes a test project `WindowsServiceManager.Tests`:

```powershell
dotnet test WindowsServiceManager.Tests
```

## 🐛 Troubleshooting

### "Access Denied" Errors

- Ensure you're running the application as Administrator
- Check that your user account has the necessary permissions

### Services Not Displayed

- Verify you have permission to query Windows services
- Check Windows Event Viewer for related errors

### Application Won't Start

- Verify .NET 8.0 runtime is installed
- Check that all required DLLs are present in the application directory

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## 📧 Support

For issues, questions, or suggestions:
- Open an issue on [GitHub Issues](../../issues)
- Contact the maintainers

## 🔄 Version History

See [CHANGELOG.md](CHANGELOG.md) for a detailed version history.

## 👨‍💻 Authors

- **raandree** - *Initial work* - [raandree](https://github.com/raandree)

## 🙏 Acknowledgments

- Built with WPF and .NET 8.0
- Uses Windows Service Control Manager API
- Inspired by the need for a modern, safe Windows service management tool
