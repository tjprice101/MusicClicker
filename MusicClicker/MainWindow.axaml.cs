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

            UpdateEssenceUI();

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
                UpdateUI();
            };
            BackButtonBottom.Click += BackButton_Click;

            FragmentationButton.Click += (s, e) =>
            {
                MainScreen.IsVisible = false;
                FragmentationScreen.IsVisible = true;
                UpdateFragmentationUI();
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
                UpdateSaveScoresUI();
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
                UpdateHeartOfHarmonyUI();
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
                UpdateUnitySymphonyUI();
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
                    UpdateFragmentationUI();
                }
            };
            HarmoniousFragmentButton.Click += (s, e) =>
            {
                if (gameState.Notes >= 5000)
                {
                    gameState.Notes -= 5000;
                    gameState.HarmoniousOwned++;
                    UpdateFragmentationUI();
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
                    UpdateUI();
                    UpdateFragmentationUI();
                    UpdateSaveScoresUI();
                    UpdateHeartOfHarmonyUI();
                    UpdateUnitySymphonyUI();
                });
            };
            _timer.Start();
        }

        // ------------------- CLICK & NAVIGATION -------------------
        private void ClickButton_Click(object? sender, RoutedEventArgs e)
        {
            gameState.Notes += gameState.NotesPerClick;
            UpdateUI();
            UpdateFragmentationUI();
            UpdateSaveScoresUI();
            UpdateHeartOfHarmonyUI();
            UpdateUnitySymphonyUI();
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

        // ------------------- UI UPDATES -------------------
        private void UpdateUI()
        {
            NotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            NpsText.Text = $"Notes Per Second: {Math.Round(gameState.NotesPerSecond, 1)}";
            UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            ChordOwnedTextUpgrade.Text = $"Number Owned: {gameState.ChordOwned}";
            ScaleOwnedTextUpgrade.Text = $"Number Owned: {gameState.ScaleOwned}";
            OrchestraOwnedTextUpgrade.Text = $"Number Owned: {gameState.OrchestraOwned}";
            SymphonyOwnedTextUpgrade.Text = $"Number Owned: {gameState.SymphonyOwned}";
            AriaOwnedTextUpgrade.Text = $"Number Owned: {gameState.AriaOwned}";
            RequiemOwnedTextUpgrade.Text = $"Number Owned: {gameState.RequiemOwned}";
            OpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.OpusOwned}";
            MagnumOpusOwnedTextUpgrade.Text = $"Number Owned: {gameState.MagnumOpusOwned}";

            ChordCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.ChordBaseCost * Math.Pow(1.15, gameState.ChordOwned), 2)}";
            ScaleCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.ScaleBaseCost * Math.Pow(1.15, gameState.ScaleOwned), 2)}";
            OrchestraCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.OrchestraBaseCost * Math.Pow(1.15, gameState.OrchestraOwned), 2)}";
            SymphonyCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.SymphonyBaseCost * Math.Pow(1.15, gameState.SymphonyOwned), 2)}";
            AriaCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.AriaBaseCost * Math.Pow(1.15, gameState.AriaOwned), 2)}";
            RequiemCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.RequiemBaseCost * Math.Pow(1.15, gameState.RequiemOwned), 2)}";
            OpusCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.OpusBaseCost * Math.Pow(1.15, gameState.OpusOwned), 2)}";
            MagnumOpusCostTextUpgrade.Text = $"Cost: {Math.Round(gameState.MagnumOpusBaseCost * Math.Pow(1.15, gameState.MagnumOpusOwned), 2)}";
        }

        private void UpdateEssenceUI()
        {
            EssenceBeethovenOwnedText.Text = $"{gameState.EssenceBeethoven} Owned";
            EssencePyotrOwnedText.Text = $"{gameState.EssencePyotr} Owned";
            EssenceElgarOwnedText.Text = $"{gameState.EssenceElgar} Owned";
            EssenceLisztOwnedText.Text = $"{gameState.EssenceLiszt} Owned";
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

        private void UpdateFragmentationUI()
        {
            FragmentationNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            MelodiousOwnedText.Text = $"{gameState.MelodiousOwned} Owned";
            HarmoniousOwnedText.Text = $"{gameState.HarmoniousOwned} Owned";
        }

        private void UpdateSaveScoresUI()
        {
            MoonlightMinorKeysText.Text = $"{gameState.MoonlightMinorKeys} Minor Keys of Moonlight Sonata Owned";
            MoonlightMinorScalesText.Text = $"{gameState.MoonlightMinorScales} Minor Scales of Moonlight Sonata Owned";
            MoonlightMinorProgressionsText.Text = $"{gameState.MoonlightMinorProgressions} Minor Progressions of Moonlight Sonata Owned";
            MoonlightMajorSheetsText.Text = $"{gameState.MoonlightMajorSheets} Major Sheets of Moonlight Sonata Owned";

            EroicaMinorKeysText.Text = $"{gameState.EroicaMinorKeys} Minor Keys of Eroica Owned";
            EroicaMinorScalesText.Text = $"{gameState.EroicaMinorScales} Minor Scales of Eroica Owned";
            EroicaMinorProgressionsText.Text = $"{gameState.EroicaMinorProgressions} Minor Progressions of Eroica Owned";
            EroicaMajorSheetsText.Text = $"{gameState.EroicaMajorSheets} Major Sheets of Eroica Owned";

            SwanLakeMinorKeysText.Text = $"{gameState.SwanLakeMinorKeys} Minor Keys of Swan Lake Owned";
            SwanLakeMinorScalesText.Text = $"{gameState.SwanLakeMinorScales} Minor Scales of Swan Lake Owned";
            SwanLakeMinorProgressionsText.Text = $"{gameState.SwanLakeMinorProgressions} Minor Progressions of Swan Lake Owned";
            SwanLakeMajorSheetsText.Text = $"{gameState.SwanLakeMajorSheets} Major Sheets of Swan Lake Owned";
            SaveScoresNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
        }

        private void UpdateHeartOfHarmonyUI()
        {
            MoonlightMajorSheetsOwnedText.Text = $"{gameState.MoonlightMajorSheets} Moonlight Sonata Major Sheets Owned";
            MoonlightMajorKeysOwnedText.Text = $"{gameState.MoonlightMajorKeys} Moonlight Sonata Major Keys Owned";
            MoonlightMajorScalesOwnedText.Text = $"{gameState.MoonlightMajorScales} Moonlight Sonata Major Scales Owned";
            MoonlightMajorProgressionsOwnedText.Text = $"{gameState.MoonlightMajorProgressions} Moonlight Sonata Major Progressions Owned";

            EroicaMajorSheetsOwnedText.Text = $"{gameState.EroicaMajorSheets} Eroica Major Sheets Owned";
            EroicaMajorKeysOwnedText.Text = $"{gameState.EroicaMajorKeys} Eroica Major Keys Owned";
            EroicaMajorScalesOwnedText.Text = $"{gameState.EroicaMajorScales} Eroica Major Scales Owned";
            EroicaMajorProgressionsOwnedText.Text = $"{gameState.EroicaMajorProgressions} Eroica Major Progressions Owned";

            SwanLakeMajorSheetsOwnedText.Text = $"{gameState.SwanLakeMajorSheets} Swan Lake Major Sheets Owned";
            SwanLakeMajorKeysOwnedText.Text = $"{gameState.SwanLakeMajorKeys} Swan Lake Major Keys Owned";
            SwanLakeMajorScalesOwnedText.Text = $"{gameState.SwanLakeMajorScales} Swan Lake Major Scales Owned";
            SwanLakeMajorProgressionsOwnedText.Text = $"{gameState.SwanLakeMajorProgressions} Swan Lake Major Progressions Owned";

            HeartOfHarmonyNotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
        }

        private void UpdateUnitySymphonyUI()
        {
            UnityNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            MoonlightMinorOwnedText.Text = $"{gameState.MoonlightMinorOwned} Owned";
            MoonlightMajorOwnedText.Text = $"{gameState.MoonlightMajorOwned} Owned";
            EroicaMinorOwnedText.Text = $"{gameState.EroicaMinorOwned} Owned";
            EroicaMajorOwnedText.Text = $"{gameState.EroicaMajorOwned} Owned";
            SwanMinorOwnedText.Text = $"{gameState.SwanMinorOwned} Owned";
            SwanMajorOwnedText.Text = $"{gameState.SwanMajorOwned} Owned";
            LaCampanellaMinorOwnedText.Text = $"{gameState.LaCampanellaMinorOwned} Owned";
            LaCampanellaMajorOwnedText.Text = $"{gameState.LaCampanellaMajorOwned} Owned";
            EnigmaMinorOwnedText.Text = $"{gameState.EnigmaMinorOwned} Owned";
            EnigmaMajorOwnedText.Text = $"{gameState.EnigmaMajorOwned} Owned";
            FateMinorOwnedText.Text = $"{gameState.FateMinorOwned} Owned";
            FateMajorOwnedText.Text = $"{gameState.FateMajorOwned} Owned";
            OdeToJoyMinorOwnedText.Text = $"{gameState.OdeToJoyMinorOwned} Owned";
            OdeToJoyMajorOwnedText.Text = $"{gameState.OdeToJoyMajorOwned} Owned";
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

            UpdateUnitySymphonyUI();
            UpdateEssenceUI();
            UpdateFragmentationUI();

            return true;
        }

        private void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Space)
            {
                gameState.Notes += 1_000_000;
                UpdateUI();
                UpdateFragmentationUI();
                UpdateSaveScoresUI();
                UpdateHeartOfHarmonyUI();
                UpdateUnitySymphonyUI();
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
                UpdateSaveScoresUI();
            });
        }
    }
}
