# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial project documentation
- README.md with comprehensive project information
- CONTRIBUTING.md with development guidelines
- CHANGELOG.md for version tracking

## [1.0.0] - 2025-12-14

### Added
- Initial release of Windows Service Manager
- View all Windows services with detailed information
- Start, stop, restart, pause, and resume service operations
- Real-time service status monitoring
- Search and filter functionality for services
- Administrator privilege detection
- Critical service warnings and confirmations
- MVVM architecture with dependency injection
- Comprehensive error handling and logging
- Modern WPF user interface
- Service dependency information display
- Service process ID and executable path display
- Status color indicators for easy visual identification

### Features
- **Service Operations**
  - Start stopped services
  - Stop running services
  - Restart services
  - Pause and resume services (when supported)
  - Refresh service status

- **Safety Features**
  - Admin privilege requirements enforcement
  - Critical service operation warnings
  - User confirmation dialogs for dangerous operations
  - Detailed operation result messages

- **User Interface**
  - Clean, modern WPF design
  - Service list with search/filter
  - Detailed service information panel
  - Status indicators with color coding
  - Responsive layout

- **Technical Features**
  - MVVM pattern implementation
  - Repository pattern for data access
  - Dependency injection
  - Async/await throughout
  - Comprehensive XML documentation
  - Structured logging

[Unreleased]: https://github.com/raandree/AiCSharpTest/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/raandree/AiCSharpTest/releases/tag/v1.0.0
