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
        }

        /// <summary>
        /// Loads and displays thumbnail previews for background options.
        /// Currently shows previews for options 1 and 2.
        /// </summary>
        private void PopulateOptionThumbnails()
        {
            try
            {
                // Option 1: show currently selected/default background from game state if available
                string defaultUri = _gameState != null && !string.IsNullOrEmpty(_gameState.CurrentBackgroundImage)
                    ? _gameState.CurrentBackgroundImage
                    : "avares://MusicClicker/Assets/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png";

                var defaultBmp = ImageHelpers.GetBitmap(defaultUri, 256);
                if (defaultBmp != null)
                {
                    BackgroundOption1.Content = new Image
                    {
                        Source = defaultBmp,
                        Stretch = Avalonia.Media.Stretch.UniformToFill
                    };
                }

                // Option 2: custom background 1
                var custom1Uri = "avares://MusicClicker/Assets/CustomBG1.png";
                var custom1Bmp = ImageHelpers.GetBitmap(custom1Uri, 256);
                if (custom1Bmp != null)
                {
                    BackgroundOption2.Content = new Image
                    {
                        Source = custom1Bmp,
                        Stretch = Avalonia.Media.Stretch.UniformToFill
                    };
                }

                // Option 3: custom background 2
                var custom2Uri = "avares://MusicClicker/Assets/CustomBG2.png";
                var custom2Bmp = ImageHelpers.GetBitmap(custom2Uri, 256);
                if (custom2Bmp != null)
                {
                    BackgroundOption3.Content = new Image
                    {
                        Source = custom2Bmp,
                        Stretch = Avalonia.Media.Stretch.UniformToFill
                    };
                }
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
                    // Default main background (musical city lights)
                    imageUri = _gameState != null && !string.IsNullOrEmpty(_gameState.CurrentBackgroundImage)
                        ? _gameState.CurrentBackgroundImage
                        : "avares://MusicClicker/Assets/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png";
                    break;
                case 2:
                    // Custom background 1
                    imageUri = "avares://MusicClicker/Assets/CustomBG1.png";
                    break;
                case 3:
                    // Custom background 2
                    imageUri = "avares://MusicClicker/Assets/CustomBG2.png";
                    break;
                // TODO: Add cases for options 3-10 when additional backgrounds are added
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