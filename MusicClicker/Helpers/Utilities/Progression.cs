using System;

namespace MusicClicker.Helpers
{
    // Central progression helper and tuning configuration.
    // Keeps formulas in one place so UI and managers can remain simple and consistent.
    public class ProgressionConfig
    {
        // Weapon idle-based pricing
        public double IdleBaseSeconds { get; set; } = 60.0;
        public double IdleGrowth { get; set; } = 6.0;
        public double ClickWeight { get; set; } = 50.0; // NPC -> NPS equivalent before clamp
        public double ClickCapFraction { get; set; } = 0.10; // max fraction of NPS clicks can substitute
        public double BaseOffsetPerTier { get; set; } = 0.0;

        // Event banner tuning
        public double EventNpsMultiplier { get; set; } = 2.0;
        public double EventNpcMultiplier { get; set; } = 5.0;
        public double EventBaseOffset { get; set; } = 0.0;

        // Upgrade cost growth (keeps exponential growth for upgrades)
        public double UpgradeCostGrowth { get; set; } = 1.15;
    }

    public static class Progression
    {
        // Public config instance. UI or tools may mutate for live tuning.
        public static ProgressionConfig Config { get; } = new ProgressionConfig();

        public static double ComputeWeaponCost(int tier, double nps, double npc)
        {
            double idleSeconds = Config.IdleBaseSeconds * Math.Pow(Config.IdleGrowth, tier);

            double clickContribution = Math.Min(npc * Config.ClickWeight, nps * Config.ClickCapFraction);
            double effectiveNps = nps + clickContribution;

            double cost = idleSeconds * effectiveNps + Config.BaseOffsetPerTier;
            return Math.Round(cost, 2);
        }

        public static double ComputeEventCost(double nps, double npc)
        {
            double cost = nps * Config.EventNpsMultiplier + npc * Config.EventNpcMultiplier + Config.EventBaseOffset;
            return Math.Round(cost, 2);
        }

        // Utility to compute upgrade cost using exponential growth (keeps previous behavior)
        public static double ComputeUpgradeCost(double baseCost, int owned)
        {
            return Math.Round(baseCost * Math.Pow(Config.UpgradeCostGrowth, owned), 2);
        }

        // Recalculate total NPS from all sources (upgrades + minor scores)
        public static double RecalculateNotesPerSecond(GameState gameState)
        {
            double totalNps = 0;

            // Add NPS from upgrades
            for (int i = 0; i < gameState.ChordOwned; i++)
            {
                totalNps += gameState.ChordBaseNpsEffect * Math.Pow(gameState.ChordNpsGrowth, i);
            }
            for (int i = 0; i < gameState.ScaleOwned; i++)
            {
                totalNps += gameState.ScaleBaseNpsEffect * Math.Pow(gameState.ScaleNpsGrowth, i);
            }
            for (int i = 0; i < gameState.OrchestraOwned; i++)
            {
                totalNps += gameState.OrchestraBaseNpsEffect * Math.Pow(gameState.OrchestraNpsGrowth, i);
            }
            for (int i = 0; i < gameState.SymphonyOwned; i++)
            {
                totalNps += gameState.SymphonyBaseNpsEffect * Math.Pow(gameState.SymphonyNpsGrowth, i);
            }

            // Add NPS from minor scores
            totalNps += gameState.MoonlightMinorOwned * 3000;
            totalNps += gameState.EroicaMinorOwned * 8000;
            totalNps += gameState.SwanMinorOwned * 15000;
            totalNps += gameState.LaCampanellaMinorOwned * 35000;
            totalNps += gameState.EnigmaMinorOwned * 75000;
            totalNps += gameState.FateMinorOwned * 135000;
            totalNps += gameState.OdeToJoyMinorOwned * 255000;

            // Add NPS from major scores
            totalNps += gameState.MoonlightMajorOwned * 6000;
            totalNps += gameState.EroicaMajorOwned * 16000;
            totalNps += gameState.SwanMajorOwned * 30000;
            totalNps += gameState.LaCampanellaMajorOwned * 70000;
            totalNps += gameState.EnigmaMajorOwned * 150000;
            totalNps += gameState.FateMajorOwned * 270000;
            totalNps += gameState.OdeToJoyMajorOwned * 510000;

            return totalNps;
        }
    }
}
