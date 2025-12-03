/*
 * File: Views/UniteTheSymphonyScreen.axaml.cs
 * Summary: Code-behind for the "Unite The Symphony" crafting screen.
 * Purpose: Provides UI handlers for crafting and displays craftable items based on GameState.
 */

using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicClicker.Helpers;
using System;

namespace MusicClicker.Views
{
    public partial class UniteTheSymphonyScreen : UserControl
    {
        private GameState? _gameState;
        private MainWindow? _mainWindow;

        public UniteTheSymphonyScreen()
        {
            InitializeComponent();
        }

        // Called by MainWindow when wiring screens so this control can access game state
        public void SetGameState(GameState gameState, MainWindow mainWindow)
        {
            _gameState = gameState;
            _mainWindow = mainWindow;

            // Wire dissolve buttons
            try
            {
                // Unsubscribe first to avoid duplicate handlers if SetGameState is called multiple times
                try { MoonlightMajorDissolveButton.Click -= MoonlightMajorDissolveButton_Click; } catch { }
                try { EroicaMajorDissolveButton.Click -= EroicaMajorDissolveButton_Click; } catch { }
                try { SwanMajorDissolveButton.Click -= SwanMajorDissolveButton_Click; } catch { }
                try { LaCampanellaMajorDissolveButton.Click -= LaCampanellaMajorDissolveButton_Click; } catch { }
                try { EnigmaMajorDissolveButton.Click -= EnigmaMajorDissolveButton_Click; } catch { }
                try { FateMajorDissolveButton.Click -= FateMajorDissolveButton_Click; } catch { }
                try { OdeToJoyMajorDissolveButton.Click -= OdeToJoyMajorDissolveButton_Click; } catch { }

                MoonlightMajorDissolveButton.Click += MoonlightMajorDissolveButton_Click;
                EroicaMajorDissolveButton.Click += EroicaMajorDissolveButton_Click;
                SwanMajorDissolveButton.Click += SwanMajorDissolveButton_Click;
                LaCampanellaMajorDissolveButton.Click += LaCampanellaMajorDissolveButton_Click;
                EnigmaMajorDissolveButton.Click += EnigmaMajorDissolveButton_Click;
                FateMajorDissolveButton.Click += FateMajorDissolveButton_Click;
                OdeToJoyMajorDissolveButton.Click += OdeToJoyMajorDissolveButton_Click;
            }
            catch { }

            // Refresh display
            if (_mainWindow != null && _gameState != null)
                UIUpdater.UpdateUnitySymphonyUIImmediate(_mainWindow, _gameState);
        }

        private void AddEntropicMelody(int amount = 1)
        {
            if (_gameState == null) return;
            _gameState.EntropicMelodies += amount;
            if (_mainWindow != null)
                UIUpdater.UpdateUnitySymphonyUIImmediate(_mainWindow, _gameState);
        }

        // Attempts to dissolve one of the given score. Returns the new owned count.
        private int TryDissolve(int owned, Action<int> setter, int entropicAmount = 1)
        {
            if (_gameState == null || _mainWindow == null) return owned;
            if (owned > 0)
            {
                int newOwned = owned - 1;
                setter(newOwned);
                AddEntropicMelody(entropicAmount);
                UIUpdater.UpdateUnitySymphonyUIImmediate(_mainWindow, _gameState);
                return newOwned;
            }
            UIUpdater.UpdateUnitySymphonyUIImmediate(_mainWindow, _gameState);
            return owned;
        }

        private void MoonlightMajorDissolveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            int owned = _gameState.MoonlightMajorOwned;
            int newOwned = TryDissolve(owned, v => _gameState.MoonlightMajorOwned = v, 10);
            if (newOwned == 0)
                _gameState.MoonlightMajorAbility = false;
        }

        private void EroicaMajorDissolveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            int owned = _gameState.EroicaMajorOwned;
            int newOwned = TryDissolve(owned, v => _gameState.EroicaMajorOwned = v, 20);
            if (newOwned == 0)
                _gameState.EroicaMajorAbility = false;
        }

        private void SwanMajorDissolveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            int owned = _gameState.SwanMajorOwned;
            int newOwned = TryDissolve(owned, v => _gameState.SwanMajorOwned = v, 40);
            if (newOwned == 0)
                _gameState.SwanMajorAbility = false;
        }

        private void LaCampanellaMajorDissolveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            int owned = _gameState.LaCampanellaMajorOwned;
            int newOwned = TryDissolve(owned, v => _gameState.LaCampanellaMajorOwned = v, 80);
            if (newOwned == 0)
                _gameState.LaCampanellaMajorAbility = false;
        }

        private void EnigmaMajorDissolveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            int owned = _gameState.EnigmaMajorOwned;
            int newOwned = TryDissolve(owned, v => _gameState.EnigmaMajorOwned = v, 160);
            if (newOwned == 0)
                _gameState.EnigmaMajorAbility = false;
        }

        private void FateMajorDissolveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            int owned = _gameState.FateMajorOwned;
            int newOwned = TryDissolve(owned, v => _gameState.FateMajorOwned = v, 320);
            if (newOwned == 0)
                _gameState.FateMajorAbility = false;
        }

        private void OdeToJoyMajorDissolveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_gameState == null) return;
            int owned = _gameState.OdeToJoyMajorOwned;
            int newOwned = TryDissolve(owned, v => _gameState.OdeToJoyMajorOwned = v, 640);
            if (newOwned == 0)
                _gameState.OdeToJoyMajorAbility = false;
        }
    }
}