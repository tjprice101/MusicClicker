using Avalonia.Controls;

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

        // Wire handlers for Heart of Harmony conversion UI elements.
        // Each button delegates to HeartOfHarmonyManager.HandleMajorSheetClick and
        // passes GameState fields by reference so the manager can mutate them directly.
        private static void InitializeHarmonyButtons(MainWindow window)
        {
            // Moonlight Sonata conversions
            window.HeartOfHarmonyScreen.MoonlightHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._moonlightMajorSheets,
                    ref window.GameState._moonlightMajorKeys, ref window.GameState._moonlightMajorScales,
                    ref window.GameState._moonlightMajorProgressions,
                    window.HeartOfHarmonyScreen.MoonlightMajorSheetsOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.MoonlightMajorScalesOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorProgressionsOwnedText, "Moonlight Sonata");

            // Eroica conversions
            window.HeartOfHarmonyScreen.EroicaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._eroicaMajorSheets,
                    ref window.GameState._eroicaMajorKeys, ref window.GameState._eroicaMajorScales,
                    ref window.GameState._eroicaMajorProgressions,
                    window.HeartOfHarmonyScreen.EroicaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EroicaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.EroicaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EroicaMajorProgressionsOwnedText, "Eroica");

            // Swan Lake conversions
            window.HeartOfHarmonyScreen.SwanLakeHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._swanLakeMajorSheets,
                    ref window.GameState._swanLakeMajorKeys, ref window.GameState._swanLakeMajorScales,
                    ref window.GameState._swanLakeMajorProgressions,
                    window.HeartOfHarmonyScreen.SwanLakeMajorSheetsOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.SwanLakeMajorScalesOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorProgressionsOwnedText, "Swan Lake");

            // La Campanella conversions
            window.HeartOfHarmonyScreen.LaCampanellaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._laCampanellaMajorSheets,
                    ref window.GameState._laCampanellaMajorKeys, ref window.GameState._laCampanellaMajorScales,
                    ref window.GameState._laCampanellaMajorProgressions,
                    window.HeartOfHarmonyScreen.LaCampanellaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.LaCampanellaMajorScalesOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorProgressionsOwnedText, "La Campanella");

            // Enigma conversions
            window.HeartOfHarmonyScreen.EnigmaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._enigmaMajorSheets,
                    ref window.GameState._enigmaMajorKeys, ref window.GameState._enigmaMajorScales,
                    ref window.GameState._enigmaMajorProgressions,
                    window.HeartOfHarmonyScreen.EnigmaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.EnigmaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorProgressionsOwnedText, "Enigma");

            // Fate conversions
            window.HeartOfHarmonyScreen.FateHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._fateMajorSheets,
                    ref window.GameState._fateMajorKeys, ref window.GameState._fateMajorScales,
                    ref window.GameState._fateMajorProgressions,
                    window.HeartOfHarmonyScreen.FateMajorSheetsOwnedText, window.HeartOfHarmonyScreen.FateMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.FateMajorScalesOwnedText, window.HeartOfHarmonyScreen.FateMajorProgressionsOwnedText, "Fate");

            // Ode to Joy conversions
            window.HeartOfHarmonyScreen.OdeToJoyHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState._odeToJoyMajorSheets,
                    ref window.GameState._odeToJoyMajorKeys, ref window.GameState._odeToJoyMajorScales,
                    ref window.GameState._odeToJoyMajorProgressions,
                    window.HeartOfHarmonyScreen.OdeToJoyMajorSheetsOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.OdeToJoyMajorScalesOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorProgressionsOwnedText, "Ode to Joy");
        }

        // Wire handlers for upgrade purchase buttons. Each button calls UpgradeManager.BuyUpgrade.
        // Note: "Max" buttons pass double.MaxValue to request buying until funds are exhausted.
        private static void InitializeUpgradeButtons(MainWindow window)
        {
            // Chord
            window.UpgradeScreen.BuyChordButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._chordOwned, window.GameState.ChordBaseCost,
                    0.5, 0, 1, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);

            window.UpgradeScreen.BuyChordMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._chordOwned, window.GameState.ChordBaseCost,
                    0.5, 0, double.MaxValue, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);

            // Scale
            window.UpgradeScreen.BuyScaleButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._scaleOwned, window.GameState.ScaleBaseCost,
                    3, 0, 1, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);

            window.UpgradeScreen.BuyScaleMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._scaleOwned, window.GameState.ScaleBaseCost,
                    3, 0, double.MaxValue, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);

            // Orchestra
            window.UpgradeScreen.BuyOrchestraButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._orchestraOwned, window.GameState.OrchestraBaseCost,
                    5, 0, 1, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);

            window.UpgradeScreen.BuyOrchestraMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._orchestraOwned, window.GameState.OrchestraBaseCost,
                    5, 0, double.MaxValue, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);

            // Symphony
            window.UpgradeScreen.BuySymphonyButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._symphonyOwned, window.GameState.SymphonyBaseCost,
                    9, 0, 1, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);

            window.UpgradeScreen.BuySymphonyMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._symphonyOwned, window.GameState.SymphonyBaseCost,
                    9, 0, double.MaxValue, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);

            // Aria
            window.UpgradeScreen.BuyAriaButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._ariaOwned, window.GameState.AriaBaseCost,
                    0, 1, 1, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);

            window.UpgradeScreen.BuyAriaMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._ariaOwned, window.GameState.AriaBaseCost,
                    0, 1, double.MaxValue, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);

            // Requiem
            window.UpgradeScreen.BuyRequiemButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._requiemOwned, window.GameState.RequiemBaseCost,
                    0, 2, 1, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);

            window.UpgradeScreen.BuyRequiemMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._requiemOwned, window.GameState.RequiemBaseCost,
                    0, 2, double.MaxValue, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);

            // Opus
            window.UpgradeScreen.BuyOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._opusOwned, window.GameState.OpusBaseCost,
                    0, 3, 1, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);

            window.UpgradeScreen.BuyOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._opusOwned, window.GameState.OpusBaseCost,
                    0, 3, double.MaxValue, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);

            // Magnum Opus
            window.UpgradeScreen.BuyMagnumOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._magnumOpusOwned, window.GameState.MagnumOpusBaseCost,
                    0, 4, 1, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);

            window.UpgradeScreen.BuyMagnumOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState._magnumOpusOwned, window.GameState.MagnumOpusBaseCost,
                    0, 4, double.MaxValue, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);
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
            // Open upgrades menu
            window.OpenUpgradesButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.UpgradeScreen.IsVisible = true;
                UIUpdater.UpdateUI(window, window.GameState);
            };
            window.UpgradeScreen.BackButtonBottom.Click += window.BackButton_Click;

            // Top-right Upgrades button
            window.TopOpenUpgradesButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.UpgradeScreen.IsVisible = true;
                UIUpdater.UpdateUI(window, window.GameState);
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

            // Fragmentation
            window.FragmentationButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.FragmentationScreen.IsVisible = true;
                UIUpdater.UpdateFragmentationUI(window, window.GameState);
            };
            window.FragmentationScreen.BackButtonFragmentation.Click += (s, e) =>
            {
                window.FragmentationScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };

            // Save Scores
            window.ResonanceButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.SaveScoresScreen.IsVisible = true;
                UIUpdater.UpdateSaveScoresUI(window, window.GameState);
            };
            window.SaveScoresScreen.BackButtonSaveScores.Click += (s, e) =>
            {
                window.SaveScoresScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };

            // Heart of Harmony
            window.MelodyButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.HeartOfHarmonyScreen.IsVisible = true;
                UIUpdater.UpdateHeartOfHarmonyUI(window, window.GameState);
            };
            window.HeartOfHarmonyScreen.BackButtonHeartOfHarmony.Click += (s, e) =>
            {
                window.HeartOfHarmonyScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };

            // Unity the Symphony crafting menu
            window.HarmonyButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.UnityTheSymphonyScreen.IsVisible = true;
                UIUpdater.UpdateUnitySymphonyUI(window, window.GameState);
            };
            window.UnityTheSymphonyScreen.BackButtonUnitySymphony.Click += (s, e) =>
            {
                window.UnityTheSymphonyScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };

            // Armor of Forte
            window.ArmorOfForteButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.ArmorOfForteScreen.IsVisible = true;
                window.ArmorOfForteScreen.UpdateUI(window.GameState);
            };

            // Eternal Modulation
            window.EternalModulationButton.Click += (s, e) =>   
            {
                window.MainScreen.IsVisible = false;
                window.EternalModulationScreen.IsVisible = true;
            };

            // Symphonic Gallery - Main hub
            window.SymphonicGalleryButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.SymphonicGalleryScreen.IsVisible = true;
            };

            
        }

        // Wire the fragmentation buttons which convert Notes into Melodious/Harmonious fragments.
        // Handlers check affordability and update GameState then refresh related UI sections.
        private static void InitializeFragmentationButtons(MainWindow window)
        {
            // Buy Melodious Fragments
            window.FragmentationScreen.MelodiousFragmentButton.Click += (s, e) =>
            {
                if (window.GameState.Notes >= 1000)
                {
                    window.GameState.Notes -= 1000;
                    window.GameState.MelodiousOwned++;
                    UIUpdater.UpdateFragmentationUI(window, window.GameState);
                }
            };

            // Buy Harmonious Fragments
            window.FragmentationScreen.HarmoniousFragmentButton.Click += (s, e) =>
            {
                if (window.GameState.Notes >= 5000)
                {
                    window.GameState.Notes -= 5000;
                    window.GameState.HarmoniousOwned++;
                    UIUpdater.UpdateFragmentationUI(window, window.GameState);
                }
            };
        }

        // Wire Save Scores buttons which spend fixed costs to attempt randomized fragment drops.
        // Each handler delegates to SaveScoresManager.HandleSaveScoreClick for the core logic.
        private static void InitializeSaveScoresButtons(MainWindow window)
        {
            // La Campanella
            window.SaveScoresScreen.LaCampanellaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState._laCampanellaMinorKeys,
                    ref window.GameState._laCampanellaMinorScales, ref window.GameState._laCampanellaMinorProgressions,
                    ref window.GameState._laCampanellaMajorSheets, 135000);

            // Enigma
            window.SaveScoresScreen.EnigmaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState._enigmaMinorKeys,
                    ref window.GameState._enigmaMinorScales, ref window.GameState._enigmaMinorProgressions,
                    ref window.GameState._enigmaMajorSheets, 405000);

            // Fate
            window.SaveScoresScreen.FateButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState._fateMinorKeys,
                    ref window.GameState._fateMinorScales, ref window.GameState._fateMinorProgressions,
                    ref window.GameState._fateMajorSheets, 1215000);

            // Ode to Joy
            window.SaveScoresScreen.OdeToJoyButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState._odeToJoyMinorKeys,
                    ref window.GameState._odeToJoyMinorScales, ref window.GameState._odeToJoyMinorProgressions,
                    ref window.GameState._odeToJoyMajorSheets, 3645000);

            // Moonlight Sonata
            window.SaveScoresScreen.MoonlightSonataButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState._moonlightMinorKeys,
                    ref window.GameState._moonlightMinorScales, ref window.GameState._moonlightMinorProgressions,
                    ref window.GameState._moonlightMajorSheets, 5000);

            // Eroica
            window.SaveScoresScreen.EroicaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState._eroicaMinorKeys,
                    ref window.GameState._eroicaMinorScales, ref window.GameState._eroicaMinorProgressions,
                    ref window.GameState._eroicaMajorSheets, 15000);

            // Swan Lake
            window.SaveScoresScreen.SwanLakeButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState._swanLakeMinorKeys,
                    ref window.GameState._swanLakeMinorScales, ref window.GameState._swanLakeMinorProgressions,
                    ref window.GameState._swanLakeMajorSheets, 45000);
        }

        // Wire manual crafting buttons on the "Unite The Symphony" screen. These buttons
        // call UniteTheSymphonyManager.TryCraft* methods which adjust GameState and refresh UI.
        private static void InitializeManualCrafting(MainWindow window)
        {
            // Moonlight Sonata crafting
            window.UnityTheSymphonyScreen.MoonlightMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftMoonlightMinor(window, ref window.GameState._moonlightMinorOwned);

            window.UnityTheSymphonyScreen.MoonlightMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftMoonlightSonataMajor(window, ref window.GameState._moonlightMajorOwned);

            // Eroica crafting
            window.UnityTheSymphonyScreen.EroicaMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEroicaMinor(window, ref window.GameState._eroicaMinorOwned);

            window.UnityTheSymphonyScreen.EroicaMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEroicaMajor(window, ref window.GameState._eroicaMajorOwned);

            // Swan Lake crafting
            window.UnityTheSymphonyScreen.SwanMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftSwanLakeMinor(window, ref window.GameState._swanMinorOwned);

            window.UnityTheSymphonyScreen.SwanMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftSwanLakeMajor(window, ref window.GameState._swanMajorOwned);

            // La Campanella crafting
            window.UnityTheSymphonyScreen.LaCampanellaMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftLaCampanellaMinor(window, ref window.GameState._laCampanellaMinorOwned);

            window.UnityTheSymphonyScreen.LaCampanellaMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftLaCampanellaMajor(window, ref window.GameState._laCampanellaMajorOwned);

            // Enigma crafting
            window.UnityTheSymphonyScreen.EnigmaMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEnigmaMinor(window, ref window.GameState._enigmaMinorOwned);

            window.UnityTheSymphonyScreen.EnigmaMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEnigmaMajor(window, ref window.GameState._enigmaMajorOwned);

            // Fate crafting
            window.UnityTheSymphonyScreen.FateMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftFateMinor(window, ref window.GameState._fateMinorOwned);

            window.UnityTheSymphonyScreen.FateMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftFateMajor(window, ref window.GameState._fateMajorOwned);

            // Ode to Joy crafting
            window.UnityTheSymphonyScreen.OdeToJoyMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftOdeToJoyMinor(window, ref window.GameState._odeToJoyMinorOwned);

            window.UnityTheSymphonyScreen.OdeToJoyMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftOdeToJoyMajor(window, ref window.GameState._odeToJoyMajorOwned);
        }
    }
}