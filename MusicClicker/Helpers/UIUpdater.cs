using Avalonia.Controls;
using System;
using MusicClicker;

namespace MusicClicker
{
    public static class UIUpdater
    {
        public static void UpdateUI(MainWindow window, GameState gameState)
        {
            window.NotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.NpsText.Text = $"Notes Per Second: {Math.Round(gameState.NotesPerSecond, 1)}";
            window.UpgradeScreen.UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            window.UpgradeScreen.ChordOwnedTextUpgrade.Text = $"Number Owned: {gameState.ChordOwned}";
            window.UpgradeScreen.ScaleOwnedTextUpgrade.Text = $"Number Owned: {gameState.ScaleOwned}";
            window.UpgradeScreen.OrchestraOwnedTextUpgrade.Text = $"Number Owned: {gameState.OrchestraOwned}";
            window.UpgradeScreen.SymphonyOwnedTextUpgrade.Text = $"Number Owned: {gameState.SymphonyOwned}";
            window.UpgradeScreen.AriaOwnedTextUpgrade.Text = $"Number Owned: {gameState.AriaOwned}";
            window.UpgradeScreen.RequiemOwnedTextUpgrade.Text = $"Number Owned: {gameState.RequiemOwned}";
            window.UpgradeScreen.OpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.OpusOwned}";
            window.UpgradeScreen.MagnumOpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.MagnumOpusOwned}";

