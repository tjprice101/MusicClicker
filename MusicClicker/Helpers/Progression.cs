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
    }
}
