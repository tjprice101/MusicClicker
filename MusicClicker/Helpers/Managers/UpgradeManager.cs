using Avalonia.Controls;
using System;
using MusicClicker.Helpers;

namespace MusicClicker
{
    // Responsible for executing upgrade purchases and applying their effects to GameState.
    // Performance-conscious changes in this class focus on reducing repeated expensive
    // math and allocations when purchasing multiple upgrades (especially "buy max").
    public static class UpgradeManager
    {
        // Purchase upgrades for a given upgrade slot.
        // - `owned` is passed by reference so calling code (and UI) can keep a synced counter.
        // - `amount == double.MaxValue` is treated as "buy max" and will loop until funds run out.
        // Implementation notes and performance choices:
        // 1) Repeated calls to Math.Pow are avoided by maintaining a running multiplier.
        //    The game models costs as baseCost * 1.15^owned (rounded to 2 decimals).
        //    Computing the exponential once and updating it incrementally avoids allocating
        //    and calling Math.Pow on every iteration, which matters when buying many items.
        // 2) We minimize property access and repeated Math.Round calls by caching values
        //    where safe (the visible cost remains rounded to 2 decimals for UI parity).
        // New signature: baseNpsEffect/growth and baseClickEffect/growth control per-purchase effect scaling.
        public static void BuyUpgrade(MainWindow window, ref int owned, double baseCost,
            double baseNpsEffect, double npsGrowth,
            double baseClickEffect, double clickGrowth,
            double amount, TextBlock ownedText, TextBlock costText)
        {
            var gameState = window.GameState;

            // Compute the starting multiplier for the current 'owned' count: 1.15^owned
            double multiplier = Math.Pow(1.15, owned);

            static double RoundedCost(double baseC, double mul) => Math.Round(baseC * mul, 2);

            // Apply Moonlight Duet Waning phase cost reduction (50% off)
            int moonPhase = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_GetCurrentPhase(gameState);
            bool allMoonPhasesActive = MusicClicker.Armory.WeaponAbilities.MoonlightDuet_AreAllPhasesActive(gameState);
            double costMultiplier = (allMoonPhasesActive || moonPhase == 3) ? 0.5 : 1.0; // Waning phase or all phases active

            // Helper to compute effect increase for current owned count
            static double EffectIncrease(double baseEffect, double growth, int ownedCount)
            {
                if (baseEffect == 0) return 0.0;
                return baseEffect * Math.Pow(growth, ownedCount);
            }

            // Track purchases for Mirror Lake queueing
            int purchasesMade = 0;
            int initialOwned = owned;

            if (amount == double.MaxValue)
            {
                while (true)
                {
                    double cost = RoundedCost(baseCost, multiplier) * costMultiplier;
                    if (MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes) >= cost)
                    {
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -cost);

                        // If any weapon abilities respond to upgrades, invoke them here.
                        try
                        {
                            if (gameState.IncisorOfMoonlightAbility)
                                MusicClicker.Armory.WeaponAbilities.IncisorOfMoonlight_OnUpgradePurchase(gameState);

                            if (gameState.EulogyOfTheMoonAbility)
                                MusicClicker.Armory.WeaponAbilities.EulogyOfTheMoon_OnUpgradePurchase(gameState);

                            // Eroica Weapon Passives
                            if (gameState.SakurasBlossom && 
                                (gameState.CurrentResonatedWeapon1 == "SakurasBlossom" || gameState.CurrentResonatedWeapon2 == "SakurasBlossom"))
                                MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnUpgradePurchase(gameState);

                            if (gameState.FuneralPrayer && 
                                (gameState.CurrentResonatedWeapon1 == "FuneralPrayer" || gameState.CurrentResonatedWeapon2 == "FuneralPrayer"))
                                MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnUpgradePurchase(gameState);

                            // Enigma Weapon Passive - Truthseeker: Revelation Burst
                            if (gameState.TruthseekerAbility)
                                MusicClicker.Armory.WeaponAbilities.Truthseeker_OnUpgradePurchase(gameState);
                        }
                        catch (Exception)
                        {
                            // Swallow exceptions from ability code to avoid breaking purchases.
                        }

                        owned++;
                        purchasesMade++;
                        // increase multiplier for next purchase
                        multiplier *= 1.15;
                    }
                    else break;
                }
            }
            else
            {
                int purchaseCount = (int)amount;
                for (int i = 0; i < purchaseCount; i++)
                {
                    double cost = RoundedCost(baseCost, multiplier) * costMultiplier;
                    if (MusicClicker.Helpers.AtomicDouble.Read(ref gameState._notes) >= cost)
                    {
                        MusicClicker.Helpers.AtomicDouble.Add(ref gameState._notes, -cost);

                            // Invoke weapon ability hooks if their flags are active.
                            try
                            {
                                if (gameState.IncisorOfMoonlightAbility)
                                    MusicClicker.Armory.WeaponAbilities.IncisorOfMoonlight_OnUpgradePurchase(gameState);

                                if (gameState.EulogyOfTheMoonAbility)
                                    MusicClicker.Armory.WeaponAbilities.EulogyOfTheMoon_OnUpgradePurchase(gameState);

                                // Eroica Weapon Passives
                                if (gameState.SakurasBlossom && 
                                    (gameState.CurrentResonatedWeapon1 == "SakurasBlossom" || gameState.CurrentResonatedWeapon2 == "SakurasBlossom"))
                                    MusicClicker.Armory.WeaponAbilities.SakurasBlossom_OnUpgradePurchase(gameState);

                                if (gameState.FuneralPrayer && 
                                    (gameState.CurrentResonatedWeapon1 == "FuneralPrayer" || gameState.CurrentResonatedWeapon2 == "FuneralPrayer"))
                                    MusicClicker.Armory.WeaponAbilities.FuneralPrayer_OnUpgradePurchase(gameState);

                                // Enigma Weapon Passive - Truthseeker: Revelation Burst
                                if (gameState.TruthseekerAbility)
                                    MusicClicker.Armory.WeaponAbilities.Truthseeker_OnUpgradePurchase(gameState);
                            }
                            catch (Exception)
                            {
                                // ignore
                            }

                        owned++;
                        purchasesMade++;
                        multiplier *= 1.15;
                    }
                    else break;
                }
            }