            window.UpgradeScreen.ChordCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.ChordBaseCost * Math.Pow(1.15, gameState.ChordOwned), 2)}";
            window.UpgradeScreen.ScaleCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.ScaleBaseCost * Math.Pow(1.15, gameState.ScaleOwned), 2)}";
            window.UpgradeScreen.OrchestraCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.OrchestraBaseCost * Math.Pow(1.15, gameState.OrchestraOwned), 2)}";
            window.UpgradeScreen.SymphonyCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.SymphonyBaseCost * Math.Pow(1.15, gameState.SymphonyOwned), 2)}";
            window.UpgradeScreen.AriaCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.AriaBaseCost * Math.Pow(1.15, gameState.AriaOwned), 2)}";
            window.UpgradeScreen.RequiemCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.RequiemBaseCost * Math.Pow(1.15, gameState.RequiemOwned), 2)}";
            window.UpgradeScreen.OpusCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.OpusBaseCost * Math.Pow(1.15, gameState.OpusOwned), 2)}";
            window.UpgradeScreen.MagnumOpusCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.MagnumOpusBaseCost * Math.Pow(1.15, gameState.MagnumOpusOwned), 2)}";
        }

        public static void UpdateEssenceUI(MainWindow window, GameState gameState)
        {
            window.EssenceBeethovenOwnedText.Text = $"{gameState.EssenceBeethoven} Owned";
            window.EssencePyotrOwnedText.Text = $"{gameState.EssencePyotr} Owned";
            window.EssenceElgarOwnedText.Text = $"{gameState.EssenceElgar} Owned";
            window.EssenceLisztOwnedText.Text = $"{gameState.EssenceLiszt} Owned";

    //         // NEW: additional essences
    // window.EssenceVivaldiOwnedText.Text = $"{gameState.EssenceVivaldi} Owned";
    // window.EssenceCelanoOwnedText.Text = $"{gameState.EssenceCelano} Owned";
    // window.EssenceMozartOwnedText.Text = $"{gameState.EssenceMozart} Owned";
        }

        public static void UpdateFragmentationUI(MainWindow window, GameState gameState)
        {
            window.FragmentationScreen.FragmentationNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.FragmentationScreen.MelodiousOwnedText.Text = $"{gameState.MelodiousOwned} Owned";
            window.FragmentationScreen.HarmoniousOwnedText.Text = $"{gameState.HarmoniousOwned} Owned";
        }

        public static void UpdateSaveScoresUI(MainWindow window, GameState gameState)
        {
            window.SaveScoresScreen.MoonlightMinorKeysText.Text = $"{gameState.MoonlightMinorKeys} Minor Keys of Moonlight Sonata Owned";
            window.SaveScoresScreen.MoonlightMinorScalesText.Text = $"{gameState.MoonlightMinorScales} Minor Scales of Moonlight Sonata Owned";
            window.SaveScoresScreen.MoonlightMinorProgressionsText.Text = $"{gameState.MoonlightMinorProgressions} Minor Progressions of Moonlight Sonata Owned";
            window.SaveScoresScreen.MoonlightMajorSheetsText.Text = $"{gameState.MoonlightMajorSheets} Major Sheets of Moonlight Sonata Owned";

            window.SaveScoresScreen.EroicaMinorKeysText.Text = $"{gameState.EroicaMinorKeys} Minor Keys of Eroica Owned";
            window.SaveScoresScreen.EroicaMinorScalesText.Text = $"{gameState.EroicaMinorScales} Minor Scales of Eroica Owned";
            window.SaveScoresScreen.EroicaMinorProgressionsText.Text = $"{gameState.EroicaMinorProgressions} Minor Progressions of Eroica Owned";
            window.SaveScoresScreen.EroicaMajorSheetsText.Text = $"{gameState.EroicaMajorSheets} Major Sheets of Eroica Owned";

            window.SaveScoresScreen.SwanLakeMinorKeysText.Text = $"{gameState.SwanLakeMinorKeys} Minor Keys of Swan Lake Owned";
            window.SaveScoresScreen.SwanLakeMinorScalesText.Text = $"{gameState.SwanLakeMinorScales} Minor Scales of Swan Lake Owned";
            window.SaveScoresScreen.SwanLakeMinorProgressionsText.Text = $"{gameState.SwanLakeMinorProgressions} Minor Progressions of Swan Lake Owned";
            window.SaveScoresScreen.SwanLakeMajorSheetsText.Text = $"{gameState.SwanLakeMajorSheets} Major Sheets of Swan Lake Owned";
            // NEW: La Campanella
    window.SaveScoresScreen.LaCampanellaMinorKeysText.Text = $"{gameState.LaCampanellaMinorKeys} Minor Keys of La Campanella Owned";
    window.SaveScoresScreen.LaCampanellaMinorScalesText.Text = $"{gameState.LaCampanellaMinorScales} Minor Scales of La Campanella Owned";
    window.SaveScoresScreen.LaCampanellaMinorProgressionsText.Text = $"{gameState.LaCampanellaMinorProgressions} Minor Progressions of La Campanella Owned";
    window.SaveScoresScreen.LaCampanellaMajorSheetsText.Text = $"{gameState.LaCampanellaMajorSheets} Major Sheets of La Campanella Owned";

    // NEW: Enigma
    window.SaveScoresScreen.EnigmaMinorKeysText.Text = $"{gameState.EnigmaMinorKeys} Minor Keys of Enigma Owned";
    window.SaveScoresScreen.EnigmaMinorScalesText.Text = $"{gameState.EnigmaMinorScales} Minor Scales of Enigma Owned";
    window.SaveScoresScreen.EnigmaMinorProgressionsText.Text = $"{gameState.EnigmaMinorProgressions} Minor Progressions of Enigma Owned";
    window.SaveScoresScreen.EnigmaMajorSheetsText.Text = $"{gameState.EnigmaMajorSheets} Major Sheets of Enigma Owned";

    // NEW: Fate
    window.SaveScoresScreen.FateMinorKeysText.Text = $"{gameState.FateMinorKeys} Minor Keys of Fate Owned";
    window.SaveScoresScreen.FateMinorScalesText.Text = $"{gameState.FateMinorScales} Minor Scales of Fate Owned";
    window.SaveScoresScreen.FateMinorProgressionsText.Text = $"{gameState.FateMinorProgressions} Minor Progressions of Fate Owned";
    window.SaveScoresScreen.FateMajorSheetsText.Text = $"{gameState.FateMajorSheets} Major Sheets of Fate Owned";

    // NEW: Ode to Joy
    window.SaveScoresScreen.OdeToJoyMinorKeysText.Text = $"{gameState.OdeToJoyMinorKeys} Minor Keys of Ode to Joy Owned";
    window.SaveScoresScreen.OdeToJoyMinorScalesText.Text = $"{gameState.OdeToJoyMinorScales} Minor Scales of Ode to Joy Owned";
    window.SaveScoresScreen.OdeToJoyMinorProgressionsText.Text = $"{gameState.OdeToJoyMinorProgressions} Minor Progressions of Ode to Joy Owned";
    window.SaveScoresScreen.OdeToJoyMajorSheetsText.Text = $"{gameState.OdeToJoyMajorSheets} Major Sheets of Ode to Joy Owned";

    // Always update notes
    window.SaveScoresScreen.SaveScoresNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
        }

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

            // NEW: La Campanella
    window.HeartOfHarmonyScreen.LaCampanellaMajorSheetsOwnedText.Text = $"{gameState.LaCampanellaMajorSheets} La Campanella Major Sheets Owned";
    window.HeartOfHarmonyScreen.LaCampanellaMajorKeysOwnedText.Text = $"{gameState.LaCampanellaMajorKeys} La Campanella Major Keys Owned";
    window.HeartOfHarmonyScreen.LaCampanellaMajorScalesOwnedText.Text = $"{gameState.LaCampanellaMajorScales} La Campanella Major Scales Owned";
    window.HeartOfHarmonyScreen.LaCampanellaMajorProgressionsOwnedText.Text = $"{gameState.LaCampanellaMajorProgressions} La Campanella Major Progressions Owned";

    // NEW: Enigma
    window.HeartOfHarmonyScreen.EnigmaMajorSheetsOwnedText.Text = $"{gameState.EnigmaMajorSheets} Enigma Major Sheets Owned";
    window.HeartOfHarmonyScreen.EnigmaMajorKeysOwnedText.Text = $"{gameState.EnigmaMajorKeys} Enigma Major Keys Owned";
    window.HeartOfHarmonyScreen.EnigmaMajorScalesOwnedText.Text = $"{gameState.EnigmaMajorScales} Enigma Major Scales Owned";
    window.HeartOfHarmonyScreen.EnigmaMajorProgressionsOwnedText.Text = $"{gameState.EnigmaMajorProgressions} Enigma Major Progressions Owned";

    // NEW: Fate
    window.HeartOfHarmonyScreen.FateMajorSheetsOwnedText.Text = $"{gameState.FateMajorSheets} Fate Major Sheets Owned";
    window.HeartOfHarmonyScreen.FateMajorKeysOwnedText.Text = $"{gameState.FateMajorKeys} Fate Major Keys Owned";
    window.HeartOfHarmonyScreen.FateMajorScalesOwnedText.Text = $"{gameState.FateMajorScales} Fate Major Scales Owned";
    window.HeartOfHarmonyScreen.FateMajorProgressionsOwnedText.Text = $"{gameState.FateMajorProgressions} Fate Major Progressions Owned";

    // NEW: Ode to Joy
    window.HeartOfHarmonyScreen.OdeToJoyMajorSheetsOwnedText.Text = $"{gameState.OdeToJoyMajorSheets} Ode to Joy Major Sheets Owned";
    window.HeartOfHarmonyScreen.OdeToJoyMajorKeysOwnedText.Text = $"{gameState.OdeToJoyMajorKeys} Ode to Joy Major Keys Owned";
    window.HeartOfHarmonyScreen.OdeToJoyMajorScalesOwnedText.Text = $"{gameState.OdeToJoyMajorScales} Ode to Joy Major Scales Owned";
    window.HeartOfHarmonyScreen.OdeToJoyMajorProgressionsOwnedText.Text = $"{gameState.OdeToJoyMajorProgressions} Ode to Joy Major Progressions Owned";

            window.HeartOfHarmonyScreen.HeartOfHarmonyNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
        }

        public static void UpdateUnitySymphonyUI(MainWindow window, GameState gameState)
        {
            window.UnityTheSymphonyScreen.UnityNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.UnityTheSymphonyScreen.MoonlightMinorOwnedText.Text = $"{gameState.MoonlightMinorOwned} Owned";
            window.UnityTheSymphonyScreen.MoonlightMajorOwnedText.Text = $"{gameState.MoonlightMajorOwned} Owned";
            window.UnityTheSymphonyScreen.EroicaMinorOwnedText.Text = $"{gameState.EroicaMinorOwned} Owned";
            window.UnityTheSymphonyScreen.EroicaMajorOwnedText.Text = $"{gameState.EroicaMajorOwned} Owned";
            window.UnityTheSymphonyScreen.SwanMinorOwnedText.Text = $"{gameState.SwanMinorOwned} Owned";
            window.UnityTheSymphonyScreen.SwanMajorOwnedText.Text = $"{gameState.SwanMajorOwned} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMinorOwnedText.Text = $"{gameState.LaCampanellaMinorOwned} Owned";
            window.UnityTheSymphonyScreen.LaCampanellaMajorOwnedText.Text = $"{gameState.LaCampanellaMajorOwned} Owned";
            window.UnityTheSymphonyScreen.EnigmaMinorOwnedText.Text = $"{gameState.EnigmaMinorOwned} Owned";
            window.UnityTheSymphonyScreen.EnigmaMajorOwnedText.Text = $"{gameState.EnigmaMajorOwned} Owned";
            window.UnityTheSymphonyScreen.FateMinorOwnedText.Text = $"{gameState.FateMinorOwned} Owned";
            window.UnityTheSymphonyScreen.FateMajorOwnedText.Text = $"{gameState.FateMajorOwned} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMinorOwnedText.Text = $"{gameState.OdeToJoyMinorOwned} Owned";
            window.UnityTheSymphonyScreen.OdeToJoyMajorOwnedText.Text = $"{gameState.OdeToJoyMajorOwned} Owned";
            MainWindow.GlobalTempoManager.RefreshDrawer();
        }
    }
}
