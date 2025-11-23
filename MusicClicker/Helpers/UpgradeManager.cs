using Avalonia.Controls;
using System;

namespace MusicClicker
{
    // Responsible for executing upgrade purchases and applying their effects to GameState.
    // Performance-conscious changes in this class focus on reducing repeated expensive
    // math and allocations when purchasing multiple upgrades (especially "buy max").
    public static class UpgradeManager
    {
        // Purchase upgrades for a given upgrade slot.
        // - `owned` is passed by reference so calling code (and UI) can keep a synced counter.
        // - `amount == double.MaxValue` is treated as "buy max" and will loop until funds run out.
        // Implementation notes and performance choices:
        // 1) Repeated calls to Math.Pow are avoided by maintaining a running multiplier.
        //    The game models costs as baseCost * 1.15^owned (rounded to 2 decimals).
        //    Computing the exponential once and updating it incrementally avoids allocating
        //    and calling Math.Pow on every iteration, which matters when buying many items.
        // 2) We minimize property access and repeated Math.Round calls by caching values
        //    where safe (the visible cost remains rounded to 2 decimals for UI parity).
        public static void BuyUpgrade(MainWindow window, ref int owned, double baseCost, double npsIncrease, double clickIncrease, double amount, TextBlock ownedText, TextBlock costText)
        {
            var gameState = window.GameState;

            // Compute the starting multiplier for the current 'owned' count: 1.15^owned
            double multiplier = Math.Pow(1.15, owned);

            // Helper local to compute the rounded cost from the current multiplier.
            static double RoundedCost(double baseC, double mul) => Math.Round(baseC * mul, 2);

            if (amount == double.MaxValue)
            {
                // Buy as many as possible until funds run out. Use an incremental multiplier
                // to avoid calling Math.Pow for every new 'owned' value.
                while (true)
                {
                    double cost = RoundedCost(baseCost, multiplier);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;
                        owned++;
                        // increase multiplier by 1.15 for the next item
                        multiplier *= 1.15;

                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else
                    {
                        break; // cannot afford next
                    }
                }
            }
            else
            {
                // Quantified buy: cast to int and loop that many times (stopping early if funds run out).
                int purchaseCount = (int)amount;
                for (int i = 0; i < purchaseCount; i++)
                {
                    double cost = RoundedCost(baseCost, multiplier);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;
                        owned++;
                        multiplier *= 1.15;

                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else break;
                }
            }

            // Update the single-upgrade UI elements. Compute the next cost from the current multiplier
            // (multiplier now equals 1.15^owned because we maintained it above).
            ownedText.Text = $"Number Owned: {owned}";
            costText.Text = $"Cost: {Math.Round(baseCost * multiplier, 2)}";

            // Update the main notes display and then refresh all UI elements dependent on game state.
            window.NotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.UpgradeScreen.UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            UIUpdater.UpdateUI(window, gameState);
        }
    }
}