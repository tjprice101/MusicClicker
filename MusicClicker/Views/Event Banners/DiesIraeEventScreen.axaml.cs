using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MusicClicker.Views
{
    /// <summary>
    /// Dies Irae Event Screen manages a limited-time event where players can:
    /// 1. Purchase "Soul Resonance" attempts to win Major Sheets (1 in 5 chance)
    /// 2. Convert Major Sheets into the actual Dies Irae Major score (1 in 10,000 chance)
    /// This is a high-stakes gambling system for event-exclusive content.
    /// </summary>
    public partial class DiesIraeEventScreen : UserControl
    {
        // Reference to game state for checking/updating player progress
        private GameState? _gameState;

        /// <summary>
        /// Sets the game state reference and immediately updates the UI.
        /// Called from external code when the screen needs to be refreshed.
        /// </summary>
        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
            UpdateUI(_gameState);
        }
        
        /// <summary>
        /// Constructor initializes the Dies Irae event screen.
        /// </summary>
        public DiesIraeEventScreen()
        {
            InitializeComponent();

            // Wire up button handlers
            BackButton.Click += BackButton_Click;                         // Return to event hub
            SoulOfDiesIraeButton.Click += SoulOfDiesIraeButton_Click;     // Purchase Soul Resonance attempt
            DiesIraeMajorButton.Click += DiesIraeMajorButton_Click;       // Convert sheet to major
        }

        /// <summary>
        /// Updates all UI elements with current game state values.
        /// Shows costs, owned counts, and current player resources.
        /// </summary>
        public void UpdateUI(GameState gameState)
        {
            // Ensure internal reference is set so click handlers can access game state
            _gameState = gameState;
            
            // Calculate Soul Resonance cost: (NPS * 5) + 250
            // This is a base cost formula - actual purchase cost is different (see click handler)
            double cost = (gameState.NotesPerSecond * 5) + 250;
            SoulOfDiesIraeCostText.Text = $"{Math.Round(cost, 1)} Notes for Soul Resonance";

            // Display how many Major Sheets player currently owns
            // These are the intermediate currency needed to attempt the Major conversion
            DiesIraeMajorOwnedText.Text = $"Major Sheets of Dies Irae Owned: {gameState.DiesIraeMajorSheets}";

            // Update current Notes and NPS display in the main info panel
            if (this.FindControl<TextBlock>("NotesText") is TextBlock notes)
            {
                notes.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            }
            if (this.FindControl<TextBlock>("NpsText") is TextBlock nps)
            {
                nps.Text = $"Notes Per Second: {Math.Round(gameState.NotesPerSecond, 1)}";
            }

            // Update right-side panel showing Major score info
            try
            {
                // Show how many actual Dies Irae Major scores are owned (the final prize)
                DiesIraeMajorOwnedRightText.Text = $"Number Owned: {gameState.DiesIraeOwned}";
                
                // Placeholder for Crescendance ability (special power granted by this major)
                DiesIraeCrescendanceText.Text = "Crescendance Ability: <placeholder>";
            }
            catch (Exception)
            {
                // If controls not loaded yet, silently ignore
                // UpdateUI will be called again when screen becomes visible
            }
        }

        /// <summary>
        /// Handler for Soul of Dies Irae button - purchases an attempt to win a Major Sheet.
        /// Cost formula: ((NPS * 5) * NotesPerClick) + 250
        /// Success rate: 1 in 5 (20% chance)
        /// </summary>
        private void SoulOfDiesIraeButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            
            // Calculate actual purchase cost (different from display formula)
            // Multiplies by NotesPerClick to scale with player power
            double cost = ((_gameState.NotesPerSecond * 5) * _gameState.NotesPerClick) + 250;

            // Only allow purchase if player has enough Notes
            if (_gameState.Notes >= cost)
            {
                // Deduct cost for this attempt
                _gameState.Notes -= cost;

                // Roll RNG from 1 to 5
                var rng = new Random();
                int result = rng.Next(1, 6); // Generates 1, 2, 3, 4, or 5
                
                // Success on rolling exactly 2 (20% chance)
                if (result == 2)
                {
                    // Award one Major Sheet
                    _gameState.DiesIraeMajorSheets++;
                    // TODO: Show success message to player
                }
                // If result is not 2, player loses the cost but gets nothing
                // This is the gambling/risk element

                // Update this event screen's UI to reflect changes
                UpdateUI(_gameState);

                // Navigate up to find MainWindow and refresh its UI as well
                // This ensures the main screen shows updated Notes/NPS values
                var current = this.Parent;
                while (current != null && current is not Window)
                {
                    current = current.Parent;
                }
                if (current is MainWindow mw)
                {
                    UIUpdater.UpdateUI(mw, _gameState);
                }
            }
            else
            {
                // Not enough notes to attempt purchase
                // Refresh UI so player sees accurate cost/notes display
                UpdateUI(_gameState);
            }
        }

        /// <summary>
        /// Handler for Dies Irae Major button - converts a Major Sheet into the actual Major score.
        /// Cost: 1 Major Sheet (consumed on attempt)
        /// Success rate: 1 in 10,000 (0.01% chance)
        /// This is an extremely rare prize that unlocks special content.
        /// </summary>
        private void DiesIraeMajorButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            
            // Check if player has at least one Major Sheet to convert
            if (_gameState.DiesIraeMajorSheets > 0)
            {
                // Consume one Major Sheet for this attempt
                _gameState.DiesIraeMajorSheets--;
                
                // Roll RNG from 1 to 10,000
                var rng = new Random();
                int result = rng.Next(1, 10001); // Generates 1 to 10,000 inclusive
                
                // Success on rolling exactly 333 (0.01% chance)
                if (result == 333)
                {
                    // Award the actual Dies Irae Major score (extremely rare)
                    _gameState.DiesIraeOwned++;
                    // TODO: Show major success message/celebration to player
                }
                // If result is not 333, player loses the Major Sheet but gets nothing
                // This creates very high-stakes gambling for the event content
                
                // Update UI to reflect changes
                UpdateUI(_gameState);
            }
            // If no sheets owned, button click does nothing
        }

        /// <summary>
        /// Handler for back button - returns to Eternal Modulation (event hub) screen.
        /// </summary>
        private async void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is MainWindow mw)
            {
                await mw.TransitionAsync(() =>
                {
                    this.IsVisible = false;
                    var eternalModulationScreen = mw.FindControl<UserControl>("EternalModulationScreen");
                    if (eternalModulationScreen != null)
                        eternalModulationScreen.IsVisible = true;
                });
            }
            else
            {
                this.IsVisible = false;
                if (current is Window parentWindow)
                {
                    var eternalModulationScreen = parentWindow.FindControl<UserControl>("EternalModulationScreen");
                    if (eternalModulationScreen != null)
                        eternalModulationScreen.IsVisible = true;
                }
            }
        }
    }
}