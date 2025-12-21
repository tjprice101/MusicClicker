/*
 * File: GameTimerManager.cs
 * Summary: Manages all game loop timers and background processes
 * Purpose: Centralizes timer setup and management for cleaner MainWindow code
 * Notes: Handles game loop, auto-save, and background NPS accumulation
 */

using Avalonia.Threading;
using MusicClicker.Helpers;
using System;
using System.Diagnostics;

namespace MusicClicker.GameLogic.Systems
{
    /// <summary>
    /// Manages all game timing systems including game loop, auto-save, and background NPS accumulation.
    /// Provides clean separation of timing concerns from main window logic.
    /// </summary>
    public class GameTimerManager
    {
        // ------------------- TIMERS -------------------
        
        private DispatcherTimer gameLoopTimer;
        private Stopwatch gameLoopStopwatch;
        
        private System.Timers.Timer backgroundNpsTimer;
        private Stopwatch backgroundStopwatch;
        
        private DispatcherTimer autoSaveTimer;
        
        // ------------------- CALLBACKS -------------------
        
        public event Action? OnGameLoopTick;
        public event Action? OnAutoSave;
        public event Action<double>? OnBackgroundNpsUpdate; // Parameter: elapsed seconds
        
        // ------------------- CONSTRUCTOR -------------------
        
        public GameTimerManager()
        {
            gameLoopStopwatch = new Stopwatch();
            backgroundStopwatch = new Stopwatch();
        }
        
        // ------------------- INITIALIZATION -------------------
        
        /// <summary>
        /// Initializes all game timers with specified intervals.
        /// </summary>
        /// <param name="gameLoopIntervalMs">Game loop tick interval in milliseconds (default 100ms = 10Hz)</param>
        /// <param name="backgroundNpsIntervalMs">Background NPS calculation interval (default 50ms = 20Hz)</param>
        /// <param name="autoSaveIntervalMs">Auto-save interval in milliseconds (default 30000ms = 30 seconds)</param>
        public void Initialize(int gameLoopIntervalMs = 100, int backgroundNpsIntervalMs = 50, int autoSaveIntervalMs = 30000)
        {
            InitializeGameLoop(gameLoopIntervalMs);
            InitializeBackgroundNps(backgroundNpsIntervalMs);
            InitializeAutoSave(autoSaveIntervalMs);
        }
        
        /// <summary>
        /// Sets up the main game loop timer (runs on UI thread).
        /// </summary>
        private void InitializeGameLoop(int intervalMs)
        {
            gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalMs)
            };
            
            gameLoopTimer.Tick += (s, e) =>
            {
                // Measure elapsed time since last tick
                double elapsedSeconds = gameLoopStopwatch.Elapsed.TotalSeconds;
                gameLoopStopwatch.Restart();
                
                // Notify subscribers
                OnGameLoopTick?.Invoke();
            };
        }
        
        /// <summary>
        /// Sets up the background NPS accumulation timer (runs on background thread).
        /// </summary>
        private void InitializeBackgroundNps(int intervalMs)
        {
            backgroundNpsTimer = new System.Timers.Timer(intervalMs);
            backgroundNpsTimer.Elapsed += (s, e) =>
            {
                double elapsed = backgroundStopwatch.Elapsed.TotalSeconds;
                backgroundStopwatch.Restart();
                
                // Notify subscribers with elapsed time
                OnBackgroundNpsUpdate?.Invoke(elapsed);
            };
        }
        
        /// <summary>
        /// Sets up the auto-save timer (runs on UI thread).
        /// </summary>
        private void InitializeAutoSave(int intervalMs)
        {
            autoSaveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalMs)
            };
            
            autoSaveTimer.Tick += (s, e) =>
            {
                OnAutoSave?.Invoke();
            };
        }
        
        // ------------------- CONTROL METHODS -------------------
        
        /// <summary>
        /// Starts all game timers.
        /// </summary>
        public void StartAll()
        {
            gameLoopStopwatch.Start();
            gameLoopTimer?.Start();
            
            backgroundStopwatch.Start();
            backgroundNpsTimer?.Start();
            
            autoSaveTimer?.Start();
        }
        
        /// <summary>
        /// Stops all game timers.
        /// </summary>
        public void StopAll()
        {
            gameLoopTimer?.Stop();
            gameLoopStopwatch.Stop();
            
            backgroundNpsTimer?.Stop();
            backgroundStopwatch.Stop();
            
            autoSaveTimer?.Stop();
        }
        
        /// <summary>
        /// Disposes of all timers and resources.
        /// </summary>
        public void Dispose()
        {
            gameLoopTimer?.Stop();
            backgroundNpsTimer?.Stop();
            backgroundNpsTimer?.Dispose();
            autoSaveTimer?.Stop();
        }
    }
}
