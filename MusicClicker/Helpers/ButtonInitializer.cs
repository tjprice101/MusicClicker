using Avalonia.Controls;

namespace MusicClicker
{
    public static class ButtonInitializer
    {
        public static void InitializeAllButtons(MainWindow window)
        {
            InitializeHarmonyButtons(window);
            InitializeEssenceButtons(window);
            InitializeUpgradeButtons(window);
            InitializeClicker(window);
            InitializeNavigationButtons(window);
            InitializeFragmentationButtons(window);
            InitializeSaveScoresButtons(window);
            InitializeManualCrafting(window);
        }

        private static void InitializeHarmonyButtons(MainWindow window)
        {
            window.MoonlightHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.MoonlightMajorSheets,
                    ref window.GameState.MoonlightMajorKeys, ref window.GameState.MoonlightMajorScales,
                    ref window.GameState.MoonlightMajorProgressions,
                    window.MoonlightMajorSheetsOwnedText, window.MoonlightMajorKeysOwnedText,
                    window.MoonlightMajorScalesOwnedText, window.MoonlightMajorProgressionsOwnedText, "Moonlight Sonata");

            window.EroicaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.EroicaMajorSheets,
                    ref window.GameState.EroicaMajorKeys, ref window.GameState.EroicaMajorScales,
                    ref window.GameState.EroicaMajorProgressions,
                    window.EroicaMajorSheetsOwnedText, window.EroicaMajorKeysOwnedText,
                    window.EroicaMajorScalesOwnedText, window.EroicaMajorProgressionsOwnedText, "Eroica");