            // Update UI
            ownedText.Text = $"Number Owned: {owned}";
            costText.Text = $"Cost: {NumberFormatter.FormatLargeNumber(baseCost * multiplier)}";

            // Recalculate NPS and NPC totals from all sources
            if (purchasesMade > 0)
            {
                gameState.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(gameState);
                gameState.NotesPerClick = MusicClicker.Helpers.Progression.RecalculateNotesPerClick(gameState);
                
                // Invalidate cached upgrade costs so they recalculate on next UI update (performance optimization)
                gameState.CachedChordCost = 0;
                gameState.CachedScaleCost = 0;
                gameState.CachedOrchestraCost = 0;
                gameState.CachedSymphonyCost = 0;
                gameState.CachedAriaCost = 0;
                gameState.CachedRequiemCost = 0;
                gameState.CachedOpusCost = 0;
                gameState.CachedMagnumOpusCost = 0;
            }

            window.NotesText.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";
            window.UpgradeScreen.UpgradeNotesTextHeader.Text = $"Notes: {NumberFormatter.FormatLargeNumber(gameState.Notes)}";

            // Queue upgrade purchases for Mirror Lake reflection
            if (purchasesMade > 0 && gameState.SwanLakeDuetActive && DateTime.Now <= gameState.SwanLakeDuetExpiry)
            {
                // Determine upgrade name from cost text header
                string upgradeName = DetermineUpgradeName(baseCost);
            }

            // Bank upgrade purchases for Fate Duet Hourglass
            if (purchasesMade > 0 && gameState.FateDuetActive && !gameState.FateDuetHasFlipped && DateTime.Now <= gameState.FateDuetExpiry)
            {
                string upgradeName = DetermineUpgradeName(baseCost);
                double queueMultiplier = Math.Pow(1.15, initialOwned);
                for (int i = 0; i < purchasesMade; i++)
                {
                    double individualCost = RoundedCost(baseCost, queueMultiplier) * costMultiplier;
                    MusicClicker.Armory.WeaponAbilities.FateDuet_BankAction(gameState, "BuyUpgrade", (upgradeName, individualCost));
                    queueMultiplier *= 1.15;
                }
            }

            UIUpdater.UpdateUI(window, gameState);
        }

        private static string DetermineUpgradeName(double baseCost)
        {
            // Map base costs to upgrade names
            return baseCost switch
            {
                15 => "Moonlight Sonata - Opus 1",
                100 => "Moonlight Sonata - Opus 2",
                1100 => "Moonlight Sonata - Opus 3",
                12000 => "Eroica - Opus 1",
                130000 => "Eroica - Opus 2",
                1400000 => "Eroica - Opus 3",
                20000000 => "Swan Lake - Opus 1",
                330000000 => "Swan Lake - Opus 2",
                5100000000 => "Swan Lake - Opus 3",
                51000000000 => "La Campanella - Opus 1",
                560000000000 => "La Campanella - Opus 2",
                6100000000000 => "La Campanella - Opus 3",
                75000000000000 => "Enigma Variations - Opus 1",
                900000000000000 => "Enigma Variations - Opus 2",
                10000000000000000 => "Enigma Variations - Opus 3",
                120000000000000000.0 => "Fate - Opus 1",
                1500000000000000000.0 => "Fate - Opus 2",
                18000000000000000000.0 => "Fate - Opus 3",
                210000000000000000000.0 => "Ode to Joy - Opus 1",
                2700000000000000000000.0 => "Ode to Joy - Opus 2",
                33000000000000000000000.0 => "Ode to Joy - Opus 3",
                _ => "Unknown"
            };
        }
    }
}