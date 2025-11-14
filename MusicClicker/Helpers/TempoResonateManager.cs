using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using System;
using System.Collections.Generic;

namespace MusicClicker
{
    public class TempoResonateManager : IDisposable
    {
        private bool _disposed = false;

        // UI references
        private readonly StackPanel _leftDrawerPanel;
        private readonly Border _equippedDisplay;
        private readonly TextBlock _equippedText;

        // Game state reference
        private readonly GameState _gameState;

        // Equip/disable prompt UI
        private readonly StackPanel _equipPromptPanel;
        private readonly TextBlock _equipPromptText;
        private readonly Button _equipYesButton;
        private readonly Button _equipNoButton;

        // Mapping of score names → image asset paths
        private readonly Dictionary<string, string> _majorScoreImages = new()
        {
            {"Moonlight", "avares://MusicClicker/Assets/Music Game 16_9 Assets [978EB92]-min2.png"},
            {"Eroica", "avares://MusicClicker/Assets/Music Game 16_9 Assets [04AC7F2]-min2.png"},
            {"Swan", "avares://MusicClicker/Assets/Music Game 16_9 Assets [876DEE6]-min2.png"},
            {"LaCampanella", "avares://MusicClicker/Assets/LaCampanellaMajor2.png"},
            {"Enigma", "avares://MusicClicker/Assets/EnigmaMajor2.png"},
            {"Fate", "avares://MusicClicker/Assets/FateMajor2.png"},
            {"OdeToJoy", "avares://MusicClicker/Assets/OdeToJoyMajor2.png"}
        };

        // Cache of already-loaded bitmaps
        private readonly Dictionary<string, Bitmap> _bitmapCache = new();

        // Currently equipped bitmap and placeholder bitmap
        private Bitmap? _equippedBitmap;
        private Bitmap? _emptyBitmap;

        public TempoResonateManager(
            StackPanel leftDrawerPanel, Border equippedDisplay, TextBlock equippedText, GameState gameState,
            StackPanel equipPromptPanel, TextBlock equipPromptText, Button equipYesButton, Button equipNoButton)
        {
            // Store UI and game state references
            _leftDrawerPanel = leftDrawerPanel;
            _equippedDisplay = equippedDisplay;
            _equippedText = equippedText;
            _gameState = gameState;

            _equipPromptPanel = equipPromptPanel;
            _equipPromptText = equipPromptText;
            _equipYesButton = equipYesButton;
            _equipNoButton = equipNoButton;

            // Load images, populate drawer, and set initial equipped state
            LoadBitmaps();
            InitializeDrawer();
            SetDefaultEquipped();
        }

        private void LoadBitmaps()
        {
            // Load all major score images into cache
            foreach (var kvp in _majorScoreImages)
            {
                if (!_bitmapCache.ContainsKey(kvp.Key))
                {
                    using var stream = AssetLoader.Open(new Uri(kvp.Value));
                    _bitmapCache[kvp.Key] = new Bitmap(stream);
                }
            }

            // Load placeholder image for "None" equipped state
            using var emptyStream = AssetLoader.Open(new Uri("avares://MusicClicker/Assets/EmptyResonate.png"));
            _emptyBitmap = new Bitmap(emptyStream);
        }

        private void SetDefaultEquipped()
        {
            // Display default "None" state
            _equippedText.Text = "None";
            _equippedBitmap = _emptyBitmap;

            // Update the equipped display image
            _equippedDisplay.Child = new Image
            {
                Source = _equippedBitmap,
                Width = 384,
                Height = 216,
                Stretch = Stretch.UniformToFill
            };
        }

        private void InitializeDrawer()
        {
            // Clear drawer + set vertical orientation
            _leftDrawerPanel.Children.Clear();
            _leftDrawerPanel.Orientation = Avalonia.Layout.Orientation.Vertical;

            // Create entry button for each score image
            foreach (var score in _majorScoreImages.Keys)
            {
                var bitmap = _bitmapCache[score];

                // Image for button content
                var imageControl = new Image
                {
                    Source = bitmap,
                    Width = 256,
                    Height = 144,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(5),
                    Tag = score,
                    Opacity = OwnsScore(score) ? 1.0 : 0.3 // Dim if not owned
                };

                // Transparent button overlay for click handling
                var button = new Button
                {
                    Width = 256,
                    Height = 144,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(5),
                    Content = imageControl,
                    IsEnabled = OwnsScore(score) // Disable if not owned
                };

                // Clicking toggles equip/disable prompt
                button.Click += (_, _) =>
                {
                    if (_equippedText.Text == score)
                    {
                        ShowDisablePrompt(score);
                    }
                    else
                    {
                        ShowEquipPrompt(score);
                    }
                };

                _leftDrawerPanel.Children.Add(button);
            }
        }

        public void RefreshDrawer()
        {
            // Updates opacity + enabled state based on ownership
            foreach (var child in _leftDrawerPanel.Children)
            {
                if (child is Button btn && btn.Content is Image img && img.Tag is string scoreName)
                {
                    bool owned = OwnsScore(scoreName);
                    img.Opacity = owned ? 1.0 : 0.3;
                    btn.IsEnabled = owned;
                }
            }
        }

