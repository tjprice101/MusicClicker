/*
 * File: Views/BackgroundCustomizeScreen.axaml.cs
 * Summary: Code-behind for background customization screen.
 * Purpose: Lets players preview and select main window backgrounds; persists selection.
 */

using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicClicker.Helpers;

namespace MusicClicker.Views
{
    /// <summary>
    /// Screen that allows players to customize the background image of the main window.
    /// Provides 10 background options that can be unlocked and selected.
    /// </summary>
    public partial class BackgroundCustomizeScreen : UserControl
    {
        // Reference to game state for saving selected background
        private GameState? _gameState;
        
        // Reference to main window so we can change its background
        private MainWindow? _mainWindow;

        /// <summary>
        /// Constructor initializes the background customization screen.
        /// </summary>
        public BackgroundCustomizeScreen()
        {
            InitializeComponent();

            // Wire up back button to return to gallery screen
            BackButton.Click += BackButton_Click;

            // Set up click handlers for all 10 background option buttons
            InitializeBackgroundOptions();
        }

        /// <summary>
        /// Sets references to game state and main window.
        /// Called when screen becomes visible to ensure it can access game data.
        /// </summary>
        public void SetGameState(GameState gameState, MainWindow mainWindow)
        {
            _gameState = gameState;
            _mainWindow = mainWindow;
            
            // Load thumbnail previews for background options
            PopulateOptionThumbnails();
        }

        /// <summary>
        /// Connects each background option button to the selection handler.
        /// Each button is linked to option number 1-10.
        /// </summary>
        private void InitializeBackgroundOptions()
        {
            BackgroundOption1.Click += (s, e) => HandleBackgroundSelection(1);
            BackgroundOption2.Click += (s, e) => HandleBackgroundSelection(2);
            BackgroundOption3.Click += (s, e) => HandleBackgroundSelection(3);
            BackgroundOption4.Click += (s, e) => HandleBackgroundSelection(4);
            BackgroundOption5.Click += (s, e) => HandleBackgroundSelection(5);
            BackgroundOption6.Click += (s, e) => HandleBackgroundSelection(6);
            BackgroundOption7.Click += (s, e) => HandleBackgroundSelection(7);
            BackgroundOption8.Click += (s, e) => HandleBackgroundSelection(8);
            BackgroundOption9.Click += (s, e) => HandleBackgroundSelection(9);
            BackgroundOption10.Click += (s, e) => HandleBackgroundSelection(10);
            BackgroundOption11.Click += (s, e) => HandleBackgroundSelection(11);
            BackgroundOption12.Click += (s, e) => HandleBackgroundSelection(12);
        }

        /// <summary>
        /// Loads and displays thumbnail previews for background options.
        /// Also sets enabled/disabled states based on unlock requirements.
        /// </summary>
        private void PopulateOptionThumbnails()
        {
            if (_gameState == null) return;

            try
            {
                // Option 1: Default background (always unlocked)
                BackgroundOption1.IsEnabled = true;

                // Option 2: Custom background 1 (always unlocked)
                BackgroundOption2.IsEnabled = true;

                // Option 3: Custom background 2 (always unlocked)
                BackgroundOption3.IsEnabled = true;

                // Option 4: Moonlight Sonata (requires Moonlight Major)
                bool moonlight = _gameState.MoonlightMajorOwned > 0;
                BackgroundOption4.IsEnabled = moonlight;
                BackgroundOption4.Opacity = moonlight ? 1.0 : 0.5;

                // Option 5: Eroica (requires Eroica Major)
                bool eroica = _gameState.EroicaMajorOwned > 0;
                BackgroundOption5.IsEnabled = eroica;
                BackgroundOption5.Opacity = eroica ? 1.0 : 0.5;

                // Option 6: Swan Lake (requires Swan Lake Major)
                bool swan = _gameState.SwanMajorOwned > 0;
                BackgroundOption6.IsEnabled = swan;
                BackgroundOption6.Opacity = swan ? 1.0 : 0.5;

                // Option 7: La Campanella (requires La Campanella Major)
                bool campanella = _gameState.LaCampanellaMajorOwned > 0;
                BackgroundOption7.IsEnabled = campanella;
                BackgroundOption7.Opacity = campanella ? 1.0 : 0.5;

                // Option 8: Enigma (requires Enigma Major)
                bool enigma = _gameState.EnigmaMajorOwned > 0;
                BackgroundOption8.IsEnabled = enigma;
                BackgroundOption8.Opacity = enigma ? 1.0 : 0.5;

                // Option 9: Fate (requires Fate Major)
                bool fate = _gameState.FateMajorOwned > 0;
                BackgroundOption9.IsEnabled = fate;
                BackgroundOption9.Opacity = fate ? 1.0 : 0.5;

                // Option 10: Ode to Joy (requires Ode to Joy Major)
                bool ode = _gameState.OdeToJoyMajorOwned > 0;
                BackgroundOption10.IsEnabled = ode;
                BackgroundOption10.Opacity = ode ? 1.0 : 0.5;

                // Option 11: Dies Irae (requires Dies Irae owned)
                bool dies = _gameState.DiesIraeOwned > 0;
                BackgroundOption11.IsEnabled = dies;
                BackgroundOption11.Opacity = dies ? 1.0 : 0.5;

                // Option 12: Winter (requires Winter owned)
                bool winter = _gameState.WinterOwned > 0;
                BackgroundOption12.IsEnabled = winter;
                BackgroundOption12.Opacity = winter ? 1.0 : 0.5;
            }
            catch
            {
                // Silently ignore asset loading failures
                // Thumbnails are optional - buttons will still work without them
            }
        }

