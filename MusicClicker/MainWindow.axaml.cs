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
        private UpgradeManager _upgradeManager;
        private SaveScoresManager _saveScoresManager;


// La Campanella
private int _laCampanellaMinorKeys = 0;
private int _laCampanellaMinorScales = 0;
private int _laCampanellaMinorProgressions = 0;
private int _laCampanellaMajorSheets = 0;

// Enigma
private int _enigmaMinorKeys = 0;
private int _enigmaMinorScales = 0;
private int _enigmaMinorProgressions = 0;
private int _enigmaMajorSheets = 0;

// Fate
private int _fateMinorKeys = 0;
private int _fateMinorScales = 0;
private int _fateMinorProgressions = 0;
private int _fateMajorSheets = 0;

// Ode to Joy
private int _odeToJoyMinorKeys = 0;
private int _odeToJoyMinorScales = 0;
private int _odeToJoyMinorProgressions = 0;
private int _odeToJoyMajorSheets = 0;
        // Base gameplay stats
        private double _notes = 0;
        private double _notesPerSecond = 0;
        private double _notesPerClick = 1;

        // Fragmentation state
        private int _melodiousOwned = 0;
        private int _harmoniousOwned = 0;

        // Essence resources
        private int _essenceBeethoven = 0;
        private int _essencePyotr = 0;
        private int _essenceElgar = 0;
        private int _essenceLiszt = 0;

        // Craft Ownership (Unite the Symphony)
        private int _moonlightMinorOwned = 0;
        private int _moonlightMajorOwned = 0;
        private int _eroicaMinorOwned = 0;
        private int _eroicaMajorOwned = 0;
        private int _swanMinorOwned = 0;
        private int _swanMajorOwned = 0;

        // Craft Ownership (Unite the Symphony)
private int _laCampanellaMinorOwned = 0;
private int _laCampanellaMajorOwned = 0;
private int _enigmaMinorOwned = 0;
private int _enigmaMajorOwned = 0;
private int _fateMinorOwned = 0;
private int _fateMajorOwned = 0;
private int _odeToJoyMinorOwned = 0;
private int _odeToJoyMajorOwned = 0;

// Moonlight Sonata
private int _moonlightMajorSheets = 0;
private int _moonlightMajorKeys = 0;
private int _moonlightMajorScales = 0;
private int _moonlightMajorProgressions = 0;

// Eroica
private int _eroicaMajorSheets = 0;
private int _eroicaMajorKeys = 0;
private int _eroicaMajorScales = 0;
private int _eroicaMajorProgressions = 0;

// Swan Lake
private int _swanLakeMajorSheets = 0;
private int _swanLakeMajorKeys = 0;
private int _swanLakeMajorScales = 0;
private int _swanLakeMajorProgressions = 0;

// Ability unlock flags
private bool _laCampanellaMajorAbility = false;
private bool _enigmaMajorAbility = false;
private bool _fateMajorAbility = false;
private bool _odeToJoyMajorAbility = false;

        // Ability unlock flags (major crafts)
        private bool _moonlightMajorAbility = false;
        private bool _eroicaMajorAbility = false;
        private bool _swanMajorAbility = false;
        
        private Timer _timer;
        private Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            // ------------------- SAVE SCORES MANAGER -------------------
