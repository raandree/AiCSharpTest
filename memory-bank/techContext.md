# Technical Context: Windows Service Manager UI

## Technology Stack

### Core Framework
- **.NET 8.0** (LTS)
  - Latest long-term support version
  - Cross-platform runtime (though this app targets Windows)
  - Modern C# 12 language features
  - Performance improvements over previous versions

### UI Framework
- **WPF (Windows Presentation Foundation)**
  - Native Windows desktop UI framework
  - Rich data binding support
  - XAML-based declarative UI
  - Hardware-accelerated rendering
  - Built-in MVVM support

### Language
- **C# 12**
  - Primary constructors
  - Collection expressions
  - Lambda expression improvements
  - Nullable reference types enabled
  - Modern async/await patterns

## Key Dependencies

### NuGet Packages

#### Essential
```xml
<PackageReference Include="System.ServiceProcess.ServiceController" Version="8.0.0" />
<!-- Core Windows service interaction -->

<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<!-- Dependency injection container -->

<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
<!-- Logging infrastructure -->

<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
<!-- Console logging provider -->
```

#### UI Enhancement (Optional)
```xml
<PackageReference Include="ModernWpfUI" Version="0.9.6" />
<!-- Modern Windows 11 styling for WPF -->

<PackageReference Include="MaterialDesignThemes" Version="4.9.0" />
<!-- Alternative: Material Design styling -->
```

#### Testing
```xml
<PackageReference Include="xUnit" Version="2.6.5" />
<PackageReference Include="xUnit.runner.visualstudio" Version="2.5.6" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
```

## Development Environment

### Required Tools
1. **Visual Studio 2022** (v17.8 or later)
   - Community/Professional/Enterprise edition
   - Workloads:
     - .NET Desktop Development
     - Windows application development
   
   OR

2. **Visual Studio Code** with extensions:
   - C# Dev Kit
   - .NET Extension Pack
   - XAML extension

3. **.NET 8.0 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Version: 8.0.x (latest patch)

4. **Git** for version control
   - Version: 2.40 or later

### Recommended Tools
- **ReSharper** or **Rider** (JetBrains) - Advanced C# IDE features
- **XAML Styler** - XAML code formatting
- **Snoop** - WPF visual tree debugging
- **WPF Inspector** - Runtime WPF debugging

### System Requirements
- **OS**: Windows 10 (1809+) or Windows 11
- **Architecture**: x64
- **RAM**: Minimum 4GB, Recommended 8GB+
- **Disk**: ~500MB for application
- **Permissions**: Administrator rights required for service control operations

## Project Configuration

### Solution Structure
```
WindowsServiceManager.sln
├── WindowsServiceManager (WPF App)
│   ├── WindowsServiceManager.csproj
│   └── [Source files]
├── WindowsServiceManager.Tests (Unit Tests)
│   ├── WindowsServiceManager.Tests.csproj
│   └── [Test files]
└── WindowsServiceManager.IntegrationTests (Integration Tests)
    ├── WindowsServiceManager.IntegrationTests.csproj
    └── [Integration test files]
```

### Project File (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
    <LangVersion>12</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Application Info -->
    <AssemblyName>WindowsServiceManager</AssemblyName>
    <RootNamespace>WindowsServiceManager</RootNamespace>
    <ApplicationIcon>Resources\app.ico</ApplicationIcon>
    
    <!-- Version Info -->
    <Version>1.0.0</Version>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    
    <!-- Build Configuration -->
    <Platforms>x64</Platforms>
    <PlatformTarget>x64</PlatformTarget>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    
    <!-- Code Analysis -->
    <AnalysisLevel>latest</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>
  
  <!-- Package References -->
  <ItemGroup>
    <PackageReference Include="System.ServiceProcess.ServiceController" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
  </ItemGroup>
</Project>
```

## Build Configuration

### Debug Configuration
```xml
<PropertyGroup Condition="'$(Configuration)'=='Debug'">
  <DefineConstants>DEBUG;TRACE</DefineConstants>
  <Optimize>false</Optimize>
  <DebugType>full</DebugType>
  <DebugSymbols>true</DebugSymbols>
  <NoWarn></NoWarn>
</PropertyGroup>
```

### Release Configuration
```xml
<PropertyGroup Condition="'$(Configuration)'=='Release'">
  <DefineConstants>TRACE</DefineConstants>
  <Optimize>true</Optimize>
  <DebugType>portable</DebugType>
  <DebugSymbols>true</DebugSymbols>
  <TrimMode>link</TrimMode>
  <PublishTrimmed>true</PublishTrimmed>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>false</SelfContained>
</PropertyGroup>
```

## Code Quality Tools

### Static Analysis
- **C# Compiler Warnings**: Elevated to highest level
- **Roslyn Analyzers**: Built-in .NET analyzers enabled
- **StyleCop**: Code style enforcement (optional)
- **SonarAnalyzer**: Security and code smell detection (optional)

### EditorConfig
```ini
# .editorconfig
root = true

[*.cs]
# Indentation
indent_style = space
indent_size = 4

# New line preferences
end_of_line = crlf
insert_final_newline = true

# Naming conventions
dotnet_naming_rule.interface_should_be_begins_with_i.severity = warning
dotnet_naming_rule.interface_should_be_begins_with_i.symbols = interface
dotnet_naming_rule.interface_should_be_begins_with_i.style = begins_with_i

