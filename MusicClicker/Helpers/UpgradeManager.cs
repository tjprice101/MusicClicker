using Avalonia.Controls;
using System;

namespace MusicClicker
{
    public static class UpgradeManager
    {
        public static void BuyUpgrade(MainWindow window, ref int owned, double baseCost, double npsIncrease, double clickIncrease, double amount, TextBlock ownedText, TextBlock costText)
        {
            var gameState = window.GameState;

            if (amount == double.MaxValue)
            {
                while (true)
                {
                    double cost = Math.Round(baseCost * Math.Pow(1.15, owned), 2);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;
                        owned++;
                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else break;
                }
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    double cost = Math.Round(baseCost * Math.Pow(1.15, owned), 2);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;
                        owned++;
                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else break;
                }
            }

            ownedText.Text = $"Number Owned: {owned}";
            costText.Text = $"Cost: {Math.Round(baseCost * Math.Pow(1.15, owned), 2)}";
        }
    }
}
