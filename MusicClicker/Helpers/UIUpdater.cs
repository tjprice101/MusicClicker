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
            window.UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            window.ChordOwnedTextUpgrade.Text = $"Number Owned: {gameState.ChordOwned}";
            window.ScaleOwnedTextUpgrade.Text = $"Number Owned: {gameState.ScaleOwned}";
            window.OrchestraOwnedTextUpgrade.Text = $"Number Owned: {gameState.OrchestraOwned}";
            window.SymphonyOwnedTextUpgrade.Text = $"Number Owned: {gameState.SymphonyOwned}";
            window.AriaOwnedTextUpgrade.Text = $"Number Owned: {gameState.AriaOwned}";
            window.RequiemOwnedTextUpgrade.Text = $"Number Owned: {gameState.RequiemOwned}";
            window.OpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.OpusOwned}";
            window.MagnumOpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.MagnumOpusOwned}";

            window.ChordCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.ChordBaseCost * Math.Pow(1.15, gameState.ChordOwned), 2)}";
            window.ScaleCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.ScaleBaseCost * Math.Pow(1.15, gameState.ScaleOwned), 2)}";
            window.OrchestraCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.OrchestraBaseCost * Math.Pow(1.15, gameState.OrchestraOwned), 2)}";
            window.SymphonyCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.SymphonyBaseCost * Math.Pow(1.15, gameState.SymphonyOwned), 2)}";
            window.AriaCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.AriaBaseCost * Math.Pow(1.15, gameState.AriaOwned), 2)}";
            window.RequiemCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.RequiemBaseCost * Math.Pow(1.15, gameState.RequiemOwned), 2)}";
            window.OpusCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.OpusBaseCost * Math.Pow(1.15, gameState.OpusOwned), 2)}";
            window.MagnumOpusCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.MagnumOpusBaseCost * Math.Pow(1.15, gameState.MagnumOpusOwned), 2)}";
        }

        public static void UpdateEssenceUI(MainWindow window, GameState gameState)
        {
            window.EssenceBeethovenOwnedText.Text = $"{gameState.EssenceBeethoven} Owned";
            window.EssencePyotrOwnedText.Text = $"{gameState.EssencePyotr} Owned";
            window.EssenceElgarOwnedText.Text = $"{gameState.EssenceElgar} Owned";
            window.EssenceLisztOwnedText.Text = $"{gameState.EssenceLiszt} Owned";
        }

        public static void UpdateFragmentationUI(MainWindow window, GameState gameState)
        {
            window.FragmentationNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.MelodiousOwnedText.Text = $"{gameState.MelodiousOwned} Owned";
            window.HarmoniousOwnedText.Text = $"{gameState.HarmoniousOwned} Owned";
        }

        public static void UpdateSaveScoresUI(MainWindow window, GameState gameState)
        {
            window.MoonlightMinorKeysText.Text = $"{gameState.MoonlightMinorKeys} Minor Keys of Moonlight Sonata Owned";
            window.MoonlightMinorScalesText.Text = $"{gameState.MoonlightMinorScales} Minor Scales of Moonlight Sonata Owned";
            window.MoonlightMinorProgressionsText.Text = $"{gameState.MoonlightMinorProgressions} Minor Progressions of Moonlight Sonata Owned";
            window.MoonlightMajorSheetsText.Text = $"{gameState.MoonlightMajorSheets} Major Sheets of Moonlight Sonata Owned";

            window.EroicaMinorKeysText.Text = $"{gameState.EroicaMinorKeys} Minor Keys of Eroica Owned";
            window.EroicaMinorScalesText.Text = $"{gameState.EroicaMinorScales} Minor Scales of Eroica Owned";
            window.EroicaMinorProgressionsText.Text = $"{gameState.EroicaMinorProgressions} Minor Progressions of Eroica Owned";
            window.EroicaMajorSheetsText.Text = $"{gameState.EroicaMajorSheets} Major Sheets of Eroica Owned";

            window.SwanLakeMinorKeysText.Text = $"{gameState.SwanLakeMinorKeys} Minor Keys of Swan Lake Owned";
            window.SwanLakeMinorScalesText.Text = $"{gameState.SwanLakeMinorScales} Minor Scales of Swan Lake Owned";
            window.SwanLakeMinorProgressionsText.Text = $"{gameState.SwanLakeMinorProgressions} Minor Progressions of Swan Lake Owned";
            window.SwanLakeMajorSheetsText.Text = $"{gameState.SwanLakeMajorSheets} Major Sheets of Swan Lake Owned";
            window.SaveScoresNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
        }

        public static void UpdateHeartOfHarmonyUI(MainWindow window, GameState gameState)
        {
            window.MoonlightMajorSheetsOwnedText.Text = $"{gameState.MoonlightMajorSheets} Moonlight Sonata Major Sheets Owned";
            window.MoonlightMajorKeysOwnedText.Text = $"{gameState.MoonlightMajorKeys} Moonlight Sonata Major Keys Owned";
            window.MoonlightMajorScalesOwnedText.Text = $"{gameState.MoonlightMajorScales} Moonlight Sonata Major Scales Owned";
            window.MoonlightMajorProgressionsOwnedText.Text = $"{gameState.MoonlightMajorProgressions} Moonlight Sonata Major Progressions Owned";

            window.EroicaMajorSheetsOwnedText.Text = $"{gameState.EroicaMajorSheets} Eroica Major Sheets Owned";
            window.EroicaMajorKeysOwnedText.Text = $"{gameState.EroicaMajorKeys} Eroica Major Keys Owned";
            window.EroicaMajorScalesOwnedText.Text = $"{gameState.EroicaMajorScales} Eroica Major Scales Owned";
            window.EroicaMajorProgressionsOwnedText.Text = $"{gameState.EroicaMajorProgressions} Eroica Major Progressions Owned";

            window.SwanLakeMajorSheetsOwnedText.Text = $"{gameState.SwanLakeMajorSheets} Swan Lake Major Sheets Owned";
            window.SwanLakeMajorKeysOwnedText.Text = $"{gameState.SwanLakeMajorKeys} Swan Lake Major Keys Owned";
            window.SwanLakeMajorScalesOwnedText.Text = $"{gameState.SwanLakeMajorScales} Swan Lake Major Scales Owned";
            window.SwanLakeMajorProgressionsOwnedText.Text = $"{gameState.SwanLakeMajorProgressions} Swan Lake Major Progressions Owned";

            window.HeartOfHarmonyNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
        }

        public static void UpdateUnitySymphonyUI(MainWindow window, GameState gameState)
        {
            window.UnityNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.MoonlightMinorOwnedText.Text = $"{gameState.MoonlightMinorOwned} Owned";
            window.MoonlightMajorOwnedText.Text = $"{gameState.MoonlightMajorOwned} Owned";
            window.EroicaMinorOwnedText.Text = $"{gameState.EroicaMinorOwned} Owned";
            window.EroicaMajorOwnedText.Text = $"{gameState.EroicaMajorOwned} Owned";
            window.SwanMinorOwnedText.Text = $"{gameState.SwanMinorOwned} Owned";
            window.SwanMajorOwnedText.Text = $"{gameState.SwanMajorOwned} Owned";
            window.LaCampanellaMinorOwnedText.Text = $"{gameState.LaCampanellaMinorOwned} Owned";
            window.LaCampanellaMajorOwnedText.Text = $"{gameState.LaCampanellaMajorOwned} Owned";
            window.EnigmaMinorOwnedText.Text = $"{gameState.EnigmaMinorOwned} Owned";
            window.EnigmaMajorOwnedText.Text = $"{gameState.EnigmaMajorOwned} Owned";
            window.FateMinorOwnedText.Text = $"{gameState.FateMinorOwned} Owned";
            window.FateMajorOwnedText.Text = $"{gameState.FateMajorOwned} Owned";
            window.OdeToJoyMinorOwnedText.Text = $"{gameState.OdeToJoyMinorOwned} Owned";
            window.OdeToJoyMajorOwnedText.Text = $"{gameState.OdeToJoyMajorOwned} Owned";
        }
    }
}
