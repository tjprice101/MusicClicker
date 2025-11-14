using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Timers;
using Avalonia.Platform;

namespace MusicClicker
{
    // TODO List:
    // 1.) AI Generate an Image for a new Essence Button and background underneath of the main clicker button.
    // 2.) Hook up the Essence screen to work properly. 
    // 3.) AI Generate the "Upgrades" button and background to put to the right of the essence button. 
    // 4.) Add functionality to Heart of Harmony for the major versions of La Campanella and up. 

    public partial class MainWindow : Window
    {
        // ------------------- FIELDS / STATE -------------------

        // Timer that triggers once per second for passive note production.
        private Timer _timer;

        // Random generator used throughout the game (click effects, loot, etc.).
        private Random _random = new Random();

        // Centralized game state that stores all persistent gameplay values.
        private GameState gameState = new GameState();

        // Public accessor for other classes that need the shared GameState.
        public GameState GameState => gameState;

        // Global instance of TempoResonateManager so other systems can access it if necessary.
        public static TempoResonateManager GlobalTempoManager = null!;

        // ------------------- CONSTRUCTOR -------------------
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the Tempo Resonate manager using UI elements from the TempoResonateScreen user control.
            // These are wired up via code-behind accessors defined in TempoResonateScreen.axaml.cs.
            GlobalTempoManager = new TempoResonateManager(
                TempoResonateScreen.LeftDrawerPanel,
                TempoResonateScreen.EquippedScoreDisplay,
                TempoResonateScreen.EquippedScoreText,
                gameState,
                TempoResonateScreen.EquipPromptPanel,
                TempoResonateScreen.EquipPromptText,
                TempoResonateScreen.EquipYesButton,
                TempoResonateScreen.EquipNoButton
            );

            // Back button inside the Tempo Resonate Screen; returns to main screen.
            TempoResonateScreen.BackButtonTempoResonate.Click += BackButtonTempoResonate_Click;

            // Update essence-related UI immediately on startup.
            UIUpdater.UpdateEssenceUI(this, gameState);

            // Register handlers for all buy / upgrade buttons across all menus.
            ButtonInitializer.InitializeAllButtons(this);

            // Handle the main navigation button to open the Tempo Resonate system.
            TempoResonateButton.Click += TempoResonateButton_Click;
            TempoResonateScreen.BackButtonTempoResonate.Click += BackButtonTempoResonate_Click;

            // ------------------- PASSIVE PRODUCTION -------------------

            // Timer triggers every second to add passive Notes based on NotesPerSecond.
            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                // Apply passive NotesPerSecond production.
                gameState.Notes += gameState.NotesPerSecond;

                // Because UI must update on the UI thread, we dispatch UI updates safely.
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
            // Notes per click starts from the default value stored in GameState.
            // Defined as double to allow temporary multipliers and Major Ability boosts.
            double notesPerClick = gameState.NotesPerClick;

            // Moonlight Major — clicking also adds your NotesPerSecond to the click.
            if (gameState.MoonlightMajorAbility)
            {
                notesPerClick += gameState.NotesPerSecond;
            }
            // Fate Major — every 5 clicks, reward 30% of current note total.
            else if (gameState.FateMajorAbility)
            {
                gameState.FateCounter++;
                if (gameState.FateCounter == 5)
                {
                    gameState.FateCounter = 0;
                    gameState.Notes += (gameState.Notes * 0.30);
                }
            }

            // Apply click reward.
            gameState.Notes += notesPerClick;

            // Refresh UI everywhere that displays note totals or score-related elements.
            UIUpdater.UpdateUI(this, gameState);
            UIUpdater.UpdateFragmentationUI(this, gameState);
            UIUpdater.UpdateSaveScoresUI(this, gameState);
            UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
            UIUpdater.UpdateUnitySymphonyUI(this, gameState);
        }

        public void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Generic back button used on submenus such as Upgrades.
            UpgradeScreen.IsVisible = false;
            MainScreen.IsVisible = true;
        }

        public void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            // Debug key — space bar grants huge resources and major scores.
            // Convenient for testing without manual grinding.
            if (e.Key == Avalonia.Input.Key.Space)
            {
                gameState.Notes += 1_000_000;

                // Add ownership flags for all Major Scores.
                gameState.MoonlightMajorOwned += 1;
                gameState.EroicaMajorOwned += 1;
                gameState.SwanMajorOwned += 1;
                gameState.LaCampanellaMajorOwned += 1;
                gameState.EnigmaMajorOwned += 1;
                gameState.FateMajorOwned += 1;
                gameState.OdeToJoyMajorOwned += 1;

                // Update all score-related UI.
                UIUpdater.UpdateUI(this, gameState);
                UIUpdater.UpdateFragmentationUI(this, gameState);
                UIUpdater.UpdateSaveScoresUI(this, gameState);
                UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            }
        }

        // ------------------- TEMPO RESONATE HANDLERS -------------------

        public void TempoResonateButton_Click(object? sender, RoutedEventArgs e)
        {
            // Open Tempo Resonate screen and hide the main gameplay screen.
            MainScreen.IsVisible = false;
            TempoResonateScreen.IsVisible = true;
        }

        public void BackButtonTempoResonate_Click(object? sender, RoutedEventArgs e)
        {
            // Return to the main gameplay screen from the Tempo Resonate UI.
            TempoResonateScreen.IsVisible = false;
            MainScreen.IsVisible = true;
        }
    }
}
