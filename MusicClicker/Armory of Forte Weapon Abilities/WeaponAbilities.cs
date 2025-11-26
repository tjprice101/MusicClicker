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
    }
}
