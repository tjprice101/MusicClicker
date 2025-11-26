using System.Threading;
using System;

namespace MusicClicker.Helpers
{
    // Lightweight atomic operations for doubles using Interlocked.CompareExchange.
    // Provides Add and Read helpers so multiple threads can add/subtract the Notes
    // value without using locks.
    public static class AtomicDouble
    {
        // Atomically adds `value` to `location` and returns the new value.
        public static double Add(ref double location, double value)
        {
            while (true)
            {
                double original = Volatile.Read(ref location);
                double computed = original + value;
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
    }
}