# Code style rules
csharp_prefer_braces = true:warning
csharp_prefer_simple_using_statement = true:suggestion
dotnet_diagnostic.CA1062.severity = warning  # Validate parameter nullability
dotnet_diagnostic.CA1031.severity = suggestion  # Do not catch general exception types
```

## Testing Framework

### Unit Testing
- **xUnit**: Primary test framework
- **Moq**: Mocking framework for dependencies
- **FluentAssertions**: Readable assertion syntax

### Test Structure
```csharp
public class MainViewModelTests
{
    private readonly Mock<IServiceRepository> _mockRepository;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly MainViewModel _viewModel;
    
    public MainViewModelTests()
    {
        _mockRepository = new Mock<IServiceRepository>();
        _mockDialogService = new Mock<IDialogService>();
        _viewModel = new MainViewModel(_mockRepository.Object, _mockDialogService.Object);
    }
    
    [Fact]
    public async Task LoadServices_ShouldPopulateServicesList()
    {
        // Arrange
        var mockServices = new List<WindowsServiceModel>
        {
            new() { ServiceName = "TestService", DisplayName = "Test Service" }
        };
        _mockRepository.Setup(r => r.GetAllServicesAsync())
            .ReturnsAsync(mockServices);
        
        // Act
        await _viewModel.LoadServicesAsync();
        
        // Assert
        _viewModel.Services.Should().HaveCount(1);
        _viewModel.Services[0].DisplayName.Should().Be("Test Service");
    }
}
```

## Logging Strategy

### Logging Levels
- **Trace**: Extremely detailed diagnostic information
- **Debug**: Development diagnostic information
- **Information**: General application flow
- **Warning**: Unexpected but recoverable issues
- **Error**: Errors and exceptions
- **Critical**: Catastrophic failures

### Logging Implementation
```csharp
public class WindowsServiceRepository : IServiceRepository
{
    private readonly ILogger<WindowsServiceRepository> _logger;
    
    public WindowsServiceRepository(ILogger<WindowsServiceRepository> logger)
    {
        _logger = logger;
    }
    
    public async Task<ServiceOperationResult> StartServiceAsync(string serviceName)
    {
        _logger.LogInformation("Attempting to start service: {ServiceName}", serviceName);
        
        try
        {
            // Implementation
            _logger.LogInformation("Successfully started service: {ServiceName}", serviceName);
            return ServiceOperationResult.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start service: {ServiceName}", serviceName);
            return ServiceOperationResult.FailureResult(ex.Message, ex);
        }
    }
}
```

### Log Output Locations
- **Development**: Console output in Visual Studio
- **Production**: File-based logging in `%LOCALAPPDATA%\WindowsServiceManager\Logs\`
- **Format**: JSON structured logging for easy parsing

## Security Considerations

### Privilege Requirements
- Application requires **Administrator** privileges for service control operations
- Manifest file includes `requestedExecutionLevel` setting

### App.manifest
```xml
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <assemblyIdentity version="1.0.0.0" name="WindowsServiceManager.app"/>
  <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
    <security>
      <requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">
        <requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
      </requestedPrivileges>
    </security>
  </trustInfo>
</assembly>
```

### Security Best Practices
- No password storage
- No remote connection support (Phase 1)
- Read-only service information by default
- Explicit user confirmation for system-critical services
- Audit logging of all service control operations

## Performance Targets

### Application Startup
- **Target**: < 2 seconds from launch to UI ready
- **Measurement**: Time to first paint with service list loaded

### Service List Loading
- **Target**: < 2 seconds for initial load
- **Measurement**: Time from GetServices() call to UI population

### Search/Filter Performance
- **Target**: < 100ms for filter results
- **Measurement**: Time from keystroke to filtered list display

### Service Operation Response
- **Target**: UI feedback within 500ms
- **Measurement**: Time from button click to progress indicator

### Memory Usage
- **Target**: < 100MB baseline, < 150MB under normal operation
- **Measurement**: Private working set in Task Manager

## Deployment

### Packaging Options

#### Option 1: Framework-Dependent Deployment
- Requires .NET 8.0 Runtime installed on target system
- Smallest package size (~2-5 MB)
- Faster startup time

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

#### Option 2: Self-Contained Deployment
- Includes .NET runtime
- No prerequisites on target system
- Larger package size (~80-120 MB)

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

#### Option 3: Single-File Deployment
- All files bundled into single .exe
- Easiest distribution
- Slightly slower startup

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Installation
- No installer required (Phase 1)
- Portable executable
- Optional: Windows Installer (MSI) for future versions

## Development Workflow

### Initial Setup
```bash
# Clone repository
git clone <repository-url>
cd WindowsServiceManager

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run application
dotnet run --project WindowsServiceManager
```

### Testing Workflow
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test
dotnet test --filter "FullyQualifiedName~MainViewModelTests"
```

### Build Workflow
```bash
# Debug build
dotnet build -c Debug

# Release build
dotnet build -c Release

# Publish release
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish
```

## Troubleshooting

### Common Issues

#### Issue: "Access Denied" when starting services
**Solution**: Run application as Administrator (Right-click → Run as administrator)

#### Issue: ServiceController throws InvalidOperationException
**Solution**: Ensure Windows Services feature is enabled, check service exists

#### Issue: High memory usage
**Solution**: Ensure proper disposal of ServiceController instances, check for memory leaks in event handlers

#### Issue: UI freezes during service operations
**Solution**: Verify all I/O operations use async/await, check for blocking calls on UI thread

## Version Control

### Git Configuration
```gitignore
# .gitignore
## Build results
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

## Visual Studio cache/options
.vs/
*.suo
*.user
*.userosscache
*.sln.docstates

## NuGet
*.nupkg
**/packages/*
!**/packages/build/

## Test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*
*.trx
*.coverage
*.coveragexml

## User-specific files
*.rsuser
.vscode/
```

---
*Created: 2024-12-14*
*Version: 1.0*
