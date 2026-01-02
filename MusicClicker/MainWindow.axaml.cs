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
        
        // Background music players (NAudio)
        private NAudio.Wave.IWavePlayer? _waveOut;
        private NAudio.Wave.Mp3FileReader? _audioFileReader;
        private bool _mainMusicPaused = false;
        
        // Cacophonic Dreams music player
        private NAudio.Wave.IWavePlayer? _dreamsWaveOut;
        private NAudio.Wave.Mp3FileReader? _dreamsAudioFileReader;
        
        // Animation flags for falling particle effects
        private bool _swanFeatherAnimationActive = false;
        private bool _eroicaPetalAnimationActive = false;
        private bool _moonbeamShimmerActive = false;


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
                        
                        // Check if Mercury Duet (Swift Delivery) has expired
                        if (gameState.MercuryDuetActive && now > gameState.MercuryDuetExpiry)
                        {
                            // Grant end bonus: NPS × stacks
                            int totalStacks = gameState.SwiftResonanceStacks;
                            double endBonus = gameState.NotesPerSecond * totalStacks;
                            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, endBonus);
                            
                            // Consume all Swift Resonance stacks
                            gameState.SwiftResonanceStacks = 0;
                            
                            // End the duet and start cooldown
                            gameState.MercuryDuetActive = false;
                            gameState.MercuryDuetCooldownExpiry = now.AddMinutes(3); // 3 minute cooldown
                            
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

                        // Mars: No background processing needed for Blade of Symphonic War
                        
                        // Update all visual overlays in a single UI dispatch for better performance
                        // Only update if main screen is visible to avoid unnecessary work
                        if (MainScreen?.IsVisible == true)
                        {
                            Dispatcher.UIThread.Post(() => 
                            {
                                UpdateMarsFlameOverlay();
                                UpdateMoonlightBeamOverlay();
                                UpdateSwanFeatherOverlay();
                                UpdateEroicaPetalOverlay();
                                UpdateLaCampanellaBellOverlay();
                            });
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

                        // Mars: No passive NPS bonuses in Blade of Symphonic War

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
                
                // Hide splash screen after initialization
                HideSplashScreen();
                
                // Start background music
                StartBackgroundMusic();
            });

        }
        
        /// <summary>
        /// Hides the splash screen with a smooth fade-out animation
        /// </summary>
        private async void HideSplashScreen()
        {
            if (SplashScreen == null) return;
            
            // Delay for 5 seconds to display splash screen
            await Task.Delay(5000);
            
            // Fade out animation
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromSeconds(0.8);
            
            while (DateTime.Now - startTime < duration)
            {
                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                var progress = elapsed / duration.TotalSeconds;
                
                // Fade out
                SplashScreen.Opacity = 1.0 - progress;
                
                await Task.Delay(16); // ~60fps
            }
            
            // Hide completely
            SplashScreen.IsVisible = false;
        }
        
        /// <summary>
        /// Starts the background music with looping
        /// </summary>
        private void StartBackgroundMusic()
        {
            try
            {
                // Get the embedded music file
                var assets = Avalonia.Platform.AssetLoader.Open(new Uri("avares://MusicClicker/Gameplay Components/Resources/Music/Main Menu Theme/Symphony in Our Skin.mp3"));
                
                // Create a memory stream to hold the audio data
                var memoryStream = new System.IO.MemoryStream();
                assets.CopyTo(memoryStream);
                memoryStream.Position = 0;
                
                // Initialize NAudio player
                _waveOut = new NAudio.Wave.WaveOutEvent();
                _audioFileReader = new NAudio.Wave.Mp3FileReader(memoryStream);
                
                // Set up looping manually
                _waveOut.Init(_audioFileReader);
                _waveOut.PlaybackStopped += (s, e) =>
                {
                    if (_audioFileReader != null && _waveOut != null && !_mainMusicPaused)
                    {
                        _audioFileReader.Position = 0;
                        _waveOut.Play();
                    }
                };
                
                _waveOut.Play();
                _mainMusicPaused = false;
            }
            catch (Exception ex)
            {
                // Log error but don't crash the game
                Console.WriteLine($"Failed to start background music: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Pauses the main menu background music
        /// </summary>
        private void PauseBackgroundMusic()
        {
            try
            {
                if (_waveOut != null && _waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    _waveOut.Pause();
                    _mainMusicPaused = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to pause background music: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Resumes the main menu background music from where it was paused
        /// </summary>
        private void ResumeBackgroundMusic()
        {
            try
            {
                if (_waveOut != null && _mainMusicPaused)
                {
                    _waveOut.Play();
                    _mainMusicPaused = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to resume background music: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Starts the Cacophonic Dreams music with looping
        /// </summary>
        private void StartDreamsMusic()
        {
            try
            {
                // Get the embedded music file
                var assets = Avalonia.Platform.AssetLoader.Open(new Uri("avares://MusicClicker/Gameplay Components/Resources/Music/Cacophonic Dreams Theme/Resonate With The Stars.mp3"));
                
                // Create a memory stream to hold the audio data
                var memoryStream = new System.IO.MemoryStream();
                assets.CopyTo(memoryStream);
                memoryStream.Position = 0;
                
                // Initialize NAudio player
                _dreamsWaveOut = new NAudio.Wave.WaveOutEvent();
                _dreamsAudioFileReader = new NAudio.Wave.Mp3FileReader(memoryStream);
                
                // Set up looping manually
                _dreamsWaveOut.Init(_dreamsAudioFileReader);
                _dreamsWaveOut.PlaybackStopped += (s, e) =>
                {
                    if (_dreamsAudioFileReader != null && _dreamsWaveOut != null)
                    {
                        _dreamsAudioFileReader.Position = 0;
                        _dreamsWaveOut.Play();
                    }
                };
                
                _dreamsWaveOut.Play();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the game
                Console.WriteLine($"Failed to start Cacophonic Dreams music: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Stops the Cacophonic Dreams music
        /// </summary>
        private void StopDreamsMusic()
        {
            try
            {
                _dreamsWaveOut?.Stop();
                _dreamsWaveOut?.Dispose();
                _dreamsAudioFileReader?.Dispose();
                _dreamsWaveOut = null;
                _dreamsAudioFileReader = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop Cacophonic Dreams music: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Switch to Cacophonic Dreams music (pause main menu music and start Dreams music)
        /// </summary>
        public void SwitchToDreamsMusic()
        {
            PauseBackgroundMusic();
            StartDreamsMusic();
        }
        
        /// <summary>
        /// Switch back to main menu music (stop Dreams music and resume main menu music)
        /// </summary>
        public void SwitchToMainMenuMusic()
        {
            StopDreamsMusic();
            ResumeBackgroundMusic();
        }
        
        /// <summary>
        /// Stops and disposes the background music
        /// </summary>
        private void StopBackgroundMusic()
        {
            try
            {
                _waveOut?.Stop();
                _waveOut?.Dispose();
                _audioFileReader?.Dispose();
                _waveOut = null;
                _audioFileReader = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping background music: {ex.Message}");
            }
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
            
            // Stop and dispose all music
            StopBackgroundMusic();
            StopDreamsMusic();
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
            // Cache DateTime.Now once at start to avoid 15+ system calls per click (performance optimization)
            DateTime clickTime = DateTime.Now;
            
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
            if (gameState.SymphonicCatharsisActive && clickTime > gameState.SymphonicCatharsisExpiry)
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
            
            // Mars: No passive NPC bonuses in Blade of Symphonic War
            
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
            
            // Mercury Weapon 1: Mercurial Overture bonus (NPC×NPC + EM×1000 on every 3rd click)
            if (gameState.MercuryWeapon1NextClickBonus)
            {
                double mercurialBonus = MusicClicker.Armory.WeaponAbilities.Mercury_MercurialOverture_GetClickBonus(gameState);
                notesPerClick += mercurialBonus;
                gameState.MercuryWeapon1NextClickBonus = false;
            }

            // Clair de Lune Duet: Chain of Temporality - apply stacking NPC multiplier
            if (gameState.DuetChainOfTemporalityActive && clickTime < gameState.DuetChainOfTemporalityExpiry)
            {
                notesPerClick *= gameState.DuetChainOfTemporalityNpcMultiplier;
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
            if (gameState.SwanLakeDuetActive && clickTime <= gameState.SwanLakeDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.SwanLakeDuet_OnClick(gameState);
            }

            // La Campanella Duet: Chime Chain click tracking
            if (gameState.LaCampanellaDuetActive && clickTime <= gameState.LaCampanellaDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.LaCampanellaDuet_OnClick(gameState);
            }

            // Enigma Duet: Cipher Wheel segment rewards
            if (gameState.EnigmaDuetActive && clickTime <= gameState.EnigmaDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.EnigmaDuet_OnClick(gameState);
            }

            // Fate Duet: Bank click during banking phase
            if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && clickTime <= gameState.FateDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "Click", notesPerClick);
            }

            // Ode to Joy Duet: Add note to crescendo
            if (gameState.OdeDuetActive && clickTime <= gameState.OdeDuetExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.OdeDuet_AddNote(gameState);
            }
            
            // Clair De Lune Duet: Chain of Temporality
            if (gameState.DuetChainOfTemporalityActive)
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
            
            // Mercury Crescendance: Swift Resonance stacking
            if (gameState.CurrentResonatedScore == "Mercury")
            {
                MusicClicker.Armory.WeaponAbilities.MercuryCrescendance_OnClick(gameState, this);
            }
            
            // Mars Crescendance: Conflagration of War
            // Skip blade charging during Infinite War critical clicks
            if (gameState.CurrentResonatedScore == "Mars" && gameState.InfiniteWarOfHarmonyCritsRemaining <= 0)
            {
                MusicClicker.Armory.WeaponAbilities.MarsCrescendance_OnClick(gameState, this);
            }
            
            // Mercury Weapon 1: Mercurial Overture passive (every 3rd click: NPC×NPC + EM×1000)
            if ((gameState.CurrentResonatedWeapon1 == "MercurialOverture" || gameState.CurrentResonatedWeapon2 == "MercurialOverture"))
            {
                MusicClicker.Armory.WeaponAbilities.Mercury_MercurialOverture_OnClick(gameState);
            }
            
            // Mercury Weapon 2: Wing of the Messenger passive (every 50th click: add EM/5 to lowest minor)
            if ((gameState.CurrentResonatedWeapon1 == "WingOfTheMessenger" || gameState.CurrentResonatedWeapon2 == "WingOfTheMessenger"))
            {
                MusicClicker.Armory.WeaponAbilities.Mercury_WingOfTheMessenger_OnClick(gameState);
            }
            
            // Mars Weapon 1: Fractal of War passive (every 25th click: double EM + +10 all upgrades)
            if ((gameState.CurrentResonatedWeapon1 == "FractalOfWar" || gameState.CurrentResonatedWeapon2 == "FractalOfWar"))
            {
                MusicClicker.Armory.WeaponAbilities.FractalOfWar_OnClick(gameState);
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
                
                // [Old Minute Hand 1 logic removed - now uses time-based system]
                
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
                if (gameState.ThousandWingedSwanNpsBoostActive && clickTime <= gameState.ThousandWingedSwanNpsBoostExpiry)
                {
                    critText = $"Dawn of the Swan's Glory!!! +{FormatNumber(notesPerClick)}";
                    critColor = Colors.White; // White text
                    hasStroke = true; // Will get dark pink outline
                }
                // 2. Dies Irae Duet: Note-Doubling Clicks (from consuming 15+ Wrathful Seals)
                else if (gameState.DiesIraeDuetNoteDoublingClicks > 0)
                {
                    gameState.DiesIraeDuetNoteDoublingClicks--;
                    // NEW FORMULA: NPC × NPS × log₁₀(Notes+1)^8 - scales with progress but prevents exponential doubling
                    double npc = gameState.NotesPerClick;
                    double nps = gameState.NotesPerSecond;
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    double logFactor = Math.Pow(Math.Log10(currentNotes + 1) + 1, 8);
                    finalNotes = npc * nps * logFactor;
                    finalNotes = MusicClicker.Helpers.AtomicDouble.SafeValue(finalNotes);
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
                    // NEW FORMULA: NPC × NPS × log₁₀(NPC×NPS+1)^10 - still powerful but logarithmically scaled
                    double npc = gameState.NotesPerClick;
                    double nps = gameState.NotesPerSecond;
                    double prod = npc * nps;
                    double logFactor = Math.Pow(Math.Log10(prod + 1) + 1, 10);
                    finalNotes = prod * logFactor;
                    finalNotes = MusicClicker.Helpers.AtomicDouble.SafeValue(finalNotes);
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
                // 6.5. Clair De Lune: Infinite Temporality!!! (Clockwork of Infinity consume)
                else if (gameState.InfiniteTemporalityCritsRemaining > 0)
                {
                    gameState.InfiniteTemporalityCritsRemaining--;
                    // Formula: NPC × log₁₀(NPC+1)^8 (nerfed from log^12)
                    double npc = gameState.NotesPerClick;
                    double logFactor = Math.Pow(Math.Log10(npc + 1) + 1, 8);
                    finalNotes = npc * logFactor;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Infinite Temporality!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Colors.Red; // Red font
                    hasStroke = true;
                    strokeColor = Color.FromRgb(64, 64, 64); // Dark gray shadow
                }
                // 6.6. Clair De Lune: Surge of Time's Fractalization!!! (Temporal Fracture consume with Clockwork Finality bond)
                else if (gameState.SurgeOfTimeFractalizationCritsRemaining > 0)
                {
                    gameState.SurgeOfTimeFractalizationCritsRemaining--;
                    // Formula: NPC × NPS × log₁₀(NPC+1)^12 (nerfed from log^24)
                    double npc = gameState.NotesPerClick;
                    double nps = gameState.NotesPerSecond;
                    double logFactor = Math.Pow(Math.Log10(npc + 1) + 1, 12);
                    finalNotes = npc * nps * logFactor;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Surge of Time's Fractalization!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(128, 128, 128); // Gray text
                    hasStroke = true;
                    strokeColor = Colors.Red; // Red shadow
                }
                // 6.7. Mercury: Messenger of Planetary Resonance!!! (Mercurial Overture bond - every Herald's Message consumed)
                else if (gameState.MessengerOfPlanetaryResonanceCritsRemaining > 0)
                {
                    gameState.MessengerOfPlanetaryResonanceCritsRemaining--;
                    // Formula: (NPS × 5,000) + (NPC × log₁₀(NPC+1)^8 × Planetary Alignment stacks)
                    double npc = gameState.NotesPerClick;
                    double logFactor = Math.Pow(Math.Log10(npc + 1) + 1, 8);
                    finalNotes = (gameState.NotesPerSecond * 5000) + (npc * logFactor * gameState.PlanetaryAlignmentStacks);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Messenger of Planetary Resonance!!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(135, 206, 250); // Light sky blue
                    hasStroke = true;
                    strokeColor = Color.FromRgb(70, 130, 180); // Steel blue outline
                }
                // 6.8. Mercury: A Thousand Winged Symphony for the Universe!!! (Wing of the Messenger bond - consume Planetary Alignment)
                else if (gameState.ThousandWingedSymphonyCritsRemaining > 0)
                {
                    gameState.ThousandWingedSymphonyCritsRemaining--;
                    // Formula: NPC × NPS × log10(NPC+1)^50 × (1 + Planetary Alignment stacks) (scaled down from NPC^1000 to prevent overflow)
                    double npc = gameState.NotesPerClick;
                    double nps = gameState.NotesPerSecond;
                    double logFactor = Math.Pow(Math.Log10(npc + 1) + 1, 50);
                    finalNotes = npc * nps * logFactor * (1 + gameState.PlanetaryAlignmentStacks);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"A Thousand Winged Symphony for the Universe!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Colors.White; // White text
                    hasStroke = true;
                    strokeColor = Color.FromRgb(173, 216, 230); // Light blue outline
                }
                // 6.9. Mercury Duet: Alignment of Mercurial Judgement (Swift Delivery active)
                else if (gameState.MercuryDuetActive && clickTime <= gameState.MercuryDuetExpiry)
                {
                    // Every click during Mercury's Haste grants +1 Swift Resonance
                    gameState.SwiftResonanceStacks++;
                    
                    // Calculate crit: NPC × NPS × log₁₀(NPC+1)^stacks (nerfed from (NPC×NPS)^stacks)
                    int stacks = gameState.SwiftResonanceStacks;
                    double npc = gameState.NotesPerClick;
                    double nps = gameState.NotesPerSecond;
                    // Clamp the exponent to avoid overflow - max of 30 for safety
                    double safeExponent = Math.Min(stacks, 30);
                    double logFactor = Math.Pow(Math.Log10(npc + 1) + 1, safeExponent);
                    finalNotes = npc * nps * logFactor;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Alignment of Mercurial Judgement!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(0, 0, 139); // Dark blue
                    hasStroke = true;
                    strokeColor = Colors.White; // White glow
                }
                // 6.9b. Mercury: Messenger of Fate's Melody!!! (old Duet - Fate's Grand Message)
                else if (gameState.MessengerOfFateMelodyCritsRemaining > 0)
                {
                    gameState.MessengerOfFateMelodyCritsRemaining--;
                    // NEW FORMULA: NPS × NPC × log₁₀(NPS+1)^10 - preserves high power scaling but prevents overflow
                    double nps = gameState.NotesPerSecond;
                    double npc = gameState.NotesPerClick;
                    double logFactor = Math.Pow(Math.Log10(nps + 1) + 1, 10);
                    finalNotes = nps * npc * logFactor;
                    finalNotes = MusicClicker.Helpers.AtomicDouble.SafeValue(finalNotes);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Messenger of Fate's Melody!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(192, 192, 192); // Silver/grey
                    hasStroke = true;
                    strokeColor = Color.FromRgb(135, 206, 250); // Light blue outline
                }
                // 6.10. Mars: Infinite War of Harmony!!! (every 5th Resolute Fractal consumed)
                else if (gameState.InfiniteWarOfHarmonyCritsRemaining > 0)
                {
                    gameState.InfiniteWarOfHarmonyCritsRemaining--;
                    
                    // Formula: NPC × (Blade charge %)
                    finalNotes = MusicClicker.Armory.WeaponAbilities.Mars_GetInfiniteWarOfHarmonyCritDamage(gameState);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, finalNotes - notesPerClick);
                    critText = $"Infinite War of Harmony!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(139, 0, 0); // Deep red text
                    hasStroke = true;
                    strokeColor = Colors.Black; // Black outline
                    
                    // Fractal of War Bond: Each Infinite War crit gives +1 Oblivion's Destruction
                    if (gameState.FractalOfWarAbility &&
                        (gameState.CurrentResonatedWeapon1 == "FractalOfWar" || gameState.CurrentResonatedWeapon2 == "FractalOfWar"))
                    {
                        gameState.OblivionsDestructionStacks++;
                    }
                    
                    // After all Infinite War crits are spent, fully deplete the blade
                    if (gameState.InfiniteWarOfHarmonyCritsRemaining == 0)
                    {
                        gameState.BladeOfSymphonicWarCharge = 0;
                    }
                    
                    // Screen shake for Infinite War
                    TriggerScreenShake(1.5, 300);
                }
                // 6.11. Mars Duet: Annihilation Nocturne (time-based, NPC × log-scaled bonus + all majors)
                else if (gameState.MarsDuetActive)
                {
                    // Calculate bonus with logarithmic scaling to prevent overflow
                    double npc = gameState.NotesPerClick;
                    double nps = gameState.NotesPerSecond;
                    double npcNpsRatio = nps > 0 ? npc / nps : 1;
                    double cappedRatio = Math.Min(npcNpsRatio, 10);
                    // Use log-based scaling: NPC × log₁₀(NPC+1)^(cappedRatio) instead of NPC^ratio
                    double logFactor = Math.Pow(Math.Log10(npc + 1) + 1, cappedRatio);
                    double annihilationBonus = npc * logFactor;
                    annihilationBonus = MusicClicker.Helpers.AtomicDouble.SafeValue(annihilationBonus);
                    finalNotes = notesPerClick + annihilationBonus;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, annihilationBonus);
                    
                    // Grant +2 to all owned major scores
                    if (gameState.MoonlightMajorSheets > 0) gameState.MoonlightMajorSheets += 2;
                    if (gameState.DiesIraeMajorSheets > 0) gameState.DiesIraeMajorSheets += 2;
                    if (gameState.WinterMajorSheets > 0) gameState.WinterMajorSheets += 2;
                    if (gameState.EroicaMajorSheets > 0) gameState.EroicaMajorSheets += 2;
                    if (gameState.SwanLakeMajorSheets > 0) gameState.SwanLakeMajorSheets += 2;
                    if (gameState.LaCampanellaMajorSheets > 0) gameState.LaCampanellaMajorSheets += 2;
                    if (gameState.EnigmaMajorSheets > 0) gameState.EnigmaMajorSheets += 2;
                    if (gameState.FateMajorSheets > 0) gameState.FateMajorSheets += 2;
                    if (gameState.OdeToJoyMajorSheets > 0) gameState.OdeToJoyMajorSheets += 2;
                    if (gameState.MercuryMajorOwned > 0) gameState.MercuryMajorOwned += 2;
                    if (gameState.ClairDeLuneMajorOwned > 0) gameState.ClairDeLuneMajorOwned += 2;
                    if (gameState.MarsMajorOwned > 0) gameState.MarsMajorOwned += 2;
                    
                    critText = $"Annihilation Nocturne!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Colors.Black; // Black text
                    hasStroke = true;
                    strokeColor = Color.FromRgb(139, 0, 0); // Deep red outline
                    
                    // Screen shake for Annihilation Nocturne
                    TriggerScreenShake(1.0, 200);
                }
                // 6.12. Mars: Consonance's Requiemic War bonus clicks (NPC^156)
                else if (gameState.ConsonanceRequiemBonusClicksRemaining > 0)
                {
                    gameState.ConsonanceRequiemBonusClicksRemaining--;
                    double consonanceBonus = Math.Pow(gameState.NotesPerClick, 1.56);
                    finalNotes = notesPerClick + consonanceBonus;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, consonanceBonus);
                    critText = $"Requiemic Resonance!!! +{NumberFormatter.FormatLargeNumber(finalNotes)}";
                    critColor = Color.FromRgb(178, 34, 34); // Firebrick red
                    hasStroke = true;
                    strokeColor = Colors.Black; // Black outline
                }
                // 7. Ode to Joy: Entropic Crescendo of Eternity from Petal of Melody (1500x multiplier, time-based)
                else if (clickTime <= gameState.EntropicCritExpiry)
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
                
                // Spawn sparkle particles on click for visual flair
                SpawnSparkleParticles(_lastClickPosition);
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
                gameState.CelestialHorology = true;

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
            RegenerateMajorScoreEffects();
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
            // Check for Mercury Duet (Celestial Messengers)
            else if ((weapon1 == "MercurialOverture" && weapon2 == "WingOfTheMessenger") ||
                     (weapon1 == "WingOfTheMessenger" && weapon2 == "MercurialOverture"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Gameplay Components/Resources/Assets/Major Scores/MercuryMajor.jpg";
            }
            // Check for Clair de Lune Duet (Chain of Temporality)
            else if ((weapon1 == "MetronomicDissonance" && weapon2 == "CelestialHorology") ||
                     (weapon1 == "CelestialHorology" && weapon2 == "MetronomicDissonance"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Gameplay Components/Resources/Assets/Major Scores/ClairDeLuneMajor.jpg";
            }
            // Check for Mars Duet (Annihilation Nocturne)
            else if ((weapon1 == "ConsonanceRequiemicWar" && weapon2 == "FractalOfWar") ||
                     (weapon1 == "FractalOfWar" && weapon2 == "ConsonanceRequiemicWar"))
            {
                hasCooldownDuet = true;
                duetImagePath = "avares://MusicClicker/Gameplay Components/Resources/Assets/Major Scores/MarsMajor.jpg";
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
            
            // Add random offset for visual variety
            Random rng = new Random();
            double offsetX = rng.Next(-20, 21); // -20 to +20 pixels
            double offsetY = rng.Next(-10, 11); // -10 to +10 pixels
            
            Canvas.SetLeft(textBlock, position.X + offsetX);
            Canvas.SetTop(textBlock, position.Y - 30 + offsetY);
            
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
        
        // ------------------- SPARKLE PARTICLE EFFECTS -------------------
        
        /// <summary>
        /// Spawns shiny sparkle particles at the click position.
        /// Creates a burst of pink/hot pink glitter particles that scatter outward with varying sizes and opacities.
        /// </summary>
        public async void SpawnSparkleParticles(Point position, int particleCount = 12)
        {
            if (SparkleParticleCanvas == null) return;
            
            // Pink color palette matching the UI theme
            Color[] sparkleColors = new Color[]
            {
                Color.FromRgb(255, 105, 180), // Hot Pink (#FF69B4) - Main UI accent
                Color.FromRgb(255, 182, 193), // Light Pink
                Color.FromRgb(255, 20, 147),  // Deep Pink
                Color.FromRgb(255, 192, 203), // Pink
                Color.FromRgb(255, 255, 255), // White (for extra shine)
                Color.FromRgb(255, 240, 245)  // Lavender Blush (soft sparkle)
            };
            
            var particles = new List<(Avalonia.Controls.Shapes.Ellipse particle, double vx, double vy, double rotation, double startX, double startY, double baseSize)>();
            
            // Create particles
            for (int i = 0; i < particleCount; i++)
            {
                // Randomize particle properties for variety
                double size = _random.Next(4, 12); // Random size 4-11px
                Color color = sparkleColors[_random.Next(sparkleColors.Length)];
                
                // Calculate random velocity for outward burst
                double angle = _random.NextDouble() * Math.PI * 2; // Random direction (0-360 degrees)
                double speed = _random.Next(80, 200); // Random speed
                double vx = Math.Cos(angle) * speed;
                double vy = Math.Sin(angle) * speed;
                
                // Create sparkle particle (ellipse with gradient for shine effect)
                var particle = new Avalonia.Controls.Shapes.Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = new RadialGradientBrush
                    {
                        GradientStops = new GradientStops
                        {
                            new GradientStop(Colors.White, 0.0),    // Bright white center
                            new GradientStop(color, 0.4),           // Colored mid-ring
                            new GradientStop(Color.FromArgb(200, color.R, color.G, color.B), 0.7), // Softer edge
                            new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1.0)    // Fade out
                        }
                    },
                    Opacity = 1.0
                };
                
                // Add glow effect for extra shine
                particle.Effect = new DropShadowEffect
                {
                    Color = color,
                    BlurRadius = size * 1.5,
                    Opacity = 0.9,
                    OffsetX = 0,
                    OffsetY = 0
                };
                
                // Position at click point with small random offset
                double startX = position.X + _random.Next(-15, 16) - size / 2;
                double startY = position.Y + _random.Next(-15, 16) - size / 2;
                
                Canvas.SetLeft(particle, startX);
                Canvas.SetTop(particle, startY);
                
                SparkleParticleCanvas.Children.Add(particle);
                particles.Add((particle, vx, vy, _random.NextDouble() * 360, startX, startY, size));
            }
            
            // Animate particles
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromSeconds(0.8); // Shorter duration for snappy effect
            
            while (DateTime.Now - startTime < duration)
            {
                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                var progress = elapsed / duration.TotalSeconds;
                
                // Ease out for natural deceleration
                double easedProgress = 1 - Math.Pow(1 - progress, 3); // Cubic ease out
                
                foreach (var (particle, vx, vy, rotation, startX, startY, baseSize) in particles)
                {
                    // Apply gravity and friction
                    double gravity = 150;
                    double friction = 0.95;
                    
                    double currentVx = vx * Math.Pow(friction, elapsed * 60);
                    double currentVy = vy * Math.Pow(friction, elapsed * 60) + gravity * elapsed;
                    
                    double newX = startX + currentVx * elapsed;
                    double newY = startY + currentVy * elapsed;
                    
                    Canvas.SetLeft(particle, newX);
                    Canvas.SetTop(particle, newY);
                    
                    // Fade out and shrink using base size
                    particle.Opacity = Math.Max(0, 1.0 - easedProgress);
                    double scale = 1.0 - easedProgress * 0.5; // Shrink to 50% size
                    particle.Width = baseSize * scale;
                    particle.Height = baseSize * scale;
                    
                    // Add twinkle effect (rapid opacity pulse)
                    if (_random.NextDouble() < 0.3) // 30% chance per frame
                    {
                        particle.Opacity = Math.Min(1.0, particle.Opacity + 0.3);
                    }
                }
                
                await Task.Delay(16); // ~60fps
            }
            
            // Remove all particles
            foreach (var (particle, _, _, _, _, _, _) in particles)
            {
                SparkleParticleCanvas.Children.Remove(particle);
            }
        }
        
        // ------------------- MARS WAR VISUAL EFFECTS -------------------
        
        /// <summary>
        /// Updates the Mars flame overlay visibility based on game state
        /// Shows during Infinite War of Harmony crits
        /// </summary>
        private void UpdateMarsFlameOverlay()
        {
            if (MarsFlameOverlay == null || gameState == null) return;
            
            bool shouldShow = false;
            double intensity = 0.7;
            
            // Show during Infinite War of Harmony crits
            if (gameState.InfiniteWarOfHarmonyCritsRemaining > 0)
            {
                shouldShow = true;
                intensity = 1.0; // Full intensity during Infinite War
            }
            
            MarsFlameOverlay.IsVisible = shouldShow;
            if (shouldShow)
            {
                MarsFlameOverlay.Opacity = intensity;
            }
        }
        
        // ------------------- MOONLIGHT SONATA VISUAL EFFECTS -------------------
        
        /// <summary>
        /// Updates the Moonlight Sonata moonbeam overlay visibility based on game state
        /// Shows when Moonlight Sonata is the current resonated score and on main menu
        /// </summary>
        private void UpdateMoonlightBeamOverlay()
        {
            if (MoonlightBeamOverlay == null || gameState == null) return;
            
            // Show moonbeams only when Moonlight Sonata is resonated AND on main menu
            bool shouldShow = gameState.CurrentResonatedScore == "Moonlight Sonata" && MainScreen?.IsVisible == true;
            
            MoonlightBeamOverlay.IsVisible = shouldShow;
        }
        
        /// <summary>
        /// Generates random moonbeam effects for Moonlight Sonata with subtle shimmer animation
        /// Called when equipping the score to create unique beams each time
        /// </summary>
        public async void GenerateMoonbeamEffects()
        {
            if (MoonlightBeamOverlay == null) return;
            
            // Stop any existing animation
            _moonbeamShimmerActive = false;
            await Task.Delay(50);
            
            // Clear existing beams
            MoonlightBeamOverlay.Children.Clear();
            
            int beamCount = _random.Next(5, 8); // 5-7 beams
            double screenWidth = 1600;
            
            // First add a subtle radial wash
            var wash = new Avalonia.Controls.Border
            {
                Width = 1800,
                Height = 1000,
                Opacity = 0.25,
                Background = new RadialGradientBrush
                {
                    Center = new RelativePoint(0.3, 0.2, RelativeUnit.Relative),
                    GradientOrigin = new RelativePoint(0.3, 0.2, RelativeUnit.Relative),
                    GradientStops = new GradientStops
                    {
                        new GradientStop(Color.Parse("#70E8F0FF"), 0),
                        new GradientStop(Color.Parse("#30D0E0F8"), 0.4),
                        new GradientStop(Color.Parse("#00000000"), 1)
                    }
                }
            };
            Canvas.SetLeft(wash, -100);
            Canvas.SetTop(wash, -50);
            MoonlightBeamOverlay.Children.Add(wash);
            
            // Store beam data for shimmer animation: (element, baseOpacity, shimmerSpeed, shimmerPhase)
            var beams = new List<(Avalonia.Controls.Border element, double baseOpacity, double shimmerSpeed, double shimmerPhase)>();
            
            // Generate random diagonal moonbeams
            for (int i = 0; i < beamCount; i++)
            {
                // Random properties for each beam
                double width = _random.Next(100, 220);    // 100-220px wide beams
                double height = _random.Next(1000, 1400); // Tall beams
                double angle = _random.Next(10, 40);      // 10-40 degrees diagonal
                double opacity = 0.3 + _random.NextDouble() * 0.4; // 0.3-0.7 opacity (brighter)
                double xPos = _random.Next(-100, (int)screenWidth);
                double yPos = _random.Next(-300, -100);
                double shimmerSpeed = 0.3 + _random.NextDouble() * 0.6; // Different shimmer speeds
                double shimmerPhase = _random.NextDouble() * Math.PI * 2; // Random starting phase
                
                // Create moonbeam border with gradient
                var beam = new Avalonia.Controls.Border
                {
                    Width = width,
                    Height = height,
                    Opacity = opacity,
                    RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                    RenderTransform = new RotateTransform(angle),
                    Background = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                        GradientStops = new GradientStops
                        {
                            new GradientStop(Color.Parse("#00C0C8E0"), 0),
                            new GradientStop(Color.Parse("#70E8EEF8"), 0.25),
                            new GradientStop(Color.Parse("#A0FFFFFF"), 0.5), // Brighter center
                            new GradientStop(Color.Parse("#70E8EEF8"), 0.75),
                            new GradientStop(Color.Parse("#00C0C8E0"), 1)
                        }
                    }
                };
                
                Canvas.SetLeft(beam, xPos);
                Canvas.SetTop(beam, yPos);
                MoonlightBeamOverlay.Children.Add(beam);
                beams.Add((beam, opacity, shimmerSpeed, shimmerPhase));
            }
            
            // Start shimmer animation loop
            _moonbeamShimmerActive = true;
            var startTime = DateTime.Now;
            
            while (_moonbeamShimmerActive && MoonlightBeamOverlay?.IsVisible == true)
            {
                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                
                // Animate each beam's opacity for a subtle shimmer
                foreach (var (beam, baseOpacity, shimmerSpeed, shimmerPhase) in beams)
                {
                    // Subtle sine wave shimmer (±15% opacity variation)
                    double shimmer = Math.Sin(elapsed * shimmerSpeed * Math.PI * 2 + shimmerPhase) * 0.15;
                    beam.Opacity = Math.Clamp(baseOpacity + shimmer, 0.15, 0.85);
                }
                
                // Also make the wash pulse very slightly
                double washShimmer = Math.Sin(elapsed * 0.2 * Math.PI * 2) * 0.05;
                wash.Opacity = 0.25 + washShimmer;
                
                await Task.Delay(50); // ~20fps is enough for subtle shimmer
            }
        }
        
        // ------------------- SWAN LAKE VISUAL EFFECTS -------------------
        
        /// <summary>
        /// Updates the Swan Lake feather overlay visibility
        /// Shows when Swan Lake is resonated and on main menu
        /// </summary>
        private void UpdateSwanFeatherOverlay()
        {
            if (SwanFeatherOverlay == null || gameState == null) return;
            
            bool shouldShow = gameState.CurrentResonatedScore == "Swan" && MainScreen?.IsVisible == true;
            SwanFeatherOverlay.IsVisible = shouldShow;
        }
        
        /// <summary>
        /// Generates random swan feather effects for Swan Lake with falling animation
        /// </summary>
        public async void GenerateSwanFeatherEffects()
        {
            if (SwanFeatherOverlay == null) return;
            
            // Stop any existing animation
            _swanFeatherAnimationActive = false;
            await Task.Delay(50); // Brief pause to let previous animation stop
            
            SwanFeatherOverlay.Children.Clear();
            
            int featherCount = _random.Next(10, 16); // 10-15 feathers
            double screenWidth = 1600;
            double screenHeight = 950;
            
            // Store feather data for animation: (element, xPos, yPos, fallSpeed, swaySpeed, swayAmount, rotation, rotationSpeed)
            var feathers = new List<(Avalonia.Controls.Shapes.Ellipse element, double x, double y, double fallSpeed, double swaySpeed, double swayAmount, double baseRotation, double rotationSpeed)>();
            
            for (int i = 0; i < featherCount; i++)
            {
                double size = _random.Next(18, 45);       // Feather size
                double xPos = _random.Next(0, (int)screenWidth);
                double yPos = _random.Next(-200, (int)screenHeight); // Start some above screen
                double opacity = 0.3 + _random.NextDouble() * 0.4; // 0.3-0.7
                double rotation = _random.Next(-60, 60);  // Random tilt
                double fallSpeed = 15 + _random.NextDouble() * 25; // 15-40 pixels per second
                double swaySpeed = 0.5 + _random.NextDouble() * 1.5; // Sway frequency
                double swayAmount = 20 + _random.NextDouble() * 40; // Horizontal sway amount
                double rotationSpeed = (_random.NextDouble() - 0.5) * 30; // -15 to +15 degrees per second
                
                // Create feather shape (elongated ellipse with gradient)
                var feather = new Avalonia.Controls.Shapes.Ellipse
                {
                    Width = size * 0.4,  // Narrow
                    Height = size,       // Elongated
                    Opacity = opacity,
                    RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                    RenderTransform = new RotateTransform(rotation),
                    Fill = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                        GradientStops = new GradientStops
                        {
                            new GradientStop(Color.Parse("#FFFFFFFF"), 0),    // White tip
                            new GradientStop(Color.Parse("#F0F5F5F5"), 0.3),  // Light gray
                            new GradientStop(Color.Parse("#D0ECECEC"), 0.6),
                            new GradientStop(Color.Parse("#A0E0E0E0"), 1)     // Faded base
                        }
                    },
                    Effect = new DropShadowEffect
                    {
                        Color = Colors.White,
                        BlurRadius = 10,
                        Opacity = 0.6,
                        OffsetX = 0,
                        OffsetY = 0
                    }
                };
                
                Canvas.SetLeft(feather, xPos);
                Canvas.SetTop(feather, yPos);
                SwanFeatherOverlay.Children.Add(feather);
                feathers.Add((feather, xPos, yPos, fallSpeed, swaySpeed, swayAmount, rotation, rotationSpeed));
            }
            
            // Start falling animation loop
            _swanFeatherAnimationActive = true;
            var startTime = DateTime.Now;
            
            while (_swanFeatherAnimationActive && SwanFeatherOverlay?.IsVisible == true)
            {
                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                
                for (int i = 0; i < feathers.Count; i++)
                {
                    var (element, baseX, baseY, fallSpeed, swaySpeed, swayAmount, baseRotation, rotationSpeed) = feathers[i];
                    
                    // Calculate new position with gentle falling and swaying
                    double newY = baseY + (fallSpeed * elapsed);
                    double swayOffset = Math.Sin(elapsed * swaySpeed + i) * swayAmount;
                    double newX = baseX + swayOffset;
                    double newRotation = baseRotation + (rotationSpeed * elapsed);
                    
                    // Wrap around when feather goes off bottom
                    if (newY > screenHeight + 50)
                    {
                        newY = -50;
                        baseY = -50 - (fallSpeed * elapsed); // Reset base position
                        baseX = _random.Next(0, (int)screenWidth);
                        feathers[i] = (element, baseX, baseY, fallSpeed, swaySpeed, swayAmount, baseRotation, rotationSpeed);
                    }
                    
                    Canvas.SetLeft(element, newX);
                    Canvas.SetTop(element, newY);
                    element.RenderTransform = new RotateTransform(newRotation);
                }
                
                await Task.Delay(33); // ~30fps
            }
        }
        
        // ------------------- EROICA VISUAL EFFECTS -------------------
        
        /// <summary>
        /// Updates the Eroica cherry blossom overlay visibility
        /// Shows when Eroica is resonated and on main menu
        /// </summary>
        private void UpdateEroicaPetalOverlay()
        {
            if (EroicaPetalOverlay == null || gameState == null) return;
            
            bool shouldShow = gameState.CurrentResonatedScore == "Eroica" && MainScreen?.IsVisible == true;
            EroicaPetalOverlay.IsVisible = shouldShow;
        }
        
        /// <summary>
        /// Generates random cherry blossom petal effects for Eroica with falling animation
        /// </summary>
        public async void GenerateEroicaPetalEffects()
        {
            if (EroicaPetalOverlay == null) return;
            
            // Stop any existing animation
            _eroicaPetalAnimationActive = false;
            await Task.Delay(50); // Brief pause to let previous animation stop
            
            EroicaPetalOverlay.Children.Clear();
            
            int petalCount = _random.Next(18, 28); // 18-27 petals for more density
            double screenWidth = 1600;
            double screenHeight = 950;
            
            Color[] petalColors = new Color[]
            {
                Color.Parse("#FFB7C9"),  // Soft pink
                Color.Parse("#FFAEC9"),  // Rose pink  
                Color.Parse("#FFD1DC"),  // Light pink
                Color.Parse("#FFC0CB"),  // Pink
                Color.Parse("#FFE4E9")   // Pale pink
            };
            
            // Store petal data for animation
            var petals = new List<(Avalonia.Controls.Shapes.Ellipse element, double x, double y, double fallSpeed, double swaySpeed, double swayAmount, double rotation, double spinSpeed)>();
            
            for (int i = 0; i < petalCount; i++)
            {
                double size = _random.Next(12, 28);       // Petal size
                double xPos = _random.Next(0, (int)screenWidth);
                double yPos = _random.Next(-300, (int)screenHeight); // Start some above screen
                double opacity = 0.4 + _random.NextDouble() * 0.35; // 0.4-0.75
                double rotation = _random.Next(0, 360);   // Random rotation
                double fallSpeed = 25 + _random.NextDouble() * 35; // 25-60 pixels per second (faster than feathers)
                double swaySpeed = 1.0 + _random.NextDouble() * 2.0; // Sway frequency
                double swayAmount = 30 + _random.NextDouble() * 50; // Horizontal sway amount
                double spinSpeed = (_random.NextDouble() - 0.5) * 80; // -40 to +40 degrees per second (more spin)
                Color petalColor = petalColors[_random.Next(petalColors.Length)];
                
                // Create petal shape (rounded ellipse)
                var petal = new Avalonia.Controls.Shapes.Ellipse
                {
                    Width = size,
                    Height = size * 0.7,  // Slightly flattened
                    Opacity = opacity,
                    RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                    RenderTransform = new RotateTransform(rotation),
                    Fill = new RadialGradientBrush
                    {
                        GradientStops = new GradientStops
                        {
                            new GradientStop(Color.FromArgb(255, 255, 255, 255), 0),     // White center
                            new GradientStop(petalColor, 0.4),                          // Pink mid
                            new GradientStop(Color.FromArgb(200, petalColor.R, petalColor.G, petalColor.B), 0.7),
                            new GradientStop(Color.FromArgb(80, petalColor.R, petalColor.G, petalColor.B), 1)
                        }
                    },
                    Effect = new DropShadowEffect
                    {
                        Color = petalColor,
                        BlurRadius = 8,
                        Opacity = 0.5,
                        OffsetX = 0,
                        OffsetY = 0
                    }
                };
                
                Canvas.SetLeft(petal, xPos);
                Canvas.SetTop(petal, yPos);
                EroicaPetalOverlay.Children.Add(petal);
                petals.Add((petal, xPos, yPos, fallSpeed, swaySpeed, swayAmount, rotation, spinSpeed));
            }
            
            // Start falling animation loop
            _eroicaPetalAnimationActive = true;
            var startTime = DateTime.Now;
            
            while (_eroicaPetalAnimationActive && EroicaPetalOverlay?.IsVisible == true)
            {
                var elapsed = (DateTime.Now - startTime).TotalSeconds;
                
                for (int i = 0; i < petals.Count; i++)
                {
                    var (element, baseX, baseY, fallSpeed, swaySpeed, swayAmount, baseRotation, spinSpeed) = petals[i];
                    
                    // Calculate new position with gentle falling, swaying, and flutter effect
                    double newY = baseY + (fallSpeed * elapsed);
                    double swayOffset = Math.Sin(elapsed * swaySpeed + i * 0.7) * swayAmount;
                    double flutter = Math.Sin(elapsed * 4 + i) * 5; // Small rapid flutter
                    double newX = baseX + swayOffset + flutter;
                    double newRotation = baseRotation + (spinSpeed * elapsed);
                    
                    // Wrap around when petal goes off bottom
                    if (newY > screenHeight + 50)
                    {
                        newY = -40;
                        baseY = -40 - (fallSpeed * elapsed); // Reset base position
                        baseX = _random.Next(0, (int)screenWidth);
                        petals[i] = (element, baseX, baseY, fallSpeed, swaySpeed, swayAmount, baseRotation, spinSpeed);
                    }
                    
                    Canvas.SetLeft(element, newX);
                    Canvas.SetTop(element, newY);
                    element.RenderTransform = new RotateTransform(newRotation);
                }
                
                await Task.Delay(33); // ~30fps
            }
        }
        
        // ------------------- LA CAMPANELLA VISUAL EFFECTS -------------------
        
        /// <summary>
        /// Updates the La Campanella bell overlay visibility
        /// Shows when La Campanella is resonated and on main menu
        /// </summary>
        private void UpdateLaCampanellaBellOverlay()
        {
            if (LaCampanellaBellOverlay == null || gameState == null) return;
            
            bool shouldShow = gameState.CurrentResonatedScore == "LaCampanella" && MainScreen?.IsVisible == true;
            LaCampanellaBellOverlay.IsVisible = shouldShow;
        }
        
        /// <summary>
        /// Generates random bell/chime effects for La Campanella
        /// </summary>
        public void GenerateLaCampanellaBellEffects()
        {
            if (LaCampanellaBellOverlay == null) return;
            
            LaCampanellaBellOverlay.Children.Clear();
            
            int bellCount = _random.Next(6, 12); // 6-11 bell shapes
            double screenWidth = 1600;
            double screenHeight = 900;
            
            for (int i = 0; i < bellCount; i++)
            {
                double size = _random.Next(12, 30);       // Bell size
                double xPos = _random.Next(0, (int)screenWidth);
                double yPos = _random.Next(0, (int)screenHeight);
                double opacity = 0.25 + _random.NextDouble() * 0.35; // 0.25-0.6
                double rotation = _random.Next(-15, 15);  // Slight tilt
                
                // Create bell shape using a combination of shapes (simplified as golden ellipse)
                var bellBody = new Avalonia.Controls.Shapes.Ellipse
                {
                    Width = size,
                    Height = size * 1.2,  // Bell-like proportions
                    Opacity = opacity,
                    RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                    RenderTransform = new RotateTransform(rotation),
                    Fill = new RadialGradientBrush
                    {
                        Center = new RelativePoint(0.4, 0.3, RelativeUnit.Relative),
                        GradientOrigin = new RelativePoint(0.4, 0.3, RelativeUnit.Relative),
                        GradientStops = new GradientStops
                        {
                            new GradientStop(Color.Parse("#FFFFD700"), 0),      // Gold highlight
                            new GradientStop(Color.Parse("#FFDAA520"), 0.3),    // Goldenrod
                            new GradientStop(Color.Parse("#FFB8860B"), 0.6),    // Dark goldenrod
                            new GradientStop(Color.Parse("#80B8860B"), 1)       // Faded edge
                        }
                    },
                    Effect = new DropShadowEffect
                    {
                        Color = Color.Parse("#FFD700"),
                        BlurRadius = 10,
                        Opacity = 0.6,
                        OffsetX = 0,
                        OffsetY = 0
                    }
                };
                
                Canvas.SetLeft(bellBody, xPos);
                Canvas.SetTop(bellBody, yPos);
                LaCampanellaBellOverlay.Children.Add(bellBody);
                
                // Add small clapper/ringer dot below some bells
                if (_random.NextDouble() > 0.5)
                {
                    var clapper = new Avalonia.Controls.Shapes.Ellipse
                    {
                        Width = size * 0.2,
                        Height = size * 0.2,
                        Opacity = opacity * 0.8,
                        Fill = new SolidColorBrush(Color.Parse("#FFB8860B"))
                    };
                    Canvas.SetLeft(clapper, xPos + size * 0.4);
                    Canvas.SetTop(clapper, yPos + size * 1.1);
                    LaCampanellaBellOverlay.Children.Add(clapper);
                }
            }
        }
        
        /// <summary>
        /// Regenerates all major score visual effects based on current resonated score
        /// Called when equipping a score to create new random effects
        /// </summary>
        public void RegenerateMajorScoreEffects()
        {
            if (gameState == null) return;
            
            string currentScore = gameState.CurrentResonatedScore ?? "None";
            
            // Stop any running animations
            _swanFeatherAnimationActive = false;
            _eroicaPetalAnimationActive = false;
            _moonbeamShimmerActive = false;
            
            // Clear all overlays first
            MoonlightBeamOverlay?.Children.Clear();
            SwanFeatherOverlay?.Children.Clear();
            EroicaPetalOverlay?.Children.Clear();
            LaCampanellaBellOverlay?.Children.Clear();
            
            // Generate effects for the currently resonated score
            switch (currentScore)
            {
                case "Moonlight Sonata":
                    GenerateMoonbeamEffects();
                    break;
                case "Swan":
                    GenerateSwanFeatherEffects();
                    break;
                case "Eroica":
                    GenerateEroicaPetalEffects();
                    break;
                case "LaCampanella":
                    GenerateLaCampanellaBellEffects();
                    break;
            }
            
            // Update visibility
            UpdateMoonlightBeamOverlay();
            UpdateSwanFeatherOverlay();
            UpdateEroicaPetalOverlay();
            UpdateLaCampanellaBellOverlay();
        }
        
        /// <summary>
        /// Triggers a screen shake effect for Mars crits
        /// </summary>
        public async void TriggerScreenShake(double intensity = 1.0, int durationMs = 200)
        {
            if (MainScreen == null) return;
            
            var rng = new Random();
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromMilliseconds(durationMs);
            
            // Save original transform
            var originalTransform = MainScreen.RenderTransform;
            MainScreen.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            
            while (DateTime.Now - startTime < duration)
            {
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                var progress = elapsed / durationMs;
                var decay = 1.0 - progress; // Shake intensity decreases over time
                
                double maxOffset = 6 * intensity * decay;
                double offsetX = (rng.NextDouble() * 2 - 1) * maxOffset;
                double offsetY = (rng.NextDouble() * 2 - 1) * maxOffset;
                
                MainScreen.RenderTransform = new TranslateTransform(offsetX, offsetY);
                
                await Task.Delay(16); // ~60fps
            }
            
            // Reset transform
            MainScreen.RenderTransform = new TranslateTransform(0, 0);
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
            
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
            
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
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
            if (MainWinterStackPanel != null) MainWinterStackPanel.IsVisible = false;
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
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
            else if (gameState.CurrentResonatedScore == "Mercury")
            {
                UpdateMainMercuryCrescendanceInfo();
            }
            else if (gameState.CurrentResonatedScore == "Mars")
            {
                UpdateMainMarsCrescendanceInfo();
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
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
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
                MainCrescendanceTitle.Text = "Clair de Lune: Clockwork Symphony";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Passive: NPC += NPS^5. Time flows through your clicks. Different hours grant different effects: 12-3 (×12K NPC), 3-6 (+Shattered Moonlight), 6-9 (+Clockwork of Infinity), 9-12 (+20% Notes). Consume stacks for powerful bonuses.";
                
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
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
            if (MainClairDeLuneStackPanel != null)
                MainClairDeLuneStackPanel.IsVisible = true;
                
            // Update stack counts for new time-based system
            if (MainShatteredMoonlightCount != null)
                MainShatteredMoonlightCount.Text = gameState.ShatteredMoonlightStacks.ToString();
                
            if (MainClockworkOfInfinityCount != null)
                MainClockworkOfInfinityCount.Text = gameState.ClockworkOfInfinityStacks.ToString();
                
            if (MainTemporalFractureCount != null)
                MainTemporalFractureCount.Text = gameState.TemporalFractureStacks.ToString();
                
            // Update button visibility based on stack availability
            if (MainConsumeShatteredMoonlightButton != null)
                MainConsumeShatteredMoonlightButton.IsEnabled = gameState.ShatteredMoonlightStacks > 0;
                
            if (MainConsumeClockworkOfInfinityButton != null)
                MainConsumeClockworkOfInfinityButton.IsEnabled = gameState.ClockworkOfInfinityStacks > 0;
                
            if (MainConsumeTemporalFractureButton != null)
                MainConsumeTemporalFractureButton.IsEnabled = gameState.TemporalFractureStacks > 0;
        }
        
        // Clair de Lune Consume Button Handlers
        private void MainConsumeShatteredMoonlight_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.ShatteredMoonlightStacks > 0)
            {
                MusicClicker.Armory.WeaponAbilities.ClairConsumeShatteredMoonlight(gameState);
                UpdateMainClairDeLuneCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainConsumeClockworkOfInfinity_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.ClockworkOfInfinityStacks > 0)
            {
                MusicClicker.Armory.WeaponAbilities.ClairConsumeClockworkOfInfinity(gameState);
                UpdateMainClairDeLuneCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainConsumeTemporalFracture_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.TemporalFractureStacks > 0)
            {
                MusicClicker.Armory.WeaponAbilities.ClairConsumeTemporalFracture(gameState);
                UpdateMainClairDeLuneCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        // Mercury Crescendance Update Method
        public void UpdateMainMercuryCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Mercury: The Winged Messenger";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Herald of Fate. Passively gain Swift Resonance every 5 clicks. Transform into Herald's Message for EM, then consume for massive rewards. Weapons grant Planetary Alignment stacks.";
                
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
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMarsCrescendancePanel != null) MainMarsCrescendancePanel.IsVisible = false;
                
            if (MainMercuryCrescendancePanel != null)
                MainMercuryCrescendancePanel.IsVisible = true;
                
            // Update stack counts
            if (MainSwiftResonanceCount != null)
                MainSwiftResonanceCount.Text = gameState.SwiftResonanceStacks.ToString();
                
            if (MainHeraldsMessageCount != null)
                MainHeraldsMessageCount.Text = gameState.HeraldsMessageStacks.ToString();
                
            if (MainPlanetaryAlignmentCount != null)
                MainPlanetaryAlignmentCount.Text = gameState.PlanetaryAlignmentStacks.ToString();
                
            // Update button states
            if (MainMercuryHeraldDescentButton != null)
                MainMercuryHeraldDescentButton.IsEnabled = DateTime.Now >= gameState.MercuryHeraldDescentCooldownExpiry;
                
            if (MainMercuryResonantTransformationButton != null)
                MainMercuryResonantTransformationButton.IsEnabled = gameState.SwiftResonanceStacks > 0;
                
            if (MainMercurySymphonysBountyButton != null)
                MainMercurySymphonysBountyButton.IsEnabled = gameState.HeraldsMessageStacks >= 3;
                
            if (MainConsumePlanetaryAlignmentButton != null)
                MainConsumePlanetaryAlignmentButton.IsEnabled = gameState.PlanetaryAlignmentStacks > 0;
        }
        
        // Mercury Ability Button Handlers
        private void MainMercuryHeraldDescent_Click(object? sender, RoutedEventArgs e)
        {
            if (DateTime.Now >= gameState.MercuryHeraldDescentCooldownExpiry)
            {
                MusicClicker.Armory.WeaponAbilities.Mercury_HeraldsDescent(gameState);
                UpdateMainMercuryCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainMercuryResonantTransformation_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.SwiftResonanceStacks >= 1)
            {
                MusicClicker.Armory.WeaponAbilities.Mercury_ResonantTransformation(gameState);
                UpdateMainMercuryCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainMercurySymphonysBounty_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.HeraldsMessageStacks >= 3)
            {
                MusicClicker.Armory.WeaponAbilities.Mercury_SymphonysBounty(gameState);
                UpdateMainMercuryCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainConsumePlanetaryAlignment_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.PlanetaryAlignmentStacks > 0)
            {
                MusicClicker.Armory.WeaponAbilities.Mercury_ConsumePlanetaryAlignment(gameState);
                UpdateMainMercuryCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        // Mars Crescendance Update Method
        public void UpdateMainMarsCrescendanceInfo()
        {
            if (MainCrescendanceTitle != null)
                MainCrescendanceTitle.Text = "Mars: Bringer of War";
                
            if (MainCrescendanceInfoText != null)
                MainCrescendanceInfoText.Text = "Blade of Symphonic War. Charge the Blade to generate Resolute Fractal. Consume for massive notes and power!";
                
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
            if (MainClairDeLuneStackPanel != null) MainClairDeLuneStackPanel.IsVisible = false;
            if (MainMercuryCrescendancePanel != null) MainMercuryCrescendancePanel.IsVisible = false;
                
            if (MainMarsCrescendancePanel != null)
                MainMarsCrescendancePanel.IsVisible = true;
                
            // Update stack counts
            if (MainResoluteFractalCount != null)
                MainResoluteFractalCount.Text = gameState.ResoluteFractalStacks.ToString();
                
            if (MainInfiniteWarCount != null)
                MainInfiniteWarCount.Text = gameState.InfiniteWarOfHarmonyCritsRemaining.ToString();
                
            if (MainOblivionsDestructionCount != null)
                MainOblivionsDestructionCount.Text = gameState.OblivionsDestructionStacks.ToString();
                
            // Update Blade charge display
            if (MainBladeChargeText != null)
                MainBladeChargeText.Text = $"Blade Charge: {gameState.BladeOfSymphonicWarCharge:F1}%";
                
            // Update button states
            if (MainConsumeResoluteFractalButton != null)
                MainConsumeResoluteFractalButton.IsEnabled = gameState.ResoluteFractalStacks >= 1;
            
            if (MainConsumeAllResoluteFractalButton != null)
                MainConsumeAllResoluteFractalButton.IsEnabled = gameState.ResoluteFractalStacks >= 5; // Only enable if we can get at least one full set
            
            if (MainMergeAllInfiniteWarButton != null)
                MainMergeAllInfiniteWarButton.IsEnabled = gameState.InfiniteWarOfHarmonyCritsRemaining >= 2; // Need at least 2 to merge
                
            if (MainConsumeOblivionsDestructionButton != null)
                MainConsumeOblivionsDestructionButton.IsEnabled = gameState.OblivionsDestructionStacks >= 5;
        }
        
        // Mars Ability Button Handlers
        private void MainConsumeResoluteFractal_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.ResoluteFractalStacks >= 1)
            {
                MusicClicker.Armory.WeaponAbilities.Mars_ConsumeResoluteFractal(gameState);
                UpdateMainMarsCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainConsumeAllResoluteFractal_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.ResoluteFractalStacks >= 1)
            {
                MusicClicker.Armory.WeaponAbilities.Mars_ConsumeAllResoluteFractals(gameState);
                UpdateMainMarsCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainMergeAllInfiniteWar_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.InfiniteWarOfHarmonyCritsRemaining >= 2)
            {
                // Store merged count before consuming
                int mergedCount = gameState.InfiniteWarOfHarmonyCritsRemaining;
                
                // Consume all Infinite War crits
                gameState.InfiniteWarOfHarmonyCritsRemaining = 0;
                gameState.BladeOfSymphonicWarCharge = 0; // Deplete blade
                
                // Calculate Pyroclastic Harmony: NPC × log₁₀(NPC+1)^(mergedCount) with capped exponent
                // Cap exponent at 50 to prevent overflow while still rewarding high merge counts
                double npc = gameState.NotesPerClick;
                double cappedExponent = Math.Min(mergedCount, 50);
                double logFactor = Math.Pow(Math.Log10(npc + 1) + 1, cappedExponent);
                double bonus = npc * logFactor * mergedCount; // Scale linearly with merge count too
                
                // Apply overflow protection
                bonus = MusicClicker.Helpers.AtomicDouble.SafeValue(bonus);
                MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
                
                // Show the special critical text
                ShowFloatingText(_lastClickPosition, 
                    $"Pyroclastic Harmony of War's Eternal Rage!!! +{NumberFormatter.FormatLargeNumber(bonus)}", 
                    Colors.Black, true, Colors.Red);
                
                // Heavy screen shake for this mega-crit
                TriggerScreenShake(2.5, 500);
                
                UpdateMainMarsCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
            }
        }
        
        private void MainConsumeOblivionsDestruction_Click(object? sender, RoutedEventArgs e)
        {
            if (gameState.OblivionsDestructionStacks >= 5)
            {
                MusicClicker.Armory.WeaponAbilities.Mars_ConsumeOblivionsDestruction(gameState);
                UpdateMainMarsCrescendanceInfo();
                UIUpdater.UpdateUI(this, gameState);
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

