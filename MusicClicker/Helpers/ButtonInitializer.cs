using Avalonia.Controls;

namespace MusicClicker
{
    public static class ButtonInitializer
    {
        // Calls all individual button-initializer groups
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

        // Initializes all Heart of Harmony conversion buttons
        private static void InitializeHarmonyButtons(MainWindow window)
        {
            // Moonlight Sonata conversions
            window.HeartOfHarmonyScreen.MoonlightHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.MoonlightMajorSheets,
                    ref window.GameState.MoonlightMajorKeys, ref window.GameState.MoonlightMajorScales,
                    ref window.GameState.MoonlightMajorProgressions,
                    window.HeartOfHarmonyScreen.MoonlightMajorSheetsOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.MoonlightMajorScalesOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorProgressionsOwnedText, "Moonlight Sonata");

            // Eroica conversions
            window.HeartOfHarmonyScreen.EroicaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.EroicaMajorSheets,
                    ref window.GameState.EroicaMajorKeys, ref window.GameState.EroicaMajorScales,
                    ref window.GameState.EroicaMajorProgressions,
                    window.HeartOfHarmonyScreen.EroicaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EroicaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.EroicaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EroicaMajorProgressionsOwnedText, "Eroica");

            // Swan Lake conversions
            window.HeartOfHarmonyScreen.SwanLakeHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.SwanLakeMajorSheets,
                    ref window.GameState.SwanLakeMajorKeys, ref window.GameState.SwanLakeMajorScales,
                    ref window.GameState.SwanLakeMajorProgressions,
                    window.HeartOfHarmonyScreen.SwanLakeMajorSheetsOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.SwanLakeMajorScalesOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorProgressionsOwnedText, "Swan Lake");

            // La Campanella conversions
            window.HeartOfHarmonyScreen.LaCampanellaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.LaCampanellaMajorSheets,
                    ref window.GameState.LaCampanellaMajorKeys, ref window.GameState.LaCampanellaMajorScales,
                    ref window.GameState.LaCampanellaMajorProgressions,
                    window.HeartOfHarmonyScreen.LaCampanellaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.LaCampanellaMajorScalesOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorProgressionsOwnedText, "La Campanella");

            // Enigma conversions
            window.HeartOfHarmonyScreen.EnigmaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.EnigmaMajorSheets,
                    ref window.GameState.EnigmaMajorKeys, ref window.GameState.EnigmaMajorScales,
                    ref window.GameState.EnigmaMajorProgressions,
                    window.HeartOfHarmonyScreen.EnigmaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.EnigmaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorProgressionsOwnedText, "Enigma");

            // Fate conversions
            window.HeartOfHarmonyScreen.FateHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.FateMajorSheets,
                    ref window.GameState.FateMajorKeys, ref window.GameState.FateMajorScales,
                    ref window.GameState.FateMajorProgressions,
                    window.HeartOfHarmonyScreen.FateMajorSheetsOwnedText, window.HeartOfHarmonyScreen.FateMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.FateMajorScalesOwnedText, window.HeartOfHarmonyScreen.FateMajorProgressionsOwnedText, "Fate");

            // Ode to Joy conversions
            window.HeartOfHarmonyScreen.OdeToJoyHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.OdeToJoyMajorSheets,
                    ref window.GameState.OdeToJoyMajorKeys, ref window.GameState.OdeToJoyMajorScales,
                    ref window.GameState.OdeToJoyMajorProgressions,
                    window.HeartOfHarmonyScreen.OdeToJoyMajorSheetsOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.OdeToJoyMajorScalesOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorProgressionsOwnedText, "Ode to Joy");
        }

        // Initializes all upgrade purchase buttons (single and max-buy)
        private static void InitializeUpgradeButtons(MainWindow window)
        {
            // Chord
            window.UpgradeScreen.BuyChordButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ChordOwned, window.GameState.ChordBaseCost,
                    0.5, 0, 1, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);

            window.UpgradeScreen.BuyChordMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ChordOwned, window.GameState.ChordBaseCost,
                    0.5, 0, double.MaxValue, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);

            // Scale
            window.UpgradeScreen.BuyScaleButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ScaleOwned, window.GameState.ScaleBaseCost,
                    3, 0, 1, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);

            window.UpgradeScreen.BuyScaleMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ScaleOwned, window.GameState.ScaleBaseCost,
                    3, 0, double.MaxValue, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);

            // Orchestra
            window.UpgradeScreen.BuyOrchestraButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OrchestraOwned, window.GameState.OrchestraBaseCost,
                    5, 0, 1, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);

            window.UpgradeScreen.BuyOrchestraMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OrchestraOwned, window.GameState.OrchestraBaseCost,
                    5, 0, double.MaxValue, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);

            // Symphony
            window.UpgradeScreen.BuySymphonyButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.SymphonyOwned, window.GameState.SymphonyBaseCost,
                    9, 0, 1, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);

            window.UpgradeScreen.BuySymphonyMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.SymphonyOwned, window.GameState.SymphonyBaseCost,
                    9, 0, double.MaxValue, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);

            // Aria
            window.UpgradeScreen.BuyAriaButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.AriaOwned, window.GameState.AriaBaseCost,
                    0, 1, 1, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);

            window.UpgradeScreen.BuyAriaMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.AriaOwned, window.GameState.AriaBaseCost,
                    0, 1, double.MaxValue, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);

            // Requiem
            window.UpgradeScreen.BuyRequiemButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.RequiemOwned, window.GameState.RequiemBaseCost,
                    0, 2, 1, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);

            window.UpgradeScreen.BuyRequiemMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.RequiemOwned, window.GameState.RequiemBaseCost,
                    0, 2, double.MaxValue, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);

            // Opus
            window.UpgradeScreen.BuyOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OpusOwned, window.GameState.OpusBaseCost,
                    0, 3, 1, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);

            window.UpgradeScreen.BuyOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OpusOwned, window.GameState.OpusBaseCost,
                    0, 3, double.MaxValue, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);

            // Magnum Opus
            window.UpgradeScreen.BuyMagnumOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.MagnumOpusOwned, window.GameState.MagnumOpusBaseCost,
                    0, 4, 1, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);

            window.UpgradeScreen.BuyMagnumOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.MagnumOpusOwned, window.GameState.MagnumOpusBaseCost,
                    0, 4, double.MaxValue, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);
        }

        // Initializes click button + keyboard input
        private static void InitializeClicker(MainWindow window)
        {
            window.ClickButton.Click += window.ClickButton_Click;
            window.KeyDown += window.MainWindow_KeyDown;
        }

        // Controls navigation between menu screens
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
        }

        // Handles buttons that convert Notes into fragments
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

        // Handles all Save Scores buttons
        private static void InitializeSaveScoresButtons(MainWindow window)
        {
            // La Campanella
            window.SaveScoresScreen.LaCampanellaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.LaCampanellaMinorKeys,
                    ref window.GameState.LaCampanellaMinorScales, ref window.GameState.LaCampanellaMinorProgressions,
                    ref window.GameState.LaCampanellaMajorSheets, 135000);

            // Enigma
            window.SaveScoresScreen.EnigmaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.EnigmaMinorKeys,
                    ref window.GameState.EnigmaMinorScales, ref window.GameState.EnigmaMinorProgressions,
                    ref window.GameState.EnigmaMajorSheets, 405000);

            // Fate
            window.SaveScoresScreen.FateButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.FateMinorKeys,
                    ref window.GameState.FateMinorScales, ref window.GameState.FateMinorProgressions,
                    ref window.GameState.FateMajorSheets, 1215000);

            // Ode to Joy
            window.SaveScoresScreen.OdeToJoyButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.OdeToJoyMinorKeys,
                    ref window.GameState.OdeToJoyMinorScales, ref window.GameState.OdeToJoyMinorProgressions,
                    ref window.GameState.OdeToJoyMajorSheets, 3645000);

            // Moonlight Sonata
            window.SaveScoresScreen.MoonlightSonataButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.MoonlightMinorKeys,
                    ref window.GameState.MoonlightMinorScales, ref window.GameState.MoonlightMinorProgressions,
                    ref window.GameState.MoonlightMajorSheets, 5000);

            // Eroica
            window.SaveScoresScreen.EroicaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.EroicaMinorKeys,
                    ref window.GameState.EroicaMinorScales, ref window.GameState.EroicaMinorProgressions,
                    ref window.GameState.EroicaMajorSheets, 15000);

            // Swan Lake
            window.SaveScoresScreen.SwanLakeButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.SwanLakeMinorKeys,
                    ref window.GameState.SwanLakeMinorScales, ref window.GameState.SwanLakeMinorProgressions,
                    ref window.GameState.SwanLakeMajorSheets, 45000);
        }

        // Initializes all manual crafting buttons on Unity the Symphony screen
        private static void InitializeManualCrafting(MainWindow window)
        {
            // Moonlight Sonata crafting
            window.UnityTheSymphonyScreen.MoonlightMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftMoonlightMinor(window, ref window.GameState.MoonlightMinorOwned);

            window.UnityTheSymphonyScreen.MoonlightMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftMoonlightSonataMajor(window, ref window.GameState.MoonlightMajorOwned);

            // Eroica crafting
            window.UnityTheSymphonyScreen.EroicaMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEroicaMinor(window, ref window.GameState.EroicaMinorOwned);

            window.UnityTheSymphonyScreen.EroicaMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEroicaMajor(window, ref window.GameState.EroicaMajorOwned);

            // Swan Lake crafting
            window.UnityTheSymphonyScreen.SwanMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftSwanLakeMinor(window, ref window.GameState.SwanMinorOwned);

            window.UnityTheSymphonyScreen.SwanMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftSwanLakeMajor(window, ref window.GameState.SwanMajorOwned);

            // La Campanella crafting
            window.UnityTheSymphonyScreen.LaCampanellaMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftLaCampanellaMinor(window, ref window.GameState.LaCampanellaMinorOwned);

            window.UnityTheSymphonyScreen.LaCampanellaMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftLaCampanellaMajor(window, ref window.GameState.LaCampanellaMajorOwned);

            // Enigma crafting
            window.UnityTheSymphonyScreen.EnigmaMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEnigmaMinor(window, ref window.GameState.EnigmaMinorOwned);

            window.UnityTheSymphonyScreen.EnigmaMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftEnigmaMajor(window, ref window.GameState.EnigmaMajorOwned);

            // Fate crafting
            window.UnityTheSymphonyScreen.FateMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftFateMinor(window, ref window.GameState.FateMinorOwned);

            window.UnityTheSymphonyScreen.FateMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftFateMajor(window, ref window.GameState.FateMajorOwned);

            // Ode to Joy crafting
            window.UnityTheSymphonyScreen.OdeToJoyMinorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftOdeToJoyMinor(window, ref window.GameState.OdeToJoyMinorOwned);

            window.UnityTheSymphonyScreen.OdeToJoyMajorButton.Click += (s, e) =>
                UniteTheSymphonyManager.TryCraftOdeToJoyMajor(window, ref window.GameState.OdeToJoyMajorOwned);
        }
    }
}