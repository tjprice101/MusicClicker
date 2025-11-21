using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace MusicClicker.Views
{
    public partial class ArmorOfForteScreen : UserControl
    {
        private GameState? _gameState;

        // Weapon names and requirements mapping
        private readonly string[] _weaponNames = new[]
        {
            "Moonlight Blade I",
            "Moonlight Blade II",
            "Eroica Sword I",
            "Eroica Sword II",
            "Swan Lance I",
            "Swan Lance II",
            "Campanella Dagger I",
            "Campanella Dagger II",
            "Enigma Staff I",
            "Enigma Staff II",
            "Fate Axe I",
            "Fate Axe II",
            "Joy Hammer I",
            "Joy Hammer II",
            "Dies Irae Scythe I",
            "Dies Irae Scythe II",
            "Winter Bow I",
            "Winter Bow II"
        };

        private readonly string[] _requiredMajors = new[]
        {
            "Moonlight Sonata Major",
            "Moonlight Sonata Major",
            "Eroica Major",
            "Eroica Major",
            "Swan Lake Major",
            "Swan Lake Major",
            "La Campanella Major",
            "La Campanella Major",
            "Enigma Major",
            "Enigma Major",
            "Fate Major",
            "Fate Major",
            "Ode to Joy Major",
            "Ode to Joy Major",
            "Dies Irae Major",
            "Dies Irae Major",
            "Winter Major",
            "Winter Major"
        };

        public ArmorOfForteScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            
            InitializeArmorButtons();
        }

        private void InitializeArmorButtons()
        {
            ArmorItem1Button.Click += (s, e) => HandleArmorPurchase(0);
            ArmorItem2Button.Click += (s, e) => HandleArmorPurchase(1);
            ArmorItem3Button.Click += (s, e) => HandleArmorPurchase(2);
            ArmorItem4Button.Click += (s, e) => HandleArmorPurchase(3);
            ArmorItem5Button.Click += (s, e) => HandleArmorPurchase(4);
            ArmorItem6Button.Click += (s, e) => HandleArmorPurchase(5);
            ArmorItem7Button.Click += (s, e) => HandleArmorPurchase(6);
            ArmorItem8Button.Click += (s, e) => HandleArmorPurchase(7);
            ArmorItem9Button.Click += (s, e) => HandleArmorPurchase(8);
            ArmorItem10Button.Click += (s, e) => HandleArmorPurchase(9);
            ArmorItem11Button.Click += (s, e) => HandleArmorPurchase(10);
            ArmorItem12Button.Click += (s, e) => HandleArmorPurchase(11);
            ArmorItem13Button.Click += (s, e) => HandleArmorPurchase(12);
            ArmorItem14Button.Click += (s, e) => HandleArmorPurchase(13);
            ArmorItem15Button.Click += (s, e) => HandleArmorPurchase(14);
            ArmorItem16Button.Click += (s, e) => HandleArmorPurchase(15);
            ArmorItem17Button.Click += (s, e) => HandleArmorPurchase(16);
            ArmorItem18Button.Click += (s, e) => HandleArmorPurchase(17);
        }

        private double CalculateWeaponCost(int weaponIndex)
        {
            if (_gameState == null)
                return 0;

            // Base multiplier: 250 * (20^weaponNumber)
            double baseMultiplier = 250 * Math.Pow(20, weaponIndex + 1);
            
            // Cost = NPS + 250 * 20^weaponNumber
            return (_gameState.NotesPerSecond + baseMultiplier) * (_gameState.NotesPerClick / 2);
        }

        private bool IsWeaponUnlocked(int weaponIndex)
        {
            if (_gameState == null || weaponIndex >= _requiredMajors.Length)
                return false;

            // Check if the required Major is owned
            return weaponIndex switch
            {
                0 or 1 => _gameState.MoonlightMajorOwned > 0,
                2 or 3 => _gameState.EroicaMajorOwned > 0,
                4 or 5 => _gameState.SwanMajorOwned > 0,
                6 or 7 => _gameState.LaCampanellaMajorOwned > 0,
                8 or 9 => _gameState.EnigmaMajorOwned > 0,
                10 or 11 => _gameState.FateMajorOwned > 0,
                12 or 13 => _gameState.OdeToJoyMajorOwned > 0,
                14 or 15 => false, // Dies Irae Major - not yet implemented
                16 or 17 => false, // Winter Major - not yet implemented
                _ => false
            };
        }

        private int GetWeaponOwned(int weaponIndex)
        {
            if (_gameState == null)
                return 0;

            return weaponIndex switch
            {
                0 => _gameState.MoonlightBladeIOwned,
                1 => _gameState.MoonlightBladeIIOwned,
                2 => _gameState.EroicaSwordIOwned,
                3 => _gameState.EroicaSwordIIOwned,
                4 => _gameState.SwanLanceIOwned,
                5 => _gameState.SwanLanceIIOwned,
                6 => _gameState.CampanellaDaggerIOwned,
                7 => _gameState.CampanellaDaggerIIOwned,
                8 => _gameState.EnigmaStaffIOwned,
                9 => _gameState.EnigmaStaffIIOwned,
                10 => _gameState.FateAxeIOwned,
                11 => _gameState.FateAxeIIOwned,
                12 => _gameState.JoyHammerIOwned,
                13 => _gameState.JoyHammerIIOwned,
                14 => _gameState.DiesIraeScytheIOwned,
                15 => _gameState.DiesIraeScytheIIOwned,
                16 => _gameState.WinterBowIOwned,
                17 => _gameState.WinterBowIIOwned,
                _ => 0
            };
        }

        private void SetWeaponOwned(int weaponIndex, int value)
        {
            if (_gameState == null)
                return;

            switch (weaponIndex)
            {
                case 0: _gameState.MoonlightBladeIOwned = value; break;
                case 1: _gameState.MoonlightBladeIIOwned = value; break;
                case 2: _gameState.EroicaSwordIOwned = value; break;
                case 3: _gameState.EroicaSwordIIOwned = value; break;
                case 4: _gameState.SwanLanceIOwned = value; break;
                case 5: _gameState.SwanLanceIIOwned = value; break;
                case 6: _gameState.CampanellaDaggerIOwned = value; break;
                case 7: _gameState.CampanellaDaggerIIOwned = value; break;
                case 8: _gameState.EnigmaStaffIOwned = value; break;
                case 9: _gameState.EnigmaStaffIIOwned = value; break;
                case 10: _gameState.FateAxeIOwned = value; break;
                case 11: _gameState.FateAxeIIOwned = value; break;
                case 12: _gameState.JoyHammerIOwned = value; break;
                case 13: _gameState.JoyHammerIIOwned = value; break;
                case 14: _gameState.DiesIraeScytheIOwned = value; break;
                case 15: _gameState.DiesIraeScytheIIOwned = value; break;
                case 16: _gameState.WinterBowIOwned = value; break;
                case 17: _gameState.WinterBowIIOwned = value; break;
            }
        }

        private void HandleArmorPurchase(int weaponIndex)
        {
            if (_gameState == null || !IsWeaponUnlocked(weaponIndex))
                return;

            // Check if already owned
            if (GetWeaponOwned(weaponIndex) > 0)
                return;

            double cost = CalculateWeaponCost(weaponIndex);
            
            if (_gameState.Notes >= cost)
            {
                _gameState.Notes -= cost;
                SetWeaponOwned(weaponIndex, 1);
                
                UpdateUI(_gameState);
            }
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show the main screen
            this.IsVisible = false;

            // Find the parent window and show MainScreen
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var mainScreen = parentWindow.FindControl<Grid>("MainScreen");
                if (mainScreen != null)
                    mainScreen.IsVisible = true;
            }
        }

        // Public method to update the UI when the screen becomes visible
        public void UpdateUI(GameState gameState)
        {
            _gameState = gameState;
            
            // Update notes display
            ArmorNotesText.Text = $"Notes: {FormatNumber(gameState.Notes)}";
            
            // Update all 18 weapon slots
            UpdateWeaponSlot(0, ArmorItem1Button, ArmorItem1CostText, ArmorItem1OwnedText);
            UpdateWeaponSlot(1, ArmorItem2Button, ArmorItem2CostText, ArmorItem2OwnedText);
            UpdateWeaponSlot(2, ArmorItem3Button, ArmorItem3CostText, ArmorItem3OwnedText);
            UpdateWeaponSlot(3, ArmorItem4Button, ArmorItem4CostText, ArmorItem4OwnedText);
            UpdateWeaponSlot(4, ArmorItem5Button, ArmorItem5CostText, ArmorItem5OwnedText);
            UpdateWeaponSlot(5, ArmorItem6Button, ArmorItem6CostText, ArmorItem6OwnedText);
            UpdateWeaponSlot(6, ArmorItem7Button, ArmorItem7CostText, ArmorItem7OwnedText);
            UpdateWeaponSlot(7, ArmorItem8Button, ArmorItem8CostText, ArmorItem8OwnedText);
            UpdateWeaponSlot(8, ArmorItem9Button, ArmorItem9CostText, ArmorItem9OwnedText);
            UpdateWeaponSlot(9, ArmorItem10Button, ArmorItem10CostText, ArmorItem10OwnedText);
            UpdateWeaponSlot(10, ArmorItem11Button, ArmorItem11CostText, ArmorItem11OwnedText);
            UpdateWeaponSlot(11, ArmorItem12Button, ArmorItem12CostText, ArmorItem12OwnedText);
            UpdateWeaponSlot(12, ArmorItem13Button, ArmorItem13CostText, ArmorItem13OwnedText);
            UpdateWeaponSlot(13, ArmorItem14Button, ArmorItem14CostText, ArmorItem14OwnedText);
            UpdateWeaponSlot(14, ArmorItem15Button, ArmorItem15CostText, ArmorItem15OwnedText);
            UpdateWeaponSlot(15, ArmorItem16Button, ArmorItem16CostText, ArmorItem16OwnedText);
            UpdateWeaponSlot(16, ArmorItem17Button, ArmorItem17CostText, ArmorItem17OwnedText);
            UpdateWeaponSlot(17, ArmorItem18Button, ArmorItem18CostText, ArmorItem18OwnedText);
        }

        private void UpdateWeaponSlot(int weaponIndex, Button button, TextBlock costText, TextBlock ownedText)
        {
            if (weaponIndex >= _weaponNames.Length)
                return;

            bool isUnlocked = IsWeaponUnlocked(weaponIndex);
            int owned = GetWeaponOwned(weaponIndex);
            
            // Update button appearance and text
            if (button.Content is TextBlock buttonTextBlock)
            {
                buttonTextBlock.Text = _weaponNames[weaponIndex];
            }

            if (isUnlocked)
            {
                double cost = CalculateWeaponCost(weaponIndex);
                
                // Check if already purchased
                if (owned > 0)
                {
                    // Already purchased - show as owned
                    button.Background = new SolidColorBrush(Color.FromArgb(68, 0, 100, 0)); // Green tint
                    button.Opacity = 1.0;
                    button.IsEnabled = false;
                    
                    costText.Text = "✓ Purchased";
                    costText.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100));
                    
                    ownedText.Text = "Owned: 1";
                    ownedText.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100));
                }
                else
                {
                    // Available for purchase
                    button.Background = new SolidColorBrush(Color.FromArgb(68, 0, 0, 0)); // #44000000
                    button.Opacity = 1.0;
                    button.IsEnabled = true;
                    
                    costText.Text = $"Cost: {FormatNumber(cost)} Notes";
                    costText.Foreground = Brushes.White;
                    
                    ownedText.Text = "Not Owned";
                    ownedText.Foreground = Brushes.White;
                }
            }
            else
            {
                // Locked - gray out
                button.Background = new SolidColorBrush(Color.FromArgb(68, 60, 60, 60)); // Grayed out
                button.Opacity = 0.5;
                button.IsEnabled = false;
                
                costText.Text = $"Requires {_requiredMajors[weaponIndex]}";
                costText.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
                
                ownedText.Text = "🔒 Locked";
                ownedText.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            }
        }

        private string FormatNumber(double num)
        {
            if (num >= 1e27)
                return $"{num / 1e27:F2}Oc";
            if (num >= 1e24)
                return $"{num / 1e24:F2}Sp";
            if (num >= 1e21)
                return $"{num / 1e21:F2}Sx";
            if (num >= 1e18)
                return $"{num / 1e18:F2}Qn";
            if (num >= 1e15)
                return $"{num / 1e15:F2}Qd";
            if (num >= 1e12)
                return $"{num / 1e12:F2}T";
            if (num >= 1e9)
                return $"{num / 1e9:F2}B";
            if (num >= 1e6)
                return $"{num / 1e6:F2}M";
            if (num >= 1e3)
                return $"{num / 1e3:F2}K";
            return $"{Math.Round(num, 1)}";
        }
    }
}