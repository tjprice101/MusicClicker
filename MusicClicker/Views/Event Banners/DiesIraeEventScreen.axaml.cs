using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MusicClicker.Views
{
    public partial class DiesIraeEventScreen : UserControl
    {
        public DiesIraeEventScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            SoulOfDiesIraeButton.Click += SoulOfDiesIraeButton_Click;
            DiesIraeMajorButton.Click += DiesIraeMajorButton_Click;
        }

        public void UpdateUI(GameState gameState)
        {
            // Calculate cost: (NPS * 5) + 250
            double cost = (gameState.NotesPerSecond * 5) + 250;
            SoulOfDiesIraeCostText.Text = $"{Math.Round(cost, 1)} Notes for Soul Resonance";
            
            // Update owned count (this will need to be added to GameState)
            // For now using placeholder: gameState.DiesIraeMajorOwned
            DiesIraeMajorOwnedText.Text = $"Major Sheets of Dies Irae Owned: 0";
        }

        private void SoulOfDiesIraeButton_Click(object? sender, RoutedEventArgs e)
        {
            // Placeholder for Soul of Dies Irae purchase logic
            // Will be implemented later
        }

        private void DiesIraeMajorButton_Click(object? sender, RoutedEventArgs e)
        {
            // Placeholder for Dies Irae Major purchase logic
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