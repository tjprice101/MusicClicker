using Avalonia.Controls;
using System;
using MusicClicker;
using MusicClicker.Helpers;

namespace MusicClicker
{
    // Centralized UI update manager that refreshes UI controls to reflect the current GameState.
    // Rewrites here focus on avoiding redundant expensive math (Math.Pow / Math.Round) and
    // reducing repeated property access where possible without changing visible behavior.
    public static class UIUpdater
    {
        // Helper function to format notes with shortened notation (K, M, B, T)
        public static string FormatNotes(double notes)
        {
            if (notes >= 1_000_000_000_000) return $"{notes / 1_000_000_000_000:F2}T";
            if (notes >= 1_000_000_000) return $"{notes / 1_000_000_000:F2}B";
            if (notes >= 1_000_000) return $"{notes / 1_000_000:F2}M";
            if (notes >= 1_000) return $"{notes / 1_000:F2}K";
            return $"{notes:F1}";
        }

        // Single responsibility: update the main upgrade UI quickly and with minimal allocations.
        public static void UpdateUI(MainWindow window, GameState gameState)
        {
            // If the user is interacting (scrolling/dragging), throttle heavy UI updates
            if (window?.IsUserInteracting == true)
                return;
            // Cache frequently used values locally to avoid repeated property access.
            double notes = gameState.Notes;
            double nps = gameState.NotesPerSecond;
            
            // Astral Chainripper: Display 5x NPS when active
            if (gameState.AstralChainripperNpsBoostActive && DateTime.Now <= gameState.AstralChainripperNpsBoostExpiry)
            {
                nps *= 5;
            }
            
            // Update Cacophonic Dreams endgame button (requires 1 trillion NPS)
            if (window.CacophonicDreamsButton != null)
            {
                bool shouldBeEnabled = nps >= 1_000_000_000_000; // 1 trillion NPS
                if (window.CacophonicDreamsButton.IsEnabled != shouldBeEnabled)
                {
                    window.CacophonicDreamsButton.IsEnabled = shouldBeEnabled;
                    window.CacophonicDreamsButton.Opacity = shouldBeEnabled ? 1.0 : 0.4;
                    
                    // Show/hide lock overlay based on unlock status
                    var lockOverlay = window.FindControl<Border>("CacophonicDreamsLockOverlay");
                    if (lockOverlay != null)
                    {
                        lockOverlay.IsVisible = !shouldBeEnabled;
                    }
                }
            }

            // Top-level displays (Notes / NPS) are animated separately by AnimateVisuals
            // which runs at the animation frame rate to produce smooth transitions.

            // Only update screens that are visible to reduce UI churn on hidden views.
            if (window.UpgradeScreen?.IsVisible == true)
            {
                window.UpgradeScreen.UpgradeNotesTextHeader.Text = $"Notes: {NumberFormatter.FormatLargeNumber(notes)}";

                // Owned counts are simple integer-to-string conversions  Einexpensive.
                window.UpgradeScreen.ChordOwnedTextUpgrade.Text = $"Number Owned: {gameState.ChordOwned}";
                window.UpgradeScreen.ScaleOwnedTextUpgrade.Text = $"Number Owned: {gameState.ScaleOwned}";
                window.UpgradeScreen.OrchestraOwnedTextUpgrade.Text = $"Number Owned: {gameState.OrchestraOwned}";
                window.UpgradeScreen.SymphonyOwnedTextUpgrade.Text = $"Number Owned: {gameState.SymphonyOwned}";
                window.UpgradeScreen.AriaOwnedTextUpgrade.Text = $"Number Owned: {gameState.AriaOwned}";
                window.UpgradeScreen.RequiemOwnedTextUpgrade.Text = $"Number Owned: {gameState.RequiemOwned}";
                window.UpgradeScreen.OpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.OpusOwned}";
                window.UpgradeScreen.MagnumOpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.MagnumOpusOwned}";

