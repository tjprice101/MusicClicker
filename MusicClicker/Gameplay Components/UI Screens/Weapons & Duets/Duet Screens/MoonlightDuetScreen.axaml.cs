using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class MoonlightDuetScreen : UserControl
    {
        private MainWindow? _mainWindow;

        public MoonlightDuetScreen()
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
            // _mainWindow?.HideMoonlightDuetScreen();
            this.IsVisible = false;
        }
    }
}
