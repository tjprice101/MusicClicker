using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Avalonia.Threading;
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
        // Right-side (weapons) UI references
        private readonly StackPanel? _rightDrawerPanel;
        private readonly Border? _equippedWeaponDisplay1;
        private readonly TextBlock? _equippedWeaponText1;
        private readonly Border? _equippedWeaponDisplay2;
        private readonly TextBlock? _equippedWeaponText2;
        private readonly TextBlock? _duetResonanceText;

        // Duet color oscillation state
        private readonly DispatcherTimer _duetColorTimer;
        private bool _duetColorToggle = false;

        // Game state reference
        private readonly GameState _gameState;

        // Equip/disable prompt UI
        private readonly StackPanel _equipPromptPanel;
        private readonly TextBlock _equipPromptText;
        private readonly Button _equipYesButton;
        private readonly Button _equipNoButton;

        // Ordered list of all major scores (implementation order: Base → Event → Boss Fight)
        private readonly List<string> _majorScoreOrder = new()
        {
            // Base Major Scores
            "Moonlight",
            "Eroica",
            "Swan",
            "LaCampanella",
            "Enigma",
            "Fate",
            "OdeToJoy",
            // Event Major Scores
            "DiesIrae",
            "Winter",
            // Boss Fight Major Scores
            "Mercury",
            "ClairDeLune",
            "Mars"
        };

        // Mapping of score names → image asset paths
        private readonly Dictionary<string, string> _majorScoreImages = new()
        {
            {"Moonlight", "avares://MusicClicker/Assets/Music Game 16_9 Assets [978EB92]-min2.png"},
            {"Eroica", "avares://MusicClicker/Assets/Music Game 16_9 Assets [04AC7F2]-min2.png"},
            {"Swan", "avares://MusicClicker/Assets/Music Game 16_9 Assets [876DEE6]-min2.png"},
            {"LaCampanella", "avares://MusicClicker/Assets/LaCampanellaMajor2.png"},
            {"Enigma", "avares://MusicClicker/Assets/EnigmaMajor2.png"},
            {"Fate", "avares://MusicClicker/Assets/FateMajor2.png"},
            {"OdeToJoy", "avares://MusicClicker/Assets/OdeToJoyMajor2.png"},
            {"DiesIrae", "avares://MusicClicker/Assets/DiesIraeMajor.png"},
            {"Winter", "avares://MusicClicker/Assets/WinterMajor.png"},
            {"Mercury", "avares://MusicClicker/Assets/MercuryMajor.jpg"},
            {"ClairDeLune", "avares://MusicClicker/Assets/ClairDeLuneMajor.jpg"},
            {"Mars", "avares://MusicClicker/Assets/MarsMajor.jpg"}
        };

        // Friendly display names for major scores (internal key -> UI string)
        private readonly Dictionary<string, string> _majorScoreDisplayNames = new()
        {
            {"Moonlight", "Moonlight"},
            {"Eroica", "Eroica"},
            {"Swan", "Swan"},
            {"LaCampanella", "La Campanella"},
            {"Enigma", "Enigma"},
            {"Fate", "Fate"},
            {"OdeToJoy", "Ode to Joy"},
            {"DiesIrae", "Dies Irae"},
            {"Winter", "Winter"},
            {"Mercury", "Mercury"},
            {"ClairDeLune", "Clair de Lune"},
            {"Mars", "Mars"}
        };

        // Ordered list of weapon keys (matches ArmoryOfForte screen order)
        // NOTE: Moonlight weapons requested: Incisor first, then Eulogy.
        private readonly List<string> _weaponOrder = new()
        {
            "IncisorOfMoonlight",
            "EulogyOfTheMoon",
            "SakurasBlossom",
            "FuneralPrayer",
            "StarScatteredWings",
            "ThousandWingedSwan",
            "SymphonyOfBells",
            "RazerOfBellsChimes",
            "CreatorOfMystery",
            "Truthseeker",
            "AstralChainripper",
            "CosmicWeaver",
            "JoyfulCatharsis",
            "OdeToCreation",
            "SevenCircles",
            "HellsWrath",
            "CacophonicBlizzard",
            "TheSnowsDesire"
        };

        // Mapping of weapon names → image asset paths (Armory of Forte)
        private readonly Dictionary<string, string> _weaponImages = new()
        {
            {"EulogyOfTheMoon", "avares://MusicClicker/Assets/EulogyOfTheMoon.png"},
            {"IncisorOfMoonlight", "avares://MusicClicker/Assets/IncisorOfMoonlight.png"},
            {"SakurasBlossom", "avares://MusicClicker/Assets/Sakura'sBlossom.png"},
            {"FuneralPrayer", "avares://MusicClicker/Assets/FuneralPrayer.png"},
            {"StarScatteredWings", "avares://MusicClicker/Assets/StarScatteredWings.png"},
            {"ThousandWingedSwan", "avares://MusicClicker/Assets/ThousandWingedSwan.png"},
            {"SymphonyOfBells", "avares://MusicClicker/Assets/SymphonyOfBells.png"},
            {"RazerOfBellsChimes", "avares://MusicClicker/Assets/RazeofBellsChimes.png"},
            {"CreatorOfMystery", "avares://MusicClicker/Assets/CreatorofMystery.png"},
            {"Truthseeker", "avares://MusicClicker/Assets/Truthseeker.png"},
            {"AstralChainripper", "avares://MusicClicker/Assets/AstralChainripper.png"},
            {"CosmicWeaver", "avares://MusicClicker/Assets/CosmicWeaver.png"},
            {"JoyfulCatharsis", "avares://MusicClicker/Assets/JoyfulCatharsis.png"},
            {"OdeToCreation", "avares://MusicClicker/Assets/OdeToCreation.png"},
            {"SevenCircles", "avares://MusicClicker/Assets/SevenCircles.png"},
            {"HellsWrath", "avares://MusicClicker/Assets/Hell'sWrath.png"},
            {"CacophonicBlizzard", "avares://MusicClicker/Assets/CacophonicBlizzard.png"},
            {"TheSnowsDesire", "avares://MusicClicker/Assets/TheSwan'sDesire.png"}
        };

        // Friendly display names for weapons (internal key -> UI string)
        private readonly Dictionary<string, string> _weaponDisplayNames = new()
        {
            {"EulogyOfTheMoon", "Eulogy of the Moon"},
            {"IncisorOfMoonlight", "Incisor of Moonlight"},
            {"SakurasBlossom", "Sakura's Blossom"},
            {"FuneralPrayer", "Funeral Prayer"},
            {"StarScatteredWings", "Star-Scattered Wings"},
            {"ThousandWingedSwan", "Thousand Winged Swan"},
            {"SymphonyOfBells", "Symphony of Bells"},
            {"RazerOfBellsChimes", "Razer of Bell's Chimes"},
            {"CreatorOfMystery", "Creator of Mystery"},
            {"Truthseeker", "Truthseeker"},
            {"AstralChainripper", "Astral Chainripper"},
            {"CosmicWeaver", "Cosmic Weaver"},
            {"JoyfulCatharsis", "Joyful Catharsis"},
            {"OdeToCreation", "Ode to Creation"},
            {"SevenCircles", "Seven Circles"},
            {"HellsWrath", "Hell's Wrath"},
            {"CacophonicBlizzard", "Cacophonic Blizzard"},
            {"TheSnowsDesire", "The Snow's Desire"}
        };

        // Cache of already-loaded bitmaps
        private readonly Dictionary<string, Bitmap> _bitmapCache = new();
        private readonly Dictionary<string, Bitmap> _weaponBitmapCache = new();

        // Currently equipped bitmap and placeholder bitmap
        private Bitmap? _equippedBitmap;
        private Bitmap? _emptyBitmap;

        public TempoResonateManager(
            StackPanel leftDrawerPanel, Border equippedDisplay, TextBlock equippedText, GameState gameState,
            StackPanel equipPromptPanel, TextBlock equipPromptText, Button equipYesButton, Button equipNoButton,
            StackPanel? rightDrawerPanel = null, Border? equippedWeaponDisplay1 = null, TextBlock? equippedWeaponText1 = null,
            Border? equippedWeaponDisplay2 = null, TextBlock? equippedWeaponText2 = null, TextBlock? duetResonanceText = null)
        {
            // Store UI and game state references
            _leftDrawerPanel = leftDrawerPanel;
            _equippedDisplay = equippedDisplay;
            _equippedText = equippedText;
            _rightDrawerPanel = rightDrawerPanel;
            _equippedWeaponDisplay1 = equippedWeaponDisplay1;
            _equippedWeaponText1 = equippedWeaponText1;
            _equippedWeaponDisplay2 = equippedWeaponDisplay2;
            _equippedWeaponText2 = equippedWeaponText2;
            _duetResonanceText = duetResonanceText;
            _gameState = gameState;

            _equipPromptPanel = equipPromptPanel;
            _equipPromptText = equipPromptText;
            _equipYesButton = equipYesButton;
            _equipNoButton = equipNoButton;

            // Initialize duet color oscillation timer
            _duetColorTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _duetColorTimer.Tick += (s, e) =>
            {
                _duetColorToggle = !_duetColorToggle;
                UpdateDuetTextColor();
            };
            _duetColorTimer.Start();

            // Load images, populate drawer, and set initial equipped state
            LoadBitmaps();
            InitializeDrawer();
            InitializeWeaponDrawer();
            RestoreSavedState();
        }

        private void LoadBitmaps()
        {
            // Preload all major score images into the bitmap cache.
            // Use TryGetValue / single dictionary access per key to reduce hash lookups.
            foreach (var kvp in _majorScoreImages)
            {
                if (!_bitmapCache.TryGetValue(kvp.Key, out _))
                {
                    var bmp = MusicClicker.Helpers.ImageHelpers.GetBitmap(kvp.Value, 256);
                    if (bmp != null)
                        _bitmapCache[kvp.Key] = bmp;
                }
            }

            // Load placeholder image for the "None" equipped state once and reuse it.
            _emptyBitmap = MusicClicker.Helpers.ImageHelpers.GetBitmap("avares://MusicClicker/Assets/EmptyResonate.png", 256);

            // Preload weapon images
            foreach (var kvp in _weaponImages)
            {
                if (!_weaponBitmapCache.TryGetValue(kvp.Key, out _))
                {
                    var bmp = MusicClicker.Helpers.ImageHelpers.GetBitmap(kvp.Value, 256);
                    if (bmp != null)
                        _weaponBitmapCache[kvp.Key] = bmp;
                }
            }
        }

        private void RestoreSavedState()
        {
            // Restore the previously saved resonated score
            string savedScore = _gameState.CurrentResonatedScore;

            if (savedScore == "None" || string.IsNullOrEmpty(savedScore))
            {
                SetDefaultEquipped();
            }
            else if (_bitmapCache.ContainsKey(savedScore) && OwnsScore(savedScore))
            {
                // Restore the saved score
                EquipScore(savedScore, saveToState: false); // Don't re-save during restoration
            }
            else
            {
                // If saved score is invalid or not owned, default to None
                SetDefaultEquipped();
            }
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

            // Update GameState
            _gameState.CurrentResonatedScore = "None";
        }

        private void InitializeDrawer()
        {
            // Clear drawer + set vertical orientation
            _leftDrawerPanel.Children.Clear();
            _leftDrawerPanel.Orientation = Avalonia.Layout.Orientation.Vertical;

            // Create entry button ONLY for owned scores, in the defined order
            foreach (var score in _majorScoreOrder)
            {
                // Skip if not owned
                if (!OwnsScore(score)) continue;

                // Get the image asset path
                if (!_majorScoreImages.TryGetValue(score, out var imagePath)) continue;

                // Try fetch the preloaded bitmap from the cache
                _bitmapCache.TryGetValue(score, out var bitmap);
                var imageSource = bitmap ?? _emptyBitmap;

                // Image for button content - use helper to ensure smooth scaling and consistent settings
                var imageControl = MusicClicker.Helpers.ImageHelpers.CreateSmoothImage(imageSource, 256, 144, score, 1.0, true);

                // Transparent button overlay for click handling
                var button = new Button
                {
                    Width = 256,
                    Height = 144,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(5),
                    Content = imageControl,
                    IsEnabled = true
                };

                // Capture the score name in a local variable to avoid closure issues
                string capturedScore = score;
                
                // Clicking toggles equip/disable prompt
                button.Click += (_, _) =>
                {
                    if (_equippedText.Text == capturedScore)
                    {
                        ShowDisablePrompt(capturedScore);
                    }
                    else
                    {
                        ShowEquipPrompt(capturedScore);
                    }
                };

                _leftDrawerPanel.Children.Add(button);
            }
        }

        private void InitializeWeaponDrawer()
        {
            if (_rightDrawerPanel == null) return;

            _rightDrawerPanel.Children.Clear();
            _rightDrawerPanel.Orientation = Avalonia.Layout.Orientation.Vertical;

            // Create entry button ONLY for owned weapons, in the defined order
            foreach (var weapon in _weaponOrder)
            {
                // Skip if not owned
                if (!OwnsWeapon(weapon)) continue;

                _weaponBitmapCache.TryGetValue(weapon, out var bitmap);
                var imageSource = bitmap ?? _emptyBitmap;

                var imageControl = MusicClicker.Helpers.ImageHelpers.CreateSmoothImage(imageSource, 256, 144, weapon, 1.0, true);

                var button = new Button
                {
                    Width = 256,
                    Height = 144,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(5),
                    Content = imageControl,
                    IsEnabled = true
                };

                // Capture the weapon name in a local variable to avoid closure issues
                string capturedWeapon = weapon;

                button.Click += (_, _) =>
                {
                    // If already equipped, ask to disable; otherwise ask to equip
                    if (_gameState.CurrentResonatedWeapon1 == capturedWeapon || _gameState.CurrentResonatedWeapon2 == capturedWeapon)
                    {
                        ShowWeaponDisablePrompt(capturedWeapon);
                    }
                    else
                    {
                        ShowWeaponEquipPrompt(capturedWeapon);
                    }
                };

                _rightDrawerPanel.Children.Add(button);
            }
        }

        private bool OwnsWeapon(string weapon)
        {
            return weapon switch
            {
                "EulogyOfTheMoon" => _gameState.EulogyOfTheMoon,
                "IncisorOfMoonlight" => _gameState.IncisorOfMoonlight,
                "SakurasBlossom" => _gameState.SakurasBlossom,
                "FuneralPrayer" => _gameState.FuneralPrayer,
                "StarScatteredWings" => _gameState.StarScatteredWings,
                "ThousandWingedSwan" => _gameState.ThousandWingedSwan,
                "SymphonyOfBells" => _gameState.SymphonyOfBells,
                "RazerOfBellsChimes" => _gameState.RazerOfBellsChimes,
                "CreatorOfMystery" => _gameState.CreatorOfMystery,
                "Truthseeker" => _gameState.Truthseeker,
                "AstralChainripper" => _gameState.AstralChainripper,
                "CosmicWeaver" => _gameState.CosmicWeaver,
                "JoyfulCatharsis" => _gameState.JoyfulCatharsis,
                "OdeToCreation" => _gameState.OdeToCreation,
                "SevenCircles" => _gameState.SevenCircles,
                "HellsWrath" => _gameState.HellsWrath,
                "CacophonicBlizzard" => _gameState.CacophonicBlizzard,
                "TheSnowsDesire" => _gameState.TheSnowsDesire,
                _ => false
            };
        }

        private void RefreshWeaponDrawer()
        {
            // Rebuild the entire weapon drawer to reflect current ownership state
            InitializeWeaponDrawer();
        }

        private void ShowWeaponEquipPrompt(string weaponName)
        {
            if (_equipPromptPanel == null) return;
            _equipPromptPanel.IsVisible = true;
            var display = _weaponDisplayNames.TryGetValue(weaponName, out var d) ? d : weaponName;
            _equipPromptText.Text = $"Resonate with {display}?";

            _equipYesButton.Click -= WeaponEquipYesHandler;
            _equipNoButton.Click -= WeaponEquipNoHandler;

            void WeaponEquipYesHandler(object? s, Avalonia.Interactivity.RoutedEventArgs e)
            {
                EquipWeapon(weaponName);
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= WeaponEquipYesHandler;
                _equipNoButton.Click -= WeaponEquipNoHandler;
            }

            void WeaponEquipNoHandler(object? s, Avalonia.Interactivity.RoutedEventArgs e)
            {
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= WeaponEquipYesHandler;
                _equipNoButton.Click -= WeaponEquipNoHandler;
            }

            _equipYesButton.Click += WeaponEquipYesHandler;
            _equipNoButton.Click += WeaponEquipNoHandler;
        }

        private void ShowWeaponDisablePrompt(string weaponName)
        {
            if (_equipPromptPanel == null) return;
            _equipPromptPanel.IsVisible = true;
            var display = _weaponDisplayNames.TryGetValue(weaponName, out var d) ? d : weaponName;
            _equipPromptText.Text = $"Do you want to disable {display}'s resonance?";

            _equipYesButton.Click -= WeaponDisableYesHandler;
            _equipNoButton.Click -= WeaponDisableNoHandler;

            void WeaponDisableYesHandler(object? s, Avalonia.Interactivity.RoutedEventArgs e)
            {
                if (_gameState.CurrentResonatedWeapon1 == weaponName) UnequipWeapon(1);
                if (_gameState.CurrentResonatedWeapon2 == weaponName) UnequipWeapon(2);
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= WeaponDisableYesHandler;
                _equipNoButton.Click -= WeaponDisableNoHandler;
            }

            void WeaponDisableNoHandler(object? s, Avalonia.Interactivity.RoutedEventArgs e)
            {
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= WeaponDisableYesHandler;
                _equipNoButton.Click -= WeaponDisableNoHandler;
            }

            _equipYesButton.Click += WeaponDisableYesHandler;
            _equipNoButton.Click += WeaponDisableNoHandler;
        }

        private void EquipWeapon(string weaponName)
        {
            // Try to equip into first empty slot, otherwise replace slot1
            if (_gameState.CurrentResonatedWeapon1 == "None")
            {
                _gameState.CurrentResonatedWeapon1 = weaponName;
                SetWeaponSlotDisplay(1, weaponName);
            }
            else if (_gameState.CurrentResonatedWeapon2 == "None")
            {
                _gameState.CurrentResonatedWeapon2 = weaponName;
                SetWeaponSlotDisplay(2, weaponName);
            }
            else
            {
                // replace slot1 by default
                UnequipWeapon(1);
                _gameState.CurrentResonatedWeapon1 = weaponName;
                SetWeaponSlotDisplay(1, weaponName);
            }

            // Enable the weapon ability flag (ownership is permanent, only toggle abilities)
            switch (weaponName)
            {
                case "EulogyOfTheMoon": _gameState.EulogyOfTheMoonAbility = true; break;
                case "IncisorOfMoonlight": _gameState.IncisorOfMoonlightAbility = true; break;
                case "SakurasBlossom": _gameState.SakurasBlossomAbility = true; break;
                case "FuneralPrayer": _gameState.FuneralPrayerAbility = true; break;
                case "StarScatteredWings": _gameState.StarScatteredWingsAbility = true; break;
                case "ThousandWingedSwan": _gameState.ThousandWingedSwanAbility = true; break;
                case "SymphonyOfBells": _gameState.SymphonyOfBellsAbility = true; break;
                case "RazerOfBellsChimes": _gameState.RazerOfBellsChimesAbility = true; break;
                case "CreatorOfMystery": _gameState.CreatorOfMysteryAbility = true; break;
                case "Truthseeker": _gameState.TruthseekerAbility = true; break;
                case "AstralChainripper": _gameState.AstralChainripperAbility = true; break;
                case "CosmicWeaver": _gameState.CosmicWeaverAbility = true; break;
                case "JoyfulCatharsis": _gameState.JoyfulCatharsisAbility = true; break;
                case "OdeToCreation": _gameState.OdeToCreationAbility = true; break;
                case "SevenCircles": _gameState.SevenCirclesAbility = true; break;
                case "HellsWrath": _gameState.HellsWrathAbility = true; break;
                case "CacophonicBlizzard": _gameState.CacophonicBlizzardAbility = true; break;
                case "TheSnowsDesire": _gameState.TheSnowsDesireAbility = true; break;
            }

            // Update duet resonance text
            UpdateDuetResonanceText();
        }

        private void UnequipWeapon(int slot)
        {
            string current = slot == 1 ? _gameState.CurrentResonatedWeapon1 : _gameState.CurrentResonatedWeapon2;
            if (current == "None") return;

            // Disable ability flag (ownership remains permanent, only toggle abilities)
            switch (current)
            {
                case "EulogyOfTheMoon": _gameState.EulogyOfTheMoonAbility = false; break;
                case "IncisorOfMoonlight": _gameState.IncisorOfMoonlightAbility = false; break;
                case "SakurasBlossom": _gameState.SakurasBlossomAbility = false; break;
                case "FuneralPrayer": _gameState.FuneralPrayerAbility = false; break;
                case "StarScatteredWings": _gameState.StarScatteredWingsAbility = false; break;
                case "ThousandWingedSwan": _gameState.ThousandWingedSwanAbility = false; break;
                case "SymphonyOfBells": _gameState.SymphonyOfBellsAbility = false; break;
                case "RazerOfBellsChimes": _gameState.RazerOfBellsChimesAbility = false; break;
                case "CreatorOfMystery": _gameState.CreatorOfMysteryAbility = false; break;
                case "Truthseeker": _gameState.TruthseekerAbility = false; break;
                case "AstralChainripper": _gameState.AstralChainripperAbility = false; break;
                case "CosmicWeaver": _gameState.CosmicWeaverAbility = false; break;
                case "JoyfulCatharsis": _gameState.JoyfulCatharsisAbility = false; break;
                case "OdeToCreation": _gameState.OdeToCreationAbility = false; break;
                case "SevenCircles": _gameState.SevenCirclesAbility = false; break;
                case "HellsWrath": _gameState.HellsWrathAbility = false; break;
                case "CacophonicBlizzard": _gameState.CacophonicBlizzardAbility = false; break;
                case "TheSnowsDesire": _gameState.TheSnowsDesireAbility = false; break;
            }

            if (slot == 1)
            {
                _gameState.CurrentResonatedWeapon1 = "None";
                SetWeaponSlotDisplay(1, "None");
            }
            else
            {
                _gameState.CurrentResonatedWeapon2 = "None";
                SetWeaponSlotDisplay(2, "None");
            }
        }

        private void SetWeaponSlotDisplay(int slot, string weaponName)
        {
            Border? targetBorder = slot == 1 ? _equippedWeaponDisplay1 : _equippedWeaponDisplay2;
            TextBlock? targetText = slot == 1 ? _equippedWeaponText1 : _equippedWeaponText2;

            if (targetBorder == null || targetText == null)
                return;

            if (weaponName == "None")
            {
                targetText.Text = "Weapon Slot " + slot;
                targetBorder.Child = new Image { Source = _emptyBitmap, Width = targetBorder.Width, Height = targetBorder.Height, Stretch = Stretch.UniformToFill };
                return;
            }

            if (_weaponBitmapCache.TryGetValue(weaponName, out var bmp))
            {
                targetBorder.Child = MusicClicker.Helpers.ImageHelpers.CreateSmoothImage(bmp, (int)targetBorder.Width, (int)targetBorder.Height, null, 1.0, true);
                targetText.Text = _weaponDisplayNames.TryGetValue(weaponName, out var disp) ? disp : weaponName;
            }
            else
            {
                targetText.Text = _weaponDisplayNames.TryGetValue(weaponName, out var disp2) ? disp2 : weaponName;
                targetBorder.Child = new Image { Source = _emptyBitmap, Width = targetBorder.Width, Height = targetBorder.Height, Stretch = Stretch.UniformToFill };
            }

            // Update duet resonance text
            UpdateDuetResonanceText();
        }

        private void UpdateDuetResonanceText()
        {
            if (_duetResonanceText == null) return;

            string weapon1 = _gameState.CurrentResonatedWeapon1;
            string weapon2 = _gameState.CurrentResonatedWeapon2;

            // Check if both weapons are equipped and from the same score
            string duetText = GetDuetText(weapon1, weapon2);
            
            if (!string.IsNullOrEmpty(duetText))
            {
                _duetResonanceText.Text = duetText;
                _duetResonanceText.IsVisible = true;
                UpdateDuetTextColor();
            }
            else
            {
                _duetResonanceText.Text = "";
                _duetResonanceText.IsVisible = false;
            }
        }

        private string GetDuetText(string weapon1, string weapon2)
        {
            // Check for matching pairs from the same score
            if ((weapon1 == "IncisorOfMoonlight" && weapon2 == "EulogyOfTheMoon") ||
                (weapon1 == "EulogyOfTheMoon" && weapon2 == "IncisorOfMoonlight"))
            {
                return DuetDescriptions.Compact.Moonlight;
            }

            if ((weapon1 == "SakurasBlossom" && weapon2 == "FuneralPrayer") ||
                (weapon1 == "FuneralPrayer" && weapon2 == "SakurasBlossom"))
            {
                return DuetDescriptions.Compact.Eroica;
            }

            if ((weapon1 == "StarScatteredWings" && weapon2 == "ThousandWingedSwan") ||
                (weapon1 == "ThousandWingedSwan" && weapon2 == "StarScatteredWings"))
            {
                return DuetDescriptions.Compact.SwanLake;
            }

            if ((weapon1 == "SymphonyOfBells" && weapon2 == "RazerOfBellsChimes") ||
                (weapon1 == "RazerOfBellsChimes" && weapon2 == "SymphonyOfBells"))
            {
                return DuetDescriptions.Compact.LaCampanella;
            }

            if ((weapon1 == "CreatorOfMystery" && weapon2 == "Truthseeker") ||
                (weapon1 == "Truthseeker" && weapon2 == "CreatorOfMystery"))
            {
                return DuetDescriptions.Compact.Enigma;
            }

            if ((weapon1 == "AstralChainripper" && weapon2 == "CosmicWeaver") ||
                (weapon1 == "CosmicWeaver" && weapon2 == "AstralChainripper"))
            {
                return DuetDescriptions.Compact.Fate;
            }

            if ((weapon1 == "JoyfulCatharsis" && weapon2 == "OdeToCreation") ||
                (weapon1 == "OdeToCreation" && weapon2 == "JoyfulCatharsis"))
            {
                return DuetDescriptions.Compact.OdeToJoy;
            }

            if ((weapon1 == "SevenCircles" && weapon2 == "HellsWrath") ||
                (weapon1 == "HellsWrath" && weapon2 == "SevenCircles"))
            {
                return DuetDescriptions.Compact.DiesIrae;
            }

            if ((weapon1 == "CacophonicBlizzard" && weapon2 == "TheSnowsDesire") ||
                (weapon1 == "TheSnowsDesire" && weapon2 == "CacophonicBlizzard"))
            {
                return DuetDescriptions.Compact.Winter;
            }

            return "";
        }

        private void UpdateDuetTextColor()
        {
            if (_duetResonanceText == null || !_duetResonanceText.IsVisible) return;

            _duetResonanceText.Foreground = _duetColorToggle 
                ? new SolidColorBrush(Color.FromRgb(255, 192, 203))  // Pink
                : Brushes.White;
        }

        public void RefreshDrawer()
        {
            // Rebuild the entire drawer to reflect current ownership state
            InitializeDrawer();
        }

        // Checks whether the player owns the specified major score
        private bool OwnsScore(string score)
        {
            return score switch
            {
                // Base Major Scores
                "Moonlight" => _gameState.MoonlightMajorOwned > 0,
                "Eroica" => _gameState.EroicaMajorOwned > 0,
                "Swan" => _gameState.SwanMajorOwned > 0,
                "LaCampanella" => _gameState.LaCampanellaMajorOwned > 0,
                "Enigma" => _gameState.EnigmaMajorOwned > 0,
                "Fate" => _gameState.FateMajorOwned > 0,
                "OdeToJoy" => _gameState.OdeToJoyMajorOwned > 0,
                // Event Major Scores
                "DiesIrae" => _gameState.DiesIraeOwned > 0,
                "Winter" => _gameState.WinterOwned > 0,
                // Boss Fight Major Scores
                "Mercury" => _gameState.MercuryMajorOwned > 0,
                "ClairDeLune" => _gameState.ClairDeLuneMajorOwned > 0,
                "Mars" => _gameState.MarsMajorOwned > 0,
                _ => false
            };
        }

        private void ShowEquipPrompt(string scoreName)
        {
            // Show prompt asking to equip a score
            _equipPromptPanel.IsVisible = true;
            var displayName = _majorScoreDisplayNames.TryGetValue(scoreName, out var name) ? name : scoreName;
            _equipPromptText.Text = $"Resonate with {displayName}'s Tempo?";

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
            var displayName = _majorScoreDisplayNames.TryGetValue(scoreName, out var name) ? name : scoreName;
            _equipPromptText.Text = $"Do you want to disable {displayName}'s resonance?";

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

        private void EquipScore(string scoreName, bool saveToState = true)
        {
            // Set new equipped score text + bitmap
            _equippedText.Text = scoreName;
            _equippedBitmap = _bitmapCache[scoreName];

            _equippedDisplay.Child = MusicClicker.Helpers.ImageHelpers.CreateSmoothImage(_equippedBitmap, 384, 216, null, 1.0, true);

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

            // Save to GameState
            if (saveToState)
            {
                _gameState.CurrentResonatedScore = scoreName;
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

            // Save to GameState
            _gameState.CurrentResonatedScore = "None";
        }

        public void Dispose()
        {
            // Destructor-safe cleanup pattern
            if (_disposed) return;
            _disposed = true;
            _duetColorTimer?.Stop();
            GC.SuppressFinalize(this);
        }

        // Called frequently (e.g., each UI tick) to refresh visual state and
        // ensure the currently equipped score is still valid (owned). If the
        // equipped score is no longer owned, it will be reset to the default
        // display so the tempo-resonate image doesn't show a non-owned score.
        public void TickUpdate()
        {
            try
            {
                // Check if currently equipped score is still valid
                string current = _gameState.CurrentResonatedScore ?? "None";
                if (!string.IsNullOrEmpty(current) && current != "None" && (!_majorScoreImages.ContainsKey(current) || !OwnsScore(current)))
                {
                    // Reset equipped display to default (None)
                    SetDefaultEquipped();
                }

                // Update equipped slot displays only (don't rebuild entire drawer)
                SetWeaponSlotDisplay(1, _gameState.CurrentResonatedWeapon1);
                SetWeaponSlotDisplay(2, _gameState.CurrentResonatedWeapon2);

                // Ensure currently resonated weapons are still owned
                if (!string.IsNullOrEmpty(_gameState.CurrentResonatedWeapon1) && _gameState.CurrentResonatedWeapon1 != "None")
                {
                    if (!_weaponImages.ContainsKey(_gameState.CurrentResonatedWeapon1) || !OwnsWeapon(_gameState.CurrentResonatedWeapon1))
                    {
                        UnequipWeapon(1);
                    }
                }
                if (!string.IsNullOrEmpty(_gameState.CurrentResonatedWeapon2) && _gameState.CurrentResonatedWeapon2 != "None")
                {
                    if (!_weaponImages.ContainsKey(_gameState.CurrentResonatedWeapon2) || !OwnsWeapon(_gameState.CurrentResonatedWeapon2))
                    {
                        UnequipWeapon(2);
                    }
                }
            }
            catch { }
        }
    }
}