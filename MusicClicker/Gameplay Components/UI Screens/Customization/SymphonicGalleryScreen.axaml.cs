/*
 * File: Views/SymphonicGalleryScreen.axaml.cs
 * Summary: Code-behind for the Symphonic Gallery (customization hub).
 * Purpose: Shows customization options and links to clicker/background screens.
 */

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
        private async void ClickerCustomizeButton_Click(object? sender, RoutedEventArgs e)
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
                var clickerScreen = parentWindow.FindControl<ClickerCustomizeScreen>("ClickerCustomizeScreen");
                if (clickerScreen != null)
                {
                    clickerScreen.SetGameState(mw.GameState, mw);

                    // Use the main window transition to fade out, switch screens, fade in
                    await mw.TransitionAsync(() =>
                    {
                        this.IsVisible = false;
                        clickerScreen.IsVisible = true;
                    });
                }
            }
            else if (current is Window parentWindow)
            {
                var clickerScreen = parentWindow.FindControl<ClickerCustomizeScreen>("ClickerCustomizeScreen");
                if (clickerScreen != null)
                {
                    clickerScreen.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handler for Background Customize button.
        /// Opens the background customization screen where player can change background image.
        /// Passes game state and main window reference so screen can apply changes.
        /// </summary>
        private async void BackgroundCustomizeButton_Click(object? sender, RoutedEventArgs e)
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
                var backgroundScreen = parentWindow.FindControl<BackgroundCustomizeScreen>("BackgroundCustomizeScreen");
                if (backgroundScreen != null)
                {
                    backgroundScreen.SetGameState(mw.GameState, mw);

                    await mw.TransitionAsync(() =>
                    {
                        this.IsVisible = false;
                        backgroundScreen.IsVisible = true;
                    });
                }
            }
            else if (current is Window parentWindow)
            {
                var backgroundScreen = parentWindow.FindControl<BackgroundCustomizeScreen>("BackgroundCustomizeScreen");
                if (backgroundScreen != null)
                {
                    backgroundScreen.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handler for back button - returns to main game screen.
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
                var mainScreen = parentWindow.FindControl<Grid>("MainScreen");
                if (mainScreen != null)
                {
                    await mw.TransitionAsync(() =>
                    {
                        this.IsVisible = false;
                        mainScreen.IsVisible = true;
                    });
                }
                mw.RegenerateMajorScoreEffects();
            }
            else if (current is Window parentWindow)
            {
                var mainScreen = parentWindow.FindControl<Grid>("MainScreen");
                if (mainScreen != null)
                    mainScreen.IsVisible = true;
            }
        }
    }
}