using Avalonia.Controls;

namespace MusicClicker
{
    /// <summary>
    /// Manages the crafting system for "Unite the Symphony" feature.
    /// Handles crafting of both Minor and Major musical scores that require various resources
    /// and provide Notes Per Second bonuses when crafted.
    /// </summary>
    public static class UniteTheSymphonyManager
    {
        // ==================== MINOR SCORES ====================
        // Minor scores require: Keys, Scales, Progressions, Essence, and Melodious upgrades
        // They provide direct NPS bonuses when crafted

        /// <summary>
        /// Attempts to craft a Moonlight Sonata Minor score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 5 Beethoven Essence, 10 Melodious
        /// Grants: +3,000 Notes Per Second
        /// </summary>
        public static bool TryCraftMoonlightMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.MoonlightMinorKeys >= 1 &&
                gameState.MoonlightMinorScales >= 1 &&
                gameState.MoonlightMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 5 &&
                gameState.MelodiousOwned >= 10)
            {
                // Deduct the crafting cost from resources
                gameState.MoonlightMinorKeys--;
                gameState.MoonlightMinorScales--;
                gameState.MoonlightMinorProgressions--;
                gameState.EssenceBeethoven -= 5;
                gameState.MelodiousOwned -= 10;

                // Increment owned count and apply NPS bonus
                owned++;
                gameState.NotesPerSecond += 3000;

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft an Eroica Minor score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 10 Beethoven Essence, 15 Melodious
        /// Grants: +8,000 Notes Per Second
        /// </summary>
        public static bool TryCraftEroicaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.EroicaMinorKeys >= 1 &&
                gameState.EroicaMinorScales >= 1 &&
                gameState.EroicaMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 10 &&
                gameState.MelodiousOwned >= 15)
            {
                // Deduct the crafting cost from resources
                gameState.EroicaMinorKeys--;
                gameState.EroicaMinorScales--;
                gameState.EroicaMinorProgressions--;
                gameState.EssenceBeethoven -= 10;
                gameState.MelodiousOwned -= 15;

                // Increment owned count and apply NPS bonus
                owned++;
                gameState.NotesPerSecond += 8000;

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft a Swan Lake Minor score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 5 Pyotr Essence, 20 Melodious
        /// Grants: +15,000 Notes Per Second
        /// </summary>
        public static bool TryCraftSwanLakeMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.SwanLakeMinorKeys >= 1 &&
                gameState.SwanLakeMinorScales >= 1 &&
                gameState.SwanLakeMinorProgressions >= 1 &&
                gameState.EssencePyotr >= 5 &&
                gameState.MelodiousOwned >= 20)
            {
                // Deduct the crafting cost from resources
                gameState.SwanLakeMinorKeys--;
                gameState.SwanLakeMinorScales--;
                gameState.SwanLakeMinorProgressions--;
                gameState.EssencePyotr -= 5;
                gameState.MelodiousOwned -= 20;

                // Increment owned count and apply NPS bonus
                owned++;
                gameState.NotesPerSecond += 15000;

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft a La Campanella Minor score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 10 Liszt Essence, 25 Melodious
        /// Grants: +35,000 Notes Per Second
        /// </summary>
        public static bool TryCraftLaCampanellaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.LaCampanellaMinorKeys >= 1 &&
                gameState.LaCampanellaMinorScales >= 1 &&
                gameState.LaCampanellaMinorProgressions >= 1 &&
                gameState.EssenceLiszt >= 10 &&
                gameState.MelodiousOwned >= 25)
            {
                // Deduct the crafting cost from resources
                gameState.LaCampanellaMinorKeys--;
                gameState.LaCampanellaMinorScales--;
                gameState.LaCampanellaMinorProgressions--;
                gameState.EssenceLiszt -= 10;
                gameState.MelodiousOwned -= 25;

                // Increment owned count and apply NPS bonus
                owned++;
                gameState.NotesPerSecond += 35000;

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft an Enigma Minor score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 10 Elgar Essence, 30 Melodious
        /// Grants: +75,000 Notes Per Second
        /// </summary>
        public static bool TryCraftEnigmaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.EnigmaMinorKeys >= 1 &&
                gameState.EnigmaMinorScales >= 1 &&
                gameState.EnigmaMinorProgressions >= 1 &&
                gameState.EssenceElgar >= 10 &&
                gameState.MelodiousOwned >= 30)
            {
                // Deduct the crafting cost from resources
                gameState.EnigmaMinorKeys--;
                gameState.EnigmaMinorScales--;
                gameState.EnigmaMinorProgressions--;
                gameState.EssenceElgar -= 10;
                gameState.MelodiousOwned -= 30;

                // Increment owned count and apply NPS bonus
                owned++;
                gameState.NotesPerSecond += 75000;

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft a Fate Minor score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 10 Beethoven Essence, 35 Melodious
        /// Grants: +135,000 Notes Per Second
        /// </summary>
        public static bool TryCraftFateMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.FateMinorKeys >= 1 &&
                gameState.FateMinorScales >= 1 &&
                gameState.FateMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 10 &&
                gameState.MelodiousOwned >= 35)
            {
                // Deduct the crafting cost from resources
                gameState.FateMinorKeys--;
                gameState.FateMinorScales--;
                gameState.FateMinorProgressions--;
                gameState.EssenceBeethoven -= 10;
                gameState.MelodiousOwned -= 35;

                // Increment owned count and apply NPS bonus
                owned++;
                gameState.NotesPerSecond += 135000;

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft an Ode to Joy Minor score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 10 Beethoven Essence, 40 Melodious
        /// Grants: +255,000 Notes Per Second
        /// </summary>
        public static bool TryCraftOdeToJoyMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.OdeToJoyMinorKeys >= 1 &&
                gameState.OdeToJoyMinorScales >= 1 &&
                gameState.OdeToJoyMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 10 &&
                gameState.MelodiousOwned >= 40)
            {
                // Deduct the crafting cost from resources
                gameState.OdeToJoyMinorKeys--;
                gameState.OdeToJoyMinorScales--;
                gameState.OdeToJoyMinorProgressions--;
                gameState.EssenceBeethoven -= 10;
                gameState.MelodiousOwned -= 40;

                // Increment owned count and apply NPS bonus
                owned++;
                gameState.NotesPerSecond += 255000;

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // ==================== MAJOR SCORES ====================
        // Major scores require: Keys, Scales, Progressions, Essence, and Harmonious upgrades
        // If Eroica Major Ability is unlocked, they grant bonus Minor scores and extra NPS

        /// <summary>
        /// Attempts to craft a Moonlight Sonata Major score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 10 Beethoven Essence, 10 Harmonious
        /// Grants: If Eroica Major Ability is active: +2 Moonlight Minor scores, +6,000 NPS
        /// </summary>
        public static bool TryCraftMoonlightSonataMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.MoonlightMajorKeys >= 1 &&
                gameState.MoonlightMajorScales >= 1 &&
                gameState.MoonlightMajorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 10 &&
                gameState.HarmoniousOwned >= 10)
            {
                // Deduct the crafting cost from resources
                gameState.MoonlightMajorKeys--;
                gameState.MoonlightMajorScales--;
                gameState.MoonlightMajorProgressions--;
                gameState.EssenceBeethoven -= 10;
                gameState.HarmoniousOwned -= 10;

                // Increment owned count for this major score
                owned++;
                
                // If Eroica Major Ability is unlocked, grant bonus Minor scores and NPS
                if (gameState.EroicaMajorAbility)
                {
                    gameState.MoonlightMinorOwned += 2;
                    gameState.NotesPerSecond += 6000;
                }

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft an Eroica Major score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 15 Beethoven Essence, 15 Harmonious
        /// Grants: If Eroica Major Ability is active: +2 Eroica Minor scores, +16,000 NPS
        /// </summary>
        public static bool TryCraftEroicaMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.EroicaMajorKeys >= 1 &&
                gameState.EroicaMajorScales >= 1 &&
                gameState.EroicaMajorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 15 &&
                gameState.HarmoniousOwned >= 15)
            {
                // Deduct the crafting cost from resources
                gameState.EroicaMajorKeys--;
                gameState.EroicaMajorScales--;
                gameState.EroicaMajorProgressions--;
                gameState.EssenceBeethoven -= 15;
                gameState.HarmoniousOwned -= 15;

                // Increment owned count for this major score
                owned++;
                
                // If Eroica Major Ability is unlocked, grant bonus Minor scores and NPS
                if (gameState.EroicaMajorAbility)
                {
                    gameState.EroicaMinorOwned += 2;
                    gameState.NotesPerSecond += 16000;
                }

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft a Swan Lake Major score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 10 Pyotr Essence, 20 Harmonious
        /// Grants: If Eroica Major Ability is active: +2 Swan Lake Minor scores, +30,000 NPS
        /// </summary>
        public static bool TryCraftSwanLakeMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.SwanLakeMajorKeys >= 1 &&
                gameState.SwanLakeMajorScales >= 1 &&
                gameState.SwanLakeMajorProgressions >= 1 &&
                gameState.EssencePyotr >= 10 &&
                gameState.HarmoniousOwned >= 20)
            {
                // Deduct the crafting cost from resources
                gameState.SwanLakeMajorKeys--;
                gameState.SwanLakeMajorScales--;
                gameState.SwanLakeMajorProgressions--;
                gameState.EssencePyotr -= 10;
                gameState.HarmoniousOwned -= 20;

                // Increment owned count for this major score
                owned++;
                
                // If Eroica Major Ability is unlocked, grant bonus Minor scores and NPS
                if (gameState.EroicaMajorAbility)
                {
                    gameState.SwanMinorOwned += 2;
                    gameState.NotesPerSecond += 30000;
                }

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft a La Campanella Major score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 15 Liszt Essence, 25 Harmonious
        /// Grants: If Eroica Major Ability is active: +2 La Campanella Minor scores, +70,000 NPS
        /// </summary>
        public static bool TryCraftLaCampanellaMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.LaCampanellaMajorKeys >= 1 &&
                gameState.LaCampanellaMajorScales >= 1 &&
                gameState.LaCampanellaMajorProgressions >= 1 &&
                gameState.EssenceLiszt >= 15 &&
                gameState.HarmoniousOwned >= 25)
            {
                // Deduct the crafting cost from resources
                gameState.LaCampanellaMajorKeys--;
                gameState.LaCampanellaMajorScales--;
                gameState.LaCampanellaMajorProgressions--;
                gameState.EssenceLiszt -= 15;
                gameState.HarmoniousOwned -= 25;

                // Increment owned count for this major score
                owned++;
                
                // If Eroica Major Ability is unlocked, grant bonus Minor scores and NPS
                if (gameState.EroicaMajorAbility)
                {
                    gameState.LaCampanellaMinorOwned += 2;
                    gameState.NotesPerSecond += 70000;
                }

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft an Enigma Major score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 15 Elgar Essence, 30 Harmonious
        /// Grants: If Eroica Major Ability is active: +2 Fate Minor scores, +150,000 NPS
        /// </summary>
        public static bool TryCraftEnigmaMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.EnigmaMajorKeys >= 1 &&
                gameState.EnigmaMajorScales >= 1 &&
                gameState.EnigmaMajorProgressions >= 1 &&
                gameState.EssenceElgar >= 15 &&
                gameState.HarmoniousOwned >= 30)
            {
                // Deduct the crafting cost from resources
                gameState.EnigmaMajorKeys--;
                gameState.EnigmaMajorScales--;
                gameState.EnigmaMajorProgressions--;
                gameState.EssenceElgar -= 15;
                gameState.HarmoniousOwned -= 30;

                // Increment owned count for this major score
                owned++;
                
                // If Eroica Major Ability is unlocked, grant bonus Minor scores and NPS
                if (gameState.EroicaMajorAbility)
                {
                    gameState.FateMinorOwned += 2;
                    gameState.NotesPerSecond += 150000;
                }

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft a Fate Major score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 15 Beethoven Essence, 35 Harmonious
        /// Grants: If Eroica Major Ability is active: +2 Fate Minor scores, +270,000 NPS
        /// </summary>
        public static bool TryCraftFateMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.FateMajorKeys >= 1 &&
                gameState.FateMajorScales >= 1 &&
                gameState.FateMajorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 15 &&
                gameState.HarmoniousOwned >= 35)
            {
                // Deduct the crafting cost from resources
                gameState.FateMajorKeys--;
                gameState.FateMajorScales--;
                gameState.FateMajorProgressions--;
                gameState.EssenceBeethoven -= 15;
                gameState.HarmoniousOwned -= 35;

                // Increment owned count for this major score
                owned++;
                
                // If Eroica Major Ability is unlocked, grant bonus Minor scores and NPS
                if (gameState.EroicaMajorAbility)
                {
                    gameState.FateMinorOwned += 2;
                    gameState.NotesPerSecond += 270000;
                }

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        /// <summary>
        /// Attempts to craft an Ode to Joy Major score.
        /// Requires: 1 Key, 1 Scale, 1 Progression, 15 Beethoven Essence, 40 Harmonious
        /// Grants: If Eroica Major Ability is active: +2 Ode to Joy Minor scores, +510,000 NPS
        /// </summary>
        public static bool TryCraftOdeToJoyMajor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if player has all required resources
            if (gameState.OdeToJoyMajorKeys >= 1 &&
                gameState.OdeToJoyMajorScales >= 1 &&
                gameState.OdeToJoyMajorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 15 &&
                gameState.HarmoniousOwned >= 40)
            {
                // Deduct the crafting cost from resources
                gameState.OdeToJoyMajorKeys--;
                gameState.OdeToJoyMajorScales--;
                gameState.OdeToJoyMajorProgressions--;
                gameState.EssenceBeethoven -= 15;
                gameState.HarmoniousOwned -= 40;

                // Increment owned count for this major score
                owned++;
                
                // If Eroica Major Ability is unlocked, grant bonus Minor scores and NPS
                if (gameState.EroicaMajorAbility)
                {
                    gameState.OdeToJoyMinorOwned += 2;
                    gameState.NotesPerSecond += 510000;
                }

                canCraft = true;
            }

            // Refresh UI to reflect resource changes if crafting succeeded
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }
    }
}