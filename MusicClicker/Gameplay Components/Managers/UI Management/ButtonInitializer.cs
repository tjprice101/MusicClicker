using Avalonia.Controls;
using System;
using System.Threading.Tasks;

namespace MusicClicker
{
    public static class ButtonInitializer
    {
        // Aggregate initializer that wires every UI button to its handler.
        // This single entry point is called during window initialization and keeps
        // button wiring centralized for easier lifecycle management and testing.
        public static void InitializeAllButtons(MainWindow window)
        {
            InitializeHarmonyButtons(window);
            InitializeUpgradeButtons(window);
            InitializeClicker(window);
            InitializeNavigationButtons(window);
            InitializeFragmentationButtons(window);
            InitializeSaveScoresButtons(window);
            InitializeManualCrafting(window);
        }

        // Shared RNG for save-score background tasks (thread-safe via lock)
        private static readonly Random _saveRandom = new Random();
        private static readonly object _saveRandomLock = new object();

        // Helper to subscribe a button click and ensure the UI is refreshed immediately
        // after the action completes. This centralizes the post-click UI update policy
        // so any button that gives the player something will refresh the HUD/screens.
        private static void SubscribeWithAutoUpdate(Button btn, MainWindow window, Action action)
        {
            btn.Click += (s, e) =>
            {
                try { action(); }
                catch (Exception)
                {
                    // Swallow to avoid crashing on UI thread; UI will still be updated.
                }

                // Immediate, lightweight updates: update top HUD Notes and any visible per-screen minimal texts.
                try
                {
                    var gs = window.GameState;
                    // Force the displayed counters to match the underlying GameState immediately
                    // so rapid clicks reflect instantly in the HUD rather than waiting for the
                    // animation lerp to catch up.
                    try
                    {
                        window.DisplayedNotes = gs.Notes;
                        window.DisplayedNps = gs.NotesPerSecond;
                    }
                    catch { }

                    string notesText = $"Notes: {Math.Round(gs.Notes, 1)}";
                    if (window.NotesText != null && window.NotesText.Text != notesText) window.NotesText.Text = notesText;

                    // Quick per-screen minimal updates (avoid heavy work while clicking rapidly)
                    if (window.SaveScoresScreen?.SaveScoresNotesText != null)
                        window.SaveScoresScreen.SaveScoresNotesText.Text = notesText;

                    if (window.HeartOfHarmonyScreen?.HeartOfHarmonyNotesText != null)
                        window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text = notesText;

                    if (window.UnityTheSymphonyScreen?.UnityNotesTextHeader != null)
                        window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = notesText;

                    if (window.ArmoryOfForteScreen?.ArmoryNotesText != null)
                        window.ArmoryOfForteScreen.ArmoryNotesText.Text = notesText;
                }
                catch (Exception)
                {
                    // ignore
                }

                // Schedule a fuller UI refresh on the UI thread at low priority to avoid blocking click handling.
                try
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                                try {
                                    UIUpdater.UpdateNotesOnly(window, window.GameState);
                                    UIUpdater.UpdateSaveScoresUIImmediate(window, window.GameState);
                                    UIUpdater.UpdateHeartOfHarmonyUI(window, window.GameState);
                                    UIUpdater.UpdateUnitySymphonyUIImmediate(window, window.GameState);
                                    UIUpdater.UpdateFragmentationUI(window, window.GameState);
                                }
                        catch (Exception) { }
                    }, Avalonia.Threading.DispatcherPriority.Background);
                }
                catch (Exception)
                {
                    // If Dispatcher not available, fall back to synchronous update (best-effort).
                    try { UIUpdater.UpdateUI(window, window.GameState); } catch { }
                }
            };
        }

        // Wire handlers for Heart of Harmony conversion UI elements.
        // Each button delegates to HeartOfHarmonyManager.HandleMajorSheetClick and
        // passes GameState fields by reference so the manager can mutate them directly.
        private static void InitializeHarmonyButtons(MainWindow window)
        {
            // Moonlight Sonata conversions
            SubscribeWithAutoUpdate(window.HeartOfHarmonyScreen.MoonlightHarmonyButton, window, () =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._moonlightMajorSheets,
                    ref window.GameState._moonlightMajorKeys, ref window.GameState._moonlightMajorScales,
                    ref window.GameState._moonlightMajorProgressions,
                    window.HeartOfHarmonyScreen.MoonlightMajorSheetsOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.MoonlightMajorScalesOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorProgressionsOwnedText, "Moonlight Sonata"));

            // Eroica conversions
            SubscribeWithAutoUpdate(window.HeartOfHarmonyScreen.EroicaHarmonyButton, window, () =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._eroicaMajorSheets,
                    ref window.GameState._eroicaMajorKeys, ref window.GameState._eroicaMajorScales,
                    ref window.GameState._eroicaMajorProgressions,
                    window.HeartOfHarmonyScreen.EroicaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EroicaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.EroicaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EroicaMajorProgressionsOwnedText, "Eroica"));

            // Swan Lake conversions
            SubscribeWithAutoUpdate(window.HeartOfHarmonyScreen.SwanLakeHarmonyButton, window, () =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._swanLakeMajorSheets,
                    ref window.GameState._swanLakeMajorKeys, ref window.GameState._swanLakeMajorScales,
                    ref window.GameState._swanLakeMajorProgressions,
                    window.HeartOfHarmonyScreen.SwanLakeMajorSheetsOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.SwanLakeMajorScalesOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorProgressionsOwnedText, "Swan Lake"));

            // La Campanella conversions
            SubscribeWithAutoUpdate(window.HeartOfHarmonyScreen.LaCampanellaHarmonyButton, window, () =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._laCampanellaMajorSheets,
                    ref window.GameState._laCampanellaMajorKeys, ref window.GameState._laCampanellaMajorScales,
                    ref window.GameState._laCampanellaMajorProgressions,
                    window.HeartOfHarmonyScreen.LaCampanellaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.LaCampanellaMajorScalesOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorProgressionsOwnedText, "La Campanella"));

            // Enigma conversions
            SubscribeWithAutoUpdate(window.HeartOfHarmonyScreen.EnigmaHarmonyButton, window, () =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._enigmaMajorSheets,
                    ref window.GameState._enigmaMajorKeys, ref window.GameState._enigmaMajorScales,
                    ref window.GameState._enigmaMajorProgressions,
                    window.HeartOfHarmonyScreen.EnigmaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.EnigmaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorProgressionsOwnedText, "Enigma"));

            // Fate conversions
            SubscribeWithAutoUpdate(window.HeartOfHarmonyScreen.FateHarmonyButton, window, () =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._fateMajorSheets,
                    ref window.GameState._fateMajorKeys, ref window.GameState._fateMajorScales,
                    ref window.GameState._fateMajorProgressions,
                    window.HeartOfHarmonyScreen.FateMajorSheetsOwnedText, window.HeartOfHarmonyScreen.FateMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.FateMajorScalesOwnedText, window.HeartOfHarmonyScreen.FateMajorProgressionsOwnedText, "Fate"));

            // Ode to Joy conversions
            SubscribeWithAutoUpdate(window.HeartOfHarmonyScreen.OdeToJoyHarmonyButton, window, () =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._odeToJoyMajorSheets,
                    ref window.GameState._odeToJoyMajorKeys, ref window.GameState._odeToJoyMajorScales,
                    ref window.GameState._odeToJoyMajorProgressions,
                    window.HeartOfHarmonyScreen.OdeToJoyMajorSheetsOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.OdeToJoyMajorScalesOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorProgressionsOwnedText, "Ode to Joy"));
        }

        // Wire handlers for upgrade purchase buttons. Each button calls UpgradeManager.BuyUpgrade.
        // Note: "Max" buttons pass double.MaxValue to request buying until funds are exhausted.
        private static void InitializeUpgradeButtons(MainWindow window)
        {
            // Chord
            window.UpgradeScreen.BuyChordButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._chordOwned, window.GameState.ChordBaseCost,
                    window.GameState.ChordBaseNpsEffect, window.GameState.ChordNpsGrowth, 0.0, 1.0, 1, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);

            window.UpgradeScreen.BuyChordMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._chordOwned, window.GameState.ChordBaseCost,
                    window.GameState.ChordBaseNpsEffect, window.GameState.ChordNpsGrowth, 0.0, 1.0, double.MaxValue, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);

            // Scale
            window.UpgradeScreen.BuyScaleButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._scaleOwned, window.GameState.ScaleBaseCost,
                    window.GameState.ScaleBaseNpsEffect, window.GameState.ScaleNpsGrowth, 0.0, 1.0, 1, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);

            window.UpgradeScreen.BuyScaleMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._scaleOwned, window.GameState.ScaleBaseCost,
                    window.GameState.ScaleBaseNpsEffect, window.GameState.ScaleNpsGrowth, 0.0, 1.0, double.MaxValue, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);

            // Orchestra
            window.UpgradeScreen.BuyOrchestraButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._orchestraOwned, window.GameState.OrchestraBaseCost,
                    window.GameState.OrchestraBaseNpsEffect, window.GameState.OrchestraNpsGrowth, 0.0, 1.0, 1, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);

            window.UpgradeScreen.BuyOrchestraMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._orchestraOwned, window.GameState.OrchestraBaseCost,
                    window.GameState.OrchestraBaseNpsEffect, window.GameState.OrchestraNpsGrowth, 0.0, 1.0, double.MaxValue, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);

            // Symphony
            window.UpgradeScreen.BuySymphonyButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._symphonyOwned, window.GameState.SymphonyBaseCost,
                    window.GameState.SymphonyBaseNpsEffect, window.GameState.SymphonyNpsGrowth, 0.0, 1.0, 1, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);

            window.UpgradeScreen.BuySymphonyMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._symphonyOwned, window.GameState.SymphonyBaseCost,
                    window.GameState.SymphonyBaseNpsEffect, window.GameState.SymphonyNpsGrowth, 0.0, 1.0, double.MaxValue, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);

            // Aria
            window.UpgradeScreen.BuyAriaButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._ariaOwned, window.GameState.AriaBaseCost,
                    0.0, 1.0, window.GameState.AriaBaseClickEffect, window.GameState.AriaClickGrowth, 1, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);

            window.UpgradeScreen.BuyAriaMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._ariaOwned, window.GameState.AriaBaseCost,
                    0.0, 1.0, window.GameState.AriaBaseClickEffect, window.GameState.AriaClickGrowth, double.MaxValue, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);

            // Requiem
            window.UpgradeScreen.BuyRequiemButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._requiemOwned, window.GameState.RequiemBaseCost,
                    0.0, 1.0, window.GameState.RequiemBaseClickEffect, window.GameState.RequiemClickGrowth, 1, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);

            window.UpgradeScreen.BuyRequiemMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._requiemOwned, window.GameState.RequiemBaseCost,
                    0.0, 1.0, window.GameState.RequiemBaseClickEffect, window.GameState.RequiemClickGrowth, double.MaxValue, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);

            // Opus
            window.UpgradeScreen.BuyOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._opusOwned, window.GameState.OpusBaseCost,
                    0.0, 1.0, window.GameState.OpusBaseClickEffect, window.GameState.OpusClickGrowth, 1, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);

            window.UpgradeScreen.BuyOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._opusOwned, window.GameState.OpusBaseCost,
                    0.0, 1.0, window.GameState.OpusBaseClickEffect, window.GameState.OpusClickGrowth, double.MaxValue, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);

            // Magnum Opus
            window.UpgradeScreen.BuyMagnumOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._magnumOpusOwned, window.GameState.MagnumOpusBaseCost,
                    0.0, 1.0, window.GameState.MagnumOpusBaseClickEffect, window.GameState.MagnumOpusClickGrowth, 1, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);

            window.UpgradeScreen.BuyMagnumOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._magnumOpusOwned, window.GameState.MagnumOpusBaseCost,
                    0.0, 1.0, window.GameState.MagnumOpusBaseClickEffect, window.GameState.MagnumOpusClickGrowth, double.MaxValue, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);
        }

        // Wire clicker button and keyboard event to the main click handler.
        // Click events are routed to MainWindow's implementations so this method keeps
        // the initialization logic in one place.
        private static void InitializeClicker(MainWindow window)
        {
            window.ClickButton.Click += window.ClickButton_Click;
            window.KeyDown += window.MainWindow_KeyDown;
        }

        // Wire navigation buttons that show/hide different screens and trigger UI updates.
        private static void InitializeNavigationButtons(MainWindow window)
        {
            // Open upgrades menu (use transition)
            window.OpenUpgradesButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.UpgradeScreen.IsVisible = true;
                    UIUpdater.UpdateUI(window, window.GameState);
                });
            };
            window.UpgradeScreen.BackButtonBottom.Click += window.BackButton_Click;

            // Top-right Upgrades button
            window.TopOpenUpgradesButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.UpgradeScreen.IsVisible = true;
                    UIUpdater.UpdateUI(window, window.GameState);
                });
            };

            // Bottom-right Duet Ability button
            window.OpenDuetAbilityButton.Click += (s, e) =>
            {
                if (window.DuetAbilityPanelContainer != null && window.DuetAbilityScreen != null)
                {
                    window.DuetAbilityPanelContainer.IsVisible = !window.DuetAbilityPanelContainer.IsVisible;
                    if (window.DuetAbilityPanelContainer.IsVisible)
                    {
                        window.DuetAbilityScreen.UpdateAbilityDisplay();
                        window.DuetAbilityScreen.StartCooldownTimer();
                    }
                    else
                    {
                        window.DuetAbilityScreen.StopCooldownTimer();
                    }
                }
            };

            // Top-left Save button
            window.TopSaveButton.Click += (s, e) =>
            {
                // Attempt to save GameState
                string err;
                bool ok = MusicClicker.Helpers.SaveManager.Save(window.GameState, out err);
                if (ok)
                {
                    window.SaveStatusText.Text = "Save Success!";
                }
                else
                {
                    window.SaveStatusText.Text = "Save Failed.";
                }
            };

            // Quit button - Save and close application
            window.QuitButton.Click += (s, e) =>
            {
                // Save first
                string err;
                bool ok = MusicClicker.Helpers.SaveManager.Save(window.GameState, out err);
                
                // Close the application
                window.Close();
            };

            // Fragmentation
            window.FragmentationButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.FragmentationScreen.IsVisible = true;
                    UIUpdater.UpdateFragmentationUI(window, window.GameState);
                });
            };
            
            // Bottom Fragmentation Button removed (duplicate)
            
            window.FragmentationScreen.BackButtonFragmentation.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.FragmentationScreen.IsVisible = false;
                    window.MainScreen.IsVisible = true;
                });
                UIUpdater.UpdateUI(window, window.GameState);
                MainWindow.GlobalTempoManager?.RefreshDrawer();
            };

            // Save Scores
            window.ResonanceButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.SaveScoresScreen.IsVisible = true;
                    UIUpdater.UpdateSaveScoresUI(window, window.GameState);
                });
            };
            window.SaveScoresScreen.BackButtonSaveScores.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.SaveScoresScreen.IsVisible = false;
                    window.MainScreen.IsVisible = true;
                });
                UIUpdater.UpdateUI(window, window.GameState);
                MainWindow.GlobalTempoManager?.RefreshDrawer();
            };

            // Heart of Harmony
            window.MelodyButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.HeartOfHarmonyScreen.IsVisible = true;
                    UIUpdater.UpdateHeartOfHarmonyUI(window, window.GameState);
                });
            };
            window.HeartOfHarmonyScreen.BackButtonHeartOfHarmony.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.HeartOfHarmonyScreen.IsVisible = false;
                    window.MainScreen.IsVisible = true;
                });
                UIUpdater.UpdateUI(window, window.GameState);
                MainWindow.GlobalTempoManager?.RefreshDrawer();
            };

            // Unity the Symphony crafting menu
            window.HarmonyButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.UnityTheSymphonyScreen.IsVisible = true;
                    // Ensure the screen has access to the current GameState so dissolve
                    // buttons and other handlers can operate on it.
                    try { window.UnityTheSymphonyScreen.SetGameState(window.GameState, window); } catch { }
                    UIUpdater.UpdateUnitySymphonyUI(window, window.GameState);
                });
            };
            window.UnityTheSymphonyScreen.BackButtonUnitySymphony.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.UnityTheSymphonyScreen.IsVisible = false;
                    window.MainScreen.IsVisible = true;
                });
                UIUpdater.UpdateUI(window, window.GameState);
                MainWindow.GlobalTempoManager?.RefreshDrawer();
            };

            // Armory of Forte
            window.ArmoryOfForteButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.ArmoryOfForteScreen.IsVisible = true;
                    window.ArmoryOfForteScreen.UpdateUI(window.GameState);
                });
            };

            // Eternal Modulation
            window.EternalModulationButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.EternalModulationScreen.IsVisible = true;
                });
            };

            // Symphonic Gallery - Main hub
            window.SymphonicGalleryButton.Click += async (s, e) =>
            {
                await window.TransitionAsync(() =>
                {
                    window.MainScreen.IsVisible = false;
                    window.SymphonicGalleryScreen.IsVisible = true;
                });
            };

            
        }

        // Wire the fragmentation buttons which convert Notes into Melodious/Harmonious fragments.
        // Handlers check affordability and update GameState then refresh related UI sections.
        private static void InitializeFragmentationButtons(MainWindow window)
        {
            // Buy Melodious Fragments
            SubscribeWithAutoUpdate(window.FragmentationScreen.MelodiousFragmentButton, window, () =>
            {
                if (MusicClicker.Helpers.AtomicDouble.Read(ref window.GameState._notes) >= 1000)
                {
                    MusicClicker.Helpers.AtomicDouble.Add(ref window.GameState._notes, -1000);
                    window.GameState.MelodiousOwned++;
                    
                    // Bank for Fate Duet Hourglass
                    if (window.GameState.FateDuetActive && !window.GameState.FateDuetHasFlipped && DateTime.Now <= window.GameState.FateDuetExpiry)
                    {
                        MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(window.GameState, "BuyFragment", ("Melodious", 1000.0));
                    }
                    
                    // Star-Scattered Wings passive now handled through click tracking (every 10th click grants fragments)
                }
            });

            // Buy Harmonious Fragments
            SubscribeWithAutoUpdate(window.FragmentationScreen.HarmoniousFragmentButton, window, () =>
            {
                if (MusicClicker.Helpers.AtomicDouble.Read(ref window.GameState._notes) >= 5000)
                {
                    MusicClicker.Helpers.AtomicDouble.Add(ref window.GameState._notes, -5000);
                    window.GameState.HarmoniousOwned++;
                    
                    // Bank for Fate Duet Hourglass
                    if (window.GameState.FateDuetActive && !window.GameState.FateDuetHasFlipped && DateTime.Now <= window.GameState.FateDuetExpiry)
                    {
                        MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(window.GameState, "BuyFragment", ("Harmonious", 5000.0));
                    }
                    
                    // Thousand Winged Swan passive now handled through score acquisition (not fragment purchase)
                    
                    // Winter weapon passives no longer trigger on Harmonious purchases (removed Blizzard's Bounty mechanic)
                }
            });
        }

        // Wire Save Scores buttons which spend fixed costs to attempt randomized fragment drops.
        // Each handler delegates to SaveScoresManager.HandleSaveScoreClick for the core logic.
        private static void InitializeSaveScoresButtons(MainWindow window)
        {
            // La Campanella (async background rolls)
            SubscribeWithAutoUpdate(window.SaveScoresScreen.LaCampanellaButton, window, () =>
            {
                var gs = window.GameState;
                const int cost = 135000;
                if (MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes) < cost) return;
                MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);

                Task.Run(() =>
                {
                    int addKeys = 0, addScales = 0, addProgressions = 0, addMajors = 0;
                    lock (_saveRandomLock) { if (_saveRandom.NextDouble() <= 0.50) addKeys = 1; if (_saveRandom.NextDouble() <= 0.25) addScales = 1; if (_saveRandom.NextDouble() <= 0.15) addProgressions = 1; if (_saveRandom.NextDouble() <= 0.05) addMajors = 1; }
                    if (gs.OdeToJoyMajorAbility) addMajors++;
                    lock (gs) { gs._laCampanellaMinorKeys += addKeys; gs._laCampanellaMinorScales += addScales; gs._laCampanellaMinorProgressions += addProgressions; gs._laCampanellaMajorSheets += addMajors; }
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => UIUpdater.UpdateSaveScoresUIImmediate(window, gs), Avalonia.Threading.DispatcherPriority.Background);
                });
            });

            // Enigma
            SubscribeWithAutoUpdate(window.SaveScoresScreen.EnigmaButton, window, () =>
            {
                var gs = window.GameState;
                const int cost = 405000;
                if (MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes) < cost) return;
                MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);

                Task.Run(() =>
                {
                    int addKeys = 0, addScales = 0, addProgressions = 0, addMajors = 0;
                    lock (_saveRandomLock) { if (_saveRandom.NextDouble() <= 0.50) addKeys = 1; if (_saveRandom.NextDouble() <= 0.25) addScales = 1; if (_saveRandom.NextDouble() <= 0.15) addProgressions = 1; if (_saveRandom.NextDouble() <= 0.05) addMajors = 1; }
                    if (gs.OdeToJoyMajorAbility) addMajors++;
                    lock (gs) { gs._enigmaMinorKeys += addKeys; gs._enigmaMinorScales += addScales; gs._enigmaMinorProgressions += addProgressions; gs._enigmaMajorSheets += addMajors; }
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => UIUpdater.UpdateSaveScoresUIImmediate(window, gs), Avalonia.Threading.DispatcherPriority.Background);
                });
            });

            // Fate
            SubscribeWithAutoUpdate(window.SaveScoresScreen.FateButton, window, () =>
            {
                var gs = window.GameState;
                const int cost = 1215000;
                if (MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes) < cost) return;
                MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);

                Task.Run(() =>
                {
                    int addKeys = 0, addScales = 0, addProgressions = 0, addMajors = 0;
                    lock (_saveRandomLock) { if (_saveRandom.NextDouble() <= 0.50) addKeys = 1; if (_saveRandom.NextDouble() <= 0.25) addScales = 1; if (_saveRandom.NextDouble() <= 0.15) addProgressions = 1; if (_saveRandom.NextDouble() <= 0.05) addMajors = 1; }
                    if (gs.OdeToJoyMajorAbility) addMajors++;
                    lock (gs) { gs._fateMinorKeys += addKeys; gs._fateMinorScales += addScales; gs._fateMinorProgressions += addProgressions; gs._fateMajorSheets += addMajors; }
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => UIUpdater.UpdateSaveScoresUIImmediate(window, gs), Avalonia.Threading.DispatcherPriority.Background);
                });
            });

            // Ode to Joy
            SubscribeWithAutoUpdate(window.SaveScoresScreen.OdeToJoyButton, window, () =>
            {
                var gs = window.GameState;
                const int cost = 3645000;
                if (MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes) < cost) return;
                MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);

                Task.Run(() =>
                {
                    int addKeys = 0, addScales = 0, addProgressions = 0, addMajors = 0;
                    lock (_saveRandomLock) { if (_saveRandom.NextDouble() <= 0.50) addKeys = 1; if (_saveRandom.NextDouble() <= 0.25) addScales = 1; if (_saveRandom.NextDouble() <= 0.15) addProgressions = 1; if (_saveRandom.NextDouble() <= 0.05) addMajors = 1; }
                    if (gs.OdeToJoyMajorAbility) addMajors++;
                    lock (gs) { gs._odeToJoyMinorKeys += addKeys; gs._odeToJoyMinorScales += addScales; gs._odeToJoyMinorProgressions += addProgressions; gs._odeToJoyMajorSheets += addMajors; }
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => UIUpdater.UpdateSaveScoresUIImmediate(window, gs), Avalonia.Threading.DispatcherPriority.Background);
                });
            });

            // Moonlight Sonata
            SubscribeWithAutoUpdate(window.SaveScoresScreen.MoonlightSonataButton, window, () =>
            {
                var gs = window.GameState;
                const int cost = 5000;
                if (MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes) < cost) return;
                MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);

                Task.Run(() =>
                {
                    int addKeys = 0, addScales = 0, addProgressions = 0, addMajors = 0;
                    lock (_saveRandomLock) { if (_saveRandom.NextDouble() <= 0.50) addKeys = 1; if (_saveRandom.NextDouble() <= 0.25) addScales = 1; if (_saveRandom.NextDouble() <= 0.15) addProgressions = 1; if (_saveRandom.NextDouble() <= 0.05) addMajors = 1; }
                    if (gs.OdeToJoyMajorAbility) addMajors++;
                    lock (gs) { gs._moonlightMinorKeys += addKeys; gs._moonlightMinorScales += addScales; gs._moonlightMinorProgressions += addProgressions; gs._moonlightMajorSheets += addMajors; }
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => UIUpdater.UpdateSaveScoresUIImmediate(window, gs), Avalonia.Threading.DispatcherPriority.Background);
                });
            });

            // Eroica
            SubscribeWithAutoUpdate(window.SaveScoresScreen.EroicaButton, window, () =>
            {
                var gs = window.GameState;
                const int cost = 15000;
                if (MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes) < cost) return;
                MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);

                Task.Run(() =>
                {
                    int addKeys = 0, addScales = 0, addProgressions = 0, addMajors = 0;
                    lock (_saveRandomLock) { if (_saveRandom.NextDouble() <= 0.50) addKeys = 1; if (_saveRandom.NextDouble() <= 0.25) addScales = 1; if (_saveRandom.NextDouble() <= 0.15) addProgressions = 1; if (_saveRandom.NextDouble() <= 0.05) addMajors = 1; }
                    if (gs.OdeToJoyMajorAbility) addMajors++;
                    lock (gs) { gs._eroicaMinorKeys += addKeys; gs._eroicaMinorScales += addScales; gs._eroicaMinorProgressions += addProgressions; gs._eroicaMajorSheets += addMajors; }
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => UIUpdater.UpdateSaveScoresUIImmediate(window, gs), Avalonia.Threading.DispatcherPriority.Background);
                });
            });

            // Swan Lake
            SubscribeWithAutoUpdate(window.SaveScoresScreen.SwanLakeButton, window, () =>
            {
                var gs = window.GameState;
                const int cost = 45000;
                if (MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes) < cost) return;
                MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);

                Task.Run(() =>
                {
                    int addKeys = 0, addScales = 0, addProgressions = 0, addMajors = 0;
                    lock (_saveRandomLock) { if (_saveRandom.NextDouble() <= 0.50) addKeys = 1; if (_saveRandom.NextDouble() <= 0.25) addScales = 1; if (_saveRandom.NextDouble() <= 0.15) addProgressions = 1; if (_saveRandom.NextDouble() <= 0.05) addMajors = 1; }
                    if (gs.OdeToJoyMajorAbility) addMajors++;
                    lock (gs) { gs._swanLakeMinorKeys += addKeys; gs._swanLakeMinorScales += addScales; gs._swanLakeMinorProgressions += addProgressions; gs._swanLakeMajorSheets += addMajors; }
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => UIUpdater.UpdateSaveScoresUIImmediate(window, gs), Avalonia.Threading.DispatcherPriority.Background);
                });
            });
        }

        // Wire manual crafting buttons on the "Unite The Symphony" screen. These buttons
        // call UniteTheSymphonyManager.TryCraft* methods which adjust GameState and refresh UI.
        private static void InitializeManualCrafting(MainWindow window)
        {
            // Moonlight Sonata crafting
            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.MoonlightMinorButton, window, () =>
                UniteTheSymphonyManager.TryCraftMoonlightMinor(window, ref window.GameState._moonlightMinorOwned));

            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.MoonlightMajorButton, window, () =>
                UniteTheSymphonyManager.TryCraftMoonlightSonataMajor(window, ref window.GameState._moonlightMajorOwned));

            // Eroica crafting
            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.EroicaMinorButton, window, () =>
                UniteTheSymphonyManager.TryCraftEroicaMinor(window, ref window.GameState._eroicaMinorOwned));

            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.EroicaMajorButton, window, () =>
                UniteTheSymphonyManager.TryCraftEroicaMajor(window, ref window.GameState._eroicaMajorOwned));

            // Swan Lake crafting
            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.SwanMinorButton, window, () =>
                UniteTheSymphonyManager.TryCraftSwanMinor(window, ref window.GameState._swanMinorOwned));

            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.SwanMajorButton, window, () =>
                UniteTheSymphonyManager.TryCraftSwanMajor(window, ref window.GameState._swanMajorOwned));

            // La Campanella crafting
            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.LaCampanellaMinorButton, window, () =>
                UniteTheSymphonyManager.TryCraftLaCampanellaMinor(window, ref window.GameState._laCampanellaMinorOwned));

            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.LaCampanellaMajorButton, window, () =>
                UniteTheSymphonyManager.TryCraftLaCampanellaMajor(window, ref window.GameState._laCampanellaMajorOwned));

            // Enigma crafting
            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.EnigmaMinorButton, window, () =>
                UniteTheSymphonyManager.TryCraftEnigmaMinor(window, ref window.GameState._enigmaMinorOwned));

            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.EnigmaMajorButton, window, () =>
                UniteTheSymphonyManager.TryCraftEnigmaMajor(window, ref window.GameState._enigmaMajorOwned));

            // Fate crafting
            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.FateMinorButton, window, () =>
                UniteTheSymphonyManager.TryCraftFateMinor(window, ref window.GameState._fateMinorOwned));

            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.FateMajorButton, window, () =>
                UniteTheSymphonyManager.TryCraftFateMajor(window, ref window.GameState._fateMajorOwned));

            // Ode to Joy crafting
            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.OdeToJoyMinorButton, window, () =>
                UniteTheSymphonyManager.TryCraftOdeToJoyMinor(window, ref window.GameState._odeToJoyMinorOwned));

            SubscribeWithAutoUpdate(window.UnityTheSymphonyScreen.OdeToJoyMajorButton, window, () =>
                UniteTheSymphonyManager.TryCraftOdeToJoyMajor(window, ref window.GameState._odeToJoyMajorOwned));
        }
    }
}