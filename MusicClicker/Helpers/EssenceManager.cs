using Avalonia.Controls;

namespace MusicClicker
{
    public static class EssenceManager
    {
        public static void BuyEssence(MainWindow window, ref int essenceAmount, int cost, TextBlock ownedText)
        {
            var gameState = window.GameState;

            if (gameState.Notes >= cost)
            {
                gameState.Notes -= cost;
                essenceAmount++;
                ownedText.Text = $"{essenceAmount} Owned";
            }
        }
    }
}
