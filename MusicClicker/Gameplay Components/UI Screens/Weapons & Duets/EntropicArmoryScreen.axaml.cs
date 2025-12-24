/*
 * File: Views/EntropicArmoryScreen.axaml.cs
 * Summary: Code-behind for Entropic Armory weapon shop screen.
 * Purpose: Displays entropic weapons, handles purchases using Entropic Melody currency.
 */

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using MusicClicker;
using MusicClicker.Armory;

namespace MusicClicker.Views
{
    /// <summary>
    /// Entropic Armory screen manages the endgame weapon shop where players
    /// can purchase 14 powerful weapons using Entropic Melody currency.
    /// </summary>
    public partial class EntropicArmoryScreen : UserControl
    {
        private GameState? _gameState;
        private MainWindow? _mainWindow;

        // Weapon names in display order
        private readonly string[] _weaponNames = new[]
        {
            "Phases of Infinity",
            "Moonlit Revelation",
            "Reborn in Sakura's Embrace",
            "A Thousand Petals",
            "Swan's Finality",
            "Star-Piercing Swan Song",
            "Heaven's Chime",
            "Bell of Chromatic Fate",
            "Ever-Burning Mystery",
            "Prismatic Enigmas",
            "Finality of the Cosmos",
            "Destiny's Conductor",
            "Cathartic Divination",
            "Dreams of Harmony"
        };

        // Costs in Entropic Melody (doubles each tier)
        private readonly int[] _weaponCosts = new[]
        {
            100,      // Phases of Infinity
            200,      // Moonlit Revelation
            400,      // Reborn in Sakura's Embrace
            800,      // A Thousand Petals
            1600,     // Swan's Finality
            3200,     // Star-Piercing Swan Song
            6400,     // Heaven's Chime
            12800,    // Bell of Chromatic Fate
            25600,    // Ever-Burning Mystery
            51200,    // Prismatic Enigmas
            102400,   // Finality of the Cosmos
            204800,   // Destiny's Conductor
            409600,   // Cathartic Divination
            819200    // Dreams of Harmony
        };

        public EntropicArmoryScreen()
        {
            InitializeComponent();
            BackButton.Click += BackButton_Click;
            InitializeWeaponButtons();
        }

        private void InitializeWeaponButtons()
        {
            EntropicWeapon1Button.Click += (s, e) => HandleWeaponPurchase(0);
            EntropicWeapon2Button.Click += (s, e) => HandleWeaponPurchase(1);
            EntropicWeapon3Button.Click += (s, e) => HandleWeaponPurchase(2);
            EntropicWeapon4Button.Click += (s, e) => HandleWeaponPurchase(3);
            EntropicWeapon5Button.Click += (s, e) => HandleWeaponPurchase(4);
            EntropicWeapon6Button.Click += (s, e) => HandleWeaponPurchase(5);
            EntropicWeapon7Button.Click += (s, e) => HandleWeaponPurchase(6);
            EntropicWeapon8Button.Click += (s, e) => HandleWeaponPurchase(7);
            EntropicWeapon9Button.Click += (s, e) => HandleWeaponPurchase(8);
            EntropicWeapon10Button.Click += (s, e) => HandleWeaponPurchase(9);
            EntropicWeapon11Button.Click += (s, e) => HandleWeaponPurchase(10);
            EntropicWeapon12Button.Click += (s, e) => HandleWeaponPurchase(11);
            EntropicWeapon13Button.Click += (s, e) => HandleWeaponPurchase(12);
            EntropicWeapon14Button.Click += (s, e) => HandleWeaponPurchase(13);
        }

        public void Initialize(GameState gameState, MainWindow mainWindow)
        {
            _gameState = gameState;
            _mainWindow = mainWindow;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (_gameState == null) return;

            // Update Entropic Melody count
            EntropicMelodyText.Text = $"{_gameState.EntropicMelodies:N0}";

            // Update all 14 weapons
            for (int i = 0; i < 14; i++)
            {
                UpdateWeaponDisplay(i);
            }
        }

