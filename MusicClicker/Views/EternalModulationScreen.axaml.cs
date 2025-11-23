using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    /// <summary>
    /// Eternal Modulation screen serves as a hub for special limited-time events.
    /// Players can access event-specific content like Dies Irae and Winter events from here.
    /// </summary>
    public partial class EternalModulationScreen : UserControl
    {
        /// <summary>
        /// Constructor initializes the event hub screen.
        /// </summary>
        public EternalModulationScreen()
        {
            InitializeComponent();

            // Wire up navigation buttons
            BackButton.Click += BackButton_Click;                     // Return to main screen
            DiesIraeBannerButton.Click += DiesIraeBannerButton_Click; // Navigate to Dies Irae event
            WinterBannerButton.Click += WinterBannerButton_Click;     // Navigate to Winter event
        }

        /// <summary>
        /// Handler for Dies Irae event banner button.
        /// Opens the Dies Irae event screen with current game state.
        /// </summary>
        private void DiesIraeBannerButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide the event hub screen
            this.IsVisible = false;

            // Navigate up the visual tree to find the parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                // Find the Dies Irae event screen control
                var diesIraeScreen = parentWindow.FindControl<DiesIraeEventScreen>("DiesIraeEventScreen");
                if (diesIraeScreen != null)
                {
                    // Get MainWindow to access GameState
                    // Need to pass game state to update event UI with player's progress
                    var mainWindow = parentWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        // Update the event screen with current game state
                        diesIraeScreen.UpdateUI(mainWindow.GameState);
                    }
                    
                    // Show the Dies Irae event screen
                    diesIraeScreen.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handler for Winter event banner button.
        /// Opens the Winter event screen with current game state.
        /// </summary>
        private void WinterBannerButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide the event hub screen
            this.IsVisible = false;

            // Navigate up the visual tree to find the parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                // Find the Winter event screen control
                var winterScreen = parentWindow.FindControl<WinterEventScreen>("WinterEventScreen");
                if (winterScreen != null)
                {
                    // Get MainWindow to access GameState
                    // Need to pass game state to update event UI with player's progress
                    var mainWindow = parentWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        // Update the event screen with current game state
                        winterScreen.UpdateUI(mainWindow.GameState);
                    }
                    
                    // Show the Winter event screen
                    winterScreen.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handler for back button - returns to main game screen.
        /// </summary>
        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide the event hub screen
            this.IsVisible = false;

            // Navigate up the visual tree to find the parent window
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