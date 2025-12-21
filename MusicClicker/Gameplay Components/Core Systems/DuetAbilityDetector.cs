/*
 * File: DuetAbilityDetector.cs
 * Summary: Detects active duet weapon combinations
 * Purpose: Centralizes duet ability detection logic for cleaner code and easier expansion
 * Notes: Extracted from MainWindow to improve maintainability
 */

namespace MusicClicker.GameLogic.Systems
{
    /// <summary>
    /// Detects and validates duet weapon combinations.
    /// Provides centralized logic for determining which duet abilities are active.
    /// </summary>
    public static class DuetAbilityDetector
    {
        /// <summary>
        /// Checks if the given weapon pair forms any duet with cooldown-based abilities.
        /// </summary>
        /// <param name="weapon1">First equipped weapon</param>
        /// <param name="weapon2">Second equipped weapon</param>
        /// <returns>True if a cooldown duet is active, false otherwise</returns>
        public static bool HasCooldownDuet(string weapon1, string weapon2)
        {
            // Moonlight Duet (Lunar Phases)
            if (IsDuetPair(weapon1, weapon2, "IncisorOfMoonlight", "EulogyOfTheMoon"))
                return true;
            
            // Dies Irae Duet (Seven Seals)
            if (IsDuetPair(weapon1, weapon2, "SevenCircles", "HellsWrath"))
                return true;
            
            // Winter Duet (Absolute Zero)
            if (IsDuetPair(weapon1, weapon2, "CacophonicBlizzard", "TheSnowsDesire"))
                return true;
            
            // Eroica Duet (Victory March)
            if (IsDuetPair(weapon1, weapon2, "SakurasBlossom", "FuneralPrayer"))
                return true;
            
            // Swan Lake Duet (Mirror Lake)
            if (IsDuetPair(weapon1, weapon2, "StarScatteredWings", "ThousandWingedSwan"))
                return true;
            
            // La Campanella Duet (Chime Chain)
            if (IsDuetPair(weapon1, weapon2, "SymphonyOfBells", "RazerOfBellsChimes"))
                return true;
            
            // Enigma Duet (Cipher Wheel)
            if (IsDuetPair(weapon1, weapon2, "CreatorOfMystery", "Truthseeker"))
                return true;
            
            // Fate Duet (Hourglass Fracture)
            if (IsDuetPair(weapon1, weapon2, "AstralChainripper", "CosmicWeaver"))
                return true;
            
            // Ode to Joy Duet (Orchestra Conductor)
            if (IsDuetPair(weapon1, weapon2, "JoyfulCatharsis", "OdeToCreation"))
                return true;
            
            return false;
        }
        
        /// <summary>
        /// Helper method to check if two weapons match a duet pair (order-independent).
        /// </summary>
        /// <param name="weapon1">First equipped weapon</param>
        /// <param name="weapon2">Second equipped weapon</param>
        /// <param name="duetWeapon1">First weapon in the duet definition</param>
        /// <param name="duetWeapon2">Second weapon in the duet definition</param>
        /// <returns>True if the equipped weapons match the duet pair in any order</returns>
        private static bool IsDuetPair(string weapon1, string weapon2, string duetWeapon1, string duetWeapon2)
        {
            return (weapon1 == duetWeapon1 && weapon2 == duetWeapon2) ||
                   (weapon1 == duetWeapon2 && weapon2 == duetWeapon1);
        }
        
        /// <summary>
        /// Gets the name of the active duet, or null if no duet is active.
        /// </summary>
        /// <param name="weapon1">First equipped weapon</param>
        /// <param name="weapon2">Second equipped weapon</param>
        /// <returns>Duet name or null</returns>
        public static string? GetActiveDuetName(string weapon1, string weapon2)
        {
            if (IsDuetPair(weapon1, weapon2, "IncisorOfMoonlight", "EulogyOfTheMoon"))
                return "Moonlight Duet";
            
            if (IsDuetPair(weapon1, weapon2, "SevenCircles", "HellsWrath"))
                return "Dies Irae Duet";
            
            if (IsDuetPair(weapon1, weapon2, "CacophonicBlizzard", "TheSnowsDesire"))
                return "Winter Duet";
            
            if (IsDuetPair(weapon1, weapon2, "SakurasBlossom", "FuneralPrayer"))
                return "Eroica Duet";
            
            if (IsDuetPair(weapon1, weapon2, "StarScatteredWings", "ThousandWingedSwan"))
                return "Swan Lake Duet";
            
            if (IsDuetPair(weapon1, weapon2, "SymphonyOfBells", "RazerOfBellsChimes"))
                return "La Campanella Duet";
            
            if (IsDuetPair(weapon1, weapon2, "CreatorOfMystery", "Truthseeker"))
                return "Enigma Duet";
            
            if (IsDuetPair(weapon1, weapon2, "AstralChainripper", "CosmicWeaver"))
                return "Fate Duet";
            
            if (IsDuetPair(weapon1, weapon2, "JoyfulCatharsis", "OdeToCreation"))
                return "Ode to Joy Duet";
            
            return null;
        }
    }
}
