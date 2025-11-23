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

                owned++;
                gameState.NotesPerSecond += 3000;

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                owned++;
                gameState.NotesPerSecond += 8000;

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftSwanLakeMinor(MainWindow window, ref int owned)
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

                owned++;
                gameState.NotesPerSecond += 15000;

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                owned++;
                gameState.NotesPerSecond += 35000;

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                owned++;
                gameState.NotesPerSecond += 75000;

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                owned++;
                gameState.NotesPerSecond += 135000;

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                owned++;
                gameState.NotesPerSecond += 255000;

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                if (gameState.EroicaMajorAbility)
                {
                    gameState.MoonlightMinorOwned += 2;
                    gameState.NotesPerSecond += 6000;
                }

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                if (gameState.EroicaMajorAbility)
                {
                    gameState.EroicaMinorOwned += 2;
                    gameState.NotesPerSecond += 16000;
                }

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        public static bool TryCraftSwanLakeMajor(MainWindow window, ref int owned)
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

                if (gameState.EroicaMajorAbility)
                {
                    gameState.SwanMinorOwned += 2;
                    gameState.NotesPerSecond += 30000;
                }

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                if (gameState.EroicaMajorAbility)
                {
                    gameState.LaCampanellaMinorOwned += 2;
                    gameState.NotesPerSecond += 70000;
                }

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                if (gameState.EroicaMajorAbility)
                {
                    gameState.FateMinorOwned += 2;
                    gameState.NotesPerSecond += 150000;
                }

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                if (gameState.EroicaMajorAbility)
                {
                    gameState.FateMinorOwned += 2;
                    gameState.NotesPerSecond += 270000;
                }

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
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

                if (gameState.EroicaMajorAbility)
                {
                    gameState.OdeToJoyMinorOwned += 2;
                    gameState.NotesPerSecond += 510000;
                }

                canCraft = true;
            }

            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }
    }
}
