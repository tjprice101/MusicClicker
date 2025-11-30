/*
 * File: CacophonicDreamsScreen.axaml.cs
 * Summary: Endgame screen for Cacophonic Dreams functionality.
 * Purpose: Displays endgame content accessible at 1 trillion NPS.
 */

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MusicClicker.Helpers;
using System;
using System.Linq;
using Avalonia.Media;
using Avalonia.Layout;
using System.Collections.Generic;
using Avalonia;

namespace MusicClicker.Views
{
    /// <summary>
    /// Cacophonic Dreams screen - endgame content unlocked at 1 trillion NPS
    /// </summary>
    public partial class CacophonicDreamsScreen : UserControl
    {
        private MainWindow? _parentWindow;
        private DispatcherTimer? _bossFightTimer;
        private BossFightManager.BossType? _activeBossType;
        private Dictionary<int, Border> _noteElements = new();
        
        public CacophonicDreamsScreen()
        {
            InitializeComponent();
            
            // Wire up buttons
            if (BackButton != null)
            {
                BackButton.Click += BackButton_Click;
            }
            
            if (MercuryChallengeButton != null)
            {
                MercuryChallengeButton.Click += (s, e) => StartBossFight(BossFightManager.BossType.Mercury);
            }
            
            if (TonalityChallengeButton != null)
            {
                TonalityChallengeButton.Click += (s, e) => StartBossFight(BossFightManager.BossType.Tonality);
            }
            
            if (MarsChallengeButton != null)
            {
                MarsChallengeButton.Click += (s, e) => StartBossFight(BossFightManager.BossType.Mars);
            }
            
            // Set up boss fight timer
            _bossFightTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // Update 10 times per second
            };
            _bossFightTimer.Tick += BossFightTimer_Tick;
        }
        
        /// <summary>
        /// Set the parent window reference for navigation
        /// </summary>
        public void SetParentWindow(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
            
            // Update owned displays when screen loads
            UpdateOwnedDisplays();
            
            // Update weekly completions displays
            UpdateWeeklyCompletionsDisplays();
        }
        
        /// <summary>
        /// Update the weekly completions text for all bosses
        /// </summary>
        private void UpdateWeeklyCompletionsDisplays()
        {
            var manager = BossFightManager.Instance;
            
            if (MercuryWeeklyText != null)
            {
                int remaining = manager.GetRemainingWeeklyCompletions(BossFightManager.BossType.Mercury);
                MercuryWeeklyText.Text = $"Weekly Completions Remaining: {remaining}/3";
            }
            
            if (TonalityWeeklyText != null)
            {
                int remaining = manager.GetRemainingWeeklyCompletions(BossFightManager.BossType.Tonality);
                TonalityWeeklyText.Text = $"Weekly Completions Remaining: {remaining}/3";
            }
            
            if (MarsWeeklyText != null)
            {
                int remaining = manager.GetRemainingWeeklyCompletions(BossFightManager.BossType.Mars);
                MarsWeeklyText.Text = $"Weekly Completions Remaining: {remaining}/3";
            }
        }
        
