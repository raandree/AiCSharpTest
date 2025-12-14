using System.Windows;
using System.Windows.Controls;
using WindowsServiceManager.ViewModels;

namespace WindowsServiceManager.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        // Load services after window is loaded
        Loaded += async (sender, args) =>
        {
            try
            {
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load services:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Error Loading Services",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        };
    }

    private async void StartupType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item)
        {
            string? newStartupType = item.Content?.ToString();
            if (!string.IsNullOrEmpty(newStartupType))
            {
                await _viewModel.ChangeStartupTypeAsync(newStartupType);
            }
        }
    }
}
