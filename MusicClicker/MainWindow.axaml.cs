/*
 * File: MainWindow.axaml.cs
 * Summary: Main window implementation for MusicClicker.
 * Purpose: Manages game state, timers, UI updates, and navigation among screens.
 * Notes: Controls the game loop, carousel animation, and screen transitions.
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using MusicClicker.Helpers;

namespace MusicClicker
{
    /// <summary>
    /// MainWindow is the primary window for the Music Clicker game.
    /// It manages the game state, UI updates, carousel navigation, and save/load functionality.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ------------------- EXISTING FIELDS -------------------
        
        // Game loop timer: we use a DispatcherTimer + Stopwatch to accumulate fractional
        // notes continuously (e.g., like Cookie Clicker) while limiting how often
        // the UI is refreshed. This keeps math frequent but UI updates batched for performance.
        private DispatcherTimer _gameLoopTimer = null!;
        private System.Diagnostics.Stopwatch _stopwatch = null!;
        // Background timer that advances Notes continuously on a threadpool thread
        private System.Timers.Timer _backgroundNpsTimer = null!;
        private System.Diagnostics.Stopwatch _bgStopwatch = null!;
        
        // DispatcherTimer that fires every 30 seconds to auto-save the game
        private DispatcherTimer _saveTimer = null!;
        
        // Random number generator for any randomized game mechanics
        private Random _random = new Random();
        
        // The core game state object containing all player progress data
        private GameState gameState;

        // Cached reference to the full-screen fader rectangle to avoid repeated FindControl lookups
        private Avalonia.Controls.Shapes.Rectangle? _screenFader;
        
        // Public accessor for game state to allow other components to read it
        public GameState GameState => gameState;
        
        // Global manager for the Tempo Resonate feature (musical score system)
        public static TempoResonateManager GlobalTempoManager = null!;

        // Smoothed display values used for visual interpolation (updated at FRAME_RATE)
        public double DisplayedNotes { get; set; }
        public double DisplayedNps { get; set; }
        
        // Flag set while the user is actively interacting (scrolling, dragging sliders, etc.).
        // When true, UI text updates are temporarily suppressed to avoid layout churn.
        public bool IsUserInteracting { get; private set; } = false;

        // Short timer used to debounce interaction end; when it elapses we set IsUserInteracting=false.
        private DispatcherTimer _interactionTimer = null!;
        
        // Track last mouse position for floating text
        private Point _lastClickPosition;

        // ------------------- CAROUSEL FIELDS -------------------
        
        // Index of the currently selected button in the carousel (0-7)
        private int currentIndex = 0;
        
        // The target rotation angle the carousel is animating towards
        private double targetRotation = 0;
        
        // The current rotation angle of the carousel
        private double currentRotation = 0;
        
        // Flag indicating whether the carousel is currently animating to a target position
        private bool isAnimating = false;
        
        // The radius of the circular carousel path in pixels
        private const double RADIUS = 350;
        
        // Total number of buttons in the carousel (8 different game screens)
        private const int BUTTON_COUNT = 8;
        // Target frame rate for animations (frames per second). Start higher for smoother visuals.
        private int _frameRate = 144;

        // Animation timer: fires at `_frameRate` to produce smooth carousel animation
        private DispatcherTimer animationTimer = null!;

        // Animation performance tracking for adaptive framerate
        private System.Diagnostics.Stopwatch _animTickStopwatch = new System.Diagnostics.Stopwatch();
        private double _animAverageMs = 0.0;
        private int _animSamples = 0;
        
        // Fields for implementing drag-to-rotate functionality
        private bool isDragging = false;              // Whether user is currently dragging
        private Point lastDragPoint;                  // Last recorded mouse/touch position
        private double dragVelocity = 0;              // Current velocity of drag motion
        private double dragMomentum = 0;              // Momentum after drag release (for inertia effect)
        
        // List storing each carousel button along with its transform components for positioning
        private List<(Button button, TranslateTransform translate, ScaleTransform scale)> carouselButtons = null!;



        /// <summary>
        /// Restores customizations (clicker image and background) from saved game state.
        /// Called after loading a saved game to apply the player's previous visual choices.
        /// </summary>
        private void RestoreSavedCustomizations()
        {
            try
            {
                // Restore the clicker button image (replace Content with a cached Image)
                if (!string.IsNullOrEmpty(gameState.CurrentClickerImage))
                {
                    var bmp = MusicClicker.Helpers.ImageHelpers.GetBitmap(gameState.CurrentClickerImage, 128);
                    if (bmp != null)
                    {
                        var img = new Avalonia.Controls.Image
                        {
                            Source = bmp,
                            Stretch = Avalonia.Media.Stretch.Uniform
                        };
                        // Prevent the image from intercepting pointer events so the Button still receives clicks
                        img.IsHitTestVisible = false;
                        ClickButton.Content = img;
                    }
                }

                // Restore background image if present in save
                if (!string.IsNullOrEmpty(gameState.CurrentBackgroundImage))
                {
                    var bg = MusicClicker.Helpers.ImageHelpers.GetBitmap(gameState.CurrentBackgroundImage, 1920);
                    if (bg != null)
                    {
                        this.Background = new Avalonia.Media.ImageBrush
                        {
                            Source = bg,
                            Stretch = Avalonia.Media.Stretch.UniformToFill
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Log errors but continue with defaults
                Console.WriteLine($"Failed to restore customizations: {ex.Message}");
            }
        }

        // ------------------- CONSTRUCTOR -------------------
        
        /// <summary>
        /// Constructor initializes the main window, loads saved game, sets up UI, and starts timers.
        /// </summary>
        public MainWindow()
        {
            // Initialize the window and all UI components defined in AXAML
            InitializeComponent();

            // Interaction debounce timer: after user input stops, clear the interaction flag.
            // Start with a base interval and allow exponential backoff while frequent interactions continue.
            _interactionTimer = new DispatcherTimer();
            _interactionTimer.Interval = TimeSpan.FromMilliseconds(400);
            _interactionTimer.Tick += (s, e) =>
            {
                // Reset interaction flag and restore debounce to base value
                IsUserInteracting = false;
                _interactionTimer.Stop();
                _interactionTimer.Interval = TimeSpan.FromMilliseconds(400);
            };

            // Listen for pointer and wheel events at the window level — these will fire
            // during scrolling and slider drags so we can temporarily suppress heavy UI updates.
            // Use AddHandler to capture pointer wheel events even when handled by ScrollViewer
            this.AddHandler(InputElement.PointerWheelChangedEvent, new EventHandler<Avalonia.Input.PointerWheelEventArgs>(MainWindow_PointerWheelChanged), handledEventsToo: true);
            this.AddHandler(InputElement.PointerPressedEvent, new EventHandler<Avalonia.Input.PointerPressedEventArgs>(MainWindow_PointerPressed), handledEventsToo: true);
            this.AddHandler(InputElement.PointerReleasedEvent, new EventHandler<Avalonia.Input.PointerReleasedEventArgs>(MainWindow_PointerReleased), handledEventsToo: true);

            // Cache the fader rectangle (used for screen transitions) and move it to top once
            try
            {
                _screenFader = this.FindControl<Rectangle>("ScreenFader");
                if (_screenFader != null && this.Content is Panel root)
                {
                    if (root.Children.Contains(_screenFader))
                    {
                        root.Children.Remove(_screenFader);
                        root.Children.Add(_screenFader);
                    }
                }
            }
            catch { }

            // Try to load a previously saved game
            string loadErr;
            if (SaveManager.TryLoad(out GameState loaded, out loadErr))
            {
                // Load successful - use the loaded game state
                gameState = loaded;
                Console.WriteLine("Game loaded successfully!");

                // Restore any visual customizations the player had applied
                RestoreSavedCustomizations();
            }
            else
            {
                // Load failed or no save exists - start fresh with default values
                gameState = new GameState();
                Console.WriteLine($"Starting new game. Load error: {loadErr}");
            }

            // Initialize smoothed display values to the loaded/current game state so
            // the animation-driven display starts from the correct baseline.
            DisplayedNotes = gameState.Notes;
            DisplayedNps = gameState.NotesPerSecond;

            // Set up the carousel UI (circular button navigation system)
            InitializeCarousel();

            // Initialize the Tempo Resonate system (musical score management)
            GlobalTempoManager = new TempoResonateManager(
                TempoResonateScreen.LeftDrawerPanel,
                TempoResonateScreen.EquippedScoreDisplay,
                TempoResonateScreen.EquippedScoreText,
                gameState,
                TempoResonateScreen.EquipPromptPanel,
                TempoResonateScreen.EquipPromptText,
                TempoResonateScreen.EquipYesButton,
                TempoResonateScreen.EquipNoButton,
                TempoResonateScreen.RightDrawerPanel,
                TempoResonateScreen.EquippedWeaponDisplay1,
                TempoResonateScreen.EquippedWeaponText1,
                TempoResonateScreen.EquippedWeaponDisplay2,
                TempoResonateScreen.EquippedWeaponText2,
                TempoResonateScreen.DuetResonanceText,
                TempoResonateScreen  // Pass the screen for tooltip registration
            );

            // Initialize Duet Ability Screen
            if (DuetAbilityScreen != null)
            {
                DuetAbilityScreen.Initialize(gameState, this);
            }
            
            // Check if duet ability button should be visible on startup
            UpdateDuetAbilityButtonVisibility();

            // Wire up button click handlers
            TempoResonateScreen.BackButtonTempoResonate.Click += BackButtonTempoResonate_Click;
            ButtonInitializer.InitializeAllButtons(this);
            TempoResonateButton.Click += TempoResonateButton_Click;
            CacophonicDreamsButton.Click += CacophonicDreamsButton_Click;
            
            // Wire up main screen crescendance consume buttons
            if (MainConsumeReveredButton != null)
                MainConsumeReveredButton.Click += (s, e) => MusicClicker.Armory.WeaponAbilities.SwanLake_ConsumeReveredFeathers(gameState);
            if (MainConsumeChromaticButton != null)
                MainConsumeChromaticButton.Click += (s, e) => MusicClicker.Armory.WeaponAbilities.SwanLake_ConsumeChromaticFeathers(gameState);
            if (MainConsumePolyphonicButton != null)
                MainConsumePolyphonicButton.Click += (s, e) => MusicClicker.Armory.WeaponAbilities.SwanLake_ConsumePolyphonicFeather(gameState);

            // Create and start a DispatcherTimer that ticks frequently (every 100ms)
            // to accumulate fractional Notes based on NotesPerSecond. We use a
            // Stopwatch to measure exact elapsed time between ticks to avoid drift.
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _gameLoopTimer = new DispatcherTimer();
            _gameLoopTimer.Interval = TimeSpan.FromMilliseconds(100); // 10hz tick for UI batching

            // We batch UI updates to a slightly lower frequency than accumulation to
            // avoid excessive UI work. Configure UI update interval (milliseconds).
            const double uiUpdateIntervalMs = 250; // update UI 4 times per second (reduces UI churn)
            double uiAccumulatorMs = 0;

            _gameLoopTimer.Tick += (s, e) =>
            {
                // Measure real elapsed time since last tick
                double elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                // Update display for responsive Notes counter
                try
                {
                    UIUpdater.UpdateNotesOnly(this, gameState);
                }
                catch { }

                // Let the TempoResonateManager inspect state each tick and update its UI
                // (e.g., reset equipped image when a resonated score is lost).
                try
                {
                    if (GlobalTempoManager != null)
                        GlobalTempoManager.TickUpdate();
                }
                catch { }
                
                // Update main screen crescendance panel
                try
                {
                    UpdateMainScreenCrescendancePanel();
                }
                catch { }

                // Accumulate time and only run UI updates at the configured rate
                uiAccumulatorMs += elapsedSeconds * 1000.0;
                if (uiAccumulatorMs >= uiUpdateIntervalMs)
                {
                    uiAccumulatorMs = 0;
                    // Update UI on the main thread (DispatcherTimer already runs on UI thread)
                    UIUpdater.UpdateUI(this, gameState);
                    UIUpdater.UpdateFragmentationUI(this, gameState);
                    UIUpdater.UpdateSaveScoresUI(this, gameState);
                    UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
                    UIUpdater.UpdateUnitySymphonyUI(this, gameState);
                    GlobalTempoManager?.RefreshDrawer();
                }
            };
            _gameLoopTimer.Start();

            // Start a background timer to advance Notes continuously even when the UI thread
            // is busy. This prevents progress from pausing during navigation or heavier UI work.
            _bgStopwatch = System.Diagnostics.Stopwatch.StartNew();
            _backgroundNpsTimer = new System.Timers.Timer(100); // 100ms interval
            _backgroundNpsTimer.Elapsed += (sender, ev) =>
            {
                try
                {
                    double elapsed = _bgStopwatch.Elapsed.TotalSeconds;
                    _bgStopwatch.Restart();
                    if (gameState != null && gameState.NotesPerSecond != 0)
                    {
                        double effectiveNps = gameState.NotesPerSecond;

                        // Check if Astral Chainripper boost has expired
                        if (gameState.AstralChainripperNpsBoostActive && DateTime.Now > gameState.AstralChainripperNpsBoostExpiry)
                        {
                            gameState.AstralChainripperNpsBoostActive = false;
                        }

                        // Check if NPS freeze has expired
                        if (gameState.NpsFrozen && DateTime.Now > gameState.NpsFreezeExpiry)
                        {
                            gameState.NpsFrozen = false;
                            // Keep FrozenNpsValue for reference, don't zero it
                        }

                        // Check if Winter Duet has expired and unfreeze NPS
                        if (gameState.WinterDuetActive && DateTime.Now > gameState.WinterDuetExpiry)
                        {
                            gameState.WinterDuetActive = false;
                            gameState.NpsFrozen = false;
                            gameState.WinterDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.Winter);
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Dies Irae Duet has expired
                        if (gameState.DiesIraeDuetActive && DateTime.Now > gameState.DiesIraeDuetExpiry)
                        {
                            gameState.DiesIraeDuetActive = false;
                            gameState.SevenSealsCounter = 0;
                            gameState.DiesIraeDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.DiesIrae);
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Blizzard's Bounty has expired
                        if (gameState.BlizzardBountyNpsBonus > 0 && DateTime.Now > gameState.BlizzardBountyExpiry)
                        {
                            gameState.BlizzardBountyNpsBonus = 0;
                        }

                        // Check if Moonlight Duet has expired - start cooldown when it expires naturally
                        if (gameState.MoonlightDuetActive && DateTime.Now > gameState.MoonlightDuetExpiry)
                        {
                            gameState.MoonlightDuetActive = false;
                            gameState.MoonlightDuetCooldownExpiry = DateTime.Now.AddSeconds(240); // 4 minutes
                            // Update duet screen if visible
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Swan Lake Duet has expired and start cooldown
                        if (gameState.SwanLakeDuetActive && DateTime.Now > gameState.SwanLakeDuetExpiry)
                        {
                            gameState.SwanLakeDuetActive = false;
                            gameState.SwanLakeDuetClickCounter = 0; // Reset click counter
                            gameState.SwanLakeDuetCooldownExpiry = DateTime.Now.AddSeconds(240); // 4 minutes
                            // Update duet screen if visible
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if La Campanella Duet has expired
                        if (gameState.LaCampanellaDuetActive && DateTime.Now > gameState.LaCampanellaDuetExpiry)
                        {
                            gameState.LaCampanellaDuetActive = false;
                            gameState.ChimeChainLength = 0;
                            gameState.LastChimeClickTime = DateTime.MinValue;
                            gameState.LaCampanellaDuetCooldownExpiry = DateTime.Now.AddSeconds(600); // 10 minutes
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Enigma Duet has expired
                        if (gameState.EnigmaDuetActive && DateTime.Now > gameState.EnigmaDuetExpiry)
                        {
                            gameState.EnigmaDuetActive = false;
                            gameState.EnigmaMysteryClickCount = 0;
                            gameState.EnigmaDuetCooldownExpiry = DateTime.Now.AddSeconds(1800); // 30 minutes
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Fate Duet has expired
                        if (gameState.FateDuetActive && DateTime.Now > gameState.FateDuetExpiry)
                        {
                            gameState.FateDuetActive = false;
                            gameState.FateDuetHasFlipped = false;
                            gameState.FateDuetClickCount = 0;
                            gameState.HourglassActionBank.Clear();
                            gameState.FateDuetCooldownExpiry = DateTime.Now.AddSeconds(480); // 8 minutes
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Ode Duet has expired
                        if (gameState.OdeDuetActive && DateTime.Now > gameState.OdeDuetExpiry)
                        {
                            // Grant entropic melody based on completed sections
                            int entropicReward = gameState.CrescendoCompletedSections * 3;
                            gameState.EntropicMelodies += entropicReward;
                            
                            gameState.OdeDuetActive = false;
                            gameState.CrescendoNotesPlaced = 0;
                            gameState.CrescendoCompletedSections = 0;
                            gameState.Crescendo4Claimed = false;
                            gameState.Crescendo8Claimed = false;
                            gameState.Crescendo12Claimed = false;
                            gameState.Crescendo16Claimed = false;
                            gameState.OdeDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.OdeToJoy);
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Swan Lake Duet: No background processing needed (all handled in click handler)

                        // Process Fate Duet hourglass flip and replay
                        if (gameState.FateDuetActive)
                        {
                            MusicClicker.Armory.WeaponAbilities.FateDuet_ProcessHourglass(gameState);
                        }

                        // Apply Moonlight Duet Full Moon phase (3x NPS)
                        int moonlightPhase = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_GetCurrentPhase(gameState);
                        bool allMoonPhasesActive = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_AreAllPhasesActive(gameState);
                        
                        if (allMoonPhasesActive || moonlightPhase == 2) // Full Moon or all phases active
                        {
                            effectiveNps *= 3.0;
                        }

                        // Apply Astral Chainripper 5x NPS boost
                        if (gameState.AstralChainripperNpsBoostActive)
                        {
                            effectiveNps *= 5.0;
                        }

                        // Apply Blizzard's Bounty NPS bonus (stacking +2% per harmonious purchase)
                        if (gameState.BlizzardBountyNpsBonus > 0)
                        {
                            effectiveNps *= (1.0 + gameState.BlizzardBountyNpsBonus);
                        }

                        // Apply Ode to Joy Duet 5x NPS boost from completing 16-note crescendo
                        if (gameState.OdeDuetNpsBoostActive && DateTime.Now <= gameState.OdeDuetNpsBoostExpiry)
                        {
                            effectiveNps *= 5.0;
                        }
                        else if (gameState.OdeDuetNpsBoostActive && DateTime.Now > gameState.OdeDuetNpsBoostExpiry)
                        {
                            gameState.OdeDuetNpsBoostActive = false;
                        }

                        // Apply Joyful Catharsis passive double NPS
                        if (gameState.JoyfulCatharsis && 
                            (gameState.CurrentResonatedWeapon1 == "JoyfulCatharsis" || gameState.CurrentResonatedWeapon2 == "JoyfulCatharsis"))
                        {
                            effectiveNps *= 2.0;
                        }

                        // Apply duet resonance NPS multipliers
                        if (gameState.CurrentResonatedWeapon1 != "None" && gameState.CurrentResonatedWeapon2 != "None")
                        {
                            // Eroica Duet: Double NPS if any minor score > 10
                            if ((gameState.CurrentResonatedWeapon1 == "SakurasBlossom" && gameState.CurrentResonatedWeapon2 == "FuneralPrayer") ||
                                (gameState.CurrentResonatedWeapon1 == "FuneralPrayer" && gameState.CurrentResonatedWeapon2 == "SakurasBlossom"))
                            {
                                effectiveNps *= MusicClicker.Armory.WeaponAbilities.EroicaDuet_GetNpsMultiplier(gameState);
                            }

                            // Swan Lake Duet: No passive NPS multiplier (has Feather Cascade active ability instead)

                            // Ode to Joy Duet: NPS becomes NPS per half-second (double the rate)
                            if ((gameState.CurrentResonatedWeapon1 == "JoyfulCatharsis" && gameState.CurrentResonatedWeapon2 == "OdeToCreation") ||
                                (gameState.CurrentResonatedWeapon1 == "OdeToCreation" && gameState.CurrentResonatedWeapon2 == "JoyfulCatharsis"))
                            {
                                effectiveNps *= 2.0; // Double the application rate
                            }
                        }

                        // Winter: If NPS is frozen, STOP accumulating notes from NPS entirely
                        // The frozen value is only used as a click multiplier, not for passive accumulation
                        if (gameState.NpsFrozen && DateTime.Now <= gameState.NpsFreezeExpiry)
                        {
                            // Do nothing - skip the note accumulation below
                        }
                        else
                        {
                            // Advance notes by elapsedSeconds * effectiveNPS using lock-free atomic add
                            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, effectiveNps * elapsed);
                        }
                    }
                }
                catch { }
            };
            _backgroundNpsTimer.Start();

            // Create and start the auto-save DispatcherTimer (fires every 30 seconds)
            _saveTimer = new DispatcherTimer();
            _saveTimer.Interval = TimeSpan.FromMilliseconds(30000);
            _saveTimer.Tick += (s, e) =>
            {
                // Automatically save the game to prevent progress loss
                SaveGame();
            };
            _saveTimer.Start();

            // Register handler to save when window closes
            this.Closing += MainWindow_Closing;

            // Perform initial UI update to display loaded/new game state
            Dispatcher.UIThread.Post(() =>
            {
                UIUpdater.UpdateUI(this, gameState);
                UIUpdater.UpdateFragmentationUI(this, gameState);
                UIUpdater.UpdateSaveScoresUI(this, gameState);
                UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            });

        }

        // ------------------- SAVE/LOAD METHODS -------------------
        
        /// <summary>
        /// Saves the current game state to disk.
        /// </summary>
        private void SaveGame()
        {
            // Use background save to avoid blocking UI thread during disk writes.
            try
            {
                SaveManager.SaveBackground(gameState);
                Console.WriteLine("Background save scheduled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save scheduling failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Event handler called when the window is closing.
        /// Ensures game is saved and timers are stopped before shutdown.
        /// </summary>
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save game one final time before closing
            SaveGame();
            
            // Stop timers to prevent them from firing after window closes
            _gameLoopTimer?.Stop();
            _saveTimer?.Stop();
            _backgroundNpsTimer?.Stop();
            _backgroundNpsTimer?.Dispose();
        }

        /// <summary>
        /// Performs a fullscreen black fade-out, runs the provided action (switch screens), then fades back in.
        /// Duration is seconds for each half of the transition.
        /// </summary>
        public async Task TransitionAsync(Action switchAction, double durationSeconds = 0.18)
        {
            // Immediate transition: perform the switch action without a fade to avoid expensive
            // fullscreen animations which were causing performance problems on some systems.
            try { switchAction?.Invoke(); } catch { }
            await Task.CompletedTask;
        }

        private async Task FadeToAsync(Rectangle fader, double targetOpacity, double durationSeconds)
        {
            // Transition animations have been disabled for performance; set final opacity immediately.
            try { await Dispatcher.UIThread.InvokeAsync(() => fader.Opacity = targetOpacity); } catch { }
        }

        // ------------------- CAROUSEL INITIALIZATION -------------------
        
        /// <summary>
        /// Sets up the carousel system with all 8 navigation buttons.
        /// Initializes transforms, drag handlers, and animation timer.
        /// </summary>
        private void InitializeCarousel()
        {
            // Create list of all carousel buttons with their transform components
            // Each button needs a TranslateTransform (for position) and ScaleTransform (for size)
            carouselButtons = new List<(Button, TranslateTransform, ScaleTransform)>
            {
                GetButtonTransforms(SymphonicGalleryButton),   // Save the Scores (now first)
                GetButtonTransforms(FragmentationButton),      // Upgrade screen
                GetButtonTransforms(ResonanceButton),          // Resonance feature
                GetButtonTransforms(MelodyButton),             // Melody feature
                GetButtonTransforms(HarmonyButton),            // Harmony feature
                GetButtonTransforms(TempoResonateButton),      // Tempo Resonate (scores)
                GetButtonTransforms(EternalModulationButton),  // Event screen
                GetButtonTransforms(ArmoryOfForteButton)       // Armory/weapons shop
            };

            // Get the canvas that contains the carousel
            var canvas = this.FindControl<Canvas>("CarouselCanvas");
            if (canvas != null)
            {
                // Register mouse/touch event handlers for drag functionality
                canvas.PointerPressed += CarouselCanvas_PointerPressed;   // Drag start
                canvas.PointerMoved += CarouselCanvas_PointerMoved;       // Drag move
                canvas.PointerReleased += CarouselCanvas_PointerReleased; // Drag end
            }

            // Create animation timer that fires at the target _frameRate (adaptive)
            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / _frameRate)
            };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            // Initialize to ensure button 0 (Fragmentation) is centered at start
            currentIndex = 0;
            currentRotation = 0;
            targetRotation = 0;
            
            // Calculate and apply initial positions for all buttons
            UpdateCarouselPositions();

            // Ensure animation tick also performs small UI animation updates for smooth visuals
            // (AnimateVisuals will be called at FRAME_RATE from AnimationTimer_Tick)
        }

        /// <summary>
        /// Gets or creates the transform components for a carousel button.
        /// Each button needs a TranslateTransform for position and ScaleTransform for size.
        /// </summary>
        private (Button, TranslateTransform, ScaleTransform) GetButtonTransforms(Button button)
        {
            // Try to get existing transforms
            var transformGroup = button.RenderTransform as TransformGroup;
            if (transformGroup != null && transformGroup.Children.Count >= 2)
            {
                var translate = transformGroup.Children[0] as TranslateTransform;
                var scale = transformGroup.Children[1] as ScaleTransform;
                
                // If valid transforms exist, return them
                if (translate != null && scale != null)
                {
                    return (button, translate, scale);
                }
            }
            
            // No valid transforms found - create new ones
            var newTranslate = new TranslateTransform();
            var newScale = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
            var newTransformGroup = new TransformGroup();
            newTransformGroup.Children.Add(newTranslate);
            newTransformGroup.Children.Add(newScale);
            button.RenderTransform = newTransformGroup;
            
            return (button, newTranslate, newScale);
        }

        // ------------------- DRAG HANDLERS -------------------
        
        /// <summary>
        /// Called when user presses mouse/touch on the carousel canvas.
        /// Initiates drag mode and records starting position.
        /// </summary>
        private void CarouselCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            isDragging = true;                              // Enable drag mode
            lastDragPoint = e.GetPosition(sender as Control); // Record starting position
            dragVelocity = 0;                               // Reset velocity
            dragMomentum = 0;                               // Reset momentum
            isAnimating = false;                            // Stop any ongoing animation
        }

        /// <summary>
        /// Called when user moves mouse/touch while dragging.
        /// Rotates the carousel based on vertical drag distance.
        /// </summary>
        private void CarouselCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!isDragging) return; // Only process if actively dragging

            // Get current pointer position
            var currentPoint = e.GetPosition(sender as Control);
            
            // Calculate vertical distance moved since last update
            double deltaY = currentPoint.Y - lastDragPoint.Y;
            
            // Convert vertical movement to rotation (0.25 is sensitivity multiplier)
            // Invert the rotation direction
            double rotationDelta = deltaY * 0.25;
            currentRotation += rotationDelta;
            
            // Store velocity for momentum calculation when drag ends
            dragVelocity = rotationDelta;
            
            // Update last position for next frame
            lastDragPoint = currentPoint;
        }

        /// <summary>
        /// Called when user releases mouse/touch after dragging.
        /// Applies momentum and initiates snap-to-nearest animation.
        /// </summary>
        private void CarouselCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!isDragging) return; // Only process if was dragging
            
            isDragging = false; // Exit drag mode
            
            // Apply momentum based on final drag velocity (2.0 is momentum multiplier)
            dragMomentum = dragVelocity * 2.0;
            
            // If momentum is very low, immediately snap to nearest button
            if (Math.Abs(dragMomentum) < 1.0)
            {
                SnapToNearest();
            }
        }

        /// <summary>
        /// Snaps the carousel to the nearest button position.
        /// Called after drag ends or when momentum decays to near-zero.
        /// </summary>
        private void SnapToNearest()
        {
            // Calculate angle between each button (360° / 8 buttons = 45°)
            double angleStep = 360.0 / BUTTON_COUNT;
            
            // Find which button is closest to current rotation
            int nearestIndex = (int)Math.Round(currentRotation / angleStep) % BUTTON_COUNT;
            if (nearestIndex < 0) nearestIndex += BUTTON_COUNT; // Handle negative wrap-around
            
            // Set target to snap to nearest button - ensure exactly one button is centered
            currentIndex = nearestIndex;
            targetRotation = nearestIndex * angleStep;
            isAnimating = true; // Begin smooth animation to target
        }

        // ---- Interaction event handlers to suppress heavy UI updates while user scrolls/drags ----

        private void MainWindow_PointerWheelChanged(object? sender, Avalonia.Input.PointerWheelEventArgs e)
        {
            MarkUserInteraction();
        }

        private void MainWindow_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            MarkUserInteraction();
        }

        private void MainWindow_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            // Mark interaction so the debounce timer restarts; the timer will clear the flag.
            MarkUserInteraction();
        }

        private void MarkUserInteraction()
        {
            IsUserInteracting = true;
            try
            {
                // If timer already running, increase interval slightly (exponential backoff up to 1000ms)
                if (_interactionTimer.IsEnabled)
                {
                    int cur = (int)_interactionTimer.Interval.TotalMilliseconds;
                    int next = Math.Min(1000, cur + 100);
                    _interactionTimer.Interval = TimeSpan.FromMilliseconds(next);
                }
                else
                {
                    _interactionTimer.Interval = TimeSpan.FromMilliseconds(400);
                }

                _interactionTimer.Stop();
                _interactionTimer.Start();
            }
            catch { }
        }

        // ------------------- CAROUSEL METHODS -------------------
        
        /// <summary>
        /// Rotates the carousel by one position in the specified direction.
        /// </summary>
        /// <param name="direction">1 for clockwise, -1 for counter-clockwise</param>
        private void RotateCarousel(int direction)
        {
            if (isAnimating) return; // Don't interrupt ongoing animation

            // Update current index with wrap-around (0-7)
            currentIndex = (currentIndex + direction + BUTTON_COUNT) % BUTTON_COUNT;
            
            // Calculate target rotation angle
            double angleStep = 360.0 / BUTTON_COUNT;
            targetRotation = currentIndex * angleStep;
            
            isAnimating = true; // Begin smooth animation to new position
        }

        /// <summary>
        /// Animation tick handler called at the configured `FRAME_RATE` (e.g., 120 times per second).
        /// Handles momentum physics and smooth interpolation to target position.
        /// </summary>
        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            // Handle momentum after drag release
            if (!isDragging && Math.Abs(dragMomentum) > 0.1)
            {
                // Apply momentum to rotation
                currentRotation += dragMomentum;
                
                // Decay momentum (0.92 = 92% remaining each frame, creates friction effect)
                dragMomentum *= 0.92;
                
                // When momentum is very low, snap to nearest button
                if (Math.Abs(dragMomentum) < 0.5)
                {
                    dragMomentum = 0;
                    SnapToNearest();
                }
            }
            // Handle smooth animation to target position
            else if (isAnimating)
            {
                // Calculate angle difference to target
                double diff = targetRotation - currentRotation;
                
                // Handle wrap-around for shortest path (e.g., 350° to 10° should go forward, not backward 340°)
                if (diff > 180) diff -= 360;
                if (diff < -180) diff += 360;
                
                // Move 15% of remaining distance each frame (creates ease-out effect)
                currentRotation += diff * 0.15;
                
                // When very close to target, snap exactly and stop animating
                if (Math.Abs(diff) < 0.5)
                {
                    currentRotation = targetRotation;
                    isAnimating = false;
                }
            }
            
            // Update visual positions of all buttons based on current rotation
            UpdateCarouselPositions();

            // Run lightweight visual smoothing for key UI elements (notes counter, NPS, etc.)
            _animTickStopwatch.Restart();
            try
            {
                UIUpdater.AnimateVisuals(this, gameState, 1.0 / _frameRate);
            }
            catch { }
            _animTickStopwatch.Stop();

            // Update moving average of tick duration and adapt _frameRate downward if ticks are
            // taking too long. This helps lower-end machines avoid overload and reduce jitter.
            double ms = _animTickStopwatch.Elapsed.TotalMilliseconds;
            _animSamples++;
            if (_animSamples > 120) _animSamples = 120; // keep sample window bounded
            _animAverageMs = (_animAverageMs * (_animSamples - 1) + ms) / _animSamples;

            // If average tick time exceeds expected interval by a factor, reduce frame rate.
            double expectedMs = 1000.0 / _frameRate;
            if (_animAverageMs > expectedMs * 1.8 && _frameRate > 30)
            {
                // reduce frame rate to relieve CPU, but not lower than 30
                _frameRate = Math.Max(30, _frameRate / 2);
                animationTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / _frameRate);
            }
        }

        /// <summary>
        /// Calculates and applies 3D carousel positions for all buttons.
        /// Creates illusion of depth using scale, opacity, and positioning.
        /// </summary>
        private void UpdateCarouselPositions()
        {
            double angleStep = 360.0 / BUTTON_COUNT;    // Angle between buttons (45°)
            double horizontalOffset = 15;                // Slight offset for visual balance

            // Update each button's position and appearance
            for (int i = 0; i < carouselButtons.Count; i++)
            {
                var (button, translate, scale) = carouselButtons[i];

                // Calculate this button's angle relative to current rotation
                // No offset needed - let button 0 be at angle 0, which with -cos gives max Y (bottom center)
                double angle = (i * angleStep - currentRotation) * (Math.PI / 180.0);

                // Calculate vertical position on carousel circle (cosine gives vertical component)
                // Use +cos so that angle=0 gives y=+RADIUS (bottom center)
                double y = Math.Cos(angle) * RADIUS;
                
                // Check if button is at the bottom (foreground) of carousel
                bool isAtBottom = y > (RADIUS - 50);

                // Calculate opacity: buttons fade out as they move to back
                // Higher y value = closer to front = more opacity
                double opacity = Math.Max(0, (y + 100) / (RADIUS + 100));

                // Calculate scale: buttons at back are smaller (depth effect)
                // t is normalized position from back (0) to front (1)
                double t = (RADIUS - y) / (2 * RADIUS);
                double scaleValue = 0.9 - 0.4 * t;  // Range from 0.5 (back) to 0.9 (front)

                // Calculate horizontal offset (sine gives horizontal component)
                double centerOffset = Math.Sin(angle) * RADIUS;

                // Apply spacing multiplier to spread buttons more when at sides
                // Use squared cosine via multiplication to avoid Math.Pow overhead
                double cos = Math.Cos(angle);
                double spacingMultiplier = 1.0 + 1.2 * (cos * cos);
                double x = centerOffset * spacingMultiplier + horizontalOffset;

                // Apply calculated transforms
                translate.X = x;                    // Horizontal position
                translate.Y = y;                    // Vertical position
                scale.ScaleX = scaleValue;          // Width scaling
                scale.ScaleY = scaleValue;          // Height scaling
                button.Opacity = opacity;           // Transparency
                
                // Only allow clicks on front button (high opacity, at bottom)
                button.IsHitTestVisible = isAtBottom && opacity > 0.8;
                
                // Set z-index so closer buttons render on top
                button.ZIndex = isAtBottom ? 50 : (int)(-y);
            }
        }

        // ------------------- EXISTING METHODS -------------------
        
        /// <summary>
        /// Track mouse position over click button
        /// </summary>
        private void ClickButton_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (FloatingTextCanvas != null)
            {
                _lastClickPosition = e.GetPosition(FloatingTextCanvas);
            }
        }
        
        /// <summary>
        /// Handler for the main clicker button click.
        /// Adds notes based on notes-per-click and applies any active abilities.
        /// </summary>
        public void ClickButton_Click(object? sender, RoutedEventArgs e)
        {
            // Cache nighttime check to avoid multiple DateTime.Now.Hour calls (performance optimization)
            bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
            
            // Start with base notes per click value
            double notesPerClick = gameState.NotesPerClick;

            // Cache Moonlight Duet phase checks (use read-only to avoid modifying state on every click)
            int moonPhase = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_GetCurrentPhaseReadOnly(gameState);
            bool allMoonPhasesActive = (moonPhase != -1) && MusicClicker.Armory.WeaponAbilities.MoonlightDuet_AreAllPhasesActive(gameState);
            
            // Apply Moonlight Duet New Moon phase (2x NPC)
            if (allMoonPhasesActive || moonPhase == 0) // New Moon or all phases active
            {
                notesPerClick *= 2.0;
            }

            // Check if Eroica Symphonic Catharsis buff has expired
            if (gameState.SymphonicCatharsisActive && DateTime.Now > gameState.SymphonicCatharsisExpiry)
            {
                gameState.SymphonicCatharsisActive = false;
            }

            // Eroica Symphonic Catharsis: Double NPC for 10s
            if (gameState.SymphonicCatharsisActive)
            {
                notesPerClick *= 2.0;
            }

            // Check Ode to Creation bonus first (before applying notes)
            if (gameState.OdeToCreationNextClickBonus)
            {
                notesPerClick *= 1.33; // 33% increase
                gameState.OdeToCreationNextClickBonus = false;
            }

            // Apply Winter Duet multiplier: Frozen NPS is used as a click multiplier
            if (gameState.CurrentResonatedWeapon1 != "None" && gameState.CurrentResonatedWeapon2 != "None")
            {
                if ((gameState.CurrentResonatedWeapon1 == "CacophonicBlizzard" && gameState.CurrentResonatedWeapon2 == "TheSnowsDesire") ||
                    (gameState.CurrentResonatedWeapon1 == "TheSnowsDesire" && gameState.CurrentResonatedWeapon2 == "CacophonicBlizzard"))
                {
                    double winterMultiplier = MusicClicker.Armory.WeaponAbilities.WinterDuet_GetClickMultiplier(gameState);
                    notesPerClick *= winterMultiplier;
                }
            }

            // Apply Moonlight Major ability: adds notes-per-second to each click
            if (gameState.MoonlightMajorAbility)
            {
                notesPerClick += gameState.NotesPerSecond;
            }
            
            // Apply Incisor of Moonlight: +500% notes during nighttime (8PM-6AM)
            if (gameState.IncisorOfMoonlightAbility && isNighttime)
            {
                notesPerClick *= 6.0; // +500% = 6x multiplier
            }
            
            // Apply Fate Major ability: every 5th click gives 30% bonus of total notes
            else if (gameState.FateMajorAbility)
            {
                gameState.FateCounter++;
                if (gameState.FateCounter == 5)
                {
                    gameState.FateCounter = 0;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, (gameState.Notes * 0.30));
                }
            }

            // Funeral Prayer: Add 6x NPS bonus to notesPerClick if empowered
            if (gameState.FuneralPrayerAbility)
            {
                double empoweredBonus = MusicClicker.Armory.WeaponAbilities.FuneralPrayer_GetEmpoweredClickBonus(gameState);
                if (empoweredBonus > 0)
                {
                    notesPerClick += empoweredBonus;
                }
            }

            // Add calculated notes to player's total
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, notesPerClick);

            // Funeral Prayer: Track clicks (after getting multiplier, empowered clicks don't count)
            if (gameState.FuneralPrayerAbility)
            {
                MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnClick(gameState);
            }

            // Swan Lake Duet: Feather Cascade - grant feathers and note bonuses
            if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.SwanLakeDuet_OnClick(gameState);
            }

            // La Campanella Duet: Chime Chain click tracking
            if (gameState.LaCampanellaDuetActive && DateTime.Now <= gameState.LaCampanellaDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.LaCampanellaDuet_OnClick(gameState);
            }

            // Enigma Duet: Cipher Wheel segment rewards
            if (gameState.EnigmaDuetActive && DateTime.Now <= gameState.EnigmaDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.EnigmaDuet_OnClick(gameState);
            }

            // Fate Duet: Bank click during banking phase
            if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "Click", notesPerClick);
            }

            // Ode to Joy Duet: Add note to crescendo
            if (gameState.OdeDuetActive && DateTime.Now <= gameState.OdeDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.OdeDuet_AddNote(gameState);
            }

            // Individual weapon click abilities
            if (gameState.OdeToCreation && 
                (gameState.CurrentResonatedWeapon1 == "OdeToCreation" || gameState.CurrentResonatedWeapon2 == "OdeToCreation"))
            {
                MusicClicker.Armory.WeaponAbilities.OdeToCreation_OnClick(gameState);
            }

            // Winter: Cacophonic Blizzard - every 50th click freezes NPS for 8s
            if (gameState.CacophonicBlizzard &&
                (gameState.CurrentResonatedWeapon1 == "CacophonicBlizzard" || gameState.CurrentResonatedWeapon2 == "CacophonicBlizzard"))
            {
                MusicClicker.Armory.WeaponAbilities.CacophonicBlizzard_OnClick(gameState);
            }

            // Swan Lake Crescendance: Feather collection on clicks
            if (gameState.CurrentResonatedScore == "Swan")
            {
                MusicClicker.Armory.WeaponAbilities.SwanLakeCrescendance_OnClick(gameState);
            }
            
            // Moonlight Sonata Crescendance: Moonbeam Resonance stacks (every 20th click at night)
            if (gameState.CurrentResonatedScore == "Moonlight" && isNighttime)
            {
                MusicClicker.Armory.WeaponAbilities.MoonlightCrescendance_OnClick(gameState, this);
            }
            
            // La Campanella Crescendance: Grandiose Bell click tracking
            if (gameState.CurrentResonatedScore == "LaCampanella")
            {
                MusicClicker.Armory.WeaponAbilities.LaCampanellaCrescendance_OnClick(gameState, this);
            }
            
            // Enigma Crescendance: Resonate Mystery stacks (every 10th click, +25th when active)
            if (gameState.CurrentResonatedScore == "Enigma")
            {
                MusicClicker.Armory.WeaponAbilities.EnigmaCrescendance_OnClick(gameState, this);
            }
            
            // Fate Crescendance: Cosmic Modulation stacks (every 5th click + 10% notes)
            if (gameState.CurrentResonatedScore == "Fate")
            {
                MusicClicker.Armory.WeaponAbilities.FateCrescendance_OnClick(gameState, this);
            }
            
            // Ode to Joy Crescendance: Petal tracking
            if (gameState.CurrentResonatedScore == "OdeToJoy")
            {
                MusicClicker.Armory.WeaponAbilities.OdeToJoyCrescendance_OnClick(gameState, this);
            }
            
            // Dies Irae Crescendance: Burning Hatred stacks
            if (gameState.CurrentResonatedScore == "DiesIrae")
            {
                MusicClicker.Armory.WeaponAbilities.DiesIraeCrescendance_OnClick(gameState, this);
            }

            // Swan Lake Weapon: Star-Scattered Wings passive (every 10th click grants fragments)
            if ((gameState.CurrentResonatedWeapon1 == "StarScatteredWings" || gameState.CurrentResonatedWeapon2 == "StarScatteredWings"))
            {
                MusicClicker.Armory.WeaponAbilities.StarScatteredWings_OnClick(gameState);
            }

            // Check for duet resonance effects on click
            if (gameState.CurrentResonatedWeapon1 != "None" && gameState.CurrentResonatedWeapon2 != "None")
            {
                // Fate duet now uses active ability system (Hourglass Fracture)
                // Old passive removed

                // Winter Duet: Absolute Zero duration extension on click
                if ((gameState.CurrentResonatedWeapon1 == "CacophonicBlizzard" && gameState.CurrentResonatedWeapon2 == "TheSnowsDesire") ||
                    (gameState.CurrentResonatedWeapon1 == "TheSnowsDesire" && gameState.CurrentResonatedWeapon2 == "CacophonicBlizzard"))
                {
                    MusicClicker.Armory.WeaponAbilities.WinterDuet_OnClick(gameState);
                }

                // Hell's Wrath: Damnation's Gift (7% chance for random minor component)
                if (gameState.HellsWrath)
                {
                    MusicClicker.Armory.WeaponAbilities.HellsWrath_OnClick(gameState);
                }

                // Moonlight Duet: Crescent phase component drop (10% chance)
                // Process component drop when in Crescent phase OR when all phases are active
                if (moonPhase == 1 || allMoonPhasesActive)
                {
                    MusicClicker.Armory.WeaponAbilities.MoonlightDuet_CrescentComponentDrop(gameState);
                }

                // Dies Irae Duet: Seven Seals click counter
                if ((gameState.CurrentResonatedWeapon1 == "SevenCircles" && gameState.CurrentResonatedWeapon2 == "HellsWrath") ||
                    (gameState.CurrentResonatedWeapon1 == "HellsWrath" && gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    MusicClicker.Armory.WeaponAbilities.DiesIraeDuet_OnClick(gameState, this);
                }

                // Eroica Duet: Victory March progress
                if ((gameState.CurrentResonatedWeapon1 == "SakurasBlossom" && gameState.CurrentResonatedWeapon2 == "FuneralPrayer") ||
                    (gameState.CurrentResonatedWeapon1 == "FuneralPrayer" && gameState.CurrentResonatedWeapon2 == "SakurasBlossom"))
                {
                    MusicClicker.Armory.WeaponAbilities.EroicaDuet_OnClick(gameState, this);
                }
            }

            // Show floating text at actual cursor position with final calculated notes per click
            if (FloatingTextCanvas != null)
            {
                // Determine critical hit type
                double roll = _random.NextDouble() * 100; // 0-100
                
                // Apply Eroica Symphonic Catharsis: +10% critical chance
                if (gameState.SymphonicCatharsisActive)
                {
                    roll -= 10.0;
                }

                // Apply Eulogy of the Moon: +5% critical rate during nighttime (8PM-6AM)
                // Shift the roll down by 5 to effectively increase crit chance
                if (gameState.EulogyOfTheMoonAbility && isNighttime)
                {
                    roll -= 5.0; // This increases the chance of hitting lower (crit) thresholds
                }
                
                string critText;
                Color critColor;
                double finalNotes = notesPerClick;
                bool hasStroke = false;
                
                // Crescendance Bond - Sakura's Blossom: Crimson Requiem (priority check)
                if (gameState.CrimsonRequiemClicksRemaining > 0)
                {
                    gameState.CrimsonRequiemClicksRemaining--;
                    finalNotes = notesPerClick + (gameState.NotesPerSecond * gameState.NotesPerClick);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Crimson Requiem!!! +{FormatNumber(finalNotes)}";
                    critColor = Colors.Black; // Deep black text with crimson outline
                    hasStroke = true;
                }
                else if (roll < 0.1) // 0.1% chance - Entropic Crescendo
                {
                    finalNotes = notesPerClick * 1500;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Entropic Crescendo of Eternity!!! +{FormatNumber(finalNotes)}";
                    critColor = Colors.Red;
                    hasStroke = true;
                }
                else if (roll < 1.1) // 1% chance - Superior Crescendo
                {
                    finalNotes = notesPerClick * 5;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Superior Crescendo!!! +{FormatNumber(finalNotes)}";
                    critColor = Color.FromRgb(255, 20, 147); // Deep pink
                }
                else if (roll < 6.1) // 5% chance - Critical Crescendo
                {
                    finalNotes = notesPerClick * 2;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Critical Crescendo!! +{FormatNumber(finalNotes)}";
                    critColor = Color.FromRgb(255, 182, 193); // Light pink
                }
                else // ~94% chance - Normal click
                {
                    critText = $"+{FormatNumber(notesPerClick)} Notes";
                    critColor = Colors.White;
                }
                
                ShowFloatingText(_lastClickPosition, critText, critColor, hasStroke);
            }
            
            // Update UI after click
            UIUpdater.UpdateUI(this, gameState);
            UIUpdater.UpdateFragmentationUI(this, gameState);
            UIUpdater.UpdateSaveScoresUI(this, gameState);
            UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
            UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            GlobalTempoManager?.RefreshDrawer();
        }

        /// <summary>
        /// Handler for back button on upgrade screen.
        /// Returns to main game screen.
        /// </summary>
        public async void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            await TransitionAsync(() =>
            {
                UpgradeScreen.IsVisible = false;
                MainScreen.IsVisible = true;
            });
            
            UIUpdater.UpdateUI(this, gameState);
            GlobalTempoManager?.RefreshDrawer();
        }

        /// <summary>
        /// Handler for keyboard input - Space key gives debug resources, M key gives Moonlight components.
        /// Used for testing/debugging during development.
        /// </summary>
        public void MainWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Space)
            {
                // Give large amount of notes for testing and grant majors
                // MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, 1_000_000);
                // MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notesPerSecond, 1_000_000_000_000);

                // Give one of each major score type
                gameState.MoonlightMajorOwned += 1;
                gameState.EroicaMajorOwned += 1;
                gameState.SwanMajorOwned += 1;
                gameState.LaCampanellaMajorOwned += 1;
                gameState.EnigmaMajorOwned += 1;
                gameState.FateMajorOwned += 1;
                gameState.OdeToJoyMajorOwned += 1;
                gameState.DiesIraeOwned += 1;
                gameState.WinterOwned += 1;

                // Give one of each weapon
                gameState.IncisorOfMoonlight = true;
                gameState.EulogyOfTheMoon = true;
                gameState.SakurasBlossom = true;
                gameState.FuneralPrayer = true;
                gameState.StarScatteredWings = true;
                gameState.ThousandWingedSwan = true;
                gameState.SymphonyOfBells = true;
                gameState.RazerOfBellsChimes = true;
                gameState.CreatorOfMystery = true;
                gameState.Truthseeker = true;
                gameState.AstralChainripper = true;
                gameState.CosmicWeaver = true;
                gameState.JoyfulCatharsis = true;
                gameState.OdeToCreation = true;
                gameState.SevenCircles = true;
                gameState.HellsWrath = true;
                gameState.CacophonicBlizzard = true;
                gameState.TheSnowsDesire = true;

                // Update all UI displays
                UIUpdater.UpdateUI(this, gameState);
                UIUpdater.UpdateFragmentationUI(this, gameState);
                UIUpdater.UpdateSaveScoresUI(this, gameState);
                UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            }
            else if (e.Key == Avalonia.Input.Key.M)
            {
                // Give Moonlight components for testing Sakura's Blossom
                gameState.MoonlightMinorKeys += 1;
                gameState.MoonlightMinorScales += 1;
                gameState.MoonlightMinorProgressions += 1;
                gameState.MoonlightMajorKeys += 1;
                gameState.MoonlightMajorScales += 1;
                gameState.MoonlightMajorProgressions += 1;
                gameState.MelodiousOwned += 10; // Minor needs 10 Melodious
                gameState.HarmoniousOwned += 10; // Major needs 10 Harmonious

                // Update UI
                UIUpdater.UpdateUI(this, gameState);
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            }
        }

        /// <summary>
        /// Handler for Tempo Resonate button click.
        /// Navigates to the Tempo Resonate (musical scores) screen.
        /// </summary>
        public async void TempoResonateButton_Click(object? sender, RoutedEventArgs e)
        {
            await TransitionAsync(() =>
            {
                MainScreen.IsVisible = false;
                TempoResonateScreen.IsVisible = true;
            });
            
            // Refresh weapon and score drawers to reflect current ownership
            GlobalTempoManager?.RefreshDrawer();
        }

        /// <summary>
        /// Handler for back button on Tempo Resonate screen.
        /// Returns to main game screen.
        /// </summary>
        public async void BackButtonTempoResonate_Click(object? sender, RoutedEventArgs e)
        {
            await TransitionAsync(() =>
            {
                TempoResonateScreen.IsVisible = false;
                MainScreen.IsVisible = true;
            });
            
            // Update main screen UI and duet button visibility when returning
            UIUpdater.UpdateUI(this, gameState);
            UpdateDuetAbilityButtonVisibility();
            GlobalTempoManager?.RefreshDrawer();
        }

        /// <summary>
        /// Updates the visibility of the Duet Ability button based on currently equipped weapons.
        /// Only shows the button if a duet with cooldown abilities is equipped.
        /// </summary>
        public void UpdateDuetAbilityButtonVisibility()
        {
            if (DuetAbilityButtonBorder == null || gameState == null) return;

            string weapon1 = gameState.CurrentResonatedWeapon1;
            string weapon2 = gameState.CurrentResonatedWeapon2;
            bool hasCooldownDuet = false;

            // Check for Moonlight Duet (Lunar Phases)
            if ((weapon1 == "IncisorOfMoonlight" && weapon2 == "EulogyOfTheMoon") ||
                (weapon1 == "EulogyOfTheMoon" && weapon2 == "IncisorOfMoonlight"))
            {
                hasCooldownDuet = true;
            }
            // Check for Dies Irae Duet (Seven Seals)
            else if ((weapon1 == "SevenCircles" && weapon2 == "HellsWrath") ||
                (weapon1 == "HellsWrath" && weapon2 == "SevenCircles"))
            {
                hasCooldownDuet = true;
            }
            // Check for Winter Duet (Absolute Zero)
            else if ((weapon1 == "CacophonicBlizzard" && weapon2 == "TheSnowsDesire") ||
                     (weapon1 == "TheSnowsDesire" && weapon2 == "CacophonicBlizzard"))
            {
                hasCooldownDuet = true;
            }
            // Check for Eroica Duet (Victory March)
            else if ((weapon1 == "SakurasBlossom" && weapon2 == "FuneralPrayer") ||
                     (weapon1 == "FuneralPrayer" && weapon2 == "SakurasBlossom"))
            {
                hasCooldownDuet = true;
            }
            // Check for Swan Lake Duet (Feather Cascade)
            else if ((weapon1 == "StarScatteredWings" && weapon2 == "ThousandWingedSwan") ||
                     (weapon1 == "ThousandWingedSwan" && weapon2 == "StarScatteredWings"))
            {
                hasCooldownDuet = true;
            }
            // Check for La Campanella Duet (Chime Chain)
            else if ((weapon1 == "SymphonyOfBells" && weapon2 == "RazerOfBellsChimes") ||
                     (weapon1 == "RazerOfBellsChimes" && weapon2 == "SymphonyOfBells"))
            {
                hasCooldownDuet = true;
            }
            // Check for Enigma Duet (Cipher Wheel)
            else if ((weapon1 == "CreatorOfMystery" && weapon2 == "Truthseeker") ||
                     (weapon1 == "Truthseeker" && weapon2 == "CreatorOfMystery"))
            {
                hasCooldownDuet = true;
            }
            // Check for Fate Duet (Hourglass Fracture)
            else if ((weapon1 == "AstralChainripper" && weapon2 == "CosmicWeaver") ||
                     (weapon1 == "CosmicWeaver" && weapon2 == "AstralChainripper"))
            {
                hasCooldownDuet = true;
            }
            // Check for Ode to Joy Duet (Orchestra Conductor)
            else if ((weapon1 == "JoyfulCatharsis" && weapon2 == "OdeToCreation") ||
                     (weapon1 == "OdeToCreation" && weapon2 == "JoyfulCatharsis"))
            {
                hasCooldownDuet = true;
            }

            DuetAbilityButtonBorder.IsVisible = hasCooldownDuet;
        }

        /// <summary>
        /// Shows the Entropic Armory screen and initializes it with current game state.
        /// </summary>
        public void ShowEntropicArmory()
        {
            if (EntropicArmoryScreen != null)
            {
                EntropicArmoryScreen.Initialize(gameState, this);
                EntropicArmoryScreen.IsVisible = true;
                EntropicArmoryScreen.UpdateDisplay();
            }
        }

        /// <summary>
        /// Shows the Armory of Forte screen.
        /// </summary>
        public void ShowArmoryOfForte()
        {
            if (ArmoryOfForteScreen != null)
            {
                ArmoryOfForteScreen.IsVisible = true;
            }
        }

        /// <summary>
        /// Handler for Cacophonic Dreams endgame button.
        /// Only accessible when NPS reaches or exceeds 1 trillion.
        /// </summary>
        public async void CacophonicDreamsButton_Click(object? sender, RoutedEventArgs e)
        {
            // Set up the parent window reference for the screen
            if (CacophonicDreamsScreen != null)
            {
                CacophonicDreamsScreen.SetParentWindow(this);
            }
            
            // Transition to the Cacophonic Dreams screen
            await TransitionAsync(() =>
            {
                MainScreen.IsVisible = false;
                CacophonicDreamsScreen.IsVisible = true;
            });
        }
        
        // ------------------- FLOATING TEXT FEEDBACK -------------------
        
        /// <summary>
        /// Shows floating text at the mouse position indicating notes gained
        /// </summary>
        private async void ShowFloatingText(Point position, string text, Color color, bool hasStroke = false)
        {
            if (FloatingTextCanvas == null) return;
            
            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = 24,
                FontWeight = FontWeight.Bold,
                Foreground = new SolidColorBrush(color),
                Opacity = 1.0
            };
            
            // Add drop shadow or stroke for visibility
            if (hasStroke)
            {
                // Crimson outline for Crimson Requiem (black text)
                if (color == Colors.Black)
                {
                    textBlock.Effect = new DropShadowEffect
                    {
                        Color = Color.FromRgb(220, 20, 60), // Crimson
                        BlurRadius = 8,
                        Opacity = 1.0
                    };
                }
                // White stroke effect for other stroked crits (like Entropic Crescendo)
                else
                {
                    textBlock.Effect = new DropShadowEffect
                    {
                        Color = Colors.White,
                        BlurRadius = 8,
                        Opacity = 1.0
                    };
                }
            }
            else
            {
                textBlock.Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    Opacity = 0.8
                };
            }
            
            Canvas.SetLeft(textBlock, position.X);
            Canvas.SetTop(textBlock, position.Y - 30);
            
            FloatingTextCanvas.Children.Add(textBlock);
            
            // Animate upward and fade out
            double startY = position.Y - 30;
            double endY = startY - 80;
            
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromSeconds(1.5);
            
            while (DateTime.Now - startTime < duration)
            {
                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                var progress = elapsed / duration.TotalSeconds;
                
                // Update position (move up)
                Canvas.SetTop(textBlock, startY + (endY - startY) * progress);
                
                // Fade out
                textBlock.Opacity = 1.0 - progress;
                
                await Task.Delay(16); // ~60fps
            }
            
            // Remove from canvas
            FloatingTextCanvas.Children.Remove(textBlock);
        }
        
        private string FormatNumber(double value)
        {
            if (value >= 1_000_000_000_000)
                return $"{value / 1_000_000_000_000:F2}T";
            if (value >= 1_000_000_000)
                return $"{value / 1_000_000_000:F2}B";
            if (value >= 1_000_000)
                return $"{value / 1_000_000:F2}M";
            if (value >= 1_000)
                return $"{value / 1_000:F2}K";
            return $"{value:F0}";
        }
        
        /// <summary>
        /// Updates Swan Lake feather display on main screen
        /// </summary>
        private void UpdateMainSwanLakeCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Swan Lake Crescendance";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Three Feather System: Collect Revered, Chromatic, and Polyphonic feathers. Consume them for powerful bonuses!";
                
            // Hide all other panels
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainSwanFeatherPanel != null)
                MainSwanFeatherPanel.IsVisible = true;
                
            // Update feather counts
            if (MainReveredFeatherCount != null)
                MainReveredFeatherCount.Text = $"{gameState.ReveredFeathers} / 5";
                
            if (MainChromaticFeatherCount != null)
                MainChromaticFeatherCount.Text = $"{gameState.ChromaticFeathers} / 10";
                
            if (MainPolyphonicFeatherCount != null)
                MainPolyphonicFeatherCount.Text = gameState.PolyphonicFeathers.ToString();
                
            // Enable/disable consume buttons
            if (MainConsumeReveredButton != null)
                MainConsumeReveredButton.IsEnabled = gameState.ReveredFeathers >= 5;
                
            if (MainConsumeChromaticButton != null)
                MainConsumeChromaticButton.IsEnabled = gameState.ChromaticFeathers >= 10;
                
            if (MainConsumePolyphonicButton != null)
                MainConsumePolyphonicButton.IsEnabled = gameState.PolyphonicFeathers >= 1;
        }
        
        /// <summary>
        /// Updates Moonlight Sonata stack display on main screen
        /// </summary>
        private void UpdateMainMoonlightCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Moonlight Sonata: Eclipse of the Nocturne";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every 20th click at night (8PM-6AM): Gain Moonbeam Resonance. At 5 stacks: +250% notes + 1 Harmonizing Moonlight.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainMoonlightStackPanel != null)
                MainMoonlightStackPanel.IsVisible = true;
                
            // Update stack counts
            if (MainMoonbeamResonanceCount != null)
                MainMoonbeamResonanceCount.Text = $"{gameState.MoonbeamResonanceStacks} / 5";
                
            if (MainHarmonizingMoonlightCount != null)
                MainHarmonizingMoonlightCount.Text = gameState.HarmonizingMoonlightStacks.ToString();
        }
        
        /// <summary>
        /// Updates La Campanella crescendance info (Grandiose Bell)
        /// </summary>
        private void UpdateMainLaCampanellaCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "La Campanella: Grandiose Bell";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Grandiose Bell cracks at 20/40/60 clicks. Mend for rewards: Crescending (+2 minors), Radiant (5 entropic crits), Harmonizing (double notes).";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainLaCampanellaStackPanel != null)
                MainLaCampanellaStackPanel.IsVisible = true;
                
            // Update bell stage text
            string stageText = gameState.GrandioseBellStage switch
            {
                1 => "Crescending",
                2 => "Radiant",
                3 => "Harmonizing",
                _ => "Intact"
            };
            
            if (MainGrandioseBellStage != null)
                MainGrandioseBellStage.Text = stageText;
                
            if (MainGrandioseBellProgress != null)
                MainGrandioseBellProgress.Value = gameState.GrandioseBellClickCounter;
                
            if (MainMendBellButton != null)
                MainMendBellButton.IsEnabled = gameState.GrandioseBellStage > 0;
                
            if (MainDeafeningChimeCount != null)
                MainDeafeningChimeCount.Text = gameState.DeafeningChimeStacks.ToString();
        }
        
        /// <summary>
        /// Placeholder for Enigma crescendance info (mystery stacks)
        /// </summary>
        /// <summary>
        /// Updates Enigma crescendance info (Resonate Mystery)
        /// </summary>
        private void UpdateMainEnigmaCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Enigma Variations: Resonate Mystery";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every 10th/25th click: Gain Resonate Mystery. Consume for: +50% notes, +50 entropic, or +1 random minor. Bulk consume 10+ for massive bonus.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainEnigmaStackPanel != null)
                MainEnigmaStackPanel.IsVisible = true;
                
            // Update stack count
            if (MainResonateMysteryCount != null)
                MainResonateMysteryCount.Text = gameState.ResonateMysteryStacks.ToString();
                
            // Enable/disable consume buttons
            bool hasStacks = gameState.ResonateMysteryStacks >= 1;
            bool hasBulk = gameState.ResonateMysteryStacks >= 10;
            
            if (MainEnigmaConsumeNotesButton != null)
                MainEnigmaConsumeNotesButton.IsEnabled = hasStacks;
                
            if (MainEnigmaConsumeEntropicButton != null)
                MainEnigmaConsumeEntropicButton.IsEnabled = hasStacks;
                
            if (MainEnigmaConsumeMinorButton != null)
                MainEnigmaConsumeMinorButton.IsEnabled = hasStacks;
                
            if (MainEnigmaConsumeAllButton != null)
                MainEnigmaConsumeAllButton.IsEnabled = hasBulk;
        }
        
        /// <summary>
        /// Updates Fate crescendance info (Cosmic Modulation)
        /// </summary>
        private void UpdateMainFateCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Fate: Cosmic Modulation";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every 5th click: +1 Cosmic Modulation + 10% notes. Tiers grant escalating effects. Every 5 Cosmic: +1 Symphony of the Stars.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainFateStackPanel != null)
                MainFateStackPanel.IsVisible = true;
                
            // Update Cosmic Modulation
            if (MainCosmicModulationCount != null)
                MainCosmicModulationCount.Text = gameState.CosmicModulationStacks.ToString();
                
            // Calculate tier
            int tier = gameState.CosmicModulationStacks / 5;
            if (MainCosmicModulationTier != null)
                MainCosmicModulationTier.Text = tier.ToString();
                
            if (MainConsumeCosmicButton != null)
                MainConsumeCosmicButton.IsEnabled = gameState.CosmicModulationStacks >= 1;
                
            // Update Symphony of the Stars
            if (MainSymphonyStarsCount != null)
                MainSymphonyStarsCount.Text = gameState.SymphonyOfTheStarsStacks.ToString();
                
            if (MainConsumeSymphonyButton != null)
                MainConsumeSymphonyButton.IsEnabled = gameState.SymphonyOfTheStarsStacks >= 1;
        }
        
        /// <summary>
        /// Updates Eroica crescendance info (Heroic Resolve, Symphonic Catharsis, Testament of Harmony)
        /// </summary>
        private void UpdateMainEroicaCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Eroica: Heroic Resolve";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every minor craft: +1 Heroic Resolve. Consume with 25% notes for Symphonic Catharsis (10s double NPC +10% crit). Funeral Prayer grants Testament.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainEroicaStackPanel != null)
                MainEroicaStackPanel.IsVisible = true;
                
            // Update Heroic Resolve count
            if (MainHeroicResolveCount != null)
                MainHeroicResolveCount.Text = gameState.HeroicResolveStacks.ToString();
                
            if (MainConsumeHeroicResolveButton != null)
            {
                double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                MainConsumeHeroicResolveButton.IsEnabled = gameState.HeroicResolveStacks >= 1 && currentNotes > 0;
            }
                
            // Update Symphonic Catharsis count
            if (MainSymphonicCatharsisCount != null)
                MainSymphonicCatharsisCount.Text = gameState.SymphonicCatharsisStacks.ToString();
                
            if (MainConsumeCatharsisButton != null)
                MainConsumeCatharsisButton.IsEnabled = gameState.SymphonicCatharsisStacks >= 1;
                
            // Show Testament panel if Funeral Prayer is equipped
            bool showTestament = gameState.FuneralPrayer && 
                (gameState.CurrentResonatedWeapon1 == "FuneralPrayer" || gameState.CurrentResonatedWeapon2 == "FuneralPrayer");
                
            if (MainTestamentPanel != null)
                MainTestamentPanel.IsVisible = showTestament;
                
            if (showTestament)
            {
                if (MainTestamentCount != null)
                    MainTestamentCount.Text = gameState.TestamentOfHarmonyStacks.ToString();
                    
                if (MainConsumeTestamentButton != null)
                    MainConsumeTestamentButton.IsEnabled = gameState.TestamentOfHarmonyStacks >= 1;
            }
        }
        
        /// <summary>
        /// Updates Dies Irae crescendance info (Burning Hatred, Discordant Malice)
        /// </summary>
        private void UpdateMainDiesIraeCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Dies Irae: Wrath of the Damned";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every click: +1 Burning Hatred (max 50), then Discordant Malice. Consume Malice to multiply notes by stack count.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
                
            if (MainDiesIraeStackPanel != null)
                MainDiesIraeStackPanel.IsVisible = true;
                
            // Update Burning Hatred count
            if (MainBurningHatredCount != null)
                MainBurningHatredCount.Text = $"{gameState.BurningHatredStacks} / 50";
                
            // Update Discordant Malice count
            if (MainDiscordantMaliceCount != null)
                MainDiscordantMaliceCount.Text = gameState.DiscordantMaliceStacks.ToString();
                
            if (MainConsumeDiscordantButton != null)
                MainConsumeDiscordantButton.IsEnabled = gameState.DiscordantMaliceStacks >= 1;
        }
        
        private void UpdateMainScreenCrescendancePanel()
        {
            if (MainScreenCrescendancePanel == null) return;
            
            // Only show if we have a resonated score
            if (string.IsNullOrEmpty(gameState.CurrentResonatedScore) || gameState.CurrentResonatedScore == "None")
            {
                MainScreenCrescendancePanel.IsVisible = false;
                return;
            }
            
            // Show panel and update content based on score
            MainScreenCrescendancePanel.IsVisible = true;
            
            if (gameState.CurrentResonatedScore == "Moonlight Sonata")
            {
                UpdateMainMoonlightCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "Swan")
            {
                UpdateMainSwanLakeCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "Eroica")
            {
                UpdateMainEroicaCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "LaCampanella")
            {
                UpdateMainLaCampanellaCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "Enigma")
            {
                UpdateMainEnigmaCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "Fate")
            {
                UpdateMainFateCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "DiesIrae")
            {
                UpdateMainDiesIraeCrescendanceInfo();
            }
            else
            {
                // No crescendance system for this score yet (OdeToJoy, Winter)
                MainScreenCrescendancePanel.IsVisible = false;
            }
        }
        
        #region Crescendance Button Click Handlers
        
        // Swan Lake Handlers
        private void MainConsumeReveredButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.SwanLake_ConsumeReveredFeathers(gameState);
            UpdateMainSwanLakeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeChromaticButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.SwanLake_ConsumeChromaticFeathers(gameState);
            UpdateMainSwanLakeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumePolyphonicButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.SwanLake_ConsumePolyphonicFeather(gameState);
            UpdateMainSwanLakeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        // Eroica Handlers
        private void MainConsumeHeroicResolveButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_ConsumeForCatharsis(gameState);
            UpdateMainEroicaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeCatharsisButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_ConsumeCatharsis(gameState);
            UpdateMainEroicaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeTestamentButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.FuneralPrayer_ConsumeTestament(gameState);
            UpdateMainEroicaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        // La Campanella Handlers

        private void MainMendBellButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.LaCampanella_MendBell(gameState, this);
            UpdateMainLaCampanellaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        // Enigma Handlers
        /// <summary>
        /// Consume Resonate Mystery for +50% notes
        /// </summary>
        private void MainEnigmaConsumeNotesButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Enigma_ConsumeStack(gameState, "Notes");
            UpdateMainEnigmaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Consume Resonate Mystery for +50 entropic melodies
        /// </summary>
        private void MainEnigmaConsumeEntropicButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Enigma_ConsumeStack(gameState, "Entropic");
            UpdateMainEnigmaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Consume Resonate Mystery for +1 random minor
        /// </summary>
        private void MainEnigmaConsumeMinorButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Enigma_ConsumeStack(gameState, "Minor");
            UpdateMainEnigmaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Consume all Resonate Mystery stacks (10+) for bulk bonus
        /// </summary>
        private void MainEnigmaConsumeAllButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Enigma_ConsumeAllStacks(gameState);
            UpdateMainEnigmaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        // Fate Handlers
        /// <summary>
        /// Consume Cosmic Modulation stacks for Entropic Melodies
        /// </summary>
        private void MainConsumeCosmicButton_Click(object? sender, RoutedEventArgs e)
        {
            // Consume all stacks for 15 entropic each
            int stackCount = gameState.CosmicModulationStacks;
            MusicClicker.Armory.WeaponAbilities.Fate_ConsumeStacksForMelodies(gameState, stackCount);
            UpdateMainFateCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Consume Symphony of the Stars for +3 to lowest minor
        /// </summary>
        private void MainConsumeSymphonyButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Fate_ConsumeSymphonyStack(gameState);
            UpdateMainFateCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        // Dies Irae Handlers
        private void MainConsumeDiscordantButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.DiesIrae_ConsumeDiscordantMalice(gameState);
            UpdateMainDiesIraeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        #endregion
    }
}