                // Apply Moonlight Duet Waning phase cost reduction (50% off)
                int moonPhase = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_GetCurrentPhase(gameState);
                bool allMoonPhasesActive = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_AreAllPhasesActive(gameState);
                double costMultiplier = (allMoonPhasesActive || moonPhase == 3) ? 0.5 : 1.0; // Waning phase or all phases active
                
                // Use cached costs to avoid expensive Math.Pow calculations every frame (performance optimization)
                // Cache is invalidated (set to 0) when purchases are made in UpgradeManager
                if (gameState.CachedChordCost == 0)
                    gameState.CachedChordCost = Math.Round(gameState.ChordBaseCost * Math.Pow(1.15, gameState.ChordOwned) * costMultiplier, 2);
                if (gameState.CachedScaleCost == 0)
                    gameState.CachedScaleCost = Math.Round(gameState.ScaleBaseCost * Math.Pow(1.15, gameState.ScaleOwned) * costMultiplier, 2);
                if (gameState.CachedOrchestraCost == 0)
                    gameState.CachedOrchestraCost = Math.Round(gameState.OrchestraBaseCost * Math.Pow(1.15, gameState.OrchestraOwned) * costMultiplier, 2);
                if (gameState.CachedSymphonyCost == 0)
                    gameState.CachedSymphonyCost = Math.Round(gameState.SymphonyBaseCost * Math.Pow(1.15, gameState.SymphonyOwned) * costMultiplier, 2);
                if (gameState.CachedAriaCost == 0)
                    gameState.CachedAriaCost = Math.Round(gameState.AriaBaseCost * Math.Pow(1.15, gameState.AriaOwned) * costMultiplier, 2);
                if (gameState.CachedRequiemCost == 0)
                    gameState.CachedRequiemCost = Math.Round(gameState.RequiemBaseCost * Math.Pow(1.15, gameState.RequiemOwned) * costMultiplier, 2);
                if (gameState.CachedOpusCost == 0)
                    gameState.CachedOpusCost = Math.Round(gameState.OpusBaseCost * Math.Pow(1.15, gameState.OpusOwned) * costMultiplier, 2);
                if (gameState.CachedMagnumOpusCost == 0)
                    gameState.CachedMagnumOpusCost = Math.Round(gameState.MagnumOpusBaseCost * Math.Pow(1.15, gameState.MagnumOpusOwned) * costMultiplier, 2);

