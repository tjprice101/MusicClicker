using Avalonia.Controls;
using Avalonia.Interactivity;

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
                // Load option 1: Default main background used in MainWindow.axaml
                var defaultUri = new System.Uri("avares://MusicClicker/Assets/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png");
                using var ds = Avalonia.Platform.AssetLoader.Open(defaultUri);
                var defaultBmp = new Avalonia.Media.Imaging.Bitmap(ds);
                
                // Set thumbnail as button content
                BackgroundOption1.Content = new Image 
                { 
                    Source = defaultBmp, 
                    Stretch = Avalonia.Media.Stretch.UniformToFill 
                };

                // Load option 2: Customization screen background asset
                var customUri = new System.Uri("avares://MusicClicker/Assets/CustomizeScreenBackground.png");
                using var cs = Avalonia.Platform.AssetLoader.Open(customUri);
                var customBmp = new Avalonia.Media.Imaging.Bitmap(cs);
                
                // Set thumbnail as button content
                BackgroundOption2.Content = new Image 
                { 
                    Source = customBmp, 
                    Stretch = Avalonia.Media.Stretch.UniformToFill 
                };
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
                    imageUri = "avares://MusicClicker/Assets/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png";
                    break;
                case 2:
                    // Customization screen background
                    imageUri = "avares://MusicClicker/Assets/CustomizeScreenBackground.png";
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
                // Load the background image from application assets
                var uri = new System.Uri(imageUri);
                using var s = Avalonia.Platform.AssetLoader.Open(uri);
                var bmp = new Avalonia.Media.Imaging.Bitmap(s);
                
                // Apply the image as the main window's background
                // Uses ImageBrush with UniformToFill to cover entire window
                _mainWindow.Background = new Avalonia.Media.ImageBrush 
                { 
                    Source = bmp, 
                    Stretch = Avalonia.Media.Stretch.UniformToFill 
                };
                
                // Save the selected background URI to GameState
                // This ensures it persists when game is saved/loaded
                _gameState.CurrentBackgroundImage = imageUri;
            }
            catch
            {
                // Silently ignore failures to change background
                // Could happen if asset doesn't exist or load fails
            }
        }

        /// <summary>
        /// Handler for back button - returns to Symphonic Gallery screen.
        /// </summary>
        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this customization screen
            this.IsVisible = false;

            // Navigate up the visual tree to find parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            // Show the Symphonic Gallery screen (parent customization hub)
            if (current is Window parentWindow)
            {
                var galleryScreen = parentWindow.FindControl<SymphonicGalleryScreen>("SymphonicGalleryScreen");
                if (galleryScreen != null)
                    galleryScreen.IsVisible = true;
            }
        }
    }
}