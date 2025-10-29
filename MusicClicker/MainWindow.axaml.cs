using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Timers;

namespace MusicClicker
{
    public partial class MainWindow : Window
    {
        private Timer _timer;
        private Random _random = new Random();

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();

            UIUpdater.UpdateEssenceUI(this, gameState);

            // ------------------- BUTTON HANDLERS -------------------

            MoonlightHarmonyButton.Click += (s, e) =>
                HandleMajorSheetClick(ref gameState.MoonlightMajorSheets, ref gameState.MoonlightMajorKeys,
                    ref gameState.MoonlightMajorScales, ref gameState.MoonlightMajorProgressions,
                    MoonlightMajorSheetsOwnedText, MoonlightMajorKeysOwnedText,
                    MoonlightMajorScalesOwnedText, MoonlightMajorProgressionsOwnedText, "Moonlight Sonata");

            EroicaHarmonyButton.Click += (s, e) =>
                HandleMajorSheetClick(ref gameState.EroicaMajorSheets, ref gameState.EroicaMajorKeys,
                    ref gameState.EroicaMajorScales, ref gameState.EroicaMajorProgressions,
                    EroicaMajorSheetsOwnedText, EroicaMajorKeysOwnedText,
                    EroicaMajorScalesOwnedText, EroicaMajorProgressionsOwnedText, "Eroica");

            SwanLakeHarmonyButton.Click += (s, e) =>
                HandleMajorSheetClick(ref gameState.SwanLakeMajorSheets, ref gameState.SwanLakeMajorKeys,
                    ref gameState.SwanLakeMajorScales, ref gameState.SwanLakeMajorProgressions,
                    SwanLakeMajorSheetsOwnedText, SwanLakeMajorKeysOwnedText,
                    SwanLakeMajorScalesOwnedText, SwanLakeMajorProgressionsOwnedText, "Swan Lake");

            // Essence buttons
            EssenceBeethovenButton.Click += (s, e) => BuyEssence(ref gameState.EssenceBeethoven, 500, EssenceBeethovenOwnedText);
            EssencePyotrButton.Click += (s, e) => BuyEssence(ref gameState.EssencePyotr, 1500, EssencePyotrOwnedText);
            EssenceElgarButton.Click += (s, e) => BuyEssence(ref gameState.EssenceElgar, 4000, EssenceElgarOwnedText);
            EssenceLisztButton.Click += (s, e) => BuyEssence(ref gameState.EssenceLiszt, 6000, EssenceLisztOwnedText);

