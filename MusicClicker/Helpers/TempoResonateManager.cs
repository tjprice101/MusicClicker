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

        private readonly StackPanel _leftDrawerPanel;
        private readonly Border _equippedDisplay;
        private readonly TextBlock _equippedText;
        private readonly GameState _gameState;

        private readonly StackPanel _equipPromptPanel;
        private readonly TextBlock _equipPromptText;
        private readonly Button _equipYesButton;
        private readonly Button _equipNoButton;

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

        private readonly Dictionary<string, Bitmap> _bitmapCache = new();
        private Bitmap? _equippedBitmap;
        private Bitmap? _emptyBitmap;

        public TempoResonateManager(
            StackPanel leftDrawerPanel, Border equippedDisplay, TextBlock equippedText, GameState gameState,
            StackPanel equipPromptPanel, TextBlock equipPromptText, Button equipYesButton, Button equipNoButton)
        {
            _leftDrawerPanel = leftDrawerPanel;
            _equippedDisplay = equippedDisplay;
            _equippedText = equippedText;
            _gameState = gameState;

            _equipPromptPanel = equipPromptPanel;
            _equipPromptText = equipPromptText;
            _equipYesButton = equipYesButton;
            _equipNoButton = equipNoButton;

            LoadBitmaps();
            InitializeDrawer();
            SetDefaultEquipped();
        }

        private void LoadBitmaps()
        {
            foreach (var kvp in _majorScoreImages)
            {
                if (!_bitmapCache.ContainsKey(kvp.Key))
                {
                    using var stream = AssetLoader.Open(new Uri(kvp.Value));
                    _bitmapCache[kvp.Key] = new Bitmap(stream);
                }
            }

            // Load empty placeholder bitmap
            using var emptyStream = AssetLoader.Open(new Uri("avares://MusicClicker/Assets/EmptyResonate.png"));
            _emptyBitmap = new Bitmap(emptyStream);
        }

        private void SetDefaultEquipped()
        {
            _equippedText.Text = "None";
            _equippedBitmap = _emptyBitmap;

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
            _leftDrawerPanel.Children.Clear();
            _leftDrawerPanel.Orientation = Avalonia.Layout.Orientation.Vertical;

            foreach (var score in _majorScoreImages.Keys)
            {
                var bitmap = _bitmapCache[score];

                var imageControl = new Image
                {
                    Source = bitmap,
                    Width = 256,
                    Height = 144,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(5),
                    Tag = score,
                    Opacity = OwnsScore(score) ? 1.0 : 0.3
                };

                var button = new Button
                {
                    Width = 256,
                    Height = 144,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(5),
                    Content = imageControl,
                    IsEnabled = OwnsScore(score)
                };

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
            _equipPromptPanel.IsVisible = true;
            _equipPromptText.Text = $"Resonate with {scoreName}'s Tempo?";

            _equipYesButton.Click -= EquipYesButtonHandler;
            _equipNoButton.Click -= EquipNoButtonHandler;

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
            _equipPromptPanel.IsVisible = true;
            _equipPromptText.Text = $"Do you want to disable {scoreName}'s resonance?";

            _equipYesButton.Click -= DisableYesHandler;
            _equipNoButton.Click -= DisableNoHandler;

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
            _equippedText.Text = scoreName;
            _equippedBitmap = _bitmapCache[scoreName];

            _equippedDisplay.Child = new Image
            {
                Source = _equippedBitmap,
                Width = 384,
                Height = 216,
                Stretch = Stretch.UniformToFill
            };

            // Reset all major abilities
            _gameState.MoonlightMajorAbility = false;
            _gameState.EroicaMajorAbility = false;
            _gameState.SwanMajorAbility = false;
            _gameState.LaCampanellaMajorAbility = false;
            _gameState.EnigmaMajorAbility = false;
            _gameState.FateMajorAbility = false;
            _gameState.OdeToJoyMajorAbility = false;

            // Enable selected ability
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
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
