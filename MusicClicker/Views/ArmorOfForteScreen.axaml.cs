using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class ArmorOfForteScreen : UserControl
    {
        public ArmorOfForteScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            
            // Initialize all armor item buttons (placeholder for future functionality)
            InitializeArmorButtons();
        }

        private void InitializeArmorButtons()
        {
            // Placeholder click handlers for all 9 armor items
            // These will be implemented with actual purchase logic later
            
            ArmorItem1Button.Click += (s, e) => HandleArmorPurchase(1, 10000);
            ArmorItem2Button.Click += (s, e) => HandleArmorPurchase(2, 25000);
            ArmorItem3Button.Click += (s, e) => HandleArmorPurchase(3, 50000);
            ArmorItem4Button.Click += (s, e) => HandleArmorPurchase(4, 100000);
            ArmorItem5Button.Click += (s, e) => HandleArmorPurchase(5, 250000);
            ArmorItem6Button.Click += (s, e) => HandleArmorPurchase(6, 500000);
            ArmorItem7Button.Click += (s, e) => HandleArmorPurchase(7, 1000000);
            ArmorItem8Button.Click += (s, e) => HandleArmorPurchase(8, 2500000);
            ArmorItem9Button.Click += (s, e) => HandleArmorPurchase(9, 5000000);
        }

        private void HandleArmorPurchase(int itemNumber, double cost)
        {
            // Placeholder for purchase logic
            // This will be implemented with GameState integration later
            // For now, just a placeholder that does nothing
            
            // Future implementation:
            // 1. Check if player has enough notes
            // 2. Deduct cost from notes
            // 3. Add item to owned count
            // 4. Apply item effects/bonuses
            // 5. Update UI
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show the main screen
            this.IsVisible = false;

            // Find the parent window and show MainScreen
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var mainScreen = parentWindow.FindControl<Grid>("MainScreen");
                if (mainScreen != null)
                    mainScreen.IsVisible = true;
            }
        }

        // Public method to update the UI when the screen becomes visible
        public void UpdateUI(GameState gameState)
        {
            // Update notes display
            ArmorNotesText.Text = $"Notes: {System.Math.Round(gameState.Notes, 1)}";
            
            // Update owned counts for all items
            // These will need to be added to GameState later
            // Example: ArmorItem1OwnedText.Text = $"Owned: {gameState.ArmorItem1Owned}";
        }
    }
}