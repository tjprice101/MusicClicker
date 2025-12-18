using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Avalonia.Threading;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using MusicClicker.Views;

namespace MusicClicker
{
    public class TempoResonateManager : IDisposable
    {
        private bool _disposed = false;

        // UI references
        private readonly TempoResonateScreen? _screen;
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
        
        // Themed panel references for dynamic coloring
        private Border? _leftPanelBorder;
        private Border? _leftPanelHeader;
        private TextBlock? _leftPanelHeaderText;
        private Border? _rightPanelBorder;
        private Border? _rightPanelHeader;
        private TextBlock? _rightPanelHeaderText;
        
        // Performance optimization: cache last known state to avoid redundant updates
        private string _lastEquippedScore = "";
        private string _lastWeapon1 = "";
        private string _lastWeapon2 = "";
        private int _tickCounter = 0;
        
        // Cache image controls to avoid recreating them on every refresh
        private readonly Dictionary<string, Image> _scoreImageCache = new();
        private readonly Dictionary<string, Image> _weaponImageCache = new();
        private bool _drawersInitialized = false;

        // Game state reference
        private readonly GameState _gameState;

        // Equip/disable prompt UI
        private readonly Border _equipPromptPanel;
        private readonly TextBlock _equipPromptText;
        private readonly Button _equipYesButton;
        private readonly Button _equipNoButton;

        // Ordered list of all major scores (implementation order: Base → Event → Boss Fight)
        private readonly List<string> _majorScoreOrder = new()
        {
            // Base Major Scores
            "Moonlight Sonata",
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
            {"Moonlight Sonata", "avares://MusicClicker/Assets/Music Game 16_9 Assets [978EB92]-min2.png"},
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
            {"Moonlight Sonata", "Moonlight Sonata"},
            {"Eroica", "Eroica"},
            {"Swan", "Swan Lake"},
            {"LaCampanella", "La Campanella"},
            {"Enigma", "Enigma Variations"},
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
            Border equipPromptPanel, TextBlock equipPromptText, Button equipYesButton, Button equipNoButton,
            StackPanel? rightDrawerPanel = null, Border? equippedWeaponDisplay1 = null, TextBlock? equippedWeaponText1 = null,
            Border? equippedWeaponDisplay2 = null, TextBlock? equippedWeaponText2 = null, TextBlock? duetResonanceText = null,
            TempoResonateScreen? screen = null)
        {
            // Store UI and game state references
            _screen = screen;
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

            // Get themed panel references for dynamic coloring
            if (_screen != null)
            {
                _leftPanelBorder = _screen.FindControl<Border>("LeftPanelBorder");
                _leftPanelHeader = _screen.FindControl<Border>("LeftPanelHeader");
                _leftPanelHeaderText = _screen.FindControl<TextBlock>("LeftPanelHeaderText");
                _rightPanelBorder = _screen.FindControl<Border>("RightPanelBorder");
                _rightPanelHeader = _screen.FindControl<Border>("RightPanelHeader");
                _rightPanelHeaderText = _screen.FindControl<TextBlock>("RightPanelHeaderText");
            }

            // Load images, populate drawer, and set initial equipped state
            LoadBitmaps();
            InitializeDrawer();
            InitializeWeaponDrawer();
            RestoreSavedState();
            UpdateThemeColors(); // Set initial theme
        }
        
        // Removed SetupDropZones - all drag-and-drop functionality removed

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
            
            // Restore equipped weapons
            var weapon1 = _gameState.CurrentResonatedWeapon1;
            var weapon2 = _gameState.CurrentResonatedWeapon2;
            if (!string.IsNullOrEmpty(weapon1) && weapon1 != "None")
                SetWeaponSlotDisplay(1, weapon1);
            else
                SetWeaponSlotDisplay(1, "None");
                
            if (!string.IsNullOrEmpty(weapon2) && weapon2 != "None")
                SetWeaponSlotDisplay(2, weapon2);
            else
                SetWeaponSlotDisplay(2, "None");
        }

