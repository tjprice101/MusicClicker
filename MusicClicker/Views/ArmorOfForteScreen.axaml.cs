using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace MusicClicker.Views
{
    /// <summary>
    /// Armor of Forte screen manages the weapon shop where players can purchase
    /// 18 different weapons, each tied to specific major scores they own.
    /// </summary>
    public partial class ArmorOfForteScreen : UserControl
    {
        // Reference to the game state for checking/updating player progress
        private GameState? _gameState;

        // Array of all weapon names displayed in the shop (18 total weapons)
        private readonly string[] _weaponNames = new[]
        {
            // Moonlight weapons — user requested Eulogy first, then Incisor
            "Eulogy of the Moon",        // Tier 1 Moonlight weapon (now first)
            "Incisor of Moonlight",      // Tier 2 Moonlight weapon (now second)

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

            // Dies Irae and Winter remain unchanged (events / placeholders)
            "Dies Irae Scythe I",       // Tier 1 Dies Irae weapon (event)
            "Dies Irae Scythe II",      // Tier 2 Dies Irae weapon (event)
            "Winter Bow I",             // Tier 1 Winter weapon (event)
            "Winter Bow II"             // Tier 2 Winter weapon (event)
        };

        // Array mapping each weapon to its required major score
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
            "Dies Irae Major",         // Required for Dies Irae Scythe I & II
            "Dies Irae Major",
            "Winter Major",            // Required for Winter Bow I & II
            "Winter Major"
        };

        /// <summary>
        /// Constructor initializes the armor shop screen and sets up button handlers.
        /// </summary>
        public ArmorOfForteScreen()
        {
            InitializeComponent();

            // Wire up back button to return to main screen
            BackButton.Click += BackButton_Click;
            
            // Set up click handlers for all 18 weapon purchase buttons
            InitializeArmorButtons();
        }

        /// <summary>
        /// Connects each weapon button to its purchase handler.
        /// Each button is linked to its corresponding weapon index (0-17).
        /// </summary>
        private void InitializeArmorButtons()
        {
            ArmorItem1Button.Click += (s, e) => HandleArmorPurchase(0);   // Moonlight Blade I
            ArmorItem2Button.Click += (s, e) => HandleArmorPurchase(1);   // Moonlight Blade II
            ArmorItem3Button.Click += (s, e) => HandleArmorPurchase(2);   // Eroica Sword I
            ArmorItem4Button.Click += (s, e) => HandleArmorPurchase(3);   // Eroica Sword II
            ArmorItem5Button.Click += (s, e) => HandleArmorPurchase(4);   // Swan Lance I
            ArmorItem6Button.Click += (s, e) => HandleArmorPurchase(5);   // Swan Lance II
            ArmorItem7Button.Click += (s, e) => HandleArmorPurchase(6);   // Campanella Dagger I
            ArmorItem8Button.Click += (s, e) => HandleArmorPurchase(7);   // Campanella Dagger II
            ArmorItem9Button.Click += (s, e) => HandleArmorPurchase(8);   // Enigma Staff I
            ArmorItem10Button.Click += (s, e) => HandleArmorPurchase(9);  // Enigma Staff II
            ArmorItem11Button.Click += (s, e) => HandleArmorPurchase(10); // Fate Axe I
            ArmorItem12Button.Click += (s, e) => HandleArmorPurchase(11); // Fate Axe II
            ArmorItem13Button.Click += (s, e) => HandleArmorPurchase(12); // Joy Hammer I
            ArmorItem14Button.Click += (s, e) => HandleArmorPurchase(13); // Joy Hammer II
            ArmorItem15Button.Click += (s, e) => HandleArmorPurchase(14); // Dies Irae Scythe I
            ArmorItem16Button.Click += (s, e) => HandleArmorPurchase(15); // Dies Irae Scythe II
            ArmorItem17Button.Click += (s, e) => HandleArmorPurchase(16); // Winter Bow I
            ArmorItem18Button.Click += (s, e) => HandleArmorPurchase(17); // Winter Bow II
        }

        /// <summary>
        /// Calculates the cost of a weapon based on its index.
        /// Cost formula: (NPS + 250 * 20^weaponNumber) * (NotesPerClick / 2)
        /// Higher weapon numbers = exponentially more expensive.
        /// </summary>
        private double CalculateWeaponCost(int weaponIndex)
        {
            if (_gameState == null)
                return 0;

            // Base multiplier grows exponentially: 250 * 20^(weaponIndex+1)
            // Weapon 0 costs 250*20^1 = 5,000 base
            // Weapon 1 costs 250*20^2 = 100,000 base, etc.
            double baseMultiplier = 250 * Math.Pow(20, weaponIndex + 1);
            
            // Final cost combines passive income (NPS) + exponential base, scaled by click power
            return (_gameState.NotesPerSecond + baseMultiplier) * (_gameState.NotesPerClick / 2);
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
                14 or 15 => false,  // Dies Irae Major - not yet implemented
                16 or 17 => false,  // Winter Major - not yet implemented
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
                0 => _gameState.EulogyOfTheMoon,
                1 => _gameState.IncisorOfMoonlight,
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
                14 => _gameState.DiesIraeScytheI,
                15 => _gameState.DiesIraeScytheII,
                16 => _gameState.WinterBowI,
                17 => _gameState.WinterBowII,
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
                case 0: _gameState.EulogyOfTheMoon = value; break;
                case 1: _gameState.IncisorOfMoonlight = value; break;
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
                case 14: _gameState.DiesIraeScytheI = value; break;
                case 15: _gameState.DiesIraeScytheII = value; break;
                case 16: _gameState.WinterBowI = value; break;
                case 17: _gameState.WinterBowII = value; break;
            }
        }

        /// <summary>
        /// Handles weapon purchase attempt.
        /// Checks if weapon is unlocked, not already owned, and player has enough notes.
        /// </summary>
        private void HandleArmorPurchase(int weaponIndex)
        {
            // Validate game state and weapon is unlocked
            if (_gameState == null || !IsWeaponUnlocked(weaponIndex))
                return;

            // Check if already owned (weapons are one-time purchases)
            if (IsWeaponOwned(weaponIndex))
                return;

            // Calculate cost and check if player can afford it
            double cost = CalculateWeaponCost(weaponIndex);
            
            if (_gameState.Notes >= cost)
            {
                // Deduct cost from player's notes
                _gameState.Notes -= cost;
                
                // Mark weapon as owned
                SetWeaponOwned(weaponIndex, true);
                
                // Refresh UI to show purchase
                UpdateUI(_gameState);
            }
        }

        /// <summary>
        /// Handler for back button - returns to main screen.
        /// </summary>
        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen
            this.IsVisible = false;

            // Navigate up the visual tree to find the parent window
            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            // Show the main screen
            if (current is Window parentWindow)
            {
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
            ArmorNotesText.Text = $"Notes: {FormatNumber(gameState.Notes)}";
            
            // Update all 18 weapon slots with current state
            UpdateWeaponSlot(0, ArmorItem1Button, ArmorItem1CostText, ArmorItem1OwnedText, ArmorItem1SoloConcertoText, ArmorItem1SymphonicModulationText, ArmorItem1DuetText);
            UpdateWeaponSlot(1, ArmorItem2Button, ArmorItem2CostText, ArmorItem2OwnedText, ArmorItem2SoloConcertoText, ArmorItem2SymphonicModulationText, ArmorItem2DuetText);
            UpdateWeaponSlot(2, ArmorItem3Button, ArmorItem3CostText, ArmorItem3OwnedText, ArmorItem3SoloConcertoText, ArmorItem3SymphonicModulationText, ArmorItem3DuetText);
            UpdateWeaponSlot(3, ArmorItem4Button, ArmorItem4CostText, ArmorItem4OwnedText, ArmorItem4SoloConcertoText, ArmorItem4SymphonicModulationText, ArmorItem4DuetText);
            UpdateWeaponSlot(4, ArmorItem5Button, ArmorItem5CostText, ArmorItem5OwnedText, ArmorItem5SoloConcertoText, ArmorItem5SymphonicModulationText, ArmorItem5DuetText);
            UpdateWeaponSlot(5, ArmorItem6Button, ArmorItem6CostText, ArmorItem6OwnedText, ArmorItem6SoloConcertoText, ArmorItem6SymphonicModulationText, ArmorItem6DuetText);
            UpdateWeaponSlot(6, ArmorItem7Button, ArmorItem7CostText, ArmorItem7OwnedText, ArmorItem7SoloConcertoText, ArmorItem7SymphonicModulationText, ArmorItem7DuetText);
            UpdateWeaponSlot(7, ArmorItem8Button, ArmorItem8CostText, ArmorItem8OwnedText, ArmorItem8SoloConcertoText, ArmorItem8SymphonicModulationText, ArmorItem8DuetText);
            UpdateWeaponSlot(8, ArmorItem9Button, ArmorItem9CostText, ArmorItem9OwnedText, ArmorItem9SoloConcertoText, ArmorItem9SymphonicModulationText, ArmorItem9DuetText);
            UpdateWeaponSlot(9, ArmorItem10Button, ArmorItem10CostText, ArmorItem10OwnedText, ArmorItem10SoloConcertoText, ArmorItem10SymphonicModulationText, ArmorItem10DuetText);
            UpdateWeaponSlot(10, ArmorItem11Button, ArmorItem11CostText, ArmorItem11OwnedText, ArmorItem11SoloConcertoText, ArmorItem11SymphonicModulationText, ArmorItem11DuetText);
            UpdateWeaponSlot(11, ArmorItem12Button, ArmorItem12CostText, ArmorItem12OwnedText, ArmorItem12SoloConcertoText, ArmorItem12SymphonicModulationText, ArmorItem12DuetText);
            UpdateWeaponSlot(12, ArmorItem13Button, ArmorItem13CostText, ArmorItem13OwnedText, ArmorItem13SoloConcertoText, ArmorItem13SymphonicModulationText, ArmorItem13DuetText);
            UpdateWeaponSlot(13, ArmorItem14Button, ArmorItem14CostText, ArmorItem14OwnedText, ArmorItem14SoloConcertoText, ArmorItem14SymphonicModulationText, ArmorItem14DuetText);
            UpdateWeaponSlot(14, ArmorItem15Button, ArmorItem15CostText, ArmorItem15OwnedText, ArmorItem15SoloConcertoText, ArmorItem15SymphonicModulationText, ArmorItem15DuetText);
            UpdateWeaponSlot(15, ArmorItem16Button, ArmorItem16CostText, ArmorItem16OwnedText, ArmorItem16SoloConcertoText, ArmorItem16SymphonicModulationText, ArmorItem16DuetText);
            UpdateWeaponSlot(16, ArmorItem17Button, ArmorItem17CostText, ArmorItem17OwnedText, ArmorItem17SoloConcertoText, ArmorItem17SymphonicModulationText, ArmorItem17DuetText);
            UpdateWeaponSlot(17, ArmorItem18Button, ArmorItem18CostText, ArmorItem18OwnedText, ArmorItem18SoloConcertoText, ArmorItem18SymphonicModulationText, ArmorItem18DuetText);
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

            // Prepare resonance strings based on the weapon category (grouped by pairs)
            string forte1 = "";
            string forte2 = "";
            string duet = "";

            switch (weaponIndex)
            {
                // Moonlight (indices 0-1)
                case 0:
                    forte1 = "Forte Resonance: On upgrade purchase, increase your current notes by 5%.";
                    forte2 = "Forte Resonance: On upgrade purchase, increase your lowest owned upgrade by 5.";
                    duet = "Duet Resonance: Every 12th click increases all upgrade values by 1.";
                    break;
                case 1:
                    forte1 = "Forte Resonance: On upgrade purchase, increase your current notes by 5%.";
                    forte2 = "Forte Resonance: On upgrade purchase, increase your lowest owned upgrade by 5.";
                    duet = "Duet Resonance: Every 12th click increases all upgrade values by 1.";
                    break;

                // Eroica (indices 2-3)
                case 2:
                    forte1 = "Forte Resonance: On acquisition of a score's Major sheet, gives 1 of each Minor Component.";
                    forte2 = "Forte Resonance: On resonance of a score's Major version, divide your current notes by 2 and give 3 minor scores of your lowest owned minor score.";
                    duet = "Duet Resonance: If any minor score exceeds 10, double its notes per second output amount (not time).";
                    break;
                case 3:
                    forte1 = "Forte Resonance: On acquisition of a score's Major sheet, gives 1 of each Minor Component.";
                    forte2 = "Forte Resonance: On resonance of a score's Major version, divide your current notes by 2 and give 3 minor scores of your lowest owned minor score.";
                    duet = "Duet Resonance: If any minor score exceeds 10, double its notes per second output amount (not time).";
                    break;

                // Swan Lake (indices 4-5)
                case 4:
                    forte1 = "Forte Resonance: On buy of Melodious Fragment, increase your current notes by your notes per second / 10.";
                    forte2 = "Forte Resonance: On buy of Harmonious Fragment, increase your current notes by your notes per second / 5.";
                    duet = "Duet Resonance: If you own 50 or more Melodious fragments and 100 or more Harmonious fragments, your notes per second is doubled until this condition is not met.";
                    break;
                case 5:
                    forte1 = "Forte Resonance: On buy of Melodious Fragment, increase your current notes by your notes per second / 10.";
                    forte2 = "Forte Resonance: On buy of Harmonious Fragment, increase your current notes by your notes per second / 5.";
                    duet = "Duet Resonance: If you own 50 or more Melodious fragments and 100 or more Harmonious fragments, your notes per second is doubled until this condition is not met.";
                    break;

                // La Campanella (6-7)
                case 6:
                    forte1 = "Forte Resonance: On craft of a minor score, give another of that minor score.";
                    forte2 = "Forte Resonance: On craft of a minor score, increase your notes per second by 50% for the next 5 seconds.";
                    duet = "Duet Resonance: On craft of a Major score, gives 5 of its corresponding minor score.";
                    break;
                case 7:
                    forte1 = "Forte Resonance: On craft of a minor score, give another of that minor score.";
                    forte2 = "Forte Resonance: On craft of a minor score, increase your notes per second by 50% for the next 5 seconds.";
                    duet = "Duet Resonance: On craft of a Major score, gives 5 of its corresponding minor score.";
                    break;

                // Enigma (8-9) - left as placeholders (user provided ???)
                case 8:
                case 9:
                    forte1 = "Forte Resonance: ???";
                    forte2 = "Forte Resonance: ???";
                    duet = "Duet Resonance: ???";
                    break;

                // Fate (10-11)
                case 10:
                    forte1 = "Forte Resonance: Every 25th click gives you a Moonlight Sonata Minor.";
                    forte2 = "Forte Resonance: Every 30th click gives you a Swan Lake Minor.";
                    duet = "Duet Resonance: Every 100th click doubles whichever minor score is the lowest owned.";
                    break;
                case 11:
                    forte1 = "Forte Resonance: Every 25th click gives you a Moonlight Sonata Minor.";
                    forte2 = "Forte Resonance: Every 30th click gives you a Swan Lake Minor.";
                    duet = "Duet Resonance: Every 100th click doubles whichever minor score is the lowest owned.";
                    break;

                // Ode to Joy (12-13)
                case 12:
                    forte1 = "Forte Resonance: Your notes per second are doubled.";
                    forte2 = "Forte Resonance: Every 5th click makes your next click have a 33% increase in notes.";
                    duet = "Duet Resonance: Notes per Second becomes Notes per Half-Second.";
                    break;
                case 13:
                    forte1 = "Forte Resonance: Your notes per second are doubled.";
                    forte2 = "Forte Resonance: Every 5th click makes your next click have a 33% increase in notes.";
                    duet = "Duet Resonance: Notes per Second becomes Notes per Half-Second.";
                    break;

                // Dies Irae & Winter (14-17) - keep existing placeholders
                default:
                    forte1 = "Forte Resonance: <placeholder>";
                    forte2 = "Forte Resonance: <placeholder>";
                    duet = "Duet Resonance: <placeholder>";
                    break;
            }

            // Determine whether both weapons of this pair are owned (duet active)
            int pairStart = weaponIndex % 2 == 0 ? weaponIndex : weaponIndex - 1;
            bool duetActive = IsWeaponOwned(pairStart) && (pairStart + 1 < _weaponNames.Length) && IsWeaponOwned(pairStart + 1);
            // Whether this weapon is the first in its pair (used to choose which forte line to show)
            bool isFirstInPair = (weaponIndex % 2) == 0;

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