        private void UpdateWeaponDisplay(int weaponIndex)
        {
            if (_gameState == null) return;

            // Get UI elements for this weapon
            var button = this.FindControl<Button>($"EntropicWeapon{weaponIndex + 1}Button");
            var nameText = this.FindControl<TextBlock>($"EntropicWeapon{weaponIndex + 1}NameText");
            var costText = this.FindControl<TextBlock>($"EntropicWeapon{weaponIndex + 1}CostText");
            var ownedText = this.FindControl<TextBlock>($"EntropicWeapon{weaponIndex + 1}OwnedText");

            if (button == null || nameText == null || costText == null || ownedText == null)
                return;

            bool owned = IsWeaponOwned(weaponIndex);
            int cost = _weaponCosts[weaponIndex];
            bool canAfford = _gameState.EntropicMelodies >= cost;

            if (owned)
            {
                // Owned - show as purchased
                button.Background = new SolidColorBrush(Color.FromArgb(68, 0, 100, 0));
                button.Opacity = 0.7;
                button.IsEnabled = false;
                costText.Text = "OWNED";
                costText.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100));
                ownedText.Text = "✓ Purchased";
                ownedText.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100));
            }
            else if (canAfford)
            {
                // Can afford - highlight
                button.Background = new SolidColorBrush(Color.FromArgb(68, 0, 0, 0));
                button.Opacity = 1.0;
                button.IsEnabled = true;
                costText.Text = $"Cost: {cost:N0} Entropic Melody";
                costText.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                ownedText.Text = "Not Owned";
                ownedText.Foreground = Brushes.White;
            }
            else
            {
                // Cannot afford
                button.Background = new SolidColorBrush(Color.FromArgb(68, 0, 0, 0));
                button.Opacity = 0.6;
                button.IsEnabled = false;
                costText.Text = $"Cost: {cost:N0} Entropic Melody";
                costText.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
                ownedText.Text = "Not Owned";
                ownedText.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            }
        }

        private bool IsWeaponOwned(int weaponIndex)
        {
            if (_gameState == null) return false;

            return weaponIndex switch
            {
                0 => _gameState.PhasesOfInfinity,
                1 => _gameState.MoonlitRevelation,
                2 => _gameState.RebornInSakurasEmbrace,
                3 => _gameState.AThousandPetals,
                4 => _gameState.SwansFinality,
                5 => _gameState.StarPiercingSwanSong,
                6 => _gameState.HeavensChime,
                7 => _gameState.BellOfChromaticFate,
                8 => _gameState.EverBurningMystery,
                9 => _gameState.PrismaticEnigmas,
                10 => _gameState.FinalityOfTheCosmos,
                11 => _gameState.DestinysConductor,
                12 => _gameState.CatharticDivination,
                13 => _gameState.DreamsOfHarmony,
                _ => false
            };
        }

        private void HandleWeaponPurchase(int weaponIndex)
        {
            if (_gameState == null || IsWeaponOwned(weaponIndex)) return;

            int cost = _weaponCosts[weaponIndex];

            if (_gameState.EntropicMelodies < cost)
                return;

            // Deduct cost
            _gameState.EntropicMelodies -= cost;
            
            // Celestial Horology passive: Chrono-Entropic Refund - refund 50% of spent entropic
            WeaponAbilities.CelestialHorology_RefundEntropic(_gameState, cost);

            // Set ownership
            switch (weaponIndex)
            {
                case 0: _gameState.PhasesOfInfinity = true; break;
                case 1: _gameState.MoonlitRevelation = true; break;
                case 2: _gameState.RebornInSakurasEmbrace = true; break;
                case 3: _gameState.AThousandPetals = true; break;
                case 4: _gameState.SwansFinality = true; break;
                case 5: _gameState.StarPiercingSwanSong = true; break;
                case 6: _gameState.HeavensChime = true; break;
                case 7: _gameState.BellOfChromaticFate = true; break;
                case 8: _gameState.EverBurningMystery = true; break;
                case 9: _gameState.PrismaticEnigmas = true; break;
                case 10: _gameState.FinalityOfTheCosmos = true; break;
                case 11: _gameState.DestinysConductor = true; break;
                case 12: _gameState.CatharticDivination = true; break;
                case 13: _gameState.DreamsOfHarmony = true; break;
            }

            // Save and update
            MusicClicker.Helpers.SaveManager.Save(_gameState, out _);
            UpdateDisplay();
        }

        private async void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_mainWindow != null)
            {
                await _mainWindow.TransitionAsync(() =>
                {
                    this.IsVisible = false;
                    _mainWindow.ShowArmoryOfForte();
                });
            }
        }
    }
}
