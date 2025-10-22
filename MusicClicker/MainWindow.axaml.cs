using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using System;
using System.Timers;

namespace MusicClicker
{
    public partial class MainWindow : Window
    {
        private double _notes = 0;
        private double _notesPerSecond = 0;
        private int _chordOwned = 0;
        private double _chordBaseCost = 10;

        private Timer _timer;

        public MainWindow()
        {
            InitializeComponent();

            // Main game clicks
            ClickButton.Click += ClickButton_Click;

            // Upgrade screen navigation
            OpenUpgradesButton.Click += (s, e) =>
            {
                MainScreen.IsVisible = false;
                UpgradeScreen.IsVisible = true;
                UpdateUI();
            };

            BackButton.Click += (s, e) =>
            {
                UpgradeScreen.IsVisible = false;
                MainScreen.IsVisible = true;
            };

            // Buy chord on upgrade screen
            BuyChordButton.Click += BuyChordButton_Click;

            // Timer for passive notes
            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                _notes += _notesPerSecond;
                Dispatcher.UIThread.Post(UpdateUI);
            };
            _timer.Start();

            // Load images safely if needed
            LoadChordImage();
        }

        private void LoadChordImage()
        {
            try
            {
                // Optional: if you want to show the image in the upgrade screen
                // Already defined in XAML, but if dynamic loading needed:
                // ChordImageUpgrade.Source = new Bitmap("avares://MusicClicker/Assets/MusicGameAssetsChord-min.png");
            }
            catch
            {
                // Handle missing image gracefully
            }
        }
        private void ClickButton_Click(object? sender, RoutedEventArgs e)
        {
            _notes++;
            UpdateUI();
        }
        private void BuyChordButton_Click(object? sender, RoutedEventArgs e)
        {
            double cost = GetChordCost();
            if (_notes >= cost)
            {
                _notes -= cost;
                _chordOwned++;
                _notesPerSecond += 0.5;
                UpdateUI();
            }
        }
        private double GetChordCost()
        {
            return Math.Round(_chordBaseCost * Math.Pow(1.15, _chordOwned), 2);
        }

        private void UpdateUI()
        {
            // Update main screen
            NotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
            NpsText.Text = $"Notes Per Second: {Math.Round(_notesPerSecond, 1)}";
            // Update upgrade screen
            UpgradeNotesText.Text = $"Notes: {Math.Round(_notes, 1)}";
            ChordOwnedTextUpgrade.Text = $"Number Owned: {_chordOwned}";
            ChordCostTextUpgrade.Text = $"Cost: {GetChordCost()}";
        }
    }
}
