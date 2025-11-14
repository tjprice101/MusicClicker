using Avalonia.Controls;
using System;

namespace MusicClicker
{
    /// <summary>
    /// Manages the purchase and application of upgrades in the game.
    /// Handles cost calculations, resource deduction, and UI updates.
    /// </summary>
    public static class UpgradeManager
    {
        /// <summary>
        /// Attempts to purchase one or more upgrades for the player.
        /// Calculates exponentially increasing costs and applies bonuses to game state.
        /// </summary>
        /// <param name="window">The main window containing game state and UI elements</param>
        /// <param name="owned">Reference to the number of upgrades already owned (will be modified)</param>
        /// <param name="baseCost">The initial cost of the first upgrade</param>
        /// <param name="npsIncrease">Notes per second increase granted by each upgrade</param>
        /// <param name="clickIncrease">Notes per click increase granted by each upgrade</param>
        /// <param name="amount">Number of upgrades to purchase (double.MaxValue = buy max possible)</param>
        /// <param name="ownedText">TextBlock displaying the number owned</param>
        /// <param name="costText">TextBlock displaying the current cost</param>
        public static void BuyUpgrade(MainWindow window, ref int owned, double baseCost, double npsIncrease, double clickIncrease, double amount, TextBlock ownedText, TextBlock costText)
        {
            // Get reference to the current game state
            var gameState = window.GameState;

            // Handle "Buy Max" case when amount is set to maximum value
            if (amount == double.MaxValue)
            {
                // Continue buying until we can't afford the next upgrade
                while (true)
                {
                    // Calculate cost with exponential scaling (15% increase per owned)
                    double cost = Math.Round(baseCost * Math.Pow(1.15, owned), 2);
                    
                    // Check if player has enough notes to purchase
                    if (gameState.Notes >= cost)
                    {
                        // Deduct cost from player's notes
                        gameState.Notes -= cost;
                        
                        // Increment owned count
                        owned++;
                        
                        // Apply bonuses to game state
                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else break; // Stop when we can't afford the next one
                }
            }
            else
            {
                // Purchase a specific number of upgrades
                for (int i = 0; i < amount; i++)
                {
                    // Calculate cost with exponential scaling (15% increase per owned)
                    double cost = Math.Round(baseCost * Math.Pow(1.15, owned), 2);
                    
                    // Check if player has enough notes to purchase
                    if (gameState.Notes >= cost)
                    {
                        // Deduct cost from player's notes
                        gameState.Notes -= cost;
                        
                        // Increment owned count
                        owned++;
                        
                        // Apply bonuses to game state
                        gameState.NotesPerSecond += npsIncrease;
                        gameState.NotesPerClick += clickIncrease;
                    }
                    else break; // Stop if we can't afford the next one
                }
            }

            // Update the specific upgrade's UI elements immediately
            ownedText.Text = $"Number Owned: {owned}";
            costText.Text = $"Cost: {Math.Round(baseCost * Math.Pow(1.15, owned), 2)}";

            // Update the main notes display on both screens to reflect the purchase
            window.NotesText.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
            window.UpgradeScreen.UpgradeNotesTextHeader.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";

            // Refresh all other upgrade costs and owned values across the UI
            UIUpdater.UpdateUI(window, gameState);
        }
    }
}