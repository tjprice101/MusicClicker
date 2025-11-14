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
            window.HeartOfHarmonyScreen.MoonlightHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.MoonlightMajorSheets,
                    ref window.GameState.MoonlightMajorKeys, ref window.GameState.MoonlightMajorScales,
                    ref window.GameState.MoonlightMajorProgressions,
                    window.HeartOfHarmonyScreen.MoonlightMajorSheetsOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.MoonlightMajorScalesOwnedText, window.HeartOfHarmonyScreen.MoonlightMajorProgressionsOwnedText, "Moonlight Sonata");

            window.HeartOfHarmonyScreen.EroicaHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.EroicaMajorSheets,
                    ref window.GameState.EroicaMajorKeys, ref window.GameState.EroicaMajorScales,
                    ref window.GameState.EroicaMajorProgressions,
                    window.HeartOfHarmonyScreen.EroicaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EroicaMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.EroicaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EroicaMajorProgressionsOwnedText, "Eroica");

            window.HeartOfHarmonyScreen.SwanLakeHarmonyButton.Click += (s, e) =>
                HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.SwanLakeMajorSheets,
                    ref window.GameState.SwanLakeMajorKeys, ref window.GameState.SwanLakeMajorScales,
                    ref window.GameState.SwanLakeMajorProgressions,
                    window.HeartOfHarmonyScreen.SwanLakeMajorSheetsOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorKeysOwnedText,
                    window.HeartOfHarmonyScreen.SwanLakeMajorScalesOwnedText, window.HeartOfHarmonyScreen.SwanLakeMajorProgressionsOwnedText, "Swan Lake");

                    // New Scores
    window.HeartOfHarmonyScreen.LaCampanellaHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.LaCampanellaMajorSheets,
            ref window.GameState.LaCampanellaMajorKeys, ref window.GameState.LaCampanellaMajorScales,
            ref window.GameState.LaCampanellaMajorProgressions,
            window.HeartOfHarmonyScreen.LaCampanellaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorKeysOwnedText,
            window.HeartOfHarmonyScreen.LaCampanellaMajorScalesOwnedText, window.HeartOfHarmonyScreen.LaCampanellaMajorProgressionsOwnedText, "La Campanella");

    window.HeartOfHarmonyScreen.EnigmaHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.EnigmaMajorSheets,
            ref window.GameState.EnigmaMajorKeys, ref window.GameState.EnigmaMajorScales,
            ref window.GameState.EnigmaMajorProgressions,
            window.HeartOfHarmonyScreen.EnigmaMajorSheetsOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorKeysOwnedText,
            window.HeartOfHarmonyScreen.EnigmaMajorScalesOwnedText, window.HeartOfHarmonyScreen.EnigmaMajorProgressionsOwnedText, "Enigma");

    window.HeartOfHarmonyScreen.FateHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.FateMajorSheets,
            ref window.GameState.FateMajorKeys, ref window.GameState.FateMajorScales,
            ref window.GameState.FateMajorProgressions,
            window.HeartOfHarmonyScreen.FateMajorSheetsOwnedText, window.HeartOfHarmonyScreen.FateMajorKeysOwnedText,
            window.HeartOfHarmonyScreen.FateMajorScalesOwnedText, window.HeartOfHarmonyScreen.FateMajorProgressionsOwnedText, "Fate");

    window.HeartOfHarmonyScreen.OdeToJoyHarmonyButton.Click += (s, e) =>
        HeartOfHarmonyManager.HandleMajorSheetClick(window, ref window.GameState.OdeToJoyMajorSheets,
            ref window.GameState.OdeToJoyMajorKeys, ref window.GameState.OdeToJoyMajorScales,
            ref window.GameState.OdeToJoyMajorProgressions,
            window.HeartOfHarmonyScreen.OdeToJoyMajorSheetsOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorKeysOwnedText,
            window.HeartOfHarmonyScreen.OdeToJoyMajorScalesOwnedText, window.HeartOfHarmonyScreen.OdeToJoyMajorProgressionsOwnedText, "Ode to Joy");
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
    window.UpgradeScreen.BuyChordButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.ChordOwned, window.GameState.ChordBaseCost, 0.5, 0, 1, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);
    window.UpgradeScreen.BuyChordMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.ChordOwned, window.GameState.ChordBaseCost, 0.5, 0, double.MaxValue, window.UpgradeScreen.ChordOwnedTextUpgrade, window.UpgradeScreen.ChordCostTextUpgrade);

    // Scale
    window.UpgradeScreen.BuyScaleButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.ScaleOwned, window.GameState.ScaleBaseCost, 3, 0, 1, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);
    window.UpgradeScreen.BuyScaleMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.ScaleOwned, window.GameState.ScaleBaseCost, 3, 0, double.MaxValue, window.UpgradeScreen.ScaleOwnedTextUpgrade, window.UpgradeScreen.ScaleCostTextUpgrade);

    // Orchestra
    window.UpgradeScreen.BuyOrchestraButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.OrchestraOwned, window.GameState.OrchestraBaseCost, 5, 0, 1, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);
    window.UpgradeScreen.BuyOrchestraMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.OrchestraOwned, window.GameState.OrchestraBaseCost, 5, 0, double.MaxValue, window.UpgradeScreen.OrchestraOwnedTextUpgrade, window.UpgradeScreen.OrchestraCostTextUpgrade);

    // Symphony
    window.UpgradeScreen.BuySymphonyButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.SymphonyOwned, window.GameState.SymphonyBaseCost, 9, 0, 1, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);
    window.UpgradeScreen.BuySymphonyMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.SymphonyOwned, window.GameState.SymphonyBaseCost, 9, 0, double.MaxValue, window.UpgradeScreen.SymphonyOwnedTextUpgrade, window.UpgradeScreen.SymphonyCostTextUpgrade);

    // Aria
    window.UpgradeScreen.BuyAriaButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.AriaOwned, window.GameState.AriaBaseCost, 0, 1, 1, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);
    window.UpgradeScreen.BuyAriaMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.AriaOwned, window.GameState.AriaBaseCost, 0, 1, double.MaxValue, window.UpgradeScreen.AriaOwnedTextUpgrade, window.UpgradeScreen.AriaCostTextUpgrade);

    // Requiem
    window.UpgradeScreen.BuyRequiemButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.RequiemOwned, window.GameState.RequiemBaseCost, 0, 2, 1, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);
    window.UpgradeScreen.BuyRequiemMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.RequiemOwned, window.GameState.RequiemBaseCost, 0, 2, double.MaxValue, window.UpgradeScreen.RequiemOwnedTextUpgrade, window.UpgradeScreen.RequiemCostTextUpgrade);

    // Opus
    window.UpgradeScreen.BuyOpusButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.OpusOwned, window.GameState.OpusBaseCost, 0, 3, 1, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);
    window.UpgradeScreen.BuyOpusMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.OpusOwned, window.GameState.OpusBaseCost, 0, 3, double.MaxValue, window.UpgradeScreen.OpusOwnedTextUpgrade, window.UpgradeScreen.OpusCostTextUpgrade);

    // Magnum Opus
    window.UpgradeScreen.BuyMagnumOpusButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.MagnumOpusOwned, window.GameState.MagnumOpusBaseCost, 0, 4, 1, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);
    window.UpgradeScreen.BuyMagnumOpusMaxButton.Click += (s, e) =>
        UpgradeManager.BuyUpgrade(window, ref window.GameState.MagnumOpusOwned, window.GameState.MagnumOpusBaseCost, 0, 4, double.MaxValue, window.UpgradeScreen.MagnumOpusOwnedTextUpgrade, window.UpgradeScreen.MagnumOpusCostTextUpgrade);
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
            window.UpgradeScreen.BackButtonBottom.Click += window.BackButton_Click;

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
        }

        private static void InitializeFragmentationButtons(MainWindow window)
        {
            window.FragmentationScreen.MelodiousFragmentButton.Click += (s, e) =>
            {
                if (window.GameState.Notes >= 1000)
                {
                    window.GameState.Notes -= 1000;
                    window.GameState.MelodiousOwned++;
                    UIUpdater.UpdateFragmentationUI(window, window.GameState);
                }
            };
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

        private static void InitializeSaveScoresButtons(MainWindow window)
        {
            window.SaveScoresScreen.LaCampanellaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.LaCampanellaMinorKeys, ref window.GameState.LaCampanellaMinorScales,
                    ref window.GameState.LaCampanellaMinorProgressions, ref window.GameState.LaCampanellaMajorSheets, 135000);

            window.SaveScoresScreen.EnigmaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.EnigmaMinorKeys, ref window.GameState.EnigmaMinorScales,
                    ref window.GameState.EnigmaMinorProgressions, ref window.GameState.EnigmaMajorSheets, 405000);

            window.SaveScoresScreen.FateButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.FateMinorKeys, ref window.GameState.FateMinorScales,
                    ref window.GameState.FateMinorProgressions, ref window.GameState.FateMajorSheets, 1215000);

            window.SaveScoresScreen.OdeToJoyButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.OdeToJoyMinorKeys, ref window.GameState.OdeToJoyMinorScales,
                    ref window.GameState.OdeToJoyMinorProgressions, ref window.GameState.OdeToJoyMajorSheets, 3645000);

            window.SaveScoresScreen.MoonlightSonataButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.MoonlightMinorKeys, ref window.GameState.MoonlightMinorScales,
                    ref window.GameState.MoonlightMinorProgressions, ref window.GameState.MoonlightMajorSheets, 5000);

            window.SaveScoresScreen.EroicaButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.EroicaMinorKeys, ref window.GameState.EroicaMinorScales,
                    ref window.GameState.EroicaMinorProgressions, ref window.GameState.EroicaMajorSheets, 15000);

            window.SaveScoresScreen.SwanLakeButton.Click += (s, e) =>
                SaveScoresManager.HandleSaveScoreClick(window, ref window.GameState.SwanLakeMinorKeys, ref window.GameState.SwanLakeMinorScales,
                    ref window.GameState.SwanLakeMinorProgressions, ref window.GameState.SwanLakeMajorSheets, 45000);
        }

        private static void InitializeManualCrafting(MainWindow window)
{
    // Moonlight Sonata - Minor and Major
    window.UnityTheSymphonyScreen.MoonlightMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftMoonlightMinor(window, ref window.GameState.MoonlightMinorOwned);

    window.UnityTheSymphonyScreen.MoonlightMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftMoonlightSonataMajor(window, ref window.GameState.MoonlightMajorOwned);

    // Eroica - Minor and Major
    window.UnityTheSymphonyScreen.EroicaMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEroicaMinor(window, ref window.GameState.EroicaMinorOwned);

    window.UnityTheSymphonyScreen.EroicaMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEroicaMajor(window, ref window.GameState.EroicaMajorOwned);

    // Swan Lake - Minor and Major
    window.UnityTheSymphonyScreen.SwanMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftSwanLakeMinor(window, ref window.GameState.SwanMinorOwned);

    window.UnityTheSymphonyScreen.SwanMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftSwanLakeMajor(window, ref window.GameState.SwanMajorOwned);

    // La Campanella - Minor and Major
    window.UnityTheSymphonyScreen.LaCampanellaMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftLaCampanellaMinor(window, ref window.GameState.LaCampanellaMinorOwned);

    window.UnityTheSymphonyScreen.LaCampanellaMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftLaCampanellaMajor(window, ref window.GameState.LaCampanellaMajorOwned);

    // Enigma - Minor and Major
    window.UnityTheSymphonyScreen.EnigmaMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEnigmaMinor(window, ref window.GameState.EnigmaMinorOwned);

    window.UnityTheSymphonyScreen.EnigmaMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftEnigmaMajor(window, ref window.GameState.EnigmaMajorOwned);

    // Fate - Minor and Major
    window.UnityTheSymphonyScreen.FateMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftFateMinor(window, ref window.GameState.FateMinorOwned);

    window.UnityTheSymphonyScreen.FateMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftFateMajor(window, ref window.GameState.FateMajorOwned);

    // Ode to Joy - Minor and Major
    window.UnityTheSymphonyScreen.OdeToJoyMinorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftOdeToJoyMinor(window, ref window.GameState.OdeToJoyMinorOwned);

    window.UnityTheSymphonyScreen.OdeToJoyMajorButton.Click += (s, e) =>
        UniteTheSymphonyManager.TryCraftOdeToJoyMajor(window, ref window.GameState.OdeToJoyMajorOwned);
}


    }
}