        // Checks whether the player owns the specified major score
        private bool OwnsScore(string score)
        {
            return score switch
            {
                "Moonlight" => _gameState.MoonlightMajorOwned > 0,
                "Eroica" => _gameState.EroicaMajorOwned > 0,
                "Swan" => _gameState.SwanMajorOwned > 0,
                "LaCampanella" => _gameState.LaCampanellaMajorOwned > 0,
                "Enigma" => _gameState.EnigmaMajorOwned > 0,
                "Fate" => _gameState.FateMajorOwned > 0,
                "OdeToJoy" => _gameState.OdeToJoyMajorOwned > 0,
                _ => false
            };
        }

        private void ShowEquipPrompt(string scoreName)
        {
            // Show prompt asking to equip a score
            _equipPromptPanel.IsVisible = true;
            _equipPromptText.Text = $"Resonate with {scoreName}'s Tempo?";

            // Remove previous handlers to prevent accumulation
            _equipYesButton.Click -= EquipYesButtonHandler;
            _equipNoButton.Click -= EquipNoButtonHandler;

            // Local handlers capture the score name
            void EquipYesButtonHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                EquipScore(scoreName);
                _equipPromptPanel.IsVisible = false;

                _equipYesButton.Click -= EquipYesButtonHandler;
                _equipNoButton.Click -= EquipNoButtonHandler;
            }

            void EquipNoButtonHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                _equipPromptPanel.IsVisible = false;

                _equipYesButton.Click -= EquipYesButtonHandler;
                _equipNoButton.Click -= EquipNoButtonHandler;
            }

            _equipYesButton.Click += EquipYesButtonHandler;
            _equipNoButton.Click += EquipNoButtonHandler;
        }

        private void ShowDisablePrompt(string scoreName)
        {
            // Show prompt asking to disable the currently equipped score
            _equipPromptPanel.IsVisible = true;
            _equipPromptText.Text = $"Do you want to disable {scoreName}'s resonance?";

            // Remove previous handlers
            _equipYesButton.Click -= DisableYesHandler;
            _equipNoButton.Click -= DisableNoHandler;

            // Local handlers
            void DisableYesHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                UnequipScore();
                _equipPromptPanel.IsVisible = false;

                _equipYesButton.Click -= DisableYesHandler;
                _equipNoButton.Click -= DisableNoHandler;
            }

            void DisableNoHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                _equipPromptPanel.IsVisible = false;

                _equipYesButton.Click -= DisableYesHandler;
                _equipNoButton.Click -= DisableNoHandler;
            }

            _equipYesButton.Click += DisableYesHandler;
            _equipNoButton.Click += DisableNoHandler;
        }

        private void EquipScore(string scoreName)
        {
            // Set new equipped score text + bitmap
            _equippedText.Text = scoreName;
            _equippedBitmap = _bitmapCache[scoreName];

            _equippedDisplay.Child = new Image
            {
                Source = _equippedBitmap,
                Width = 384,
                Height = 216,
                Stretch = Stretch.UniformToFill
            };

            // Disable all major ability flags
            _gameState.MoonlightMajorAbility = false;
            _gameState.EroicaMajorAbility = false;
            _gameState.SwanMajorAbility = false;
            _gameState.LaCampanellaMajorAbility = false;
            _gameState.EnigmaMajorAbility = false;
            _gameState.FateMajorAbility = false;
            _gameState.OdeToJoyMajorAbility = false;

            // Enable the selected ability
            switch (scoreName)
            {
                case "Moonlight": _gameState.MoonlightMajorAbility = true; break;
                case "Eroica": _gameState.EroicaMajorAbility = true; break;
                case "Swan": _gameState.SwanMajorAbility = true; break;
                case "LaCampanella": _gameState.LaCampanellaMajorAbility = true; break;
                case "Enigma": _gameState.EnigmaMajorAbility = true; break;
                case "Fate": _gameState.FateMajorAbility = true; break;
                case "OdeToJoy": _gameState.OdeToJoyMajorAbility = true; break;
            }
        }

        private void UnequipScore()
        {
            // Reset equipped score to "None"
            _equippedText.Text = "None";
            _equippedBitmap = _emptyBitmap;

            _equippedDisplay.Child = new Image
            {
                Source = _equippedBitmap,
                Width = 384,
                Height = 216,
                Stretch = Stretch.UniformToFill
            };

            // Disable all major abilities
            _gameState.MoonlightMajorAbility = false;
            _gameState.EroicaMajorAbility = false;
            _gameState.SwanMajorAbility = false;
            _gameState.LaCampanellaMajorAbility = false;
            _gameState.EnigmaMajorAbility = false;
            _gameState.FateMajorAbility = false;
            _gameState.OdeToJoyMajorAbility = false;
        }

        public void Dispose()
        {
            // Destructor-safe cleanup pattern
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