        private void SetDefaultEquipped()
        {
            // Display default "None" state
            _equippedText.Text = "None";
            _equippedBitmap = _emptyBitmap;

            // Update the equipped display image with 16:9 dimensions
            _equippedDisplay.Child = new Image
            {
                Source = _emptyBitmap,
                Width = 480,
                Height = 270,
                Stretch = Stretch.Uniform
            };
            
            // Initialize weapon slots with empty images
            SetWeaponSlotDisplay(1, "None");
            SetWeaponSlotDisplay(2, "None");

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
                // Skip if not owned - only show owned scores
                if (!OwnsScore(score)) continue;

                // Get the image asset path
                if (!_majorScoreImages.TryGetValue(score, out var imagePath)) continue;

                // Try fetch the preloaded bitmap from the cache
                _bitmapCache.TryGetValue(score, out var bitmap);
                var imageSource = bitmap ?? _emptyBitmap;

                // Create draggable image (no button wrapper needed)
                var imageControl = new Image
                {
                    Source = imageSource,
                    Width = 256,
                    Height = 144,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(5),
                    Tag = score,
                    Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                    UseLayoutRounding = true
                };
                
                // Apply high quality rendering
                RenderOptions.SetBitmapInterpolationMode(imageControl, BitmapInterpolationMode.HighQuality);

                // Capture the score name
                string capturedScore = score;
                
                // Click to equip/unequip (single click for responsiveness)
                imageControl.Tapped += (_, _) =>
                {
                    if (_equippedText.Text == GetScoreDisplayName(capturedScore))
                    {
                        ShowDisablePrompt(capturedScore);
                    }
                    else
                    {
                        ShowEquipPrompt(capturedScore);
                    }
                };

                _leftDrawerPanel.Children.Add(imageControl);
                
                // Register tooltip for this score
                _screen?.RegisterTooltipItem(imageControl, ScoreToDisplayName(score));
            }
        }

        private void InitializeWeaponDrawer()
        {
            if (_rightDrawerPanel == null) return;

            _rightDrawerPanel.Children.Clear();
            _rightDrawerPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
            
            // If already initialized and images are cached, reuse them
            if (_drawersInitialized && _weaponImageCache.Count > 0)
            {
                foreach (var weapon in _weaponOrder)
                {
                    if (_weaponImageCache.TryGetValue(weapon, out var cachedImage))
                    {
                        // Update opacity based on current ownership
                        bool isOwned = OwnsWeapon(weapon);
                        cachedImage.Opacity = isOwned ? 1.0 : 0.3;
                        cachedImage.IsEnabled = isOwned;
                        _rightDrawerPanel.Children.Add(cachedImage);
                    }
                }
                return;
            }

            // Create entry for ALL weapons, grayed out and monochrome if not owned
            foreach (var weapon in _weaponOrder)
            {
                bool isOwned = OwnsWeapon(weapon);

                _weaponBitmapCache.TryGetValue(weapon, out var bitmap);
                var imageSource = bitmap ?? _emptyBitmap;

                // Create draggable image (no button wrapper)
                var imageControl = new Image
                {
                    Source = imageSource,
                    Width = 256,
                    Height = 144,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(5),
                    Tag = weapon,
                    Opacity = isOwned ? 1.0 : 0.3,
                    IsEnabled = isOwned,
                    Cursor = isOwned ? new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand) : Avalonia.Input.Cursor.Default,
                    UseLayoutRounding = true
                };
                
                // Apply high quality rendering
                RenderOptions.SetBitmapInterpolationMode(imageControl, BitmapInterpolationMode.HighQuality);

                // Capture the weapon name
                string capturedWeapon = weapon;

                if (isOwned)
                {
                    // Click to equip/unequip (single click for responsiveness)
                    imageControl.Tapped += (_, _) =>
                    {
                        if (_gameState.CurrentResonatedWeapon1 == capturedWeapon || _gameState.CurrentResonatedWeapon2 == capturedWeapon)
                        {
                            ShowWeaponDisablePrompt(capturedWeapon);
                        }
                        else
                        {
                            ShowWeaponEquipPrompt(capturedWeapon);
                        }
                    };
                }
                
                // Cache the weapon image
                _weaponImageCache[weapon] = imageControl;

                _rightDrawerPanel.Children.Add(imageControl);
                
                // Register tooltip for this weapon
                _screen?.RegisterTooltipItem(imageControl, WeaponToDisplayName(weapon));
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
                e.Handled = true;
                EquipWeapon(weaponName);
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= WeaponEquipYesHandler;
                _equipNoButton.Click -= WeaponEquipNoHandler;
            }

            void WeaponEquipNoHandler(object? s, Avalonia.Interactivity.RoutedEventArgs e)
            {
                e.Handled = true;
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
                e.Handled = true;
                if (_gameState.CurrentResonatedWeapon1 == weaponName) UnequipWeapon(1);
                if (_gameState.CurrentResonatedWeapon2 == weaponName) UnequipWeapon(2);
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= WeaponDisableYesHandler;
                _equipNoButton.Click -= WeaponDisableNoHandler;
            }

            void WeaponDisableNoHandler(object? s, Avalonia.Interactivity.RoutedEventArgs e)
            {
                e.Handled = true;
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
                targetBorder.Child = new Image { Source = _emptyBitmap, Width = 320, Height = 180, Stretch = Stretch.Uniform };
                return;
            }

            // Set the display name
            targetText.Text = _weaponDisplayNames.TryGetValue(weaponName, out var disp) ? disp : weaponName;

            // Get the weapon bitmap and create the image
            if (_weaponBitmapCache.TryGetValue(weaponName, out var bmp))
            {
                targetBorder.Child = MusicClicker.Helpers.ImageHelpers.CreateSmoothImage(bmp, 320, 180, null, 1.0, true);
            }
            else
            {
                // Fallback to empty image if bitmap not found
                targetBorder.Child = new Image { Source = _emptyBitmap, Width = 320, Height = 180, Stretch = Stretch.Uniform };
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
                UpdateThemeColors();
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

        private void UpdateThemeColors()
        {
            // Update panel colors based on currently equipped score
            string equipped = _gameState.CurrentResonatedScore;
            
            // Define color themes for each score
            var theme = equipped switch
            {
                "Moonlight Sonata" => new { // Light blue and Purple
                    LeftBg = "#DD87CEEB", LeftBorder = "#9966CCFF", LeftHeaderBg = "#BB99DDFF",
                    LeftHeaderText = "#EEEEEEFF", RightBg = "#DD9966CC", RightBorder = "#66B3D9FF",
                    RightHeaderBg = "#BB8855BB", RightHeaderText = "#F0F0F0FF", DuetText = "#CC99FFFF"
                },
                "Eroica" => new { // Red-ish pink and Black
                    LeftBg = "#DDFF6B8A", LeftBorder = "#000000FF", LeftHeaderBg = "#BBFF8899",
                    LeftHeaderText = "#000000FF", RightBg = "#DD000000", RightBorder = "#FF5577FF",
                    RightHeaderBg = "#BB1A0A0A", RightHeaderText = "#FF99BBFF", DuetText = "#FF88AAFF"
                },
                "Swan Lake" => new { // White and Dark pink
                    LeftBg = "#DDFAFAFAF", LeftBorder = "#C71585FF", LeftHeaderBg = "#BBFFFFFFF",
                    LeftHeaderText = "#8B008BFF", RightBg = "#DDC71585", RightBorder = "#EEEEEEFF",
                    RightHeaderBg = "#BBB22A6D", RightHeaderText = "#FFFFFFFF", DuetText = "#FF88CCFF"
                },
                "La Campanella" => new { // White and Orange
                    LeftBg = "#DDFAFAFAF", LeftBorder = "#FFA500FF", LeftHeaderBg = "#BBFFFFFFF",
                    LeftHeaderText = "#FF8800FF", RightBg = "#DDFFA500", RightBorder = "#EEEEEEFF",
                    RightHeaderBg = "#BBFF9933", RightHeaderText = "#FFFFFFFF", DuetText = "#FFAA66FF"
                },
                "Enigma Variations" => new { // Black and Dark purple
                    LeftBg = "#DD000000", LeftBorder = "#663399FF", LeftHeaderBg = "#BB1A1A1A",
                    LeftHeaderText = "#9966AAFF", RightBg = "#DD663399", RightBorder = "#000000FF",
                    RightHeaderBg = "#BB552288", RightHeaderText = "#EEEEEEFF", DuetText = "#9977CCFF"
                },
                "Fate" => new { // Black and Scarlet-ish purple
                    LeftBg = "#DD000000", LeftBorder = "#A0206FFF", LeftHeaderBg = "#BB1A0A0A",
                    LeftHeaderText = "#C94B9AFF", RightBg = "#DDA0206F", RightBorder = "#000000FF",
                    RightHeaderBg = "#BB8B1A5E", RightHeaderText = "#EEEEEEFF", DuetText = "#D946A0FF"
                },
                "Ode to Joy" => new { // White and Light pink
                    LeftBg = "#DDFAFAFAF", LeftBorder = "#FFB6C1FF", LeftHeaderBg = "#BBFFFFFFF",
                    LeftHeaderText = "#FF99AAFF", RightBg = "#DDFFB6C1", RightBorder = "#EEEEEEFF",
                    RightHeaderBg = "#BBFFCCDD", RightHeaderText = "#333333FF", DuetText = "#FFBBDDFF"
                },
                "Dies Irae" => new { // Black and Orange
                    LeftBg = "#DD000000", LeftBorder = "#FFA500FF", LeftHeaderBg = "#BB1A1A1A",
                    LeftHeaderText = "#FFBB66FF", RightBg = "#DDFFA500", RightBorder = "#000000FF",
                    RightHeaderBg = "#BBFF9933", RightHeaderText = "#000000FF", DuetText = "#FFAA44FF"
                },
                "Winter" => new { // White and Light blue
                    LeftBg = "#DDFAFAFAF", LeftBorder = "#ADD8E6FF", LeftHeaderBg = "#BBFFFFFFF",
                    LeftHeaderText = "#4682B4FF", RightBg = "#DDADD8E6", RightBorder = "#EEEEEEFF",
                    RightHeaderBg = "#BB99CCEE", RightHeaderText = "#FFFFFFFF", DuetText = "#AAE6FFFF"
                },
                _ => new { // Default (no score equipped)
                    LeftBg = "#DD1A1A1A", LeftBorder = "#666666", LeftHeaderBg = "#BB2A2A2A",
                    LeftHeaderText = "#AAAAAA", RightBg = "#DD1A1A1A", RightBorder = "#666666",
                    RightHeaderBg = "#BB2A2A2A", RightHeaderText = "#AAAAAA", DuetText = "#FFFFFF"
                }
            };

            // Helper to safely parse color strings and fallback to opaque white on error
            Color SafeParse(string s)
            {
                try
                {
                    return Color.Parse(s);
                }
                catch
                {
                    return Color.Parse("#FFFFFFFF");
                }
            }

            // Apply theme colors (use SafeParse to avoid runtime exceptions from malformed strings)
            if (_leftPanelBorder != null)
                _leftPanelBorder.Background = new SolidColorBrush(SafeParse(theme.LeftBg));
            if (_leftPanelBorder != null)
                _leftPanelBorder.BorderBrush = new SolidColorBrush(SafeParse(theme.LeftBorder));
            if (_leftPanelHeader != null)
                _leftPanelHeader.Background = new SolidColorBrush(SafeParse(theme.LeftHeaderBg));
            if (_leftPanelHeaderText != null)
                _leftPanelHeaderText.Foreground = new SolidColorBrush(SafeParse(theme.LeftHeaderText));

            if (_rightPanelBorder != null)
                _rightPanelBorder.Background = new SolidColorBrush(SafeParse(theme.RightBg));
            if (_rightPanelBorder != null)
                _rightPanelBorder.BorderBrush = new SolidColorBrush(SafeParse(theme.RightBorder));
            if (_rightPanelHeader != null)
                _rightPanelHeader.Background = new SolidColorBrush(SafeParse(theme.RightHeaderBg));
            if (_rightPanelHeaderText != null)
                _rightPanelHeaderText.Foreground = new SolidColorBrush(SafeParse(theme.RightHeaderText));

            // Update duet text color (static, no flashing)
            if (_duetResonanceText != null && _duetResonanceText.IsVisible)
                _duetResonanceText.Foreground = new SolidColorBrush(SafeParse(theme.DuetText));
        }

        private void ThemePromptButtons(string scoreName)
        {
            // Theme the prompt buttons based on the score's aesthetic
            var (yesColor, noColor, yesBorder, noBorder) = scoreName switch
            {
                "Moonlight Sonata" => ("#6633DDFF", "#CC6633FF", "#9966FFFF", "#FF8855FF"), // Purple/Orange
                "Eroica" => ("#CC4488FF", "#884444FF", "#FF66AAFF", "#AA6666FF"), // Pink/Dark red
                "Swan" => ("#4488FFFF", "#8844FFFF", "#66AAFFFF", "#AA66FFFF"), // Blue/Purple
                "LaCampanella" => ("#FFAA22FF", "#AA55CCFF", "#FFCC66FF", "#CC77EEFF"), // Orange/Purple
                "Enigma" => ("#338855FF", "#663399FF", "#55AA77FF", "#8855BBFF"), // Dark green/Dark purple
                "Fate" => ("#CC2244FF", "#442222FF", "#FF4466FF", "#664444FF"), // Scarlet red/Dark red
                "OdeToJoy" => ("#FF66AAFF", "#CC4488FF", "#FF88CCFF", "#FF66AAFF"), // Pink/Pink
                "DiesIrae" => ("#FF8833FF", "#664422FF", "#FFAA66FF", "#886644FF"), // Orange/Dark orange
                "Winter" => ("#66AAFFFF", "#4488CCFF", "#88CCFFFF", "#66AAEEFF"), // Light blue/Blue
                _ => ("#22AA22FF", "#AA2222FF", "#44FF44FF", "#FF4444FF") // Default green/red
            };

            _equipYesButton.Background = new SolidColorBrush(Color.Parse(yesColor));
            _equipYesButton.BorderBrush = new SolidColorBrush(Color.Parse(yesBorder));
            _equipNoButton.Background = new SolidColorBrush(Color.Parse(noColor));
            _equipNoButton.BorderBrush = new SolidColorBrush(Color.Parse(noBorder));
        }

        public void RefreshDrawer()
        {
            // Clear caches to rebuild with new ownership state
            _scoreImageCache.Clear();
            _weaponImageCache.Clear();
            _drawersInitialized = false;
            
            // Rebuild the entire drawer to reflect current ownership state
            InitializeDrawer();
            InitializeWeaponDrawer();
        }

        // Checks whether the player owns the specified major score
        private bool OwnsScore(string score)
        {
            return score switch
            {
                // Base Major Scores
                "Moonlight Sonata" => _gameState.MoonlightMajorOwned > 0,
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

            // Theme the prompt buttons based on score
            ThemePromptButtons(scoreName);

            // Local handlers capture the score name
            void EquipYesButtonHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                e.Handled = true;
                EquipScore(scoreName);
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= EquipYesButtonHandler;
                _equipNoButton.Click -= EquipNoButtonHandler;
            }

            void EquipNoButtonHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                e.Handled = true;
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

            // Theme the prompt buttons based on score
            ThemePromptButtons(scoreName);

            // Local handlers
            void DisableYesHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                e.Handled = true;
                UnequipScore();
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= DisableYesHandler;
                _equipNoButton.Click -= DisableNoHandler;
            }

            void DisableNoHandler(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            {
                e.Handled = true;
                _equipPromptPanel.IsVisible = false;
                _equipYesButton.Click -= DisableYesHandler;
                _equipNoButton.Click -= DisableNoHandler;
            }

            _equipYesButton.Click += DisableYesHandler;
            _equipNoButton.Click += DisableNoHandler;
        }

        private void EquipScore(string scoreName, bool saveToState = true)
        {
            // Set new equipped score text + bitmap (use full display name)
            _equippedText.Text = GetScoreDisplayName(scoreName);
            // Use TryGetValue to avoid KeyNotFoundException if bitmap failed to load
            if (!_bitmapCache.TryGetValue(scoreName, out _equippedBitmap))
            {
                _equippedBitmap = _emptyBitmap;
            }

            // Create 16:9 image (480x270 fills the Viewbox container)
            var scoreImage = MusicClicker.Helpers.ImageHelpers.CreateSmoothImage(_equippedBitmap, 480, 270, null, 1.0, true);
            
            // Add click-to-unequip functionality (single click)
            scoreImage.Tapped += (s, e) =>
            {
                if (_gameState.CurrentResonatedScore != "None")
                {
                    ShowDisablePrompt(_gameState.CurrentResonatedScore);
                }
                e.Handled = true;
            };
            scoreImage.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);
            
            _equippedDisplay.Child = scoreImage;

            // Disable all major ability flags (include Winter)
            _gameState.MoonlightMajorAbility = false;
            _gameState.EroicaMajorAbility = false;
            _gameState.SwanMajorAbility = false;
            _gameState.LaCampanellaMajorAbility = false;
            _gameState.EnigmaMajorAbility = false;
            _gameState.FateMajorAbility = false;
            _gameState.OdeToJoyMajorAbility = false;
            _gameState.WinterAbility = false;

            // Enable the selected ability
            switch (scoreName)
            {
                case "Moonlight Sonata": _gameState.MoonlightMajorAbility = true; break;
                case "Eroica": _gameState.EroicaMajorAbility = true; break;
                case "Swan": _gameState.SwanMajorAbility = true; break;
                case "LaCampanella": _gameState.LaCampanellaMajorAbility = true; break;
                case "Enigma": _gameState.EnigmaMajorAbility = true; break;
                case "Fate": _gameState.FateMajorAbility = true; break;
                case "OdeToJoy": _gameState.OdeToJoyMajorAbility = true; break;
                case "Winter": _gameState.WinterAbility = true; break;
            }

            // Save to GameState
            if (saveToState)
            {
                _gameState.CurrentResonatedScore = scoreName;
            }
            
            // Update UI theme colors based on equipped score
            UpdateThemeColors();
        }

        private void UnequipScore()
        {
            // Reset equipped score to "None"
            _equippedText.Text = "None";
            _equippedBitmap = _emptyBitmap;

            _equippedDisplay.Child = new Image
            {
                Source = _emptyBitmap,
                Width = 480,
                Height = 270,
                Stretch = Stretch.Uniform
            };

            // Disable all major abilities (include Winter)
            _gameState.MoonlightMajorAbility = false;
            _gameState.EroicaMajorAbility = false;
            _gameState.SwanMajorAbility = false;
            _gameState.LaCampanellaMajorAbility = false;
            _gameState.EnigmaMajorAbility = false;
            _gameState.FateMajorAbility = false;
            _gameState.OdeToJoyMajorAbility = false;
            _gameState.WinterAbility = false;

            // Save to GameState
            _gameState.CurrentResonatedScore = "None";
            
            // Update UI theme to default colors
            UpdateThemeColors();
        }

        // Helper methods to convert internal names to display names for tooltips
        private string ScoreToDisplayName(string internalName)
        {
            // Map internal names to display names that match GameDescriptions keys
            return internalName switch
            {
                "Moonlight Sonata" => "Moonlight Sonata",
                "Eroica" => "Eroica",
                "Swan" => "Swan Lake",
                "LaCampanella" => "La Campanella",
                "Enigma" => "Enigma Variations",
                "Fate" => "Fate",
                "OdeToJoy" => "Ode to Joy",
                "DiesIrae" => "Dies Irae",
                "Winter" => "Winter",
                "Mercury" => "Mercury",
                "ClairDeLune" => "Clair de Lune",
                "Mars" => "Mars",
                _ => internalName // Fallback to original name
            };
        }

        private string WeaponToDisplayName(string internalName)
        {
            return internalName switch
            {
                "IncisorOfMoonlight" => "Incisor of Moonlight",
                "EulogyOfTheMoon" => "Eulogy of the Moon",
                "SakurasBlossom" => "Sakura's Blossom",
                "FuneralPrayer" => "Funeral Prayer",
                "StarScatteredWings" => "Star-Scattered Wings",
                "ThousandWingedSwan" => "Thousand Winged Swan",
                "SymphonyOfBells" => "Symphony of Bells",
                "RazerOfBellsChimes" => "Razer of Bell's Chimes",
                "CreatorOfMystery" => "Creator of Mystery",
                "Truthseeker" => "Truthseeker",
                "AstralChainripper" => "Astral Chainripper",
                "CosmicWeaver" => "Cosmic Weaver",
                "JoyfulCatharsis" => "Joyful Catharsis",
                "OdeToCreation" => "Ode to Creation",
                "SevenCircles" => "Seven Circles",
                "HellsWrath" => "Hell's Wrath",
                "CacophonicBlizzard" => "Cacophonic Blizzard",
                "TheSnowsDesire" => "The Snow's Desire",
                _ => internalName
            };
        }

        public void Dispose()
        {
            // Destructor-safe cleanup pattern
            if (_disposed) return;
            _disposed = true;
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
                // Only do checks every 30 ticks to significantly reduce overhead
                _tickCounter++;
                if (_tickCounter % 30 != 0) return;
                
                string currentScore = _gameState.CurrentResonatedScore ?? "None";
                string currentWeapon1 = _gameState.CurrentResonatedWeapon1 ?? "None";
                string currentWeapon2 = _gameState.CurrentResonatedWeapon2 ?? "None";
                
                // Only update if something changed
                if (currentScore != _lastEquippedScore)
                {
                    _lastEquippedScore = currentScore;
                    if (!string.IsNullOrEmpty(currentScore) && currentScore != "None" && (!_majorScoreImages.ContainsKey(currentScore) || !OwnsScore(currentScore)))
                    {
                        SetDefaultEquipped();
                    }
                    UpdateThemeColors();
                }
                
                // Only update weapon displays if they changed
                if (currentWeapon1 != _lastWeapon1)
                {
                    _lastWeapon1 = currentWeapon1;
                    SetWeaponSlotDisplay(1, currentWeapon1);
                    if (!string.IsNullOrEmpty(currentWeapon1) && currentWeapon1 != "None")
                    {
                        if (!_weaponImages.ContainsKey(currentWeapon1) || !OwnsWeapon(currentWeapon1))
                            UnequipWeapon(1);
                    }
                }
                
                if (currentWeapon2 != _lastWeapon2)
                {
                    _lastWeapon2 = currentWeapon2;
                    SetWeaponSlotDisplay(2, currentWeapon2);
                    if (!string.IsNullOrEmpty(currentWeapon2) && currentWeapon2 != "None")
                    {
                        if (!_weaponImages.ContainsKey(currentWeapon2) || !OwnsWeapon(currentWeapon2))
                            UnequipWeapon(2);
                    }
                }
            }
            catch { }
        }
        
        private string GetScoreDisplayName(string scoreKey)
        {
            return _majorScoreDisplayNames.TryGetValue(scoreKey, out var displayName) ? displayName : scoreKey;
        }
    }
}