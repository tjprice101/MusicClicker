using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class SwanLakeDuetScreen : UserControl
    {
        private MainWindow? _mainWindow;

        public SwanLakeDuetScreen()
        {
            InitializeComponent();
            CloseButton.Click += OnCloseClicked;
        }

        public void Initialize(MainWindow mainWindow, GameState gameState)
        {
            _mainWindow = mainWindow;
        }

        private void OnCloseClicked(object? sender, RoutedEventArgs e)
        {
            // Screen removed from UI - no longer needed
            // _mainWindow?.HideSwanLakeDuetScreen();
            this.IsVisible = false;
        }
    }
}
