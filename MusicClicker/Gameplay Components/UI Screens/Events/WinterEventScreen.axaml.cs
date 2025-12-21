using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MusicClicker.Helpers;
using System;

namespace MusicClicker.Views
{
    public partial class WinterEventScreen : UserControl
    {
        private GameState? _gameState;
        private DispatcherTimer? _countdownTimer;
        private static readonly DateTime _eventTarget = new DateTime(2026, 1, 1, 0, 0, 0);

        public WinterEventScreen()
        {
            InitializeComponent();
            BackButton.Click += BackButton_Click;
            SoulOfWinterButton.Click += SoulOfWinterButton_Click;
            WinterMajorButton.Click += WinterMajorButton_Click;

            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _countdownTimer.Tick += (s, e) => UpdateCountdown();
            _countdownTimer.Start();
        }

        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
            UpdateUI(gameState);
        }

        public void UpdateUI(GameState gameState)
        {
            _gameState = gameState;

            double cost = (gameState.NotesPerSecond * 3) + 100;
            SoulOfWinterCostText.Text = $"{NumberFormatter.FormatLargeNumber(cost)} Notes for Soul Resonance";
            WinterMajorOwnedText.Text = $"Major Sheets of Winter Owned: {gameState.WinterMajorSheets}";

            if (this.FindControl<TextBlock>("NotesText") is TextBlock notes)
            {
                notes.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";
            }

            try
            {
                WinterMajorOwnedRightText.Text = $"Number Owned: {gameState.WinterOwned}";
                WinterCrescendanceText.Text = "Crescendance Ability: Requiem of the Frozen Choir\n\n• Frigid Melody stacks are gained by Crescendance actions and are consumed to produce Eternal Frost or Regal Snowlight.\n• Eternal Frost grants a fixed number of enhanced offensive critical clicks when consumed; these criticals scale with NotesPerSecond.\n• Regal Snowlight grants immediate resource and fragment bonuses when consumed; stacking increases potency.\n• Crescendance Bonds (for example, The Snow's Desire) modify how Frigid Melody is gained or consumed and change the exact numerical effects of Eternal Frost and Regal Snowlight.";

                if (NpsFreezeTimerText != null)
                {
                    if (DateTime.Now <= gameState.NpsFreezeExpiry)
                    {
                        var remaining = gameState.NpsFreezeExpiry - DateTime.Now;
                        NpsFreezeTimerText.Text = $"NPS Frozen: {remaining.TotalSeconds:F1}s";
                        NpsFreezeTimerText.IsVisible = true;
                    }
                    else
                    {
                        NpsFreezeTimerText.IsVisible = false;
                    }
                }

                if (BlizzardBountyTimerText != null)
                {
                    BlizzardBountyTimerText.IsVisible = false;
                }
            }
            catch { }
        }

        private void SoulOfWinterButton_Click(object? sender, RoutedEventArgs e)
        {
            var current = this.Parent;
            while (current != null && current is not Window) current = current.Parent;
            MainWindow? mw = current as MainWindow;

            GameState? gs = _gameState ?? mw?.GameState;
            if (gs == null) return;

            double cost = (gs.NotesPerSecond * 3) + 100;
            double before = AtomicDouble.Read(ref gs._notes);
            if (before < cost) { UpdateUI(gs); return; }

            double after = AtomicDouble.Add(ref gs._notes, -cost);
            if (new Random().Next(1, 4) == 1)
            {
                gs.WinterMajorSheets++;
            }

            if (mw != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    UpdateUI(gs);
                    UIUpdater.UpdateUI(mw, gs);
                }, DispatcherPriority.Background);
            }
            else
            {
                UpdateUI(gs);
            }
        }

        private void WinterMajorButton_Click(object? sender, RoutedEventArgs e)
        {
            var current = this.Parent;
            while (current != null && current is not Window) current = current.Parent;
            MainWindow? mw = current as MainWindow;

            GameState? gs = _gameState ?? mw?.GameState;
            if (gs == null) return;

            if (gs.WinterMajorSheets > 0)
            {
                gs.WinterMajorSheets--;
                if (new Random().Next(1, 251) == 1)
                {
                    gs.WinterOwned++;
                }

                if (mw != null)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        UpdateUI(gs);
                        UIUpdater.UpdateUI(mw, gs);
                    }, DispatcherPriority.Background);
                }
                else
                {
                    UpdateUI(gs);
                }
            }
        }

        private async void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            var current = this.Parent;
            while (current != null && current is not Window) current = current.Parent;

            if (current is MainWindow mw)
            {
                await mw.TransitionAsync(() =>
                {
                    this.IsVisible = false;
                    var eternalModulationScreen = mw.FindControl<UserControl>("EternalModulationScreen");
                    if (eternalModulationScreen != null)
                        eternalModulationScreen.IsVisible = true;
                });
            }
            else
            {
                this.IsVisible = false;
                if (current is Window parentWindow)
                {
                    var eternalModulationScreen = parentWindow.FindControl<UserControl>("EternalModulationScreen");
                    if (eternalModulationScreen != null)
                        eternalModulationScreen.IsVisible = true;
                }
            }
        }

        private void UpdateCountdown()
        {
            try
            {
                var remaining = _eventTarget - DateTime.Now;
                if (remaining <= TimeSpan.Zero)
                {
                    if (this.FindControl<TextBlock>("WinterCountdownText") is TextBlock txt)
                        txt.Text = "Time Remaining: 00:00:00:00";
                    _countdownTimer?.Stop();
                    return;
                }
                string formatted = $"{remaining.Days:D2}:{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                if (this.FindControl<TextBlock>("WinterCountdownText") is TextBlock countdown)
                    countdown.Text = $"Time Remaining: {formatted}";
            }
            catch { }
        }
    }
}
