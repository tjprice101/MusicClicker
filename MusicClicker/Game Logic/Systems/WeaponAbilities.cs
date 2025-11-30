using System;
using System.Collections.Generic;
using System.Linq;
using MusicClicker;

namespace MusicClicker.Armory
{
    /// <summary>
    /// Centralized container for Armory of Forte weapon ability functions.
    /// These functions are intended to be called by the main program (or managers)
    /// with the appropriate parameters; they are intentionally standalone and
    /// not wired into the project yet.
    ///
    /// Each method is documented with the parameters it expects and the
    /// observable side-effects it performs on the provided GameState.
    /// </summary>
    public static class WeaponAbilities
    {
        // Static Random instance to avoid creating new instances on every call (performance optimization)
        private static readonly Random _random = new Random();
        
        // Keeping only the Incisor upgrade for focused development.
        // All other weapon-specific helpers were removed to simplify early development.

        /// <summary>
        /// Helper method to check if current time is nighttime (8PM - 6AM local time)
        /// </summary>
        public static bool IsNighttime()
        {
            int currentHour = DateTime.Now.Hour;
            // Nighttime is 20:00 (8PM) to 05:59 (6AM)
            return currentHour >= 20 || currentHour < 6;
        }

        /// <summary>
        /// Incisor of Moonlight: effect described on the Armory page.
        /// On upgrade purchase, increase the player's current notes by 5%.
        /// This method applies the effect directly to `gameState.Notes`.
        /// </summary>
        public static void IncisorOfMoonlight_OnUpgradePurchase(GameState gameState)
        {
            if (gameState == null) return;
            double bonus = Math.Round(gameState.Notes * 0.05, 2);
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
        }

        /// <summary>
        /// Eulogy of the Moon: On upgrade purchase, increase the player's lowest-owned
        /// upgrade by 2 (simulate 2 purchases of that upgrade). This will increment
        /// the owned counter and apply the corresponding NotesPerSecond or
        /// NotesPerClick effects for each simulated purchase.
        /// </summary>
        public static void EulogyOfTheMoon_OnUpgradePurchase(GameState gameState)
        {
            if (gameState == null) return;

            // Gather current owned counts for the main upgrades
            int[] owned = new int[] {
                gameState.ChordOwned,
                gameState.ScaleOwned,
                gameState.OrchestraOwned,
                gameState.SymphonyOwned,
                gameState.AriaOwned,
                gameState.RequiemOwned,
                gameState.OpusOwned,
                gameState.MagnumOpusOwned
            };

            // Find index of the lowest owned upgrade (deterministic: first min encountered)
            int minIndex = 0;
            int minVal = owned[0];
            for (int i = 1; i < owned.Length; i++)
            {
                if (owned[i] < minVal)
                {
                    minVal = owned[i];
                    minIndex = i;
                }
            }

            // Simulate 2 purchases of the selected upgrade, applying effects each time
            for (int i = 0; i < 2; i++)
            {
                switch (minIndex)
                {
                    // NPS upgrades
                    case 0: // Chord
                        {
                            double effect = gameState.ChordBaseNpsEffect * Math.Pow(gameState.ChordNpsGrowth, gameState.ChordOwned);
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += effect;
                            }
                            gameState.ChordOwned++;
                        }
                        break;
                    case 1: // Scale
                        {
                            double effect = gameState.ScaleBaseNpsEffect * Math.Pow(gameState.ScaleNpsGrowth, gameState.ScaleOwned);
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += effect;
                            }
                            gameState.ScaleOwned++;
                        }
                        break;
                    case 2: // Orchestra
                        {
                            double effect = gameState.OrchestraBaseNpsEffect * Math.Pow(gameState.OrchestraNpsGrowth, gameState.OrchestraOwned);
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += effect;
                            }
                            gameState.OrchestraOwned++;
                        }
                        break;
                    case 3: // Symphony
                        {
                            double effect = gameState.SymphonyBaseNpsEffect * Math.Pow(gameState.SymphonyNpsGrowth, gameState.SymphonyOwned);
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += effect;
                            }
                            gameState.SymphonyOwned++;
                        }
                        break;

