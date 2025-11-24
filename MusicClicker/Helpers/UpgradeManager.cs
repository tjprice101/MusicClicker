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
        // New signature: baseNpsEffect/growth and baseClickEffect/growth control per-purchase effect scaling.
        public static void BuyUpgrade(MainWindow window, ref int owned, double baseCost,
            double baseNpsEffect, double npsGrowth,
            double baseClickEffect, double clickGrowth,
            double amount, TextBlock ownedText, TextBlock costText)
        {
            var gameState = window.GameState;

            // Compute the starting multiplier for the current 'owned' count: 1.15^owned
            double multiplier = Math.Pow(1.15, owned);

            static double RoundedCost(double baseC, double mul) => Math.Round(baseC * mul, 2);

            // Helper to compute effect increase for current owned count
            static double EffectIncrease(double baseEffect, double growth, int ownedCount)
            {
                if (baseEffect == 0) return 0.0;
                return baseEffect * Math.Pow(growth, ownedCount);
            }

            if (amount == double.MaxValue)
            {
                while (true)
                {
                    double cost = RoundedCost(baseCost, multiplier);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;

                        // apply effects based on current owned (before increment)
                        double npsInc = EffectIncrease(baseNpsEffect, npsGrowth, owned);
                        double clickInc = EffectIncrease(baseClickEffect, clickGrowth, owned);

                        gameState.NotesPerSecond += npsInc;
                        gameState.NotesPerClick += clickInc;

                        owned++;
                        // increase multiplier for next purchase
                        multiplier *= 1.15;
                    }
                    else break;
                }
            }
            else
            {
                int purchaseCount = (int)amount;
                for (int i = 0; i < purchaseCount; i++)
                {
                    double cost = RoundedCost(baseCost, multiplier);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;

                        double npsInc = EffectIncrease(baseNpsEffect, npsGrowth, owned);
                        double clickInc = EffectIncrease(baseClickEffect, clickGrowth, owned);

                        gameState.NotesPerSecond += npsInc;
                        gameState.NotesPerClick += clickInc;

                        owned++;
                        multiplier *= 1.15;
                    }
                    else break;
                }
            }

            // Update UI
            ownedText.Text = $"Number Owned: {owned}";
            costText.Text = $"Cost: {Math.Round(baseCost * multiplier, 2)}";

            window.NotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.UpgradeScreen.UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            UIUpdater.UpdateUI(window, gameState);
        }
    }
}