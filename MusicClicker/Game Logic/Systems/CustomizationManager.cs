/*
 * File: CustomizationManager.cs
 * Summary: Manages visual customization (clicker image and background)
 * Purpose: Centralizes customization logic for saving and restoring player choices
 * Notes: Handles bitmap loading and UI element updates
 */

using Avalonia.Controls;
using Avalonia.Media;
using MusicClicker.Helpers;
using System;

namespace MusicClicker.GameLogic.Systems
{
    /// <summary>
    /// Manages visual customization including clicker button images and background images.
    /// Handles saving, loading, and applying customizations from game state.
    /// </summary>
    public static class CustomizationManager
    {
        /// <summary>
        /// Restores customizations (clicker image and background) from saved game state.
        /// Called after loading a saved game to apply the player's previous visual choices.
        /// </summary>
        /// <param name="window">The main window to apply customizations to</param>
        /// <param name="gameState">Game state containing customization data</param>
        /// <param name="clickButton">The clicker button to customize</param>
        public static void RestoreCustomizations(Window window, GameState gameState, Button clickButton)
        {
            try
            {
                // Restore the clicker button image
                if (!string.IsNullOrEmpty(gameState.CurrentClickerImage))
                {
                    ApplyClickerImage(clickButton, gameState.CurrentClickerImage);
                }

                // Restore background image
                if (!string.IsNullOrEmpty(gameState.CurrentBackgroundImage))
                {
                    ApplyBackgroundImage(window, gameState.CurrentBackgroundImage);
                }
            }
            catch (Exception ex)
            {
                // Log errors but continue with defaults
                Console.WriteLine($"Failed to restore customizations: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Applies a clicker button image from the asset path.
        /// </summary>
        /// <param name="clickButton">The button to customize</param>
        /// <param name="imagePath">Path to the image asset</param>
        public static void ApplyClickerImage(Button clickButton, string imagePath)
        {
            try
            {
                var bitmap = ImageHelpers.GetBitmap(imagePath, 128);
                if (bitmap != null)
                {
                    var image = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.Uniform,
                        IsHitTestVisible = false // Prevent image from intercepting pointer events
                    };
                    
                    clickButton.Content = image;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to apply clicker image: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Applies a background image to the window.
        /// </summary>
        /// <param name="window">The window to apply the background to</param>
        /// <param name="imagePath">Path to the background image asset</param>
        public static void ApplyBackgroundImage(Window window, string imagePath)
        {
            try
            {
                var bitmap = ImageHelpers.GetBitmap(imagePath, 1920);
                if (bitmap != null)
                {
                    window.Background = new ImageBrush
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to apply background image: {ex.Message}");
            }
        }
    }
}