                    // Click upgrades
                    case 4: // Aria
                        {
                            double effect = gameState.AriaBaseClickEffect * Math.Pow(gameState.AriaClickGrowth, gameState.AriaOwned);
                            gameState.NotesPerClick += effect;
                            gameState.AriaOwned++;
                        }
                        break;
                    case 5: // Requiem
                        {
                            double effect = gameState.RequiemBaseClickEffect * Math.Pow(gameState.RequiemClickGrowth, gameState.RequiemOwned);
                            gameState.NotesPerClick += effect;
                            gameState.RequiemOwned++;
                        }
                        break;
                    case 6: // Opus
                        {
                            double effect = gameState.OpusBaseClickEffect * Math.Pow(gameState.OpusClickGrowth, gameState.OpusOwned);
                            gameState.NotesPerClick += effect;
                            gameState.OpusOwned++;
                        }
                        break;
                    case 7: // Magnum Opus
                        {
                            double effect = gameState.MagnumOpusBaseClickEffect * Math.Pow(gameState.MagnumOpusClickGrowth, gameState.MagnumOpusOwned);
                            gameState.NotesPerClick += effect;
                            gameState.MagnumOpusOwned++;
                        }
                        break;
                }
            }
        }

        // ==================== MOONLIGHT DUET ====================

        /// <summary>
        /// Moonlight Duet: "Lunar Phases"
        /// <summary>
        /// Duet Ability (Toggle, 4min cooldown, 20s duration):
        /// Cycles through 4 moon phases every 5 seconds:
        /// - New Moon (0): 2x NPC
        /// - Crescent (1): Components drop on click (10% chance per click)
        /// - Full Moon (2): 3x NPS
        /// - Waning (3): Upgrades cost 50% less
        /// 
        /// SPECIAL: If resonating with Moonlight Major score, all phases are active simultaneously!
        /// 
        /// Returns the current phase (0-3) or -1 if not active.
        /// For checking if all phases active, use MoonlightDuet_AreAllPhasesActive()
        /// </summary>
        public static int MoonlightDuet_GetCurrentPhase(GameState gameState)
        {
            if (gameState == null) return -1;
            
            if (!gameState.MoonlightDuetActive || DateTime.Now > gameState.MoonlightDuetExpiry)
                return -1;
            
            // Check if we need to advance to the next phase (every 5 seconds)
            double elapsedSincePhaseStart = (DateTime.Now - gameState.MoonlightPhaseChangeTime).TotalSeconds;
            if (elapsedSincePhaseStart >= 5.0)
            {
                // Advance to next phase
                gameState.MoonlightCurrentPhase = (gameState.MoonlightCurrentPhase + 1) % 4;
                gameState.MoonlightPhaseChangeTime = DateTime.Now;
            }
            
            return gameState.MoonlightCurrentPhase;
        }

        /// <summary>
        /// Check if all Moonlight Duet phases are active (when resonating with Moonlight Major)
        /// Note: Only call this if GetCurrentPhase returns a valid phase (not -1) to avoid redundant checks
        /// </summary>
        public static bool MoonlightDuet_AreAllPhasesActive(GameState gameState)
        {
            if (gameState == null) return false;
            
            // Quick check: If not even active, return false immediately
            if (!gameState.MoonlightDuetActive)
                return false;
            
            // Check if resonating with Moonlight Sonata Major score
            return gameState.CurrentResonatedScore == "Moonlight Sonata";
        }

        /// <summary>
        /// Get the current moon phase without advancing it (read-only, fast check for clicks)
        /// Returns -1 if duet is not active
        /// </summary>
        public static int MoonlightDuet_GetCurrentPhaseReadOnly(GameState gameState)
        {
            if (gameState == null) return -1;
            if (!gameState.MoonlightDuetActive) return -1;
            return gameState.MoonlightCurrentPhase;
        }

        /// <summary>
        /// Moonlight Duet: Handle component drop during Crescent phase (10% chance per click)
        /// </summary>
        public static void MoonlightDuet_CrescentComponentDrop(GameState gameState)
        {
            if (gameState == null) return;
            
            // Early return if random check fails (90% of the time) - performance optimization
            if (_random.NextDouble() >= 0.10) return;

            // Pick a random score (0-6) and component type (0=Keys, 1=Scales, 2=Progressions)
            int scoreIndex = _random.Next(7);
            int componentType = _random.Next(3);

            switch (scoreIndex)
            {
                case 0: // Moonlight
                    if (componentType == 0) gameState.MoonlightMinorKeys++;
                    else if (componentType == 1) gameState.MoonlightMinorScales++;
                    else gameState.MoonlightMinorProgressions++;
                    break;
                case 1: // Eroica
                    if (componentType == 0) gameState.EroicaMinorKeys++;
                    else if (componentType == 1) gameState.EroicaMinorScales++;
                    else gameState.EroicaMinorProgressions++;
                    break;
                case 2: // Swan Lake
                    if (componentType == 0) gameState.SwanLakeMinorKeys++;
                    else if (componentType == 1) gameState.SwanLakeMinorScales++;
                    else gameState.SwanLakeMinorProgressions++;
                    break;
                case 3: // La Campanella
                    if (componentType == 0) gameState.LaCampanellaMinorKeys++;
                    else if (componentType == 1) gameState.LaCampanellaMinorScales++;
                    else gameState.LaCampanellaMinorProgressions++;
                    break;
                case 4: // Enigma
                    if (componentType == 0) gameState.EnigmaMinorKeys++;
                    else if (componentType == 1) gameState.EnigmaMinorScales++;
                    else gameState.EnigmaMinorProgressions++;
                    break;
                case 5: // Fate
                    if (componentType == 0) gameState.FateMinorKeys++;
                    else if (componentType == 1) gameState.FateMinorScales++;
                    else gameState.FateMinorProgressions++;
                    break;
                case 6: // Ode to Joy
                    if (componentType == 0) gameState.OdeToJoyMinorKeys++;
                    else if (componentType == 1) gameState.OdeToJoyMinorScales++;
                    else gameState.OdeToJoyMinorProgressions++;
                    break;
            }
        }

        // ==================== EROICA WEAPONS (2-3) ====================

        /// <summary>
        /// Sakura's Blossom (Eroica I): On acquisition of a score's Major sheet, 
        /// gives 1 of each Minor Component (Key, Scale, Progression).
        /// Call this when a major sheet is acquired.
        /// </summary>
        public static void SakurasBlossom_OnMajorSheetAcquired(GameState gameState, string scoreName)
        {
            if (gameState == null) return;

            // Add 1 to each minor component type for the given score
            switch (scoreName.ToLower())
            {
                case "moonlight sonata":
                    gameState.MoonlightMinorKeys++;
                    gameState.MoonlightMinorScales++;
                    gameState.MoonlightMinorProgressions++;
                    break;
                case "eroica":
                    gameState.EroicaMinorKeys++;
                    gameState.EroicaMinorScales++;
                    gameState.EroicaMinorProgressions++;
                    break;
                case "swanlake":
                    gameState.SwanLakeMinorKeys++;
                    gameState.SwanLakeMinorScales++;
                    gameState.SwanLakeMinorProgressions++;
                    break;
                case "lacampanella":
                    gameState.LaCampanellaMinorKeys++;
                    gameState.LaCampanellaMinorScales++;
                    gameState.LaCampanellaMinorProgressions++;
                    break;
                case "enigma":
                    gameState.EnigmaMinorKeys++;
                    gameState.EnigmaMinorScales++;
                    gameState.EnigmaMinorProgressions++;
                    break;
                case "fate":
                    gameState.FateMinorKeys++;
                    gameState.FateMinorScales++;
                    gameState.FateMinorProgressions++;
                    break;
                case "odetojoy":
                    gameState.OdeToJoyMinorKeys++;
                    gameState.OdeToJoyMinorScales++;
                    gameState.OdeToJoyMinorProgressions++;
                    break;
            }
        }

        /// <summary>
        /// Funeral Prayer (Eroica II): On acquisition of a score's Major version,
        /// divide current notes by 2 and give 3 minor scores of lowest owned minor score.
        /// </summary>
        public static void FuneralPrayer_OnMajorAcquisition(GameState gameState)
        {
            if (gameState == null) return;

            // (Removed old behavior that halved the player's notes on major acquisition.)

            // Find lowest owned minor score
            int[] minorScores = new int[] {
                gameState.MoonlightMinorOwned,
                gameState.EroicaMinorOwned,
                gameState.SwanMinorOwned,
                gameState.LaCampanellaMinorOwned,
                gameState.EnigmaMinorOwned,
                gameState.FateMinorOwned,
                gameState.OdeToJoyMinorOwned
            };

            int minIndex = 0;
            int minVal = minorScores[0];
            for (int i = 1; i < minorScores.Length; i++)
            {
                if (minorScores[i] < minVal)
                {
                    minVal = minorScores[i];
                    minIndex = i;
                }
            }

            // Give 3 of that minor score
            switch (minIndex)
            {
                case 0:
                    gameState.MoonlightMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 9000; // 3000 * 3
                    }
                    break;
                case 1:
                    gameState.EroicaMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 24000; // 8000 * 3
                    }
                    break;
                case 2:
                    gameState.SwanMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 45000; // 15000 * 3
                    }
                    break;
                case 3:
                    gameState.LaCampanellaMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 105000; // 35000 * 3
                    }
                    break;
                case 4:
                    gameState.EnigmaMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 225000; // 75000 * 3
                    }
                    break;
                case 5:
                    gameState.FateMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 405000; // 135000 * 3
                    }
                    break;
                case 6:
                    gameState.OdeToJoyMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 765000; // 255000 * 3
                    }
                    break;
            }
        }

        // ==================== NEW EROICA WEAPON ABILITIES ====================

        /// <summary>
        /// Sakura's Blossom (New Effect): On Major score acquisition,
        /// grants 5 of the corresponding Minor score.
        /// </summary>
        public static void SakurasBlossom_OnMajorAcquisition(GameState gameState, string scoreName)
        {
            if (gameState == null) return;
            
            // Grant 5 minor scores based on which major was crafted
            switch (scoreName)
            {
                case "Moonlight Sonata":
                    gameState.MoonlightMinorOwned += 5;
                    break;
                case "Eroica":
                    gameState.EroicaMinorOwned += 5;
                    break;
                case "Swan Lake":
                    gameState.SwanMinorOwned += 5;
                    break;
                case "La Campanella":
                    gameState.LaCampanellaMinorOwned += 5;
                    break;
                case "Enigma Variations":
                    gameState.EnigmaMinorOwned += 5;
                    break;
                case "Fate":
                    gameState.FateMinorOwned += 5;
                    break;
                case "Ode to Joy":
                    gameState.OdeToJoyMinorOwned += 5;
                    break;
            }
            
            // Debug output to console
            System.Diagnostics.Debug.WriteLine($"Sakura's Blossom: Granted 5 {scoreName} Minor scores");
        }

        /// <summary>
        /// Funeral Prayer (New Effect): Every 10th click grants a Prayer stack.
        /// At 3 stacks, consume all to empower your next 15 clicks to add 6x NPS to each click.
        /// Empowered clicks do not count toward the next amplification.
        /// Call this on every click.
        /// </summary>
        public static void FuneralPrayer_OnClick(GameState gameState)
        {
            if (gameState == null) return;

            // Only increment counter if not currently empowered (empowered clicks don't count)
            if (gameState.FuneralPrayerEmpoweredClicks <= 0)
            {
                gameState.FuneralPrayerClickCounter++;

                // Every 10th click grants a Prayer stack
                if (gameState.FuneralPrayerClickCounter >= 10)
                {
                    gameState.FuneralPrayerClickCounter = 0;
                    gameState.FuneralPrayerStacks++;

                    // At 3 stacks, consume all and empower next 15 clicks
                    if (gameState.FuneralPrayerStacks >= 3)
                    {
                        gameState.FuneralPrayerStacks = 0;
                        gameState.FuneralPrayerEmpoweredClicks = 15;
                    }
                }
            }
        }

        /// <summary>
        /// Funeral Prayer: Get additional NPS bonus to add to click if empowered.
        /// Returns 6x NPS if empowered, 0 otherwise.
        /// </summary>
        public static double FuneralPrayer_GetEmpoweredClickBonus(GameState gameState)
        {
            if (gameState == null) return 0;

            if (gameState.FuneralPrayerEmpoweredClicks > 0)
            {
                gameState.FuneralPrayerEmpoweredClicks--;
                // Return 6x NPS to be added to the click
                return gameState.NotesPerSecond * 6.0;
            }

            return 0;
        }

        // ==================== EROICA MAJOR: TRIUMPH OF HEROES ====================

        /// <summary>
        /// Triumph of Heroes (Eroica Major - Passive): Every Major score acquisition
        /// doubles your current notes.
        /// </summary>
        public static void TriumphOfHeroes_OnMajorAcquisition(GameState gameState)
        {
            if (gameState == null) return;

            double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
            // Add current notes to itself to double the total
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, currentNotes);
            
            // Debug output to console
            System.Diagnostics.Debug.WriteLine($"Triumph of Heroes: Doubled notes from {currentNotes} to {currentNotes * 2}");
        }

        // ==================== SWAN LAKE WEAPONS (4-5) ====================

        /// <summary>
        /// Star-Scattered Wings (Swan I): On buy of Melodious Fragment,
        /// increase current notes by 5 times your NPS.
        /// </summary>
        public static void StarScatteredWings_OnMelodiousPurchase(GameState gameState)
        {
            if (gameState == null) return;
            double bonus = gameState.NotesPerSecond * 5.0;
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
        }

        /// <summary>
        /// Thousand Winged Swan (Swan II): On buy of Harmonious Fragment,
        /// increase current notes by 10 times your NPS.
        /// </summary>
        public static void ThousandWingedSwan_OnHarmoniousPurchase(GameState gameState)
        {
            if (gameState == null) return;
            double bonus = gameState.NotesPerSecond * 10.0;
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
        }

        // ==================== LA CAMPANELLA WEAPONS (6-7) ====================

        /// <summary>
        /// Symphony of Bells (La Campanella I): On craft of a minor score,
        /// give another of that minor score.
        /// Pass the score name to duplicate it.
        /// </summary>
        public static void SymphonyOfBells_OnMinorCraft(GameState gameState, string scoreName)
        {
            if (gameState == null) return;

            switch (scoreName.ToLower())
            {
                case "moonlight":
                    gameState.MoonlightMinorOwned++;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 3000;
                    }
                    break;
                case "eroica":
                    gameState.EroicaMinorOwned++;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 8000;
                    }
                    break;
                case "swanlake":
                    gameState.SwanMinorOwned++;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 15000;
                    }
                    break;
                case "lacampanella":
                    gameState.LaCampanellaMinorOwned++;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 35000;
                    }
                    break;
                case "enigma":
                    gameState.EnigmaMinorOwned++;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 75000;
                    }
                    break;
                case "fate":
                    gameState.FateMinorOwned++;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 135000;
                    }
                    break;
                case "odetojoy":
                    gameState.OdeToJoyMinorOwned++;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 255000;
                    }
                    break;
            }
        }

        /// <summary>
        /// Razer of Bell's Chimes (La Campanella II): On craft of a minor score,
        /// increase NPS by 50% for the next 5 seconds.
        /// </summary>
        public static void RazerOfBellsChimes_OnMinorCraft(GameState gameState)
        {
            if (gameState == null) return;
            // Set NPS boost to expire 5 seconds from now
            gameState.RazerNpsBoostActive = true;
            gameState.RazerNpsBoostExpiry = DateTime.Now.AddSeconds(5);
        }

        // ==================== FATE WEAPONS (10-11) ====================

        /// <summary>
        /// Astral Chainripper (Fate I): On Fate minor craft, quintuple NPS for the next 10 seconds.
        /// </summary>
        public static void AstralChainripper_OnFateMinorCraft(GameState gameState)
        {
            if (gameState == null) return;
            
            // Set NPS boost to expire 10 seconds from now
            gameState.AstralChainripperNpsBoostActive = true;
            gameState.AstralChainripperNpsBoostExpiry = DateTime.Now.AddSeconds(10);
        }

        /// <summary>
        /// Cosmic Weaver (Fate II): On Fate minor craft, quintuple notes per click for the next 10 seconds.
        /// </summary>
        public static void CosmicWeaver_OnFateMinorCraft(GameState gameState)
        {
            if (gameState == null) return;
            
            // Set click boost to expire 10 seconds from now
            gameState.CosmicWeaverClickBoostActive = true;
            gameState.CosmicWeaverClickBoostExpiry = DateTime.Now.AddSeconds(10);
        }

        // ==================== ODE TO JOY WEAPONS (12-13) ====================

        /// <summary>
        /// Joyful Catharsis (Ode to Joy I): Your notes per second are doubled.
        /// This is a passive effect - check the ability flag when calculating NPS.
        /// No method needed here; handled by checking gameState.JoyfulCatharsisAbility in NPS calculation.
        /// </summary>
        // (No method needed - passive multiplier)

        /// <summary>
        /// Ode to Creation (Ode to Joy II): Every 5th click makes your next click
        /// have a 33% increase in notes.
        /// </summary>
        public static void OdeToCreation_OnClick(GameState gameState)
        {
            if (gameState == null) return;

            gameState.OdeToCreationClickCounter++;
            if (gameState.OdeToCreationClickCounter >= 5)
            {
                gameState.OdeToCreationClickCounter = 0;
                gameState.OdeToCreationNextClickBonus = true;
            }
        }

        // ==================== DUET RESONANCES ====================

        /// <summary>
        /// Eroica Duet: If any minor score exceeds 10, double its NPS output.
        /// This is a passive check - should be evaluated during NPS calculation.
        /// Returns the NPS multiplier to apply.
        /// </summary>
        public static double EroicaDuet_GetNpsMultiplier(GameState gameState)
        {
            if (gameState == null) return 1.0;

            // Check if any minor score > 10
            if (gameState.MoonlightMinorOwned > 10 ||
                gameState.EroicaMinorOwned > 10 ||
                gameState.SwanMinorOwned > 10 ||
                gameState.LaCampanellaMinorOwned > 10 ||
                gameState.EnigmaMinorOwned > 10 ||
                gameState.FateMinorOwned > 10 ||
                gameState.OdeToJoyMinorOwned > 10)
            {
                return 2.0; // Double NPS
            }
            return 1.0;
        }

        // Swan Lake Duet passive removed - replaced by Mirror Lake active ability

        /// <summary>
        /// La Campanella Duet "Chime Chain": Click within 1 second to extend chain. Rewards = chainLength² × NPS.
        /// </summary>
        public static void LaCampanellaDuet_OnClick(GameState gameState)
        {
            if (gameState == null || !gameState.LaCampanellaDuetActive) return;

            double timeSinceLastClick = (DateTime.Now - gameState.LastChimeClickTime).TotalSeconds;
            
            if (timeSinceLastClick <= 1.0 || gameState.ChimeChainLength == 0)
            {
                // Extend chain
                gameState.ChimeChainLength++;
                gameState.LastChimeClickTime = DateTime.Now;
                
                // Grant reward: chainLength² × NPS as instant notes
                double reward = gameState.ChimeChainLength * gameState.ChimeChainLength * gameState.NotesPerSecond;
                gameState.Notes += reward;
            }
            else
            {
                // Chain broken, reset
                gameState.ChimeChainLength = 0;
                gameState.LastChimeClickTime = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Enigma Duet "Mystery Clicks": Each click triggers 1 of 8 random effects.
        /// </summary>
        public static void EnigmaDuet_OnClick(GameState gameState)
        {
            if (gameState == null || !gameState.EnigmaDuetActive) return;

            // Increment mystery click count
            gameState.EnigmaMysteryClickCount++;

            // Randomly select one of 8 effects (0-7)
            Random random = new Random();
            int effect = random.Next(0, 8);
            
            switch (effect)
            {
                case 0: // Red - +15× NPS
                    gameState.Notes += gameState.NotesPerSecond * 15;
                    break;
                case 1: // Blue - 2x NPS boost until ability ends
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond *= 2.0;
                    }
                    break;
                case 2: // Green - +30 Harmonious Fragments
                    gameState.HarmoniousOwned += 30;
                    break;
                case 3: // Yellow - +30 Melodious Fragments
                    gameState.MelodiousOwned += 30;
                    break;
                case 4: // Purple - +1 major sheet for each major score owned
                    if (gameState.MoonlightMajorSheets > 0) gameState.MoonlightMajorSheets++;
                    if (gameState.EroicaMajorSheets > 0) gameState.EroicaMajorSheets++;
                    if (gameState.SwanLakeMajorSheets > 0) gameState.SwanLakeMajorSheets++;
                    if (gameState.LaCampanellaMajorSheets > 0) gameState.LaCampanellaMajorSheets++;
                    if (gameState.EnigmaMajorSheets > 0) gameState.EnigmaMajorSheets++;
                    if (gameState.FateMajorSheets > 0) gameState.FateMajorSheets++;
                    if (gameState.OdeToJoyMajorSheets > 0) gameState.OdeToJoyMajorSheets++;
                    break;
                case 5: // Orange - +1 minor component (key, scale, progression) for each minor score owned
                    // Add 1 to each component type for each minor score that has at least 1 of that component
                    if (gameState.MoonlightMinorKeys > 0) gameState.MoonlightMinorKeys++;
                    if (gameState.MoonlightMinorScales > 0) gameState.MoonlightMinorScales++;
                    if (gameState.MoonlightMinorProgressions > 0) gameState.MoonlightMinorProgressions++;
                    if (gameState.EroicaMinorKeys > 0) gameState.EroicaMinorKeys++;
                    if (gameState.EroicaMinorScales > 0) gameState.EroicaMinorScales++;
                    if (gameState.EroicaMinorProgressions > 0) gameState.EroicaMinorProgressions++;
                    if (gameState.SwanLakeMinorKeys > 0) gameState.SwanLakeMinorKeys++;
                    if (gameState.SwanLakeMinorScales > 0) gameState.SwanLakeMinorScales++;
                    if (gameState.SwanLakeMinorProgressions > 0) gameState.SwanLakeMinorProgressions++;
                    if (gameState.LaCampanellaMinorKeys > 0) gameState.LaCampanellaMinorKeys++;
                    if (gameState.LaCampanellaMinorScales > 0) gameState.LaCampanellaMinorScales++;
                    if (gameState.LaCampanellaMinorProgressions > 0) gameState.LaCampanellaMinorProgressions++;
                    if (gameState.EnigmaMinorKeys > 0) gameState.EnigmaMinorKeys++;
                    if (gameState.EnigmaMinorScales > 0) gameState.EnigmaMinorScales++;
                    if (gameState.EnigmaMinorProgressions > 0) gameState.EnigmaMinorProgressions++;
                    if (gameState.FateMinorKeys > 0) gameState.FateMinorKeys++;
                    if (gameState.FateMinorScales > 0) gameState.FateMinorScales++;
                    if (gameState.FateMinorProgressions > 0) gameState.FateMinorProgressions++;
                    if (gameState.OdeToJoyMinorKeys > 0) gameState.OdeToJoyMinorKeys++;
                    if (gameState.OdeToJoyMinorScales > 0) gameState.OdeToJoyMinorScales++;
                    if (gameState.OdeToJoyMinorProgressions > 0) gameState.OdeToJoyMinorProgressions++;
                    break;
                case 6: // White - Best reward (+1 owned to 3 random minor scores)
                    List<Action> minorScoreOwned = new List<Action>
                    {
                        () => gameState.MoonlightMinorOwned++,
                        () => gameState.EroicaMinorOwned++,
                        () => gameState.SwanMinorOwned++,
                        () => gameState.LaCampanellaMinorOwned++,
                        () => gameState.EnigmaMinorOwned++,
                        () => gameState.FateMinorOwned++,
                        () => gameState.OdeToJoyMinorOwned++
                    };
                    for (int i = 0; i < 3; i++)
                    {
                        minorScoreOwned[random.Next(minorScoreOwned.Count)]();
                    }
                    break;
                case 7: // Black - Penalty (Lose 65% of current notes)
                    gameState.Notes *= 0.35;
                    break;
            }
        }

        // OLD DUET METHODS - DEPRECATED AND REMOVED
        // Enigma cipher wheel, Fate time streams, and Ode orchestra conductor have been replaced
        // with new mechanics (Mystery Clicks, Cosmic Dust Harvest, Petal Storm)

        /// <summary>
        /// Fate Duet: Every 45th click doubles whichever minor score is the lowest owned.
        /// DEPRECATED - Old implementation, keeping for reference.
        /// </summary>
        public static void FateDuet_OnClick_OLD(GameState gameState)

        {
            if (gameState == null) return;

            gameState.FateDuetClickCounter++;
            if (gameState.FateDuetClickCounter >= 45)
            {
                gameState.FateDuetClickCounter = 0;
                
                // Find lowest owned minor
                int[] minorScores = new int[] {
                    gameState.MoonlightMinorOwned, gameState.EroicaMinorOwned,
                    gameState.SwanMinorOwned, gameState.LaCampanellaMinorOwned,
                    gameState.EnigmaMinorOwned, gameState.FateMinorOwned,
                    gameState.OdeToJoyMinorOwned
                };
                int minIndex = 0, minVal = minorScores[0];
                for (int i = 1; i < minorScores.Length; i++)
                    if (minorScores[i] < minVal) { minVal = minorScores[i]; minIndex = i; }
                
                // Double it
                switch (minIndex)
                {
                    case 0:
                        {
                            int originalCount = gameState.MoonlightMinorOwned;
                            gameState.MoonlightMinorOwned *= 2;
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += originalCount * 3000; // Add NPS for the doubled amount
                            }
                        }
                        break;
                    case 1:
                        {
                            int originalCount = gameState.EroicaMinorOwned;
                            gameState.EroicaMinorOwned *= 2;
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += originalCount * 8000;
                            }
                        }
                        break;
                    case 2:
                        {
                            int originalCount = gameState.SwanMinorOwned;
                            gameState.SwanMinorOwned *= 2;
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += originalCount * 15000;
                            }
                        }
                        break;
                    case 3:
                        {
                            int originalCount = gameState.LaCampanellaMinorOwned;
                            gameState.LaCampanellaMinorOwned *= 2;
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += originalCount * 35000;
                            }
                        }
                        break;
                    case 4:
                        {
                            int originalCount = gameState.EnigmaMinorOwned;
                            gameState.EnigmaMinorOwned *= 2;
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += originalCount * 75000;
                            }
                        }
                        break;
                    case 5:
                        {
                            int originalCount = gameState.FateMinorOwned;
                            gameState.FateMinorOwned *= 2;
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += originalCount * 135000;
                            }
                        }
                        break;
                    case 6:
                        {
                            int originalCount = gameState.OdeToJoyMinorOwned;
                            gameState.OdeToJoyMinorOwned *= 2;
                            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                            {
                                gameState.NotesPerSecond += originalCount * 255000;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Ode to Joy Duet: Notes per Second becomes Notes per Half-Second.
        /// This effectively doubles the rate at which NPS is applied.
        /// Handle this in the main timer loop by checking the duet ability flag
        /// and running NPS updates twice as frequently (or doubling the amount per tick).
        /// No method needed here - passive timer modification.
        /// </summary>
        // (No method needed - handled in timer logic)

        // ==================== WINTER EVENT WEAPONS (16-17) ====================
        // Theme: "Eternal Frost" - Freeze mechanics with extended duration and multiplier effects

        /// <summary>
        /// Cacophonic Blizzard (Winter I): Crystalline Shatter
        /// Forte Effect: Every 10th click grants notes equal to 10 seconds worth of your current NPS instantly.
        /// Duet Effect (with The Snow's Desire): Every 50th click freezes NPS for 12 seconds.
        /// </summary>
        public static void CacophonicBlizzard_OnClick(GameState gameState)
        {
            if (gameState == null) return;

            // Crystalline Shatter - solo forte effect (every 10th click)
            if (gameState.CacophonicBlizzardAbility)
            {
                gameState.CrystallineShatterCounter++;
                if (gameState.CrystallineShatterCounter >= 10)
                {
                    gameState.CrystallineShatterCounter = 0;
                    // Grant 10 seconds worth of NPS instantly
                    double burst = gameState.NotesPerSecond * 10.0;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, burst);
                }
            }

            // Duet effect - freeze NPS every 50th click (only when both Winter weapons equipped)
            gameState.CacophonicBlizzardClickCounter++;
            if (gameState.CacophonicBlizzardClickCounter >= 50)
            {
                gameState.CacophonicBlizzardClickCounter = 0;
                gameState.NpsFrozen = true;
                gameState.FrozenNpsValue = gameState.NotesPerSecond;
                gameState.NpsFreezeExpiry = DateTime.Now.AddSeconds(12); // Extended duration
            }
        }

        /// <summary>
        /// The Snow's Desire (Winter II): Blizzard's Bounty
        /// Forte Effect: Each Harmonious fragment purchase grants +2% NPS for 30 seconds (stacks additively, multiple purchases extend duration).
        /// </summary>
        public static void TheSnowsDesire_OnHarmoniousPurchase(GameState gameState)
        {
            if (gameState == null) return;

            // Blizzard's Bounty - solo forte effect
            if (gameState.TheSnowsDesireAbility)
            {
                // Add 2% to the bonus (stacks additively)
                gameState.BlizzardBountyNpsBonus += 0.02;
                // Extend/set duration to 30 seconds from now
                gameState.BlizzardBountyExpiry = DateTime.Now.AddSeconds(30);
            }
        }

        /// <summary>
        /// Winter Duet: "Absolute Zero"
        /// Duet Ability (Manual activation, 5min cooldown, 15s base duration):
        /// Activate to convert frozen NPS into a click multiplier (requires NPS to be frozen first).
        /// Every click during this time extends duration by 0.5s (max +10s extension).
        /// Returns the multiplier to apply to clicks when duet is active.
        /// </summary>
        public static double WinterDuet_GetClickMultiplier(GameState gameState)
        {
            if (gameState == null) return 1.0;

            // Check if Winter Duet ability is active
            if (gameState.WinterDuetActive && DateTime.Now <= gameState.WinterDuetExpiry)
            {
                // Check if NPS is frozen and hasn't expired
                if (gameState.NpsFrozen && DateTime.Now <= gameState.NpsFreezeExpiry)
                {
                    return gameState.FrozenNpsValue; // Frozen NPS as multiplier
                }
            }
            return 1.0;
        }

        /// <summary>
        /// Winter Duet: Extend duration on click (called from click handler).
        /// Each click extends the ability duration by 0.5s, up to a maximum of +10s total extension.
        /// </summary>
        public static void WinterDuet_OnClick(GameState gameState)
        {
            if (gameState == null) return;

            if (gameState.WinterDuetActive && DateTime.Now <= gameState.WinterDuetExpiry)
            {
                // Only extend if we haven't hit the max extension
                if (gameState.WinterDuetExtensionTime < 10.0)
                {
                    gameState.WinterDuetExpiry = gameState.WinterDuetExpiry.AddSeconds(0.5);
                    gameState.WinterDuetExtensionTime += 0.5;
                }
            }
        }

        // ==================== DIES IRAE EVENT WEAPONS (14-15) ====================
        // Theme: "Descending Judgment" - Seven Seals mechanic with resource efficiency

        /// <summary>
        /// Seven Circles (Dies Irae I): Hellfire Rebate
        /// Forte Effect: Minor score crafts have 33% chance to not consume components.
        /// Returns true if components should be refunded.
        /// </summary>
        public static bool SevenCircles_CheckMinorCraftRefund()
        {
            Random rand = new Random();
            return rand.NextDouble() < 0.33; // 33% chance
        }

        /// <summary>
        /// Hell's Wrath (Dies Irae II): Damnation's Gift
        /// Forte Effect: Each click has a 7% chance to grant a random minor component.
        /// Call this from the click handler when Hell's Wrath ability is active.
        /// </summary>
        public static void HellsWrath_OnClick(GameState gameState)
        {
            if (gameState == null) return;
            if (!gameState.HellsWrath) return;

            Random rand = new Random();
            if (rand.NextDouble() < 0.07) // 7% chance
            {
                // Pick a random score (0-6)
                int scoreIndex = rand.Next(7);
                // Pick a random component type (0=Keys, 1=Scales, 2=Progressions)
                int componentType = rand.Next(3);

                switch (scoreIndex)
                {
                    case 0: // Moonlight
                        if (componentType == 0) gameState.MoonlightMinorKeys++;
                        else if (componentType == 1) gameState.MoonlightMinorScales++;
                        else gameState.MoonlightMinorProgressions++;
                        break;
                    case 1: // Eroica
                        if (componentType == 0) gameState.EroicaMinorKeys++;
                        else if (componentType == 1) gameState.EroicaMinorScales++;
                        else gameState.EroicaMinorProgressions++;
                        break;
                    case 2: // Swan Lake
                        if (componentType == 0) gameState.SwanLakeMinorKeys++;
                        else if (componentType == 1) gameState.SwanLakeMinorScales++;
                        else gameState.SwanLakeMinorProgressions++;
                        break;
                    case 3: // La Campanella
                        if (componentType == 0) gameState.LaCampanellaMinorKeys++;
                        else if (componentType == 1) gameState.LaCampanellaMinorScales++;
                        else gameState.LaCampanellaMinorProgressions++;
                        break;
                    case 4: // Enigma
                        if (componentType == 0) gameState.EnigmaMinorKeys++;
                        else if (componentType == 1) gameState.EnigmaMinorScales++;
                        else gameState.EnigmaMinorProgressions++;
                        break;
                    case 5: // Fate
                        if (componentType == 0) gameState.FateMinorKeys++;
                        else if (componentType == 1) gameState.FateMinorScales++;
                        else gameState.FateMinorProgressions++;
                        break;
                    case 6: // Ode to Joy
                        if (componentType == 0) gameState.OdeToJoyMinorKeys++;
                        else if (componentType == 1) gameState.OdeToJoyMinorScales++;
                        else gameState.OdeToJoyMinorProgressions++;
                        break;
                }
            }
        }

        /// <summary>
        /// Dies Irae Duet: "Seven Seals"
        /// Duet Ability (Toggle, 120s cooldown, 30s duration):
        /// Each click places a seal. At 7 seals, auto-triggers a random minor craft for free.
        /// Keeps stacking seals during duration (resets at 7 each time).
        /// Call this from click handler when duet is active.
        /// </summary>
        public static void DiesIraeDuet_OnClick(GameState gameState, MainWindow mainWindow)
        {
            if (gameState == null || mainWindow == null) return;

            // Only process if duet ability is active
            if (!gameState.DiesIraeDuetActive || DateTime.Now > gameState.DiesIraeDuetExpiry)
                return;

            gameState.SevenSealsCounter++;

            // When we hit 7 seals, trigger a random free minor craft
            if (gameState.SevenSealsCounter >= 7)
            {
                gameState.SevenSealsCounter = 0;
                TriggerRandomMinorCraft(gameState);
                
                // Recalculate NPS/NPC after granting minor scores
                if (mainWindow != null)
                {
                    UIUpdater.UpdateUnitySymphonyUI(mainWindow, gameState);
                }
            }
        }

        /// <summary>
        /// Helper method to grant 3 random minor scores for free (for Seven Seals ability).
        /// Picks a random score type and gives 3 of that minor score without consuming components.
        /// </summary>
        private static void TriggerRandomMinorCraft(GameState gameState)
        {
            if (gameState == null) return;

            Random rand = new Random();
            int scoreType = rand.Next(0, 7); // 0-6 for the 7 score types

            switch (scoreType)
            {
                case 0: // Moonlight Sonata
                    gameState.MoonlightMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 9000; // 3000 * 3
                    }
                    break;
                case 1: // Eroica
                    gameState.EroicaMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 24000; // 8000 * 3
                    }
                    break;
                case 2: // Swan Lake
                    gameState.SwanMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 45000; // 15000 * 3
                    }
                    break;
                case 3: // La Campanella
                    gameState.LaCampanellaMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 105000; // 35000 * 3
                    }
                    break;
                case 4: // Enigma
                    gameState.EnigmaMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 225000; // 75000 * 3
                    }
                    break;
                case 5: // Fate
                    gameState.FateMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 405000; // 135000 * 3
                    }
                    break;
                case 6: // Ode to Joy
                    gameState.OdeToJoyMinorOwned += 3;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += 765000; // 255000 * 3
                    }
                    break;
            }
        }

        /// <summary>
        /// Eroica Duet: Victory March - Progress bar that fills with clicks (100 clicks).
        /// Grants escalating rewards at 25%, 50%, 75%, and 100%.
        /// Call this from click handler when duet is active.
        /// </summary>
        public static void EroicaDuet_OnClick(GameState gameState, MainWindow mainWindow)
        {
            if (gameState == null || mainWindow == null) return;

            // Only process if duet ability is active
            if (!gameState.EroicaDuetActive)
                return;

            gameState.VictoryMarchClicks++;

            // Check for milestone rewards
            if (gameState.VictoryMarchClicks >= 25 && !gameState.VictoryMarch25Claimed)
            {
                // 25%: Grant 1 random minor score (that player owns)
                gameState.VictoryMarch25Claimed = true;
                GrantRandomMinorScore(gameState);
            }

            if (gameState.VictoryMarchClicks >= 50 && !gameState.VictoryMarch50Claimed)
            {
                // 50%: Grant 1 random major score (that player owns)
                gameState.VictoryMarch50Claimed = true;
                GrantRandomMajorScore(gameState);
            }

            if (gameState.VictoryMarchClicks >= 75 && !gameState.VictoryMarch75Claimed)
            {
                // 75%: Grant +50 Melodious and Harmonious fragments
                gameState.VictoryMarch75Claimed = true;
                gameState.MelodiousOwned += 50;
                gameState.HarmoniousOwned += 50;
            }

            if (gameState.VictoryMarchClicks >= 100 && !gameState.VictoryMarch100Claimed)
            {
                // 100%: Double all owned score values
                gameState.VictoryMarch100Claimed = true;
                DoubleAllScoreValues(gameState);
                
                // Auto-deactivate and start cooldown
                gameState.EroicaDuetActive = false;
                gameState.EroicaDuetCooldownExpiry = DateTime.Now.AddSeconds(300); // 5 minutes
                gameState.VictoryMarchClicks = 0;
                gameState.VictoryMarch25Claimed = false;
                gameState.VictoryMarch50Claimed = false;
                gameState.VictoryMarch75Claimed = false;
                gameState.VictoryMarch100Claimed = false;
            }

            // Recalculate NPS/NPC after granting scores
            if (mainWindow != null && gameState.VictoryMarchClicks >= 25)
            {
                UIUpdater.UpdateUnitySymphonyUI(mainWindow, gameState);
            }
        }

        /// <summary>
        /// Helper method to grant 1 random minor score from ones the player owns.
        /// </summary>
        private static void GrantRandomMinorScore(GameState gameState)
        {
            if (gameState == null) return;

            // Find all minor scores the player owns
            var ownedMinorScores = new System.Collections.Generic.List<int>();
            if (gameState.MoonlightMinorOwned > 0) ownedMinorScores.Add(0);
            if (gameState.EroicaMinorOwned > 0) ownedMinorScores.Add(1);
            if (gameState.SwanMinorOwned > 0) ownedMinorScores.Add(2);
            if (gameState.LaCampanellaMinorOwned > 0) ownedMinorScores.Add(3);
            if (gameState.EnigmaMinorOwned > 0) ownedMinorScores.Add(4);
            if (gameState.FateMinorOwned > 0) ownedMinorScores.Add(5);
            if (gameState.OdeToJoyMinorOwned > 0) ownedMinorScores.Add(6);

            if (ownedMinorScores.Count == 0) return; // No minor scores owned yet

            Random rand = new Random();
            int selectedScore = ownedMinorScores[rand.Next(ownedMinorScores.Count)];

            switch (selectedScore)
            {
                case 0: // Moonlight
                    gameState.MoonlightMinorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 3000;
                    break;
                case 1: // Eroica
                    gameState.EroicaMinorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 8000;
                    break;
                case 2: // Swan Lake
                    gameState.SwanMinorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 15000;
                    break;
                case 3: // La Campanella
                    gameState.LaCampanellaMinorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 35000;
                    break;
                case 4: // Enigma
                    gameState.EnigmaMinorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 75000;
                    break;
                case 5: // Fate
                    gameState.FateMinorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 135000;
                    break;
                case 6: // Ode to Joy
                    gameState.OdeToJoyMinorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 255000;
                    break;
            }
        }

        /// <summary>
        /// Helper method to grant 1 random major score from ones the player owns.
        /// </summary>
        private static void GrantRandomMajorScore(GameState gameState)
        {
            if (gameState == null) return;

            // Find all major scores the player owns
            var ownedMajorScores = new System.Collections.Generic.List<int>();
            if (gameState.MoonlightMajorOwned > 0) ownedMajorScores.Add(0);
            if (gameState.EroicaMajorOwned > 0) ownedMajorScores.Add(1);
            if (gameState.SwanMajorOwned > 0) ownedMajorScores.Add(2);
            if (gameState.LaCampanellaMajorOwned > 0) ownedMajorScores.Add(3);
            if (gameState.EnigmaMajorOwned > 0) ownedMajorScores.Add(4);
            if (gameState.FateMajorOwned > 0) ownedMajorScores.Add(5);
            if (gameState.OdeToJoyMajorOwned > 0) ownedMajorScores.Add(6);

            if (ownedMajorScores.Count == 0) return; // No major scores owned yet

            Random rand = new Random();
            int selectedScore = ownedMajorScores[rand.Next(ownedMajorScores.Count)];

            switch (selectedScore)
            {
                case 0: // Moonlight
                    gameState.MoonlightMajorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 180000;
                    break;
                case 1: // Eroica
                    gameState.EroicaMajorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 480000;
                    break;
                case 2: // Swan Lake
                    gameState.SwanMajorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 900000;
                    break;
                case 3: // La Campanella
                    gameState.LaCampanellaMajorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 2100000;
                    break;
                case 4: // Enigma
                    gameState.EnigmaMajorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 4500000;
                    break;
                case 5: // Fate
                    gameState.FateMajorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 8100000;
                    break;
                case 6: // Ode to Joy
                    gameState.OdeToJoyMajorOwned += 1;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 15300000;
                    break;
            }
        }

        /// <summary>
        /// Helper method to double all owned score values (minor and major).
        /// Recalculates NPS based on the doubled values.
        /// </summary>
        private static void DoubleAllScoreValues(GameState gameState)
        {
            if (gameState == null) return;

            // Store old values to calculate the NPS increase
            int oldMoonlightMinor = gameState.MoonlightMinorOwned;
            int oldEroicaMinor = gameState.EroicaMinorOwned;
            int oldSwanMinor = gameState.SwanMinorOwned;
            int oldLaCampanellaMinor = gameState.LaCampanellaMinorOwned;
            int oldEnigmaMinor = gameState.EnigmaMinorOwned;
            int oldFateMinor = gameState.FateMinorOwned;
            int oldOdeMinor = gameState.OdeToJoyMinorOwned;

            int oldMoonlightMajor = gameState.MoonlightMajorOwned;
            int oldEroicaMajor = gameState.EroicaMajorOwned;
            int oldSwanMajor = gameState.SwanMajorOwned;
            int oldLaCampanellaMajor = gameState.LaCampanellaMajorOwned;
            int oldEnigmaMajor = gameState.EnigmaMajorOwned;
            int oldFateMajor = gameState.FateMajorOwned;
            int oldOdeMajor = gameState.OdeToJoyMajorOwned;

            // Double all owned scores
            gameState.MoonlightMinorOwned *= 2;
            gameState.EroicaMinorOwned *= 2;
            gameState.SwanMinorOwned *= 2;
            gameState.LaCampanellaMinorOwned *= 2;
            gameState.EnigmaMinorOwned *= 2;
            gameState.FateMinorOwned *= 2;
            gameState.OdeToJoyMinorOwned *= 2;

            gameState.MoonlightMajorOwned *= 2;
            gameState.EroicaMajorOwned *= 2;
            gameState.SwanMajorOwned *= 2;
            gameState.LaCampanellaMajorOwned *= 2;
            gameState.EnigmaMajorOwned *= 2;
            gameState.FateMajorOwned *= 2;
            gameState.OdeToJoyMajorOwned *= 2;

            // Add NPS for the doubled amounts (only add the difference)
            if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
            {
                gameState.NotesPerSecond += oldMoonlightMinor * 3000;
                gameState.NotesPerSecond += oldEroicaMinor * 8000;
                gameState.NotesPerSecond += oldSwanMinor * 15000;
                gameState.NotesPerSecond += oldLaCampanellaMinor * 35000;
                gameState.NotesPerSecond += oldEnigmaMinor * 75000;
                gameState.NotesPerSecond += oldFateMinor * 135000;
                gameState.NotesPerSecond += oldOdeMinor * 255000;

                gameState.NotesPerSecond += oldMoonlightMajor * 180000;
                gameState.NotesPerSecond += oldEroicaMajor * 480000;
                gameState.NotesPerSecond += oldSwanMajor * 900000;
                gameState.NotesPerSecond += oldLaCampanellaMajor * 2100000;
                gameState.NotesPerSecond += oldEnigmaMajor * 4500000;
                gameState.NotesPerSecond += oldFateMajor * 8100000;
                gameState.NotesPerSecond += oldOdeMajor * 15300000;
            }
        }

        #region Swan Lake Duet - Mirror Lake (Action Reflection)

        /// <summary>
        /// Queue an action to be reflected after 3 seconds during Mirror Lake duet
        /// </summary>
        public static void QueueMirrorAction(GameState gameState, string action, object data)
        {
            if (!gameState.SwanLakeDuetActive) return;
            if (DateTime.Now > gameState.SwanLakeDuetExpiry) return;

            // Add to queue with 3-second delay
            DateTime executeTime = DateTime.Now.AddSeconds(3);
            gameState.MirrorLakeQueue.Add((action, data, executeTime));
        }

        /// <summary>
        /// Process queued Mirror Lake actions (called from background timer)
        /// </summary>
        public static void ProcessMirrorLakeQueue(GameState gameState)
        {
            if (gameState.MirrorLakeQueue.Count == 0) return;

            DateTime now = DateTime.Now;
            List<(string action, object data, DateTime executeTime)> toRemove = new();

            foreach (var queuedAction in gameState.MirrorLakeQueue)
            {
                if (now >= queuedAction.executeTime)
                {
                    // Execute the reflected action
                    ExecuteMirrorAction(gameState, queuedAction.action, queuedAction.data);
                    toRemove.Add(queuedAction);
                }
            }

            // Remove executed actions
            foreach (var action in toRemove)
            {
                gameState.MirrorLakeQueue.Remove(action);
            }
        }

        private static void ExecuteMirrorAction(GameState gameState, string action, object data)
        {
            switch (action)
            {
                case "Click":
                    // Add NPC directly without re-queueing
                    if (data is double npcValue)
                    {
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, npcValue);
                    }
                    break;

                case "BuyUpgrade":
                    // Re-purchase upgrade without re-queueing
                    if (data is (string upgradeName, double cost))
                    {
                        if (MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes) >= cost)
                        {
                            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -cost);
                            // Increment the appropriate upgrade counter
                            IncrementUpgrade(gameState, upgradeName);
                        }
                    }
                    break;

                case "CraftMinor":
                    if (data is string minorScore)
                    {
                        CraftMinorScore(gameState, minorScore);
                    }
                    break;

                case "CraftMajor":
                    if (data is string majorScore)
                    {
                        CraftMajorScore(gameState, majorScore);
                    }
                    break;

                case "BuyFragment":
                    if (data is (string fragmentType, double fragmentCost))
                    {
                        if (MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes) >= fragmentCost)
                        {
                            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -fragmentCost);
                            if (fragmentType == "Melodious")
                                gameState.MelodiousOwned++;
                            else if (fragmentType == "Harmonious")
                                gameState.HarmoniousOwned++;
                        }
                    }
                    break;
            }
        }

        private static void IncrementUpgrade(GameState gameState, string upgradeName)
        {
            // Map upgrade names to their fields
            switch (upgradeName)
            {
                case "Moonlight Sonata - Opus 1": gameState.ChordOwned++; break;
                case "Moonlight Sonata - Opus 2": gameState.ScaleOwned++; break;
                case "Moonlight Sonata - Opus 3": gameState.OrchestraOwned++; break;
                case "Eroica - Opus 1": gameState.SymphonyOwned++; break;
                case "Eroica - Opus 2": gameState.AriaOwned++; break;
                case "Eroica - Opus 3": gameState.RequiemOwned++; break;
                case "Swan Lake - Opus 1": gameState.OpusOwned++; break;
                case "Swan Lake - Opus 2": gameState.MagnumOpusOwned++; break;
                case "Swan Lake - Opus 3": gameState.ChordOwned++; break;
                case "La Campanella - Opus 1": gameState.ScaleOwned++; break;
                case "La Campanella - Opus 2": gameState.OrchestraOwned++; break;
                case "La Campanella - Opus 3": gameState.SymphonyOwned++; break;
                case "Enigma Variations - Opus 1": gameState.AriaOwned++; break;
                case "Enigma Variations - Opus 2": gameState.RequiemOwned++; break;
                case "Enigma Variations - Opus 3": gameState.OpusOwned++; break;
                case "Fate - Opus 1": gameState.MagnumOpusOwned++; break;
                case "Fate - Opus 2": gameState.ChordOwned++; break;
                case "Fate - Opus 3": gameState.ScaleOwned++; break;
                case "Ode to Joy - Opus 1": gameState.OrchestraOwned++; break;
                case "Ode to Joy - Opus 2": gameState.SymphonyOwned++; break;
                case "Ode to Joy - Opus 3": gameState.AriaOwned++; break;
            }
        }

        private static void CraftMinorScore(GameState gameState, string scoreName)
        {
            // Check if player has fragments
            if (gameState.MelodiousOwned < 10 || gameState.HarmoniousOwned < 20) return;

            gameState.MelodiousOwned -= 10;
            gameState.HarmoniousOwned -= 20;

            // Add to owned count
            switch (scoreName)
            {
                case "Moonlight Sonata": gameState.MoonlightMinorOwned++; break;
                case "Eroica": gameState.EroicaMinorOwned++; break;
                case "Swan Lake": gameState.SwanMinorOwned++; break;
                case "La Campanella": gameState.LaCampanellaMinorOwned++; break;
                case "Enigma Variations": gameState.EnigmaMinorOwned++; break;
                case "Fate": gameState.FateMinorOwned++; break;
                case "Ode to Joy": gameState.OdeToJoyMinorOwned++; break;
            }
        }

        private static void CraftMajorScore(GameState gameState, string scoreName)
        {
            // Check if player has fragments
            if (gameState.MelodiousOwned < 60 || gameState.HarmoniousOwned < 120) return;

            gameState.MelodiousOwned -= 60;
            gameState.HarmoniousOwned -= 120;

            // Add to owned count
            switch (scoreName)
            {
                case "Moonlight Sonata": gameState.MoonlightMajorOwned++; break;
                case "Eroica": gameState.EroicaMajorOwned++; break;
                case "Swan Lake": gameState.SwanMajorOwned++; break;
                case "La Campanella": gameState.LaCampanellaMajorOwned++; break;
                case "Enigma Variations": gameState.EnigmaMajorOwned++; break;
                case "Fate": gameState.FateMajorOwned++; break;
                case "Ode to Joy": gameState.OdeToJoyMajorOwned++; break;
            }

            // Trigger Sakura's Blossom effect if weapon ability is active
            if (gameState.SakurasBlossomAbility)
            {
                SakurasBlossom_OnMajorAcquisition(gameState, scoreName);
            }

            // Trigger Triumph of Heroes effect if Eroica Major is resonated
            if (gameState.EroicaMajorAbility)
            {
                TriumphOfHeroes_OnMajorAcquisition(gameState);
            }
        }

        #endregion

        #region Fate Duet: Hourglass Reversal

        /// <summary>
        /// Fate Duet: Bank an action during the banking phase (first 10 seconds)
        /// </summary>
        public static void FateDuet_BankAction(GameState gameState, string actionType, object actionData)
        {
            if (!gameState.FateDuetActive || gameState.FateDuetHasFlipped) return;

            // Increment click counter if this is a click action
            if (actionType == "Click")
            {
                gameState.FateDuetClickCount++;
            }

            // Add action to bank with timestamp
            gameState.HourglassActionBank.Add((actionType, actionData, DateTime.Now));
        }

        /// <summary>
        /// Fate Duet: Check if hourglass should flip and process replay phase
        /// Call this every game tick
        /// </summary>
        public static void FateDuet_ProcessHourglass(GameState gameState)
        {
            if (!gameState.FateDuetActive) return;

            // Check if it's time to flip (after 10 seconds)
            if (!gameState.FateDuetHasFlipped && DateTime.Now >= gameState.FateDuetFlipTime)
            {
                gameState.FateDuetHasFlipped = true;
                
                // Use click count as multiplier (minimum 1x)
                int multiplier = Math.Max(1, gameState.FateDuetClickCount);
                
                // Reverse the action bank for replay
                gameState.HourglassActionBank.Reverse();
                
                // Replay all actions at (click count)× effectiveness
                foreach (var (actionType, actionData, recordTime) in gameState.HourglassActionBank)
                {
                    FateDuet_ReplayAction(gameState, actionType, actionData, multiplier);
                }
            }
        }

        private static void FateDuet_ReplayAction(GameState gameState, string actionType, object actionData, int multiplier)
        {
            switch (actionType)
            {
                case "Click":
                    // Dynamic multiplier based on click count
                    double clickValue = gameState.NotesPerClick * multiplier;
                    
                    // Apply all standard click multipliers
                    if (gameState.MoonlightDuetActive && MoonlightDuet_GetCurrentPhase(gameState) == 0)
                        clickValue *= 2.0; // New Moon phase
                    
                    if (gameState.WinterDuetActive && gameState.NpsFrozen)
                        clickValue *= gameState.FrozenNpsValue;
                    
                    if (gameState.CosmicWeaverClickBoostActive)
                        clickValue *= 5.0;
                    
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, clickValue);
                    break;

                case "BuyUpgrade":
                    if (actionData is (string upgradeName, double cost))
                    {
                        double multipliedCost = cost * multiplier;
                        if (MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes) >= multipliedCost)
                        {
                            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -multipliedCost);
                            // Purchase upgrade (multiplier) times
                            for (int i = 0; i < multiplier; i++)
                            {
                                IncrementUpgrade(gameState, upgradeName);
                            }
                        }
                    }
                    break;

                case "CraftMinor":
                    if (actionData is string minorScore)
                    {
                        // Craft (multiplier) times
                        for (int i = 0; i < multiplier; i++)
                        {
                            CraftMinorScore(gameState, minorScore);
                        }
                    }
                    break;

                case "CraftMajor":
                    if (actionData is string majorScore)
                    {
                        // Craft (multiplier) times
                        for (int i = 0; i < multiplier; i++)
                        {
                            CraftMajorScore(gameState, majorScore);
                        }
                    }
                    break;

                case "BuyFragment":
                    if (actionData is (string fragmentType, double fragmentCost))
                    {
                        double multipliedCost = fragmentCost * multiplier;
                        if (MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes) >= multipliedCost)
                        {
                            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -multipliedCost);
                            // Give (multiplier)x fragments
                            if (fragmentType == "Melodious")
                                gameState.MelodiousOwned += multiplier;
                            else if (fragmentType == "Harmonious")
                                gameState.HarmoniousOwned += multiplier;
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Ode to Joy Duet: Crescendo Conductor

        /// <summary>
        /// Ode to Joy Duet: Add a note to the melody and check for rewards
        /// Repeatable: Every 16 notes completes a section and starts a new cycle
        /// Call this on each click during the duet
        /// </summary>
        public static void OdeDuet_AddNote(GameState gameState)
        {
            if (!gameState.OdeDuetActive) return;

            gameState.CrescendoNotesPlaced++;

            // Check for phrase rewards within current cycle
            if (gameState.CrescendoNotesPlaced == 4 && !gameState.Crescendo4Claimed)
            {
                gameState.Crescendo4Claimed = true;
                // Reward: +25 Fragments (split evenly)
                gameState.MelodiousOwned += 13;
                gameState.HarmoniousOwned += 12;
            }
            else if (gameState.CrescendoNotesPlaced == 8 && !gameState.Crescendo8Claimed)
            {
                gameState.Crescendo8Claimed = true;
                // Reward: Random minor score
                OdeDuet_GrantRandomMinor(gameState);
            }
            else if (gameState.CrescendoNotesPlaced == 12 && !gameState.Crescendo12Claimed)
            {
                gameState.Crescendo12Claimed = true;
                // Reward: Random major sheet
                OdeDuet_GrantRandomMajorSheet(gameState);
            }
            else if (gameState.CrescendoNotesPlaced == 16 && !gameState.Crescendo16Claimed)
            {
                gameState.Crescendo16Claimed = true;
                // Reward: 5x NPS boost for 15 seconds
                gameState.OdeDuetNpsBoostActive = true;
                gameState.OdeDuetNpsBoostExpiry = DateTime.Now.AddSeconds(15);
                
                // Complete the section and reset for next cycle
                gameState.CrescendoCompletedSections++;
                gameState.CrescendoNotesPlaced = 0;
                gameState.Crescendo4Claimed = false;
                gameState.Crescendo8Claimed = false;
                gameState.Crescendo12Claimed = false;
                gameState.Crescendo16Claimed = false;
            }
        }

        private static void OdeDuet_GrantRandomMinor(GameState gameState)
        {
            Random rng = new Random();
            int scoreType = rng.Next(7); // 0-6 for 7 score types

            switch (scoreType)
            {
                case 0: gameState.MoonlightMinorOwned++; break;
                case 1: gameState.EroicaMinorOwned++; break;
                case 2: gameState.SwanMinorOwned++; break;
                case 3: gameState.LaCampanellaMinorOwned++; break;
                case 4: gameState.EnigmaMinorOwned++; break;
                case 5: gameState.FateMinorOwned++; break;
                case 6: gameState.OdeToJoyMinorOwned++; break;
            }
        }

        private static void OdeDuet_GrantRandomMajorSheet(GameState gameState)
        {
            Random rng = new Random();
            int scoreType = rng.Next(7); // 0-6 for 7 score types

            switch (scoreType)
            {
                case 0: gameState.MoonlightMajorSheets++; break;
                case 1: gameState.EroicaMajorSheets++; break;
                case 2: gameState.SwanLakeMajorSheets++; break;
                case 3: gameState.LaCampanellaMajorSheets++; break;
                case 4: gameState.EnigmaMajorSheets++; break;
                case 5: gameState.FateMajorSheets++; break;
                case 6: gameState.OdeToJoyMajorSheets++; break;
            }
        }

        #endregion
    }
}
