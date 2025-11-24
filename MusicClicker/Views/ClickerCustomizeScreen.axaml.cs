/*
 * File: Views/ClickerCustomizeScreen.axaml.cs
 * Summary: Code-behind for clicker customization screen.
 * Purpose: Allows player to change the clicker image and manages unlock logic for options.
 */

using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicClicker.Helpers;

namespace MusicClicker.Views
{
    /// <summary>
    /// Screen that allows players to customize the clicker button image.
    /// Provides 16 clicker options that unlock based on major scores owned.
    /// </summary>
    public partial class ClickerCustomizeScreen : UserControl
    {
        // Reference to game state for checking unlock requirements
        private GameState? _gameState;
        
        // Reference to main window so we can change the clicker image
        private MainWindow? _mainWindow;

        /// <summary>
        /// Sets references to game state and main window.
        /// Called when screen becomes visible to ensure it can access game data.
        /// Also updates button states based on unlock requirements.
        /// </summary>
        public void SetGameState(GameState gameState, MainWindow mainWindow)
        {
            _gameState = gameState;
            _mainWindow = mainWindow;
            
            // Update which buttons are enabled/disabled based on player progress
            UpdateButtonStates();

            // Ensure the first option shows the packaged default clicker image
            try
            {
                string defaultUri = "avares://MusicClicker/Assets/Music Game Assets [A961E2A]-min.png";
                var bmp = ImageHelpers.GetBitmap(defaultUri, 128);
                if (bmp != null)
                {
                    ClickerOption1.Content = new Avalonia.Controls.Image
                    {
                        Source = bmp,
                        Stretch = Avalonia.Media.Stretch.Uniform
                    };
                }
            }
            catch
            {
                // ignore thumbnail failures
            }
        }
        
        /// <summary>
        /// Constructor initializes the clicker customization screen.
        /// </summary>
        public ClickerCustomizeScreen()
        {
            InitializeComponent();

            // Wire up back button to return to gallery screen
            BackButton.Click += BackButton_Click;
            
            // Set up click handlers for all 16 clicker option buttons
            InitializeClickerOptions();
        }

        /// <summary>
        /// Connects each clicker option button to the selection handler.
        /// Each button is linked to option number 1-16.
        /// </summary>
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

        /// <summary>
        /// Updates the enabled/disabled state and appearance of all clicker option buttons.
        /// Each option has different unlock requirements based on major scores owned.
        /// </summary>
        private void UpdateButtonStates()
        {
            if (_gameState == null)
                return;

            // Option 1: Always enabled (default clicker, no requirements)
            ClickerOption1.IsEnabled = true;
            ClickerOption1.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White);

            // Option 2: Requires ALL non-event major scores
            // (Excludes Dies Irae and Winter which are event-only)
            bool allNonEventMajors =
                _gameState.MoonlightMajorOwned > 0 &&
                _gameState.EroicaMajorOwned > 0 &&
                _gameState.SwanMajorOwned > 0 &&
                _gameState.LaCampanellaMajorOwned > 0 &&
                _gameState.EnigmaMajorOwned > 0 &&
                _gameState.FateMajorOwned > 0 &&
                _gameState.OdeToJoyMajorOwned > 0;
            ClickerOption2.IsEnabled = allNonEventMajors;
            ClickerOption2.Background = new Avalonia.Media.SolidColorBrush(
                allNonEventMajors ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 3: Requires Moonlight Sonata Major
            bool moonlight = _gameState.MoonlightMajorOwned > 0;
            ClickerOption3.IsEnabled = moonlight;
            ClickerOption3.Background = new Avalonia.Media.SolidColorBrush(
                moonlight ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 4: Requires Eroica Major
            bool eroica = _gameState.EroicaMajorOwned > 0;
            ClickerOption4.IsEnabled = eroica;
            ClickerOption4.Background = new Avalonia.Media.SolidColorBrush(
                eroica ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 5: Requires Swan Lake Major
            bool swan = _gameState.SwanMajorOwned > 0;
            ClickerOption5.IsEnabled = swan;
            ClickerOption5.Background = new Avalonia.Media.SolidColorBrush(
                swan ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 6: Requires La Campanella Major
            bool campanella = _gameState.LaCampanellaMajorOwned > 0;
            ClickerOption6.IsEnabled = campanella;
            ClickerOption6.Background = new Avalonia.Media.SolidColorBrush(
                campanella ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 7: Requires Enigma Major
            bool enigma = _gameState.EnigmaMajorOwned > 0;
            ClickerOption7.IsEnabled = enigma;
            ClickerOption7.Background = new Avalonia.Media.SolidColorBrush(
                enigma ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 8: Requires Fate Major
            bool fate = _gameState.FateMajorOwned > 0;
            ClickerOption8.IsEnabled = fate;
            ClickerOption8.Background = new Avalonia.Media.SolidColorBrush(
                fate ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 9: Requires Ode to Joy Major
            bool ode = _gameState.OdeToJoyMajorOwned > 0;
            ClickerOption9.IsEnabled = ode;
            ClickerOption9.Background = new Avalonia.Media.SolidColorBrush(
                ode ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 10: Requires Dies Irae Major (event score)
            bool dies = _gameState.DiesIraeMajorSheets > 0;
            ClickerOption10.IsEnabled = dies;
            ClickerOption10.Background = new Avalonia.Media.SolidColorBrush(
                dies ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Option 11: Requires Winter Major (event score)
            bool winter = _gameState.WinterMajorSheets > 0;
            ClickerOption11.IsEnabled = winter;
            ClickerOption11.Background = new Avalonia.Media.SolidColorBrush(
                winter ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Gray);

            // Options 12-16: Not yet implemented, disabled and grayed out
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

        /// <summary>
        /// Handles clicker image selection when player clicks an option button.
        /// Changes the main clicker button image to the selected option.
        /// </summary>
        private void HandleClickerSelection(int optionNumber)
        {
            // Validate we have access to required objects
            if (_mainWindow == null || _gameState == null)
                return;

            string imageUri = "";

            // Map option number to its corresponding clicker image URI
            switch (optionNumber)
            {
                case 1:
                    // Default clicker image (musical note)
                    imageUri = "avares://MusicClicker/Assets/Music Game Assets [A961E2A]-min.png";
                    break;
                case 2:
                    // Check if option 2 is enabled before allowing selection
                    if (!ClickerOption2.IsEnabled) return;
                    // Special all-majors clicker button (requires all non-event majors)
                    imageUri = "avares://MusicClicker/Assets/ALLMAJORClickerButton.png";
                    break;
                // TODO: Add cases for options 3-16 when additional clicker images are added
                default:
                    return; // Invalid option number
            }

            // Validate we got a valid image URI
            if (string.IsNullOrEmpty(imageUri))
                return;

            // Update the clicker button image
            if (_mainWindow?.ClickButton?.Content is Avalonia.Controls.Image clickerImage)
            {
                // Use the cached loader which avoids repeated decoding
                var bmp = ImageHelpers.GetBitmap(imageUri, 128);
                if (bmp != null)
                {
                    clickerImage.Source = bmp;
                    _gameState.CurrentClickerImage = imageUri;
                }
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
                var galleryScreen = parentWindow.FindControl<Views.SymphonicGalleryScreen>("SymphonicGalleryScreen");
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
                var galleryScreen = parentWindow.FindControl<Views.SymphonicGalleryScreen>("SymphonicGalleryScreen");
                if (galleryScreen != null)
                    galleryScreen.IsVisible = true;
            }
        }
    }
}