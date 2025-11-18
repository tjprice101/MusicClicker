using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MusicClicker.Views
{
    public partial class WinterEventScreen : UserControl
    {
        public WinterEventScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            SoulOfWinterButton.Click += SoulOfWinterButton_Click;
            WinterMajorButton.Click += WinterMajorButton_Click;
        }

        public void UpdateUI(GameState gameState)
        {
            // Calculate cost: (NPS * 5) + 250
            double cost = (gameState.NotesPerSecond * 5) + 250;
            SoulOfWinterCostText.Text = $"{Math.Round(cost, 1)} Notes for Soul Resonance";
            
            // Update owned count (this will need to be added to GameState)
            // For now using placeholder: gameState.WinterMajorOwned
            WinterMajorOwnedText.Text = $"Major Sheets of Winter Owned: 0";
        }

        private void SoulOfWinterButton_Click(object? sender, RoutedEventArgs e)
        {
            // Placeholder for Soul of Winter purchase logic
            // Will be implemented later
        }

        private void WinterMajorButton_Click(object? sender, RoutedEventArgs e)
        {
            // Placeholder for Winter Major purchase logic
            // Will be implemented later
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