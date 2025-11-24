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
            // If the user is interacting (scrolling/dragging), throttle heavy UI updates
            if (window?.IsUserInteracting == true)
                return;
            // Cache frequently used values locally to avoid repeated property access.
            double notes = gameState.Notes;
            double nps = gameState.NotesPerSecond;

            // Top-level displays (Notes / NPS) are animated separately by AnimateVisuals
            // which runs at the animation frame rate to produce smooth transitions.

            // Only update screens that are visible to reduce UI churn on hidden views.
            if (window.UpgradeScreen?.IsVisible == true)
            {
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
                // If the user is actively interacting (scrolling or dragging), skip updating
                // text properties to avoid layout churn which causes stutter.
                if (window?.IsUserInteracting == true)
                    return;
                // Use the smoothed displayed notes when available (updated at FRAME_RATE by AnimateVisuals).
                double displayNotes = window?.DisplayedNotes ?? gameState.Notes;
                string notesText = $"Notes: {Math.Round(displayNotes, 1)}";

                // Note: intentionally inlined simple checks to avoid allocating delegates.

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

                // Armor of Forte screen (direct access avoids FindControl allocations)
                if (window.ArmorOfForteScreen?.ArmorNotesText != null && window.ArmorOfForteScreen.ArmorNotesText.Text != notesText)
                    window.ArmorOfForteScreen.ArmorNotesText.Text = notesText;

                // Event banners (DiesIrae and Winter) - direct access if available
                if (window.DiesIraeEventScreen?.NotesText != null && window.DiesIraeEventScreen.NotesText.Text != notesText)
                    window.DiesIraeEventScreen.NotesText.Text = notesText;

                if (window.WinterEventScreen?.NotesText != null && window.WinterEventScreen.NotesText.Text != notesText)
                    window.WinterEventScreen.NotesText.Text = notesText;
            }
            catch (Exception)
            {
                // If any control is missing during initialization, ignore and continue.
            }
        }

        /// <summary>
        /// AnimateVisuals: called at animation frame rate to smoothly interpolate
        /// displayed values toward the underlying GameState. Keeps visual counters
        /// (Notes, NPS) fluid without updating expensive UI elements unnecessarily.
        /// </summary>
        public static void AnimateVisuals(MainWindow window, GameState gameState, double deltaSeconds)
        {
            if (window == null || gameState == null) return;

            // Assign displayed values directly so the HUD reflects discrete changes
            // (e.g., NPS bursts) immediately without smoothing between updates.
            window.DisplayedNotes = gameState.Notes;
            window.DisplayedNps = gameState.NotesPerSecond;

            // Update top-level HUD texts if changed enough to be visible
            // Avoid changing text properties while the user is interacting to prevent layout
            // thrashing that can cause jitter (scrolling, slider drags, etc.). We still update
            // the internal displayed values so the numbers appear smooth once interaction stops.
            string notesText = $"Notes: {Math.Round(window.DisplayedNotes, 1)}";
            string npsText = $"Notes Per Second: {Math.Round(window.DisplayedNps, 1)}";
            if (window.IsUserInteracting != true)
            {
                if (window.NotesText.Text != notesText) window.NotesText.Text = notesText;
                if (window.NpsText.Text != npsText) window.NpsText.Text = npsText;

                // Also update minimal per-screen headers when not interacting
                if (window.UpgradeScreen?.IsVisible == true && window.UpgradeScreen.UpgradeNotesTextHeader != null)
                {
                    if (window.UpgradeScreen.UpgradeNotesTextHeader.Text != notesText)
                        window.UpgradeScreen.UpgradeNotesTextHeader.Text = notesText;
                }

                if (window.FragmentationScreen?.IsVisible == true && window.FragmentationScreen.FragmentationNotesText != null)
                {
                    if (window.FragmentationScreen.FragmentationNotesText.Text != notesText)
                        window.FragmentationScreen.FragmentationNotesText.Text = notesText;
                }

                if (window.SaveScoresScreen?.IsVisible == true && window.SaveScoresScreen.SaveScoresNotesText != null)
                {
                    if (window.SaveScoresScreen.SaveScoresNotesText.Text != notesText)
                        window.SaveScoresScreen.SaveScoresNotesText.Text = notesText;
                }

                if (window.HeartOfHarmonyScreen?.IsVisible == true && window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText != null)
                {
                    if (window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text != notesText)
                        window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text = notesText;
                }

                if (window.UnityTheSymphonyScreen?.IsVisible == true && window.UnityTheSymphonyScreen.UnityNotesTextHeader != null)
                {
                    if (window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text != notesText)
                        window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = notesText;
                }

                if (window.ArmorOfForteScreen?.IsVisible == true && window.ArmorOfForteScreen.ArmorNotesText != null)
                {
                    if (window.ArmorOfForteScreen.ArmorNotesText.Text != notesText)
                        window.ArmorOfForteScreen.ArmorNotesText.Text = notesText;
                }
            }

            // Also update the per-screen header texts minimally (only if visible)
            if (window.UpgradeScreen?.IsVisible == true && window.UpgradeScreen.UpgradeNotesTextHeader != null)
            {
                if (window.UpgradeScreen.UpgradeNotesTextHeader.Text != notesText)
                    window.UpgradeScreen.UpgradeNotesTextHeader.Text = notesText;
            }

            if (window.FragmentationScreen?.IsVisible == true && window.FragmentationScreen.FragmentationNotesText != null)
            {
                if (window.FragmentationScreen.FragmentationNotesText.Text != notesText)
                    window.FragmentationScreen.FragmentationNotesText.Text = notesText;
            }

            // Other visible screens that show notes - update similarly if visible
            if (window.SaveScoresScreen?.IsVisible == true && window.SaveScoresScreen.SaveScoresNotesText != null)
            {
                if (window.SaveScoresScreen.SaveScoresNotesText.Text != notesText)
                    window.SaveScoresScreen.SaveScoresNotesText.Text = notesText;
            }

            if (window.HeartOfHarmonyScreen?.IsVisible == true && window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText != null)
            {
                if (window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text != notesText)
                    window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text = notesText;
            }

            if (window.UnityTheSymphonyScreen?.IsVisible == true && window.UnityTheSymphonyScreen.UnityNotesTextHeader != null)
            {
                if (window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text != notesText)
                    window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = notesText;
            }

            if (window.ArmorOfForteScreen?.IsVisible == true && window.ArmorOfForteScreen.ArmorNotesText != null)
            {
                if (window.ArmorOfForteScreen.ArmorNotesText.Text != notesText)
                    window.ArmorOfForteScreen.ArmorNotesText.Text = notesText;
            }
        }

        // Update the fragmentation screen; cheap operations only.
        public static void UpdateFragmentationUI(MainWindow window, GameState gameState)
        {
            if (window.FragmentationScreen?.IsVisible != true)
                return;

            window.FragmentationScreen.FragmentationNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.FragmentationScreen.MelodiousOwnedText.Text = $"{gameState.MelodiousOwned} Owned";
            window.FragmentationScreen.HarmoniousOwnedText.Text = $"{gameState.HarmoniousOwned} Owned";
        }

        // Update the Save Scores UI. This method sets many text fields — keep it straightforward
        // and avoid repeating Math.Round or expensive operations more than once per value.
        public static void UpdateSaveScoresUI(MainWindow window, GameState gameState)
        {
            if (window.SaveScoresScreen?.IsVisible != true)
                return;
            // Throttle heavy save-scores updates during interaction
            if (window.IsUserInteracting == true) return;

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

        // Immediate variant which bypasses interaction throttling so rapid-click actions
        // can still reflect their results in the UI. Use sparingly for click handlers.
        public static void UpdateSaveScoresUIImmediate(MainWindow window, GameState gameState)
        {
            if (window.SaveScoresScreen?.IsVisible != true)
                return;

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
            if (window.HeartOfHarmonyScreen?.IsVisible != true)
                return;

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
            if (window.UnityTheSymphonyScreen?.IsVisible != true)
                return;
            // Throttle heavy updates during user interaction
            if (window.IsUserInteracting == true) return;

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

        // Immediate variant: bypass interaction throttling so rapid crafts update counts.
        public static void UpdateUnitySymphonyUIImmediate(MainWindow window, GameState gameState)
        {
            if (window.UnityTheSymphonyScreen?.IsVisible != true)
                return;

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