using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class LaCampanellaDuetScreen : UserControl
    {
        private MainWindow? _mainWindow;

        public LaCampanellaDuetScreen()
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
            // _mainWindow?.HideLaCampanellaDuetScreen();
            this.IsVisible = false;
        }
    }
}
