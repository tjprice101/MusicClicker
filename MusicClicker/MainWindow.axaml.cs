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
        
        // Random number generator for any randomized game mechanics (public for reuse in DuetAbilityScreen - performance optimization)
        public Random _random = new Random();
        
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


        // Carousel removed: buttons are static in the grid-based layout.
        // Previous carousel fields and animation logic were removed to prevent
        // runtime transforms that relocated buttons from their panel positions.

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

            // Carousel removed: no initialization required for grid layout.

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

                // Drive animation-driven visuals (smooth Notes/NPS) so HUD shows
                // the up-to-date game values even when UI updates are throttled.
                try
                {
                    UIUpdater.AnimateVisuals(this, gameState, elapsedSeconds);
                }
                catch { }

                // Update lightweight notes-only text frequently for responsiveness
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
                    // Cache DateTime.Now once per tick to avoid 30+ system calls (performance optimization)
                    DateTime now = DateTime.Now;
                    
                    double elapsed = _bgStopwatch.Elapsed.TotalSeconds;
                    _bgStopwatch.Restart();
                    if (gameState != null && gameState.NotesPerSecond != 0)
                    {
                        double effectiveNps = gameState.NotesPerSecond;

                        // Check if Astral Chainripper boost has expired
                        if (gameState.AstralChainripperNpsBoostActive && now > gameState.AstralChainripperNpsBoostExpiry)
                        {
                            gameState.AstralChainripperNpsBoostActive = false;
                        }

                        // Check if Ode to Creation double petal generation has expired
                        if (gameState.OdeToCreationDoubleActive && now > gameState.OdeToCreationDoubleExpiry)
                        {
                            gameState.OdeToCreationDoubleActive = false;
                        }

                        // Check if NPS freeze has expired
                        if (gameState.NpsFrozen && now > gameState.NpsFreezeExpiry)
                        {
                            gameState.NpsFrozen = false;
                            // Keep FrozenNpsValue for reference, don't zero it
                        }

                        // Check if Winter Duet has expired and unfreeze NPS
                        if (gameState.WinterDuetActive && now > gameState.WinterDuetExpiry)
                        {
                            gameState.WinterDuetActive = false;
                            gameState.NpsFrozen = false;
                            gameState.WinterDuetCooldownExpiry = now.AddSeconds(DuetDescriptions.Cooldown.Winter);
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Dies Irae Duet has expired
                        if (gameState.DiesIraeDuetActive && now > gameState.DiesIraeDuetExpiry)
                        {
                            gameState.DiesIraeDuetActive = false;
                            gameState.SevenSealsCounter = 0;
                            gameState.DiesIraeDuetCooldownExpiry = now.AddSeconds(DuetDescriptions.Cooldown.DiesIrae);
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Accelerating Flurry has decayed (The Snow's Desire passive)
                        if (gameState.AcceleratingFlurryBonus > 0 && 
                            (now - gameState.AcceleratingFlurryLastClickTime).TotalSeconds > 5)
                        {
                            gameState.AcceleratingFlurryBonus = 0;
                        }

                        // Check if Moonlight Duet has expired - start cooldown when it expires naturally
                        if (gameState.MoonlightDuetActive && now > gameState.MoonlightDuetExpiry)
                        {
                            gameState.MoonlightDuetActive = false;
                            gameState.MoonlightDuetCooldownExpiry = now.AddSeconds(240); // 4 minutes
                            // Update duet screen if visible
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Swan Lake Duet has expired and start cooldown
                        if (gameState.SwanLakeDuetActive && now > gameState.SwanLakeDuetExpiry)
                        {
                            gameState.SwanLakeDuetActive = false;
                            gameState.SwanLakeDuetClickCounter = 0; // Reset click counter
                            gameState.SwanLakeDuetCooldownExpiry = now.AddSeconds(240); // 4 minutes
                            // Update duet screen if visible
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if La Campanella Duet has expired
                        if (gameState.LaCampanellaDuetActive && now > gameState.LaCampanellaDuetExpiry)
                        {
                            gameState.LaCampanellaDuetActive = false;
                            gameState.ChimeChainLength = 0;
                            gameState.LastChimeClickTime = DateTime.MinValue;
                            gameState.LaCampanellaDuetCooldownExpiry = now.AddSeconds(600); // 10 minutes
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Enigma Duet has expired
                        if (gameState.EnigmaDuetActive && now > gameState.EnigmaDuetExpiry)
                        {
                            gameState.EnigmaDuetActive = false;
                            gameState.EnigmaMysteryClickCount = 0;
                            gameState.EnigmaDuetCooldownExpiry = now.AddSeconds(1800); // 30 minutes
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Fate Duet has expired
                        if (gameState.FateDuetActive && now > gameState.FateDuetExpiry)
                        {
                            gameState.FateDuetActive = false;
                            gameState.FateDuetHasFlipped = false;
                            gameState.FateDuetClickCount = 0;
                            gameState.HourglassActionBank.Clear();
                            gameState.FateDuetCooldownExpiry = now.AddSeconds(480); // 8 minutes
                            if (DuetAbilityScreen?.IsVisible == true)
                            {
                                DuetAbilityScreen.UpdateAbilityDisplay();
                            }
                        }

                        // Check if Ode Duet has expired
                        if (gameState.OdeDuetActive && now > gameState.OdeDuetExpiry)
                        {
                            // Grant +3 Entropic Melodies per completed section
                            int entropicReward = gameState.CrescendoCompletedSections * 3;
                            gameState.EntropicMelodies += entropicReward;
                            
                            gameState.OdeDuetActive = false;
                            gameState.CrescendoNotesPlaced = 0;
                            gameState.CrescendoCompletedSections = 0;
                            gameState.Crescendo4Claimed = false;
                            gameState.Crescendo8Claimed = false;
                            gameState.Crescendo12Claimed = false;
                            gameState.Crescendo16Claimed = false;
                            gameState.OdeDuetCooldownExpiry = now.AddSeconds(DuetDescriptions.Cooldown.OdeToJoy);
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

                        // Apply Ode to Joy Duet 5x NPS boost from completing 16-note crescendo
                        if (gameState.OdeDuetNpsBoostActive && now <= gameState.OdeDuetNpsBoostExpiry)
                        {
                            effectiveNps *= 5.0;
                        }
                        else if (gameState.OdeDuetNpsBoostActive && now > gameState.OdeDuetNpsBoostExpiry)
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
                        if (gameState.NpsFrozen && now <= gameState.NpsFreezeExpiry)
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

            // Wire up FeedbackButton to open the Google Form
            var feedbackButton = this.FindControl<Button>("FeedbackButton");
            if (feedbackButton != null)
            {
                feedbackButton.Click += (_, __) =>
                {
                    try
                    {
                        var url = "https://forms.gle/gmbzf6voQWXNgjMq9";
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to open feedback form: {ex.Message}");
                    }
                };
            }

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

        // Carousel logic removed: buttons are static in the grid-based layout.
        // All initialization, transforms, drag handlers and animation were removed
        // to prevent runtime repositioning of UI elements.

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

        // Carousel methods removed: animation and position updates are no longer used.

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
            
            // Apply The Snow's Desire Accelerating Flurry bonus (+1% to +50% NPC)
            if (gameState.AcceleratingFlurryBonus > 0)
            {
                notesPerClick *= (1.0 + (gameState.AcceleratingFlurryBonus / 100.0));
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
            
            // Apply Cacophonic Blizzard bonus clicks (+50% notes for next 20 clicks)
            if (gameState.CacophonicBlizzardBonusClicksRemaining > 0)
            {
                notesPerClick *= 1.5; // +50% notes
                gameState.CacophonicBlizzardBonusClicksRemaining--;
            }
            
            // Apply Incisor of Moonlight passive: Every 4th click bonus (while equipped)
            if (gameState.IncisorOfMoonlight && 
                (gameState.CurrentResonatedWeapon1 == "IncisorOfMoonlight" || gameState.CurrentResonatedWeapon2 == "IncisorOfMoonlight"))
            {
                gameState.IncisorClickCounter++;
                if (gameState.IncisorClickCounter >= 4)
                {
                    gameState.IncisorClickCounter = 0;
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    double bonusPercent = isNighttime ? 0.05 : 0.01; // +5% at night, +1% normally
                    double bonus = currentNotes * bonusPercent;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
                }
            }
            
            // Funeral Prayer: Add 6x NPS bonus to notesPerClick if empowered
            bool funeralPrayerEmpoweredThisClick = false;
            if (gameState.FuneralPrayerAbility)
            {
                double empoweredBonus = MusicClicker.Armory.WeaponAbilities.FuneralPrayer_GetEmpoweredClickBonus(gameState);
                if (empoweredBonus > 0)
                {
                    notesPerClick += empoweredBonus;
                    funeralPrayerEmpoweredThisClick = true;
                }
            }

            // Thousand Winged Swan: Replace NPC with NPS × 50^stacks
            bool thousandWingedSwanBoostActive = false;
            double swanNpcReplacement = MusicClicker.Armory.WeaponAbilities.ThousandWingedSwan_GetNpcBoost(gameState);
            if (swanNpcReplacement > 0)
            {
                // Replace base NPC entirely with the boosted value
                notesPerClick = swanNpcReplacement;
                thousandWingedSwanBoostActive = true;
            }

            // Add calculated notes to player's total
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, notesPerClick);

            // Funeral Prayer: Track clicks (only if not currently using empowered clicks)
            // Self-contained rule: Empowered clicks don't build more Prayer stacks
            if (gameState.FuneralPrayerAbility && gameState.FuneralPrayerEmpoweredClicksRemaining == 0)
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
            
            // Clair De Lune Duet: 12-click sequence
            if (gameState.ClairDeLuneDuetActive)
            {
                MusicClicker.Armory.WeaponAbilities.ClairDeLuneDuet_OnClick(gameState);
            }

            // Individual weapon click abilities
            if (gameState.OdeToCreation && 
                (gameState.CurrentResonatedWeapon1 == "OdeToCreation" || gameState.CurrentResonatedWeapon2 == "OdeToCreation"))
            {
                MusicClicker.Armory.WeaponAbilities.OdeToCreation_OnClick(gameState);
            }

            // Joyful Catharsis passive: Every 50th click grants Entropic Melodies
            if (gameState.JoyfulCatharsis && 
                (gameState.CurrentResonatedWeapon1 == "JoyfulCatharsis" || gameState.CurrentResonatedWeapon2 == "JoyfulCatharsis"))
            {
                MusicClicker.Armory.WeaponAbilities.JoyfulCatharsis_OnClick(gameState);
            }

            // Winter: Cacophonic Blizzard - every 20th click freezes NPS + buffs next 20 clicks
            if (gameState.CacophonicBlizzard &&
                (gameState.CurrentResonatedWeapon1 == "CacophonicBlizzard" || gameState.CurrentResonatedWeapon2 == "CacophonicBlizzard"))
            {
                MusicClicker.Armory.WeaponAbilities.CacophonicBlizzard_OnClick(gameState);
            }
            
            // Winter: The Snow's Desire - Accelerating Flurry (+1% NPC per click, max 50%)
            if (gameState.TheSnowsDesire &&
                (gameState.CurrentResonatedWeapon1 == "TheSnowsDesire" || gameState.CurrentResonatedWeapon2 == "TheSnowsDesire"))
            {
                MusicClicker.Armory.WeaponAbilities.TheSnowsDesire_OnClick(gameState);
            }

            // Swan Lake Crescendance: Feather collection on clicks
            if (gameState.CurrentResonatedScore == "Swan")
            {
                MusicClicker.Armory.WeaponAbilities.SwanLakeCrescendance_OnClick(gameState);
            }
            
            // Moonlight Sonata Crescendance: Moonbeam Resonance stacks (every 20th click at night)
            // Use canonical internal name set by TempoResonateManager (`"Moonlight Sonata"`).
            if (gameState.CurrentResonatedScore == "Moonlight Sonata" && isNighttime)
            {
                MusicClicker.Armory.WeaponAbilities.MoonlightCrescendance_OnClick(gameState, this);
            }
            
            // La Campanella Crescendance: Grandiose Bell click tracking
            if (gameState.CurrentResonatedScore == "LaCampanella")
            {
                MusicClicker.Armory.WeaponAbilities.LaCampanellaCrescendance_OnClick(gameState, this);
            }
            
            // Enigma Crescendance: Resonant Mystery stacks (every 10th click, +15th with Creator)
            if (gameState.CurrentResonatedScore == "Enigma")
            {
                MusicClicker.Armory.WeaponAbilities.EnigmaCrescendance_OnClick(gameState, this);
            }
            
            // Fate Crescendance: Cosmic Modulation stacks (every 8th click + 10% notes)
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
            
            // Clair De Lune Crescendance: Clockwork Symphony
            if (gameState.CurrentResonatedScore == "ClairDeLune")
            {
                MusicClicker.Armory.WeaponAbilities.ClairDeLuneCrescendance_OnClick(gameState, this);
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
                if (gameState.HellsWrathAbility)
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

                // Eulogy of the Moon passive does NOT modify crit chance.
                // (Nocturnal Refund and other Eulogy effects are handled elsewhere.)
                
                // Apply Fate Cosmic Modulation tier bonuses
                int cosmicTier = gameState.CosmicModulationStacks;
                bool fateGuaranteedCrit = false;
                if (gameState.CurrentResonatedScore == "Fate")
                {
                    // Tier 1: Every click grants +5 Entropic Melodies
                    if (cosmicTier >= 1)
                    {
                        gameState.EntropicMelodies += 5;
                    }
                    
                    // Tier 2: Every click grants +5 Melodious and Harmonious Fragments
                    if (cosmicTier >= 2)
                    {
                        gameState.MelodiousOwned += 5;
                        gameState.HarmoniousOwned += 5;
                    }
                    
                    // Tier 5: Guaranteed Entropic Crescendo on every click
                    if (cosmicTier >= 5)
                    {
                        fateGuaranteedCrit = true;
                    }
                }
                
                // Clair De Lune: Minute Hand 1 effect (next 3 clicks after Temporal Harmony gain)
                if (gameState.ClairDeLunePowerSpikePending > 0)
                {
                    gameState.ClairDeLunePowerSpikePending--;
                    double entropicGain = 1.5;
                    
                    // Minute Hand 4: increase notes by 0.1% per Entropic Melodies gained
                    if (gameState.ClairDeLuneMinuteHand == 4)
                    {
                        double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                        double noteBonus = currentNotes * (entropicGain * 0.001);
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, noteBonus);
                    }
                    
                    gameState.EntropicMelodies += (int)entropicGain;
                }
                
                string critText;
                Color critColor;
                double finalNotes = notesPerClick;
                bool hasStroke = false;
                Color strokeColor = Colors.Black; // Default stroke color
                
                // ==================== SPECIAL CLICK EFFECT PRIORITY SYSTEM ====================
                // Priority Order (highest to lowest value):
                // 1. Dawn of Swan's Glory (display-only, unique)
                // 2. Symphony of Hell's Retribution (Dies Irae, value-based)
                // 3. Seal-breaking Melody (Dies Irae, value-based)
                // 4. Crimson Requiem vs Funeral Prayer Empowered (highest value)
                // 5. Thousand Winged Swan: Dawn of Swan's Glory (NPS-to-NPC boost)
                // 6. Petal's Entropic Bloom (Ode to Joy counter, 1500x)
                // 7. Stellar Cascade (Cosmic Weaver counter, 1700x)
                // 8. Fate Tier 5: Guaranteed Entropic Crescendo (1500x)
                // 9. Random Entropic Crescendo (0.1%/2.1%, 1500x)
                // 10. Random Superior Crescendo (1%, 5x)
                // 11. Random Critical Crescendo (5%, 2x)
                // 12. Normal click
                
                // 1. Crescendance Bond - Thousand Winged Swan: Dawn of Swan's Glory (absolute highest priority - display only)
                if (gameState.ThousandWingedSwanNpsBoostActive && DateTime.Now <= gameState.ThousandWingedSwanNpsBoostExpiry)
                {
                    critText = $"Dawn of the Swan's Glory!!! +{FormatNumber(notesPerClick)}";
                    critColor = Colors.White; // White text
                    hasStroke = true; // Will get dark pink outline
                }
                // 2. Dies Irae Duet: Note-Doubling Clicks (from consuming 15+ Wrathful Seals)
                else if (gameState.DiesIraeDuetNoteDoublingClicks > 0)
                {
                    gameState.DiesIraeDuetNoteDoublingClicks--;
                    finalNotes = gameState.Notes * 2; // Double current notes
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Infernal Symphony of Oblivion!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(139, 0, 0); // Dark red
                    hasStroke = true;
                    strokeColor = Color.FromRgb(255, 140, 0); // Dark orange outline
                }
                // 3. Dies Irae: Symphony of Hell's Retribution crit (NPC × NPS × Dissonant Hatred stacks)
                else if (gameState.SymphonyOfHellClicks > 0)
                {
                    gameState.SymphonyOfHellClicks--;
                    finalNotes = notesPerClick * gameState.NotesPerSecond * Math.Max(1, gameState.DissonantHatredStacks);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Symphony of Hell's Retribution!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(139, 0, 0); // Dark red
                    hasStroke = true;
                    strokeColor = Color.FromRgb(255, 140, 0); // Dark orange outline
                    
                    // Seven Circles: While Dies Irae Crescendance is active, Symphony of Hell's Retribution crits grant +1 Wrathful Seal
                    if (gameState.SevenCirclesAbility)
                    {
                        gameState.WrathfulSealStacks++;
                    }
                    
                    // Hell's Wrath Crescendance Bond: +50 Entropic Melodies per Symphony of Hell's Retribution crit
                    if (gameState.HellsWrathAbility)
                    {
                        gameState.EntropicMelodies += 50;
                    }
                }
                // 4. Dies Irae: Seal-breaking Melody of Oblivion (DOUBLE Symphony of Hell's Retribution formula)
                // During duet: Every click is a guaranteed Seal-breaking crit and returns +1 Wrathful Seal
                else if (gameState.SealBreakingMelodyClicks > 0 || gameState.DiesIraeDuetActive)
                {
                    if (gameState.SealBreakingMelodyClicks > 0)
                    {
                        gameState.SealBreakingMelodyClicks--;
                    }
                    finalNotes = notesPerClick * gameState.NotesPerSecond * Math.Max(1, gameState.DissonantHatredStacks) * 2; // Double Symphony of Hell's Retribution
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Seal-breaking Melody of Oblivion!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(178, 34, 34); // Firebrick red
                    hasStroke = true;
                    strokeColor = Color.FromRgb(255, 69, 0); // Orange-red outline
                    
                    // Duet: Each Seal-breaking crit returns +1 Wrathful Seal
                    if (gameState.DiesIraeDuetActive)
                    {
                        gameState.WrathfulSealStacks++;
                    }
                    
                    // Hell's Wrath Crescendance Bond: +50 Entropic Melodies per Seal-breaking crit
                    if (gameState.HellsWrathAbility)
                    {
                        gameState.EntropicMelodies += 50;
                    }
                }
                // 4. Winter: Blizzard's Command of Eternal Ice (enhanced clicks from Eternal Frost or Duet)
                else if (gameState.BlizzardCommandClicksRemaining > 0)
                {
                    gameState.BlizzardCommandClicksRemaining--;
                    // Blizzard's Command new formula: give notes equal to (NPS * NPC)^2 per click
                    double prod = gameState.NotesPerSecond * gameState.NotesPerClick;
                    finalNotes = prod * prod;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Blizzard's Command of Eternal Ice!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(173, 216, 230); // Light blue
                    hasStroke = true;
                    strokeColor = Color.FromRgb(0, 191, 255); // Deep sky blue outline
                }
                // 5. Priority comparison: Crimson Requiem vs Funeral Prayer Empowered (use highest value)
                else if (gameState.CrimsonRequiemClicksRemaining > 0 || funeralPrayerEmpoweredThisClick)
                {
                    // Calculate values for comparison
                    double crimsonValue = gameState.CrimsonRequiemClicksRemaining > 0 
                        ? (notesPerClick + (gameState.NotesPerSecond * gameState.NotesPerClick)) 
                        : 0;
                    double funeralValue = funeralPrayerEmpoweredThisClick 
                        ? notesPerClick // Already includes NPS * 6 from empowered bonus
                        : 0;
                    
                    // Show the effect with highest value (Crimson Requiem if tied or higher)
                    if (crimsonValue >= funeralValue && crimsonValue > 0)
                    {
                        // Crescendance Bond - Sakura's Blossom: Crimson Requiem
                        gameState.CrimsonRequiemClicksRemaining--;
                        finalNotes = notesPerClick + (gameState.NotesPerSecond * gameState.NotesPerClick);
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                        critText = $"Blossom's Blooming in Crimson Light!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                        critColor = Color.FromRgb(250, 128, 114); // Salmon-red
                        hasStroke = true;
                    }
                    else
                    {
                        // Forte Resonance - Funeral Prayer: Prayer of Valor
                        critText = $"Retribution of the Symphonic Sakura!!! +{NumberFormatter.FormatLargeNumber(notesPerClick)}";
                        critColor = Color.FromRgb(199, 21, 133); // Dark pink (MediumVioletRed)
                        hasStroke = true;
                    }
                }
                // 6. Thousand Winged Swan: Dawn of the Swan's Glory (NPS-to-NPC boost active)
                else if (thousandWingedSwanBoostActive)
                {
                    critText = $"Dawn of the Swan's Glory!!! +{NumberFormatter.FormatLargeNumber(notesPerClick)}";
                    critColor = Colors.White; // White text
                    hasStroke = true;
                    strokeColor = Color.FromRgb(199, 21, 133); // Dark pink outline
                }
                // 6.5. Clair De Lune: Lunar Everlasting (Weapon 1 crit)
                else if (gameState.ClairWeapon1CritClicksRemaining > 0)
                {
                    gameState.ClairWeapon1CritClicksRemaining--;
                    // Formula: ((NPS^NPC)/(144)) notes
                    double nps = gameState.NotesPerSecond;
                    double npc = gameState.NotesPerClick;
                    finalNotes = (Math.Pow(nps, npc)) / 144.0;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Lunar Everlasting!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(135, 206, 250); // Light sky blue (time/clock theme)
                    hasStroke = true;
                    strokeColor = Color.FromRgb(25, 25, 112); // Midnight blue outline
                }
                // 6.6. Clair De Lune: Eternal Moonlight Opus (Duet 12th click effect)
                else if (gameState.ClairSymphonyOfInfinityClicksRemaining > 0)
                {
                    gameState.ClairSymphonyOfInfinityClicksRemaining--;
                    // Formula: (NPC^NPS^12) notes
                    double nps = gameState.NotesPerSecond;
                    double npc = gameState.NotesPerClick;
                    finalNotes = Math.Pow(npc, Math.Pow(nps, 12));
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Eternal Moonlight Opus!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(218, 165, 32); // Golden rod (infinity theme)
                    hasStroke = true;
                    strokeColor = Colors.White;
                }
                // 6.7. Clair De Lune: Twilight Rupture (Hour Hand 9 effect)
                else if (gameState.ClairCadenzaCataclysmClicksRemaining > 0)
                {
                    gameState.ClairCadenzaCataclysmClicksRemaining--;
                    // Formula: ((NPS^9)/5) notes
                    double nps = gameState.NotesPerSecond;
                    finalNotes = (Math.Pow(nps, 9)) / 5.0;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Twilight Rupture!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(147, 112, 219); // Medium purple (chronos/time theme)
                    hasStroke = true;
                    strokeColor = Colors.Black;
                }
                // 6.8. Clair De Lune: Timeless Melody (Minute Hand 2 effect)
                else if (gameState.ClairInfinityArpeggioClicksRemaining > 0)
                {
                    gameState.ClairInfinityArpeggioClicksRemaining--;
                    // Timeless Melody is a guaranteed critical - use 1500x multiplier like Entropic Crescendo
                    finalNotes = notesPerClick * 1500;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Timeless Melody!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(0, 255, 255); // Cyan (infinity/time theme)
                    hasStroke = true;
                    strokeColor = Color.FromRgb(25, 25, 112); // Midnight blue outline
                    
                    // La Campanella: Entropic-level crits grant +3 Deafening Chime stacks (max 15)
                    if (gameState.CurrentResonatedScore == "LaCampanella")
                    {
                        int stacksToAdd = Math.Min(3, 15 - gameState.DeafeningChimeStacks);
                        gameState.DeafeningChimeStacks += stacksToAdd;
                    }
                }
                // 6.9. Clair De Lune: Entropic Crescendo of Eternity (Duet 9th click)
                else if (gameState.ClairDuetEntropicCritClicksRemaining > 0)
                {
                    gameState.ClairDuetEntropicCritClicksRemaining--;
                        
                    finalNotes = notesPerClick * 1500;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Entropic Crescendo of Eternity!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Colors.Red;
                    hasStroke = true;
                    strokeColor = Colors.Black;
                    
                    // La Campanella: Entropic Crescendo grants +3 Deafening Chime stacks (max 15)
                    if (gameState.CurrentResonatedScore == "LaCampanella")
                    {
                        int stacksToAdd = Math.Min(3, 15 - gameState.DeafeningChimeStacks);
                        gameState.DeafeningChimeStacks += stacksToAdd;
                    }
                }
                // 7. Ode to Joy: Entropic Crescendo of Eternity from Petal of Melody (1500x multiplier, time-based)
                else if (DateTime.Now <= gameState.EntropicCritExpiry)
                {
                    finalNotes = notesPerClick * 1500;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Entropic Crescendo of Eternity!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Colors.Red;
                    hasStroke = true;
                    strokeColor = Colors.Black;
                    
                    // La Campanella: Entropic Crescendo grants +3 Deafening Chime stacks (max 15)
                    if (gameState.CurrentResonatedScore == "LaCampanella")
                    {
                        int stacksToAdd = Math.Min(3, 15 - gameState.DeafeningChimeStacks);
                        gameState.DeafeningChimeStacks += stacksToAdd;
                    }
                }
                // 7. Cosmic Weaver: Stellar Cascade Crit Clicks from Symphony consume (1700x multiplier, requires Fate weapon equipped)
                else if (gameState.CosmicWeaverEntropicCritClicks > 0 && 
                         (gameState.CurrentResonatedScore == "Fate" || 
                          gameState.AstralChainripperAbility || 
                          gameState.CosmicWeaverAbility))
                {
                    gameState.CosmicWeaverEntropicCritClicks--;
                    finalNotes = notesPerClick * 1700;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Stellar Cascade!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(138, 43, 226); // Blue Violet (cosmic/fate themed)
                    hasStroke = true;
                    strokeColor = Colors.White; // White glow
                }
                else
                {
                    // 8. Fate Tier 5+: Guaranteed Entropic Crescendo
                    if (fateGuaranteedCrit)
                    {
                        finalNotes = notesPerClick * 1500;
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                        critText = $"Entropic Crescendo of Eternity!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                        critColor = Colors.Red;
                        hasStroke = true;
                        strokeColor = Colors.Black;
                        
                        // La Campanella: Entropic Crescendo grants +3 Deafening Chime stacks (max 15)
                        if (gameState.CurrentResonatedScore == "LaCampanella")
                        {
                            int stacksToAdd = Math.Min(3, 15 - gameState.DeafeningChimeStacks);
                            gameState.DeafeningChimeStacks += stacksToAdd;
                        }
                    }
                    // 9. Random Entropic Crescendo (0.1% base + 2% if Razer equipped, 1500x multiplier)
                    else if (roll < (gameState.RazerOfBellsChimesAbility ? 2.1 : 0.1))
                    {
                        finalNotes = notesPerClick * 1500;
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                        critText = $"Entropic Crescendo of Eternity!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                        critColor = Colors.Red;
                        hasStroke = true;
                        strokeColor = Colors.Black;
                        
                        // La Campanella: Entropic Crescendo grants +3 Deafening Chime stacks (max 15)
                        if (gameState.CurrentResonatedScore == "LaCampanella")
                        {
                            int stacksToAdd = Math.Min(3, 15 - gameState.DeafeningChimeStacks);
                            gameState.DeafeningChimeStacks += stacksToAdd;
                        }
                    }
                    // 10. Random Superior Crescendo (1% chance, 5x multiplier)
                    else if (roll < (gameState.RazerOfBellsChimesAbility ? 3.1 : 1.1))
                    {
                        finalNotes = notesPerClick * 5;
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                        critText = $"Superior Crescendo!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                        critColor = Color.FromRgb(255, 20, 147); // Deep pink
                    }
                    // 11. Random Critical Crescendo (5% chance, 2x multiplier)
                    else if (roll < (gameState.RazerOfBellsChimesAbility ? 8.1 : 6.1))
                    {
                        finalNotes = notesPerClick * 2;
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                        critText = $"Critical Crescendo!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                        critColor = Color.FromRgb(255, 182, 193); // Light pink
                    }
                    // 11. Normal click
                    else
                    {
                        critText = $"+{NumberFormatter.FormatLargeNumber(notesPerClick)} Notes";
                        critColor = Colors.White;
                    }
                }
                
                // Seven Circles Passive: Every critical hit in the game grants +20% of current notes
                if (gameState.SevenCirclesAbility && critText != null && critText.Contains("!!!"))
                {
                    double bonusNotes = gameState.Notes * 0.20;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonusNotes);
                }
                
                ShowFloatingText(_lastClickPosition, critText, critColor, hasStroke, strokeColor);
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
                gameState.ClairDeLuneMajorOwned += 1;
                gameState.MercuryMajorOwned += 1;
                gameState.MarsMajorOwned += 1;

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
                gameState.MetronomicDissonance = true;
                gameState.ClockworksHarmony = true;

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
        /// Also updates the button's image to match the active duet.
        /// </summary>
        public void UpdateDuetAbilityButtonVisibility()
        {
            if (OpenDuetAbilityButton == null || DuetAbilityButtonImage == null || gameState == null) return;

            string weapon1 = gameState.CurrentResonatedWeapon1;
            string weapon2 = gameState.CurrentResonatedWeapon2;
            bool hasCooldownDuet = false;
            string? duetImagePath = null;

            // Check for Moonlight Duet (Lunar Phases)
            if ((weapon1 == "IncisorOfMoonlight" && weapon2 == "EulogyOfTheMoon") ||
                (weapon1 == "EulogyOfTheMoon" && weapon2 == "IncisorOfMoonlight"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/MoonlightSonataDuetResonance.jpg";
            }
            // Check for Dies Irae Duet (Seven Seals)
            else if ((weapon1 == "SevenCircles" && weapon2 == "HellsWrath") ||
                (weapon1 == "HellsWrath" && weapon2 == "SevenCircles"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/DIESIRAEDuetResonance.jpg";
            }
            // Check for Winter Duet (Absolute Zero)
            else if ((weapon1 == "CacophonicBlizzard" && weapon2 == "TheSnowsDesire") ||
                     (weapon1 == "TheSnowsDesire" && weapon2 == "CacophonicBlizzard"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/WINTERDuetResonance.jpg";
            }
            // Check for Eroica Duet (Victory March)
            else if ((weapon1 == "SakurasBlossom" && weapon2 == "FuneralPrayer") ||
                     (weapon1 == "FuneralPrayer" && weapon2 == "SakurasBlossom"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/EroicaDuetResonance.jpg";
            }
            // Check for Swan Lake Duet (Feather Cascade)
            else if ((weapon1 == "StarScatteredWings" && weapon2 == "ThousandWingedSwan") ||
                     (weapon1 == "ThousandWingedSwan" && weapon2 == "StarScatteredWings"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/SwanLakeDuetResonance.jpg";
            }
            // Check for La Campanella Duet (Chime Chain)
            else if ((weapon1 == "SymphonyOfBells" && weapon2 == "RazerOfBellsChimes") ||
                     (weapon1 == "RazerOfBellsChimes" && weapon2 == "SymphonyOfBells"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/LaCampanellaDuetResonance.jpg";
            }
            // Check for Enigma Duet (Cipher Wheel)
            else if ((weapon1 == "CreatorOfMystery" && weapon2 == "Truthseeker") ||
                     (weapon1 == "Truthseeker" && weapon2 == "CreatorOfMystery"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/EnigmaDuetResonance.jpg";
            }
            // Check for Fate Duet (Hourglass Fracture)
            else if ((weapon1 == "AstralChainripper" && weapon2 == "CosmicWeaver") ||
                     (weapon1 == "CosmicWeaver" && weapon2 == "AstralChainripper"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/FateDuetResonance.jpg";
            }
            // Check for Ode to Joy Duet (Orchestra Conductor)
            else if ((weapon1 == "JoyfulCatharsis" && weapon2 == "OdeToCreation") ||
                     (weapon1 == "OdeToCreation" && weapon2 == "JoyfulCatharsis"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Assets/OdeToJoyDuetResonance.jpg";
            }

            OpenDuetAbilityButton.IsVisible = hasCooldownDuet;
            
            // Update image source if a duet is active
            if (hasCooldownDuet && duetImagePath != null)
            {
                var bitmap = MusicClicker.Helpers.ImageHelpers.GetBitmap(duetImagePath, 231);
                if (bitmap != null)
                {
                    DuetAbilityButtonImage.Source = bitmap;
                }
            }
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
        /// Handler for Eulogy of the Moon: Consume Harmonizing Moonlight stacks
        /// </summary>
        public void ConsumeHarmonizingMoonlight_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState == null) return;

            // Check if Eulogy is equipped
            bool eulogyEquipped = gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || 
                                  gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon";
            
            if (!eulogyEquipped)
            {
                return; // Button shouldn't be visible if not equipped, but safety check
            }

            // Check if there are stacks to consume
            if (gameState.HarmonizingMoonlightStacks <= 0)
            {
                return;
            }

            // Consume the stack
            MusicClicker.Armory.WeaponAbilities.EulogyOfTheMoon_ConsumeHarmonizingMoonlight(gameState);
            
            // Update UI
            UIUpdater.UpdateUI(this, gameState);
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
        public async void ShowFloatingText(Point position, string text, Color color, bool hasStroke = false, Color? strokeColor = null)
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
                // Use provided stroke color if specified
                if (strokeColor.HasValue)
                {
                    textBlock.Effect = new DropShadowEffect
                    {
                        Color = strokeColor.Value,
                        BlurRadius = 8,
                        Opacity = 1.0
                    };
                }
                // White text gets dark pink outline (Dawn of Swan's Glory) - legacy fallback
                else if (color == Colors.White)
                {
                    textBlock.Effect = new DropShadowEffect
                    {
                        Color = Color.FromRgb(199, 21, 133), // Dark pink
                        BlurRadius = 8,
                        Opacity = 1.0
                    };
                }
                // Salmon-red or dark pink text gets black outline
                else if (color.R == 250 || color.R == 199) // Salmon-red or dark pink
                {
                    textBlock.Effect = new DropShadowEffect
                    {
                        Color = Colors.Black,
                        BlurRadius = 8,
                        Opacity = 1.0
                    };
                }
                // Black text gets crimson outline (legacy for other effects)
                else if (color == Colors.Black)
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
                MainCrescendanceTitle.Text = "Swan Lake: Wings of Transcendence";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Collect Revered, Chromatic, and Polyphonic feathers. Consume them for powerful bonuses!";
                
            // Hide all other panels
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
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
                
            // Update Thousand Winged Swan timer display
            if (MainThousandWingTimerText != null)
            {
                if (gameState.ThousandWingedSwanNpsBoostActive && DateTime.Now <= gameState.ThousandWingedSwanNpsBoostExpiry)
                {
                    double remainingSeconds = (gameState.ThousandWingedSwanNpsBoostExpiry - DateTime.Now).TotalSeconds;
                    int stacks = gameState.ThousandWingedSwanFeathersConsumed;
                    MainThousandWingTimerText.Text = $"Dawn of Swan's Glory: {remainingSeconds:F1}s ({stacks}/8 stacks)";
                    MainThousandWingTimerText.IsVisible = true;
                }
                else
                {
                    MainThousandWingTimerText.IsVisible = false;
                }
            }
                
            // Enable/disable consume buttons
            if (MainConsumeReveredButton != null)
                MainConsumeReveredButton.IsEnabled = gameState.ReveredFeathers >= 5;
                
            if (MainConsumeChromaticButton != null)
                MainConsumeChromaticButton.IsEnabled = gameState.ChromaticFeathers >= 10;
                
            if (MainConsumePolyphonicButton != null)
            {
                // Always enabled if player has feathers (no max stack blocking)
                MainConsumePolyphonicButton.IsEnabled = gameState.PolyphonicFeathers >= 1;
            }
        }
        
        /// <summary>
        /// Updates Moonlight Sonata stack display on main screen
        /// </summary>
        private void UpdateMainMoonlightCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Moonlight Sonata: Eclipse of the Nocturne";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every 20th click at night (8PM-6AM): Gain Moonbeam Resonance. At 8 stacks: +100% notes + 1 Harmonizing Moonlight.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
            
            if (MainMoonlightStackPanel != null)
                MainMoonlightStackPanel.IsVisible = true;            // Update stack counts
            if (MainMoonbeamResonanceCount != null)
                MainMoonbeamResonanceCount.Text = $"{gameState.MoonbeamResonanceStacks} / 8";
                
            if (MainHarmonizingMoonlightCount != null)
                MainHarmonizingMoonlightCount.Text = gameState.HarmonizingMoonlightStacks.ToString();
                
            // Show/hide Eulogy consume button
            if (ConsumeHarmonizingMoonlightButton != null)
            {
                bool eulogyEquipped = gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || 
                                      gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon";
                bool hasStacks = gameState.HarmonizingMoonlightStacks > 0;
                ConsumeHarmonizingMoonlightButton.IsVisible = eulogyEquipped && hasStacks;
            }
        }
        
        /// <summary>
        /// Updates La Campanella crescendance info (Grandiose Bell)
        /// </summary>
        private void UpdateMainLaCampanellaCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "La Campanella: Grandiose Bell";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Grandiose Bell cracks at 20/40/60 clicks (+5% notes each crack). Mend for rewards: Crescending (+2 minors), Radiant (+5 Deafening Chime, max 15), Harmonizing (gain notes = stacks × NPS).";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
            
            if (MainLaCampanellaStackPanel != null)
                MainLaCampanellaStackPanel.IsVisible = true;            // Update bell stage text
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
                
            // Update Deafening Chime timer
            if (MainDeafeningChimeTimerText != null)
            {
                if (gameState.DeafeningChimeStacks > 0 && DateTime.Now <= gameState.DeafeningChimeExpiry)
                {
                    var remaining = gameState.DeafeningChimeExpiry - DateTime.Now;
                    MainDeafeningChimeTimerText.Text = $"Active: {remaining.TotalSeconds:F1}s";
                    MainDeafeningChimeTimerText.IsVisible = true;
                }
                else
                {
                    MainDeafeningChimeTimerText.IsVisible = false;
                }
            }
        }
        
        /// <summary>
        /// Placeholder for Enigma crescendance info (mystery stacks)
        /// </summary>
        /// <summary>
        /// Updates Enigma crescendance info (Resonant Mystery)
        /// </summary>
        private void UpdateMainEnigmaCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Enigma Variations: Resonant Mystery";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every 10th/15th click: Gain Resonant Mystery. Consume for: +50% notes, +17 entropic, or +1 random owned minor. Bulk consume (10+ stacks): +25% notes per stack.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
            
            if (MainEnigmaStackPanel != null)
                MainEnigmaStackPanel.IsVisible = true;            // Update stack count
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
                MainCrescendanceInfoText.Text = "Every 8th click: +1 Cosmic + 10% notes. Tiers (cumulative): T1 +5 Entropic/click, T2 +5 Melodious+Harmonious/click, T3 +1 Stellar/stack, T4 Symphony on consume (÷2), T5 guaranteed Entropic Crescendo. Consume for +5 Entropic/stack.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainFateStackPanel != null)
                MainFateStackPanel.IsVisible = true;
                
            // Update Cosmic Modulation
            if (MainCosmicModulationCount != null)
                MainCosmicModulationCount.Text = gameState.CosmicModulationStacks.ToString();
                
            // Update Cosmic Modulation tier (1 stack per tier)
            int tier = gameState.CosmicModulationStacks;
            if (MainCosmicModulationTier != null)
                MainCosmicModulationTier.Text = tier.ToString();
                
            if (MainConsumeCosmicButton != null)
                MainConsumeCosmicButton.IsEnabled = gameState.CosmicModulationStacks >= 1;
                
            // Update Symphony of the Stars
            if (MainSymphonyStarsCount != null)
                MainSymphonyStarsCount.Text = gameState.SymphonyOfTheStarsStacks.ToString();
            
            // Show/hide note text based on Astral equipped
            if (MainSymphonyStarsNote != null)
                MainSymphonyStarsNote.IsVisible = !gameState.AstralChainripperAbility;
                
            if (MainConsumeSymphonyButton != null)
            {
                bool canConsume = gameState.AstralChainripperAbility && gameState.SymphonyOfTheStarsStacks >= 1;
                MainConsumeSymphonyButton.IsEnabled = canConsume;
            }
            
            // Update Astral Chainripper NPS boost timer
            if (MainAstralChainripperTimerText != null)
            {
                if (gameState.AstralChainripperNpsBoostActive && DateTime.Now <= gameState.AstralChainripperNpsBoostExpiry)
                {
                    var remaining = gameState.AstralChainripperNpsBoostExpiry - DateTime.Now;
                    MainAstralChainripperTimerText.Text = $"Active: {remaining.TotalSeconds:F1}s";
                    MainAstralChainripperTimerText.IsVisible = true;
                }
                else
                {
                    MainAstralChainripperTimerText.IsVisible = false;
                }
            }
            
            // Update Fate Craft NPS boost timer
            if (MainFateCraftTimerText != null)
            {
                if (DateTime.Now <= gameState.FateCraftNpsBoostExpiry)
                {
                    var remaining = gameState.FateCraftNpsBoostExpiry - DateTime.Now;
                    MainFateCraftTimerText.Text = $"Active: {remaining.TotalSeconds:F1}s";
                    MainFateCraftTimerText.IsVisible = true;
                }
                else
                {
                    MainFateCraftTimerText.IsVisible = false;
                }
            }
        }
        
        /// <summary>
        /// Updates Eroica crescendance info (Heroic Resolve, Symphonic Catharsis, Testament of Harmony)
        /// </summary>
        private void UpdateMainEroicaCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Eroica: Symphony of Triumph";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Symphony of Triumph: Every minor craft: +1 Heroic Resolve. Consume with 25% notes for Symphonic Catharsis (10s double NPC +10% crit).";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
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
                
            // Update Symphonic Catharsis timer
            if (MainCatharsisTimerText != null)
            {
                if (gameState.SymphonicCatharsisActive && DateTime.Now <= gameState.SymphonicCatharsisExpiry)
                {
                    var remaining = gameState.SymphonicCatharsisExpiry - DateTime.Now;
                    MainCatharsisTimerText.Text = $"Active: {remaining.TotalSeconds:F1}s";
                    MainCatharsisTimerText.IsVisible = true;
                }
                else
                {
                    MainCatharsisTimerText.IsVisible = false;
                }
            }
                
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
        /// Updates Dies Irae crescendance info (Dissonant Hatred, Discordant Malice, Cacophonic Oblivion, Wrathful Seal)
        /// </summary>
        private void UpdateMainDiesIraeCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Dies Irae: Wrathful Orchestration";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Every click: +1 Dissonant Hatred (max 50), then Discordant Malice. Consume for infernal power.";
            
            // Update crescendance timer (for duet duration)
            if (MainDiesIraeTimerText != null)
            {
                if (gameState.DiesIraeDuetActive && DateTime.Now <= gameState.DiesIraeDuetExpiry)
                {
                    var remaining = gameState.DiesIraeDuetExpiry - DateTime.Now;
                    MainDiesIraeTimerText.Text = $"Duet Active: {remaining.TotalSeconds:F1}s";
                    MainDiesIraeTimerText.IsVisible = true;
                }
                else
                {
                    MainDiesIraeTimerText.IsVisible = false;
                }
            }
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
                
            if (MainDiesIraeStackPanel != null)
                MainDiesIraeStackPanel.IsVisible = true;
                
            // Update Dissonant Hatred count
            if (MainDissonantHatredCount != null)
                MainDissonantHatredCount.Text = $"{gameState.DissonantHatredStacks} / 50";
                
            if (MainConsumeDissonantHatredButton != null)
                MainConsumeDissonantHatredButton.IsEnabled = gameState.DissonantHatredStacks >= 5;
                
            // Update Discordant Malice count
            if (MainDiscordantMaliceCount != null)
                MainDiscordantMaliceCount.Text = gameState.DiscordantMaliceStacks.ToString();
                
            if (MainConsumeDiscordantButton != null)
                MainConsumeDiscordantButton.IsEnabled = gameState.DiscordantMaliceStacks >= 1;
                
            if (MainConsumeAllDiscordantButton != null)
                MainConsumeAllDiscordantButton.IsEnabled = gameState.DiscordantMaliceStacks >= 1;
                
            // Update Cacophonic Oblivion count
            if (MainCacophonicOblivionCount != null)
                MainCacophonicOblivionCount.Text = gameState.CacophonicOblivionStacks.ToString();
                
            if (MainConsumeCacophonicOblivionButton != null)
                MainConsumeCacophonicOblivionButton.IsEnabled = gameState.CacophonicOblivionStacks >= 1;
                
            // Update Wrathful Seal count
            if (MainWrathfulSealCount != null)
                MainWrathfulSealCount.Text = gameState.WrathfulSealStacks.ToString();
                
            if (MainConsumeWrathfulSealButton != null)
                MainConsumeWrathfulSealButton.IsEnabled = gameState.WrathfulSealStacks >= 1;
                
            // Duet button: Only enabled during active duet with 15+ Wrathful Seals
            if (MainDiesIraeDuetButton != null)
                MainDiesIraeDuetButton.IsEnabled = gameState.DiesIraeDuetActive && gameState.WrathfulSealStacks >= 15;
        }
        
        /// <summary>
        /// Updates Ode to Joy crescendance info (Petals of Harmony/Melody, Ode to Life)
        /// </summary>
        private void UpdateMainOdeToJoyCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Ode to Joy: Petals of Life";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Minor craft: +1 Harmony. Major craft: +1 Melody. Combine 1 Harmony + 1 Melody + 17 Entropic = Ode to Life (doubles all minors).";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainOdeToJoyStackPanel != null)
                MainOdeToJoyStackPanel.IsVisible = true;
                
            // Update Petals of Harmony count
            if (MainPetalsOfHarmonyCount != null)
                MainPetalsOfHarmonyCount.Text = gameState.PetalsOfHarmony.ToString();
                
            if (MainConsumeHarmonyPetalButton != null)
                MainConsumeHarmonyPetalButton.IsEnabled = gameState.PetalsOfHarmony >= 1;
                
            // Update Petals of Melody count
            if (MainPetalsOfMelodyCount != null)
                MainPetalsOfMelodyCount.Text = gameState.PetalsOfMelody.ToString();
                
            // Update Entropic Crescendo timer
            if (MainEntropicCritTimer != null)
            {
                if (DateTime.Now <= gameState.EntropicCritExpiry)
                {
                    var remaining = gameState.EntropicCritExpiry - DateTime.Now;
                    MainEntropicCritTimer.Text = $"Active: {remaining.TotalSeconds:F1}s";
                }
                else
                {
                    MainEntropicCritTimer.Text = "";
                }
            }
                
            if (MainConsumeMelodyPetalButton != null)
                MainConsumeMelodyPetalButton.IsEnabled = gameState.PetalsOfMelody >= 1;
                
            // Update Ode to Creation Double timer
            if (MainOdeToCreationDoubleTimerText != null)
            {
                if (gameState.OdeToCreationDoubleActive && DateTime.Now <= gameState.OdeToCreationDoubleExpiry)
                {
                    var remaining = gameState.OdeToCreationDoubleExpiry - DateTime.Now;
                    MainOdeToCreationDoubleTimerText.Text = $"Active: {remaining.TotalSeconds:F1}s";
                    MainOdeToCreationDoubleTimerText.IsVisible = true;
                }
                else
                {
                    MainOdeToCreationDoubleTimerText.IsVisible = false;
                }
            }
            
            // Update Joyful Catharsis NPS boost timer
            if (MainJoyfulCatharsisTimerText != null)
            {
                if (DateTime.Now <= gameState.JoyfulCatharsisNpsBoostExpiry)
                {
                    var remaining = gameState.JoyfulCatharsisNpsBoostExpiry - DateTime.Now;
                    MainJoyfulCatharsisTimerText.Text = $"Active: {remaining.TotalSeconds:F1}s";
                    MainJoyfulCatharsisTimerText.IsVisible = true;
                }
                else
                {
                    MainJoyfulCatharsisTimerText.IsVisible = false;
                }
            }
                
            // Update Ode to Life count
            if (MainOdeToLifeCount != null)
                MainOdeToLifeCount.Text = gameState.OdeToLifeStacks.ToString();
                
            if (MainCombineForOdeToLifeButton != null)
                MainCombineForOdeToLifeButton.IsEnabled = 
                    gameState.PetalsOfHarmony >= 1 && 
                    gameState.PetalsOfMelody >= 1 && 
                    gameState.EntropicMelodies >= 17;
                    
            if (MainConsumeOdeToLifeButton != null)
                MainConsumeOdeToLifeButton.IsEnabled = gameState.OdeToLifeStacks >= 1;
        }
        
        public void UpdateMainScreenCrescendancePanel()
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
            else if (gameState.CurrentResonatedScore == "OdeToJoy")
            {
                UpdateMainOdeToJoyCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "DiesIrae")
            {
                UpdateMainDiesIraeCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "Winter")
            {
                UpdateMainWinterCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "ClairDeLune")
            {
                UpdateMainClairDeLuneCrescendanceInfo();
            }
            else
            {
                // No crescendance system for this score yet
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
        /// Consume Resonant Mystery for +50% notes
        /// </summary>
        private void MainEnigmaConsumeNotesButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Enigma_ConsumeStack(gameState, "Notes");
            UpdateMainEnigmaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Consume Resonant Mystery for +17 entropic melodies
        /// </summary>
        private void MainEnigmaConsumeEntropicButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Enigma_ConsumeStack(gameState, "Entropic");
            UpdateMainEnigmaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Consume Resonant Mystery for +1 random owned minor
        /// </summary>
        private void MainEnigmaConsumeMinorButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Enigma_ConsumeStack(gameState, "Minor");
            UpdateMainEnigmaCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Consume all Resonant Mystery stacks (10+) for bulk bonus
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
            // Consume all stacks for 5 entropic each
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
        private void MainConsumeDissonantHatredButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.DiesIrae_ConsumeDissonantHatred(gameState);
            UpdateMainDiesIraeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeDiscordantButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.DiesIrae_ConsumeDiscordantMalice(gameState);
            UpdateMainDiesIraeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeAllDiscordantButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.DiesIrae_ConsumeAllDiscordantMalice(gameState);
            UpdateMainDiesIraeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeCacophonicOblivionButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.DiesIrae_ConsumeCacophonicOblivion(gameState);
            UpdateMainDiesIraeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeWrathfulSealButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.DiesIrae_ConsumeWrathfulSeal(gameState);
            UpdateMainDiesIraeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainDiesIraeDuetButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.DiesIrae_ConsumeDuetWrathfulSeals(gameState);
            UpdateMainDiesIraeCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        // Ode to Joy Handlers
        private void MainConsumeHarmonyPetalButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.OdeToJoy_ConsumePetalOfHarmony(gameState);
            UpdateMainOdeToJoyCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeMelodyPetalButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.OdeToJoy_ConsumePetalOfMelody(gameState);
            UpdateMainOdeToJoyCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainCombineForOdeToLifeButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.OdeToJoy_CombineForOdeToLife(gameState);
            UpdateMainOdeToJoyCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeOdeToLifeButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.OdeToJoy_ConsumeOdeToLife(gameState);
            gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
            gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
            UpdateMainOdeToJoyCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        /// <summary>
        /// Updates Winter crescendance info (Frigid Melody, Eternal Frost, Regal Snowlight, Snow's Oblivion)
        /// </summary>
        private void UpdateMainWinterCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Winter: Requiem of the Frozen Choir";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Freeze NPS to gain Frigid Melody. Ignite into Eternal Frost (offensive) or Regal Snowlight (utility). Consume for powerful effects!";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
                
            if (MainWinterStackPanel != null)
                MainWinterStackPanel.IsVisible = true;
                
            // Update Frigid Melody count
            if (MainFrigidMelodyCount != null)
                MainFrigidMelodyCount.Text = gameState.FrigidMelodyStacks.ToString();
                
            // Update Freezing Harmony button (enabled if off cooldown)
            if (MainFreezingHarmonyButton != null)
            {
                bool onCooldown = DateTime.Now < gameState.FreezingHarmonyCooldownExpiry;
                MainFreezingHarmonyButton.IsEnabled = !onCooldown;
                if (onCooldown)
                {
                    double remaining = (gameState.FreezingHarmonyCooldownExpiry - DateTime.Now).TotalSeconds;
                    MainFreezingHarmonyButton.Content = $"Freezing Harmony ({remaining:F1}s)";
                }
                else
                {
                    MainFreezingHarmonyButton.Content = "Freezing Harmony (Freeze NPS)";
                }
            }
                
            // Update Eternal Frost count and buttons
            if (MainEternalFrostCount != null)
                MainEternalFrostCount.Text = gameState.EternalFrostStacks.ToString();
                
            if (MainIgniteEternalFrostButton != null)
                MainIgniteEternalFrostButton.IsEnabled = gameState.FrigidMelodyStacks >= 1;
                
            if (MainConsumeEternalFrostButton != null)
                MainConsumeEternalFrostButton.IsEnabled = gameState.EternalFrostStacks >= 1;
                
            // Update Blizzard Command clicks remaining
            if (MainBlizzardCommandText != null)
            {
                if (gameState.BlizzardCommandClicksRemaining > 0)
                {
                    MainBlizzardCommandText.Text = $"Blizzard Command: {gameState.BlizzardCommandClicksRemaining} clicks";
                    MainBlizzardCommandText.IsVisible = true;
                }
                else
                {
                    MainBlizzardCommandText.IsVisible = false;
                }
            }
                
            // Update Regal Snowlight count and buttons
            if (MainRegalSnowlightCount != null)
                MainRegalSnowlightCount.Text = gameState.RegalSnowlightStacks.ToString();
                
            if (MainIgniteRegalSnowlightButton != null)
                MainIgniteRegalSnowlightButton.IsEnabled = gameState.FrigidMelodyStacks >= 1;
                
            if (MainConsumeRegalSnowlightButton != null)
                MainConsumeRegalSnowlightButton.IsEnabled = gameState.RegalSnowlightStacks >= 1;
                
            // Update Snow's Oblivion count and button
            if (MainSnowsOblivionCount != null)
                MainSnowsOblivionCount.Text = gameState.SnowsOblivionStacks.ToString();
                
            if (MainConsumeSnowsOblivionButton != null)
                MainConsumeSnowsOblivionButton.IsEnabled = gameState.SnowsOblivionStacks >= 1;
                
            // Update Accelerating Flurry display (only if The Snow's Desire equipped)
            if (MainAcceleratingFlurryText != null)
            {
                bool hasSnowsDesire = gameState.CurrentResonatedWeapon1 == "TheSnowsDesire" || 
                                      gameState.CurrentResonatedWeapon2 == "TheSnowsDesire";
                if (hasSnowsDesire && gameState.AcceleratingFlurryBonus > 0)
                {
                    MainAcceleratingFlurryText.Text = $"Accelerating Flurry: +{gameState.AcceleratingFlurryBonus:F0}% NPC";
                    MainAcceleratingFlurryText.IsVisible = true;
                }
                else
                {
                    MainAcceleratingFlurryText.IsVisible = false;
                }
            }
                
            // Update Cacophonic Blizzard bonus clicks (only if Cacophonic Blizzard equipped)
            if (MainCacophonicBlizzardBonusText != null)
            {
                bool hasCacophonicBlizzard = gameState.CurrentResonatedWeapon1 == "CacophonicBlizzard" || 
                                             gameState.CurrentResonatedWeapon2 == "CacophonicBlizzard";
                if (hasCacophonicBlizzard && gameState.CacophonicBlizzardBonusClicksRemaining > 0)
                {
                    MainCacophonicBlizzardBonusText.Text = $"Permafrost Strikes: +50% notes for {gameState.CacophonicBlizzardBonusClicksRemaining} clicks";
                    MainCacophonicBlizzardBonusText.IsVisible = true;
                }
                else
                {
                    MainCacophonicBlizzardBonusText.IsVisible = false;
                }
            }
        }
        
        // Winter Handlers
        private void MainFreezingHarmonyButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Winter_FreezingHarmony(gameState);
            UpdateMainWinterCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainIgniteEternalFrostButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Winter_IgniteToEternalFrost(gameState);
            UpdateMainWinterCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeEternalFrostButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Winter_ConsumeEternalFrost(gameState);
            UpdateMainWinterCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainIgniteRegalSnowlightButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Winter_IgniteToRegalSnowlight(gameState);
            UpdateMainWinterCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeRegalSnowlightButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.Winter_ConsumeRegalSnowlight(gameState);
            UpdateMainWinterCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        private void MainConsumeSnowsOblivionButton_Click(object? sender, RoutedEventArgs e)
        {
            MusicClicker.Armory.WeaponAbilities.CacophonicBlizzard_ConsumeSnowsOblivion(gameState);
            UpdateMainWinterCrescendanceInfo();
            UIUpdater.UpdateUI(this, gameState);
        }
        
        // Clair de Lune Crescendance Info
        public void UpdateMainClairDeLuneCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Clair de Lune: Shattered Time";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Master the clockwork mechanisms. Configure Hour and Minute hands to generate and consume stacks for devastating temporal effects.";
                
            // Hide all other panels
            if (MainSwanFeatherPanel != null) MainSwanFeatherPanel.IsVisible = false;
            if (MainMoonlightStackPanel != null) MainMoonlightStackPanel.IsVisible = false;
            if (MainLaCampanellaStackPanel != null) MainLaCampanellaStackPanel.IsVisible = false;
            if (MainEnigmaStackPanel != null) MainEnigmaStackPanel.IsVisible = false;
            if (MainFateStackPanel != null) MainFateStackPanel.IsVisible = false;
            if (MainEroicaStackPanel != null) MainEroicaStackPanel.IsVisible = false;
            if (MainOdeToJoyStackPanel != null) MainOdeToJoyStackPanel.IsVisible = false;
            if (MainDiesIraeStackPanel != null) MainDiesIraeStackPanel.IsVisible = false;
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
                
            if (MainClairDeLuneStackPanel != null)
                MainClairDeLuneStackPanel.IsVisible = true;
                
            // Update current hand positions
            if (MainCurrentHourHandText != null)
                MainCurrentHourHandText.Text = $"Current: {gameState.ClairDeLuneHourHand}";
                
            if (MainCurrentMinuteHandText != null)
                MainCurrentMinuteHandText.Text = $"Current: {gameState.ClairDeLuneMinuteHand}";
                
            // Update Clockwork Forte count
            if (MainClockworkForteCount != null)
                MainClockworkForteCount.Text = gameState.ClockworkForteStacks.ToString();
                
            // Update Temporal Harmony count
            if (MainTemporalHarmonyCount != null)
                MainTemporalHarmonyCount.Text = $"{gameState.TemporalHarmonyStacks} / 50";
                
            // Update Clock of Eternity count
            if (MainClockOfEternityCount != null)
                MainClockOfEternityCount.Text = gameState.ClockOfEternityStacks.ToString();
        }
        
        // Clair de Lune Clock Configuration Handlers
        private void MainSetHourHand_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string positionStr && int.TryParse(positionStr, out int position))
            {
                gameState.ClairDeLuneHourHand = position;
                UpdateMainClairDeLuneCrescendanceInfo();
            }
        }
        
        private void MainSetMinuteHand_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string positionStr && int.TryParse(positionStr, out int position))
            {
                gameState.ClairDeLuneMinuteHand = position;
                UpdateMainClairDeLuneCrescendanceInfo();
            }
        }
        
        #endregion
        
        #region Hover Tooltip Handlers
        
        private void ShowButtonTooltip(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (sender is Button button && button.Tag is string tooltipText && HoverTooltip != null && HoverTooltipText != null)
            {
                HoverTooltipText.Text = tooltipText;
                HoverTooltip.IsVisible = true;
                UpdateTooltipPosition(sender, e);
            }
        }
        
        private void HideButtonTooltip(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (HoverTooltip != null)
            {
                HoverTooltip.IsVisible = false;
            }
        }
        
        private void UpdateTooltipPosition(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (HoverTooltip != null && HoverTooltip.IsVisible)
            {
                var position = e.GetPosition(this);
                
                // Offset tooltip to appear next to the cursor (to the right and slightly below)
                double offsetX = 20;
                double offsetY = 20;
                
                // Calculate tooltip bounds
                double tooltipWidth = HoverTooltip.Bounds.Width;
                double tooltipHeight = HoverTooltip.Bounds.Height;
                
                // Ensure tooltip doesn't go off-screen
                double left = position.X + offsetX;
                double top = position.Y + offsetY;
                
                if (left + tooltipWidth > this.Bounds.Width)
                    left = position.X - tooltipWidth - 10;
                    
                if (top + tooltipHeight > this.Bounds.Height)
                    top = position.Y - tooltipHeight - 10;
                
                // Position the tooltip
                HoverTooltip.Margin = new Thickness(left, top, 0, 0);
            }
        }
        
        #endregion
    }
}

