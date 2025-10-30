using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Timers;
using Avalonia.Platform;

namespace MusicClicker

// TODO List:
// 1.) AI Generate an Image for a new Essence Button and background underneath of the main clicker button.
// 2.) Hook up the Essence screen to work properly. 
// 3.) AI Generate the "Upgrades" button and background to put to the right of the essence button. 
// 4.) Add functionality to Heart of Harmony for the major versions of La Campanella and up. 

{
    public partial class MainWindow : Window
    {
        // ------------------- FIELDS / STATE -------------------
        private Timer _timer;
        private Random _random = new Random();
        private GameState gameState = new GameState();
        public GameState GameState => gameState;
        public static TempoResonateManager GlobalTempoManager = null!;

        // ------------------- CONSTRUCTOR -------------------
        public MainWindow()
{
    InitializeComponent();

    GlobalTempoManager = new TempoResonateManager(
    LeftDrawerPanel,
    EquippedScoreDisplay,
    EquippedScoreText,
    gameState,
    EquipPromptPanel,
    EquipPromptText,
    EquipYesButton,
    EquipNoButton
);

BackButtonTempoResonate.Click += BackButtonTempoResonate_Click;


    UIUpdater.UpdateEssenceUI(this, gameState);

    // ------------------- BUTTON HANDLERS -------------------
    ButtonInitializer.InitializeAllButtons(this);

    // Hook up Tempo Resonate
    TempoResonateButton.Click += TempoResonateButton_Click;
    BackButtonTempoResonate.Click += BackButtonTempoResonate_Click;

    // ------------------- PASSIVE PRODUCTION -------------------
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

        // ------------------- CLICK & NAVIGATION HANDLERS -------------------
        public void ClickButton_Click(object? sender, RoutedEventArgs e)
{
    // Make notesPerClick a double to allow temporary boosts
    double notesPerClick = gameState.NotesPerClick;

    // Check the ability flag in gameState (or MainWindow, wherever you defined it)
    if (gameState.MoonlightMajorAbility) 
    {
        notesPerClick += 5000; // 5000 is fine as an integer literal
    }

    // Increment notes
    gameState.Notes += notesPerClick;

    // Update all UI
    UIUpdater.UpdateUI(this, gameState);
    UIUpdater.UpdateFragmentationUI(this, gameState);
    UIUpdater.UpdateSaveScoresUI(this, gameState);
    UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
    UIUpdater.UpdateUnitySymphonyUI(this, gameState);
}

        public void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            UpgradeScreen.IsVisible = false;
            MainScreen.IsVisible = true;
        }

        public void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Space)
            {
                gameState.Notes += 1_000_000;
                gameState.MoonlightMajorOwned += 1;
                gameState.EroicaMajorOwned += 1;
                UIUpdater.UpdateUI(this, gameState);
                UIUpdater.UpdateFragmentationUI(this, gameState);
                UIUpdater.UpdateSaveScoresUI(this, gameState);
                UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            }
        }

        // Open Tempo Resonate screen
public void TempoResonateButton_Click(object? sender, RoutedEventArgs e)
{
    MainScreen.IsVisible = false;       // Hide main screen
    TempoResonateScreen.IsVisible = true; // Show Tempo Resonate screen
}

// Back button
public void BackButtonTempoResonate_Click(object? sender, RoutedEventArgs e)
{
    TempoResonateScreen.IsVisible = false;
    MainScreen.IsVisible = true;
}
    }
}