_saveScoresManager = new SaveScoresManager(
    getNotes: () => _notes,
    setNotes: value => _notes = value
);
_saveScoresManager.RegisterButtons(this);
_saveScoresManager.UpdateUI(this, _notes);

            // ------------------- UPGRADE MANAGER -------------------
            _upgradeManager = new UpgradeManager(
    getNotes: () => _notes,
    setNotes: value => _notes = value,
    addNps: value => _notesPerSecond += value,
    addClick: value => _notesPerClick += value
);

            _upgradeManager.RegisterButtons(this); // Wires all upgrade buttons
            _upgradeManager.OnUpgradeChanged += () =>
{
    UpdateUpgradeUI();
    UpdateUI();  // <--- make NPS update immediately after buying upgrades
};
            UpdateEssenceUI();

            // Heart of Harmony Buttons
            MoonlightHarmonyButton.Click += (s, e) =>
                HandleMajorSheetClick(ref _moonlightMajorSheets, ref _moonlightMajorKeys, ref _moonlightMajorScales, ref _moonlightMajorProgressions,
                    MoonlightMajorSheetsOwnedText, MoonlightMajorKeysOwnedText, MoonlightMajorScalesOwnedText, MoonlightMajorProgressionsOwnedText,
                    "Moonlight Sonata");

            EroicaHarmonyButton.Click += (s, e) =>
                HandleMajorSheetClick(ref _eroicaMajorSheets, ref _eroicaMajorKeys, ref _eroicaMajorScales, ref _eroicaMajorProgressions,
                    EroicaMajorSheetsOwnedText, EroicaMajorKeysOwnedText, EroicaMajorScalesOwnedText, EroicaMajorProgressionsOwnedText,
                    "Eroica");

            SwanLakeHarmonyButton.Click += (s, e) =>
                HandleMajorSheetClick(ref _swanLakeMajorSheets, ref _swanLakeMajorKeys, ref _swanLakeMajorScales, ref _swanLakeMajorProgressions,
                    SwanLakeMajorSheetsOwnedText, SwanLakeMajorKeysOwnedText, SwanLakeMajorScalesOwnedText, SwanLakeMajorProgressionsOwnedText,
                    "Swan Lake");

            // Essence buttons
            EssenceBeethovenButton.Click += (s, e) => BuyEssence(ref _essenceBeethoven, 500, EssenceBeethovenOwnedText);
            EssencePyotrButton.Click += (s, e) => BuyEssence(ref _essencePyotr, 1500, EssencePyotrOwnedText);
            EssenceElgarButton.Click += (s, e) => BuyEssence(ref _essenceElgar, 4000, EssenceElgarOwnedText);
            EssenceLisztButton.Click += (s, e) => BuyEssence(ref _essenceLiszt, 6000, EssenceLisztOwnedText);

            // Clicker button
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

            // Fragmentation navigation
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

            // Save Scores navigation
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

            // Heart of Harmony navigation
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

            // Unite the Symphony navigation
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

            // Manual crafting buttons (Unite the Symphony)
            MoonlightMinorButton.Click += (s, e) =>
                TryCraft(ref _moonlightMinorOwned, 5, 0, 10, 0, 3000, false, ref _moonlightMajorAbility);
            MoonlightMajorButton.Click += (s, e) =>
                TryCraft(ref _moonlightMajorOwned, 10, 0, 0, 10, 0, true, ref _moonlightMajorAbility);

            EroicaMinorButton.Click += (s, e) =>
                TryCraft(ref _eroicaMinorOwned, 10, 0, 15, 0, 8000, false, ref _eroicaMajorAbility);
            EroicaMajorButton.Click += (s, e) =>
                TryCraft(ref _eroicaMajorOwned, 15, 0, 0, 15, 0, true, ref _eroicaMajorAbility);

            SwanMinorButton.Click += (s, e) =>
                TryCraft(ref _swanMinorOwned, 0, 5, 20, 0, 15000, false, ref _swanMajorAbility);
            SwanMajorButton.Click += (s, e) =>
                TryCraft(ref _swanMajorOwned, 0, 10, 0, 20, 0, true, ref _swanMajorAbility);

            // La Campanella
LaCampanellaMinorButton.Click += (s, e) => 
    TryCraft(ref _laCampanellaMinorOwned, 0, 0, 25, 0, 35000, false, ref _laCampanellaMajorAbility);

LaCampanellaMajorButton.Click += (s, e) => 
    TryCraft(ref _laCampanellaMajorOwned, 0, 0, 0, 25, 0, true, ref _laCampanellaMajorAbility);

// Enigma
EnigmaMinorButton.Click += (s, e) => 
    TryCraft(ref _enigmaMinorOwned, 0, 10, 30, 0, 75000, false, ref _enigmaMajorAbility);

EnigmaMajorButton.Click += (s, e) => 
    TryCraft(ref _enigmaMajorOwned, 0, 15, 0, 30, 0, true, ref _enigmaMajorAbility);

// Fate
FateMinorButton.Click += (s, e) => 
    TryCraft(ref _fateMinorOwned, 20, 0, 35, 0, 125000, false, ref _fateMajorAbility);

FateMajorButton.Click += (s, e) => 
    TryCraft(ref _fateMajorOwned, 25, 0, 0, 35, 0, true, ref _fateMajorAbility);

