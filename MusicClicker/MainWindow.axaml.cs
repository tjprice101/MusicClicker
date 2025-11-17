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
using System.Timers;

namespace MusicClicker
{
    public partial class MainWindow : Window
    {
        // ------------------- EXISTING FIELDS -------------------
        private Timer _timer;
        private Random _random = new Random();
        private GameState gameState = new GameState();
        public GameState GameState => gameState;
        public static TempoResonateManager GlobalTempoManager = null!;

        // ------------------- CAROUSEL FIELDS -------------------
        private int currentIndex = 0;
        private double targetRotation = 0;
        private double currentRotation = 0;
        private bool isAnimating = false;
        
        private const double RADIUS = 350; // Distance from center
        private const int BUTTON_COUNT = 8;
        
        private DispatcherTimer animationTimer = null!;
        
        // Drag fields
        private bool isDragging = false;
        private Point lastDragPoint;
        private double dragVelocity = 0;
        private double dragMomentum = 0;
        
        private List<(Button button, TranslateTransform translate, ScaleTransform scale)> carouselButtons = null!;

        // ------------------- CONSTRUCTOR -------------------
        public MainWindow()
        {
            InitializeComponent();

            InitializeCarousel();

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

            TempoResonateScreen.BackButtonTempoResonate.Click += BackButtonTempoResonate_Click;
            ButtonInitializer.InitializeAllButtons(this);
            TempoResonateButton.Click += TempoResonateButton_Click;

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

        // ------------------- CAROUSEL INITIALIZATION -------------------
        private void InitializeCarousel()
        {
            carouselButtons = new List<(Button, TranslateTransform, ScaleTransform)>
            {
                GetButtonTransforms(FragmentationButton),
                GetButtonTransforms(ResonanceButton),
                GetButtonTransforms(MelodyButton),
                GetButtonTransforms(HarmonyButton),
                GetButtonTransforms(TempoResonateButton),
                GetButtonTransforms(EternalModulationButton),
                GetButtonTransforms(ArmorOfForteButton),
                GetButtonTransforms(SymphonicGalleryButton)
            };

            // Add drag event handlers to the carousel canvas
            var canvas = this.FindControl<Canvas>("CarouselCanvas");
            if (canvas != null)
            {
                canvas.PointerPressed += CarouselCanvas_PointerPressed;
                canvas.PointerMoved += CarouselCanvas_PointerMoved;
                canvas.PointerReleased += CarouselCanvas_PointerReleased;
            }

            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            UpdateCarouselPositions();
        }

        private (Button, TranslateTransform, ScaleTransform) GetButtonTransforms(Button button)
        {
            var transformGroup = button.RenderTransform as TransformGroup;
            if (transformGroup != null && transformGroup.Children.Count >= 2)
            {
                var translate = transformGroup.Children[0] as TranslateTransform;
                var scale = transformGroup.Children[1] as ScaleTransform;
                
                if (translate != null && scale != null)
                {
                    return (button, translate, scale);
                }
            }
            
            var newTranslate = new TranslateTransform();
            var newScale = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
            var newTransformGroup = new TransformGroup();
            newTransformGroup.Children.Add(newTranslate);
            newTransformGroup.Children.Add(newScale);
            button.RenderTransform = newTransformGroup;
            
            return (button, newTranslate, newScale);
        }

        // ------------------- DRAG HANDLERS -------------------
        private void CarouselCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            isDragging = true;
            lastDragPoint = e.GetPosition(sender as Control);
            dragVelocity = 0;
            dragMomentum = 0;
            isAnimating = false;
        }

        private void CarouselCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!isDragging) return;

            var currentPoint = e.GetPosition(sender as Control);
            double deltaY = currentPoint.Y - lastDragPoint.Y;
            
            // Reduced sensitivity from 0.5 to 0.25
            double rotationDelta = deltaY * 0.25;
            currentRotation -= rotationDelta;
            
            // Track velocity for momentum
            dragVelocity = -rotationDelta;
            