        /// <summary>
        /// Handles background selection when player clicks an option button.
        /// Changes the main window background to the selected image.
        /// </summary>
        private void HandleBackgroundSelection(int optionNumber)
        {
            // Validate we have access to required objects
            if (_mainWindow == null || _gameState == null)
                return;

            string imageUri = "";

            // Map option number to its corresponding background image URI
            switch (optionNumber)
            {
                case 1:
                    // Default main background (musical city lights) - reset to the packaged default
                    imageUri = "avares://MusicClicker/Assets/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png";
                    break;
                case 2:
                    // Custom background 1
                    imageUri = "avares://MusicClicker/Assets/CustomBG1.png";
                    break;
                case 3:
                    // Custom background 2
                    imageUri = "avares://MusicClicker/Assets/CustomBG2.png";
                    break;
                case 4:
                    // Moonlight Sonata background
                    imageUri = "avares://MusicClicker/Assets/MoonlightCUSTOMBG.jpg";
                    break;
                case 5:
                    // Eroica background
                    imageUri = "avares://MusicClicker/Assets/EroicaCUSTOMBG.jpg";
                    break;
                case 6:
                    // Swan Lake background
                    imageUri = "avares://MusicClicker/Assets/SwanLakeCUSTOMBG.jpg";
                    break;
                case 7:
                    // La Campanella background
                    imageUri = "avares://MusicClicker/Assets/LaCampanellaCUSTOMBG.jpg";
                    break;
                case 8:
                    // Enigma background
                    imageUri = "avares://MusicClicker/Assets/EnigmaCUSTOMBG.jpg";
                    break;
                case 9:
                    // Fate background
                    imageUri = "avares://MusicClicker/Assets/FateCUSTOMBG.jpg";
                    break;
                case 10:
                    // Ode to Joy background
                    imageUri = "avares://MusicClicker/Assets/OdeToJoyCUSTOMBG.jpg";
                    break;
                case 11:
                    // Dies Irae background
                    imageUri = "avares://MusicClicker/Assets/DiesIraeCUSTOMBG.jpg";
                    break;
                case 12:
                    // Winter background
                    imageUri = "avares://MusicClicker/Assets/WinterCUSTOMBG.jpg";
                    break;
                // TODO: Add cases for additional backgrounds if needed
                default:
                    return; // Invalid option number
            }

            // Validate we got a valid image URI
            if (string.IsNullOrEmpty(imageUri))
                return;

            try
            {
                var bmp = ImageHelpers.GetBitmap(imageUri, 1920);
                if (bmp != null)
                {
                    _mainWindow.Background = new Avalonia.Media.ImageBrush
                    {
                        Source = bmp,
                        Stretch = Avalonia.Media.Stretch.UniformToFill
                    };
                    _gameState.CurrentBackgroundImage = imageUri;
                }
            }
            catch
            {
                // Ignore failures
            }
        }

        /// <summary>
        /// Handler for back button - returns to Symphonic Gallery screen.
        /// </summary>
        private async void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Navigate up the visual tree to find parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is MainWindow mw)
            {
                var parentWindow = mw as Window;
                var galleryScreen = parentWindow.FindControl<SymphonicGalleryScreen>("SymphonicGalleryScreen");
                if (galleryScreen != null)
                {
                    await mw.TransitionAsync(() =>
                    {
                        this.IsVisible = false;
                        galleryScreen.IsVisible = true;
                    });
                }
            }
            else if (current is Window parentWindow)
            {
                var galleryScreen = parentWindow.FindControl<SymphonicGalleryScreen>("SymphonicGalleryScreen");
                if (galleryScreen != null)
                    galleryScreen.IsVisible = true;
            }
        }
    }
}