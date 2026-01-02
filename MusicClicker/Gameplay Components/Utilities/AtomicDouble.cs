using System.Threading;
using System;

namespace MusicClicker.Helpers
{
    // Lightweight atomic operations for doubles using Interlocked.CompareExchange.
    // Provides Add and Read helpers so multiple threads can add/subtract the Notes
    // value without using locks.
    public static class AtomicDouble
    {
        // Maximum safe value - cap at 1e300 to prevent overflow to infinity
        // This leaves headroom below double.MaxValue (~1.79e308)
        public const double MaxSafeValue = 1e300;
        
        // Atomically adds `value` to `location` and returns the new value.
        // Clamps the result to MaxSafeValue to prevent infinity.
        public static double Add(ref double location, double value)
        {
            // Pre-check: if value is infinity or NaN, clamp it
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                value = MaxSafeValue * 0.01; // Cap ridiculous values at 1% of max
            }
            
            while (true)
            {
                double original = Volatile.Read(ref location);
                double computed = original + value;
                
                // Clamp result to prevent overflow to infinity
                if (computed > MaxSafeValue)
                {
                    computed = MaxSafeValue;
                }
                else if (double.IsInfinity(computed) || double.IsNaN(computed))
                {
                    computed = MaxSafeValue;
                }
                
                double prev = Interlocked.CompareExchange(ref location, computed, original);
                if (prev == original) return computed;
                // otherwise retry
            }
        }

        // Read the current value (volatile read)
        public static double Read(ref double location)
        {
            return Volatile.Read(ref location);
        }
        
        // Helper to safely compute values with overflow protection
        public static double SafeValue(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value) || value > MaxSafeValue)
            {
                return MaxSafeValue;
            }
            return value;
        }
    }
}
