/*
 * File: Views/ArmoryOfForteScreen.axaml.cs
 * Summary: Code-behind for Armory of Forte weapon shop screen.
 * Purpose: Displays weapon list, handles purchase logic, and shows Forte/Duet resonances.
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using MusicClicker;

namespace MusicClicker.Views
{
    /// <summary>
    /// Armory of Forte screen manages the weapon shop where players can purchase
    /// 18 different weapons, each tied to specific major scores they own.
    /// </summary>
    public partial class ArmoryOfForteScreen : UserControl
    {
        // Reference to the game state for checking/updating player progress
        private GameState? _gameState;

        // Array of all weapon names displayed in the shop (20 total weapons)
        private readonly string[] _weaponNames = new[]
        {
            // Moonlight weapons — user requested Incisor first, then Eulogy
            "Incisor of Moonlight",      // Tier 1 Moonlight weapon (now first)
            "Eulogy of the Moon",        // Tier 2 Moonlight weapon (now second)

            // Eroica (user-provided replacements)
            "Sakura's Blossom",          // Eroica 1
            "Funeral Prayer",            // Eroica 2

            // Swan Lake (user-provided replacements)
            "Star-Scattered Wings",      // Swan Lake 1
            "Thousand Winged Swan",      // Swan Lake 2

            // La Campanella (user-provided replacements)
            "Symphony of Bells",         // La Campanella 1
            "Razer of Bell's Chimes",    // La Campanella 2

            // Enigma (user-provided replacements)
            "Creator of Mystery",        // Enigma 1
            "Truthseeker",               // Enigma 2

            // Fate (user-provided replacements)
            "Astral Chainripper",        // Fate 1
            "Cosmic Weaver",             // Fate 2

            // Ode to Joy (user-provided replacements)
            "Joyful Catharsis",          // Ode to Joy 1
            "Ode to Creation",           // Ode to Joy 2

            // Event weapons (Dies Irae / Winter) - replaced with updated flavor names
            "Seven Circles",            // Formerly: Dies Irae Scythe I (event)
            "Hell's Wrath",             // Formerly: Dies Irae Scythe II (event)
            "Cacophonic Blizzard",      // Formerly: Winter Bow I (event)
            "The Snow's Desire",        // Formerly: Winter Bow II (event)

            // Boss Fight weapons (Clair de Lune)
            "Clockwork's Harmony",      // Clair de Lune 1
            "Metronomic Dissonance"     // Clair de Lune 2
        };

        // Array mapping each weapon to its required major
        // Player must own this major to unlock the corresponding weapon
        private readonly string[] _requiredMajors = new[]
        {
            "Moonlight Sonata Major",  // Required for Moonlight Blade I & II
            "Moonlight Sonata Major",
            "Eroica Major",            // Required for Eroica Sword I & II
            "Eroica Major",
            "Swan Lake Major",         // Required for Swan Lance I & II
            "Swan Lake Major",
            "La Campanella Major",     // Required for Campanella Dagger I & II
            "La Campanella Major",
            "Enigma Major",            // Required for Enigma Staff I & II
            "Enigma Major",
            "Fate Major",              // Required for Fate Axe I & II
            "Fate Major",
            "Ode to Joy Major",        // Required for Joy Hammer I & II
            "Ode to Joy Major",
            "Dies Irae",               // Required for Dies Irae Scythe I & II (actual score)
            "Dies Irae",
            "Winter",                  // Required for Winter Bow I & II (actual score)
            "Winter",
            "Clair de Lune",           // Required for Clair de Lune weapons
            "Clair de Lune"
        };

        // Precomputed exponential base multipliers for weapon costs to avoid repeated Math.Pow calls.
        private static readonly double[] _weaponBaseMultipliers;

        static ArmoryOfForteScreen()
        {
            _weaponBaseMultipliers = new double[20];
            for (int i = 0; i < _weaponBaseMultipliers.Length; i++)
            {
                _weaponBaseMultipliers[i] = 250 * Math.Pow(3, i + 1);
            }
        }

        /// <summary>
        /// Constructor initializes the armory shop screen and sets up button handlers.
        /// </summary>
        public ArmoryOfForteScreen()
        {
            InitializeComponent();

            // Wire up back button to return to main screen
            BackButton.Click += BackButton_Click;
            
            // Wire up Entropic Armory button
            EntropicArmoryButton.Click += EntropicArmoryButton_Click;
            
            // Set up click handlers for all 20 weapon purchase buttons
            InitializeArmoryButtons();
        }

        /// <summary>
        /// Connects each weapon button to its purchase handler.
        /// Each button is linked to its corresponding weapon index (0-19).
        /// </summary>
        private void InitializeArmoryButtons()
        {
            ArmoryItem1Button.Click += (s, e) => HandleArmoryPurchase(0);   // Moonlight Blade I
            ArmoryItem2Button.Click += (s, e) => HandleArmoryPurchase(1);   // Moonlight Blade II
            ArmoryItem3Button.Click += (s, e) => HandleArmoryPurchase(2);   // Eroica Sword I
            ArmoryItem4Button.Click += (s, e) => HandleArmoryPurchase(3);   // Eroica Sword II
            ArmoryItem5Button.Click += (s, e) => HandleArmoryPurchase(4);   // Swan Lance I
            ArmoryItem6Button.Click += (s, e) => HandleArmoryPurchase(5);   // Swan Lance II
            ArmoryItem7Button.Click += (s, e) => HandleArmoryPurchase(6);   // Campanella Dagger I
            ArmoryItem8Button.Click += (s, e) => HandleArmoryPurchase(7);   // Campanella Dagger II
            ArmoryItem9Button.Click += (s, e) => HandleArmoryPurchase(8);   // Enigma Staff I
            ArmoryItem10Button.Click += (s, e) => HandleArmoryPurchase(9);  // Enigma Staff II
            ArmoryItem11Button.Click += (s, e) => HandleArmoryPurchase(10); // Fate Axe I
            ArmoryItem12Button.Click += (s, e) => HandleArmoryPurchase(11); // Fate Axe II
            ArmoryItem13Button.Click += (s, e) => HandleArmoryPurchase(12); // Joy Hammer I
            ArmoryItem14Button.Click += (s, e) => HandleArmoryPurchase(13); // Joy Hammer II
            ArmoryItem15Button.Click += (s, e) => HandleArmoryPurchase(14); // Seven Circles
            ArmoryItem16Button.Click += (s, e) => HandleArmoryPurchase(15); // Hell's Wrath
            ArmoryItem17Button.Click += (s, e) => HandleArmoryPurchase(16); // Cacophonic Blizzard
            ArmoryItem18Button.Click += (s, e) => HandleArmoryPurchase(17); // The Snow's Desire
            ArmoryItem19Button.Click += (s, e) => HandleArmoryPurchase(18); // Clockwork's Harmony
            ArmoryItem20Button.Click += (s, e) => HandleArmoryPurchase(19); // Metronomic Dissonance
        }

        /// <summary>
        /// Calculates the cost of a weapon based on its index.
        /// Cost formula: (NPS + 250 * 3^weaponNumber) * NotesPerClick
        /// Higher weapon numbers = exponentially more expensive.
        /// </summary>
        private double CalculateWeaponCost(int weaponIndex)
        {
            if (_gameState == null)
                return 0;

            // Use precomputed base multiplier to avoid repeated Math.Pow calls
            double baseMultiplier = (weaponIndex >= 0 && weaponIndex < _weaponBaseMultipliers.Length)
                ? _weaponBaseMultipliers[weaponIndex]
                : 250 * Math.Pow(3, weaponIndex + 1);
            
            // Final cost combines passive income (NPS) + exponential base, scaled by click power
            return (_gameState.NotesPerSecond + baseMultiplier) * _gameState.NotesPerClick;
        }

        /// <summary>
        /// Checks if a weapon is unlocked by verifying the player owns its required major.
        /// </summary>
        private bool IsWeaponUnlocked(int weaponIndex)
        {
            if (_gameState == null || weaponIndex >= _requiredMajors.Length)
                return false;

            // Check ownership of required major based on weapon index
            // Each major unlocks 2 weapons (tier I and II)
            return weaponIndex switch
            {
                0 or 1 => _gameState.MoonlightMajorOwned > 0,      // Moonlight weapons
                2 or 3 => _gameState.EroicaMajorOwned > 0,         // Eroica weapons
                4 or 5 => _gameState.SwanMajorOwned > 0,           // Swan Lake weapons
                6 or 7 => _gameState.LaCampanellaMajorOwned > 0,   // La Campanella weapons
                8 or 9 => _gameState.EnigmaMajorOwned > 0,         // Enigma weapons
                10 or 11 => _gameState.FateMajorOwned > 0,         // Fate weapons
                12 or 13 => _gameState.OdeToJoyMajorOwned > 0,     // Ode to Joy weapons
                14 or 15 => _gameState.DiesIraeOwned > 0,          // Dies Irae weapons
                16 or 17 => _gameState.WinterOwned > 0,            // Winter weapons
                18 or 19 => _gameState.ClairDeLuneMajorOwned > 0,       // Clair de Lune weapons
                _ => false
            };
        }

        /// <summary>
        /// Returns whether the specific weapon is owned (weapons are one-time purchases).
        /// </summary>
        private bool IsWeaponOwned(int weaponIndex)
        {
            if (_gameState == null)
                return false;

            return weaponIndex switch
            {
                0 => _gameState.IncisorOfMoonlight,
                1 => _gameState.EulogyOfTheMoon,
                2 => _gameState.SakurasBlossom,
                3 => _gameState.FuneralPrayer,
                4 => _gameState.StarScatteredWings,
                5 => _gameState.ThousandWingedSwan,
                6 => _gameState.SymphonyOfBells,
                7 => _gameState.RazerOfBellsChimes,
                8 => _gameState.CreatorOfMystery,
                9 => _gameState.Truthseeker,
                10 => _gameState.AstralChainripper,
                11 => _gameState.CosmicWeaver,
                12 => _gameState.JoyfulCatharsis,
                13 => _gameState.OdeToCreation,
                14 => _gameState.SevenCircles,
                15 => _gameState.HellsWrath,
                16 => _gameState.CacophonicBlizzard,
                17 => _gameState.TheSnowsDesire,
                18 => _gameState.ClockworksHarmony,
                19 => _gameState.MetronomicDissonance,
                _ => false
            };
        }

        /// <summary>
        /// Sets the owned count for a specific weapon in the game state.
        /// </summary>
        private void SetWeaponOwned(int weaponIndex, bool value)
        {
            if (_gameState == null)
                return;

            // Update the appropriate game state property based on weapon index
            switch (weaponIndex)
            {
                case 0: _gameState.IncisorOfMoonlight = value; break;
                case 1: _gameState.EulogyOfTheMoon = value; break;
                case 2: _gameState.SakurasBlossom = value; break;
                case 3: _gameState.FuneralPrayer = value; break;
                case 4: _gameState.StarScatteredWings = value; break;
                case 5: _gameState.ThousandWingedSwan = value; break;
                case 6: _gameState.SymphonyOfBells = value; break;
                case 7: _gameState.RazerOfBellsChimes = value; break;
                case 8: _gameState.CreatorOfMystery = value; break;
                case 9: _gameState.Truthseeker = value; break;
                case 10: _gameState.AstralChainripper = value; break;
                case 11: _gameState.CosmicWeaver = value; break;
                case 12: _gameState.JoyfulCatharsis = value; break;
                case 13: _gameState.OdeToCreation = value; break;
                case 14: _gameState.SevenCircles = value; break;
                case 15: _gameState.HellsWrath = value; break;
                case 16: _gameState.CacophonicBlizzard = value; break;
                case 17: _gameState.TheSnowsDesire = value; break;
                case 18: _gameState.ClockworksHarmony = value; break;
                case 19: _gameState.MetronomicDissonance = value; break;
            }
        }

        /// <summary>
        /// Handles weapon purchase attempt.
        /// Checks if weapon is unlocked, not already owned, and player has enough notes.
        /// </summary>
        private void HandleArmoryPurchase(int weaponIndex)
        {
            // Validate game state and weapon is unlocked
            if (_gameState == null || !IsWeaponUnlocked(weaponIndex))
                return;

            // Check if already owned (weapons are one-time purchases)
            if (IsWeaponOwned(weaponIndex))
                return;

            // Calculate cost and check if player can afford it
            double cost = CalculateWeaponCost(weaponIndex);
            
                if (MusicClicker.Helpers.AtomicDouble.Read(ref _gameState._notes) >= cost)
                {
                    MusicClicker.Helpers.AtomicDouble.Add(ref _gameState._notes, -cost);
                    SetWeaponOwned(weaponIndex, true);
                    
                    // Refresh UI to show purchase
                    UpdateUI(_gameState);
                }
        }

        /// <summary>
        /// Handler for Entropic Armory button - navigates to Entropic Armory screen.
        /// </summary>
        private async void EntropicArmoryButton_Click(object? sender, RoutedEventArgs e)
        {
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is MainWindow mainWindow)
            {
                await mainWindow.TransitionAsync(() =>
                {
                    this.IsVisible = false;
                    mainWindow.ShowEntropicArmory();
                });
            }
        }

        /// <summary>
        /// Handler for back button - returns to main screen.
        /// </summary>
        private async void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Navigate up the visual tree to find the parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            // If we found the main window, run the transition helper
            if (current is MainWindow mainWindow)
            {
                await mainWindow.TransitionAsync(() =>
                {
                    this.IsVisible = false;
                    var mainScreen = mainWindow.FindControl<Grid>("MainScreen");
                    if (mainScreen != null)
                        mainScreen.IsVisible = true;
                });
            }
            else if (current is Window parentWindow)
            {
                // Fallback: immediate switch without transition
                this.IsVisible = false;
                var mainScreen = parentWindow.FindControl<Grid>("MainScreen");
                if (mainScreen != null)
                    mainScreen.IsVisible = true;
            }
        }

        /// <summary>
        /// Public method to update the entire UI when the screen becomes visible.
        /// Called from external code when switching to this screen.
        /// </summary>
        public void UpdateUI(GameState gameState)
        {
            _gameState = gameState;
            
            // Update the notes display at top of screen
            ArmoryNotesText.Text = $"Notes: {FormatNumber(gameState.Notes)}";
            
            // Update all 20 weapon slots with current state
            UpdateWeaponSlot(0, ArmoryItem1Button, ArmoryItem1CostText, ArmoryItem1OwnedText, ArmoryItem1SoloConcertoText, ArmoryItem1SymphonicModulationText, ArmoryItem1DuetText);
            UpdateWeaponSlot(1, ArmoryItem2Button, ArmoryItem2CostText, ArmoryItem2OwnedText, ArmoryItem2SoloConcertoText, ArmoryItem2SymphonicModulationText, ArmoryItem2DuetText);
            UpdateWeaponSlot(2, ArmoryItem3Button, ArmoryItem3CostText, ArmoryItem3OwnedText, ArmoryItem3SoloConcertoText, ArmoryItem3SymphonicModulationText, ArmoryItem3DuetText);
            UpdateWeaponSlot(3, ArmoryItem4Button, ArmoryItem4CostText, ArmoryItem4OwnedText, ArmoryItem4SoloConcertoText, ArmoryItem4SymphonicModulationText, ArmoryItem4DuetText);
            UpdateWeaponSlot(4, ArmoryItem5Button, ArmoryItem5CostText, ArmoryItem5OwnedText, ArmoryItem5SoloConcertoText, ArmoryItem5SymphonicModulationText, ArmoryItem5DuetText);
            UpdateWeaponSlot(5, ArmoryItem6Button, ArmoryItem6CostText, ArmoryItem6OwnedText, ArmoryItem6SoloConcertoText, ArmoryItem6SymphonicModulationText, ArmoryItem6DuetText);
            UpdateWeaponSlot(6, ArmoryItem7Button, ArmoryItem7CostText, ArmoryItem7OwnedText, ArmoryItem7SoloConcertoText, ArmoryItem7SymphonicModulationText, ArmoryItem7DuetText);
            UpdateWeaponSlot(7, ArmoryItem8Button, ArmoryItem8CostText, ArmoryItem8OwnedText, ArmoryItem8SoloConcertoText, ArmoryItem8SymphonicModulationText, ArmoryItem8DuetText);
            UpdateWeaponSlot(8, ArmoryItem9Button, ArmoryItem9CostText, ArmoryItem9OwnedText, ArmoryItem9SoloConcertoText, ArmoryItem9SymphonicModulationText, ArmoryItem9DuetText);
            UpdateWeaponSlot(9, ArmoryItem10Button, ArmoryItem10CostText, ArmoryItem10OwnedText, ArmoryItem10SoloConcertoText, ArmoryItem10SymphonicModulationText, ArmoryItem10DuetText);
            UpdateWeaponSlot(10, ArmoryItem11Button, ArmoryItem11CostText, ArmoryItem11OwnedText, ArmoryItem11SoloConcertoText, ArmoryItem11SymphonicModulationText, ArmoryItem11DuetText);
            UpdateWeaponSlot(11, ArmoryItem12Button, ArmoryItem12CostText, ArmoryItem12OwnedText, ArmoryItem12SoloConcertoText, ArmoryItem12SymphonicModulationText, ArmoryItem12DuetText);
            UpdateWeaponSlot(12, ArmoryItem13Button, ArmoryItem13CostText, ArmoryItem13OwnedText, ArmoryItem13SoloConcertoText, ArmoryItem13SymphonicModulationText, ArmoryItem13DuetText);
            UpdateWeaponSlot(13, ArmoryItem14Button, ArmoryItem14CostText, ArmoryItem14OwnedText, ArmoryItem14SoloConcertoText, ArmoryItem14SymphonicModulationText, ArmoryItem14DuetText);
            UpdateWeaponSlot(14, ArmoryItem15Button, ArmoryItem15CostText, ArmoryItem15OwnedText, ArmoryItem15SoloConcertoText, ArmoryItem15SymphonicModulationText, ArmoryItem15DuetText);
            UpdateWeaponSlot(15, ArmoryItem16Button, ArmoryItem16CostText, ArmoryItem16OwnedText, ArmoryItem16SoloConcertoText, ArmoryItem16SymphonicModulationText, ArmoryItem16DuetText);
            UpdateWeaponSlot(16, ArmoryItem17Button, ArmoryItem17CostText, ArmoryItem17OwnedText, ArmoryItem17SoloConcertoText, ArmoryItem17SymphonicModulationText, ArmoryItem17DuetText);
            UpdateWeaponSlot(17, ArmoryItem18Button, ArmoryItem18CostText, ArmoryItem18OwnedText, ArmoryItem18SoloConcertoText, ArmoryItem18SymphonicModulationText, ArmoryItem18DuetText);
            UpdateWeaponSlot(18, ArmoryItem19Button, ArmoryItem19CostText, ArmoryItem19OwnedText, ArmoryItem19SoloConcertoText, ArmoryItem19SymphonicModulationText, ArmoryItem19DuetText);
            UpdateWeaponSlot(19, ArmoryItem20Button, ArmoryItem20CostText, ArmoryItem20OwnedText, ArmoryItem20SoloConcertoText, ArmoryItem20SymphonicModulationText, ArmoryItem20DuetText);
        }

        /// <summary>
        /// Updates the visual appearance and text of a single weapon slot.
        /// Handles locked, available, and purchased states with appropriate styling.
        /// </summary>
        private void UpdateWeaponSlot(int weaponIndex, Button button, TextBlock costText, TextBlock ownedText, TextBlock soloConcertoText, TextBlock symphonicText, TextBlock duetText)
        {
            if (weaponIndex >= _weaponNames.Length)
                return;

            // Check current unlock and ownership status
            bool isUnlocked = IsWeaponUnlocked(weaponIndex);
            bool owned = IsWeaponOwned(weaponIndex);
            
            // Update button text to weapon name
            if (button.Content is TextBlock buttonTextBlock)
            {
                buttonTextBlock.Text = _weaponNames[weaponIndex];
            }

            // Get descriptions from central GameDescriptions
            // Determine which weapon in the pair we are (first or second)
            int pairStart = weaponIndex % 2 == 0 ? weaponIndex : weaponIndex - 1;
            bool isFirstInPair = (weaponIndex % 2) == 0;
            
            string weaponName1 = _weaponNames[pairStart];
            string weaponName2 = pairStart + 1 < _weaponNames.Length ? _weaponNames[pairStart + 1] : "";
            
            string forte1 = GameDescriptions.GetForteDescription(weaponName1);
            string forte2 = !string.IsNullOrEmpty(weaponName2) ? GameDescriptions.GetForteDescription(weaponName2) : "";
            
            // Get duet description based on weapon pair
            string duet = (pairStart / 2) switch
            {
                0 => DuetDescriptions.Short.Moonlight,    // Moonlight (0-1)
                1 => DuetDescriptions.Short.Eroica,       // Eroica (2-3)
                2 => DuetDescriptions.Short.SwanLake,     // Swan Lake (4-5)
                3 => DuetDescriptions.Short.LaCampanella, // La Campanella (6-7)
                4 => DuetDescriptions.Short.Enigma,       // Enigma (8-9)
                5 => DuetDescriptions.Short.Fate,         // Fate (10-11)
                6 => DuetDescriptions.Short.OdeToJoy,     // Ode to Joy (12-13)
                7 => DuetDescriptions.Short.DiesIrae,     // Dies Irae (14-15)
                8 => DuetDescriptions.Short.Winter,       // Winter (16-17)
                _ => "Duet Resonance: <placeholder>"
            };

            // Determine whether both weapons of this pair are owned (duet active)
            bool duetActive = IsWeaponOwned(pairStart) && (pairStart + 1 < _weaponNames.Length) && IsWeaponOwned(pairStart + 1);

                if (isUnlocked)
                {
                    double cost = CalculateWeaponCost(weaponIndex);

                    if (owned)
                    {
                        // Already purchased - show as owned with green tint
                        button.Background = new SolidColorBrush(Color.FromArgb(68, 0, 100, 0)); // Semi-transparent green
                        button.Opacity = 1.0;
                        button.IsEnabled = false; // Disable button (can't buy again)

                        costText.Text = "✓ Purchased";
                        costText.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100)); // Bright green

                        ownedText.Text = "Owned";
                        ownedText.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100));

                        // Show the appropriate Forte line on the first text row
                        soloConcertoText.Text = isFirstInPair ? forte1 : forte2;
                        soloConcertoText.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100));

                        // Show duet on the second text row (centered under image)
                        symphonicText.Text = duet;
                        symphonicText.Foreground = duetActive ? new SolidColorBrush(Color.FromRgb(100, 255, 100)) : Brushes.White;

                        // Clear the auxiliary duet textblock (we only use two lines now)
                        duetText.Text = string.Empty;
                    }
                    else
                    {
                        // Available for purchase - show cost and enable button
                        button.Background = new SolidColorBrush(Color.FromArgb(68, 0, 0, 0)); // Semi-transparent black
                        button.Opacity = 1.0;
                        button.IsEnabled = true;

                        costText.Text = $"Cost: {FormatNumber(cost)} Notes";
                        costText.Foreground = Brushes.White;

                        ownedText.Text = "Not Owned";
                        ownedText.Foreground = Brushes.White;

                        // Show the appropriate Forte line on the first text row
                        soloConcertoText.Text = isFirstInPair ? forte1 : forte2;
                        soloConcertoText.Foreground = Brushes.White;

                        // Show duet on the second text row
                        symphonicText.Text = duet;
                        symphonicText.Foreground = duetActive ? new SolidColorBrush(Color.FromRgb(100, 255, 100)) : Brushes.White;

                        // Clear extra duet textblock
                        duetText.Text = string.Empty;
                    }
                }
                else
                {
                    // Locked - gray out and show requirement
                    button.Background = new SolidColorBrush(Color.FromArgb(68, 60, 60, 60)); // Gray background
                    button.Opacity = 0.5; // Dimmed
                    button.IsEnabled = false; // Disabled

                    costText.Text = $"Requires {_requiredMajors[weaponIndex]}";
                    costText.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180)); // Light gray

                    ownedText.Text = "🔒 Locked";
                    ownedText.Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180));

                    // Show the appropriate Forte line (dimmed) and duet dimmed
                    soloConcertoText.Text = isFirstInPair ? forte1 : forte2;
                    soloConcertoText.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));

                    symphonicText.Text = duet;
                    symphonicText.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));

                    duetText.Text = string.Empty;
                }
        }

        /// <summary>
        /// Formats large numbers with abbreviated suffixes for readability.
        /// E.g., 1,500,000 becomes "1.50M"
        /// </summary>
        private string FormatNumber(double num)
        {
            if (num >= 1e27)
                return $"{num / 1e27:F2}Oc";  // Octillion
            if (num >= 1e24)
                return $"{num / 1e24:F2}Sp";  // Septillion
            if (num >= 1e21)
                return $"{num / 1e21:F2}Sx";  // Sextillion
            if (num >= 1e18)
                return $"{num / 1e18:F2}Qn";  // Quintillion
            if (num >= 1e15)
                return $"{num / 1e15:F2}Qd";  // Quadrillion
            if (num >= 1e12)
                return $"{num / 1e12:F2}T";   // Trillion
            if (num >= 1e9)
                return $"{num / 1e9:F2}B";    // Billion
            if (num >= 1e6)
                return $"{num / 1e6:F2}M";    // Million
            if (num >= 1e3)
                return $"{num / 1e3:F2}K";    // Thousand
            return $"{Math.Round(num, 1)}";    // Less than 1000, show actual number
        }
    }
}
