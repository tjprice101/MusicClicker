using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    /// <summary>
    /// Symphonic Gallery Screen serves as the main customization hub.
    /// From here, players can access sub-screens to customize:
    /// - Clicker button appearance
    /// - Background image
    /// This is the central location for all visual personalization options.
    /// </summary>
    public partial class SymphonicGalleryScreen : UserControl
    {
        /// <summary>
        /// Constructor initializes the gallery hub screen.
        /// </summary>
        public SymphonicGalleryScreen()
        {
            InitializeComponent();

            // Wire up navigation buttons
            BackButton.Click += BackButton_Click;                             // Return to main screen
            ClickerCustomizeButton.Click += ClickerCustomizeButton_Click;     // Open clicker customization
            BackgroundCustomizeButton.Click += BackgroundCustomizeButton_Click; // Open background customization
        }

        /// <summary>
        /// Handler for Clicker Customize button.
        /// Opens the clicker customization screen where player can change clicker image.
        /// Passes game state and main window reference so screen can check unlock requirements.
        /// </summary>
        private void ClickerCustomizeButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this gallery hub screen
            this.IsVisible = false;

            // Navigate up the visual tree to find parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                // Find the ClickerCustomizeScreen control
                var clickerScreen = parentWindow.FindControl<ClickerCustomizeScreen>("ClickerCustomizeScreen");
                if (clickerScreen != null)
                {
                    // Provide the game state and main window reference
                    // This allows customize screen to:
                    // 1. Check which clickers are unlocked based on majors owned
                    // 2. Actually change the clicker image on the main window
                    if (parentWindow is MainWindow mw)
                    {
                        clickerScreen.SetGameState(mw.GameState, mw);
                    }
                    
                    // Show the clicker customization screen
                    clickerScreen.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handler for Background Customize button.
        /// Opens the background customization screen where player can change background image.
        /// Passes game state and main window reference so screen can apply changes.
        /// </summary>
        private void BackgroundCustomizeButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this gallery hub screen
            this.IsVisible = false;

            // Navigate up the visual tree to find parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                // Find the BackgroundCustomizeScreen control
                var backgroundScreen = parentWindow.FindControl<BackgroundCustomizeScreen>("BackgroundCustomizeScreen");
                if (backgroundScreen != null)
                {
                    // Provide the game state and main window reference
                    // This allows customize screen to:
                    // 1. Save the selected background to game state (for persistence)
                    // 2. Actually change the main window's background image
                    if (parentWindow is MainWindow mw)
                    {
                        backgroundScreen.SetGameState(mw.GameState, mw);
                    }
                    
                    // Show the background customization screen
                    backgroundScreen.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handler for back button - returns to main game screen.
        /// </summary>
        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this gallery hub screen
            this.IsVisible = false;

            // Navigate up the visual tree to find parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                // Find and show the main game screen
                var mainScreen = parentWindow.FindControl<Grid>("MainScreen");
                if (mainScreen != null)
                    mainScreen.IsVisible = true;
            }
        }
    }
}