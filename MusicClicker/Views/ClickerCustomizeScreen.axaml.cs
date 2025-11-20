using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class ClickerCustomizeScreen : UserControl
    {
        private GameState? _gameState;
        private MainWindow? _mainWindow;

        public void SetGameState(GameState gameState, MainWindow mainWindow)
        {
            _gameState = gameState;
            _mainWindow = mainWindow;
            UpdateButtonStates();
        }
        public ClickerCustomizeScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            
            // Initialize all clicker option buttons (placeholder for future functionality)
            InitializeClickerOptions();
        }

        private void InitializeClickerOptions()
        {
            ClickerOption1.Click += (s, e) => HandleClickerSelection(1);
            ClickerOption2.Click += (s, e) => HandleClickerSelection(2);
            ClickerOption3.Click += (s, e) => HandleClickerSelection(3);
            ClickerOption4.Click += (s, e) => HandleClickerSelection(4);
            ClickerOption5.Click += (s, e) => HandleClickerSelection(5);
            ClickerOption6.Click += (s, e) => HandleClickerSelection(6);
            ClickerOption7.Click += (s, e) => HandleClickerSelection(7);
            ClickerOption8.Click += (s, e) => HandleClickerSelection(8);
            ClickerOption9.Click += (s, e) => HandleClickerSelection(9);
            ClickerOption10.Click += (s, e) => HandleClickerSelection(10);
            ClickerOption11.Click += (s, e) => HandleClickerSelection(11);
            ClickerOption12.Click += (s, e) => HandleClickerSelection(12);
            ClickerOption13.Click += (s, e) => HandleClickerSelection(13);
            ClickerOption14.Click += (s, e) => HandleClickerSelection(14);
            ClickerOption15.Click += (s, e) => HandleClickerSelection(15);
            ClickerOption16.Click += (s, e) => HandleClickerSelection(16);
        }

        private void UpdateButtonStates()
        {
            if (_gameState == null)
                return;

            // Requirements for each button
            // 1: Always enabled (default)
            ClickerOption1.IsEnabled = true;
            ClickerOption1.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White);

            // 2: All non-event major scores (exclude Dies Irae and Winter)
            bool allNonEventMajors =
                _gameState.MoonlightMajorOwned > 0 &&
                _gameState.EroicaMajorOwned > 0 &&
                _gameState.SwanMajorOwned > 0 &&
                _gameState.LaCampanellaMajorOwned > 0 &&
                _gameState.EnigmaMajorOwned > 0 &&
                _gameState.FateMajorOwned > 0 &&
                _gameState.OdeToJoyMajorOwned > 0;
            ClickerOption2.IsEnabled = allNonEventMajors;
            ClickerOption2.Background = new Avalonia.Media.SolidColorBrush(allNonEventMajors ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 3: Moonlight
            bool moonlight = _gameState.MoonlightMajorOwned > 0;
            ClickerOption3.IsEnabled = moonlight;
            ClickerOption3.Background = new Avalonia.Media.SolidColorBrush(moonlight ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 4: Eroica
            bool eroica = _gameState.EroicaMajorOwned > 0;
            ClickerOption4.IsEnabled = eroica;
            ClickerOption4.Background = new Avalonia.Media.SolidColorBrush(eroica ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 5: Swan Lake
            bool swan = _gameState.SwanMajorOwned > 0;
            ClickerOption5.IsEnabled = swan;
            ClickerOption5.Background = new Avalonia.Media.SolidColorBrush(swan ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 6: La Campanella
            bool campanella = _gameState.LaCampanellaMajorOwned > 0;
            ClickerOption6.IsEnabled = campanella;
            ClickerOption6.Background = new Avalonia.Media.SolidColorBrush(campanella ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 7: Enigma
            bool enigma = _gameState.EnigmaMajorOwned > 0;
            ClickerOption7.IsEnabled = enigma;
            ClickerOption7.Background = new Avalonia.Media.SolidColorBrush(enigma ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 8: Fate
            bool fate = _gameState.FateMajorOwned > 0;
            ClickerOption8.IsEnabled = fate;
            ClickerOption8.Background = new Avalonia.Media.SolidColorBrush(fate ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 9: Ode to Joy
            bool ode = _gameState.OdeToJoyMajorOwned > 0;
            ClickerOption9.IsEnabled = ode;
            ClickerOption9.Background = new Avalonia.Media.SolidColorBrush(ode ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 10: Dies Irae Major
            bool dies = _gameState.DiesIraeMajorSheets > 0;
            ClickerOption10.IsEnabled = dies;
            ClickerOption10.Background = new Avalonia.Media.SolidColorBrush(dies ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 11: Winter Major
            bool winter = _gameState.WinterMajorSheets > 0;
            ClickerOption11.IsEnabled = winter;
            ClickerOption11.Background = new Avalonia.Media.SolidColorBrush(winter ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // 12-16: Custom requirements (disabled, gray)
            ClickerOption12.IsEnabled = false;
            ClickerOption12.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray);
            ClickerOption13.IsEnabled = false;
            ClickerOption13.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray);
            ClickerOption14.IsEnabled = false;
            ClickerOption14.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray);
            ClickerOption15.IsEnabled = false;
            ClickerOption15.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray);
            ClickerOption16.IsEnabled = false;
            ClickerOption16.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray);
        }

        private void HandleClickerSelection(int optionNumber)
        {
            if (_mainWindow == null)
                return;

            // Option 1: Reset to default image

            if (_mainWindow?.ClickButton?.Content is Avalonia.Controls.Image clickerImage)
            {
                if (optionNumber == 1)
                {
                    var uri = new System.Uri("avares://MusicClicker/Assets/Music Game Assets [A961E2A]-min.png");
                    if (Avalonia.Platform.AssetLoader.Exists(uri))
                    {
                        var assets = Avalonia.Platform.AssetLoader.Open(uri);
                        clickerImage.Source = new Avalonia.Media.Imaging.Bitmap(assets);
                    }
                    return;
                }
                if (optionNumber == 2 && ClickerOption2.IsEnabled)
                {
                    var uri = new System.Uri("avares://MusicClicker/Assets/EssenceOfElgar.png");
                    if (Avalonia.Platform.AssetLoader.Exists(uri))
                    {
                        var assets = Avalonia.Platform.AssetLoader.Open(uri);
                        clickerImage.Source = new Avalonia.Media.Imaging.Bitmap(assets);
                    }
                    return;
                }
            }
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show SymphonicGalleryScreen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var galleryScreen = parentWindow.FindControl<Views.SymphonicGalleryScreen>("SymphonicGalleryScreen");
                if (galleryScreen != null)
                    galleryScreen.IsVisible = true;
            }
        }
    }
}