                window.UpgradeScreen.ChordCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedChordCost)}";
                window.UpgradeScreen.ScaleCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedScaleCost)}";
                window.UpgradeScreen.OrchestraCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedOrchestraCost)}";
                window.UpgradeScreen.SymphonyCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedSymphonyCost)}";
                window.UpgradeScreen.AriaCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedAriaCost)}";
                window.UpgradeScreen.RequiemCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedRequiemCost)}";
                window.UpgradeScreen.OpusCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedOpusCost)}";
                window.UpgradeScreen.MagnumOpusCostTextUpgrade.Text = $"Cost: {NumberFormatter.FormatLargeNumber(gameState.CachedMagnumOpusCost)}";
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
                // When the user is interacting (scrolling/dragging), avoid expensive
                // per-screen updates but still refresh the lightweight HUD so the
                // Notes counter appears to increase continuously.
                if (window?.IsUserInteracting == true)
                {
                    double displayNotesInteraction = window?.DisplayedNotes ?? gameState.Notes;
                    string notesTextInteraction = $"Notes: {NumberFormatter.FormatLargeNumber(displayNotesInteraction)}";
                    if (window.NotesText.Text != notesTextInteraction) window.NotesText.Text = notesTextInteraction;
                    return;
                }
                // Use the smoothed displayed notes when available (updated at FRAME_RATE by AnimateVisuals).
                double displayNotes = window?.DisplayedNotes ?? gameState.Notes;
                string notesText = $"Notes: {NumberFormatter.FormatLargeNumber(displayNotes)}";

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

                // Armory of Forte screen (direct access avoids FindControl allocations)
                if (window.ArmoryOfForteScreen?.ArmoryNotesText != null && window.ArmoryOfForteScreen.ArmoryNotesText.Text != notesText)
                    window.ArmoryOfForteScreen.ArmoryNotesText.Text = notesText;

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
            
            // Calculate displayed NPS (multiply by 5 if Astral Chainripper boost is active)
            double displayNps = gameState.NotesPerSecond;
            if (gameState.AstralChainripperNpsBoostActive && DateTime.Now <= gameState.AstralChainripperNpsBoostExpiry)
            {
                displayNps *= 5;
            }
            
            // Joyful Catharsis: Double NPS for 5s on petal gain
            if (gameState.JoyfulCatharsisAbility && DateTime.Now <= gameState.JoyfulCatharsisNpsBoostExpiry)
            {
                displayNps *= 2;
            }
            
            window.DisplayedNps = displayNps;

            // Update top-level HUD texts if changed enough to be visible
            // Avoid changing text properties while the user is interacting to prevent layout
            // thrashing that can cause jitter (scrolling, slider drags, etc.). We still update
            // the internal displayed values so the numbers appear smooth once interaction stops.
            string notesText = $"Notes: {NumberFormatter.FormatLargeNumber(window.DisplayedNotes)}";
            string npsText = $"Notes Per Second: {NumberFormatter.FormatLargeNumber(window.DisplayedNps)}";
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

                if (window.ArmoryOfForteScreen?.IsVisible == true && window.ArmoryOfForteScreen.ArmoryNotesText != null)
                {
                    if (window.ArmoryOfForteScreen.ArmoryNotesText.Text != notesText)
                        window.ArmoryOfForteScreen.ArmoryNotesText.Text = notesText;
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

            if (window.ArmoryOfForteScreen?.IsVisible == true && window.ArmoryOfForteScreen.ArmoryNotesText != null)
            {
                if (window.ArmoryOfForteScreen.ArmoryNotesText.Text != notesText)
                    window.ArmoryOfForteScreen.ArmoryNotesText.Text = notesText;
            }
            
            // Update Prayer Stacks display (Funeral Prayer)
            if (window.PrayerStacksDisplayBorder != null && window.PrayerStacksText != null)
            {
                bool shouldShowPrayer = gameState.FuneralPrayerAbility;
                if (window.PrayerStacksDisplayBorder.IsVisible != shouldShowPrayer)
                    window.PrayerStacksDisplayBorder.IsVisible = shouldShowPrayer;
                    
                if (shouldShowPrayer)
                {
                    string prayerText = $"{gameState.FuneralPrayerStacks} / 3";
                    if (window.PrayerStacksText.Text != prayerText)
                        window.PrayerStacksText.Text = prayerText;
                }
            }
            
            // Update Symphonic Catharsis timer display (Eroica Crescendance)
            if (window.MainCatharsisTimerText != null)
            {
                bool catharsisActive = gameState.SymphonicCatharsisActive && gameState.SymphonicCatharsisExpiry > DateTime.Now;
                if (window.MainCatharsisTimerText.IsVisible != catharsisActive)
                    window.MainCatharsisTimerText.IsVisible = catharsisActive;
                    
                if (catharsisActive)
                {
                    TimeSpan remaining = gameState.SymphonicCatharsisExpiry - DateTime.Now;
                    int remainingSeconds = (int)Math.Ceiling(remaining.TotalSeconds);
                    string timerText = $"Active: {remainingSeconds}s";
                    if (window.MainCatharsisTimerText.Text != timerText)
                        window.MainCatharsisTimerText.Text = timerText;
                }
            }
        }

        // Update the fragmentation screen; cheap operations only.
        public static void UpdateFragmentationUI(MainWindow window, GameState gameState)
        {
            if (window.FragmentationScreen?.IsVisible != true)
                return;

            window.FragmentationScreen.FragmentationNotesText.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";
            window.FragmentationScreen.MelodiousOwnedText.Text = $"{gameState.MelodiousOwned} Owned";
            window.FragmentationScreen.HarmoniousOwnedText.Text = $"{gameState.HarmoniousOwned} Owned";
        }

        // Update the Save Scores UI. This method sets many text fields  Ekeep it straightforward
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
            window.SaveScoresScreen.SaveScoresNotesText.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";

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
                // If any controls are not available yet, ignore  Eupdate will run again later.
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
            window.SaveScoresScreen.SaveScoresNotesText.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";

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
                // If any controls are not available yet, ignore  Eupdate will run again later.
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

            window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";
        }

        // Update the Unite the Symphony screen showing completed minor/major scores and refresh
        // the tempo drawer to keep the tuned images in sync with owned state.
        public static void UpdateUnitySymphonyUI(MainWindow window, GameState gameState)
        {
            if (window.UnityTheSymphonyScreen?.IsVisible != true)
                return;
            // Throttle heavy updates during user interaction
            if (window.IsUserInteracting == true) return;

            window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";
            try { window.UnityTheSymphonyScreen.EntropicMelodyText.Text = $"Entropic Melody: {gameState.EntropicMelodies}"; } catch { }

            window.UnityTheSymphonyScreen.MoonlightMinorOwnedText.Text = $"{Math.Max(0, gameState.MoonlightMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.EroicaMinorOwnedText.Text = $"{Math.Max(0, gameState.EroicaMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.SwanMinorOwnedText.Text = $"{Math.Max(0, gameState.SwanMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMinorOwnedText.Text = $"{Math.Max(0, gameState.LaCampanellaMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.EnigmaMinorOwnedText.Text = $"{Math.Max(0, gameState.EnigmaMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.FateMinorOwnedText.Text = $"{Math.Max(0, gameState.FateMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMinorOwnedText.Text = $"{Math.Max(0, gameState.OdeToJoyMinorOwned)} Owned";

            window.UnityTheSymphonyScreen.MoonlightMajorOwnedText.Text = $"{Math.Max(0, gameState.MoonlightMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.EroicaMajorOwnedText.Text = $"{Math.Max(0, gameState.EroicaMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.SwanMajorOwnedText.Text = $"{Math.Max(0, gameState.SwanMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMajorOwnedText.Text = $"{Math.Max(0, gameState.LaCampanellaMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.EnigmaMajorOwnedText.Text = $"{Math.Max(0, gameState.EnigmaMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.FateMajorOwnedText.Text = $"{Math.Max(0, gameState.FateMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMajorOwnedText.Text = $"{Math.Max(0, gameState.OdeToJoyMajorOwned)} Owned";

            // Update component requirement texts with owned counts
            // Moonlight Minor
            window.UnityTheSymphonyScreen.MoonlightMinorKeyText.Text = $"1 Minor Key of Moonlight Sonata ({gameState.MoonlightMinorKeys} owned)";
            window.UnityTheSymphonyScreen.MoonlightMinorScaleText.Text = $"1 Minor Scale of Moonlight Sonata ({gameState.MoonlightMinorScales} owned)";
            window.UnityTheSymphonyScreen.MoonlightMinorProgressionText.Text = $"1 Minor Progression of Moonlight Sonata ({gameState.MoonlightMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.MoonlightMinorMelodiousText.Text = $"10 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Moonlight Major
            window.UnityTheSymphonyScreen.MoonlightMajorKeyText.Text = $"1 Major Key of Moonlight Sonata ({gameState.MoonlightMajorKeys} owned)";
            window.UnityTheSymphonyScreen.MoonlightMajorScaleText.Text = $"1 Major Scale of Moonlight Sonata ({gameState.MoonlightMajorScales} owned)";
            window.UnityTheSymphonyScreen.MoonlightMajorProgressionText.Text = $"1 Major Progression of Moonlight Sonata ({gameState.MoonlightMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.MoonlightMajorHarmoniousText.Text = $"10 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Eroica Minor
            window.UnityTheSymphonyScreen.EroicaMinorKeyText.Text = $"1 Minor Key of Eroica ({gameState.EroicaMinorKeys} owned)";
            window.UnityTheSymphonyScreen.EroicaMinorScaleText.Text = $"1 Minor Scale of Eroica ({gameState.EroicaMinorScales} owned)";
            window.UnityTheSymphonyScreen.EroicaMinorProgressionText.Text = $"1 Minor Progression of Eroica ({gameState.EroicaMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.EroicaMinorMelodiousText.Text = $"15 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Eroica Major
            window.UnityTheSymphonyScreen.EroicaMajorKeyText.Text = $"1 Major Key of Eroica ({gameState.EroicaMajorKeys} owned)";
            window.UnityTheSymphonyScreen.EroicaMajorScaleText.Text = $"1 Major Scale of Eroica ({gameState.EroicaMajorScales} owned)";
            window.UnityTheSymphonyScreen.EroicaMajorProgressionText.Text = $"1 Major Progression of Eroica ({gameState.EroicaMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.EroicaMajorHarmoniousText.Text = $"15 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Swan Minor
            window.UnityTheSymphonyScreen.SwanMinorKeyText.Text = $"1 Minor Key of Swan Lake ({gameState.SwanLakeMinorKeys} owned)";
            window.UnityTheSymphonyScreen.SwanMinorScaleText.Text = $"1 Minor Scale of Swan Lake ({gameState.SwanLakeMinorScales} owned)";
            window.UnityTheSymphonyScreen.SwanMinorProgressionText.Text = $"1 Minor Progression of Swan Lake ({gameState.SwanLakeMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.SwanMinorMelodiousText.Text = $"20 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Swan Major
            window.UnityTheSymphonyScreen.SwanMajorKeyText.Text = $"1 Major Key of Swan Lake ({gameState.SwanLakeMajorKeys} owned)";
            window.UnityTheSymphonyScreen.SwanMajorScaleText.Text = $"1 Major Scale of Swan Lake ({gameState.SwanLakeMajorScales} owned)";
            window.UnityTheSymphonyScreen.SwanMajorProgressionText.Text = $"1 Major Progression of Swan Lake ({gameState.SwanLakeMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.SwanMajorHarmoniousText.Text = $"20 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // LaCampanella Minor
            window.UnityTheSymphonyScreen.LaCampanellaMinorKeyText.Text = $"1 Minor Key of La Campanella ({gameState.LaCampanellaMinorKeys} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMinorScaleText.Text = $"1 Minor Scale of La Campanella ({gameState.LaCampanellaMinorScales} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMinorProgressionText.Text = $"1 Minor Progression of La Campanella ({gameState.LaCampanellaMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMinorMelodiousText.Text = $"25 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // LaCampanella Major
            window.UnityTheSymphonyScreen.LaCampanellaMajorKeyText.Text = $"1 Major Key of La Campanella ({gameState.LaCampanellaMajorKeys} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMajorScaleText.Text = $"1 Major Scale of La Campanella ({gameState.LaCampanellaMajorScales} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMajorProgressionText.Text = $"1 Major Progression of La Campanella ({gameState.LaCampanellaMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMajorHarmoniousText.Text = $"25 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Enigma Minor
            window.UnityTheSymphonyScreen.EnigmaMinorKeyText.Text = $"1 Minor Key of Enigma ({gameState.EnigmaMinorKeys} owned)";
            window.UnityTheSymphonyScreen.EnigmaMinorScaleText.Text = $"1 Minor Scale of Enigma ({gameState.EnigmaMinorScales} owned)";
            window.UnityTheSymphonyScreen.EnigmaMinorProgressionText.Text = $"1 Minor Progression of Enigma ({gameState.EnigmaMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.EnigmaMinorMelodiousText.Text = $"30 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Enigma Major
            window.UnityTheSymphonyScreen.EnigmaMajorKeyText.Text = $"1 Major Key of Enigma ({gameState.EnigmaMajorKeys} owned)";
            window.UnityTheSymphonyScreen.EnigmaMajorScaleText.Text = $"1 Major Scale of Enigma ({gameState.EnigmaMajorScales} owned)";
            window.UnityTheSymphonyScreen.EnigmaMajorProgressionText.Text = $"1 Major Progression of Enigma ({gameState.EnigmaMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.EnigmaMajorHarmoniousText.Text = $"30 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Fate Minor
            window.UnityTheSymphonyScreen.FateMinorKeyText.Text = $"1 Minor Key of Fate ({gameState.FateMinorKeys} owned)";
            window.UnityTheSymphonyScreen.FateMinorScaleText.Text = $"1 Minor Scale of Fate ({gameState.FateMinorScales} owned)";
            window.UnityTheSymphonyScreen.FateMinorProgressionText.Text = $"1 Minor Progression of Fate ({gameState.FateMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.FateMinorMelodiousText.Text = $"35 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Fate Major
            window.UnityTheSymphonyScreen.FateMajorKeyText.Text = $"1 Major Key of Fate ({gameState.FateMajorKeys} owned)";
            window.UnityTheSymphonyScreen.FateMajorScaleText.Text = $"1 Major Scale of Fate ({gameState.FateMajorScales} owned)";
            window.UnityTheSymphonyScreen.FateMajorProgressionText.Text = $"1 Major Progression of Fate ({gameState.FateMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.FateMajorHarmoniousText.Text = $"35 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // OdeToJoy Minor
            window.UnityTheSymphonyScreen.OdeToJoyMinorKeyText.Text = $"1 Minor Key of Ode to Joy ({gameState.OdeToJoyMinorKeys} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMinorScaleText.Text = $"1 Minor Scale of Ode to Joy ({gameState.OdeToJoyMinorScales} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMinorProgressionText.Text = $"1 Minor Progression of Ode to Joy ({gameState.OdeToJoyMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMinorMelodiousText.Text = $"40 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // OdeToJoy Major
            window.UnityTheSymphonyScreen.OdeToJoyMajorKeyText.Text = $"1 Major Key of Ode to Joy ({gameState.OdeToJoyMajorKeys} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMajorScaleText.Text = $"1 Major Scale of Ode to Joy ({gameState.OdeToJoyMajorScales} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMajorProgressionText.Text = $"1 Major Progression of Ode to Joy ({gameState.OdeToJoyMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMajorHarmoniousText.Text = $"40 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            MainWindow.GlobalTempoManager.RefreshDrawer();
        }

        // Immediate variant: bypass interaction throttling so rapid crafts update counts.
        public static void UpdateUnitySymphonyUIImmediate(MainWindow window, GameState gameState)
        {
            if (window.UnityTheSymphonyScreen?.IsVisible != true)
                return;

            window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";
            try { window.UnityTheSymphonyScreen.EntropicMelodyText.Text = $"Entropic Melody: {gameState.EntropicMelodies}"; } catch { }

            window.UnityTheSymphonyScreen.MoonlightMinorOwnedText.Text = $"{Math.Max(0, gameState.MoonlightMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.EroicaMinorOwnedText.Text = $"{Math.Max(0, gameState.EroicaMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.SwanMinorOwnedText.Text = $"{Math.Max(0, gameState.SwanMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMinorOwnedText.Text = $"{Math.Max(0, gameState.LaCampanellaMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.EnigmaMinorOwnedText.Text = $"{Math.Max(0, gameState.EnigmaMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.FateMinorOwnedText.Text = $"{Math.Max(0, gameState.FateMinorOwned)} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMinorOwnedText.Text = $"{Math.Max(0, gameState.OdeToJoyMinorOwned)} Owned";

            window.UnityTheSymphonyScreen.MoonlightMajorOwnedText.Text = $"{Math.Max(0, gameState.MoonlightMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.EroicaMajorOwnedText.Text = $"{Math.Max(0, gameState.EroicaMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.SwanMajorOwnedText.Text = $"{Math.Max(0, gameState.SwanMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMajorOwnedText.Text = $"{Math.Max(0, gameState.LaCampanellaMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.EnigmaMajorOwnedText.Text = $"{Math.Max(0, gameState.EnigmaMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.FateMajorOwnedText.Text = $"{Math.Max(0, gameState.FateMajorOwned)} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMajorOwnedText.Text = $"{Math.Max(0, gameState.OdeToJoyMajorOwned)} Owned";

            // Update component requirement texts with owned counts
            // Moonlight Minor
            window.UnityTheSymphonyScreen.MoonlightMinorKeyText.Text = $"1 Minor Key of Moonlight Sonata ({gameState.MoonlightMinorKeys} owned)";
            window.UnityTheSymphonyScreen.MoonlightMinorScaleText.Text = $"1 Minor Scale of Moonlight Sonata ({gameState.MoonlightMinorScales} owned)";
            window.UnityTheSymphonyScreen.MoonlightMinorProgressionText.Text = $"1 Minor Progression of Moonlight Sonata ({gameState.MoonlightMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.MoonlightMinorMelodiousText.Text = $"10 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Moonlight Major
            window.UnityTheSymphonyScreen.MoonlightMajorKeyText.Text = $"1 Major Key of Moonlight Sonata ({gameState.MoonlightMajorKeys} owned)";
            window.UnityTheSymphonyScreen.MoonlightMajorScaleText.Text = $"1 Major Scale of Moonlight Sonata ({gameState.MoonlightMajorScales} owned)";
            window.UnityTheSymphonyScreen.MoonlightMajorProgressionText.Text = $"1 Major Progression of Moonlight Sonata ({gameState.MoonlightMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.MoonlightMajorHarmoniousText.Text = $"10 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Eroica Minor
            window.UnityTheSymphonyScreen.EroicaMinorKeyText.Text = $"1 Minor Key of Eroica ({gameState.EroicaMinorKeys} owned)";
            window.UnityTheSymphonyScreen.EroicaMinorScaleText.Text = $"1 Minor Scale of Eroica ({gameState.EroicaMinorScales} owned)";
            window.UnityTheSymphonyScreen.EroicaMinorProgressionText.Text = $"1 Minor Progression of Eroica ({gameState.EroicaMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.EroicaMinorMelodiousText.Text = $"15 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Eroica Major
            window.UnityTheSymphonyScreen.EroicaMajorKeyText.Text = $"1 Major Key of Eroica ({gameState.EroicaMajorKeys} owned)";
            window.UnityTheSymphonyScreen.EroicaMajorScaleText.Text = $"1 Major Scale of Eroica ({gameState.EroicaMajorScales} owned)";
            window.UnityTheSymphonyScreen.EroicaMajorProgressionText.Text = $"1 Major Progression of Eroica ({gameState.EroicaMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.EroicaMajorHarmoniousText.Text = $"15 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Swan Minor
            window.UnityTheSymphonyScreen.SwanMinorKeyText.Text = $"1 Minor Key of Swan Lake ({gameState.SwanLakeMinorKeys} owned)";
            window.UnityTheSymphonyScreen.SwanMinorScaleText.Text = $"1 Minor Scale of Swan Lake ({gameState.SwanLakeMinorScales} owned)";
            window.UnityTheSymphonyScreen.SwanMinorProgressionText.Text = $"1 Minor Progression of Swan Lake ({gameState.SwanLakeMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.SwanMinorMelodiousText.Text = $"20 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Swan Major
            window.UnityTheSymphonyScreen.SwanMajorKeyText.Text = $"1 Major Key of Swan Lake ({gameState.SwanLakeMajorKeys} owned)";
            window.UnityTheSymphonyScreen.SwanMajorScaleText.Text = $"1 Major Scale of Swan Lake ({gameState.SwanLakeMajorScales} owned)";
            window.UnityTheSymphonyScreen.SwanMajorProgressionText.Text = $"1 Major Progression of Swan Lake ({gameState.SwanLakeMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.SwanMajorHarmoniousText.Text = $"20 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // LaCampanella Minor
            window.UnityTheSymphonyScreen.LaCampanellaMinorKeyText.Text = $"1 Minor Key of La Campanella ({gameState.LaCampanellaMinorKeys} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMinorScaleText.Text = $"1 Minor Scale of La Campanella ({gameState.LaCampanellaMinorScales} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMinorProgressionText.Text = $"1 Minor Progression of La Campanella ({gameState.LaCampanellaMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMinorMelodiousText.Text = $"25 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // LaCampanella Major
            window.UnityTheSymphonyScreen.LaCampanellaMajorKeyText.Text = $"1 Major Key of La Campanella ({gameState.LaCampanellaMajorKeys} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMajorScaleText.Text = $"1 Major Scale of La Campanella ({gameState.LaCampanellaMajorScales} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMajorProgressionText.Text = $"1 Major Progression of La Campanella ({gameState.LaCampanellaMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.LaCampanellaMajorHarmoniousText.Text = $"25 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Enigma Minor
            window.UnityTheSymphonyScreen.EnigmaMinorKeyText.Text = $"1 Minor Key of Enigma ({gameState.EnigmaMinorKeys} owned)";
            window.UnityTheSymphonyScreen.EnigmaMinorScaleText.Text = $"1 Minor Scale of Enigma ({gameState.EnigmaMinorScales} owned)";
            window.UnityTheSymphonyScreen.EnigmaMinorProgressionText.Text = $"1 Minor Progression of Enigma ({gameState.EnigmaMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.EnigmaMinorMelodiousText.Text = $"30 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Enigma Major
            window.UnityTheSymphonyScreen.EnigmaMajorKeyText.Text = $"1 Major Key of Enigma ({gameState.EnigmaMajorKeys} owned)";
            window.UnityTheSymphonyScreen.EnigmaMajorScaleText.Text = $"1 Major Scale of Enigma ({gameState.EnigmaMajorScales} owned)";
            window.UnityTheSymphonyScreen.EnigmaMajorProgressionText.Text = $"1 Major Progression of Enigma ({gameState.EnigmaMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.EnigmaMajorHarmoniousText.Text = $"30 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // Fate Minor
            window.UnityTheSymphonyScreen.FateMinorKeyText.Text = $"1 Minor Key of Fate ({gameState.FateMinorKeys} owned)";
            window.UnityTheSymphonyScreen.FateMinorScaleText.Text = $"1 Minor Scale of Fate ({gameState.FateMinorScales} owned)";
            window.UnityTheSymphonyScreen.FateMinorProgressionText.Text = $"1 Minor Progression of Fate ({gameState.FateMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.FateMinorMelodiousText.Text = $"35 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // Fate Major
            window.UnityTheSymphonyScreen.FateMajorKeyText.Text = $"1 Major Key of Fate ({gameState.FateMajorKeys} owned)";
            window.UnityTheSymphonyScreen.FateMajorScaleText.Text = $"1 Major Scale of Fate ({gameState.FateMajorScales} owned)";
            window.UnityTheSymphonyScreen.FateMajorProgressionText.Text = $"1 Major Progression of Fate ({gameState.FateMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.FateMajorHarmoniousText.Text = $"35 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            // OdeToJoy Minor
            window.UnityTheSymphonyScreen.OdeToJoyMinorKeyText.Text = $"1 Minor Key of Ode to Joy ({gameState.OdeToJoyMinorKeys} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMinorScaleText.Text = $"1 Minor Scale of Ode to Joy ({gameState.OdeToJoyMinorScales} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMinorProgressionText.Text = $"1 Minor Progression of Ode to Joy ({gameState.OdeToJoyMinorProgressions} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMinorMelodiousText.Text = $"40 Melodious Fragments ({gameState.MelodiousOwned} owned)";

            // OdeToJoy Major
            window.UnityTheSymphonyScreen.OdeToJoyMajorKeyText.Text = $"1 Major Key of Ode to Joy ({gameState.OdeToJoyMajorKeys} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMajorScaleText.Text = $"1 Major Scale of Ode to Joy ({gameState.OdeToJoyMajorScales} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMajorProgressionText.Text = $"1 Major Progression of Ode to Joy ({gameState.OdeToJoyMajorProgressions} owned)";
            window.UnityTheSymphonyScreen.OdeToJoyMajorHarmoniousText.Text = $"40 Harmonious Fragments ({gameState.HarmoniousOwned} owned)";

            MainWindow.GlobalTempoManager.RefreshDrawer();
        }
    }
}