            // Upgrade buttons
            BuyChordButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.ChordOwned, gameState.ChordBaseCost, 0.5, 0, 1, ChordOwnedTextUpgrade, ChordCostTextUpgrade);
            BuyChordMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.ChordOwned, gameState.ChordBaseCost, 0.5, 0, double.MaxValue, ChordOwnedTextUpgrade, ChordCostTextUpgrade);

            BuyScaleButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.ScaleOwned, gameState.ScaleBaseCost, 3, 0, 1, ScaleOwnedTextUpgrade, ScaleCostTextUpgrade);
            BuyScaleMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.ScaleOwned, gameState.ScaleBaseCost, 3, 0, double.MaxValue, ScaleOwnedTextUpgrade, ScaleCostTextUpgrade);

            BuyOrchestraButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.OrchestraOwned, gameState.OrchestraBaseCost, 5, 0, 1, OrchestraOwnedTextUpgrade, OrchestraCostTextUpgrade);
            BuyOrchestraMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.OrchestraOwned, gameState.OrchestraBaseCost, 5, 0, double.MaxValue, OrchestraOwnedTextUpgrade, OrchestraCostTextUpgrade);

            BuySymphonyButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.SymphonyOwned, gameState.SymphonyBaseCost, 9, 0, 1, SymphonyOwnedTextUpgrade, SymphonyCostTextUpgrade);
            BuySymphonyMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.SymphonyOwned, gameState.SymphonyBaseCost, 9, 0, double.MaxValue, SymphonyOwnedTextUpgrade, SymphonyCostTextUpgrade);

            BuyAriaButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.AriaOwned, gameState.AriaBaseCost, 0, 1, 1, AriaOwnedTextUpgrade, AriaCostTextUpgrade);
            BuyAriaMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.AriaOwned, gameState.AriaBaseCost, 0, 1, double.MaxValue, AriaOwnedTextUpgrade, AriaCostTextUpgrade);

            BuyRequiemButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.RequiemOwned, gameState.RequiemBaseCost, 0, 2, 1, RequiemOwnedTextUpgrade, RequiemCostTextUpgrade);
            BuyRequiemMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.RequiemOwned, gameState.RequiemBaseCost, 0, 2, double.MaxValue, RequiemOwnedTextUpgrade, RequiemCostTextUpgrade);

            BuyOpusButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.OpusOwned, gameState.OpusBaseCost, 0, 3, 1, OpusOwnedTextUpgrade, OpusCostTextUpgrade);
            BuyOpusMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.OpusOwned, gameState.OpusBaseCost, 0, 3, double.MaxValue, OpusOwnedTextUpgrade, OpusCostTextUpgrade);

            BuyMagnumOpusButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.MagnumOpusOwned, gameState.MagnumOpusBaseCost, 0, 4, 1, MagnumOpusOwnedTextUpgrade, MagnumOpusCostTextUpgrade);
            BuyMagnumOpusMaxButton.Click += (s, e) =>
                BuyUpgrade(ref gameState.MagnumOpusOwned, gameState.MagnumOpusBaseCost, 0, 4, double.MaxValue, MagnumOpusOwnedTextUpgrade, MagnumOpusCostTextUpgrade);

            // Clicker
            ClickButton.Click += ClickButton_Click;

            // Cheat key
            this.KeyDown += MainWindow_KeyDown;

            // Navigation Buttons
            OpenUpgradesButton.Click += (s, e) =>
            {
                MainScreen.IsVisible = false;
                UpgradeScreen.IsVisible = true;
                UIUpdater.UpdateUI(this, gameState);
            };
            BackButtonBottom.Click += BackButton_Click;

            FragmentationButton.Click += (s, e) =>
            {
                MainScreen.IsVisible = false;
                FragmentationScreen.IsVisible = true;
                UIUpdater.UpdateFragmentationUI(this, gameState);
            };
            BackButtonFragmentation.Click += (s, e) =>
            {
                FragmentationScreen.IsVisible = false;
                MainScreen.IsVisible = true;
            };

            ResonanceButton.Click += (s, e) =>
            {
                MainScreen.IsVisible = false;
                SaveScoresScreen.IsVisible = true;
                UIUpdater.UpdateSaveScoresUI(this, gameState);
            };
            BackButtonSaveScores.Click += (s, e) =>
            {
                SaveScoresScreen.IsVisible = false;
                MainScreen.IsVisible = true;
            };

            MelodyButton.Click += (s, e) =>
            {
                MainScreen.IsVisible = false;
                HeartOfHarmonyScreen.IsVisible = true;
                UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
            };
            BackButtonHeartOfHarmony.Click += (s, e) =>
            {
                HeartOfHarmonyScreen.IsVisible = false;
                MainScreen.IsVisible = true;
            };

            HarmonyButton.Click += (s, e) =>
            {
                MainScreen.IsVisible = false;
                UnityTheSymphonyScreen.IsVisible = true;
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            };
            BackButtonUnitySymphony.Click += (s, e) =>
            {
                UnityTheSymphonyScreen.IsVisible = false;
                MainScreen.IsVisible = true;
            };

            // 🆕 Save Scores buttons
            LaCampanellaButton.Click += (s, e) =>
                HandleSaveScoreClick(ref gameState.LaCampanellaMinorKeys, ref gameState.LaCampanellaMinorScales,
                    ref gameState.LaCampanellaMinorProgressions, ref gameState.LaCampanellaMajorSheets, 12000);

            EnigmaButton.Click += (s, e) =>
                HandleSaveScoreClick(ref gameState.EnigmaMinorKeys, ref gameState.EnigmaMinorScales,
                    ref gameState.EnigmaMinorProgressions, ref gameState.EnigmaMajorSheets, 25000);

            FateButton.Click += (s, e) =>
                HandleSaveScoreClick(ref gameState.FateMinorKeys, ref gameState.FateMinorScales,
                    ref gameState.FateMinorProgressions, ref gameState.FateMajorSheets, 35000);

            OdeToJoyButton.Click += (s, e) =>
                HandleSaveScoreClick(ref gameState.OdeToJoyMinorKeys, ref gameState.OdeToJoyMinorScales,
                    ref gameState.OdeToJoyMinorProgressions, ref gameState.OdeToJoyMajorSheets, 55000);

            // Manual crafting (Unite the Symphony)
            MoonlightMinorButton.Click += (s, e) =>
                TryCraft(ref gameState.MoonlightMinorOwned, 5, 0, 10, 0, 3000, false, ref gameState.MoonlightMajorAbility);
            MoonlightMajorButton.Click += (s, e) =>
                TryCraft(ref gameState.MoonlightMajorOwned, 10, 0, 0, 10, 0, true, ref gameState.MoonlightMajorAbility);

            EroicaMinorButton.Click += (s, e) =>
                TryCraft(ref gameState.EroicaMinorOwned, 10, 0, 15, 0, 8000, false, ref gameState.EroicaMajorAbility);
            EroicaMajorButton.Click += (s, e) =>
                TryCraft(ref gameState.EroicaMajorOwned, 15, 0, 0, 15, 0, true, ref gameState.EroicaMajorAbility);

            SwanMinorButton.Click += (s, e) =>
                TryCraft(ref gameState.SwanMinorOwned, 0, 5, 20, 0, 15000, false, ref gameState.SwanMajorAbility);
            SwanMajorButton.Click += (s, e) =>
                TryCraft(ref gameState.SwanMajorOwned, 0, 10, 0, 20, 0, true, ref gameState.SwanMajorAbility);

            // La Campanella
            LaCampanellaMinorButton.Click += (s, e) =>
                TryCraft(ref gameState.LaCampanellaMinorOwned, 0, 0, 25, 0, 35000, false, ref gameState.LaCampanellaMajorAbility);
            LaCampanellaMajorButton.Click += (s, e) =>
                TryCraft(ref gameState.LaCampanellaMajorOwned, 0, 0, 0, 25, 0, true, ref gameState.LaCampanellaMajorAbility);

            // Enigma
            EnigmaMinorButton.Click += (s, e) =>
                TryCraft(ref gameState.EnigmaMinorOwned, 0, 10, 30, 0, 75000, false, ref gameState.EnigmaMajorAbility);
            EnigmaMajorButton.Click += (s, e) =>
                TryCraft(ref gameState.EnigmaMajorOwned, 0, 15, 0, 30, 0, true, ref gameState.EnigmaMajorAbility);

            // Fate
            FateMinorButton.Click += (s, e) =>
                TryCraft(ref gameState.FateMinorOwned, 20, 0, 35, 0, 125000, false, ref gameState.FateMajorAbility);
            FateMajorButton.Click += (s, e) =>
                TryCraft(ref gameState.FateMajorOwned, 25, 0, 0, 35, 0, true, ref gameState.FateMajorAbility);

            // Ode to Joy
            OdeToJoyMinorButton.Click += (s, e) =>
                TryCraft(ref gameState.OdeToJoyMinorOwned, 10, 0, 40, 0, 255000, false, ref gameState.OdeToJoyMajorAbility);
            OdeToJoyMajorButton.Click += (s, e) =>
                TryCraft(ref gameState.OdeToJoyMajorOwned, 15, 0, 0, 40, 0, true, ref gameState.OdeToJoyMajorAbility);

            // Fragmentation
            MelodiousFragmentButton.Click += (s, e) =>
            {
                if (gameState.Notes >= 1000)
                {
                    gameState.Notes -= 1000;
                    gameState.MelodiousOwned++;
                    UIUpdater.UpdateFragmentationUI(this, gameState);
                }
            };
            HarmoniousFragmentButton.Click += (s, e) =>
            {
                if (gameState.Notes >= 5000)
                {
                    gameState.Notes -= 5000;
                    gameState.HarmoniousOwned++;
                    UIUpdater.UpdateFragmentationUI(this, gameState);
                }
            };

            // Save Scores Crafting
            MoonlightSonataButton.Click += (s, e) =>
                HandleSaveScoreClick(ref gameState.MoonlightMinorKeys, ref gameState.MoonlightMinorScales,
                    ref gameState.MoonlightMinorProgressions, ref gameState.MoonlightMajorSheets, 1000);
            EroicaButton.Click += (s, e) =>
                HandleSaveScoreClick(ref gameState.EroicaMinorKeys, ref gameState.EroicaMinorScales,
                    ref gameState.EroicaMinorProgressions, ref gameState.EroicaMajorSheets, 3000);
            SwanLakeButton.Click += (s, e) =>
                HandleSaveScoreClick(ref gameState.SwanLakeMinorKeys, ref gameState.SwanLakeMinorScales,
                    ref gameState.SwanLakeMinorProgressions, ref gameState.SwanLakeMajorSheets, 8000);

            // Passive production & UI update
            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                gameState.Notes += gameState.NotesPerSecond;
                Dispatcher.UIThread.Post(() =>
                {
                    UIUpdater.UpdateUI(this, gameState);
    UIUpdater.UpdateFragmentationUI(this, gameState);
    UIUpdater.UpdateSaveScoresUI(this, gameState);
    UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
    UIUpdater.UpdateUnitySymphonyUI(this, gameState);
                });
            };
            _timer.Start();
        }

        // ------------------- CLICK & NAVIGATION -------------------
        private void ClickButton_Click(object? sender, RoutedEventArgs e)
        {
            gameState.Notes += gameState.NotesPerClick;
            UIUpdater.UpdateUI(this, gameState);
UIUpdater.UpdateFragmentationUI(this, gameState);
UIUpdater.UpdateSaveScoresUI(this, gameState);
UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
UIUpdater.UpdateUnitySymphonyUI(this, gameState);
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            UpgradeScreen.IsVisible = false;
            MainScreen.IsVisible = true;
        }

        // ------------------- HEART OF HARMONY -------------------
        private void HandleMajorSheetClick(
            ref int majorSheets,
            ref int majorKeys,
            ref int majorScales,
            ref int majorProgressions,
            TextBlock majorSheetsText,
            TextBlock majorKeysText,
            TextBlock majorScalesText,
            TextBlock majorProgressionsText,
            string scoreName)
        {
            if (majorSheets <= 0) return;

            majorSheets--;

            double roll = _random.NextDouble();
            if (roll < 0.33) majorKeys++;
            else if (roll < 0.66) majorScales++;
            else majorProgressions++;

            majorSheetsText.Text = $"{majorSheets} {scoreName} Major Sheets Owned";
            majorKeysText.Text = $"{majorKeys} {scoreName} Major Keys Owned";
            majorScalesText.Text = $"{majorScales} {scoreName} Major Scales Owned";
            majorProgressionsText.Text = $"{majorProgressions} {scoreName} Major Progressions Owned";
        }

        // ------------------- UPGRADE SYSTEM -------------------
        private void BuyUpgrade(ref int owned, double baseCost, double npsIncrease, double clickIncrease, double amount, TextBlock ownedText, TextBlock costText)
        {
            if (amount == double.MaxValue)
            {
                while (true)
                {
                    double cost = Math.Round(baseCost * Math.Pow(1.15, owned), 2);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;
                        owned++;
                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else break;
                }
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    double cost = Math.Round(baseCost * Math.Pow(1.15, owned), 2);
                    if (gameState.Notes >= cost)
                    {
                        gameState.Notes -= cost;
                        owned++;
                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else break;
                }
            }

            ownedText.Text = $"Number Owned: {owned}";
            costText.Text = $"Cost: {Math.Round(baseCost * Math.Pow(1.15, owned), 2)}";
        }

        private void BuyEssence(ref int essenceAmount, int cost, TextBlock ownedText)
        {
            if (gameState.Notes >= cost)
            {
                gameState.Notes -= cost;
                essenceAmount++;
                ownedText.Text = $"{essenceAmount} Owned";
            }
        }

        // ------------------- CRAFTING -------------------
        private bool TryCraft(ref int owned, int costEssenceBeethoven, int costEssencePyotr, int costMelodious, int costHarmonious, int addNps, bool isMajor, ref bool majorAbilityFlag)
        {
            if (gameState.EssenceBeethoven < costEssenceBeethoven) return false;
            if (gameState.EssencePyotr < costEssencePyotr) return false;
            if (gameState.MelodiousOwned < costMelodious) return false;
            if (gameState.HarmoniousOwned < costHarmonious) return false;

            gameState.EssenceBeethoven -= costEssenceBeethoven;
            gameState.EssencePyotr -= costEssencePyotr;
            gameState.MelodiousOwned -= costMelodious;
            gameState.HarmoniousOwned -= costHarmonious;

            owned++;
            if (addNps > 0) gameState.NotesPerSecond += addNps;
            if (isMajor) majorAbilityFlag = true;

            UIUpdater.UpdateUnitySymphonyUI(this, gameState);
UIUpdater.UpdateEssenceUI(this, gameState);
UIUpdater.UpdateFragmentationUI(this, gameState);

            return true;
        }

        private void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Space)
            {
                gameState.Notes += 1_000_000;
                UIUpdater.UpdateUI(this, gameState);
                UIUpdater.UpdateFragmentationUI(this, gameState);
                UIUpdater.UpdateSaveScoresUI(this, gameState);
                UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            }
        }

        private void HandleSaveScoreClick(ref int minorKeys, ref int minorScales, ref int minorProgressions, ref int majorSheets, int cost)
        {
            if (gameState.Notes >= cost)
            {
                gameState.Notes -= cost;
                if (_random.NextDouble() <= 0.50) minorKeys++;
                if (_random.NextDouble() <= 0.25) minorScales++;
                if (_random.NextDouble() <= 0.15) minorProgressions++;
                if (_random.NextDouble() <= 0.05) majorSheets++;
            }

            Dispatcher.UIThread.Post(() =>
            {
                UIUpdater.UpdateSaveScoresUI(this, gameState);
            });
        }
    }
}
