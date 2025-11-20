using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MusicClicker.Views
{
    public partial class DiesIraeEventScreen : UserControl
    {
        private GameState? _gameState;

        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
            UpdateUI(_gameState);
        }
        public DiesIraeEventScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            SoulOfDiesIraeButton.Click += SoulOfDiesIraeButton_Click;
            DiesIraeMajorButton.Click += DiesIraeMajorButton_Click;
        }

        public void UpdateUI(GameState gameState)
        {
            // Ensure internal reference is set so click handlers can access the game state
            _gameState = gameState;
            // Calculate cost: (NPS * 5) + 250
            double cost = (gameState.NotesPerSecond * 5) + 250;
            SoulOfDiesIraeCostText.Text = $"{Math.Round(cost, 1)} Notes for Soul Resonance";

            // Update owned count
            DiesIraeMajorOwnedText.Text = $"Major Sheets of Dies Irae Owned: {gameState.DiesIraeMajorSheets}";
            // Optionally, update other UI elements for DiesIraeOwned if needed

            // Update current Notes and NPS display
            if (this.FindControl<TextBlock>("NotesText") is TextBlock notes)
            {
                notes.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            }
            if (this.FindControl<TextBlock>("NpsText") is TextBlock nps)
            {
                nps.Text = $"Notes Per Second: {Math.Round(gameState.NotesPerSecond, 1)}";
            }

            // Right-side major info: number owned and Crescendance ability placeholder
            try
            {
                DiesIraeMajorOwnedRightText.Text = $"Number Owned: {gameState.DiesIraeOwned}";
                DiesIraeCrescendanceText.Text = "Crescendance Ability: <placeholder>";
            }
            catch (Exception)
            {
                // If controls not loaded yet, ignore — UpdateUI will run again when visible.
            }
        }

        private void SoulOfDiesIraeButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            // Calculate cost using same formula as the UI
            double cost = (_gameState.NotesPerSecond * 5) + 250;

            // Only allow attempt if player has enough Notes
            if (_gameState.Notes >= cost)
            {
                // Deduct cost for this attempt
                _gameState.Notes -= cost;

                // RNG 1-5
                var rng = new Random();
                int result = rng.Next(1, 6); // 1 to 5 inclusive
                if (result == 2)
                {
                    _gameState.DiesIraeMajorSheets++;
                    // Optionally, show a message to the user
                }

                // Update this screen's UI
                UpdateUI(_gameState);

                // If we can find the MainWindow parent, refresh its UI as well
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
                // Not enough notes; simply refresh UI so cost/notes display is accurate
                UpdateUI(_gameState);
            }
        }

        private void DiesIraeMajorButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            if (_gameState.DiesIraeMajorSheets > 0)
            {
                _gameState.DiesIraeMajorSheets--;
                var rng = new Random();
                int result = rng.Next(1, 10001); // 1 to 10,000 inclusive
                if (result == 333)
                {
                    _gameState.DiesIraeOwned++;
                    // Optionally, show a message to the user
                }
                UpdateUI(_gameState);
            }
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show Eternal Modulation screen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var eternalModulationScreen = parentWindow.FindControl<UserControl>("EternalModulationScreen");
                if (eternalModulationScreen != null)
                    eternalModulationScreen.IsVisible = true;
            }
        }
    }
}