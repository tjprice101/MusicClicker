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
    /// Provides 15 background options that can be unlocked and selected.
    /// </summary>
    public partial class BackgroundCustomizeScreen : UserControl
    {
        // Reference to game state for saving selected background
        private GameState? _gameState;
        
        // Reference to main window so we can change its background
        private MainWindow? _mainWindow;
        
        // Track which option is currently applied (0 = none/default)
        private int _currentAppliedOption = 0;

        /// <summary>
        /// Constructor initializes the background customization screen.
        /// </summary>
        public BackgroundCustomizeScreen()
        {
            InitializeComponent();

            // Wire up back button to return to gallery screen
            BackButton.Click += BackButton_Click;

            // Set up click handlers for all 15 background option buttons
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
        /// Each button is linked to option number 1-15.
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
            BackgroundOption13.Click += (s, e) => HandleBackgroundSelection(13);
            BackgroundOption14.Click += (s, e) => HandleBackgroundSelection(14);
            BackgroundOption15.Click += (s, e) => HandleBackgroundSelection(15);
        }
        
        /// <summary>
        /// Gets the display name for a major score option
        /// </summary>
        private string GetScoreDisplayName(int option)
        {
            return option switch
            {
                4 => "Moonlight Sonata",
                5 => "Eroica",
                6 => "Swan Lake",
                7 => "La Campanella",
                8 => "Enigma Variations",
                9 => "Fate",
                10 => "Ode to Joy",
                11 => "Dies Irae",
                12 => "Winter",
                13 => "Mercury",
                14 => "Clair de Lune",
                15 => "Mars",
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
                1 => BackgroundText1,
                2 => BackgroundText2,
                3 => BackgroundText3,
                4 => BackgroundText4,
                5 => BackgroundText5,
                6 => BackgroundText6,
                7 => BackgroundText7,
                8 => BackgroundText8,
                9 => BackgroundText9,
                10 => BackgroundText10,
                11 => BackgroundText11,
                12 => BackgroundText12,
                13 => BackgroundText13,
                14 => BackgroundText14,
                15 => BackgroundText15,
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
                textBlock.Text = isApplied ? "Applied!" : "Custom Background 1";
            }
            else if (option == 3)
            {
                textBlock.Text = isApplied ? "Applied!" : "Custom Background 2";
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
                BackgroundOption1.Opacity = 1.0;
                UpdateOptionText(1, true, _currentAppliedOption == 1);

                // Option 2: Custom background 1 (always unlocked)
                BackgroundOption2.IsEnabled = true;
                BackgroundOption2.Opacity = 1.0;
                UpdateOptionText(2, true, _currentAppliedOption == 2);

                // Option 3: Custom background 2 (always unlocked)
                BackgroundOption3.IsEnabled = true;
                BackgroundOption3.Opacity = 1.0;
                UpdateOptionText(3, true, _currentAppliedOption == 3);

                // Option 4: Moonlight Sonata (requires Moonlight Major)
                bool moonlight = _gameState.MoonlightMajorOwned > 0;
                BackgroundOption4.IsEnabled = moonlight;
                BackgroundOption4.Opacity = moonlight ? 1.0 : 0.4;
                UpdateOptionText(4, moonlight, _currentAppliedOption == 4);

                // Option 5: Eroica (requires Eroica Major)
                bool eroica = _gameState.EroicaMajorOwned > 0;
                BackgroundOption5.IsEnabled = eroica;
                BackgroundOption5.Opacity = eroica ? 1.0 : 0.4;
                UpdateOptionText(5, eroica, _currentAppliedOption == 5);

                // Option 6: Swan Lake (requires Swan Lake Major)
                bool swan = _gameState.SwanMajorOwned > 0;
                BackgroundOption6.IsEnabled = swan;
                BackgroundOption6.Opacity = swan ? 1.0 : 0.4;
                UpdateOptionText(6, swan, _currentAppliedOption == 6);

                // Option 7: La Campanella (requires La Campanella Major)
                bool campanella = _gameState.LaCampanellaMajorOwned > 0;
                BackgroundOption7.IsEnabled = campanella;
                BackgroundOption7.Opacity = campanella ? 1.0 : 0.4;
                UpdateOptionText(7, campanella, _currentAppliedOption == 7);

                // Option 8: Enigma (requires Enigma Major)
                bool enigma = _gameState.EnigmaMajorOwned > 0;
                BackgroundOption8.IsEnabled = enigma;
                BackgroundOption8.Opacity = enigma ? 1.0 : 0.4;
                UpdateOptionText(8, enigma, _currentAppliedOption == 8);

                // Option 9: Fate (requires Fate Major)
                bool fate = _gameState.FateMajorOwned > 0;
                BackgroundOption9.IsEnabled = fate;
                BackgroundOption9.Opacity = fate ? 1.0 : 0.4;
                UpdateOptionText(9, fate, _currentAppliedOption == 9);

                // Option 10: Ode to Joy (requires Ode to Joy Major)
                bool ode = _gameState.OdeToJoyMajorOwned > 0;
                BackgroundOption10.IsEnabled = ode;
                BackgroundOption10.Opacity = ode ? 1.0 : 0.4;
                UpdateOptionText(10, ode, _currentAppliedOption == 10);

                // Option 11: Dies Irae (requires Dies Irae owned)
                bool dies = _gameState.DiesIraeOwned > 0;
                BackgroundOption11.IsEnabled = dies;
                BackgroundOption11.Opacity = dies ? 1.0 : 0.4;
                UpdateOptionText(11, dies, _currentAppliedOption == 11);

                // Option 12: Winter (requires Winter owned)
                bool winter = _gameState.WinterOwned > 0;
                BackgroundOption12.IsEnabled = winter;
                BackgroundOption12.Opacity = winter ? 1.0 : 0.4;
                UpdateOptionText(12, winter, _currentAppliedOption == 12);

                // Option 13: Mercury (requires Mercury Major)
                bool mercury = _gameState.MercuryMajorOwned > 0;
                BackgroundOption13.IsEnabled = mercury;
                BackgroundOption13.Opacity = mercury ? 1.0 : 0.4;
                UpdateOptionText(13, mercury, _currentAppliedOption == 13);

                // Option 14: Clair de Lune (requires Clair de Lune Major)
                bool clairDeLune = _gameState.ClairDeLuneMajorOwned > 0;
                BackgroundOption14.IsEnabled = clairDeLune;
                BackgroundOption14.Opacity = clairDeLune ? 1.0 : 0.4;
                UpdateOptionText(14, clairDeLune, _currentAppliedOption == 14);

                // Option 15: Mars (requires Mars Major)
                bool mars = _gameState.MarsMajorOwned > 0;
                BackgroundOption15.IsEnabled = mars;
                BackgroundOption15.Opacity = mars ? 1.0 : 0.4;
                UpdateOptionText(15, mars, _currentAppliedOption == 15);
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
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Backgrounds & Generic/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png";
                    break;
                case 2:
                    // Custom background 1
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Backgrounds & Generic/CustomBG1.png";
                    break;
                case 3:
                    // Custom background 2
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Backgrounds & Generic/CustomBG2.png";
                    break;
                case 4:
                    // Moonlight Sonata background
                    if (!BackgroundOption4.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/MoonlightCUSTOMBG.jpg";
                    break;
                case 5:
                    // Eroica background
                    if (!BackgroundOption5.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/EroicaCUSTOMBG.jpg";
                    break;
                case 6:
                    // Swan Lake background
                    if (!BackgroundOption6.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/SwanLakeCUSTOMBG.jpg";
                    break;
                case 7:
                    // La Campanella background
                    if (!BackgroundOption7.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/LaCampanellaCUSTOMBG.jpg";
                    break;
                case 8:
                    // Enigma background
                    if (!BackgroundOption8.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/EnigmaCUSTOMBG.jpg";
                    break;
                case 9:
                    // Fate background
                    if (!BackgroundOption9.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/FateCUSTOMBG.jpg";
                    break;
                case 10:
                    // Ode to Joy background
                    if (!BackgroundOption10.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/OdeToJoyCUSTOMBG.jpg";
                    break;
                case 11:
                    // Dies Irae background
                    if (!BackgroundOption11.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/DiesIraeCUSTOMBG.jpg";
                    break;
                case 12:
                    // Winter background
                    if (!BackgroundOption12.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/WinterCUSTOMBG.jpg";
                    break;
                case 13:
                    // Mercury background
                    if (!BackgroundOption13.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/MercuryCustomBG.jpg";
                    break;
                case 14:
                    // Clair de Lune background
                    if (!BackgroundOption14.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/ClairDeLuneCustomBG.jpg";
                    break;
                case 15:
                    // Mars background
                    if (!BackgroundOption15.IsEnabled) return;
                    imageUri = "avares://MusicClicker/Gameplay Components/Resources/Assets/Customization Backgrounds/MarsCustomBG.jpg";
                    break;
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
                    
                    // Track applied option and update text states
                    _currentAppliedOption = optionNumber;
                    PopulateOptionThumbnails();
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