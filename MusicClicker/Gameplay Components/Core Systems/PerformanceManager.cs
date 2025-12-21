/*
 * File: PerformanceManager.cs
 * Summary: Manages frame rate optimization and performance monitoring
 * Purpose: Tracks animation performance and adaptively adjusts frame rate to maintain smooth gameplay
 * Notes: Uses moving average to prevent frame rate oscillation
 */

using System;
using System.Diagnostics;

namespace MusicClicker.GameLogic.Systems
{
    /// <summary>
    /// Manages performance monitoring and adaptive frame rate control.
    /// Automatically reduces frame rate on lower-end hardware to maintain smooth performance.
    /// </summary>
    public class PerformanceManager
    {
        // ------------------- CONSTANTS -------------------
        
        /// <summary>Minimum allowed frame rate (won't drop below this even on slow hardware)</summary>
        private const int MIN_FRAME_RATE = 30;
        
        /// <summary>Initial target frame rate on startup</summary>
        private const int INITIAL_FRAME_RATE = 144;
        
        /// <summary>Maximum number of samples to keep in moving average (prevents old data from skewing results)</summary>
        private const int MAX_SAMPLES = 120;
        
        /// <summary>Threshold multiplier - if frame time exceeds expected time by this factor, reduce frame rate</summary>
        private const double FRAME_TIME_THRESHOLD = 1.8;
        
        // ------------------- STATE -------------------
        
        /// <summary>Current target frame rate (frames per second)</summary>
        public int FrameRate { get; private set; }
        
        /// <summary>Stopwatch for measuring frame processing time</summary>
        private Stopwatch frameStopwatch;
        
        /// <summary>Moving average of frame processing time (milliseconds)</summary>
        private double averageFrameTimeMs;
        
        /// <summary>Number of samples collected for moving average</summary>
        private int sampleCount;
        
        // ------------------- CONSTRUCTOR -------------------
        
        public PerformanceManager()
        {
            FrameRate = INITIAL_FRAME_RATE;
            frameStopwatch = new Stopwatch();
            averageFrameTimeMs = 0.0;
            sampleCount = 0;
        }
        
        // ------------------- PERFORMANCE TRACKING -------------------
        
        /// <summary>
        /// Starts timing a new frame. Call this at the beginning of each animation frame.
        /// </summary>
        public void BeginFrame()
        {
            frameStopwatch.Restart();
        }
        
        /// <summary>
        /// Ends frame timing and updates performance metrics. Call this at the end of each animation frame.
        /// Automatically adjusts frame rate if performance is poor.
        /// </summary>
        /// <returns>True if frame rate was adjusted, false otherwise</returns>
        public bool EndFrame()
        {
            frameStopwatch.Stop();
            
            // Get frame processing time in milliseconds
            double frameTimeMs = frameStopwatch.Elapsed.TotalMilliseconds;
            
            // Update moving average
            sampleCount++;
            if (sampleCount > MAX_SAMPLES)
            {
                sampleCount = MAX_SAMPLES; // Cap sample window to prevent overflow
            }
            
            // Calculate new moving average
            averageFrameTimeMs = (averageFrameTimeMs * (sampleCount - 1) + frameTimeMs) / sampleCount;
            
            // Check if we need to reduce frame rate
            double expectedFrameTimeMs = 1000.0 / FrameRate;
            
            // If average frame time consistently exceeds expected time by threshold factor,
            // reduce frame rate to prevent jitter and lag
            if (averageFrameTimeMs > expectedFrameTimeMs * FRAME_TIME_THRESHOLD && FrameRate > MIN_FRAME_RATE)
            {
                // Reduce frame rate by half, but not below minimum
                FrameRate = Math.Max(MIN_FRAME_RATE, FrameRate / 2);
                
                // Reset samples to quickly adapt to new frame rate
                sampleCount = 0;
                averageFrameTimeMs = 0.0;
                
                return true; // Frame rate was adjusted
            }
            
            return false; // No adjustment needed
        }
        
        /// <summary>
        /// Gets the current frame interval in milliseconds (1000 / frame rate).
        /// </summary>
        public double GetFrameIntervalMs()
        {
            return 1000.0 / FrameRate;
        }
        
        /// <summary>
        /// Gets the current frame interval as a delta time in seconds (1 / frame rate).
        /// Useful for physics calculations that need time-based updates.
        /// </summary>
        public double GetDeltaTime()
        {
            return 1.0 / FrameRate;
        }
        
        /// <summary>
        /// Gets current performance statistics for debugging/monitoring.
        /// </summary>
        public (int frameRate, double avgFrameTimeMs, int samples) GetStats()
        {
            return (FrameRate, averageFrameTimeMs, sampleCount);
        }
        
        /// <summary>
        /// Resets performance tracking. Useful after major game state changes or loading.
        /// </summary>
        public void Reset()
        {
            FrameRate = INITIAL_FRAME_RATE;
            sampleCount = 0;
            averageFrameTimeMs = 0.0;
        }
    }
}
