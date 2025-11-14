using Avalonia.Controls;

namespace MusicClicker
{
    public static class EssenceManager
    {
        // Handles purchasing essence items
        public static void BuyEssence(MainWindow window, ref int essenceAmount, int cost, TextBlock ownedText)
        {
            var gameState = window.GameState;

            // Must have enough Notes
            if (gameState.Notes >= cost)
            {
                // Deduct cost and increment the essence count
                gameState.Notes -= cost;
                essenceAmount++;

                // Update the UI text
                ownedText.Text = $"{essenceAmount} Owned";
            }
        }
    }
}
