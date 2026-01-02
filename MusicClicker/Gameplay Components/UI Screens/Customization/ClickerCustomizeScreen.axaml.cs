/*
 * File: Views/ClickerCustomizeScreen.axaml.cs
 * Summary: Code-behind for clicker customization screen.
 * Purpose: Allows player to change the clicker image and manages unlock logic for options.
 */

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MusicClicker.Helpers;

namespace MusicClicker.Views
{
    /// <summary>
    /// Screen that allows players to customize the clicker button image.
    /// Provides 14 clicker options that unlock based on major scores owned.
    /// </summary>
    public partial class ClickerCustomizeScreen : UserControl
    {
        // Reference to game state for checking unlock requirements
        private GameState? _gameState;
        
        // Reference to main window so we can change the clicker image
        private MainWindow? _mainWindow;
        
        // Track which option is currently applied (0 = none/default)
        private int _currentAppliedOption = 0;

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
                string defaultUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Backgrounds & Generic/Music Game Assets [A961E2A]-min.png";
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
        /// Each button is linked to option number 1-14.
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
        }
        
        /// <summary>
        /// Gets the display name for a major score option
        /// </summary>
        private string GetScoreDisplayName(int option)
        {
            return option switch
            {
                3 => "Moonlight Sonata",
                4 => "Eroica",
                5 => "Swan Lake",
                6 => "La Campanella",
                7 => "Enigma Variations",
                8 => "Fate",
                9 => "Ode to Joy",
                10 => "Dies Irae",
                11 => "Winter",
                12 => "Mercury",
                13 => "Clair de Lune",
                14 => "Mars",
                _ => ""
            };
        }
        
        /// <summary>
        /// Gets the TextBlock for a given option number
        /// </summary>
        private TextBlock? GetTextBlockForOption(int option)
        {
            return option switch
            {
                1 => ClickerText1,
                2 => ClickerText2,
                3 => ClickerText3,
                4 => ClickerText4,
                5 => ClickerText5,
                6 => ClickerText6,
                7 => ClickerText7,
                8 => ClickerText8,
                9 => ClickerText9,
                10 => ClickerText10,
                11 => ClickerText11,
                12 => ClickerText12,
                13 => ClickerText13,
                14 => ClickerText14,
                _ => null
            };
        }
        
