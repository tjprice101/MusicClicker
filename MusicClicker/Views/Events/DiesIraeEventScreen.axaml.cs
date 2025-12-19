using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MusicClicker.Helpers;
using System;

namespace MusicClicker.Views
{
    public partial class DiesIraeEventScreen : UserControl
    {
        private GameState? _gameState;
        private DispatcherTimer? _countdownTimer;
        private static readonly DateTime _eventTarget = new DateTime(2026, 1, 1, 0, 0, 0);

        public DiesIraeEventScreen()
        {
            InitializeComponent();
            BackButton.Click += BackButton_Click;
            SoulOfDiesIraeButton.Click += SoulOfDiesIraeButton_Click;
            DiesIraeMajorButton.Click += DiesIraeMajorButton_Click;

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
            SoulOfDiesIraeCostText.Text = $"{NumberFormatter.FormatLargeNumber(cost)} Notes for Soul Resonance";
            DiesIraeMajorOwnedText.Text = $"Major Sheets of Dies Irae Owned: {gameState.DiesIraeMajorSheets}";

            if (this.FindControl<TextBlock>("NotesText") is TextBlock notes)
            {
                notes.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";
            }
            if (this.FindControl<TextBlock>("NpsText") is TextBlock nps)
            {
                nps.Text = $"Notes Per Second: {NumberFormatter.FormatLargeNumber(gameState.NotesPerSecond)}";
            }

            try
            {
                DiesIraeMajorOwnedRightText.Text = $"Number Owned: {gameState.DiesIraeOwned}";
                DiesIraeCrescendanceText.Text = "Every click grants +1 Dissonant Hatred (max 50). Combine 5 Dissonant → 1 Cacophonic Oblivion. Consume Cacophonic for 20 stacking 'Symphony of Hell's Retribution' crits (NPC × NPS × Dissonant Hatred). Consume Wrathful Seals for 5 stacking 'Seal-breaking Melody' crits (DOUBLE Symphony damage).";
            }
            catch { }
        }

        private void SoulOfDiesIraeButton_Click(object? sender, RoutedEventArgs e)
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
                gs.DiesIraeMajorSheets++;
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

        private void DiesIraeMajorButton_Click(object? sender, RoutedEventArgs e)
        {
            var current = this.Parent;
            while (current != null && current is not Window) current = current.Parent;
            MainWindow? mw = current as MainWindow;

            GameState? gs = _gameState ?? mw?.GameState;
            if (gs == null) return;

            if (gs.DiesIraeMajorSheets > 0)
            {
                gs.DiesIraeMajorSheets--;
                if (new Random().Next(1, 251) == 1)
                {
                    gs.DiesIraeOwned++;
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
                    if (this.FindControl<TextBlock>("DiesIraeCountdownText") is TextBlock txt)
                        txt.Text = "Time Remaining: 00:00:00:00";
                    _countdownTimer?.Stop();
                    return;
                }
                string formatted = $"{remaining.Days:D2}:{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                if (this.FindControl<TextBlock>("DiesIraeCountdownText") is TextBlock countdown)
                    countdown.Text = $"Time Remaining: {formatted}";
            }
            catch { }
        }
    }
}
