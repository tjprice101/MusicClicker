using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class WinterDuetScreen : UserControl
    {
        private MainWindow? _mainWindow;

        public WinterDuetScreen()
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
            // _mainWindow?.HideWinterDuetScreen();
            this.IsVisible = false;
        }
    }
}