        /// <summary>
        /// Updates text for an option based on ownership and applied state
        /// </summary>
        private void UpdateOptionText(int option, bool isOwned, bool isApplied)
        {
            var textBlock = GetTextBlockForOption(option);
            if (textBlock == null) return;
            
            string scoreName = GetScoreDisplayName(option);
            
            if (option == 1)
            {
                textBlock.Text = isApplied ? "Applied!" : "(Click to restore to default)";
            }
            else if (option == 2)
            {
                textBlock.Text = isApplied ? "Applied!" : (isOwned ? "All Majors Owned! Click to Apply" : "Requires all Non-Event Major Scores Owned");
            }
            else if (isOwned)
            {
                textBlock.Text = isApplied ? "Applied!" : $"{scoreName} Owned! Click to Apply";
            }
            else
            {
                textBlock.Text = $"Requires {scoreName} Major Owned";
            }
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
            ClickerOption1.Opacity = 1.0;
            UpdateOptionText(1, true, _currentAppliedOption == 1);

            // Option 2: Requires ALL non-event major scores
            bool allNonEventMajors =
                _gameState.MoonlightMajorOwned > 0 &&
                _gameState.EroicaMajorOwned > 0 &&
                _gameState.SwanMajorOwned > 0 &&
                _gameState.LaCampanellaMajorOwned > 0 &&
                _gameState.EnigmaMajorOwned > 0 &&
                _gameState.FateMajorOwned > 0 &&
                _gameState.OdeToJoyMajorOwned > 0;
            ClickerOption2.IsEnabled = allNonEventMajors;
            ClickerOption2.Opacity = allNonEventMajors ? 1.0 : 0.4;
            UpdateOptionText(2, allNonEventMajors, _currentAppliedOption == 2);

            // Option 3: Requires Moonlight Sonata Major
            bool moonlight = _gameState.MoonlightMajorOwned > 0;
            ClickerOption3.IsEnabled = moonlight;
            ClickerOption3.Opacity = moonlight ? 1.0 : 0.4;
            UpdateOptionText(3, moonlight, _currentAppliedOption == 3);

            // Option 4: Requires Eroica Major
            bool eroica = _gameState.EroicaMajorOwned > 0;
            ClickerOption4.IsEnabled = eroica;
            ClickerOption4.Opacity = eroica ? 1.0 : 0.4;
            UpdateOptionText(4, eroica, _currentAppliedOption == 4);

            // Option 5: Requires Swan Lake Major
            bool swan = _gameState.SwanMajorOwned > 0;
            ClickerOption5.IsEnabled = swan;
            ClickerOption5.Opacity = swan ? 1.0 : 0.4;
            UpdateOptionText(5, swan, _currentAppliedOption == 5);

            // Option 6: Requires La Campanella Major
            bool campanella = _gameState.LaCampanellaMajorOwned > 0;
            ClickerOption6.IsEnabled = campanella;
            ClickerOption6.Opacity = campanella ? 1.0 : 0.4;
            UpdateOptionText(6, campanella, _currentAppliedOption == 6);

            // Option 7: Requires Enigma Major
            bool enigma = _gameState.EnigmaMajorOwned > 0;
            ClickerOption7.IsEnabled = enigma;
            ClickerOption7.Opacity = enigma ? 1.0 : 0.4;
            UpdateOptionText(7, enigma, _currentAppliedOption == 7);

            // Option 8: Requires Fate Major
            bool fate = _gameState.FateMajorOwned > 0;
            ClickerOption8.IsEnabled = fate;
            ClickerOption8.Opacity = fate ? 1.0 : 0.4;
            UpdateOptionText(8, fate, _currentAppliedOption == 8);

            // Option 9: Requires Ode to Joy Major
            bool ode = _gameState.OdeToJoyMajorOwned > 0;
            ClickerOption9.IsEnabled = ode;
            ClickerOption9.Opacity = ode ? 1.0 : 0.4;
            UpdateOptionText(9, ode, _currentAppliedOption == 9);

            // Option 10: Requires Dies Irae Major (event score)
            bool dies = _gameState.DiesIraeOwned > 0;
            ClickerOption10.IsEnabled = dies;
            ClickerOption10.Opacity = dies ? 1.0 : 0.4;
            UpdateOptionText(10, dies, _currentAppliedOption == 10);

            // Option 11: Requires Winter Major (event score)
            bool winter = _gameState.WinterOwned > 0;
            ClickerOption11.IsEnabled = winter;
            ClickerOption11.Opacity = winter ? 1.0 : 0.4;
            UpdateOptionText(11, winter, _currentAppliedOption == 11);

            // Option 12: Requires Mercury Major
            bool mercury = _gameState.MercuryMajorOwned > 0;
            ClickerOption12.IsEnabled = mercury;
            ClickerOption12.Opacity = mercury ? 1.0 : 0.4;
            UpdateOptionText(12, mercury, _currentAppliedOption == 12);

            // Option 13: Requires Clair de Lune Major
            bool clairDeLune = _gameState.ClairDeLuneMajorOwned > 0;
            ClickerOption13.IsEnabled = clairDeLune;
            ClickerOption13.Opacity = clairDeLune ? 1.0 : 0.4;
            UpdateOptionText(13, clairDeLune, _currentAppliedOption == 13);

            // Option 14: Requires Mars Major
            bool mars = _gameState.MarsMajorOwned > 0;
            ClickerOption14.IsEnabled = mars;
            ClickerOption14.Opacity = mars ? 1.0 : 0.4;
            UpdateOptionText(14, mars, _currentAppliedOption == 14);
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
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Backgrounds & Generic/Music Game Assets [A961E2A]-min.png";
                    break;
                case 2:
                    // Check if option 2 is enabled before allowing selection
                    if (!ClickerOption2.IsEnabled) return;
                    // Special all-majors clicker button (requires all non-event majors)
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/UI Buttons & Menus/ALLMAJORClickerButton.png";
                    break;
                case 3:
                    if (!ClickerOption3.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/MoonlightIcon.jpg";
                    break;
                case 4:
                    if (!ClickerOption4.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/EroicaIcon.jpg";
                    break;
                case 5:
                    if (!ClickerOption5.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/SwanLakeIcon.jpg";
                    break;
                case 6:
                    if (!ClickerOption6.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/LaCampanellaIcon.jpg";
                    break;
                case 7:
                    if (!ClickerOption7.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/EnigmaIcon.jpg";
                    break;
                case 8:
                    if (!ClickerOption8.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/FateIcon.jpg";
                    break;
                case 9:
                    if (!ClickerOption9.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/OdeToJoyIcon.png";
                    break;
                case 10:
                    if (!ClickerOption10.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/DiesIraeIcon.jpg";
                    break;
                case 11:
                    if (!ClickerOption11.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/WinterIcon.jpg";
                    break;
                case 12:
                    if (!ClickerOption12.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/MercuryClickerImage.jpg";
                    break;
                case 13:
                    if (!ClickerOption13.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/ClairDeLuneClickerImage.jpg";
                    break;
                case 14:
                    if (!ClickerOption14.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Score Icons/MarsClickerImage.jpg";
                    break;
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
                    
                    // Track applied option and update text states
                    _currentAppliedOption = optionNumber;
                    UpdateButtonStates();
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