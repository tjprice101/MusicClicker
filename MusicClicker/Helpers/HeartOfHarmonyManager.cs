using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace MusicClicker
{
    public static class HeartOfHarmonyManager
    {
        // Random number generator for major-score conversions
        private static Random _random = new Random();

        // Handles converting a Major Sheet into Key / Scale / Progression
        public static void HandleMajorSheetClick(MainWindow window,
            ref int majorSheets,
            ref int majorKeys,
            ref int majorScales,
            ref int majorProgressions,
            TextBlock majorSheetsText,
            TextBlock majorKeysText,
            TextBlock majorScalesText,
            TextBlock majorProgressionsText,
            string scoreName)
        {
            // Can't convert if none are owned
            if (majorSheets <= 0) return;

            // Consume 1 major sheet
            majorSheets--;

            // Randomly choose which reward to give
            double roll = _random.NextDouble();
            if (roll < 0.33) majorKeys++;
            else if (roll < 0.66) majorScales++;
            else majorProgressions++;

            // Update UI text
            majorSheetsText.Text = $"{majorSheets} {scoreName} Major Sheets Owned";
            majorKeysText.Text = $"{majorKeys} {scoreName} Major Keys Owned";
            majorScalesText.Text = $"{majorScales} {scoreName} Major Scales Owned";
            majorProgressionsText.Text = $"{majorProgressions} {scoreName} Major Progressions Owned";
        }
    }
}
