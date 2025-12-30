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
                double npsToAdd = gameState.ChordBaseNpsEffect * Math.Pow(gameState.ChordNpsGrowth, i);
                if (double.IsInfinity(npsToAdd) || double.IsNaN(npsToAdd))
                    break;
                totalNps += npsToAdd;
            }
            for (int i = 0; i < gameState.ScaleOwned; i++)
            {
                double npsToAdd = gameState.ScaleBaseNpsEffect * Math.Pow(gameState.ScaleNpsGrowth, i);
                if (double.IsInfinity(npsToAdd) || double.IsNaN(npsToAdd))
                    break;
                totalNps += npsToAdd;
            }
            for (int i = 0; i < gameState.OrchestraOwned; i++)
            {
                double npsToAdd = gameState.OrchestraBaseNpsEffect * Math.Pow(gameState.OrchestraNpsGrowth, i);
                if (double.IsInfinity(npsToAdd) || double.IsNaN(npsToAdd))
                    break;
                totalNps += npsToAdd;
            }
            for (int i = 0; i < gameState.SymphonyOwned; i++)
            {
                double npsToAdd = gameState.SymphonyBaseNpsEffect * Math.Pow(gameState.SymphonyNpsGrowth, i);
                if (double.IsInfinity(npsToAdd) || double.IsNaN(npsToAdd))
                    break;
                totalNps += npsToAdd;
            }

            // Add NPS from minor scores with overflow protection
            double moonlightNps = (double)gameState.MoonlightMinorOwned * 3000.0;
            double eroicaNps = (double)gameState.EroicaMinorOwned * 8000.0;
            double swanNps = (double)gameState.SwanMinorOwned * 15000.0;
            double laCampanellaNps = (double)gameState.LaCampanellaMinorOwned * 35000.0;
            double enigmaNps = (double)gameState.EnigmaMinorOwned * 75000.0;
            double fateNps = (double)gameState.FateMinorOwned * 135000.0;
            double odeToJoyNps = (double)gameState.OdeToJoyMinorOwned * 255000.0;

            totalNps += moonlightNps;
            totalNps += eroicaNps;
            totalNps += swanNps;
            totalNps += laCampanellaNps;
            totalNps += enigmaNps;
            totalNps += fateNps;
            totalNps += odeToJoyNps;

            // Cap at double.MaxValue to prevent overflow
            if (double.IsInfinity(totalNps) || double.IsNaN(totalNps) || totalNps < 0)
                return double.MaxValue / 2; // Use half of max to leave room for multipliers

            // Major scores do not provide base NPS - they only provide Crescendance abilities

            return totalNps;
        }

        // Recalculate total NPC from all sources (upgrades)
        public static double RecalculateNotesPerClick(GameState gameState)
        {
            double totalNpc = 1.0; // Base click value

            // Add NPC from click-based upgrades
            for (int i = 0; i < gameState.AriaOwned; i++)
            {
                totalNpc += gameState.AriaBaseClickEffect * Math.Pow(gameState.AriaClickGrowth, i);
            }
            for (int i = 0; i < gameState.RequiemOwned; i++)
            {
                totalNpc += gameState.RequiemBaseClickEffect * Math.Pow(gameState.RequiemClickGrowth, i);
            }
            for (int i = 0; i < gameState.OpusOwned; i++)
            {
                totalNpc += gameState.OpusBaseClickEffect * Math.Pow(gameState.OpusClickGrowth, i);
            }
            for (int i = 0; i < gameState.MagnumOpusOwned; i++)
            {
                totalNpc += gameState.MagnumOpusBaseClickEffect * Math.Pow(gameState.MagnumOpusClickGrowth, i);
            }

            // Clair de Lune Crescendance Passive: NPC += NPS^5
            if (gameState.CurrentResonatedScore == "ClairDeLune")
            {
                double npsBonus = Math.Pow(gameState.NotesPerSecond, 5);
                if (!double.IsInfinity(npsBonus) && !double.IsNaN(npsBonus))
                {
                    totalNpc += npsBonus;
                }
            }

            return totalNpc;
        }
    }
}
