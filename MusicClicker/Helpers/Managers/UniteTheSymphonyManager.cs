using System;
using Avalonia.Controls;

namespace MusicClicker
{
    public static class UniteTheSymphonyManager
    {
        // Crafting manager for combining fragments into full scores.
        // Each TryCraft* method verifies required fragment counts and consumes them
        // atomically. On success the method increments the owned count and applies
        // any NotesPerSecond bonuses or minor-score rewards from active abilities.
        // The methods call UIUpdater to refresh the related screens; this keeps UI
        // logic separate from crafting rules and keeps rendering consistent.
        //
        // Performance note: crafting methods update UI only when a craft succeeds,
        // which reduces unnecessary UI churn.
        // ==================== MINOR SCORES ====================

        public static bool TryCraftMoonlightMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.MoonlightMinorKeys >= 1 &&
                gameState.MoonlightMinorScales >= 1 &&
                gameState.MoonlightMinorProgressions >= 1 &&
                gameState.MelodiousOwned >= 10)
            {
                gameState.MoonlightMinorKeys--;
                gameState.MoonlightMinorScales--;
                gameState.MoonlightMinorProgressions--;
                gameState.MelodiousOwned -= 10;

                // Check Seven Circles refund (50% chance)
                if (gameState.SevenCircles && 
                    (gameState.CurrentResonatedWeapon1 == "SevenCircles" || gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    if (MusicClicker.Armory.WeaponAbilities.SevenCircles_CheckMinorCraftRefund())
                    {
                        gameState.MoonlightMinorKeys++;
                        gameState.MoonlightMinorScales++;
                        gameState.MoonlightMinorProgressions++;
                        gameState.MelodiousOwned += 10;
                    }
                }

                // Check Eulogy of the Moon refund (25% normally, 50% during nighttime)
                if (gameState.EulogyOfTheMoon && 
                    (gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon"))
                {
                    bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
                    double refundChance = isNighttime ? 0.50 : 0.25;
                    var random = new Random();
                    if (random.NextDouble() < refundChance)
                    {
                        gameState.MoonlightMinorKeys++;
                        gameState.MoonlightMinorScales++;
                        gameState.MoonlightMinorProgressions++;
                        gameState.MelodiousOwned += 10;
                    }
                }

                owned++;

                // Trigger Symphony of Bells ability (La Campanella I)
                if (gameState.SymphonyOfBellsAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SymphonyOfBells_OnMinorCraft(gameState, "moonlight sonata");
                }

                // Trigger Razer of Bell's Chimes ability (La Campanella II)
                if (gameState.RazerOfBellsChimes && 
                    (gameState.CurrentResonatedWeapon1 == "RazerOfBellsChimes" || gameState.CurrentResonatedWeapon2 == "RazerOfBellsChimes"))
                {
                    MusicClicker.Armory.WeaponAbilities.RazerOfBellsChimes_OnMinorCraft(gameState, "moonlight sonata");
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "moonlight sonata");
                }

                // Trigger Eroica Crescendance: Heroic Resolve
                MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_OnMinorCraft(gameState);

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMinor", "Moonlight Sonata");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMinor", "Moonlight Sonata");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftEroicaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.EroicaMinorKeys >= 1 &&
                gameState.EroicaMinorScales >= 1 &&
                gameState.EroicaMinorProgressions >= 1 &&
                gameState.MelodiousOwned >= 15)
            {
                gameState.EroicaMinorKeys--;
                gameState.EroicaMinorScales--;
                gameState.EroicaMinorProgressions--;
                gameState.MelodiousOwned -= 15;

                // Check Seven Circles refund (50% chance)
                if (gameState.SevenCircles && 
                    (gameState.CurrentResonatedWeapon1 == "SevenCircles" || gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    if (MusicClicker.Armory.WeaponAbilities.SevenCircles_CheckMinorCraftRefund())
                    {
                        gameState.EroicaMinorKeys++;
                        gameState.EroicaMinorScales++;
                        gameState.EroicaMinorProgressions++;
                        gameState.MelodiousOwned += 15;
                    }
                }

                // Check Eulogy of the Moon refund (25% normally, 50% during nighttime)
                if (gameState.EulogyOfTheMoon && 
                    (gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon"))
                {
                    bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
                    double refundChance = isNighttime ? 0.50 : 0.25;
                    var random = new Random();
                    if (random.NextDouble() < refundChance)
                    {
                        gameState.EroicaMinorKeys++;
                        gameState.EroicaMinorScales++;
                        gameState.EroicaMinorProgressions++;
                        gameState.MelodiousOwned += 15;
                    }
                }

                owned++;

                // Trigger Symphony of Bells ability (La Campanella I)
                if (gameState.SymphonyOfBellsAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SymphonyOfBells_OnMinorCraft(gameState, "eroica");
                }

                // Trigger Razer of Bell's Chimes ability (La Campanella II)
                if (gameState.RazerOfBellsChimes && 
                    (gameState.CurrentResonatedWeapon1 == "RazerOfBellsChimes" || gameState.CurrentResonatedWeapon2 == "RazerOfBellsChimes"))
                {
                    MusicClicker.Armory.WeaponAbilities.RazerOfBellsChimes_OnMinorCraft(gameState, "eroica");
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "eroica");
                }

                // Trigger Eroica Crescendance: Heroic Resolve
                MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_OnMinorCraft(gameState);

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMinor", "Eroica");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMinor", "Eroica");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftSwanMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.SwanLakeMinorKeys >= 1 &&
                gameState.SwanLakeMinorScales >= 1 &&
                gameState.SwanLakeMinorProgressions >= 1 &&
                gameState.MelodiousOwned >= 20)
            {
                gameState.SwanLakeMinorKeys--;
                gameState.SwanLakeMinorScales--;
                gameState.SwanLakeMinorProgressions--;
                gameState.MelodiousOwned -= 20;

                // Check Seven Circles refund (50% chance)
                if (gameState.SevenCircles && 
                    (gameState.CurrentResonatedWeapon1 == "SevenCircles" || gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    if (MusicClicker.Armory.WeaponAbilities.SevenCircles_CheckMinorCraftRefund())
                    {
                        gameState.SwanLakeMinorKeys++;
                        gameState.SwanLakeMinorScales++;
                        gameState.SwanLakeMinorProgressions++;
                        gameState.MelodiousOwned += 20;
                    }
                }

                // Check Eulogy of the Moon refund (25% normally, 50% during nighttime)
                if (gameState.EulogyOfTheMoon && 
                    (gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon"))
                {
                    bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
                    double refundChance = isNighttime ? 0.50 : 0.25;
                    var random = new Random();
                    if (random.NextDouble() < refundChance)
                    {
                        gameState.SwanLakeMinorKeys++;
                        gameState.SwanLakeMinorScales++;
                        gameState.SwanLakeMinorProgressions++;
                        gameState.MelodiousOwned += 20;
                    }
                }

                owned++;

                // Trigger Symphony of Bells ability (La Campanella I)
                if (gameState.SymphonyOfBellsAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SymphonyOfBells_OnMinorCraft(gameState, "swanlake");
                }

                // Trigger Razer of Bell's Chimes ability (La Campanella II)
                if (gameState.RazerOfBellsChimes && 
                    (gameState.CurrentResonatedWeapon1 == "RazerOfBellsChimes" || gameState.CurrentResonatedWeapon2 == "RazerOfBellsChimes"))
                {
                    MusicClicker.Armory.WeaponAbilities.RazerOfBellsChimes_OnMinorCraft(gameState, "swanlake");
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "swanlake");
                }

                // Trigger Eroica Crescendance: Heroic Resolve
                MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_OnMinorCraft(gameState);

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMinor", "Swan Lake");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMinor", "Swan Lake");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftLaCampanellaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.LaCampanellaMinorKeys >= 1 &&
                gameState.LaCampanellaMinorScales >= 1 &&
                gameState.LaCampanellaMinorProgressions >= 1 &&
                gameState.MelodiousOwned >= 25)
            {
                gameState.LaCampanellaMinorKeys--;
                gameState.LaCampanellaMinorScales--;
                gameState.LaCampanellaMinorProgressions--;
                gameState.MelodiousOwned -= 25;

                // Check Seven Circles refund (50% chance)
                if (gameState.SevenCircles && 
                    (gameState.CurrentResonatedWeapon1 == "SevenCircles" || gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    if (MusicClicker.Armory.WeaponAbilities.SevenCircles_CheckMinorCraftRefund())
                    {
                        gameState.LaCampanellaMinorKeys++;
                        gameState.LaCampanellaMinorScales++;
                        gameState.LaCampanellaMinorProgressions++;
                        gameState.MelodiousOwned += 25;
                    }
                }

                // Check Eulogy of the Moon refund (25% normally, 50% during nighttime)
                if (gameState.EulogyOfTheMoon && 
                    (gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon"))
                {
                    bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
                    double refundChance = isNighttime ? 0.50 : 0.25;
                    var random = new Random();
                    if (random.NextDouble() < refundChance)
                    {
                        gameState.LaCampanellaMinorKeys++;
                        gameState.LaCampanellaMinorScales++;
                        gameState.LaCampanellaMinorProgressions++;
                        gameState.MelodiousOwned += 25;
                    }
                }

                owned++;

                // Trigger Symphony of Bells ability (La Campanella I)
                if (gameState.SymphonyOfBellsAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SymphonyOfBells_OnMinorCraft(gameState, "lacampanella");
                }

                // Trigger Razer of Bell's Chimes ability (La Campanella II)
                if (gameState.RazerOfBellsChimes && 
                    (gameState.CurrentResonatedWeapon1 == "RazerOfBellsChimes" || gameState.CurrentResonatedWeapon2 == "RazerOfBellsChimes"))
                {
                    MusicClicker.Armory.WeaponAbilities.RazerOfBellsChimes_OnMinorCraft(gameState, "lacampanella");
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "lacampanella");
                }

                // Trigger Eroica Crescendance: Heroic Resolve
                MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_OnMinorCraft(gameState);

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMinor", "La Campanella");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMinor", "La Campanella");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftEnigmaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.EnigmaMinorKeys >= 1 &&
                gameState.EnigmaMinorScales >= 1 &&
                gameState.EnigmaMinorProgressions >= 1 &&
                gameState.MelodiousOwned >= 30)
            {
                gameState.EnigmaMinorKeys--;
                gameState.EnigmaMinorScales--;
                gameState.EnigmaMinorProgressions--;
                gameState.MelodiousOwned -= 30;

                // Check Seven Circles refund (50% chance)
                if (gameState.SevenCircles && 
                    (gameState.CurrentResonatedWeapon1 == "SevenCircles" || gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    if (MusicClicker.Armory.WeaponAbilities.SevenCircles_CheckMinorCraftRefund())
                    {
                        gameState.EnigmaMinorKeys++;
                        gameState.EnigmaMinorScales++;
                        gameState.EnigmaMinorProgressions++;
                        gameState.MelodiousOwned += 30;
                    }
                }

                // Check Eulogy of the Moon refund (25% normally, 50% during nighttime)
                if (gameState.EulogyOfTheMoon && 
                    (gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon"))
                {
                    bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
                    double refundChance = isNighttime ? 0.50 : 0.25;
                    var random = new Random();
                    if (random.NextDouble() < refundChance)
                    {
                        gameState.EnigmaMinorKeys++;
                        gameState.EnigmaMinorScales++;
                        gameState.EnigmaMinorProgressions++;
                        gameState.MelodiousOwned += 30;
                    }
                }

                owned++;

                // Trigger Symphony of Bells ability (La Campanella I)
                if (gameState.SymphonyOfBellsAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SymphonyOfBells_OnMinorCraft(gameState, "enigma");
                }

                // Trigger Razer of Bell's Chimes ability (La Campanella II)
                if (gameState.RazerOfBellsChimes && 
                    (gameState.CurrentResonatedWeapon1 == "RazerOfBellsChimes" || gameState.CurrentResonatedWeapon2 == "RazerOfBellsChimes"))
                {
                    MusicClicker.Armory.WeaponAbilities.RazerOfBellsChimes_OnMinorCraft(gameState, "enigma");
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "enigma");
                }

                // Trigger Eroica Crescendance: Heroic Resolve
                MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_OnMinorCraft(gameState);

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMinor", "Enigma Variations");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMinor", "Enigma Variations");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftFateMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.FateMinorKeys >= 1 &&
                gameState.FateMinorScales >= 1 &&
                gameState.FateMinorProgressions >= 1 &&
                gameState.MelodiousOwned >= 35)
            {
                gameState.FateMinorKeys--;
                gameState.FateMinorScales--;
                gameState.FateMinorProgressions--;
                gameState.MelodiousOwned -= 35;

                // Check Seven Circles refund (50% chance)
                if (gameState.SevenCircles && 
                    (gameState.CurrentResonatedWeapon1 == "SevenCircles" || gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    if (MusicClicker.Armory.WeaponAbilities.SevenCircles_CheckMinorCraftRefund())
                    {
                        gameState.FateMinorKeys++;
                        gameState.FateMinorScales++;
                        gameState.FateMinorProgressions++;
                        gameState.MelodiousOwned += 35;
                    }
                }

                // Check Eulogy of the Moon refund (25% normally, 50% during nighttime)
                if (gameState.EulogyOfTheMoon && 
                    (gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon"))
                {
                    bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
                    double refundChance = isNighttime ? 0.50 : 0.25;
                    var random = new Random();
                    if (random.NextDouble() < refundChance)
                    {
                        gameState.FateMinorKeys++;
                        gameState.FateMinorScales++;
                        gameState.FateMinorProgressions++;
                        gameState.HarmoniousOwned += 10;
                    }
                }

                owned++;

                // Trigger Symphony of Bells ability (La Campanella I)
                if (gameState.SymphonyOfBellsAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SymphonyOfBells_OnMinorCraft(gameState, "fate");
                }

                // Trigger Razer of Bell's Chimes ability (La Campanella II)
                if (gameState.RazerOfBellsChimes && 
                    (gameState.CurrentResonatedWeapon1 == "RazerOfBellsChimes" || gameState.CurrentResonatedWeapon2 == "RazerOfBellsChimes"))
                {
                    MusicClicker.Armory.WeaponAbilities.RazerOfBellsChimes_OnMinorCraft(gameState, "fate");
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "fate");
                }

                // Trigger Astral Chainripper ability (Fate I)
                if (gameState.AstralChainripperAbility && 
                    (gameState.CurrentResonatedWeapon1 == "AstralChainripper" || gameState.CurrentResonatedWeapon2 == "AstralChainripper"))
                {
                    MusicClicker.Armory.WeaponAbilities.AstralChainripper_OnFateMinorCraft(gameState);
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "fate");
                }

                // Trigger Eroica Crescendance: Heroic Resolve
                MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_OnMinorCraft(gameState);

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftOdeToJoyMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.OdeToJoyMinorKeys >= 1 &&
                gameState.OdeToJoyMinorScales >= 1 &&
                gameState.OdeToJoyMinorProgressions >= 1 &&
                gameState.MelodiousOwned >= 40)
            {
                gameState.OdeToJoyMinorKeys--;
                gameState.OdeToJoyMinorScales--;
                gameState.OdeToJoyMinorProgressions--;
                gameState.MelodiousOwned -= 40;

                // Check Seven Circles refund (50% chance)
                if (gameState.SevenCircles && 
                    (gameState.CurrentResonatedWeapon1 == "SevenCircles" || gameState.CurrentResonatedWeapon2 == "SevenCircles"))
                {
                    if (MusicClicker.Armory.WeaponAbilities.SevenCircles_CheckMinorCraftRefund())
                    {
                        gameState.OdeToJoyMinorKeys++;
                        gameState.OdeToJoyMinorScales++;
                        gameState.OdeToJoyMinorProgressions++;
                        gameState.MelodiousOwned += 40;
                    }
                }

                // Check Eulogy of the Moon refund (25% normally, 50% during nighttime)
                if (gameState.EulogyOfTheMoon && 
                    (gameState.CurrentResonatedWeapon1 == "EulogyOfTheMoon" || gameState.CurrentResonatedWeapon2 == "EulogyOfTheMoon"))
                {
                    bool isNighttime = MusicClicker.Armory.WeaponAbilities.IsNighttime();
                    double refundChance = isNighttime ? 0.50 : 0.25;
                    var random = new Random();
                    if (random.NextDouble() < refundChance)
                    {
                        gameState.OdeToJoyMinorKeys++;
                        gameState.OdeToJoyMinorScales++;
                        gameState.OdeToJoyMinorProgressions++;
                        gameState.HarmoniousOwned += 15;
                    }
                }

                owned++;

                // Trigger Symphony of Bells ability (La Campanella I)
                if (gameState.SymphonyOfBellsAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SymphonyOfBells_OnMinorCraft(gameState, "odetojoy");
                }

                // Trigger Razer of Bell's Chimes ability (La Campanella II)
                if (gameState.RazerOfBellsChimes && 
                    (gameState.CurrentResonatedWeapon1 == "RazerOfBellsChimes" || gameState.CurrentResonatedWeapon2 == "RazerOfBellsChimes"))
                {
                    MusicClicker.Armory.WeaponAbilities.RazerOfBellsChimes_OnMinorCraft(gameState, "odetojoy");
                }

                // Trigger Cosmic Weaver ability (Fate II)
                if (gameState.CosmicWeaverAbility && 
                    (gameState.CurrentResonatedWeapon1 == "CosmicWeaver" || gameState.CurrentResonatedWeapon2 == "CosmicWeaver"))
                {
                    MusicClicker.Armory.WeaponAbilities.CosmicWeaver_OnMinorCraft(gameState, "odetojoy");
                }

                // Trigger Eroica Crescendance: Heroic Resolve
                MusicClicker.Armory.WeaponAbilities.EroicaCrescendance_OnMinorCraft(gameState);

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMinor", "Ode to Joy");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMinor", "Ode to Joy");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        // ==================== MAJOR SCORES ====================

        public static bool TryCraftMoonlightSonataMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.MoonlightMajorKeys >= 1 &&
                gameState.MoonlightMajorScales >= 1 &&
                gameState.MoonlightMajorProgressions >= 1 &&
                gameState.HarmoniousOwned >= 10)
            {
                gameState.MoonlightMajorKeys--;
                gameState.MoonlightMajorScales--;
                gameState.MoonlightMajorProgressions--;
                gameState.HarmoniousOwned -= 10;

                owned++;

                // Trigger Sakura's Blossom ability (Eroica I) - on major acquisition
                if (gameState.SakurasBlossomAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnMajorSheetAcquired(gameState, "Moonlight Sonata");
                }

                // Trigger Funeral Prayer ability (Eroica II) - on major acquisition
                if (gameState.FuneralPrayerAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnMajorAcquisition(gameState);
                }

                // Trigger Triumph of Heroes passive (Eroica Major Crescendance)
                if (gameState.EroicaMajorAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.TriumphOfHeroes_OnMajorAcquisition(gameState);
                }

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMajor", "Moonlight Sonata");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMajor", "Moonlight Sonata");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftEroicaMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.EroicaMajorKeys >= 1 &&
                gameState.EroicaMajorScales >= 1 &&
                gameState.EroicaMajorProgressions >= 1 &&
                gameState.HarmoniousOwned >= 15)
            {
                gameState.EroicaMajorKeys--;
                gameState.EroicaMajorScales--;
                gameState.EroicaMajorProgressions--;
                gameState.HarmoniousOwned -= 15;

                owned++;

                // Trigger Sakura's Blossom ability (Eroica I) - on major acquisition
                if (gameState.SakurasBlossomAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnMajorSheetAcquired(gameState, "Eroica");
                }

                // Trigger Funeral Prayer ability (Eroica II) - on major acquisition
                if (gameState.FuneralPrayerAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnMajorAcquisition(gameState);
                }

                // Trigger Triumph of Heroes passive (Eroica Major Crescendance)
                if (gameState.EroicaMajorAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.TriumphOfHeroes_OnMajorAcquisition(gameState);
                }

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMajor", "Eroica");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMajor", "Eroica");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftSwanMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.SwanLakeMajorKeys >= 1 &&
                gameState.SwanLakeMajorScales >= 1 &&
                gameState.SwanLakeMajorProgressions >= 1 &&
                gameState.HarmoniousOwned >= 20)
            {
                gameState.SwanLakeMajorKeys--;
                gameState.SwanLakeMajorScales--;
                gameState.SwanLakeMajorProgressions--;
                gameState.HarmoniousOwned -= 20;

                owned++;

                // Trigger Sakura's Blossom ability (Eroica I) - on major acquisition
                if (gameState.SakurasBlossomAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnMajorSheetAcquired(gameState, "Swan Lake");
                }

                // Trigger Funeral Prayer ability (Eroica II) - on major acquisition
                if (gameState.FuneralPrayerAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnMajorAcquisition(gameState);
                }

                // Trigger Triumph of Heroes passive (Eroica Major Crescendance)
                if (gameState.EroicaMajorAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.TriumphOfHeroes_OnMajorAcquisition(gameState);
                }

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMajor", "Swan Lake");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMajor", "Swan Lake");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftLaCampanellaMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.LaCampanellaMajorKeys >= 1 &&
                gameState.LaCampanellaMajorScales >= 1 &&
                gameState.LaCampanellaMajorProgressions >= 1 &&
                gameState.HarmoniousOwned >= 25)
            {
                gameState.LaCampanellaMajorKeys--;
                gameState.LaCampanellaMajorScales--;
                gameState.LaCampanellaMajorProgressions--;
                gameState.HarmoniousOwned -= 25;

                owned++;

                // Trigger Sakura's Blossom ability (Eroica I) - on major acquisition
                if (gameState.SakurasBlossomAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnMajorSheetAcquired(gameState, "La Campanella");
                }

                // Trigger Funeral Prayer ability (Eroica II) - on major acquisition
                if (gameState.FuneralPrayerAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnMajorAcquisition(gameState);
                }

                // Trigger Triumph of Heroes passive (Eroica Major Crescendance)
                if (gameState.EroicaMajorAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.TriumphOfHeroes_OnMajorAcquisition(gameState);
                }

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMajor", "La Campanella");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMajor", "La Campanella");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftEnigmaMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.EnigmaMajorKeys >= 1 &&
                gameState.EnigmaMajorScales >= 1 &&
                gameState.EnigmaMajorProgressions >= 1 &&
                gameState.HarmoniousOwned >= 30)
            {
                gameState.EnigmaMajorKeys--;
                gameState.EnigmaMajorScales--;
                gameState.EnigmaMajorProgressions--;
                gameState.HarmoniousOwned -= 30;

                owned++;

                // Trigger Sakura's Blossom ability (Eroica I) - on major acquisition
                if (gameState.SakurasBlossomAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnMajorSheetAcquired(gameState, "Enigma Variations");
                }

                // Trigger Funeral Prayer ability (Eroica II) - on major acquisition
                if (gameState.FuneralPrayerAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnMajorAcquisition(gameState);
                }

                // Trigger Triumph of Heroes passive (Eroica Major Crescendance)
                if (gameState.EroicaMajorAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.TriumphOfHeroes_OnMajorAcquisition(gameState);
                }

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMajor", "Enigma Variations");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMajor", "Enigma Variations");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftFateMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.FateMajorKeys >= 1 &&
                gameState.FateMajorScales >= 1 &&
                gameState.FateMajorProgressions >= 1 &&
                gameState.HarmoniousOwned >= 35)
            {
                gameState.FateMajorKeys--;
                gameState.FateMajorScales--;
                gameState.FateMajorProgressions--;
                gameState.HarmoniousOwned -= 35;

                owned++;

                // Trigger Sakura's Blossom ability (Eroica I) - on major acquisition
                if (gameState.SakurasBlossomAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnMajorSheetAcquired(gameState, "Fate");
                }

                // Trigger Funeral Prayer ability (Eroica II) - on major acquisition
                if (gameState.FuneralPrayerAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnMajorAcquisition(gameState);
                }

                // Trigger Triumph of Heroes passive (Eroica Major Crescendance)
                if (gameState.EroicaMajorAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.TriumphOfHeroes_OnMajorAcquisition(gameState);
                }

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMajor", "Fate");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMajor", "Fate");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftOdeToJoyMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            if (gameState.OdeToJoyMajorKeys >= 1 &&
                gameState.OdeToJoyMajorScales >= 1 &&
                gameState.OdeToJoyMajorProgressions >= 1 &&
                gameState.HarmoniousOwned >= 40)
            {
                gameState.OdeToJoyMajorKeys--;
                gameState.OdeToJoyMajorScales--;
                gameState.OdeToJoyMajorProgressions--;
                gameState.HarmoniousOwned -= 40;

                owned++;

                // Trigger Sakura's Blossom ability (Eroica I) - on major acquisition
                if (gameState.SakurasBlossomAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnMajorSheetAcquired(gameState, "Ode to Joy");
                }

                // Trigger Funeral Prayer ability (Eroica II) - on major acquisition
                if (gameState.FuneralPrayerAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnMajorAcquisition(gameState);
                }

                // Trigger Triumph of Heroes passive (Eroica Major Crescendance)
                if (gameState.EroicaMajorAbility)
                {
                    MusicClicker.Armory.WeaponAbilities.TriumphOfHeroes_OnMajorAcquisition(gameState);
                }

                canCraft = true;
            }

            if (canCraft)
            {
                // Queue for Mirror Lake reflection
                if (gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.QueueMirrorAction(gameState, "CraftMajor", "Ode to Joy");
                }

                // Bank for Fate Duet Hourglass
                if (gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
                {
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "CraftMajor", "Ode to Joy");
                }

                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                UIUpdater.UpdateUI(window, gameState);
            }

            return canCraft;
        }
    }
}
