/*
 * File: CarouselManager.cs
 * Summary: Manages 3D carousel navigation and animation
 * Purpose: Handles carousel rotation, drag interactions, momentum, and smooth animations
 * Notes: Extracted from MainWindow to improve modularity and readability
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace MusicClicker.GameLogic.Systems
{
    /// <summary>
    /// Manages the 3D carousel UI system including rotation, drag interactions, and animations.
    /// Provides smooth transitions between carousel positions with momentum physics.
    /// </summary>
    public class CarouselManager
    {
        // ------------------- CONSTANTS -------------------
        
        /// <summary>The radius of the circular carousel path in pixels</summary>
        private const double RADIUS = 350;
        
        /// <summary>Total number of buttons in the carousel (8 different game screens)</summary>
        private const int BUTTON_COUNT = 8;
        
        // ------------------- STATE -------------------
        
        /// <summary>Index of the currently selected button in the carousel (0-7)</summary>
        public int CurrentIndex { get; private set; } = 0;
        
        /// <summary>The target rotation angle the carousel is animating towards</summary>
        public double TargetRotation { get; private set; } = 0;
        
        /// <summary>The current rotation angle of the carousel</summary>
        public double CurrentRotation { get; private set; } = 0;
        
        /// <summary>Flag indicating whether the carousel is currently animating to a target position</summary>
        public bool IsAnimating { get; private set; } = false;
        
        // ------------------- DRAG STATE -------------------
        
        /// <summary>Whether user is currently dragging</summary>
        private bool isDragging = false;
        
        /// <summary>Last recorded mouse/touch position</summary>
        private Point lastDragPoint;
        
        /// <summary>Current velocity of drag motion</summary>
        private double dragVelocity = 0;
        
        /// <summary>Momentum after drag release (for inertia effect)</summary>
        private double dragMomentum = 0;
        
        /// <summary>List storing each carousel button along with its transform components for positioning</summary>
        public List<(Button button, TranslateTransform translate, ScaleTransform scale)> CarouselButtons { get; private set; }
        
        // ------------------- CONSTRUCTOR -------------------
        
        public CarouselManager()
        {
            CarouselButtons = new List<(Button, TranslateTransform, ScaleTransform)>();
        }
        
        // ------------------- BUTTON MANAGEMENT -------------------
        
        /// <summary>
        /// Adds a button to the carousel with proper transform setup.
        /// </summary>
        public void AddButton(Button button)
        {
            var transforms = GetButtonTransforms(button);
            CarouselButtons.Add(transforms);
        }
        
        /// <summary>
        /// Gets or creates the transform components for a carousel button.
        /// Each button needs a TranslateTransform for position and ScaleTransform for size.
        /// </summary>
        private (Button, TranslateTransform, ScaleTransform) GetButtonTransforms(Button button)
        {
            // Try to get existing transforms
            var transformGroup = button.RenderTransform as TransformGroup;
            if (transformGroup != null && transformGroup.Children.Count >= 2)
            {
                var translate = transformGroup.Children[0] as TranslateTransform;
                var scale = transformGroup.Children[1] as ScaleTransform;
                
                // If valid transforms exist, return them
                if (translate != null && scale != null)
                {
                    return (button, translate, scale);
                }
            }
            
            // No valid transforms found - create new ones
            var newTranslate = new TranslateTransform();
            var newScale = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
            var newTransformGroup = new TransformGroup();
            newTransformGroup.Children.Add(newTranslate);
            newTransformGroup.Children.Add(newScale);
            button.RenderTransform = newTransformGroup;
            
            return (button, newTranslate, newScale);
        }
        
        // ------------------- ROTATION METHODS -------------------
        
        /// <summary>
        /// Rotates the carousel by one position in the specified direction.
        /// </summary>
        /// <param name="direction">1 for clockwise, -1 for counter-clockwise</param>
        public void RotateCarousel(int direction)
        {
            if (IsAnimating) return; // Don't interrupt ongoing animation

            // Update current index with wrap-around (0-7)
            CurrentIndex = (CurrentIndex + direction + BUTTON_COUNT) % BUTTON_COUNT;
            
            // Calculate target rotation angle (each button is 45° apart)
            TargetRotation = CurrentIndex * (360.0 / BUTTON_COUNT);
            
            // Enable smooth animation to new position
            IsAnimating = true;
        }
        
        /// <summary>
        /// Snaps the carousel to the nearest button position.
        /// Called after drag ends or when momentum decays to near-zero.
        /// </summary>
        public void SnapToNearest()
        {
            // Calculate angle between each button (360° / 8 buttons = 45°)
            double angleStep = 360.0 / BUTTON_COUNT;
            
            // Find which button is closest to current rotation
            int nearestIndex = (int)Math.Round(CurrentRotation / angleStep) % BUTTON_COUNT;
            if (nearestIndex < 0) nearestIndex += BUTTON_COUNT; // Handle negative wrap-around
            
            // Set target to snap to nearest button
            CurrentIndex = nearestIndex;
            TargetRotation = nearestIndex * angleStep;
            IsAnimating = true; // Begin smooth animation to target
        }
        
        // ------------------- ANIMATION UPDATE -------------------
        
        /// <summary>
        /// Updates carousel animation state. Should be called every animation frame.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame (in seconds)</param>
        public void UpdateAnimation(double deltaTime)
        {
            // Apply drag momentum if present (decays over time)
            if (!isDragging && Math.Abs(dragMomentum) > 0.1)
            {
                CurrentRotation += dragMomentum;
                dragMomentum *= 0.92; // Decay momentum (friction effect)
                
                // When momentum is nearly zero, snap to nearest position
                if (Math.Abs(dragMomentum) < 1.0)
                {
                    dragMomentum = 0;
                    SnapToNearest();
                }
            }
            
            // Smooth animation towards target rotation
            if (IsAnimating && !isDragging)
            {
                double diff = TargetRotation - CurrentRotation;
                
                // Normalize angle difference to [-180, 180] range for shortest path
                while (diff > 180) diff -= 360;
                while (diff < -180) diff += 360;
                
                // Smooth interpolation (ease-out effect)
                double step = diff * 0.15; // 15% of remaining distance per frame
                
                // Stop animating when close enough to target
                if (Math.Abs(diff) < 0.5)
                {
                    CurrentRotation = TargetRotation;
                    IsAnimating = false;
                }
                else
                {
                    CurrentRotation += step;
                }
            }
        }
        
        /// <summary>
        /// Calculates and applies 3D carousel positions for all buttons.
        /// Creates illusion of depth using scale, opacity, and positioning.
        /// </summary>
        public void UpdateCarouselPositions()
        {
            double angleStep = 360.0 / BUTTON_COUNT;    // Angle between buttons (45°)
            double horizontalOffset = 15;                // Slight offset for visual balance

            // Update each button's position and appearance
            for (int i = 0; i < CarouselButtons.Count; i++)
            {
                var (button, translate, scale) = CarouselButtons[i];

                // Calculate this button's angle relative to current rotation
                double angle = (i * angleStep - CurrentRotation) * (Math.PI / 180.0);

                // Calculate vertical position on carousel circle (cosine gives vertical component)
                double y = -Math.Cos(angle) * RADIUS;
                
                // Check if button is at the bottom (foreground) of carousel
                bool isBottom = Math.Abs(Math.Cos(angle) + 1.0) < 0.1;
                
                // Calculate horizontal offset (sine gives horizontal component)
                double x = Math.Sin(angle) * horizontalOffset;
                
                // Apply position transforms
                translate.X = x;
                translate.Y = y;
                
                // Create depth illusion: buttons closer to viewer (bottom) appear larger
                double scaleValue = 0.7 + 0.3 * (Math.Cos(angle) + 1.0) / 2.0;
                scale.ScaleX = scaleValue;
                scale.ScaleY = scaleValue;
                
                // Fade buttons that are further away (at top of carousel)
                button.Opacity = 0.3 + 0.7 * (Math.Cos(angle) + 1.0) / 2.0;
                
                // Bring bottom button to front visually
                if (isBottom)
                {
                    button.ZIndex = 100;
                }
                else
                {
                    button.ZIndex = (int)(-y); // Further buttons have lower z-index
                }
            }
        }
        
        // ------------------- DRAG HANDLERS -------------------
        
        /// <summary>
        /// Called when user presses mouse/touch on the carousel canvas.
        /// Initiates drag mode and records starting position.
        /// </summary>
        public void OnPointerPressed(Point position)
        {
            isDragging = true;                // Enable drag mode
            lastDragPoint = position;         // Record starting position
            dragVelocity = 0;                 // Reset velocity
            dragMomentum = 0;                 // Reset momentum
            IsAnimating = false;              // Stop any ongoing animation
        }
        
        /// <summary>
        /// Called when user moves mouse/touch while dragging.
        /// Rotates the carousel based on vertical drag distance.
        /// </summary>
        public void OnPointerMoved(Point position)
        {
            if (!isDragging) return; // Only process if actively dragging

            // Calculate vertical distance moved since last update
            double deltaY = position.Y - lastDragPoint.Y;
            
            // Convert vertical movement to rotation (0.25 is sensitivity multiplier)
            double rotationDelta = deltaY * 0.25;
            CurrentRotation -= rotationDelta;
            
            // Store velocity for momentum calculation when drag ends
            dragVelocity = -rotationDelta;
            
            // Update last position for next frame
            lastDragPoint = position;
        }
        
        /// <summary>
        /// Called when user releases mouse/touch after dragging.
        /// Applies momentum and initiates snap-to-nearest animation.
        /// </summary>
        public void OnPointerReleased()
        {
            if (!isDragging) return; // Only process if was dragging
            
            isDragging = false; // Exit drag mode
            
            // Apply momentum based on final drag velocity (2.0 is momentum multiplier)
            dragMomentum = dragVelocity * 2.0;
            
            // If momentum is very low, immediately snap to nearest button
            if (Math.Abs(dragMomentum) < 1.0)
            {
                SnapToNearest();
            }
        }
    }
}
