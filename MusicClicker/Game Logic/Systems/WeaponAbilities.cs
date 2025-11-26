using System;
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
        // Keeping only the Incisor upgrade for focused development.
        // All other weapon-specific helpers were removed to simplify early development.

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
                            gameState.NotesPerSecond += effect;
                            gameState.ChordOwned++;
                        }
                        break;
                    case 1: // Scale
                        {
                            double effect = gameState.ScaleBaseNpsEffect * Math.Pow(gameState.ScaleNpsGrowth, gameState.ScaleOwned);
                            gameState.NotesPerSecond += effect;
                            gameState.ScaleOwned++;
                        }
                        break;
                    case 2: // Orchestra
                        {
                            double effect = gameState.OrchestraBaseNpsEffect * Math.Pow(gameState.OrchestraNpsGrowth, gameState.OrchestraOwned);
                            gameState.NotesPerSecond += effect;
                            gameState.OrchestraOwned++;
                        }
                        break;
                    case 3: // Symphony
                        {
                            double effect = gameState.SymphonyBaseNpsEffect * Math.Pow(gameState.SymphonyNpsGrowth, gameState.SymphonyOwned);
                            gameState.NotesPerSecond += effect;
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
                case "moonlight":
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

            // Divide current notes by 2
            double currentNotes = MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes);
            MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -currentNotes / 2);

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
                case 0: gameState.MoonlightMinorOwned += 3; break;
                case 1: gameState.EroicaMinorOwned += 3; break;
                case 2: gameState.SwanMinorOwned += 3; break;
                case 3: gameState.LaCampanellaMinorOwned += 3; break;
                case 4: gameState.EnigmaMinorOwned += 3; break;
                case 5: gameState.FateMinorOwned += 3; break;
                case 6: gameState.OdeToJoyMinorOwned += 3; break;
            }
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
                case "moonlight": gameState.MoonlightMinorOwned++; break;
                case "eroica": gameState.EroicaMinorOwned++; break;
                case "swanlake": gameState.SwanMinorOwned++; break;
                case "lacampanella": gameState.LaCampanellaMinorOwned++; break;
                case "enigma": gameState.EnigmaMinorOwned++; break;
                case "fate": gameState.FateMinorOwned++; break;
                case "odetojoy": gameState.OdeToJoyMinorOwned++; break;
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
        /// Moonlight Duet: Every 12th click increases all upgrade values by 1.
        /// Needs a click counter in GameState.
        /// </summary>
        public static void MoonlightDuet_OnClick(GameState gameState)
        {
            if (gameState == null) return;

            gameState.MoonlightDuetClickCounter++;
            if (gameState.MoonlightDuetClickCounter >= 12)
            {
                gameState.MoonlightDuetClickCounter = 0;
                
                // Increment owned counts
                gameState.ChordOwned++;
                gameState.ScaleOwned++;
                gameState.OrchestraOwned++;
                gameState.SymphonyOwned++;
                gameState.AriaOwned++;
                gameState.RequiemOwned++;
                gameState.OpusOwned++;
                gameState.MagnumOpusOwned++;

                // Apply the NPS effects for each NPS upgrade
                double chordEffect = gameState.ChordBaseNpsEffect * Math.Pow(gameState.ChordNpsGrowth, gameState.ChordOwned - 1);
                gameState.NotesPerSecond += chordEffect;

                double scaleEffect = gameState.ScaleBaseNpsEffect * Math.Pow(gameState.ScaleNpsGrowth, gameState.ScaleOwned - 1);
                gameState.NotesPerSecond += scaleEffect;

                double orchestraEffect = gameState.OrchestraBaseNpsEffect * Math.Pow(gameState.OrchestraNpsGrowth, gameState.OrchestraOwned - 1);
                gameState.NotesPerSecond += orchestraEffect;

                double symphonyEffect = gameState.SymphonyBaseNpsEffect * Math.Pow(gameState.SymphonyNpsGrowth, gameState.SymphonyOwned - 1);
                gameState.NotesPerSecond += symphonyEffect;

                // Apply the NPC effects for each click upgrade
                double ariaEffect = gameState.AriaBaseClickEffect * Math.Pow(gameState.AriaClickGrowth, gameState.AriaOwned - 1);
                gameState.NotesPerClick += ariaEffect;

                double requiemEffect = gameState.RequiemBaseClickEffect * Math.Pow(gameState.RequiemClickGrowth, gameState.RequiemOwned - 1);
                gameState.NotesPerClick += requiemEffect;

                double opusEffect = gameState.OpusBaseClickEffect * Math.Pow(gameState.OpusClickGrowth, gameState.OpusOwned - 1);
                gameState.NotesPerClick += opusEffect;

                double magnumOpusEffect = gameState.MagnumOpusBaseClickEffect * Math.Pow(gameState.MagnumOpusClickGrowth, gameState.MagnumOpusOwned - 1);
                gameState.NotesPerClick += magnumOpusEffect;
            }
        }

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

        /// <summary>
        /// Swan Lake Duet: If you own 50+ Melodious and 100+ Harmonious fragments,
        /// NPS is doubled. Returns multiplier to apply.
        /// </summary>
        public static double SwanLakeDuet_GetNpsMultiplier(GameState gameState)
        {
            if (gameState == null) return 1.0;

            if (gameState.MelodiousOwned >= 50 && gameState.HarmoniousOwned >= 100)
            {
                return 2.0;
            }
            return 1.0;
        }

        /// <summary>
        /// La Campanella Duet: On craft of a Major score, gives 5 of its corresponding minor score.
        /// </summary>
        public static void LaCampanellaDuet_OnMajorCraft(GameState gameState, string scoreName)
        {
            if (gameState == null) return;

            switch (scoreName.ToLower())
            {
                case "moonlight": gameState.MoonlightMinorOwned += 5; break;
                case "eroica": gameState.EroicaMinorOwned += 5; break;
                case "swanlake": gameState.SwanMinorOwned += 5; break;
                case "lacampanella": gameState.LaCampanellaMinorOwned += 5; break;
                case "enigma": gameState.EnigmaMinorOwned += 5; break;
                case "fate": gameState.FateMinorOwned += 5; break;
                case "odetojoy": gameState.OdeToJoyMinorOwned += 5; break;
            }
        }

        /// <summary>
        /// Fate Duet: Every 45th click doubles whichever minor score is the lowest owned.
        /// </summary>
        public static void FateDuet_OnClick(GameState gameState)
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
                    case 0: gameState.MoonlightMinorOwned *= 2; break;
                    case 1: gameState.EroicaMinorOwned *= 2; break;
                    case 2: gameState.SwanMinorOwned *= 2; break;
                    case 3: gameState.LaCampanellaMinorOwned *= 2; break;
                    case 4: gameState.EnigmaMinorOwned *= 2; break;
                    case 5: gameState.FateMinorOwned *= 2; break;
                    case 6: gameState.OdeToJoyMinorOwned *= 2; break;
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

        /// <summary>
        /// Cacophonic Blizzard (Winter I): Every 50th click "freezes" your current NPS value for 8 seconds.
        /// While frozen, NPS is immune to changes from upgrades/purchases.
        /// </summary>
        public static void CacophonicBlizzard_OnClick(GameState gameState)
        {
            if (gameState == null) return;

            gameState.CacophonicBlizzardClickCounter++;
            if (gameState.CacophonicBlizzardClickCounter >= 50)
            {
                gameState.CacophonicBlizzardClickCounter = 0;
                gameState.NpsFrozen = true;
                gameState.FrozenNpsValue = gameState.NotesPerSecond;
                gameState.NpsFreezeExpiry = DateTime.Now.AddSeconds(8);
            }
        }

        /// <summary>
        /// The Snow's Desire (Winter II): On Harmonious fragment purchase, 
        /// gain notes equal to frozen NPS × 20 (if NPS is currently frozen).
        /// </summary>
        public static void TheSnowsDesire_OnHarmoniousPurchase(GameState gameState)
        {
            if (gameState == null) return;

            // Only grant bonus if NPS is currently frozen
            if (gameState.NpsFrozen && DateTime.Now <= gameState.NpsFreezeExpiry)
            {
                double bonus = gameState.FrozenNpsValue * 20.0;
                MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, bonus);
            }
        }

        /// <summary>
        /// Winter Duet: Frozen NPS is used as a multiplier for all clicks.
        /// Returns the multiplier to apply to clicks (frozen NPS value).
        /// </summary>
        public static double WinterDuet_GetClickMultiplier(GameState gameState)
        {
            if (gameState == null) return 1.0;

            // Check if NPS is frozen and hasn't expired
            if (gameState.NpsFrozen && DateTime.Now <= gameState.NpsFreezeExpiry)
            {
                return gameState.FrozenNpsValue;
            }
            return 1.0;
        }

        // ==================== DIES IRAE EVENT WEAPONS (14-15) ====================
        // Placeholder - abilities to be implemented later
    }
}