        /// <summary>
        /// Update the "Owned" text for all boss rewards
        /// </summary>
        private void UpdateOwnedDisplays()
        {
            if (_parentWindow == null) return;
            
            var gameState = _parentWindow.GameState;
            
            // Update Mercury owned text
            if (MercuryOwnedText != null)
            {
                if (gameState.MercuryMajorOwned > 0)
                {
                    MercuryOwnedText.Text = $"Owned: {gameState.MercuryMajorOwned}";
                    MercuryOwnedText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(0, 206, 209)); // Dark Cyan
                }
                else
                {
                    MercuryOwnedText.Text = "Not Owned";
                    MercuryOwnedText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(135, 206, 250)); // Light Sky Blue
                }
            }
            
            // Update Clair De Lune (Tonality) owned text
            if (TonalityOwnedText != null)
            {
                if (gameState.ClairDeLuneMajorOwned > 0)
                {
                    TonalityOwnedText.Text = $"Owned: {gameState.ClairDeLuneMajorOwned}";
                    TonalityOwnedText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(255, 192, 203)); // Pink
                }
                else
                {
                    TonalityOwnedText.Text = "Not Owned";
                    TonalityOwnedText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(255, 0, 0)); // Red
                }
            }
            
            // Update Mars owned text
            if (MarsOwnedText != null)
            {
                if (gameState.MarsMajorOwned > 0)
                {
                    MarsOwnedText.Text = $"Owned: {gameState.MarsMajorOwned}";
                    MarsOwnedText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(255, 68, 68)); // Bright Red
                }
                else
                {
                    MarsOwnedText.Text = "Not Owned";
                    MarsOwnedText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(224, 224, 224)); // Light Gray
                }
            }
        }
        
        /// <summary>
        /// Start a boss fight
        /// </summary>
        private void StartBossFight(BossFightManager.BossType bossType)
        {
            var manager = BossFightManager.Instance;
            
            // Check if can fight (weekly limit)
            if (!manager.CanFight(bossType))
            {
                // Show message that weekly limit reached
                return;
            }
            
            _activeBossType = bossType;
            manager.StartFight(bossType);
            
            // TODO: Show fight overlay with health bar
            // For now, just show a basic fight window
            ShowFightOverlay(bossType);
            
            _bossFightTimer?.Start();
        }
        
        /// <summary>
        /// Show the fight overlay UI
        /// </summary>
        private void ShowFightOverlay(BossFightManager.BossType bossType)
        {
            if (FightOverlay == null || BossScroller == null) return;
            
            var manager = BossFightManager.Instance;
            if (manager.CurrentFight == null) return;
            
            // Hide scroller and back button, show fight overlay
            BossScroller.IsVisible = false;
            FightOverlay.IsVisible = true;
            if (BackButton != null)
                BackButton.IsVisible = false;
            
            // Set boss name
            if (FightBossName != null)
            {
                FightBossName.Text = manager.CurrentFight.FullName;
            }
            
            // Calculate and display multiplier
            UpdateMultiplierDisplay();
            
            // Initialize health bar
            UpdateFightUI();
        }
        
        /// <summary>
        /// Update the damage multiplier display
        /// </summary>
        private void UpdateMultiplierDisplay()
        {
            if (_parentWindow == null || MultiplierText == null || EffectivenesText == null) return;
            
            var manager = BossFightManager.Instance;
            var gameState = _parentWindow.GameState;
            
            if (manager.CurrentFight == null) return;
            
            string currentMajor = gameState.CurrentResonatedScore ?? "None";
            var equippedWeapons = new System.Collections.Generic.List<string>();
            if (gameState.CurrentResonatedWeapon1 != "None")
                equippedWeapons.Add(gameState.CurrentResonatedWeapon1);
            if (gameState.CurrentResonatedWeapon2 != "None")
                equippedWeapons.Add(gameState.CurrentResonatedWeapon2);
            
            double multiplier = manager.CalculateDamageMultiplier(currentMajor, equippedWeapons);
            MultiplierText.Text = $"Damage Multiplier: {multiplier:F2}x";
            
            // Show effectiveness breakdown
            string normalizedScore = currentMajor.Replace(" Major", "").Trim();
            string effectiveness = "";
            
            if (manager.CurrentFight.WeakTo.Contains(normalizedScore))
            {
                effectiveness = $"Your {currentMajor} is EFFECTIVE! (+100% damage)";
                EffectivenesText.Foreground = Avalonia.Media.Brushes.LimeGreen;
            }
            else if (manager.CurrentFight.ResistantTo.Contains(normalizedScore))
            {
                effectiveness = $"Your {currentMajor} is RESISTED! (-75% damage)";
                EffectivenesText.Foreground = Avalonia.Media.Brushes.Red;
            }
            else
            {
                effectiveness = $"Your {currentMajor} deals neutral damage";
                EffectivenesText.Foreground = Avalonia.Media.Brushes.Gray;
            }
            
            EffectivenesText.Text = effectiveness;
        }
        
        /// <summary>
        /// Handle click area pressed - process fight click
        /// </summary>
        private void ClickArea_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            OnFightClick();
        }
        
        /// <summary>
        /// Handle give up button
        /// </summary>
        private void GiveUpButton_Click(object? sender, RoutedEventArgs e)
        {
            OnFightLost();
        }
        
        /// <summary>
        /// Timer tick for boss fight - applies boss drain
        /// </summary>
        private void BossFightTimer_Tick(object? sender, EventArgs e)
        {
            var manager = BossFightManager.Instance;
            
            if (manager.CurrentFight == null || !manager.CurrentFight.IsActive)
            {
                _bossFightTimer?.Stop();
                return;
            }
            
            // Update fight timer and boss pushback (0.1 seconds)
            manager.UpdateFightTimer(0.1);
            
            // Update UI
            UpdateFightUI();
            
            // Check for note mechanic trigger
            if (manager.CurrentFight.NoteMechanicActive && NoteMechanicCanvas != null && !NoteMechanicCanvas.IsVisible)
            {
                ShowNoteMechanic();
            }
            
            // Check win/lose conditions
            if (manager.IsFightWon())
            {
                OnFightWon();
            }
            else if (manager.IsFightLost())
            {
                OnFightLost();
            }
        }
        
        /// <summary>
        /// Update the fight UI (health bar, etc.)
        /// </summary>
        private void UpdateFightUI()
        {
            var manager = BossFightManager.Instance;
            if (manager.CurrentFight == null) return;
            
            double healthPercent = manager.CurrentFight.BossHealth / manager.CurrentFight.MaxBossHealth;
            
            // Update health bar
            if (HealthBarFill != null)
            {
                // Get the Grid parent that contains the health bar
                if (HealthBarFill.Parent is Grid parentGrid)
                {
                    double parentWidth = parentGrid.Bounds.Width > 0 ? parentGrid.Bounds.Width : 1000;
                    HealthBarFill.Width = parentWidth * healthPercent;
                }
                else
                {
                    // Fallback if Grid parent not found
                    HealthBarFill.Width = 1000 * healthPercent;
                }
                
                // Change color based on health and boss type
                Avalonia.Media.Color color;
                
                switch (manager.CurrentFight.Type)
                {
                    case BossFightManager.BossType.Tonality:
                        // White -> Pink -> Red (as health DECREASES from 100% to 0%)
                        if (healthPercent >= 0.5)
                        {
                            // Interpolate from White (255,255,255) to Pink (255,192,203)
                            byte g = (byte)(255 - (63 * (1 - (healthPercent - 0.5) / 0.5)));
                            byte b = (byte)(255 - (52 * (1 - (healthPercent - 0.5) / 0.5)));
                            color = Avalonia.Media.Color.FromRgb(255, g, b);
                        }
                        else
                        {
                            // Interpolate from Pink (255,192,203) to Red (255,0,0)
                            byte g = (byte)(192 * (healthPercent / 0.5));
                            byte b = (byte)(203 * (healthPercent / 0.5));
                            color = Avalonia.Media.Color.FromRgb(255, g, b);
                        }
                        break;
                    
                    case BossFightManager.BossType.Mercury:
                        // Dark Cyan (0,139,139) -> Black (0,0,0) as health decreases
                        byte r = (byte)(0 * healthPercent);
                        byte g_merc = (byte)(139 * healthPercent);
                        byte b_merc = (byte)(139 * healthPercent);
                        color = Avalonia.Media.Color.FromRgb(r, g_merc, b_merc);
                        break;
                    
                    case BossFightManager.BossType.Mars:
                        // Vibrant Red (255,0,0) -> White (255,255,255) as health decreases
                        byte g_mars = (byte)(255 * (1 - healthPercent));
                        byte b_mars = (byte)(255 * (1 - healthPercent));
                        color = Avalonia.Media.Color.FromRgb(255, g_mars, b_mars);
                        break;
                    
                    default:
                        color = Avalonia.Media.Colors.LimeGreen;
                        break;
                }
                
                HealthBarFill.Background = new Avalonia.Media.SolidColorBrush(color);
            }
            
            // Update health text with time remaining
            if (HealthText != null)
            {
                int timeRemaining = (int)Math.Ceiling(manager.CurrentFight.FightTimeRemaining);
                HealthText.Text = $"{(healthPercent * 100):F1}% | {timeRemaining}s";
            }
            
            // Update special mechanic text
            if (SpecialMechanicText != null)
            {
                if (manager.CurrentFight.Type == BossFightManager.BossType.Tonality && 
                    manager.CurrentFight.TonalityFreezeActive &&
                    DateTime.UtcNow < manager.CurrentFight.TonalityFreezeExpiry)
                {
                    SpecialMechanicText.Text = "⚠️ TONALITY FROZEN! ⚠️";
                    SpecialMechanicText.IsVisible = true;
                }
                else
                {
                    SpecialMechanicText.IsVisible = false;
                }
            }
        }
        
        /// <summary>
        /// Handle fight victory
        /// </summary>
        private void OnFightWon()
        {
            _bossFightTimer?.Stop();
            HideNoteMechanic();
            
            var manager = BossFightManager.Instance;
            if (_activeBossType.HasValue)
            {
                manager.IncrementWeeklyCompletion(_activeBossType.Value);
                
                // Roll for reward
                bool gotReward = manager.RollForReward();
                
                // Award the major score if won
                if (gotReward && _parentWindow != null)
                {
                    var gameState = _parentWindow.GameState;
                    switch (_activeBossType.Value)
                    {
                        case BossFightManager.BossType.Mercury:
                            gameState.MercuryMajorOwned++;
                            break;
                        case BossFightManager.BossType.Tonality:
                            gameState.ClairDeLuneMajorOwned++;
                            break;
                        case BossFightManager.BossType.Mars:
                            gameState.MarsMajorOwned++;
                            break;
                    }
                    
                    // Update the UI immediately
                    UpdateOwnedDisplays();
                }
                
                // Show success message
                ShowSuccessMessage(_activeBossType.Value);
                
                // Update weekly completions display
                UpdateWeeklyCompletionsDisplays();
                
                HideFightOverlay();
            }
            
            manager.EndFight();
            _activeBossType = null;
        }
        
        /// <summary>
        /// Handle fight defeat
        /// </summary>
        private void OnFightLost()
        {
            _bossFightTimer?.Stop();
            HideNoteMechanic();
            
            var manager = BossFightManager.Instance;
            if (_activeBossType.HasValue)
            {
                // DON'T consume weekly attempt on failure
                // Show "Failed to Modulate" message
                ShowFailedMessage(_activeBossType.Value);
                
                HideFightOverlay();
            }
            
            manager.EndFight();
            _activeBossType = null;
        }
        
        /// <summary>
        /// Show "Failed to Modulate" message under the challenge button
        /// </summary>
        private void ShowFailedMessage(BossFightManager.BossType bossType)
        {
            TextBlock? failedText = bossType switch
            {
                BossFightManager.BossType.Mercury => MercuryFailedText,
                BossFightManager.BossType.Tonality => TonalityFailedText,
                BossFightManager.BossType.Mars => MarsFailedText,
                _ => null
            };
            
            if (failedText != null)
            {
                failedText.IsVisible = true;
                
                // Hide the message after 5 seconds
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                timer.Tick += (s, e) =>
                {
                    failedText.IsVisible = false;
                    timer.Stop();
                };
                timer.Start();
            }
        }
        
        /// <summary>
        /// Show "Victory! Modulation Complete!" message under the challenge button
        /// </summary>
        private void ShowSuccessMessage(BossFightManager.BossType bossType)
        {
            TextBlock? successText = bossType switch
            {
                BossFightManager.BossType.Mercury => MercurySuccessText,
                BossFightManager.BossType.Tonality => TonalitySuccessText,
                BossFightManager.BossType.Mars => MarsSuccessText,
                _ => null
            };
            
            if (successText != null)
            {
                successText.IsVisible = true;
                
                // Hide the message after 5 seconds
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(5)
                };
                timer.Tick += (s, e) =>
                {
                    successText.IsVisible = false;
                    timer.Stop();
                };
                timer.Start();
            }
        }
        
        /// <summary>
        /// Hide the fight overlay and return to boss selection
        /// </summary>
        private void HideFightOverlay()
        {
            if (FightOverlay != null && BossScroller != null)
            {
                FightOverlay.IsVisible = false;
                BossScroller.IsVisible = true;
                if (BackButton != null)
                    BackButton.IsVisible = true;
            }
        }
        
        /// <summary>
        /// Handle click during boss fight
        /// </summary>
        public void OnFightClick()
        {
            if (_parentWindow == null) return;
            
            var manager = BossFightManager.Instance;
            var gameState = _parentWindow.GameState;
            
            if (manager.CurrentFight == null || !manager.CurrentFight.IsActive) return;
            
            // Get current equipped items
            string currentMajor = gameState.CurrentResonatedScore ?? "None";
            var equippedWeapons = new System.Collections.Generic.List<string>();
            if (gameState.CurrentResonatedWeapon1 != "None")
                equippedWeapons.Add(gameState.CurrentResonatedWeapon1);
            if (gameState.CurrentResonatedWeapon2 != "None")
                equippedWeapons.Add(gameState.CurrentResonatedWeapon2);
            
            // Calculate damage multiplier
            double multiplier = manager.CalculateDamageMultiplier(currentMajor, equippedWeapons);
            
            // Process the click
            double damage = manager.ProcessClick(gameState.NotesPerSecond, multiplier);
            
            // Update UI
            UpdateFightUI();
            
            // Check for note mechanic trigger
            if (manager.CurrentFight != null && manager.CurrentFight.NoteMechanicActive && NoteMechanicCanvas != null)
            {
                ShowNoteMechanic();
            }
            
            // Check if boss is defeated immediately
            if (manager.IsFightWon())
            {
                OnFightWon();
            }
        }
        
        /// <summary>
        /// Show the musical note mechanic overlay
        /// </summary>
        private void ShowNoteMechanic()
        {
            var manager = BossFightManager.Instance;
            if (manager.CurrentFight == null || NoteMechanicCanvas == null) return;
            
            // Show the canvas
            NoteMechanicCanvas.IsVisible = true;
            NoteMechanicCanvas.Children.Clear();
            _noteElements.Clear();
            
            // Wait for layout to complete, then position elements
            Dispatcher.UIThread.Post(() =>
            {
                PositionNoteMechanicElements();
            }, DispatcherPriority.Loaded);
        }
        
        /// <summary>
        /// Position note mechanic elements after layout is complete
        /// </summary>
        private void PositionNoteMechanicElements()
        {
            var manager = BossFightManager.Instance;
            if (manager.CurrentFight == null || NoteMechanicCanvas == null) return;
            
            NoteMechanicCanvas.Children.Clear();
            _noteElements.Clear();
            
            // Use actual canvas dimensions (with fallback if not yet measured)
            double canvasWidth = NoteMechanicCanvas.Bounds.Width > 0 ? NoteMechanicCanvas.Bounds.Width : 1600;
            double canvasHeight = NoteMechanicCanvas.Bounds.Height > 0 ? NoteMechanicCanvas.Bounds.Height : 800;
            double noteSize = 80;
            
            // Add instruction text centered on screen
            var instructionText = new TextBlock
            {
                Text = "Click the notes in order!",
                FontSize = 48,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                [Canvas.TopProperty] = (canvasHeight / 2) - 150.0, // Center vertically, offset up for notes
                [Canvas.LeftProperty] = 0.0,
                Width = canvasWidth
            };
            NoteMechanicCanvas.Children.Add(instructionText);
            
            // Determine note color and positions based on boss type
            Color noteColor = manager.CurrentFight.Type switch
            {
                BossFightManager.BossType.Tonality => Colors.White,
                BossFightManager.BossType.Mars => Colors.Red,
                BossFightManager.BossType.Mercury => Color.FromRgb(0, 139, 139), // Dark Cyan
                _ => Colors.White
            };
            
            int totalNotes = manager.CurrentFight.TotalNotesRequired;
            
            // Spawn notes in a horizontal line centered on screen
            double centerY = canvasHeight / 2;
            double lineY = centerY - 50; // Position below the instruction text
            
            // Calculate horizontal spacing to fit all notes in a line
            double totalWidth = 1200; // Safe visible width
            double startX = (canvasWidth - totalWidth) / 2;
            double spacing = totalWidth / (totalNotes + 1);
            
            for (int i = 0; i < totalNotes; i++)
            {
                double x = startX + spacing * (i + 1) - (noteSize / 2);
                double y = lineY;
                
                CreateNoteButton(i, x, y, noteColor);
            }
        }
        
        /// <summary>
        /// Create a clickable note button
        /// </summary>
        private void CreateNoteButton(int noteIndex, double x, double y, Color color)
        {
            if (NoteMechanicCanvas == null) return;
            
            // Create a border container for the music note
            var noteBorder = new Border
            {
                Width = 80,
                Height = 80,
                Background = new SolidColorBrush(color),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(8),
                [Canvas.LeftProperty] = x,
                [Canvas.TopProperty] = y,
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };
            
            // Add music note symbol (♪) with number
            var noteStack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            var musicSymbol = new TextBlock
            {
                Text = "♪",
                FontSize = 40,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            
            var numberText = new TextBlock
            {
                Text = (noteIndex + 1).ToString(),
                FontSize = 20,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, -5, 0, 0)
            };
            
            noteStack.Children.Add(musicSymbol);
            noteStack.Children.Add(numberText);
            noteBorder.Child = noteStack;
            
            // Make the border clickable by handling pointer events
            int capturedIndex = noteIndex;
            noteBorder.PointerPressed += (s, e) =>
            {
                OnNoteClick(capturedIndex);
                e.Handled = true;
            };
            
            _noteElements[noteIndex] = noteBorder;
            NoteMechanicCanvas.Children.Add(noteBorder);
            
            // Note buttons no longer have animations
        }
        
        /// <summary>
        /// Add a gentle sway animation to a note element (DISABLED)
        /// </summary>
        private void AddSwayAnimation(Border noteElement)
        {
            // Animation removed - notes now sit static
        }
        
        /// <summary>
        /// Handle clicking a note
        /// </summary>
        private void OnNoteClick(int noteIndex)
        {
            var manager = BossFightManager.Instance;
            if (manager.CurrentFight == null) return;
            
            bool correctNote = manager.ProcessNoteClick(noteIndex);
            
            if (correctNote)
            {
                // Flash the background
                FlashBackground();
                
                // Remove the clicked note
                if (_noteElements.TryGetValue(noteIndex, out var noteElement))
                {
                    NoteMechanicCanvas?.Children.Remove(noteElement);
                    _noteElements.Remove(noteIndex);
                }
                
                // Check if all notes are done
                if (!manager.CurrentFight.NoteMechanicActive && NoteMechanicCanvas != null)
                {
                    HideNoteMechanic();
                }
            }
        }
        
        /// <summary>
        /// Hide the note mechanic overlay
        /// </summary>
        private void HideNoteMechanic()
        {
            if (NoteMechanicCanvas != null)
            {
                NoteMechanicCanvas.IsVisible = false;
                NoteMechanicCanvas.Children.Clear();
                _noteElements.Clear();
            }
        }
        
        /// <summary>
        /// Flash the background when a correct note is clicked
        /// </summary>
        private async void FlashBackground()
        {
            if (NoteMechanicCanvas == null) return;
            
            var originalBrush = NoteMechanicCanvas.Background;
            
            // Flash to lighter color
            NoteMechanicCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            
            await System.Threading.Tasks.Task.Delay(100);
            
            // Return to original
            NoteMechanicCanvas.Background = originalBrush;
        }
        
        /// <summary>
        /// Handle back button click - return to main screen
        /// </summary>
        private async void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_parentWindow != null)
            {
                // Use the parent window's transition system
                await _parentWindow.TransitionAsync(() =>
                {
                    this.IsVisible = false;
                    _parentWindow.MainScreen.IsVisible = true;
                });
            }
        }
    }
}