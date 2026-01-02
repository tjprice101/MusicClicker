/*
 * File: Views/EternalModulationScreen.axaml.cs
 * Summary: Code-behind for event screens and modulation features.
 * Purpose: Manages event banner navigation and event-specific UI.
 */

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
        private async void DiesIraeBannerButton_Click(object? sender, RoutedEventArgs e)
        {
            // Navigate up the visual tree to find the parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is MainWindow mw)
            {
                var parentWindow = mw as Window;
                var diesIraeScreen = parentWindow.FindControl<DiesIraeEventScreen>("DiesIraeEventScreen");
                if (diesIraeScreen != null)
                {
                    diesIraeScreen.UpdateUI(mw.GameState);
                    await mw.TransitionAsync(() =>
                    {
                        this.IsVisible = false;
                        diesIraeScreen.IsVisible = true;
                    });
                }
            }
            else if (current is Window parentWindow)
            {
                var diesIraeScreen = parentWindow.FindControl<DiesIraeEventScreen>("DiesIraeEventScreen");
                if (diesIraeScreen != null)
                    diesIraeScreen.IsVisible = true;
            }
        }

        /// <summary>
        /// Handler for Winter event banner button.
        /// Opens the Winter event screen with current game state.
        /// </summary>
        private async void WinterBannerButton_Click(object? sender, RoutedEventArgs e)
        {
            // Navigate up the visual tree to find the parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is MainWindow mw)
            {
                var parentWindow = mw as Window;
                var winterScreen = parentWindow.FindControl<WinterEventScreen>("WinterEventScreen");
                if (winterScreen != null)
                {
                    winterScreen.UpdateUI(mw.GameState);
                    await mw.TransitionAsync(() =>
                    {
                        this.IsVisible = false;
                        winterScreen.IsVisible = true;
                    });
                }
            }
            else if (current is Window parentWindow)
            {
                var winterScreen = parentWindow.FindControl<WinterEventScreen>("WinterEventScreen");
                if (winterScreen != null)
                    winterScreen.IsVisible = true;
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