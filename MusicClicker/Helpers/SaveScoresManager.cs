using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace MusicClicker
{
    public static class SaveScoresManager
    {
        // Random number generator for determining what drops
        private static Random _random = new Random();

        // Handles the logic when a score-save button is clicked
        public static void HandleSaveScoreClick(MainWindow window,
            ref int minorKeys,
            ref int minorScales,
            ref int minorProgressions,
            ref int majorSheets,
            int cost)
        {
            var gameState = window.GameState;

            // Must have enough Notes to save the score
            if (gameState.Notes >= cost)
            {
                // If Ode to Joy major perk is active, add a bonus major sheet
                if (gameState.OdeToJoyMajorAbility)
                {
                    majorSheets++;
                }

                // Deduct Notes cost
                gameState.Notes -= cost;

                // Random rolls for minor rewards
                if (_random.NextDouble() <= 0.50) minorKeys++;
                if (_random.NextDouble() <= 0.25) minorScales++;
                if (_random.NextDouble() <= 0.15) minorProgressions++;

                // Small chance to generate a major sheet
                if (_random.NextDouble() <= 0.05) majorSheets++;
            }

            // Update UI on the main thread
            Dispatcher.UIThread.Post(() =>
            {
                UIUpdater.UpdateSaveScoresUI(window, gameState);
            });
        }
    }
}
