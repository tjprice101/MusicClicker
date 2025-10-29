using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace MusicClicker
{
    public static class SaveScoresManager
    {
        private static Random _random = new Random();

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
                gameState.Notes -= cost;
                if (_random.NextDouble() <= 0.50) minorKeys++;
                if (_random.NextDouble() <= 0.25) minorScales++;
                if (_random.NextDouble() <= 0.15) minorProgressions++;
                if (_random.NextDouble() <= 0.05) majorSheets++;
            }

            Dispatcher.UIThread.Post(() =>
            {
                UIUpdater.UpdateSaveScoresUI(window, gameState);
            });
        }
    }
}
