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

        // ==================== EROICA CRESCENDANCE ====================

        /// <summary>
        /// Eroica Crescendance: Every minor craft grants +1 Heroic Resolve stack.
        /// Call this when a minor score is crafted.
        /// </summary>
        public static void EroicaCrescendance_OnMinorCraft(GameState gameState)
        {
            if (gameState == null) return;
            if (!gameState.EroicaMajorAbility) return; // Requires Eroica Major

            gameState.HeroicResolveStacks++;
        }

        /// <summary>
        /// Eroica Crescendance: Consume 1 Heroic Resolve + 25% current notes to gain 1 Symphonic Catharsis.
        /// </summary>
        public static bool EroicaCrescendance_ConsumeForCatharsis(GameState gameState)
        {
            if (gameState == null) return false;
            if (!gameState.EroicaMajorAbility) return false;
            if (gameState.HeroicResolveStacks < 1) return false;

            double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
            double costNotes = currentNotes * 0.25;

            if (currentNotes < costNotes) return false;

            // Consume resources
            gameState.HeroicResolveStacks--;
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -costNotes);

            // Grant Symphonic Catharsis
            gameState.SymphonicCatharsisStacks++;

            return true;
        }

        /// <summary>
        /// Eroica Crescendance: Consume Symphonic Catharsis for 10s of double NPC + 10% crit chance.
        /// Crescendance Bond - Sakura's Blossom: Triggers Crimson Requiem (30 special crits).
        /// </summary>
        public static bool EroicaCrescendance_ConsumeCatharsis(GameState gameState)
        {
            if (gameState == null) return false;
            if (!gameState.EroicaMajorAbility) return false;
            if (gameState.SymphonicCatharsisStacks < 1) return false;

            // Consume stack
            gameState.SymphonicCatharsisStacks--;

            // Activate buff: 10s double NPC + 10% crit
            gameState.SymphonicCatharsisActive = true;
            gameState.SymphonicCatharsisExpiry = DateTime.Now.AddSeconds(10);

            // Crescendance Bond - Sakura's Blossom: Crimson Requiem
            if (gameState.SakurasBlossom && 
                (gameState.CurrentResonatedWeapon1 == "SakurasBlossom" || gameState.CurrentResonatedWeapon2 == "SakurasBlossom"))
            {
                gameState.CrimsonRequiemClicksRemaining = 30;
            }

            // Crescendance Bond - Funeral Prayer: Testament of Harmony
            if (gameState.FuneralPrayer && 
                (gameState.CurrentResonatedWeapon1 == "FuneralPrayer" || gameState.CurrentResonatedWeapon2 == "FuneralPrayer"))
            {
                // Each Heroic Resolve consumed earlier grants Testament - we just consumed Catharsis made from 1 Resolve
                gameState.TestamentOfHarmonyStacks++;
            }

            return true;
        }

        /// <summary>
        /// Crescendance Bond - Funeral Prayer: Consume Testament of Harmony for +100 Melodic and Harmonic Fragments.
        /// </summary>
        public static bool FuneralPrayer_ConsumeTestament(GameState gameState)
        {
            if (gameState == null) return false;
            if (gameState.TestamentOfHarmonyStacks < 1) return false;

            gameState.TestamentOfHarmonyStacks--;
            gameState.MelodiousOwned += 100;
            gameState.HarmoniousOwned += 100;

            return true;
        }

        /// <summary>
        /// Funeral Prayer Forte Resonance: Track clicks and build Prayer stacks (every 10th click)
        /// </summary>
        public static void FuneralPrayer_OnClick(GameState gameState)
        {
            if (gameState == null) return;
            
            gameState.FuneralPrayerClickCounter++;
            if (gameState.FuneralPrayerClickCounter >= 10)
            {
                gameState.FuneralPrayerClickCounter = 0;
                gameState.FuneralPrayerStacks++;
            }
        }

        /// <summary>
        /// Funeral Prayer Forte Resonance: Get empowered click bonus (6x NPS for next 15 clicks if 3+ stacks)
        /// </summary>
        public static double FuneralPrayer_GetEmpoweredClickBonus(GameState gameState)
        {
            if (gameState == null) return 0;
            
            // Check if we have 3+ stacks to consume
            if (gameState.FuneralPrayerStacks >= 3 && gameState.FuneralPrayerEmpoweredClicksRemaining == 0)
            {
                gameState.FuneralPrayerStacks -= 3;
                gameState.FuneralPrayerEmpoweredClicksRemaining = 15;
            }
            
            // If we have empowered clicks remaining, give bonus
            if (gameState.FuneralPrayerEmpoweredClicksRemaining > 0)
            {
                gameState.FuneralPrayerEmpoweredClicksRemaining--;
                return gameState.NotesPerSecond * 6.0; // 6x NPS bonus
            }
            
            return 0;
        }

        /// <summary>
        /// Sakura's Blossom Passive - Ascending Harmony: Every upgrade bought grants +3 to lowest owned upgrade.
        /// </summary>
        public static void SakurasBlossom_OnUpgradePurchase(GameState gameState)
        {
            if (gameState == null) return;

            // Find lowest owned upgrade
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

            // Grant +3 to lowest owned upgrade, simulating 3 purchases
            for (int i = 0; i < 3; i++)
            {
                switch (minIndex)
                {
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

        /// <summary>
        /// Funeral Prayer Passive - Melodic Ascension: Every upgrade bought grants +2 to lowest owned minor score.
        /// </summary>
        public static void FuneralPrayer_OnUpgradePurchase(GameState gameState)
        {
            if (gameState == null) return;

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

            // Grant +2 to lowest minor
            switch (minIndex)
            {
                case 0: 
                    gameState.MoonlightMinorOwned += 2;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 6000; // 3000 * 2
                    break;
                case 1: 
                    gameState.EroicaMinorOwned += 2;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 16000; // 8000 * 2
                    break;
                case 2: 
                    gameState.SwanMinorOwned += 2;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 30000; // 15000 * 2
                    break;
                case 3: 
                    gameState.LaCampanellaMinorOwned += 2;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 70000; // 35000 * 2
                    break;
                case 4: 
                    gameState.EnigmaMinorOwned += 2;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 150000; // 75000 * 2
                    break;
                case 5: 
                    gameState.FateMinorOwned += 2;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 270000; // 135000 * 2
                    break;
                case 6: 
                    gameState.OdeToJoyMinorOwned += 2;
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                        gameState.NotesPerSecond += 510000; // 255000 * 2
                    break;
            }
        }

        // ==================== EROICA MAJOR: TRIUMPH OF HEROES ====================

        /// <summary>
        /// Eroica Major Ability: Removed - no longer has passive major acquisition effect
        /// </summary>
        public static void TriumphOfHeroes_OnMajorAcquisition(GameState gameState)
        {
            // Functionality removed - Eroica Major Ability no longer doubles notes on acquisition
            return;
        }

        // ==================== SWAN LAKE WEAPONS (4-5) ====================

        /// <summary>
        /// Star-Scattered Wings (Swan I): Every 10th click grants +5 Melodious and +5 Harmonious fragments (passive).
        /// When Swan Lake Crescendance is active: whenever you obtain a Revered or Chromatic Feather,
        /// give +2 of each minor component to a random owned minor score.
        /// </summary>
        public static void StarScatteredWings_OnClick(GameState gameState)
        {
            if (gameState == null) return;
            
            gameState.StarScatteredWingsClickCounter++;
            if (gameState.StarScatteredWingsClickCounter >= 10)
            {
                gameState.StarScatteredWingsClickCounter = 0;
                gameState.MelodiousOwned += 5;
                gameState.HarmoniousOwned += 5;
            }
        }
        
        /// <summary>
        /// Star-Scattered Wings active effect: on obtaining Revered or Chromatic feather during Crescendance,
        /// grant +2 of each component to a random owned minor.
        /// </summary>
        public static void StarScatteredWings_OnFeatherObtained(GameState gameState, string featherType)
        {
            if (gameState == null) return;
            if (!gameState.SwanMajorAbility) return; // Requires Crescendance active
            if (featherType != "Revered" && featherType != "Chromatic") return;
            
            // Find all owned minors
            var ownedMinors = new System.Collections.Generic.List<int>();
            if (gameState.MoonlightMinorOwned > 0) ownedMinors.Add(0);
            if (gameState.EroicaMinorOwned > 0) ownedMinors.Add(1);
            if (gameState.SwanMinorOwned > 0) ownedMinors.Add(2);
            if (gameState.LaCampanellaMinorOwned > 0) ownedMinors.Add(3);
            if (gameState.EnigmaMinorOwned > 0) ownedMinors.Add(4);
            if (gameState.FateMinorOwned > 0) ownedMinors.Add(5);
            if (gameState.OdeToJoyMinorOwned > 0) ownedMinors.Add(6);
            
            if (ownedMinors.Count == 0) return;
            
            // Pick random owned minor and grant +2 of each component
            int randomMinor = ownedMinors[_random.Next(ownedMinors.Count)];
            switch (randomMinor)
            {
                case 0: gameState.MoonlightMinorKeys += 2; gameState.MoonlightMinorScales += 2; gameState.MoonlightMinorProgressions += 2; break;
                case 1: gameState.EroicaMinorKeys += 2; gameState.EroicaMinorScales += 2; gameState.EroicaMinorProgressions += 2; break;
                case 2: gameState.SwanLakeMinorKeys += 2; gameState.SwanLakeMinorScales += 2; gameState.SwanLakeMinorProgressions += 2; break;
                case 3: gameState.LaCampanellaMinorKeys += 2; gameState.LaCampanellaMinorScales += 2; gameState.LaCampanellaMinorProgressions += 2; break;
                case 4: gameState.EnigmaMinorKeys += 2; gameState.EnigmaMinorScales += 2; gameState.EnigmaMinorProgressions += 2; break;
                case 5: gameState.FateMinorKeys += 2; gameState.FateMinorScales += 2; gameState.FateMinorProgressions += 2; break;
                case 6: gameState.OdeToJoyMinorKeys += 2; gameState.OdeToJoyMinorScales += 2; gameState.OdeToJoyMinorProgressions += 2; break;
            }
        }

        /// <summary>
        /// Thousand Winged Swan (Swan II): On acquisition of minor/major score or weapon, increase notes by +50% (passive).
        /// When Swan Lake Crescendance is active: whenever you obtain a Polyphonic Feather,
        /// for the next 10 seconds your NPS is added to your NPC at double rate (NPC += NPS * 2).
        /// </summary>
        public static void ThousandWingedSwan_OnScoreAcquisition(GameState gameState)
        {
            if (gameState == null) return;
            double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
            double bonus = currentNotes * 0.50;
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
        }
        
        /// <summary>
        /// Thousand Winged Swan active effect: on obtaining Polyphonic feather during Crescendance,
        /// activate 10-second NPS-to-NPC boost (NPC += NPS * 2).
        /// </summary>
        public static void ThousandWingedSwan_OnPolyphonicFeatherObtained(GameState gameState)
        {
            if (gameState == null) return;
            if (!gameState.SwanMajorAbility) return; // Requires Crescendance active
            
            gameState.ThousandWingedSwanNpsBoostActive = true;
            gameState.ThousandWingedSwanNpsBoostExpiry = DateTime.Now.AddSeconds(10);
        }
        
        /// <summary>
        /// Get Thousand Winged Swan NPC boost value if active.
        /// Returns NPS * 2 to add to NPC, or 0 if boost is not active.
        /// </summary>
        public static double ThousandWingedSwan_GetNpcBoost(GameState gameState)
        {
            if (gameState == null) return 0;
            if (!gameState.ThousandWingedSwanNpsBoostActive) return 0;
            if (DateTime.Now > gameState.ThousandWingedSwanNpsBoostExpiry)
            {
                gameState.ThousandWingedSwanNpsBoostActive = false;
                return 0;
            }
            return gameState.NotesPerSecond * 2.0;
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
        /// Razer of Bell's Chimes (La Campanella II): Passive - Component Echo
        /// On craft of a minor score, refund 2 random components for that minor.
        /// </summary>
        public static void RazerOfBellsChimes_OnMinorCraft(GameState gameState, string scoreName)
        {
            if (gameState == null) return;
            
            // Refund 2 random components for the crafted minor
            for (int i = 0; i < 2; i++)
            {
                int randomComponent = _random.Next(3); // 0=Keys, 1=Scales, 2=Progressions
                
                switch (scoreName.ToLower())
                {
                    case "moonlight":
                        if (randomComponent == 0) gameState.MoonlightMinorKeys++;
                        else if (randomComponent == 1) gameState.MoonlightMinorScales++;
                        else gameState.MoonlightMinorProgressions++;
                        break;
                    case "eroica":
                        if (randomComponent == 0) gameState.EroicaMinorKeys++;
                        else if (randomComponent == 1) gameState.EroicaMinorScales++;
                        else gameState.EroicaMinorProgressions++;
                        break;
                    case "swanlake":
                        if (randomComponent == 0) gameState.SwanLakeMinorKeys++;
                        else if (randomComponent == 1) gameState.SwanLakeMinorScales++;
                        else gameState.SwanLakeMinorProgressions++;
                        break;
                    case "lacampanella":
                        if (randomComponent == 0) gameState.LaCampanellaMinorKeys++;
                        else if (randomComponent == 1) gameState.LaCampanellaMinorScales++;
                        else gameState.LaCampanellaMinorProgressions++;
                        break;
                    case "enigma":
                        if (randomComponent == 0) gameState.EnigmaMinorKeys++;
                        else if (randomComponent == 1) gameState.EnigmaMinorScales++;
                        else gameState.EnigmaMinorProgressions++;
                        break;
                    case "fate":
                        if (randomComponent == 0) gameState.FateMinorKeys++;
                        else if (randomComponent == 1) gameState.FateMinorScales++;
                        else gameState.FateMinorProgressions++;
                        break;
                    case "odetojoy":
                        if (randomComponent == 0) gameState.OdeToJoyMinorKeys++;
                        else if (randomComponent == 1) gameState.OdeToJoyMinorScales++;
                        else gameState.OdeToJoyMinorProgressions++;
                        break;
                }
            }
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
        /// Cosmic Weaver (Fate II): Passive - Harmonic Multiplication
        /// On any minor craft, if you own 1+ of that score's major, grant +3 of that major.
        /// </summary>
        public static void CosmicWeaver_OnMinorCraft(GameState gameState, string scoreName)
        {
            if (gameState == null) return;
            
            switch (scoreName.ToLower())
            {
                case "moonlight sonata":
                case "moonlight":
                    if (gameState.MoonlightMajorOwned > 0)
                        gameState.MoonlightMajorOwned += 3;
                    break;
                case "eroica":
                    if (gameState.EroicaMajorOwned > 0)
                        gameState.EroicaMajorOwned += 3;
                    break;
                case "swanlake":
                    if (gameState.SwanMajorOwned > 0)
                        gameState.SwanMajorOwned += 3;
                    break;
                case "lacampanella":
                    if (gameState.LaCampanellaMajorOwned > 0)
                        gameState.LaCampanellaMajorOwned += 3;
                    break;
                case "enigma":
                    if (gameState.EnigmaMajorOwned > 0)
                        gameState.EnigmaMajorOwned += 3;
                    break;
                case "fate":
                    if (gameState.FateMajorOwned > 0)
                        gameState.FateMajorOwned += 3;
                    break;
                case "odetojoy":
                    if (gameState.OdeToJoyMajorOwned > 0)
                        gameState.OdeToJoyMajorOwned += 3;
                    break;
            }
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

        // ==================== SWAN LAKE CRESCENDANCE: FEATHER COLLECTION ====================
        
        /// <summary>
        /// Swan Lake Crescendance: Check for feather drops on click.
        /// Every 10th click: 5% chance for Revered Feather
        /// Every 15th click: 5% chance for Chromatic Feather
        /// Every 30th click: 5% chance for Polyphonic Feather
        /// Evaluate highest rarity first to avoid multiple simultaneous grants.
        /// </summary>
        public static void SwanLakeCrescendance_OnClick(GameState gameState)
        {
            if (gameState == null) return;
            if (!gameState.SwanMajorAbility) return; // Requires Crescendance active
            
            gameState.SwanLakeClickCounter++;
            
            // Check for Polyphonic first (every 30 clicks, 5% chance)
            if (gameState.SwanLakeClickCounter % 30 == 0)
            {
                if (_random.NextDouble() < 0.05)
                {
                    gameState.PolyphonicFeathers++;
                    // Trigger weapon effects
                    if (gameState.ThousandWingedSwanAbility)
                        ThousandWingedSwan_OnPolyphonicFeatherObtained(gameState);
                    return; // Only one feather per click
                }
            }
            
            // Check for Chromatic (every 15 clicks, 10% chance)
            if (gameState.SwanLakeClickCounter % 15 == 0)
            {
                if (_random.NextDouble() < 0.10)
                {
                    gameState.ChromaticFeathers++;
                    // Trigger weapon effects
                    if (gameState.StarScatteredWingsAbility)
                        StarScatteredWings_OnFeatherObtained(gameState, "Chromatic");
                    return;
                }
            }
            
            // Check for Revered (every 10 clicks, 20% chance)
            if (gameState.SwanLakeClickCounter % 10 == 0)
            {
                if (_random.NextDouble() < 0.20)
                {
                    gameState.ReveredFeathers++;
                    // Trigger weapon effects
                    if (gameState.StarScatteredWingsAbility)
                        StarScatteredWings_OnFeatherObtained(gameState, "Revered");
                }
            }
        }
        
        /// <summary>
        /// Consume Revered Feathers (5 stacks → +20% current notes)
        /// </summary>
        public static void SwanLake_ConsumeReveredFeathers(GameState gameState)
        {
            if (gameState == null) return;
            if (gameState.ReveredFeathers < 5) return;
            
            gameState.ReveredFeathers -= 5;
            double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
            double bonus = currentNotes * 0.20;
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
        }
        
        /// <summary>
        /// Consume Chromatic Feathers (10 stacks → +2 of every minor score)
        /// </summary>
        public static void SwanLake_ConsumeChromaticFeathers(GameState gameState)
        {
            if (gameState == null) return;
            if (gameState.ChromaticFeathers < 10) return;
            
            gameState.ChromaticFeathers -= 10;
            
            // Grant 2 of every minor score the player currently owns
            if (gameState.MoonlightMinorOwned > 0)
            {
                gameState.MoonlightMinorOwned += 2;
                if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    gameState.NotesPerSecond += 6000; // 3000 * 2
            }
            if (gameState.EroicaMinorOwned > 0)
            {
                gameState.EroicaMinorOwned += 2;
                if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    gameState.NotesPerSecond += 16000; // 8000 * 2
            }
            if (gameState.SwanMinorOwned > 0)
            {
                gameState.SwanMinorOwned += 2;
                if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    gameState.NotesPerSecond += 30000; // 15000 * 2
            }
            if (gameState.LaCampanellaMinorOwned > 0)
            {
                gameState.LaCampanellaMinorOwned += 2;
                if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    gameState.NotesPerSecond += 70000; // 35000 * 2
            }
            if (gameState.EnigmaMinorOwned > 0)
            {
                gameState.EnigmaMinorOwned += 2;
                if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    gameState.NotesPerSecond += 150000; // 75000 * 2
            }
            if (gameState.FateMinorOwned > 0)
            {
                gameState.FateMinorOwned += 2;
                if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    gameState.NotesPerSecond += 270000; // 135000 * 2
            }
            if (gameState.OdeToJoyMinorOwned > 0)
            {
                gameState.OdeToJoyMinorOwned += 2;
                if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    gameState.NotesPerSecond += 510000; // 255000 * 2
            }
        }
        
        /// <summary>
        /// Consume Polyphonic Feather (1 stack → +250 entropic melodies + 75% current notes)
        /// </summary>
        public static void SwanLake_ConsumePolyphonicFeather(GameState gameState)
        {
            if (gameState == null) return;
            if (gameState.PolyphonicFeathers < 1) return;
            
            gameState.PolyphonicFeathers--;
            gameState.EntropicMelodies += 250;
            
            double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
            double bonus = currentNotes * 0.75;
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
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
                case 0: // Red - 5× NPS as instant notes
                    gameState.Notes += gameState.NotesPerSecond * 5;
                    break;
                case 1: // Blue - +10% NPS boost (additive)
                    if (!gameState.NpsFrozen || DateTime.Now > gameState.NpsFreezeExpiry)
                    {
                        gameState.NotesPerSecond += gameState.NotesPerSecond * 0.10;
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

        #region Swan Lake Duet - Feather Cascade

        /// <summary>
        /// Swan Lake Duet: For the next 10 clicks, each click grants 1 of each feather type.
        /// Every 5 clicks grants +25% current notes bonus.
        /// Duration: 10 seconds, Cooldown: 5 minutes.
        /// </summary>
        public static void SwanLakeDuet_OnClick(GameState gameState)
        {
            if (gameState == null) return;
            if (!gameState.SwanLakeDuetActive) return;
            if (DateTime.Now > gameState.SwanLakeDuetExpiry) return;
            
            // Increment click counter for this duet activation
            gameState.SwanLakeDuetClickCounter++;
            
            // Grant 1 of each feather type per click (up to 10 clicks)
            if (gameState.SwanLakeDuetClickCounter <= 10)
            {
                gameState.ReveredFeathers++;
                gameState.ChromaticFeathers++;
                gameState.PolyphonicFeathers++;
                
                // Every 5 clicks (5th and 10th), grant +25% current notes
                if (gameState.SwanLakeDuetClickCounter % 5 == 0)
                {
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    double bonus = currentNotes * 0.25;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
                }
                
                // Auto-deactivate after 10 clicks
                if (gameState.SwanLakeDuetClickCounter >= 10)
                {
                    gameState.SwanLakeDuetActive = false;
                    gameState.SwanLakeDuetClickCounter = 0;
                    gameState.SwanLakeDuetCooldownExpiry = DateTime.Now.AddMinutes(5);
                }
            }
        }
        
        /// <summary>
        /// (Legacy) Queue an action to be reflected after 3 seconds during Mirror Lake duet
        /// Kept for backward compatibility but not used in new Swan Lake Duet.
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
                SakurasBlossom_OnMajorSheetAcquired(gameState, scoreName);
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
                // Reward: +5 Petals of Harmony
                gameState.PetalsOfHarmony += 5;
            }
            else if (gameState.CrescendoNotesPlaced == 12 && !gameState.Crescendo12Claimed)
            {
                gameState.Crescendo12Claimed = true;
                // Reward: +5 Petals of Melody
                gameState.PetalsOfMelody += 5;
            }
            else if (gameState.CrescendoNotesPlaced == 16 && !gameState.Crescendo16Claimed)
            {
                gameState.Crescendo16Claimed = true;
                // Reward: +1 Ode to Life
                gameState.OdeToLifeStacks++;
                
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
        
        #region Moonlight Sonata Crescendance: Eclipse of the Nocturne
        
        /// <summary>
        /// Moonlight Crescendance: Track clicks at night, grant Moonbeam Resonance every 20th click
        /// Auto-consume at 8 stacks for +100% notes and 1 Harmonizing Moonlight stack
        /// </summary>
        public static void MoonlightCrescendance_OnClick(GameState gameState, MainWindow mainWindow)
        {
            if (!IsNighttime()) return;
            
            gameState.MoonlightCrescendanceClickCounter++;
            
            if (gameState.MoonlightCrescendanceClickCounter >= 20)
            {
                gameState.MoonlightCrescendanceClickCounter = 0;
                gameState.MoonbeamResonanceStacks++;
                
                // Incisor of Moonlight crescendance ability: +10% notes per stack gained
                if (gameState.IncisorOfMoonlightAbility)
                {
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    double bonus = currentNotes * 0.10;
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
                }
                
                // Auto-consume at 8 stacks
                if (gameState.MoonbeamResonanceStacks >= 8)
                {
                    gameState.MoonbeamResonanceStacks -= 8;
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    double bonus = currentNotes * 1.00; // +100% (doubled notes)
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
                    gameState.HarmonizingMoonlightStacks++;
                }
            }
        }
        
        /// <summary>
        /// Eulogy of the Moon crescendance ability: Consume 1 Harmonizing Moonlight for 3 Moonbeam + components
        /// </summary>
        public static void EulogyOfTheMoon_ConsumeHarmonizingMoonlight(GameState gameState)
        {
            if (gameState.HarmonizingMoonlightStacks <= 0) return;
            
            gameState.HarmonizingMoonlightStacks--;
            gameState.MoonbeamResonanceStacks += 3;
            
            // Check if Moonbeam stacks overflow (>= 8), convert to Harmonizing
            while (gameState.MoonbeamResonanceStacks >= 8)
            {
                gameState.MoonbeamResonanceStacks -= 8;
                gameState.HarmonizingMoonlightStacks++;
                
                // Also grant the +100% notes bonus
                double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                double bonus = currentNotes * 1.00;
                MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
            }
            
            // Give +1 of each component to lowest owned minor
            int lowestMinorIndex = GetLowestOwnedMinorIndex(gameState);
            AddComponentsToMinor(gameState, lowestMinorIndex, 1);
        }
        
        #endregion
        
        #region La Campanella Crescendance: Grandiose Bell
        
        /// <summary>
        /// La Campanella Crescendance: Track clicks toward bell crack stages
        /// Stages: Crescending(20), Radiant(40), Harmonizing(60)
        /// </summary>
        public static void LaCampanellaCrescendance_OnClick(GameState gameState, MainWindow mainWindow)
        {
            gameState.GrandioseBellClickCounter++;
            
            // Update crack stage
            if (gameState.GrandioseBellClickCounter >= 60)
                gameState.GrandioseBellStage = 3; // Harmonizing
            else if (gameState.GrandioseBellClickCounter >= 40)
                gameState.GrandioseBellStage = 2; // Radiant
            else if (gameState.GrandioseBellClickCounter >= 20)
                gameState.GrandioseBellStage = 1; // Crescending
            
            bool bellCracked = (gameState.GrandioseBellClickCounter == 20 || gameState.GrandioseBellClickCounter == 40 || gameState.GrandioseBellClickCounter == 60);
            
            // Major Ability: Bell's Fortune - +10% notes on crack (if La Campanella Major owned)
            if (gameState.LaCampanellaMajorOwned > 0 && bellCracked)
            {
                double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                double bonus = currentNotes * 0.10;
                MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
            }
            
            // Symphony of Bells: +75% notes on crack (stacks with major ability)
            if (gameState.SymphonyOfBellsAbility && bellCracked)
            {
                double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                double bonus = currentNotes * 0.75;
                MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
            }
            
            // Handle entropic crit clicks from Radiant mend - each grants 3 Deafening Chime stacks
            if (gameState.LaCampanellaEntropicCritClicks > 0)
            {
                gameState.LaCampanellaEntropicCritClicks--;
                gameState.DeafeningChimeStacks += 3;
                gameState.DeafeningChimeExpiry = DateTime.Now.AddMinutes(2); // Reset expiry on each crit
                // Note: The entropic crit effect would be applied in the main click handler
            }
        }
        
        /// <summary>
        /// Mend the Grandiose Bell at current crack stage
        /// </summary>
        public static void LaCampanella_MendBell(GameState gameState, MainWindow mainWindow)
        {
            if (gameState.GrandioseBellStage == 0) return; // Nothing to mend
            
            if (gameState.GrandioseBellStage == 1) // Crescending: +2 random owned minors
            {
                for (int i = 0; i < 2; i++)
                {
                    int randomMinor = _random.Next(7);
                    switch (randomMinor)
                    {
                        case 0: if (gameState.MoonlightMinorOwned > 0) gameState.MoonlightMinorOwned++; break;
                        case 1: if (gameState.EroicaMinorOwned > 0) gameState.EroicaMinorOwned++; break;
                        case 2: if (gameState.SwanMinorOwned > 0) gameState.SwanMinorOwned++; break;
                        case 3: if (gameState.LaCampanellaMinorOwned > 0) gameState.LaCampanellaMinorOwned++; break;
                        case 4: if (gameState.EnigmaMinorOwned > 0) gameState.EnigmaMinorOwned++; break;
                        case 5: if (gameState.FateMinorOwned > 0) gameState.FateMinorOwned++; break;
                        case 6: if (gameState.OdeToJoyMinorOwned > 0) gameState.OdeToJoyMinorOwned++; break;
                    }
                }
            }
            else if (gameState.GrandioseBellStage == 2) // Radiant: +5 Deafening Chime stacks (2-minute expiry)
            {
                gameState.DeafeningChimeStacks += 5;
                gameState.DeafeningChimeExpiry = DateTime.Now.AddMinutes(2);
            }
            else if (gameState.GrandioseBellStage == 3) // Harmonizing: Consume Deafening Chime for doubling
            {
                if (gameState.DeafeningChimeStacks > 0)
                {
                    int stacksToConsume = Math.Min(gameState.DeafeningChimeStacks, 6); // Cap at 6 for optimal 64× multiplier
                    double multiplier = Math.Pow(2, stacksToConsume) - 1; // -1 because we're adding, not setting
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, currentNotes * multiplier);
                    gameState.DeafeningChimeStacks -= stacksToConsume;
                }
            }
            
            // Razer of Bells Chimes: Grant entropic melodies = click count × 2 (max 250)
            if (gameState.RazerOfBellsChimesAbility)
            {
                int entropicGrant = Math.Min(gameState.GrandioseBellClickCounter * 2, 250); // Double click count, cap at 250
                gameState.EntropicMelodies += entropicGrant;
            }
            
            // Reset bell
            gameState.GrandioseBellClickCounter = 0;
            gameState.GrandioseBellStage = 0;
        }
        
        #endregion
        
        #region Enigma Crescendance: Resonate Mystery
        
        /// <summary>
        /// Enigma Crescendance: Grant Resonate Mystery stacks every 10th/15th click
        /// Creator of Mystery: Every 3rd click ±25% notes
        /// </summary>
        public static void EnigmaCrescendance_OnClick(GameState gameState, MainWindow mainWindow)
        {
            gameState.EnigmaClickCounter++;
            
            // Every 10th click grants 1 stack
            if (gameState.EnigmaClickCounter % 10 == 0)
            {
                gameState.ResonateMysteryStacks++;
            }
            
            // Every 15th click grants additional stack when Creator of Mystery equipped
            if (gameState.CreatorOfMysteryAbility && gameState.EnigmaClickCounter % 15 == 0)
            {
                gameState.ResonateMysteryStacks++;
            }
            
            // Creator of Mystery passive: every 3rd click ±25% notes
            if (gameState.CreatorOfMysteryAbility)
            {
                gameState.EnigmaPassiveClickCounter++;
                if (gameState.EnigmaPassiveClickCounter >= 3)
                {
                    gameState.EnigmaPassiveClickCounter = 0;
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    bool positive = _random.NextDouble() < 0.6; // 60% chance positive
                    double change = currentNotes * 0.25;
                    if (positive)
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, change);
                    else
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -change);
                }
            }
        }
        
        /// <summary>
        /// Consume a single Resonate Mystery stack for chosen effect
        /// </summary>
        public static void Enigma_ConsumeStack(GameState gameState, string effectChoice)
        {
            if (gameState.ResonateMysteryStacks <= 0) return;
            
            gameState.ResonateMysteryStacks--;
            
            switch (effectChoice)
            {
                case "Notes":
                    double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                    MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, currentNotes * 0.50);
                    break;
                case "Entropic":
                    gameState.EntropicMelodies += 50;
                    break;
                case "Minor":
                    int randomMinor = _random.Next(7);
                    switch (randomMinor)
                    {
                        case 0: if (gameState.MoonlightMinorOwned > 0) gameState.MoonlightMinorOwned++; break;
                        case 1: if (gameState.EroicaMinorOwned > 0) gameState.EroicaMinorOwned++; break;
                        case 2: if (gameState.SwanMinorOwned > 0) gameState.SwanMinorOwned++; break;
                        case 3: if (gameState.LaCampanellaMinorOwned > 0) gameState.LaCampanellaMinorOwned++; break;
                        case 4: if (gameState.EnigmaMinorOwned > 0) gameState.EnigmaMinorOwned++; break;
                        case 5: if (gameState.FateMinorOwned > 0) gameState.FateMinorOwned++; break;
                        case 6: if (gameState.OdeToJoyMinorOwned > 0) gameState.OdeToJoyMinorOwned++; break;
                    }
                    break;
            }
            
            // Truthseeker: Grant 1 random minor per 2 stacks consumed
            if (gameState.TruthseekerAbility)
            {
                int randomMinor2 = _random.Next(7);
                switch (randomMinor2)
                {
                    case 0: if (gameState.MoonlightMinorOwned > 0) gameState.MoonlightMinorOwned++; break;
                    case 1: if (gameState.EroicaMinorOwned > 0) gameState.EroicaMinorOwned++; break;
                    case 2: if (gameState.SwanMinorOwned > 0) gameState.SwanMinorOwned++; break;
                    case 3: if (gameState.LaCampanellaMinorOwned > 0) gameState.LaCampanellaMinorOwned++; break;
                    case 4: if (gameState.EnigmaMinorOwned > 0) gameState.EnigmaMinorOwned++; break;
                    case 5: if (gameState.FateMinorOwned > 0) gameState.FateMinorOwned++; break;
                    case 6: if (gameState.OdeToJoyMinorOwned > 0) gameState.OdeToJoyMinorOwned++; break;
                }
            }
        }
        
        /// <summary>
        /// Consume all Resonate Mystery stacks (10+) for bulk bonus
        /// </summary>
        public static void Enigma_ConsumeAllStacks(GameState gameState)
        {
            if (gameState.ResonateMysteryStacks < 10) return;
            
            int stackCount = gameState.ResonateMysteryStacks;
            double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
            double bonus = currentNotes * (0.25 * stackCount); // Reduced from 75% to 25% per stack
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
            
            // Truthseeker: Grant 1 minor per 2 stacks consumed
            if (gameState.TruthseekerAbility)
            {
                int minorsToGrant = stackCount / 2; // Integer division - 1 minor per 2 stacks
                for (int i = 0; i < minorsToGrant; i++)
                {
                    int randomMinor = _random.Next(7);
                    switch (randomMinor)
                    {
                        case 0: if (gameState.MoonlightMinorOwned > 0) gameState.MoonlightMinorOwned++; break;
                        case 1: if (gameState.EroicaMinorOwned > 0) gameState.EroicaMinorOwned++; break;
                        case 2: if (gameState.SwanMinorOwned > 0) gameState.SwanMinorOwned++; break;
                        case 3: if (gameState.LaCampanellaMinorOwned > 0) gameState.LaCampanellaMinorOwned++; break;
                        case 4: if (gameState.EnigmaMinorOwned > 0) gameState.EnigmaMinorOwned++; break;
                        case 5: if (gameState.FateMinorOwned > 0) gameState.FateMinorOwned++; break;
                        case 6: if (gameState.OdeToJoyMinorOwned > 0) gameState.OdeToJoyMinorOwned++; break;
                    }
                }
            }
            
            gameState.ResonateMysteryStacks = 0;
        }
        
        /// <summary>
        /// Truthseeker Passive: Revelation Burst - On upgrade purchase, grant 5 Resonant Mystery stacks
        /// </summary>
        public static void Truthseeker_OnUpgradePurchase(GameState gameState)
        {
            if (gameState == null) return;
            gameState.ResonateMysteryStacks += 5;
        }
        
        #endregion
        
        #region Fate Crescendance: Cosmic Modulation
        
        /// <summary>
        /// Fate Crescendance: Every 8th click grants stack + 10% notes
        /// Tier effects: 1-4 entropic crits, 5-9 double NPS, 10+ guaranteed crits
        /// </summary>
        public static void FateCrescendance_OnClick(GameState gameState, MainWindow mainWindow)
        {
            gameState.FateClickCounter++;
            
            if (gameState.FateClickCounter >= 8)
            {
                gameState.FateClickCounter = 0;
                gameState.CosmicModulationStacks++;
                
                // Immediate +10% notes
                double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
                MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, currentNotes * 0.10);
                
                // Astral Chainripper: Every 5 Cosmic stacks grants 1 Symphony stack
                if (gameState.AstralChainripperAbility && gameState.CosmicModulationStacks % 5 == 0)
                {
                    gameState.SymphonyOfTheStarsStacks++;
                }
            }
        }
        
        /// <summary>
        /// Consume Cosmic Modulation stacks for Entropic Melodies (15 per stack)
        /// </summary>
        public static void Fate_ConsumeStacksForMelodies(GameState gameState, int stackCount)
        {
            if (gameState.CosmicModulationStacks < stackCount) return;
            
            gameState.CosmicModulationStacks -= stackCount;
            gameState.EntropicMelodies += stackCount * 15;
        }
        
        /// <summary>
        /// Consume 1 Symphony of the Stars stack to increase lowest minor by +3
        /// Cosmic Weaver: Also grants 5 entropic crit clicks per stack
        /// </summary>
        public static void Fate_ConsumeSymphonyStack(GameState gameState)
        {
            if (gameState.SymphonyOfTheStarsStacks <= 0) return;
            
            gameState.SymphonyOfTheStarsStacks--;
            
            // Increase lowest owned minor by +3
            int lowestMinorIndex = GetLowestOwnedMinorIndex(gameState);
            switch (lowestMinorIndex)
            {
                case 0: gameState.MoonlightMinorOwned += 3; break;
                case 1: gameState.EroicaMinorOwned += 3; break;
                case 2: gameState.SwanMinorOwned += 3; break;
                case 3: gameState.LaCampanellaMinorOwned += 3; break;
                case 4: gameState.EnigmaMinorOwned += 3; break;
                case 5: gameState.FateMinorOwned += 3; break;
                case 6: gameState.OdeToJoyMinorOwned += 3; break;
            }
            
            // Cosmic Weaver: Grant 5 entropic crit clicks
            if (gameState.CosmicWeaverAbility)
            {
                gameState.LaCampanellaEntropicCritClicks += 5; // Reusing this counter for entropic crits
            }
        }
        
        /// <summary>
        /// Helper: Get index of lowest owned minor score
        /// </summary>
        private static int GetLowestOwnedMinorIndex(GameState gameState)
        {
            int[] minorCounts = {
                gameState.MoonlightMinorOwned,
                gameState.EroicaMinorOwned,
                gameState.SwanMinorOwned,
                gameState.LaCampanellaMinorOwned,
                gameState.EnigmaMinorOwned,
                gameState.FateMinorOwned,
                gameState.OdeToJoyMinorOwned
            };
            
            int lowestIndex = 0;
            int lowestCount = minorCounts[0];
            
            for (int i = 1; i < minorCounts.Length; i++)
            {
                if (minorCounts[i] < lowestCount && minorCounts[i] > 0)
                {
                    lowestCount = minorCounts[i];
                    lowestIndex = i;
                }
            }
            
            return lowestIndex;
        }
        
        /// <summary>
        /// Helper: Add components to a specific minor score
        /// </summary>
        private static void AddComponentsToMinor(GameState gameState, int minorIndex, int amount)
        {
            switch (minorIndex)
            {
                case 0: // Moonlight
                    gameState.MoonlightMinorKeys += amount;
                    gameState.MoonlightMinorScales += amount;
                    gameState.MoonlightMinorProgressions += amount;
                    break;
                case 1: // Eroica
                    gameState.EroicaMinorKeys += amount;
                    gameState.EroicaMinorScales += amount;
                    gameState.EroicaMinorProgressions += amount;
                    break;
                case 2: // Swan
                    gameState.SwanLakeMinorKeys += amount;
                    gameState.SwanLakeMinorScales += amount;
                    gameState.SwanLakeMinorProgressions += amount;
                    break;
                case 3: // La Campanella
                    gameState.LaCampanellaMinorKeys += amount;
                    gameState.LaCampanellaMinorScales += amount;
                    gameState.LaCampanellaMinorProgressions += amount;
                    break;
                case 4: // Enigma
                    gameState.EnigmaMinorKeys += amount;
                    gameState.EnigmaMinorScales += amount;
                    gameState.EnigmaMinorProgressions += amount;
                    break;
                case 5: // Fate
                    gameState.FateMinorKeys += amount;
                    gameState.FateMinorScales += amount;
                    gameState.FateMinorProgressions += amount;
                    break;
                case 6: // Ode to Joy
                    gameState.OdeToJoyMinorKeys += amount;
                    gameState.OdeToJoyMinorScales += amount;
                    gameState.OdeToJoyMinorProgressions += amount;
                    break;
            }
        }
        
        #endregion
        
        #region Ode to Joy Crescendance: Petals of Harmony and Melody
        
        /// <summary>
        /// Ode to Joy Crescendance: Click tracking for weapon passives
        /// </summary>
        public static void OdeToJoyCrescendance_OnClick(GameState gameState, MainWindow mainWindow)
        {
            // Joyful Catharsis passive: Every 50th click
            if (gameState.JoyfulCatharsisAbility)
            {
                gameState.JoyfulCatharsisClickCounter++;
                if (gameState.JoyfulCatharsisClickCounter >= 50)
                {
                    gameState.JoyfulCatharsisClickCounter = 0;
                    // Grant entropic melodies based on critical notes * 3
                    // This will be calculated in the main click handler based on actual crits
                }
            }
            
            // Ode to Creation passive: Every 20th click for random petal
            if (gameState.OdeToCreationAbility)
            {
                gameState.OdeToCreationClickCounter++;
                int threshold = gameState.OdeToCreationDoubleActive ? 10 : 20; // Doubled = half the clicks
                
                if (gameState.OdeToCreationClickCounter >= threshold)
                {
                    gameState.OdeToCreationClickCounter = 0;
                    
                    // Grant random petal
                    if (_random.NextDouble() < 0.5)
                    {
                        gameState.PetalsOfHarmony++;
                    }
                    else
                    {
                        gameState.PetalsOfMelody++;
                    }
                    
                    // Joyful Catharsis: Double NPS for 5s on petal gain
                    if (gameState.JoyfulCatharsisAbility)
                    {
                        gameState.JoyfulCatharsisNpsBoostExpiry = DateTime.Now.AddSeconds(5);
                    }
                }
            }
        }
        
        /// <summary>
        /// Grant petal on minor craft
        /// </summary>
        public static void OdeToJoy_OnMinorCraft(GameState gameState)
        {
            gameState.PetalsOfHarmony++;
            
            // Joyful Catharsis: Double NPS for 5s
            if (gameState.JoyfulCatharsisAbility)
            {
                gameState.JoyfulCatharsisNpsBoostExpiry = DateTime.Now.AddSeconds(5);
            }
        }
        
        /// <summary>
        /// Grant petal on major craft
        /// </summary>
        public static void OdeToJoy_OnMajorCraft(GameState gameState)
        {
            gameState.PetalsOfMelody++;
            
            // Joyful Catharsis: Double NPS for 5s
            if (gameState.JoyfulCatharsisAbility)
            {
                gameState.JoyfulCatharsisNpsBoostExpiry = DateTime.Now.AddSeconds(5);
            }
        }
        
        /// <summary>
        /// Consume Petal of Harmony for +250 Entropic Melodies
        /// </summary>
        public static void OdeToJoy_ConsumePetalOfHarmony(GameState gameState)
        {
            if (gameState.PetalsOfHarmony <= 0) return;
            
            gameState.PetalsOfHarmony--;
            gameState.EntropicMelodies += 250;
        }
        
        /// <summary>
        /// Consume Petal of Melody for +10s full entropic crits (stackable)
        /// </summary>
        public static void OdeToJoy_ConsumePetalOfMelody(GameState gameState)
        {
            if (gameState.PetalsOfMelody <= 0) return;
            
            gameState.PetalsOfMelody--;
            gameState.EntropicCritExpiry = gameState.EntropicCritExpiry > DateTime.Now 
                ? gameState.EntropicCritExpiry.AddSeconds(10)
                : DateTime.Now.AddSeconds(10);
        }
        
        /// <summary>
        /// Combine petals and entropic to create Ode to Life
        /// </summary>
        public static void OdeToJoy_CombineForOdeToLife(GameState gameState)
        {
            if (gameState.PetalsOfHarmony < 1 || gameState.PetalsOfMelody < 1 || gameState.EntropicMelodies < 50)
                return;
            
            gameState.PetalsOfHarmony--;
            gameState.PetalsOfMelody--;
            gameState.EntropicMelodies -= 50;
            gameState.OdeToLifeStacks++;
        }
        
        /// <summary>
        /// Consume Ode to Life to double all owned minors
        /// </summary>
        public static void OdeToJoy_ConsumeOdeToLife(GameState gameState)
        {
            if (gameState.OdeToLifeStacks <= 0) return;
            
            gameState.OdeToLifeStacks--;
            
            // Double all owned minors
            gameState.MoonlightMinorOwned *= 2;
            gameState.EroicaMinorOwned *= 2;
            gameState.SwanMinorOwned *= 2;
            gameState.LaCampanellaMinorOwned *= 2;
            gameState.EnigmaMinorOwned *= 2;
            gameState.FateMinorOwned *= 2;
            gameState.OdeToJoyMinorOwned *= 2;
            
            // Ode to Creation: Double passive effect for 25s
            if (gameState.OdeToCreationAbility)
            {
                gameState.OdeToCreationDoubleActive = true;
                gameState.OdeToCreationDoubleExpiry = DateTime.Now.AddSeconds(25);
            }
        }
        
        #endregion
        
        #region Dies Irae Crescendance: Burning Hatred and Discordant Malice
        
        /// <summary>
        /// Dies Irae Crescendance: Every click gives Burning Hatred (up to 50), then Discordant Malice
        /// </summary>
        public static void DiesIraeCrescendance_OnClick(GameState gameState, MainWindow mainWindow)
        {
            if (gameState.BurningHatredStacks < 50)
            {
                gameState.BurningHatredStacks++;
            }
            else
            {
                gameState.DiscordantMaliceStacks++;
            }
            
            // Hell's Wrath passive: Every 50th click
            if (gameState.HellsWrathAbility)
            {
                gameState.HellsWrathClickCounter++;
                if (gameState.HellsWrathClickCounter >= 50)
                {
                    gameState.HellsWrathClickCounter = 0;
                    
                    // Add +2 to 2 lowest owned minors
                    int[] minorCounts = {
                        gameState.MoonlightMinorOwned,
                        gameState.EroicaMinorOwned,
                        gameState.SwanMinorOwned,
                        gameState.LaCampanellaMinorOwned,
                        gameState.EnigmaMinorOwned,
                        gameState.FateMinorOwned,
                        gameState.OdeToJoyMinorOwned
                    };
                    
                    // Find two lowest
                    int lowest1 = -1, lowest2 = -1;
                    int lowestCount1 = int.MaxValue, lowestCount2 = int.MaxValue;
                    
                    for (int i = 0; i < minorCounts.Length; i++)
                    {
                        if (minorCounts[i] > 0 && minorCounts[i] < lowestCount1)
                        {
                            lowest2 = lowest1;
                            lowestCount2 = lowestCount1;
                            lowest1 = i;
                            lowestCount1 = minorCounts[i];
                        }
                        else if (minorCounts[i] > 0 && minorCounts[i] < lowestCount2 && i != lowest1)
                        {
                            lowest2 = i;
                            lowestCount2 = minorCounts[i];
                        }
                    }
                    
                    if (lowest1 >= 0)
                    {
                        switch (lowest1)
                        {
                            case 0: gameState.MoonlightMinorOwned += 2; break;
                            case 1: gameState.EroicaMinorOwned += 2; break;
                            case 2: gameState.SwanMinorOwned += 2; break;
                            case 3: gameState.LaCampanellaMinorOwned += 2; break;
                            case 4: gameState.EnigmaMinorOwned += 2; break;
                            case 5: gameState.FateMinorOwned += 2; break;
                            case 6: gameState.OdeToJoyMinorOwned += 2; break;
                        }
                    }
                    
                    if (lowest2 >= 0)
                    {
                        switch (lowest2)
                        {
                            case 0: gameState.MoonlightMinorOwned += 2; break;
                            case 1: gameState.EroicaMinorOwned += 2; break;
                            case 2: gameState.SwanMinorOwned += 2; break;
                            case 3: gameState.LaCampanellaMinorOwned += 2; break;
                            case 4: gameState.EnigmaMinorOwned += 2; break;
                            case 5: gameState.FateMinorOwned += 2; break;
                            case 6: gameState.OdeToJoyMinorOwned += 2; break;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Consume 5 Burning Hatred for 1 Dissonant Oblivion
        /// </summary>
        public static void DiesIrae_ConsumeBurningHatred(GameState gameState)
        {
            if (gameState.BurningHatredStacks < 5) return;
            
            gameState.BurningHatredStacks -= 5;
            gameState.DissonantOblivionStacks++;
        }
        
        /// <summary>
        /// Consume Discordant Malice for entropic melodies
        /// </summary>
        public static void DiesIrae_ConsumeDiscordantMalice(GameState gameState)
        {
            if (gameState.DiscordantMaliceStacks <= 0) return;
            
            gameState.DiscordantMaliceStacks--;
            gameState.EntropicMelodies += gameState.BurningHatredStacks; // X = current Burning Hatred
        }
        
        /// <summary>
        /// Consume Dissonant Oblivion for Symphony of Hell's Retribution (20 clicks)
        /// </summary>
        public static void DiesIrae_ConsumeDissonantOblivion(GameState gameState)
        {
            if (gameState.DissonantOblivionStacks <= 0) return;
            
            gameState.DissonantOblivionStacks--;
            gameState.SymphonyOfHellClicks = 20;
        }
        
        /// <summary>
        /// Consume Wrathful Seal for Seal-breaking Melody (5 clicks with double power)
        /// </summary>
        public static void DiesIrae_ConsumeWrathfulSeal(GameState gameState)
        {
            if (gameState.WrathfulSealStacks <= 0) return;
            
            gameState.WrathfulSealStacks--;
            gameState.SealBreakingMelodyClicks = 5;
        }
        
        #endregion
    }
}

