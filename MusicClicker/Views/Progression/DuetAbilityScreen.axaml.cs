using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace MusicClicker.Views
{
    public partial class DuetAbilityScreen : UserControl
    {
        private GameState? _gameState;
        private MainWindow? _mainWindow;
        private DispatcherTimer? _cooldownTimer;

        // Reusable brushes and effects to avoid creating new instances every update (performance optimization)
        private static readonly SolidColorBrush _purpleBrush = new SolidColorBrush(Color.FromRgb(138, 43, 226)); // Bright Purple
        private static readonly SolidColorBrush _silverBrush = new SolidColorBrush(Color.FromRgb(192, 192, 220)); // Silver/moonlight
        private static readonly SolidColorBrush _greenBrush = new SolidColorBrush(Color.FromRgb(78, 204, 163)); // Green
        private static readonly SolidColorBrush _moonlightBlueBrush = new SolidColorBrush(Color.FromRgb(100, 120, 200)); // Moonlight blue
        private static readonly DropShadowEffect _moonlightGlowEffect = new DropShadowEffect
        {
            Color = Color.FromRgb(100, 120, 200),
            BlurRadius = 15,
            OffsetX = 0,
            OffsetY = 0,
            Opacity = 0.8
        };

        public DuetAbilityScreen()
        {
            InitializeComponent();

            if (ActivateDuetAbilityButton != null)
                ActivateDuetAbilityButton.Click += OnActivateButtonClicked;

            if (CloseDuetAbilityButton != null)
                CloseDuetAbilityButton.Click += OnCloseButtonClicked;

            _cooldownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // Reduced frequency from 100ms to 500ms for better performance
            };
            _cooldownTimer.Tick += UpdateCooldownDisplay;
        }
        
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            
            // Performance optimization: Only run timer when screen is visible
            if (change.Property.Name == nameof(IsVisible))
            {
                if (IsVisible && _cooldownTimer != null)
                {
                    _cooldownTimer.Start();
                }
                else if (!IsVisible && _cooldownTimer != null)
                {
                    _cooldownTimer.Stop();
                }
            }
        }

        public void Initialize(GameState gameState, MainWindow mainWindow)
        {
            _gameState = gameState;
            _mainWindow = mainWindow;
            UpdateAbilityDisplay();
        }

        public void UpdateAbilityDisplay()
        {
            if (_gameState == null) return;

            string weapon1 = _gameState.CurrentResonatedWeapon1;
            string weapon2 = _gameState.CurrentResonatedWeapon2;

            // Moonlight Duet
            if ((weapon1 == "IncisorOfMoonlight" && weapon2 == "EulogyOfTheMoon") ||
                (weapon1 == "EulogyOfTheMoon" && weapon2 == "IncisorOfMoonlight"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.Moonlight;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.Moonlight;
                ApplyMoonlightTheme();
                UpdateMoonlightStatus();
            }
            // Dies Irae Duet
            else if ((weapon1 == "SevenCircles" && weapon2 == "HellsWrath") ||
                (weapon1 == "HellsWrath" && weapon2 == "SevenCircles"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.DiesIrae;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.DiesIrae;
                ApplyDiesIraeTheme();
                UpdateDiesIraeStatus();
            }
            // Winter Duet
            else if ((weapon1 == "CacophonicBlizzard" && weapon2 == "TheSnowsDesire") ||
                     (weapon1 == "TheSnowsDesire" && weapon2 == "CacophonicBlizzard"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.Winter;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.Winter;
                ApplyWinterTheme();
                UpdateWinterStatus();
            }
            // Eroica Duet
            else if ((weapon1 == "SakurasBlossom" && weapon2 == "FuneralPrayer") ||
                     (weapon1 == "FuneralPrayer" && weapon2 == "SakurasBlossom"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.Eroica;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.Eroica;
                ApplyEroicaTheme();
                UpdateEroicaStatus();
                // Hide other duet visuals
                if (FateHourglassDisplay != null) FateHourglassDisplay.IsVisible = false;
                if (OdeStaffDisplay != null) OdeStaffDisplay.IsVisible = false;
            }
            // Swan Lake Duet
            else if ((weapon1 == "StarScatteredWings" && weapon2 == "ThousandWingedSwan") ||
                     (weapon1 == "ThousandWingedSwan" && weapon2 == "StarScatteredWings"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.SwanLake;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.SwanLake;
                ApplySwanLakeTheme();
                UpdateSwanLakeStatus();
                // Hide other duet visuals
                if (FateHourglassDisplay != null) FateHourglassDisplay.IsVisible = false;
                if (OdeStaffDisplay != null) OdeStaffDisplay.IsVisible = false;
                if (VictoryMarchProgressBorder != null) VictoryMarchProgressBorder.IsVisible = false;
            }
            // La Campanella Duet
            else if ((weapon1 == "SymphonyOfBells" && weapon2 == "RazerOfBellsChimes") ||
                     (weapon1 == "RazerOfBellsChimes" && weapon2 == "SymphonyOfBells"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.LaCampanella;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.LaCampanella;
                ApplyLaCampanellaTheme();
                UpdateLaCampanellaStatus();
                // Hide other duet visuals
                if (FateHourglassDisplay != null) FateHourglassDisplay.IsVisible = false;
                if (OdeStaffDisplay != null) OdeStaffDisplay.IsVisible = false;
                if (VictoryMarchProgressBorder != null) VictoryMarchProgressBorder.IsVisible = false;
            }
            // Enigma Duet
            else if ((weapon1 == "CreatorOfMystery" && weapon2 == "Truthseeker") ||
                     (weapon1 == "Truthseeker" && weapon2 == "CreatorOfMystery"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.Enigma;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.Enigma;
                ApplyEnigmaTheme();
                UpdateEnigmaStatus();
                // Hide other duet visuals
                if (FateHourglassDisplay != null) FateHourglassDisplay.IsVisible = false;
                if (OdeStaffDisplay != null) OdeStaffDisplay.IsVisible = false;
                if (VictoryMarchProgressBorder != null) VictoryMarchProgressBorder.IsVisible = false;
            }
            // Fate Duet
            else if ((weapon1 == "AstralChainripper" && weapon2 == "CosmicWeaver") ||
                     (weapon1 == "CosmicWeaver" && weapon2 == "AstralChainripper"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.Fate;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.Fate;
                ApplyFateTheme();
                UpdateFateStatus();
                // Hide other duet visuals
                if (OdeStaffDisplay != null) OdeStaffDisplay.IsVisible = false;
                if (VictoryMarchProgressBorder != null) VictoryMarchProgressBorder.IsVisible = false;
            }
            // Ode to Joy Duet
            else if ((weapon1 == "JoyfulCatharsis" && weapon2 == "OdeToCreation") ||
                     (weapon1 == "OdeToCreation" && weapon2 == "JoyfulCatharsis"))
            {
                if (DuetAbilityNameText != null)
                    DuetAbilityNameText.Text = DuetDescriptions.Names.OdeToJoy;
                if (DuetAbilityDescriptionText != null)
                    DuetAbilityDescriptionText.Text = DuetDescriptions.Full.OdeToJoy;
                ApplyOdeTheme();
                UpdateOdeStatus();
                // Hide other duet visuals
                if (FateHourglassDisplay != null) FateHourglassDisplay.IsVisible = false;
                if (VictoryMarchProgressBorder != null) VictoryMarchProgressBorder.IsVisible = false;
            }
        }

        private void ApplyMoonlightTheme()
        {
            // Moonlight: Deep blue/silver theme
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 10, 15, 35)); // Dark blue-tinted background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(15, 25, 50)); // Deep midnight blue
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Color.FromRgb(192, 192, 220)); // Silver/moonlight
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(100, 120, 200), 
                    BlurRadius = 15, 
                    Opacity = 0.8 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(100, 120, 200)); // Moonlight blue button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(192, 192, 220), 
                    BlurRadius = 20, 
                    Opacity = 0.6 
                };
            }
            
            if (DuetAbilityNameText != null)
            {
                DuetAbilityNameText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(192, 192, 220), 
                    BlurRadius = 12, 
                    Opacity = 0.7 
                };
            }
        }

        private void ApplyDiesIraeTheme()
        {
            // Dies Irae: Dark red/crimson theme
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 26, 10, 20)); // Dark red-tinted background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(40, 10, 15)); // Deep crimson
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Color.FromRgb(233, 69, 96)); // Bright red
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(150, 0, 0), 
                    BlurRadius = 15, 
                    Opacity = 0.8 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(233, 69, 96)); // Red button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(233, 69, 96), 
                    BlurRadius = 20, 
                    Opacity = 0.6 
                };
            }
            
            if (DuetAbilityNameText != null)
            {
                DuetAbilityNameText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(233, 69, 96), 
                    BlurRadius = 12, 
                    Opacity = 0.7 
                };
            }
        }

        private void ApplySwanLakeTheme()
        {
            // Swan Lake: Elegant white/grey theme
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 240, 240, 245)); // Light white-grey background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(220, 220, 230)); // Light grey title
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 80)); // Dark grey text
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(180, 180, 200), 
                    BlurRadius = 12, 
                    Opacity = 0.6 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(200, 200, 220)); // Light grey button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(160, 160, 180), 
                    BlurRadius = 15, 
                    Opacity = 0.4 
                };
            }
            
            if (DuetAbilityNameText != null)
            {
                DuetAbilityNameText.Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 70)); // Dark grey name
                DuetAbilityNameText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(180, 180, 200), 
                    BlurRadius = 10, 
                    Opacity = 0.5 
                };
            }
            
            if (DuetAbilityDescriptionText != null)
            {
                DuetAbilityDescriptionText.Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 100)); // Medium grey description
            }
        }

        private void ApplyLaCampanellaTheme()
        {
            // La Campanella: Orangeish yellow theme
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 255, 200, 100)); // Orange-yellow background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(255, 180, 60)); // Bright orange title
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Color.FromRgb(80, 40, 0)); // Dark brown text
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(255, 150, 0), 
                    BlurRadius = 15, 
                    Opacity = 0.7 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // Orange button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(255, 200, 100), 
                    BlurRadius = 20, 
                    Opacity = 0.6 
                };
            }
            
            if (DuetAbilityNameText != null)
            {
                DuetAbilityNameText.Foreground = new SolidColorBrush(Color.FromRgb(100, 50, 0)); // Dark orange name
                DuetAbilityNameText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(255, 180, 0), 
                    BlurRadius = 12, 
                    Opacity = 0.6 
                };
            }
            
            if (DuetAbilityDescriptionText != null)
            {
                DuetAbilityDescriptionText.Foreground = new SolidColorBrush(Color.FromRgb(120, 60, 10)); // Medium brown description
            }
        }

        private void ApplyEnigmaTheme()
        {
            // Enigma: Smooth rainbow pulse (dampened colors)
            // Calculate pulse position based on current time (slow cycle)
            double time = DateTime.Now.TimeOfDay.TotalSeconds;
            double hue = (time * 30) % 360; // Complete cycle every 12 seconds
            
            // Convert HSV to RGB with dampened saturation and value
            var (r, g, b) = HsvToRgb(hue, 0.4, 0.6); // Low saturation, medium value for dampened effect
            var (r2, g2, b2) = HsvToRgb(hue, 0.5, 0.5); // Darker version for title
            
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, r, g, b));
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(r2, g2, b2));
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Colors.White);
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(r, g, b), 
                    BlurRadius = 15, 
                    Opacity = 0.5 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb((byte)(r * 0.7), (byte)(g * 0.7), (byte)(b * 0.7)), 
                    BlurRadius = 20, 
                    Opacity = 0.4 
                };
            }
        }

        // Helper method to convert HSV to RGB
        private (byte r, byte g, byte b) HsvToRgb(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = v - c;
            
            double r1, g1, b1;
            if (h < 60) { r1 = c; g1 = x; b1 = 0; }
            else if (h < 120) { r1 = x; g1 = c; b1 = 0; }
            else if (h < 180) { r1 = 0; g1 = c; b1 = x; }
            else if (h < 240) { r1 = 0; g1 = x; b1 = c; }
            else if (h < 300) { r1 = x; g1 = 0; b1 = c; }
            else { r1 = c; g1 = 0; b1 = x; }
            
            return (
                (byte)((r1 + m) * 255),
                (byte)((g1 + m) * 255),
                (byte)((b1 + m) * 255)
            );
        }

        private void ApplyFateTheme()
        {
            // Fate: Purpleish pink theme
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 180, 100, 200)); // Purple-pink background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(150, 60, 180)); // Deep purple title
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 255)); // Light pink text
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(200, 100, 220), 
                    BlurRadius = 15, 
                    Opacity = 0.8 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(200, 100, 220)); // Purple-pink button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(255, 150, 255), 
                    BlurRadius = 20, 
                    Opacity = 0.6 
                };
            }
        }

        private void ApplyOdeTheme()
        {
            // Ode: Monochrome with rainbow accents
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 240, 240, 240)); // Light grey background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)); // Dark grey title
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Colors.White);
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(150, 150, 255), // Rainbow accent
                    BlurRadius = 15, 
                    Opacity = 0.8 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)); // Grey button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(255, 100, 200), // Rainbow accent
                    BlurRadius = 20, 
                    Opacity = 0.7 
                };
            }
            
            if (DuetAbilityNameText != null)
            {
                DuetAbilityNameText.Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30)); // Dark grey name
            }
            
            if (DuetAbilityDescriptionText != null)
            {
                DuetAbilityDescriptionText.Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)); // Medium grey description
            }
        }

        private void ApplyEroicaTheme()
        {
            // Eroica: Deep red/crimson theme
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 30, 10, 10)); // Dark red background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(60, 15, 15)); // Deep crimson
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)); // Deep red
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(180, 30, 30), 
                    BlurRadius = 15, 
                    Opacity = 0.8 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(200, 40, 40)); // Deep red button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(220, 50, 50), 
                    BlurRadius = 20, 
                    Opacity = 0.6 
                };
            }
            
            if (DuetAbilityNameText != null)
            {
                DuetAbilityNameText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(220, 50, 50), 
                    BlurRadius = 12, 
                    Opacity = 0.7 
                };
            }
        }

        private void ApplyWinterTheme()
        {
            // Winter: Light blue/icy theme
            if (MainBorder != null)
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(221, 15, 25, 40)); // Dark blue-tinted background
            
            if (TitleBorder != null)
                TitleBorder.Background = new SolidColorBrush(Color.FromRgb(20, 35, 60)); // Deep icy blue
            
            if (TitleText != null)
            {
                TitleText.Foreground = new SolidColorBrush(Color.FromRgb(135, 206, 250)); // Light sky blue
                TitleText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(0, 100, 200), 
                    BlurRadius = 15, 
                    Opacity = 0.8 
                };
            }
            
            if (ActivateButtonBorder != null)
            {
                ActivateButtonBorder.Background = new SolidColorBrush(Color.FromRgb(100, 180, 255)); // Bright blue button
                ActivateButtonBorder.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(135, 206, 250), 
                    BlurRadius = 20, 
                    Opacity = 0.6 
                };
            }
            
            if (DuetAbilityNameText != null)
            {
                DuetAbilityNameText.Effect = new DropShadowEffect 
                { 
                    Color = Color.FromRgb(135, 206, 250), 
                    BlurRadius = 12, 
                    Opacity = 0.7 
                };
            }
        }

        private void UpdateDiesIraeStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.DiesIraeDuetActive)
            {
                double remaining = (_gameState.DiesIraeDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    DuetAbilityStatusText.Text = $"Active: {remaining:F1}s remaining | Seals: {_gameState.SevenSealsCounter}/7";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // Gold
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163));
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
            }
            else if (_gameState.DiesIraeDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.DiesIraeDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s | Seals: {_gameState.SevenSealsCounter}/7";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(233, 69, 96)); // Red
                ActivateDuetAbilityButton.IsEnabled = false;
            }
            else
            {
                DuetAbilityStatusText.Text = $"Ready | Seals: {_gameState.SevenSealsCounter}/7";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163)); // Green
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;
            }
        }

        private void UpdateMoonlightStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            // Check if all phases are active (resonating with Moonlight Sonata)
            bool allPhasesActive = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_AreAllPhasesActive(_gameState);
            
            // Get current phase name for display
            string phaseName = _gameState.MoonlightCurrentPhase switch
            {
                0 => "New Moon",
                1 => "Crescent",
                2 => "Full Moon",
                3 => "Waning",
                _ => "Unknown"
            };

            if (_gameState.MoonlightDuetActive)
            {
                double remaining = (_gameState.MoonlightDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    // If resonating with Moonlight Sonata, show SPECIAL status
                    if (allPhasesActive)
                    {
                        DuetAbilityStatusText.Text = $"SPECIAL: All Phases Active! | {remaining:F1}s";
                        DuetAbilityStatusText.Foreground = _silverBrush;
                        DuetAbilityStatusText.Effect = _moonlightGlowEffect;
                    }
                    else
                    {
                        DuetAbilityStatusText.Text = $"Active: {remaining:F1}s | Phase: {phaseName}";
                        DuetAbilityStatusText.Foreground = _silverBrush;
                        DuetAbilityStatusText.Effect = _moonlightGlowEffect;
                    }
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = _greenBrush;
                    DuetAbilityStatusText.Effect = null;
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
            }
            else if (_gameState.MoonlightDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.MoonlightDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = _silverBrush;
                DuetAbilityStatusText.Effect = _moonlightGlowEffect;
                ActivateDuetAbilityButton.Content = "DISABLED";
                ActivateDuetAbilityButton.IsEnabled = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = _greenBrush;
                DuetAbilityStatusText.Effect = null;
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;
            }
        }

        private void UpdateEroicaStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.EroicaDuetActive)
            {
                int progress = _gameState.VictoryMarchClicks;
                string nextReward = "";
                
                if (!_gameState.VictoryMarch25Claimed)
                    nextReward = "Next: 1 Minor Score (25%)";
                else if (!_gameState.VictoryMarch50Claimed)
                    nextReward = "Next: 1 Major Score (50%)";
                else if (!_gameState.VictoryMarch75Claimed)
                    nextReward = "Next: +50 Fragments (75%)";
                else if (!_gameState.VictoryMarch100Claimed)
                    nextReward = "Next: Double Scores (100%)";
                else
                    nextReward = "Complete!";
                
                DuetAbilityStatusText.Text = nextReward;
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // Gold
                ActivateDuetAbilityButton.Content = "END RESONANCE";
                ActivateDuetAbilityButton.IsEnabled = true;

                // Update progress bar
                if (VictoryMarchProgressBorder != null)
                    VictoryMarchProgressBorder.IsVisible = true;
                if (VictoryMarchProgressFill != null)
                    VictoryMarchProgressFill.Width = (progress / 100.0) * 340; // Max width is 340
                if (VictoryMarchProgressText != null)
                    VictoryMarchProgressText.Text = $"{progress} / 100";
            }
            else if (_gameState.EroicaDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.EroicaDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 50)); // Orange-red
                ActivateDuetAbilityButton.IsEnabled = false;

                // Hide progress bar
                if (VictoryMarchProgressBorder != null)
                    VictoryMarchProgressBorder.IsVisible = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163)); // Green
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;

                // Hide progress bar
                if (VictoryMarchProgressBorder != null)
                    VictoryMarchProgressBorder.IsVisible = false;
            }
        }

        private void UpdateSwanLakeStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.SwanLakeDuetActive)
            {
                double remaining = (_gameState.SwanLakeDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    int clickCount = _gameState.SwanLakeDuetClickCounter;
                    DuetAbilityStatusText.Text = $"Active: {remaining:F1}s | Clicks: {clickCount}/10";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(70, 70, 90)); // Dark grey
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163));
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
            }
            else if (_gameState.SwanLakeDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.SwanLakeDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(130, 130, 150)); // Medium grey
                ActivateDuetAbilityButton.IsEnabled = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163)); // Green
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;
            }
        }

        private void UpdateLaCampanellaStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.LaCampanellaDuetActive)
            {
                double remaining = (_gameState.LaCampanellaDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    DuetAbilityStatusText.Text = $"Active: {remaining:F1}s | Chain: {_gameState.ChimeChainLength}";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(100, 50, 0));
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
            }
            else if (_gameState.LaCampanellaDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.LaCampanellaDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(150, 100, 50));
                ActivateDuetAbilityButton.IsEnabled = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;
            }
        }

        private void UpdateEnigmaStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.EnigmaDuetActive)
            {
                double remaining = (_gameState.EnigmaDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    DuetAbilityStatusText.Text = $"Active: {remaining:F1}s | Mysteries: {_gameState.EnigmaMysteryClickCount}";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 150));
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 200));
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
            }
            else if (_gameState.EnigmaDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.EnigmaDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(120, 120, 150));
                ActivateDuetAbilityButton.IsEnabled = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 200));
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;
            }
        }

        private void UpdateFateStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.FateDuetActive)
            {
                double remaining = (_gameState.FateDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    string phase = _gameState.FateDuetHasFlipped ? "REPLAY" : "BANKING";
                    int bankedActions = _gameState.HourglassActionBank.Count;
                    DuetAbilityStatusText.Text = $"{phase}: {remaining:F1}s | Banked: {bankedActions}";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(200, 100, 220));
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;

                    // Update hourglass visual
                    UpdateFateHourglassDisplay();
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(200, 100, 220));
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;

                    // Hide hourglass visual
                    if (FateHourglassDisplay != null)
                        FateHourglassDisplay.IsVisible = false;
                }
            }
            else if (_gameState.FateDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.FateDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(150, 100, 150));
                ActivateDuetAbilityButton.IsEnabled = false;

                // Hide hourglass visual
                if (FateHourglassDisplay != null)
                    FateHourglassDisplay.IsVisible = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(200, 100, 220));
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;

                // Hide hourglass visual
                if (FateHourglassDisplay != null)
                    FateHourglassDisplay.IsVisible = false;
            }
        }

        private void UpdateFateHourglassDisplay()
        {
            if (FateHourglassDisplay == null || FatePhaseText == null || FateBankedActionsText == null) return;

            FateHourglassDisplay.IsVisible = true;
            
            if (_gameState.FateDuetHasFlipped)
            {
                FatePhaseText.Text = "⚡ REPLAY PHASE ⚡";
                FatePhaseText.Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 255)); // Bright purple
                if (FateHourglassArt != null)
                    FateHourglassArt.Text = ""; // Hourglass symbol removed
            }
            else
            {
                FatePhaseText.Text = "📥 BANKING PHASE";
                FatePhaseText.Foreground = new SolidColorBrush(Color.FromRgb(200, 100, 220)); // Purple
                if (FateHourglassArt != null)
                    FateHourglassArt.Text = "⌛"; // Normal hourglass
            }

            FateBankedActionsText.Text = $"Actions Banked: {_gameState.HourglassActionBank.Count}";
        }

        private void UpdateOdeStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.OdeDuetActive)
            {
                double remaining = (_gameState.OdeDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    DuetAbilityStatusText.Text = $"Active: {remaining:F1}s | Notes: {_gameState.CrescendoNotesPlaced}/16 | Sections: {_gameState.CrescendoCompletedSections}";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;

                    // Update staff visual
                    UpdateOdeStaffDisplay();
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;

                    // Hide staff visual
                    if (OdeStaffDisplay != null)
                        OdeStaffDisplay.IsVisible = false;
                }
            }
            else if (_gameState.OdeDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.OdeDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                ActivateDuetAbilityButton.IsEnabled = false;

                // Hide staff visual
                if (OdeStaffDisplay != null)
                    OdeStaffDisplay.IsVisible = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;

                // Hide staff visual
                if (OdeStaffDisplay != null)
                    OdeStaffDisplay.IsVisible = false;
            }
        }

        private void UpdateOdeStaffDisplay()
        {
            if (OdeStaffDisplay == null || OdeNotesPanel == null || OdeNotesCountText == null) return;

            OdeStaffDisplay.IsVisible = true;
            
            // Update notes counter with completed sections
            int noteCount = _gameState.CrescendoNotesPlaced;
            int completedSections = _gameState.CrescendoCompletedSections;
            OdeNotesCountText.Text = $"Notes: {noteCount} / 16 | Sections: {completedSections}";

            // Add notes to staff (only if we need to add more)
            int currentNoteElements = OdeNotesPanel.Children.Count;
            if (noteCount > currentNoteElements)
            {
                // Reuse MainWindow's Random instance for performance (avoid creating new instance)
                Random rng = _mainWindow?._random ?? new Random();
                for (int i = currentNoteElements; i < noteCount; i++)
                {
                    // Randomize vertical position (0-4 for 5 staff lines)
                    int linePosition = rng.Next(5);
                    double topMargin = 10 + (linePosition * 20); // Each line is 20px apart
                    
                    var noteBlock = new TextBlock
                    {
                        Text = "•",
                        FontSize = 24,
                        Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)),
                        Margin = new Thickness(2, topMargin, 2, 0),
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
                    };

                    // Add glow effect at milestone notes (4, 8, 12, 16)
                    if ((i + 1) % 4 == 0)
                    {
                        noteBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // Gold
                        noteBlock.FontWeight = Avalonia.Media.FontWeight.Bold;
                    }

                    OdeNotesPanel.Children.Add(noteBlock);
                }
            }
            else if (noteCount < currentNoteElements)
            {
                // Clear notes if count decreased (cycle completed and reset)
                OdeNotesPanel.Children.Clear();
            }
        }

        private void UpdateWinterStatus()
        {
            if (_gameState == null || DuetAbilityStatusText == null || ActivateDuetAbilityButton == null) return;

            if (_gameState.WinterDuetActive)
            {
                double remaining = (_gameState.WinterDuetExpiry - DateTime.Now).TotalSeconds;
                if (remaining > 0)
                {
                    DuetAbilityStatusText.Text = $"Active: {remaining:F1}s remaining";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(135, 206, 250)); // Light blue
                    ActivateDuetAbilityButton.Content = "END RESONANCE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
                else
                {
                    DuetAbilityStatusText.Text = "Ready";
                    DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163));
                    ActivateDuetAbilityButton.Content = "RESONATE";
                    ActivateDuetAbilityButton.IsEnabled = true;
                }
            }
            else if (_gameState.WinterDuetCooldownExpiry > DateTime.Now)
            {
                double cooldown = (_gameState.WinterDuetCooldownExpiry - DateTime.Now).TotalSeconds;
                DuetAbilityStatusText.Text = $"Cooldown: {cooldown:F1}s";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(233, 69, 96)); // Red
                ActivateDuetAbilityButton.Content = "DISABLED";
                ActivateDuetAbilityButton.IsEnabled = false;
            }
            else
            {
                DuetAbilityStatusText.Text = "Ready";
                DuetAbilityStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 204, 163)); // Green
                ActivateDuetAbilityButton.Content = "RESONATE";
                ActivateDuetAbilityButton.IsEnabled = true;
            }
        }

        private void OnActivateButtonClicked(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;

            string weapon1 = _gameState.CurrentResonatedWeapon1;
            string weapon2 = _gameState.CurrentResonatedWeapon2;

            // Moonlight Duet
            if ((weapon1 == "IncisorOfMoonlight" && weapon2 == "EulogyOfTheMoon") ||
                (weapon1 == "EulogyOfTheMoon" && weapon2 == "IncisorOfMoonlight"))
            {
                // Check if on cooldown
                if (_gameState.MoonlightDuetCooldownExpiry > DateTime.Now)
                {
                    // Still on cooldown, do nothing
                    return;
                }
                
                if (_gameState.MoonlightDuetActive)
                {
                    // Deactivate and start cooldown
                    _gameState.MoonlightDuetActive = false;
                    _gameState.MoonlightDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.Moonlight);
                    _cooldownTimer?.Start();
                }
                else
                {
                    // Activate
                    _gameState.MoonlightDuetActive = true;
                    _gameState.MoonlightDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.Moonlight);
                    _gameState.MoonlightCurrentPhase = 0; // Start with New Moon
                    _gameState.MoonlightPhaseChangeTime = DateTime.Now; // Track phase start time
                }
            }
            // Dies Irae Duet
            else if ((weapon1 == "SevenCircles" && weapon2 == "HellsWrath") ||
                (weapon1 == "HellsWrath" && weapon2 == "SevenCircles"))
            {
                if (_gameState.DiesIraeDuetActive)
                {
                    // Deactivate
                    _gameState.DiesIraeDuetActive = false;
                    _gameState.DiesIraeDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.DiesIrae);
                    _gameState.SevenSealsCounter = 0;
                }
                else if (_gameState.DiesIraeDuetCooldownExpiry <= DateTime.Now)
                {
                    // Activate
                    _gameState.DiesIraeDuetActive = true;
                    _gameState.DiesIraeDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.DiesIrae);
                    _gameState.SevenSealsCounter = 0;
                    _cooldownTimer?.Start();
                }
            }
            // Winter Duet
            else if ((weapon1 == "CacophonicBlizzard" && weapon2 == "TheSnowsDesire") ||
                     (weapon1 == "TheSnowsDesire" && weapon2 == "CacophonicBlizzard"))
            {
                if (_gameState.WinterDuetActive)
                {
                    // Deactivate
                    _gameState.WinterDuetActive = false;
                    _gameState.WinterDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.Winter);
                }
                else if (_gameState.WinterDuetCooldownExpiry <= DateTime.Now)
                {
                    // Activate immediately and freeze NPS for 20 seconds
                    _gameState.WinterDuetActive = true;
                    _gameState.WinterDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.Winter);
                    _gameState.WinterDuetExtensionTime = 0; // Reset extension counter to 0
                    
                    // Freeze NPS for 20 seconds (matching duet duration)
                    _gameState.NpsFrozen = true;
                    _gameState.FrozenNpsValue = _gameState.NotesPerSecond;
                    _gameState.NpsFreezeExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.Winter);
                    
                    _cooldownTimer?.Start();
                }
            }
            // Eroica Duet
            else if ((weapon1 == "SakurasBlossom" && weapon2 == "FuneralPrayer") ||
                     (weapon1 == "FuneralPrayer" && weapon2 == "SakurasBlossom"))
            {
                // Check if on cooldown
                if (_gameState.EroicaDuetCooldownExpiry > DateTime.Now)
                {
                    // Still on cooldown, do nothing
                    return;
                }
                
                if (_gameState.EroicaDuetActive)
                {
                    // Deactivate and start cooldown
                    _gameState.EroicaDuetActive = false;
                    _gameState.EroicaDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.Eroica);
                    _gameState.VictoryMarchClicks = 0;
                    _gameState.VictoryMarch25Claimed = false;
                    _gameState.VictoryMarch50Claimed = false;
                    _gameState.VictoryMarch75Claimed = false;
                    _gameState.VictoryMarch100Claimed = false;
                    _cooldownTimer?.Start();
                }
                else
                {
                    // Activate
                    _gameState.EroicaDuetActive = true;
                    _gameState.VictoryMarchClicks = 0;
                    _gameState.VictoryMarch25Claimed = false;
                    _gameState.VictoryMarch50Claimed = false;
                    _gameState.VictoryMarch75Claimed = false;
                    _gameState.VictoryMarch100Claimed = false;
                }
            }
            // Swan Lake Duet
            else if ((weapon1 == "StarScatteredWings" && weapon2 == "ThousandWingedSwan") ||
                     (weapon1 == "ThousandWingedSwan" && weapon2 == "StarScatteredWings"))
            {
                // Check if on cooldown
                if (_gameState.SwanLakeDuetCooldownExpiry > DateTime.Now)
                {
                    // Still on cooldown, do nothing
                    return;
                }
                
                if (_gameState.SwanLakeDuetActive)
                {
                    // Deactivate and start cooldown
                    _gameState.SwanLakeDuetActive = false;
                    _gameState.SwanLakeDuetClickCounter = 0;
                    _gameState.SwanLakeDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.SwanLake);
                    _cooldownTimer?.Start();
                }
                else
                {
                    // Activate
                    _gameState.SwanLakeDuetActive = true;
                    _gameState.SwanLakeDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.SwanLake);
                    _gameState.SwanLakeDuetClickCounter = 0; // Reset click counter
                }
            }

            // La Campanella Duet
            else if ((weapon1 == "SymphonyOfBells" && weapon2 == "RazerOfBellsChimes") ||
                     (weapon1 == "RazerOfBellsChimes" && weapon2 == "SymphonyOfBells"))
            {
                // Check if on cooldown
                if (_gameState.LaCampanellaDuetCooldownExpiry > DateTime.Now)
                {
                    // Still on cooldown, do nothing
                    return;
                }
                
                if (_gameState.LaCampanellaDuetActive)
                {
                    // Deactivate, clear chain, and start cooldown
                    _gameState.LaCampanellaDuetActive = false;
                    _gameState.ChimeChainLength = 0;
                    _gameState.LastChimeClickTime = DateTime.MinValue;
                    _gameState.LaCampanellaDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.LaCampanella);
                    _cooldownTimer?.Start();
                }
                else
                {
                    // Activate
                    _gameState.LaCampanellaDuetActive = true;
                    _gameState.LaCampanellaDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.LaCampanella);
                    _gameState.ChimeChainLength = 0;
                    _gameState.LastChimeClickTime = DateTime.MinValue;
                }
            }

            // Enigma Duet
            else if ((weapon1 == "CreatorOfMystery" && weapon2 == "Truthseeker") ||
                     (weapon1 == "Truthseeker" && weapon2 == "CreatorOfMystery"))
            {
                // Check if on cooldown
                if (_gameState.EnigmaDuetCooldownExpiry > DateTime.Now)
                {
                    // Still on cooldown, do nothing
                    return;
                }
                
                if (_gameState.EnigmaDuetActive)
                {
                    // Deactivate and start cooldown
                    _gameState.EnigmaDuetActive = false;
                    _gameState.EnigmaMysteryClickCount = 0;
                    _gameState.EnigmaDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.Enigma);
                    _cooldownTimer?.Start();
                }
                else
                {
                    // Activate
                    _gameState.EnigmaDuetActive = true;
                    _gameState.EnigmaDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.Enigma);
                    _gameState.EnigmaMysteryClickCount = 0;
                }
            }

            // Fate Duet
            else if ((weapon1 == "AstralChainripper" && weapon2 == "CosmicWeaver") ||
                     (weapon1 == "CosmicWeaver" && weapon2 == "AstralChainripper"))
            {
                // Check if on cooldown
                if (_gameState.FateDuetCooldownExpiry > DateTime.Now)
                {
                    // Still on cooldown, do nothing
                    return;
                }

                if (_gameState.FateDuetActive)
                {
                    // Deactivate and start cooldown
                    _gameState.FateDuetActive = false;
                    _gameState.FateDuetHasFlipped = false;
                    _gameState.HourglassActionBank.Clear();
                    _gameState.FateDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.Fate);
                    _cooldownTimer?.Start();
                }
                else
                {
                    // Activate
                    _gameState.FateDuetActive = true;
                    _gameState.FateDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.Fate);
                    _gameState.FateDuetFlipTime = DateTime.Now.AddSeconds(10); // Flip at 10s
                    _gameState.FateDuetHasFlipped = false;
                    _gameState.HourglassActionBank.Clear();
                }
            }

            // Ode to Joy Duet
            else if ((weapon1 == "JoyfulCatharsis" && weapon2 == "OdeToCreation") ||
                     (weapon1 == "OdeToCreation" && weapon2 == "JoyfulCatharsis"))
            {
                // Check if on cooldown
                if (_gameState.OdeDuetCooldownExpiry > DateTime.Now)
                {
                    // Still on cooldown, do nothing
                    return;
                }

                if (_gameState.OdeDuetActive)
                {
                    // Deactivate and start cooldown
                    _gameState.OdeDuetActive = false;
                    _gameState.CrescendoNotesPlaced = 0;
                    _gameState.Crescendo4Claimed = false;
                    _gameState.Crescendo8Claimed = false;
                    _gameState.Crescendo12Claimed = false;
                    _gameState.Crescendo16Claimed = false;
                    _gameState.CrescendoCompletedSections = 0;
                    _gameState.OdeDuetCooldownExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Cooldown.OdeToJoy);
                    _cooldownTimer?.Start();
                }
                else
                {
                    // Activate
                    _gameState.OdeDuetActive = true;
                    _gameState.OdeDuetExpiry = DateTime.Now.AddSeconds(DuetDescriptions.Duration.OdeToJoy);
                    _gameState.CrescendoNotesPlaced = 0;
                    _gameState.CrescendoCompletedSections = 0;
                    _gameState.Crescendo4Claimed = false;
                    _gameState.Crescendo8Claimed = false;
                    _gameState.Crescendo12Claimed = false;
                    _gameState.Crescendo16Claimed = false;
                }
            }

            UpdateAbilityDisplay();
        }

        private void OnCloseButtonClicked(object? sender, RoutedEventArgs e)
        {
            this.IsVisible = false;
            _cooldownTimer?.Stop();
        }

        private void UpdateCooldownDisplay(object? sender, EventArgs e)
        {
            UpdateAbilityDisplay();
        }

        public void StartCooldownTimer()
        {
            _cooldownTimer?.Start();
        }

        public void StopCooldownTimer()
        {
            _cooldownTimer?.Stop();
        }
    }
}