            lastDragPoint = currentPoint;
        }

        private void CarouselCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!isDragging) return;
            
            isDragging = false;
            
            // Apply momentum
            dragMomentum = dragVelocity * 2.0;
            
            // Snap to nearest item after momentum
            if (Math.Abs(dragMomentum) < 1.0)
            {
                SnapToNearest();
            }
        }

        private void SnapToNearest()
        {
            double angleStep = 360.0 / BUTTON_COUNT;
            int nearestIndex = (int)Math.Round(currentRotation / angleStep) % BUTTON_COUNT;
            if (nearestIndex < 0) nearestIndex += BUTTON_COUNT;
            
            currentIndex = nearestIndex;
            targetRotation = nearestIndex * angleStep;
            isAnimating = true;
        }

        // ------------------- CAROUSEL METHODS -------------------
        private void RotateCarousel(int direction)
        {
            if (isAnimating) return;

            currentIndex = (currentIndex + direction + BUTTON_COUNT) % BUTTON_COUNT;
            
            double angleStep = 360.0 / BUTTON_COUNT;
            targetRotation = currentIndex * angleStep;
            
            isAnimating = true;
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            // Apply momentum decay
            if (!isDragging && Math.Abs(dragMomentum) > 0.1)
            {
                currentRotation += dragMomentum;
                dragMomentum *= 0.92;
                
                // When momentum is low, snap to nearest
                if (Math.Abs(dragMomentum) < 0.5)
                {
                    dragMomentum = 0;
                    SnapToNearest();
                }
            }
            else if (isAnimating)
            {
                double diff = targetRotation - currentRotation;
                
                if (diff > 180) diff -= 360;
                if (diff < -180) diff += 360;
                
                currentRotation += diff * 0.15;
                
                if (Math.Abs(diff) < 0.5)
                {
                    currentRotation = targetRotation;
                    isAnimating = false;
                }
            }
            
            UpdateCarouselPositions();
        }

        private void UpdateCarouselPositions()
{
    double angleStep = 360.0 / BUTTON_COUNT;
    double horizontalOffset = 15; // Shift entire carousel 50px to the right

    for (int i = 0; i < carouselButtons.Count; i++)
    {
        var (button, translate, scale) = carouselButtons[i];

        double angle = (i * angleStep - currentRotation) * (Math.PI / 180.0);

        // Vertical position
        double y = -Math.Cos(angle) * RADIUS;
        bool isAtBottom = y > (RADIUS - 50);

        // Opacity
        double opacity = Math.Max(0, (y + 100) / (RADIUS + 100));

        // Scale
        double t = (RADIUS - y) / (2 * RADIUS); // 0 = bottom, 1 = top
        double scaleValue = 0.9 - 0.4 * t;

        // Horizontal spacing adjustment
        double centerOffset = Math.Sin(angle) * RADIUS;

        // Extra spacing for middle button
        double spacingMultiplier = 1.0 + 1.2 * Math.Pow(Math.Cos(angle), 2); 
        double x = centerOffset * spacingMultiplier + horizontalOffset; // <-- add offset here

        translate.X = x;
        translate.Y = y;
        scale.ScaleX = scaleValue;
        scale.ScaleY = scaleValue;
        button.Opacity = opacity;
        button.IsHitTestVisible = isAtBottom && opacity > 0.8;
        button.ZIndex = isAtBottom ? 50 : (int)(-y);
    }
}

        // ------------------- EXISTING METHODS -------------------
        public void ClickButton_Click(object? sender, RoutedEventArgs e)
        {
            double notesPerClick = gameState.NotesPerClick;

            if (gameState.MoonlightMajorAbility)
            {
                notesPerClick += gameState.NotesPerSecond;
            }
            else if (gameState.FateMajorAbility)
            {
                gameState.FateCounter++;
                if (gameState.FateCounter == 5)
                {
                    gameState.FateCounter = 0;
                    gameState.Notes += (gameState.Notes * 0.30);
                }
            }

            gameState.Notes += notesPerClick;

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
                gameState.SwanMajorOwned += 1;
                gameState.LaCampanellaMajorOwned += 1;
                gameState.EnigmaMajorOwned += 1;
                gameState.FateMajorOwned += 1;
                gameState.OdeToJoyMajorOwned += 1;

                UIUpdater.UpdateUI(this, gameState);
                UIUpdater.UpdateFragmentationUI(this, gameState);
                UIUpdater.UpdateSaveScoresUI(this, gameState);
                UIUpdater.UpdateHeartOfHarmonyUI(this, gameState);
                UIUpdater.UpdateUnitySymphonyUI(this, gameState);
            }
        }

        public void TempoResonateButton_Click(object? sender, RoutedEventArgs e)
        {
            MainScreen.IsVisible = false;
            TempoResonateScreen.IsVisible = true;
        }

        public void BackButtonTempoResonate_Click(object? sender, RoutedEventArgs e)
        {
            TempoResonateScreen.IsVisible = false;
            MainScreen.IsVisible = true;
        }
    }
}