// Ode to Joy
OdeToJoyMinorButton.Click += (s, e) =>
    TryCraft(
        ref _odeToJoyMinorOwned,
        costEssenceBeethoven: 10,     // 10 Essence of Beethoven
        costEssencePyotr: 0,
        costMelodious: 40,            // 40 Melodious Fragments
        costHarmonious: 0,
        addNps: 255000,                // Minor craft adds 255,000 NPS
        isMajor: false,
        ref _odeToJoyMajorAbility);

OdeToJoyMajorButton.Click += (s, e) =>
    TryCraft(
        ref _odeToJoyMajorOwned,
        costEssenceBeethoven: 15,     // 15 Essence of Beethoven
        costEssencePyotr: 0,
        costMelodious: 0,
        costHarmonious: 40,           // 40 Harmonious Fragments
        addNps: 0,                     // Major craft does NOT add NPS directly
        isMajor: true,
        ref _odeToJoyMajorAbility);

            // Fragmentation Buttons
            MelodiousFragmentButton.Click += (s, e) =>
            {
                if (_notes >= 1000)
                {
                    _notes -= 1000;
                    _melodiousOwned++;
                    UpdateFragmentationUI();
                }
            };
            HarmoniousFragmentButton.Click += (s, e) =>
            {
                if (_notes >= 5000)
                {
                    _notes -= 5000;
                    _harmoniousOwned++;
                    UpdateFragmentationUI();
                }
            };

            // Passive production & UI update for all screens
            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                _notes += _notesPerSecond;
                Dispatcher.UIThread.Post(() =>
                {
                    // Update all screens' top note fields
                    UpdateUI();
                    UpdateFragmentationUI();
                    UpdateSaveScoresUI();
                    UpdateHeartOfHarmonyUI();
                    UpdateUnitySymphonyUI();
                });
            };
            _timer.Start();
        }

        private void UpdateSaveScoresUI()
{
    _saveScoresManager.UpdateUI(this, _notes);
}

        // ------------------- CLICK & NAVIGATION -------------------
        private void ClickButton_Click(object? sender, RoutedEventArgs e)
        {
            _notes += _notesPerClick;

            // Immediately update UI after click
            NotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
            UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(_notes, 1)}";

            // Optional: update Notes in other screens too, if needed
            FragmentationNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
            SaveScoresNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
            HeartOfHarmonyNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
            UnityNotesTextHeader.Text = $"Notes: {Math.Round(_notes, 1)}";
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

        private void UpdateUI()
{
    NotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
    UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(_notes, 1)}";
    NpsText.Text = $"Notes per Second: {Math.Round(_notesPerSecond, 1)}";

    // Keep other screens in sync
    FragmentationNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
    SaveScoresNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
    HeartOfHarmonyNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
    UnityNotesTextHeader.Text = $"Notes: {Math.Round(_notes, 1)}";
}


        // ------------------- UI UPDATES -------------------
        // Upgrade UI
        private void UpdateUpgradeUI()
        {
            ChordOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.ChordOwned}";
            ChordCostTextUpgrade.Text = $"Cost: {ChordCost()}";

            ScaleOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.ScaleOwned}";
            ScaleCostTextUpgrade.Text = $"Cost: {ScaleCost()}";

            OrchestraOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.OrchestraOwned}";
            OrchestraCostTextUpgrade.Text = $"Cost: {OrchestraCost()}";

            SymphonyOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.SymphonyOwned}";
            SymphonyCostTextUpgrade.Text = $"Cost: {SymphonyCost()}";

            AriaOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.AriaOwned}";
            AriaCostTextUpgrade.Text = $"Cost: {AriaCost()}";

            RequiemOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.RequiemOwned}";
            RequiemCostTextUpgrade.Text = $"Cost: {RequiemCost()}";

            OpusOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.OpusOwned}";
            OpusCostTextUpgrade.Text = $"Cost: {OpusCost()}";

            MagnumOpusOwnedTextUpgrade.Text = $"Number Owned: {_upgradeManager.MagnumOpusOwned}";
            MagnumOpusCostTextUpgrade.Text = $"Cost: {MagnumOpusCost()}";
        }

        // Upgrade cost helpers
        private double ChordCost() => Math.Round(_upgradeManager.ChordBaseCost * Math.Pow(1.15, _upgradeManager.ChordOwned), 2);
        private double ScaleCost() => Math.Round(_upgradeManager.ScaleBaseCost * Math.Pow(1.15, _upgradeManager.ScaleOwned), 2);
        private double OrchestraCost() => Math.Round(_upgradeManager.OrchestraBaseCost * Math.Pow(1.15, _upgradeManager.OrchestraOwned), 2);
        private double SymphonyCost() => Math.Round(_upgradeManager.SymphonyBaseCost * Math.Pow(1.15, _upgradeManager.SymphonyOwned), 2);
        private double AriaCost() => Math.Round(_upgradeManager.AriaBaseCost * Math.Pow(1.15, _upgradeManager.AriaOwned), 2);
        private double RequiemCost() => Math.Round(_upgradeManager.RequiemBaseCost * Math.Pow(1.15, _upgradeManager.RequiemOwned), 2);
        private double OpusCost() => Math.Round(_upgradeManager.OpusBaseCost * Math.Pow(1.15, _upgradeManager.OpusOwned), 2);
        private double MagnumOpusCost() => Math.Round(_upgradeManager.MagnumOpusBaseCost * Math.Pow(1.15, _upgradeManager.MagnumOpusOwned), 2);

        private void UpdateHeartOfHarmonyUI()
        {
            MoonlightMajorSheetsOwnedText.Text = $"{_moonlightMajorSheets} Moonlight Sonata Major Sheets Owned";
            MoonlightMajorKeysOwnedText.Text = $"{_moonlightMajorKeys} Moonlight Sonata Major Keys Owned";
            MoonlightMajorScalesOwnedText.Text = $"{_moonlightMajorScales} Moonlight Sonata Major Scales Owned";
            MoonlightMajorProgressionsOwnedText.Text = $"{_moonlightMajorProgressions} Moonlight Sonata Major Progressions Owned";

            EroicaMajorSheetsOwnedText.Text = $"{_eroicaMajorSheets} Eroica Major Sheets Owned";
            EroicaMajorKeysOwnedText.Text = $"{_eroicaMajorKeys} Eroica Major Keys Owned";
            EroicaMajorScalesOwnedText.Text = $"{_eroicaMajorScales} Eroica Major Scales Owned";
            EroicaMajorProgressionsOwnedText.Text = $"{_eroicaMajorProgressions} Eroica Major Progressions Owned";

            SwanLakeMajorSheetsOwnedText.Text = $"{_swanLakeMajorSheets} Swan Lake Major Sheets Owned";
            SwanLakeMajorKeysOwnedText.Text = $"{_swanLakeMajorKeys} Swan Lake Major Keys Owned";
            SwanLakeMajorScalesOwnedText.Text = $"{_swanLakeMajorScales} Swan Lake Major Scales Owned";
            SwanLakeMajorProgressionsOwnedText.Text = $"{_swanLakeMajorProgressions} Swan Lake Major Progressions Owned";

            // 🆕 New Scores
            LaCampanellaMinorKeysText.Text = $"{_laCampanellaMinorKeys} Minor Keys of La Campanella Owned";
            LaCampanellaMinorScalesText.Text = $"{_laCampanellaMinorScales} Minor Scales of La Campanella Owned";
            LaCampanellaMinorProgressionsText.Text = $"{_laCampanellaMinorProgressions} Minor Progressions of La Campanella Owned";
            LaCampanellaMajorSheetsText.Text = $"{_laCampanellaMajorSheets} Major Sheets of La Campanella Owned";

            EnigmaMinorKeysText.Text = $"{_enigmaMinorKeys} Minor Keys of Enigma Owned";
            EnigmaMinorScalesText.Text = $"{_enigmaMinorScales} Minor Scales of Enigma Owned";
            EnigmaMinorProgressionsText.Text = $"{_enigmaMinorProgressions} Minor Progressions of Enigma Owned";
            EnigmaMajorSheetsText.Text = $"{_enigmaMajorSheets} Major Sheets of Enigma Owned";

            FateMinorKeysText.Text = $"{_fateMinorKeys} Minor Keys of Fate Owned";
            FateMinorScalesText.Text = $"{_fateMinorScales} Minor Scales of Fate Owned";
            FateMinorProgressionsText.Text = $"{_fateMinorProgressions} Minor Progressions of Fate Owned";
            FateMajorSheetsText.Text = $"{_fateMajorSheets} Major Sheets of Fate Owned";

            OdeToJoyMinorKeysText.Text = $"{_odeToJoyMinorKeys} Minor Keys of Ode to Joy Owned";
            OdeToJoyMinorScalesText.Text = $"{_odeToJoyMinorScales} Minor Scales of Ode to Joy Owned";
            OdeToJoyMinorProgressionsText.Text = $"{_odeToJoyMinorProgressions} Minor Progressions of Ode to Joy Owned";
            OdeToJoyMajorSheetsText.Text = $"{_odeToJoyMajorSheets} Major Sheets of Ode to Joy Owned";

            HeartOfHarmonyNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
        }

        private void UpdateEssenceUI()
        {
            EssenceBeethovenOwnedText.Text = $"{_essenceBeethoven} Owned";
            EssencePyotrOwnedText.Text = $"{_essencePyotr} Owned";
            EssenceElgarOwnedText.Text = $"{_essenceElgar} Owned";
            EssenceLisztOwnedText.Text = $"{_essenceLiszt} Owned";
        }

        private void BuyEssence(ref int essenceAmount, int cost, TextBlock ownedText)
        {
            if (_notes >= cost)
            {
                _notes -= cost;
                essenceAmount++;
                ownedText.Text = $"{essenceAmount} Owned";
            }
        }

        private void UpdateFragmentationUI()
        {
            FragmentationNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
            MelodiousOwnedText.Text = $"{_melodiousOwned} Owned";
            HarmoniousOwnedText.Text = $"{_harmoniousOwned} Owned";
        }

        private void UpdateUnitySymphonyUI()
        {
            UnityNotesTextHeader.Text = $"Notes: {Math.Round(_notes, 1)}";
            MoonlightMinorOwnedText.Text = $"{_moonlightMinorOwned} Owned";
            MoonlightMajorOwnedText.Text = $"{_moonlightMajorOwned} Owned";
            EroicaMinorOwnedText.Text = $"{_eroicaMinorOwned} Owned";
            EroicaMajorOwnedText.Text = $"{_eroicaMajorOwned} Owned";
            SwanMinorOwnedText.Text = $"{_swanMinorOwned} Owned";
            SwanMajorOwnedText.Text = $"{_swanMajorOwned} Owned";
            LaCampanellaMinorOwnedText.Text = $"{_laCampanellaMinorOwned} Owned";
LaCampanellaMajorOwnedText.Text = $"{_laCampanellaMajorOwned} Owned";
EnigmaMinorOwnedText.Text = $"{_enigmaMinorOwned} Owned";
EnigmaMajorOwnedText.Text = $"{_enigmaMajorOwned} Owned";
FateMinorOwnedText.Text = $"{_fateMinorOwned} Owned";
FateMajorOwnedText.Text = $"{_fateMajorOwned} Owned";
OdeToJoyMinorOwnedText.Text = $"{_odeToJoyMinorOwned} Owned";
OdeToJoyMajorOwnedText.Text = $"{_odeToJoyMajorOwned} Owned";

        }

        // ------------------- CRAFTING -------------------
        private bool TryCraft(ref int owned, int costEssenceBeethoven, int costEssencePyotr, int costMelodious, int costHarmonious, int addNps, bool isMajor, ref bool majorAbilityFlag)
        {
            if (_essenceBeethoven < costEssenceBeethoven) return false;
            if (_essencePyotr < costEssencePyotr) return false;
            if (_melodiousOwned < costMelodious) return false;
            if (_harmoniousOwned < costHarmonious) return false;

            _essenceBeethoven -= costEssenceBeethoven;
            _essencePyotr -= costEssencePyotr;
            _melodiousOwned -= costMelodious;
            _harmoniousOwned -= costHarmonious;

            owned++;
            if (addNps > 0) _notesPerSecond += addNps;
            if (isMajor) majorAbilityFlag = true;

            UpdateUnitySymphonyUI();
    UpdateEssenceUI();
    UpdateFragmentationUI();

            return true;
        }

        // ------------------- UTIL -------------------
        private void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Space)
            {
                _notes += 1_000_000;
            }
        }
    }
}
