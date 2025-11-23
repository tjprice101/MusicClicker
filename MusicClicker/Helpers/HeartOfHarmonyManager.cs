using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace MusicClicker
{
    public static class HeartOfHarmonyManager
    {
        // Uses a shared RNG instance for conversions. Keeping a static Random avoids
        // repeated reseeding and reduces allocation compared with creating a new
        // Random per click.
        private static Random _random = new Random();

        // Convert a single Major Sheet into one of three major fragments: Keys, Scales,
        // or Progressions. The method is intentionally minimal: it updates the passed-in
        // counters by reference and then updates the UI texts directly. The probability
        // distribution is roughly uniform (three equal buckets) using NextDouble.
        // Notes:
        // - This method mutates the integer counters directly to keep the call-site
        //   code simple (many callers pass GameState fields by ref).
        // - UI updates are synchronous and lightweight string assignments; they should
        //   be called on the UI thread — callers in this project already run on UI thread.
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
            if (majorSheets <= 0) return; // nothing to convert

            majorSheets--;

            double roll = _random.NextDouble();
            if (roll < 0.33) majorKeys++;
            else if (roll < 0.66) majorScales++;
            else majorProgressions++;

            // Reflect the changes immediately in the UI text blocks passed by the caller.
            majorSheetsText.Text = $"{majorSheets} {scoreName} Major Sheets Owned";
            majorKeysText.Text = $"{majorKeys} {scoreName} Major Keys Owned";
            majorScalesText.Text = $"{majorScales} {scoreName} Major Scales Owned";
            majorProgressionsText.Text = $"{majorProgressions} {scoreName} Major Progressions Owned";
        }
    }
}
