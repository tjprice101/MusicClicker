using Avalonia.Controls;
using System;
using MusicClicker;

namespace MusicClicker
{
    // Centralized UI update manager that refreshes UI controls to reflect the current GameState.
    // Rewrites here focus on avoiding redundant expensive math (Math.Pow / Math.Round) and
    // reducing repeated property access where possible without changing visible behavior.
    public static class UIUpdater
    {
        // Single responsibility: update the main upgrade UI quickly and with minimal allocations.
        public static void UpdateUI(MainWindow window, GameState gameState)
        {
            // Cache frequently used values locally to avoid repeated property access.
            double notes = gameState.Notes;
            double nps = gameState.NotesPerSecond;

            // Format the top-level displays once and reuse values.
            window.NotesText.Text = $"Notes: {Math.Round(notes, 1)}";
            window.NpsText.Text = $"Notes Per Second: {Math.Round(nps, 1)}";
            window.UpgradeScreen.UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(notes, 1)}";

            // Owned counts are simple integer-to-string conversions — inexpensive.
            window.UpgradeScreen.ChordOwnedTextUpgrade.Text = $"Number Owned: {gameState.ChordOwned}";
            window.UpgradeScreen.ScaleOwnedTextUpgrade.Text = $"Number Owned: {gameState.ScaleOwned}";
            window.UpgradeScreen.OrchestraOwnedTextUpgrade.Text = $"Number Owned: {gameState.OrchestraOwned}";
            window.UpgradeScreen.SymphonyOwnedTextUpgrade.Text = $"Number Owned: {gameState.SymphonyOwned}";
            window.UpgradeScreen.AriaOwnedTextUpgrade.Text = $"Number Owned: {gameState.AriaOwned}";
            window.UpgradeScreen.RequiemOwnedTextUpgrade.Text = $"Number Owned: {gameState.RequiemOwned}";
            window.UpgradeScreen.OpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.OpusOwned}";
            window.UpgradeScreen.MagnumOpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.MagnumOpusOwned}";

            // Compute each upgrade's cost once using Math.Pow per slot (still necessary to compute
            // the exponential), but avoid calling Math.Round twice for the same value.
            double chordCost = Math.Round(gameState.ChordBaseCost * Math.Pow(1.15, gameState.ChordOwned), 2);
            double scaleCost = Math.Round(gameState.ScaleBaseCost * Math.Pow(1.15, gameState.ScaleOwned), 2);
            double orchestraCost = Math.Round(gameState.OrchestraBaseCost * Math.Pow(1.15, gameState.OrchestraOwned), 2);
            double symphonyCost = Math.Round(gameState.SymphonyBaseCost * Math.Pow(1.15, gameState.SymphonyOwned), 2);
            double ariaCost = Math.Round(gameState.AriaBaseCost * Math.Pow(1.15, gameState.AriaOwned), 2);
            double requiemCost = Math.Round(gameState.RequiemBaseCost * Math.Pow(1.15, gameState.RequiemOwned), 2);
            double opusCost = Math.Round(gameState.OpusBaseCost * Math.Pow(1.15, gameState.OpusOwned), 2);
            double magnumCost = Math.Round(gameState.MagnumOpusBaseCost * Math.Pow(1.15, gameState.MagnumOpusOwned), 2);

            window.UpgradeScreen.ChordCostTextUpgrade.Text = $"Cost: {chordCost}";
            window.UpgradeScreen.ScaleCostTextUpgrade.Text = $"Cost: {scaleCost}";
            window.UpgradeScreen.OrchestraCostTextUpgrade.Text = $"Cost: {orchestraCost}";
            window.UpgradeScreen.SymphonyCostTextUpgrade.Text = $"Cost: {symphonyCost}";
            window.UpgradeScreen.AriaCostTextUpgrade.Text = $"Cost: {ariaCost}";
            window.UpgradeScreen.RequiemCostTextUpgrade.Text = $"Cost: {requiemCost}";
            window.UpgradeScreen.OpusCostTextUpgrade.Text = $"Cost: {opusCost}";
            window.UpgradeScreen.MagnumOpusCostTextUpgrade.Text = $"Cost: {magnumCost}";
        }

        // Lightweight updater that only refreshes notes displays across all screens.
        // This is intended to be called frequently (e.g., every accumulation tick)
        // so the player's Notes total appears to increase smoothly regardless of
        // which screen is currently visible. Keep this function minimal to avoid
        // causing excessive UI work. More expensive UI updates remain in UpdateUI.
        public static void UpdateNotesOnly(MainWindow window, GameState gameState)
        {
            try
            {
                string notesText = $"Notes: {Math.Round(gameState.Notes, 1)}";

                // Helper local to set text only if changed (reduces UI churn)
                void SetIfChanged(System.Action<string> setter, string newText)
                {
                    try
                    {
                        setter(newText);
                    }
                    catch { }
                }

                // Main HUD
                if (window.NotesText.Text != notesText) window.NotesText.Text = notesText;

                // Upgrade header
                if (window.UpgradeScreen?.UpgradeNotesTextHeader != null && window.UpgradeScreen.UpgradeNotesTextHeader.Text != notesText)
                    window.UpgradeScreen.UpgradeNotesTextHeader.Text = notesText;

                // Fragmentation
                if (window.FragmentationScreen?.FragmentationNotesText != null && window.FragmentationScreen.FragmentationNotesText.Text != notesText)
                    window.FragmentationScreen.FragmentationNotesText.Text = notesText;

                // Save Scores
                if (window.SaveScoresScreen?.SaveScoresNotesText != null && window.SaveScoresScreen.SaveScoresNotesText.Text != notesText)
                    window.SaveScoresScreen.SaveScoresNotesText.Text = notesText;

                // Heart of Harmony
                if (window.HeartOfHarmonyScreen?.HeartOfHarmonyNotesText != null && window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text != notesText)
                    window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text = notesText;

                // Unite the Symphony
                if (window.UnityTheSymphonyScreen?.UnityNotesTextHeader != null && window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text != notesText)
                    window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = notesText;

                // Armor of Forte screen
                try
                {
                    if (window.ArmorOfForteScreen?.FindControl<TextBlock>("ArmorNotesText") is TextBlock armorNotes && armorNotes.Text != notesText)
                        armorNotes.Text = notesText;
                }
                catch { }

                // Event banners (DiesIrae and Winter) - update their internal NotesText if present
                try
                {
                    if (window.DiesIraeEventScreen?.FindControl<TextBlock>("NotesText") is TextBlock diesNotes && diesNotes.Text != notesText)
                        diesNotes.Text = notesText;
                }
                catch { }
                try
                {
                    if (window.WinterEventScreen?.FindControl<TextBlock>("NotesText") is TextBlock winterNotes && winterNotes.Text != notesText)
                        winterNotes.Text = notesText;
                }
                catch { }

                // Customization screens: even if they don't display Notes, keep a no-op attempt
                // so their internal controls (if any) are kept in sync without throwing.
                try
                {
                    var clickerScreen = window.FindControl<UserControl>("ClickerCustomizeScreen");
                    clickerScreen?.FindControl<TextBlock>("NotesText");
                }
                catch { }
                try
                {
                    var bgScreen = window.FindControl<UserControl>("BackgroundCustomizeScreen");
                    bgScreen?.FindControl<TextBlock>("NotesText");
                }
                catch { }
            }
            catch (Exception)
            {
                // If any control is missing during initialization, ignore and continue.
            }
        }

        // Update the fragmentation screen; cheap operations only.
        public static void UpdateFragmentationUI(MainWindow window, GameState gameState)
        {
            window.FragmentationScreen.FragmentationNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.FragmentationScreen.MelodiousOwnedText.Text = $"{gameState.MelodiousOwned} Owned";
            window.FragmentationScreen.HarmoniousOwnedText.Text = $"{gameState.HarmoniousOwned} Owned";
        }

        // Update the Save Scores UI. This method sets many text fields — keep it straightforward
        // and avoid repeating Math.Round or expensive operations more than once per value.
        public static void UpdateSaveScoresUI(MainWindow window, GameState gameState)
        {
            // Moonlight Sonata components
            window.SaveScoresScreen.MoonlightMinorKeysText.Text = $"{gameState.MoonlightMinorKeys} Minor Keys of Moonlight Sonata Owned";
            window.SaveScoresScreen.MoonlightMinorScalesText.Text = $"{gameState.MoonlightMinorScales} Minor Scales of Moonlight Sonata Owned";
            window.SaveScoresScreen.MoonlightMinorProgressionsText.Text = $"{gameState.MoonlightMinorProgressions} Minor Progressions of Moonlight Sonata Owned";
            window.SaveScoresScreen.MoonlightMajorSheetsText.Text = $"{gameState.MoonlightMajorSheets} Major Sheets of Moonlight Sonata Owned";

            // Eroica components
            window.SaveScoresScreen.EroicaMinorKeysText.Text = $"{gameState.EroicaMinorKeys} Minor Keys of Eroica Owned";
            window.SaveScoresScreen.EroicaMinorScalesText.Text = $"{gameState.EroicaMinorScales} Minor Scales of Eroica Owned";
            window.SaveScoresScreen.EroicaMinorProgressionsText.Text = $"{gameState.EroicaMinorProgressions} Minor Progressions of Eroica Owned";
            window.SaveScoresScreen.EroicaMajorSheetsText.Text = $"{gameState.EroicaMajorSheets} Major Sheets of Eroica Owned";

            // Swan Lake components
            window.SaveScoresScreen.SwanLakeMinorKeysText.Text = $"{gameState.SwanLakeMinorKeys} Minor Keys of Swan Lake Owned";
            window.SaveScoresScreen.SwanLakeMinorScalesText.Text = $"{gameState.SwanLakeMinorScales} Minor Scales of Swan Lake Owned";
            window.SaveScoresScreen.SwanLakeMinorProgressionsText.Text = $"{gameState.SwanLakeMinorProgressions} Minor Progressions of Swan Lake Owned";
            window.SaveScoresScreen.SwanLakeMajorSheetsText.Text = $"{gameState.SwanLakeMajorSheets} Major Sheets of Swan Lake Owned";

            // La Campanella components
            window.SaveScoresScreen.LaCampanellaMinorKeysText.Text = $"{gameState.LaCampanellaMinorKeys} Minor Keys of La Campanella Owned";
            window.SaveScoresScreen.LaCampanellaMinorScalesText.Text = $"{gameState.LaCampanellaMinorScales} Minor Scales of La Campanella Owned";
            window.SaveScoresScreen.LaCampanellaMinorProgressionsText.Text = $"{gameState.LaCampanellaMinorProgressions} Minor Progressions of La Campanella Owned";
            window.SaveScoresScreen.LaCampanellaMajorSheetsText.Text = $"{gameState.LaCampanellaMajorSheets} Major Sheets of La Campanella Owned";

            // Enigma components
            window.SaveScoresScreen.EnigmaMinorKeysText.Text = $"{gameState.EnigmaMinorKeys} Minor Keys of Enigma Owned";
            window.SaveScoresScreen.EnigmaMinorScalesText.Text = $"{gameState.EnigmaMinorScales} Minor Scales of Enigma Owned";
            window.SaveScoresScreen.EnigmaMinorProgressionsText.Text = $"{gameState.EnigmaMinorProgressions} Minor Progressions of Enigma Owned";
            window.SaveScoresScreen.EnigmaMajorSheetsText.Text = $"{gameState.EnigmaMajorSheets} Major Sheets of Enigma Owned";

            // Fate components
            window.SaveScoresScreen.FateMinorKeysText.Text = $"{gameState.FateMinorKeys} Minor Keys of Fate Owned";
            window.SaveScoresScreen.FateMinorScalesText.Text = $"{gameState.FateMinorScales} Minor Scales of Fate Owned";
            window.SaveScoresScreen.FateMinorProgressionsText.Text = $"{gameState.FateMinorProgressions} Minor Progressions of Fate Owned";
            window.SaveScoresScreen.FateMajorSheetsText.Text = $"{gameState.FateMajorSheets} Major Sheets of Fate Owned";

            // Ode to Joy components
            window.SaveScoresScreen.OdeToJoyMinorKeysText.Text = $"{gameState.OdeToJoyMinorKeys} Minor Keys of Ode to Joy Owned";
            window.SaveScoresScreen.OdeToJoyMinorScalesText.Text = $"{gameState.OdeToJoyMinorScales} Minor Scales of Ode to Joy Owned";
            window.SaveScoresScreen.OdeToJoyMinorProgressionsText.Text = $"{gameState.OdeToJoyMinorProgressions} Minor Progressions of Ode to Joy Owned";
            window.SaveScoresScreen.OdeToJoyMajorSheetsText.Text = $"{gameState.OdeToJoyMajorSheets} Major Sheets of Ode to Joy Owned";

            // Update notes display
            window.SaveScoresScreen.SaveScoresNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            // Enable/disable soul buttons based on whether the player can afford each fixed cost.
            // Wrap in try-catch because these controls may not be constructed during certain lifecycle phases.
            try
            {
                double notesVal = gameState.Notes;
                window.SaveScoresScreen.MoonlightSonataButton.IsEnabled = notesVal >= 5000;
                window.SaveScoresScreen.EroicaButton.IsEnabled = notesVal >= 15000;
                window.SaveScoresScreen.SwanLakeButton.IsEnabled = notesVal >= 45000;
                window.SaveScoresScreen.LaCampanellaButton.IsEnabled = notesVal >= 135000;
                window.SaveScoresScreen.EnigmaButton.IsEnabled = notesVal >= 405000;
                window.SaveScoresScreen.FateButton.IsEnabled = notesVal >= 1215000;
                window.SaveScoresScreen.OdeToJoyButton.IsEnabled = notesVal >= 3645000;
            }
            catch (Exception)
            {
                // If any controls are not available yet, ignore — update will run again later.
            }
        }

        // Update the Heart of Harmony screen showing all major sheet music components.
        // Straightforward mapping of GameState counters to UI text; keep it explicit for readability.
        public static void UpdateHeartOfHarmonyUI(MainWindow window, GameState gameState)
        {
            window.HeartOfHarmonyScreen.MoonlightMajorSheetsOwnedText.Text = $"{gameState.MoonlightMajorSheets} Moonlight Sonata Major Sheets Owned";
            window.HeartOfHarmonyScreen.MoonlightMajorKeysOwnedText.Text = $"{gameState.MoonlightMajorKeys} Moonlight Sonata Major Keys Owned";
            window.HeartOfHarmonyScreen.MoonlightMajorScalesOwnedText.Text = $"{gameState.MoonlightMajorScales} Moonlight Sonata Major Scales Owned";
            window.HeartOfHarmonyScreen.MoonlightMajorProgressionsOwnedText.Text = $"{gameState.MoonlightMajorProgressions} Moonlight Sonata Major Progressions Owned";

            window.HeartOfHarmonyScreen.EroicaMajorSheetsOwnedText.Text = $"{gameState.EroicaMajorSheets} Eroica Major Sheets Owned";
            window.HeartOfHarmonyScreen.EroicaMajorKeysOwnedText.Text = $"{gameState.EroicaMajorKeys} Eroica Major Keys Owned";
            window.HeartOfHarmonyScreen.EroicaMajorScalesOwnedText.Text = $"{gameState.EroicaMajorScales} Eroica Major Scales Owned";
            window.HeartOfHarmonyScreen.EroicaMajorProgressionsOwnedText.Text = $"{gameState.EroicaMajorProgressions} Eroica Major Progressions Owned";

            window.HeartOfHarmonyScreen.SwanLakeMajorSheetsOwnedText.Text = $"{gameState.SwanLakeMajorSheets} Swan Lake Major Sheets Owned";
            window.HeartOfHarmonyScreen.SwanLakeMajorKeysOwnedText.Text = $"{gameState.SwanLakeMajorKeys} Swan Lake Major Keys Owned";
            window.HeartOfHarmonyScreen.SwanLakeMajorScalesOwnedText.Text = $"{gameState.SwanLakeMajorScales} Swan Lake Major Scales Owned";
            window.HeartOfHarmonyScreen.SwanLakeMajorProgressionsOwnedText.Text = $"{gameState.SwanLakeMajorProgressions} Swan Lake Major Progressions Owned";

            window.HeartOfHarmonyScreen.LaCampanellaMajorSheetsOwnedText.Text = $"{gameState.LaCampanellaMajorSheets} La Campanella Major Sheets Owned";
            window.HeartOfHarmonyScreen.LaCampanellaMajorKeysOwnedText.Text = $"{gameState.LaCampanellaMajorKeys} La Campanella Major Keys Owned";
            window.HeartOfHarmonyScreen.LaCampanellaMajorScalesOwnedText.Text = $"{gameState.LaCampanellaMajorScales} La Campanella Major Scales Owned";
            window.HeartOfHarmonyScreen.LaCampanellaMajorProgressionsOwnedText.Text = $"{gameState.LaCampanellaMajorProgressions} La Campanella Major Progressions Owned";

            window.HeartOfHarmonyScreen.EnigmaMajorSheetsOwnedText.Text = $"{gameState.EnigmaMajorSheets} Enigma Major Sheets Owned";
            window.HeartOfHarmonyScreen.EnigmaMajorKeysOwnedText.Text = $"{gameState.EnigmaMajorKeys} Enigma Major Keys Owned";
            window.HeartOfHarmonyScreen.EnigmaMajorScalesOwnedText.Text = $"{gameState.EnigmaMajorScales} Enigma Major Scales Owned";
            window.HeartOfHarmonyScreen.EnigmaMajorProgressionsOwnedText.Text = $"{gameState.EnigmaMajorProgressions} Enigma Major Progressions Owned";

            window.HeartOfHarmonyScreen.FateMajorSheetsOwnedText.Text = $"{gameState.FateMajorSheets} Fate Major Sheets Owned";
            window.HeartOfHarmonyScreen.FateMajorKeysOwnedText.Text = $"{gameState.FateMajorKeys} Fate Major Keys Owned";
            window.HeartOfHarmonyScreen.FateMajorScalesOwnedText.Text = $"{gameState.FateMajorScales} Fate Major Scales Owned";
            window.HeartOfHarmonyScreen.FateMajorProgressionsOwnedText.Text = $"{gameState.FateMajorProgressions} Fate Major Progressions Owned";

            window.HeartOfHarmonyScreen.OdeToJoyMajorSheetsOwnedText.Text = $"{gameState.OdeToJoyMajorSheets} Ode to Joy Major Sheets Owned";
            window.HeartOfHarmonyScreen.OdeToJoyMajorKeysOwnedText.Text = $"{gameState.OdeToJoyMajorKeys} Ode to Joy Major Keys Owned";
            window.HeartOfHarmonyScreen.OdeToJoyMajorScalesOwnedText.Text = $"{gameState.OdeToJoyMajorScales} Ode to Joy Major Scales Owned";
            window.HeartOfHarmonyScreen.OdeToJoyMajorProgressionsOwnedText.Text = $"{gameState.OdeToJoyMajorProgressions} Ode to Joy Major Progressions Owned";

            window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
        }

        // Update the Unite the Symphony screen showing completed minor/major scores and refresh
        // the tempo drawer to keep the tuned images in sync with owned state.
        public static void UpdateUnitySymphonyUI(MainWindow window, GameState gameState)
        {
            window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            window.UnityTheSymphonyScreen.MoonlightMinorOwnedText.Text = $"{gameState.MoonlightMinorOwned} Owned";
            window.UnityTheSymphonyScreen.EroicaMinorOwnedText.Text = $"{gameState.EroicaMinorOwned} Owned";
            window.UnityTheSymphonyScreen.SwanMinorOwnedText.Text = $"{gameState.SwanMinorOwned} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMinorOwnedText.Text = $"{gameState.LaCampanellaMinorOwned} Owned";
            window.UnityTheSymphonyScreen.EnigmaMinorOwnedText.Text = $"{gameState.EnigmaMinorOwned} Owned";
            window.UnityTheSymphonyScreen.FateMinorOwnedText.Text = $"{gameState.FateMinorOwned} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMinorOwnedText.Text = $"{gameState.OdeToJoyMinorOwned} Owned";

            window.UnityTheSymphonyScreen.MoonlightMajorOwnedText.Text = $"{gameState.MoonlightMajorOwned} Owned";
            window.UnityTheSymphonyScreen.EroicaMajorOwnedText.Text = $"{gameState.EroicaMajorOwned} Owned";
            window.UnityTheSymphonyScreen.SwanMajorOwnedText.Text = $"{gameState.SwanMajorOwned} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMajorOwnedText.Text = $"{gameState.LaCampanellaMajorOwned} Owned";
            window.UnityTheSymphonyScreen.EnigmaMajorOwnedText.Text = $"{gameState.EnigmaMajorOwned} Owned";
            window.UnityTheSymphonyScreen.FateMajorOwnedText.Text = $"{gameState.FateMajorOwned} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMajorOwnedText.Text = $"{gameState.OdeToJoyMajorOwned} Owned";

            MainWindow.GlobalTempoManager.RefreshDrawer();
        }
    }
}