using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WindowsServiceManager.Services;
using WindowsServiceManager.ViewModels;
using WindowsServiceManager.Views;

namespace WindowsServiceManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // Configure dependency injection
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Create and show main window
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Application startup failed:\n\n{ex.Message}\n\n{ex.StackTrace}",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            Shutdown(1);
        }
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // Logging
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.AddDebug();
            configure.SetMinimumLevel(LogLevel.Information);
        });

        // Services
        services.AddSingleton<IServiceRepository, WindowsServiceRepository>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IServiceManager, ServiceManager>();

        // ViewModels
        services.AddTransient<MainViewModel>();

        // Views
        services.AddTransient<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
