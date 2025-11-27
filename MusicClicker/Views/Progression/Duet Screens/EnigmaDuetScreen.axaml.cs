using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class EnigmaDuetScreen : UserControl
    {
        private MainWindow? _mainWindow;

        public EnigmaDuetScreen()
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
            // _mainWindow?.HideEnigmaDuetScreen();
            this.IsVisible = false;
        }
    }
}
