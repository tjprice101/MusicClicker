using Avalonia.Controls;

namespace MusicClicker
{
    public static class UniteTheSymphonyManager
    {
        public static bool TryCraft(MainWindow window,
            ref int owned,
            int costEssenceBeethoven,
            int costEssencePyotr,
            int costMelodious,
            int costHarmonious,
            int addNps,
            bool isMajor,
            ref bool majorAbilityFlag)
        {
            var gameState = window.GameState;

            if (gameState.EssenceBeethoven < costEssenceBeethoven) return false;
            if (gameState.EssencePyotr < costEssencePyotr) return false;
            if (gameState.MelodiousOwned < costMelodious) return false;
            if (gameState.HarmoniousOwned < costHarmonious) return false;

            gameState.EssenceBeethoven -= costEssenceBeethoven;
            gameState.EssencePyotr -= costEssencePyotr;
            gameState.MelodiousOwned -= costMelodious;
            gameState.HarmoniousOwned -= costHarmonious;

            owned++;
            if (addNps > 0) gameState.NotesPerSecond += addNps;
            if (isMajor) majorAbilityFlag = true;

            // Update relevant UI
            UIUpdater.UpdateUnitySymphonyUI(window, gameState);
            UIUpdater.UpdateEssenceUI(window, gameState);
            UIUpdater.UpdateFragmentationUI(window, gameState);

            return true;
        }
    }
}
