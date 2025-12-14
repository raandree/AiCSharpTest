# Contributing to Windows Service Manager

First off, thank you for considering contributing to Windows Service Manager! It's people like you that make this tool better for everyone.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Enhancements](#suggesting-enhancements)
  - [Pull Requests](#pull-requests)
- [Development Setup](#development-setup)
- [Coding Guidelines](#coding-guidelines)
- [Commit Message Guidelines](#commit-message-guidelines)
- [Testing](#testing)

## Code of Conduct

This project and everyone participating in it is governed by our commitment to providing a welcoming and inspiring community for all. Please be respectful and constructive in all interactions.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates. When you create a bug report, include as many details as possible:

**Bug Report Template:**

```markdown
**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Go to '...'
2. Click on '....'
3. See error

**Expected behavior**
A clear description of what you expected to happen.

**Screenshots**
If applicable, add screenshots to help explain your problem.

**Environment:**
- OS: [e.g., Windows 11]
- .NET Version: [e.g., .NET 8.0]
- Application Version: [e.g., 1.0.0]

**Additional context**
Add any other context about the problem here.
```

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, include:

- A clear and descriptive title
- A detailed description of the proposed feature
- Explanation of why this enhancement would be useful
- Any possible implementation details you've considered

### Pull Requests

1. Fork the repository and create your branch from `main`
2. If you've added code that should be tested, add tests
3. Ensure your code follows the coding guidelines
4. Make sure your code compiles without warnings
5. Update the documentation if needed
6. Write a clear commit message

## Development Setup

### Prerequisites

- Visual Studio 2022 or later (Community Edition is fine)
- .NET 8.0 SDK
- Git for Windows

### Setting Up Your Development Environment

1. **Clone the repository:**
   ```powershell
   git clone https://github.com/raandree/AiCSharpTest.git
   cd AiCSharpTest
   ```

2. **Open the solution:**
   ```powershell
   # Using Visual Studio
   start WindowsServiceManager.sln
   
   # Or using Visual Studio Code
   code .
   ```

3. **Restore NuGet packages:**
   ```powershell
   dotnet restore
   ```

4. **Build the solution:**
   ```powershell
   dotnet build
   ```

5. **Run the application:**
   - In Visual Studio: Press F5 (ensure "Run as Administrator" is configured)
   - From command line: Run the executable as Administrator

### Project Structure Overview

```
WindowsServiceManager/
├── Commands/          # ICommand implementations (MVVM pattern)
├── Converters/        # WPF value converters
├── Helpers/           # Utility classes
├── Models/            # Data models
├── Services/          # Business logic and data access
├── ViewModels/        # MVVM ViewModels
└── Views/             # WPF XAML views
```

## Coding Guidelines

### General Principles

- Follow SOLID principles
- Keep methods small and focused (single responsibility)
- Use meaningful names for variables, methods, and classes
- Avoid code duplication (DRY principle)
- Write self-documenting code

### C# Coding Standards

#### Naming Conventions

- **Classes**: PascalCase (e.g., `ServiceManager`)
- **Interfaces**: PascalCase with 'I' prefix (e.g., `IServiceManager`)
- **Methods**: PascalCase (e.g., `LoadServicesAsync`)
- **Properties**: PascalCase (e.g., `ServiceName`)
- **Private fields**: camelCase with underscore prefix (e.g., `_serviceManager`)
- **Local variables**: camelCase (e.g., `serviceName`)
- **Constants**: PascalCase (e.g., `MaxRetryCount`)

#### Code Style

```csharp
// ✅ Good
public async Task<ServiceOperationResult> StartServiceAsync(WindowsServiceModel service)
{
    if (service == null)
    {
        throw new ArgumentNullException(nameof(service));
    }

    _logger.LogInformation("Starting service {ServiceName}", service.ServiceName);
    return await _repository.StartServiceAsync(service.ServiceName);
}

// ❌ Avoid
public async Task<ServiceOperationResult> start(WindowsServiceModel s){
    return await _repository.StartServiceAsync(s.ServiceName);}
```

#### XML Documentation

All public types and members must have XML documentation:

```csharp
/// <summary>
/// Starts a Windows service with permission checks.
/// </summary>
/// <param name="service">The service to start.</param>
/// <returns>Result of the operation.</returns>
/// <exception cref="ArgumentNullException">Thrown when service is null.</exception>
public async Task<ServiceOperationResult> StartServiceAsync(WindowsServiceModel service)
{
    // Implementation
}
```

### MVVM Pattern Guidelines

- **ViewModels** should not reference Views directly
- **Views** should only contain UI logic, not business logic
- Use **Commands** for all user interactions
- Implement **INotifyPropertyChanged** for all bindable properties
- Use **Dependency Injection** for service dependencies

### Async/Await Guidelines

- Use `async`/`await` for all I/O operations
- Suffix async methods with `Async`
- Don't use `async void` except for event handlers
- Always configure `ConfigureAwait` appropriately

```csharp
// ✅ Good
public async Task<IEnumerable<WindowsServiceModel>> LoadServicesAsync()
{
    return await _repository.GetAllServicesAsync().ConfigureAwait(false);
}

// ❌ Avoid
public async void LoadServices()
{
    var services = _repository.GetAllServicesAsync().Result;
}
```

## Commit Message Guidelines

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: A new feature
- `fix`: A bug fix
- `docs`: Documentation only changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

### Examples

```
feat(services): add service dependency visualization

Implemented a tree view to show service dependencies
and dependent services for better understanding of
service relationships.

Closes #42
```

```
fix(ui): correct status color update on refresh

The service status color was not updating after refresh
due to missing property change notification.

Fixes #38
```

## Testing

### Writing Tests

- Write unit tests for all business logic
- Use descriptive test method names: `MethodName_Scenario_ExpectedBehavior`
- Follow the Arrange-Act-Assert pattern
- Mock external dependencies

```csharp
[Fact]
public async Task StartServiceAsync_WithNullService_ThrowsArgumentNullException()
{
    // Arrange
    var serviceManager = new ServiceManager(_mockRepository.Object, _mockDialog.Object, _mockLogger.Object);

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => 
        serviceManager.StartServiceAsync(null));
}
```

### Running Tests

```powershell
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## Questions?

Feel free to open an issue with your question or reach out to the maintainers.

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (MIT License).

---

Thank you for contributing to Windows Service Manager! 🎉