            window.SwanLakeHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.SwanLakeMajorSheets,
                    ref window.GameState.SwanLakeMajorKeys, ref window.GameState.SwanLakeMajorScales,
                    ref window.GameState.SwanLakeMajorProgressions,
                    window.SwanLakeMajorSheetsOwnedText, window.SwanLakeMajorKeysOwnedText,
                    window.SwanLakeMajorScalesOwnedText, window.SwanLakeMajorProgressionsOwnedText, "Swan Lake");

                    // New Scores
    window.LaCampanellaHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.LaCampanellaMajorSheets,
            ref window.GameState.LaCampanellaMajorKeys, ref window.GameState.LaCampanellaMajorScales,
            ref window.GameState.LaCampanellaMajorProgressions,
            window.LaCampanellaMajorSheetsOwnedText, window.LaCampanellaMajorKeysOwnedText,
            window.LaCampanellaMajorScalesOwnedText, window.LaCampanellaMajorProgressionsOwnedText, "La Campanella");

    window.EnigmaHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.EnigmaMajorSheets,
            ref window.GameState.EnigmaMajorKeys, ref window.GameState.EnigmaMajorScales,
            ref window.GameState.EnigmaMajorProgressions,
            window.EnigmaMajorSheetsOwnedText, window.EnigmaMajorKeysOwnedText,
            window.EnigmaMajorScalesOwnedText, window.EnigmaMajorProgressionsOwnedText, "Enigma");

    window.FateHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.FateMajorSheets,
            ref window.GameState.FateMajorKeys, ref window.GameState.FateMajorScales,
            ref window.GameState.FateMajorProgressions,
            window.FateMajorSheetsOwnedText, window.FateMajorKeysOwnedText,
            window.FateMajorScalesOwnedText, window.FateMajorProgressionsOwnedText, "Fate");

    window.OdeToJoyHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.OdeToJoyMajorSheets,
            ref window.GameState.OdeToJoyMajorKeys, ref window.GameState.OdeToJoyMajorScales,
            ref window.GameState.OdeToJoyMajorProgressions,
            window.OdeToJoyMajorSheetsOwnedText, window.OdeToJoyMajorKeysOwnedText,
            window.OdeToJoyMajorScalesOwnedText, window.OdeToJoyMajorProgressionsOwnedText, "Ode to Joy");
        }

        private static void InitializeEssenceButtons(MainWindow window)
        {
            window.EssenceBeethovenButton.Click += (s, e) => EssenceManager.BuyEssence(window, ref window.GameState.EssenceBeethoven, 500, window.EssenceBeethovenOwnedText);
            window.EssencePyotrButton.Click += (s, e) => EssenceManager.BuyEssence(window, ref window.GameState.EssencePyotr, 1500, window.EssencePyotrOwnedText);
            window.EssenceElgarButton.Click += (s, e) => EssenceManager.BuyEssence(window, ref window.GameState.EssenceElgar, 4000, window.EssenceElgarOwnedText);
            window.EssenceLisztButton.Click += (s, e) => EssenceManager.BuyEssence(window, ref window.GameState.EssenceLiszt, 6000, window.EssenceLisztOwnedText);
        }

        private static void InitializeUpgradeButtons(MainWindow window)
        {
            // Chord
            window.BuyChordButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ChordOwned, window.GameState.ChordBaseCost, 0.5, 0, 1, window.ChordOwnedTextUpgrade, window.ChordCostTextUpgrade);
            window.BuyChordMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ChordOwned, window.GameState.ChordBaseCost, 0.5, 0, double.MaxValue, window.ChordOwnedTextUpgrade, window.ChordCostTextUpgrade);

            // Scale
            window.BuyScaleButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ScaleOwned, window.GameState.ScaleBaseCost, 3, 0, 1, window.ScaleOwnedTextUpgrade, window.ScaleCostTextUpgrade);
            window.BuyScaleMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.ScaleOwned, window.GameState.ScaleBaseCost, 3, 0, double.MaxValue, window.ScaleOwnedTextUpgrade, window.ScaleCostTextUpgrade);

            // Orchestra
            window.BuyOrchestraButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OrchestraOwned, window.GameState.OrchestraBaseCost, 5, 0, 1, window.OrchestraOwnedTextUpgrade, window.OrchestraCostTextUpgrade);
            window.BuyOrchestraMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OrchestraOwned, window.GameState.OrchestraBaseCost, 5, 0, double.MaxValue, window.OrchestraOwnedTextUpgrade, window.OrchestraCostTextUpgrade);

            // Symphony
            window.BuySymphonyButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.SymphonyOwned, window.GameState.SymphonyBaseCost, 9, 0, 1, window.SymphonyOwnedTextUpgrade, window.SymphonyCostTextUpgrade);
            window.BuySymphonyMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.SymphonyOwned, window.GameState.SymphonyBaseCost, 9, 0, double.MaxValue, window.SymphonyOwnedTextUpgrade, window.SymphonyCostTextUpgrade);

            // Aria
            window.BuyAriaButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.AriaOwned, window.GameState.AriaBaseCost, 0, 1, 1, window.AriaOwnedTextUpgrade, window.AriaCostTextUpgrade);
            window.BuyAriaMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.AriaOwned, window.GameState.AriaBaseCost, 0, 1, double.MaxValue, window.AriaOwnedTextUpgrade, window.AriaCostTextUpgrade);

            // Requiem
            window.BuyRequiemButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.RequiemOwned, window.GameState.RequiemBaseCost, 0, 2, 1, window.RequiemOwnedTextUpgrade, window.RequiemCostTextUpgrade);
            window.BuyRequiemMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.RequiemOwned, window.GameState.RequiemBaseCost, 0, 2, double.MaxValue, window.RequiemOwnedTextUpgrade, window.RequiemCostTextUpgrade);

            // Opus
            window.BuyOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OpusOwned, window.GameState.OpusBaseCost, 0, 3, 1, window.OpusOwnedTextUpgrade, window.OpusCostTextUpgrade);
            window.BuyOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.OpusOwned, window.GameState.OpusBaseCost, 0, 3, double.MaxValue, window.OpusOwnedTextUpgrade, window.OpusCostTextUpgrade);

            // Magnum Opus
            window.BuyMagnumOpusButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.MagnumOpusOwned, window.GameState.MagnumOpusBaseCost, 0, 4, 1, window.MagnumOpusOwnedTextUpgrade, window.MagnumOpusCostTextUpgrade);
            window.BuyMagnumOpusMaxButton.Click += (s, e) =>
                UpgradeManager.BuyUpgrade(window, ref window.GameState.MagnumOpusOwned, window.GameState.MagnumOpusBaseCost, 0, 4, double.MaxValue, window.MagnumOpusOwnedTextUpgrade, window.MagnumOpusCostTextUpgrade);
        }

        private static void InitializeClicker(MainWindow window)
        {
            window.ClickButton.Click += window.ClickButton_Click;
            window.KeyDown += window.MainWindow_KeyDown;
        }

        private static void InitializeNavigationButtons(MainWindow window)
        {
            window.OpenUpgradesButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.UpgradeScreen.IsVisible = true;
                UIUpdater.UpdateUI(window, window.GameState);
            };
            window.BackButtonBottom.Click += window.BackButton_Click;

            window.FragmentationButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.FragmentationScreen.IsVisible = true;
                UIUpdater.UpdateFragmentationUI(window, window.GameState);
            };
            window.BackButtonFragmentation.Click += (s, e) =>
            {
                window.FragmentationScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };

            window.ResonanceButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.SaveScoresScreen.IsVisible = true;
                UIUpdater.UpdateSaveScoresUI(window, window.GameState);
            };
            window.BackButtonSaveScores.Click += (s, e) =>
            {
                window.SaveScoresScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };

            window.MelodyButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.HeartOfHarmonyScreen.IsVisible = true;
                UIUpdater.UpdateHeartOfHarmonyUI(window, window.GameState);
            };
            window.BackButtonHeartOfHarmony.Click += (s, e) =>
            {
                window.HeartOfHarmonyScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };

            window.HarmonyButton.Click += (s, e) =>
            {
                window.MainScreen.IsVisible = false;
                window.UnityTheSymphonyScreen.IsVisible = true;
                UIUpdater.UpdateUnitySymphonyUI(window, window.GameState);
            };
            window.BackButtonUnitySymphony.Click += (s, e) =>
            {
                window.UnityTheSymphonyScreen.IsVisible = false;
                window.MainScreen.IsVisible = true;
            };
        }

        private static void InitializeFragmentationButtons(MainWindow window)
        {
            window.MelodiousFragmentButton.Click += (s, e) =>
            {
                if (window.GameState.Notes >= 1000)
                {
                    window.GameState.Notes -= 1000;
                    window.GameState.MelodiousOwned++;
                    UIUpdater.UpdateFragmentationUI(window, window.GameState);
                }
            };
            window.HarmoniousFragmentButton.Click += (s, e) =>
            {
                if (window.GameState.Notes >= 5000)
                {
                    window.GameState.Notes -= 5000;
                    window.GameState.HarmoniousOwned++;
                    UIUpdater.UpdateFragmentationUI(window, window.GameState);
                }
            };
        }

        private static void InitializeSaveScoresButtons(MainWindow window)
        {
            window.LaCampanellaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.LaCampanellaMinorKeys, ref window.GameState.LaCampanellaMinorScales,
                    ref window.GameState.LaCampanellaMinorProgressions, ref window.GameState.LaCampanellaMajorSheets, 12000);

            window.EnigmaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.EnigmaMinorKeys, ref window.GameState.EnigmaMinorScales,
                    ref window.GameState.EnigmaMinorProgressions, ref window.GameState.EnigmaMajorSheets, 25000);

            window.FateButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.FateMinorKeys, ref window.GameState.FateMinorScales,
                    ref window.GameState.FateMinorProgressions, ref window.GameState.FateMajorSheets, 35000);

            window.OdeToJoyButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.OdeToJoyMinorKeys, ref window.GameState.OdeToJoyMinorScales,
                    ref window.GameState.OdeToJoyMinorProgressions, ref window.GameState.OdeToJoyMajorSheets, 55000);

            window.MoonlightSonataButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.MoonlightMinorKeys, ref window.GameState.MoonlightMinorScales,
                    ref window.GameState.MoonlightMinorProgressions, ref window.GameState.MoonlightMajorSheets, 1000);

            window.EroicaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.EroicaMinorKeys, ref window.GameState.EroicaMinorScales,
                    ref window.GameState.EroicaMinorProgressions, ref window.GameState.EroicaMajorSheets, 3000);

            window.SwanLakeButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.SwanLakeMinorKeys, ref window.GameState.SwanLakeMinorScales,
                    ref window.GameState.SwanLakeMinorProgressions, ref window.GameState.SwanLakeMajorSheets, 8000);
        }

        private static void InitializeManualCrafting(MainWindow window)
{
    // Moonlight Sonata - Minor and Major
    window.MoonlightMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftMoonlightMinor(window, ref window.GameState.MoonlightMinorOwned);

    window.MoonlightMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftMoonlightSonataMajor(window, ref window.GameState.MoonlightMajorOwned);

    // Eroica - Minor and Major
    window.EroicaMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEroicaMinor(window, ref window.GameState.EroicaMinorOwned);

    window.EroicaMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEroicaMajor(window, ref window.GameState.EroicaMajorOwned);

    // Swan Lake - Minor and Major
    window.SwanMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftSwanLakeMinor(window, ref window.GameState.SwanMinorOwned);

    window.SwanMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftSwanLakeMajor(window, ref window.GameState.SwanMajorOwned);

    // La Campanella - Minor and Major
    window.LaCampanellaMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftLaCampanellaMinor(window, ref window.GameState.LaCampanellaMinorOwned);

    window.LaCampanellaMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftLaCampanellaMajor(window, ref window.GameState.LaCampanellaMajorOwned);

    // Enigma - Minor and Major
    window.EnigmaMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEnigmaMinor(window, ref window.GameState.EnigmaMinorOwned);

    window.EnigmaMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEnigmaMajor(window, ref window.GameState.EnigmaMajorOwned);

    // Fate - Minor and Major
    window.FateMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftFateMinor(window, ref window.GameState.FateMinorOwned);

    window.FateMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftFateMajor(window, ref window.GameState.FateMajorOwned);

    // Ode to Joy - Minor and Major
    window.OdeToJoyMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftOdeToJoyMinor(window, ref window.GameState.OdeToJoyMinorOwned);

    window.OdeToJoyMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftOdeToJoyMajor(window, ref window.GameState.OdeToJoyMajorOwned);
}


    }
}
