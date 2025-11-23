using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace MusicClicker
{
    public static class SaveScoresManager
    {
        // Shared RNG instance reduces overhead compared to creating a Random per call.
        private static Random _random = new Random();

        // Handle the logic when the player spends notes to "save" a score.
        // This method:
        // - checks affordability
        // - deducts the cost
        // - applies deterministic bonuses (OdeToJoyMajorAbility)
        // - performs several randomized rolls for additional minor/major drops
        // - dispatches a UI update to ensure controls are updated on the UI thread
        // Notes on randomness: each roll is independent and uses NextDouble thresholds
        // matching the original game's probability values.
        public static void HandleSaveScoreClick(MainWindow window,
            ref int minorKeys,
            ref int minorScales,
            ref int minorProgressions,
            ref int majorSheets,
            int cost)
        {
            var gameState = window.GameState;

            if (gameState.Notes >= cost)
            {
                if (gameState.OdeToJoyMajorAbility)
                {
                    majorSheets++;
                }

                gameState.Notes -= cost;

                // Independent random rolls; preserve original probability distribution.
                if (_random.NextDouble() <= 0.50) minorKeys++;
                if (_random.NextDouble() <= 0.25) minorScales++;
                if (_random.NextDouble() <= 0.15) minorProgressions++;
                if (_random.NextDouble() <= 0.05) majorSheets++;
            }

            // Ensure UI updates happen on the UI thread. Posting a single update call is
            // cheaper than attempting fine-grained control updates here and keeps the
            // rendering decisions centralized in UIUpdater.
            Dispatcher.UIThread.Post(() =>
            {
                UIUpdater.UpdateSaveScoresUI(window, gameState);
            });
        }
    }
}
