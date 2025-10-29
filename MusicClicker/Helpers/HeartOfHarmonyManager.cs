using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace MusicClicker
{
    public static class HeartOfHarmonyManager
    {
        private static Random _random = new Random();

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
            if (majorSheets <= 0) return;

            majorSheets--;

            double roll = _random.NextDouble();
            if (roll < 0.33) majorKeys++;
            else if (roll < 0.66) majorScales++;
            else majorProgressions++;

            majorSheetsText.Text = $"{majorSheets} {scoreName} Major Sheets Owned";
            majorKeysText.Text = $"{majorKeys} {scoreName} Major Keys Owned";
            majorScalesText.Text = $"{majorScales} {scoreName} Major Scales Owned";
            majorProgressionsText.Text = $"{majorProgressions} {scoreName} Major Progressions Owned";
        }
    }
}
