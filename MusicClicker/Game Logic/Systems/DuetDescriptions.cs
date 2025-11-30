using System;

namespace MusicClicker
{
    /// <summary>
    /// Single source of truth for all duet ability descriptions.
    /// Update descriptions here and they will automatically propagate to all screens.
    /// </summary>
    public static class DuetDescriptions
    {
        // Duet Names
        public static class Names
        {
            public const string Moonlight = "Moonlight Sonata:\nLunar Phases";
            public const string DiesIrae = "Descending Judgment:\nSeven Seals";
            public const string Winter = "Eternal Frost:\nAbsolute Zero";
            public const string Eroica = "Symphony of Heroes:\nVictory March";
            public const string SwanLake = "Reflection of Grace:\nMirror Lake";
            public const string LaCampanella = "Bell Resonance:\nChime Chain";
            public const string Enigma = "Enigmatic Puzzle:\nMystery Clicks";
            public const string Fate = "Threads of Destiny:\nHourglass Reversal";
            public const string OdeToJoy = "Symphony of Celebration:\nCrescendo Conductor";
        }

        // Duet Descriptions (Full format for DuetAbilityScreen)
        public static class Full
        {
            public const string Moonlight = "Cycles through 4 moon phases every 5s:\n• New Moon: 2× NPC\n• Crescent: 10% component drop per click\n• Full Moon: 3× NPS\n• Waning: Upgrades 50% off\n\nSPECIAL: All phases active simultaneously when resonating with Moonlight Major!\n\n20s duration | 4min cooldown";
            
            public const string DiesIrae = "Each click places a seal. At 7 seals, grants 3 random minor scores without consuming components. Stacks during duration.\n\n15s duration | 3.5min cooldown";
            
            public const string Winter = "Converts frozen NPS into click multiplier. Each click extends duration by 0.5s (max +10s).\n\n15s base duration | 5min cooldown";
            
            public const string Eroica = "Fill bar with 100 clicks:\n• 25%: Random minor score\n• 50%: Random major score\n• 75%: +50 Fragments\n• 100%: Double all scores\n\n5min cooldown";
            
            public const string SwanLake = "Every action reflects and happens again after 3 seconds. Affects clicks, crafts, upgrades, and fragments.\n\n20s duration | 4min cooldown";
            
            public const string LaCampanella = "Click to create chimes. Clicking within 1s extends chain. Chain gives length² × NPS as instant notes. Breaks after 1s of no clicks.\n\n10s duration | 10min cooldown";
            
            public const string Enigma = "Each click triggers 1 random effect:\nRed: 5× NPS notes | Blue: 10% NPS boost | Green: 30 Harmonious | Yellow: 30 Melodious | Purple: +1 major per owned | Orange: +1 minor component per owned | White: +1 to 3 random minors | Black: -65% notes\n\n10s duration | 30min cooldown";
            
            public const string Fate = "All actions performed during the first 10 seconds will be replayed during the last 10 seconds at X× effectiveness, where X equals the number of clicks you performed.\n\nDuration: 20s | Cooldown: 8min";
            
            public const string OdeToJoy = "Build 16-note melodies by clicking (repeatable). Every 16 notes resets and grants rewards:\n• 4 notes: +25 Fragments\n• 8 notes: Random minor score\n• 12 notes: Random major sheet\n• 16 notes: 5× NPS for 15s\nOn expiry: +3 Entropic Melody per completed section\n\n20s duration | 10min cooldown";
        }

        // Compact Descriptions (for TempoResonateScreen)
        public static class Compact
        {
            public const string Moonlight = "Moonlight Sonata - Lunar Phases: Cycles through 4 moon phases every 5s (New Moon: 2× NPC, Crescent: 10% component drop per click, Full Moon: 3× NPS, Waning: Upgrades 50% off). When resonating with Moonlight Major, all phases active simultaneously! - 20s duration, 4min cooldown";
            
            public const string DiesIrae = "Descending Judgment - Seven Seals: Each click places a seal. At 7 seals, grants 3 random minor scores without consuming components. Stacks during duration (15s duration, 3.5min cooldown)";
            
            public const string Winter = "Eternal Frost - Absolute Zero: Converts frozen NPS into click multiplier. Each click extends duration by 0.5s (max +10s, 15s base duration, 5min cooldown)";
            
            public const string Eroica = "Symphony of Heroes - Victory March: Fill bar with 100 clicks (25%: random minor score, 50%: random major score, 75%: +50 Fragments, 100%: double all scores) - 5min cooldown";
            
            public const string SwanLake = "Reflection of Grace - Mirror Lake: Every action reflects and happens again after 3s. Affects clicks, crafts, upgrades, and fragments (20s duration, 4min cooldown)";
            
            public const string LaCampanella = "Bell Resonance - Chime Chain: Click to create chimes. Clicking within 1s extends chain. Chain gives length² × NPS as instant notes. Breaks after 1s of no clicks (10s duration, 10min cooldown)";
            
            public const string Enigma = "Enigmatic Puzzle - Mystery Clicks: Each click triggers 1 random effect (Red: 5× NPS notes, Blue: 10% NPS boost, Green: 30 Harmonious, Yellow: 30 Melodious, Purple: +1 major per owned, Orange: +1 minor component per owned, White: +1 to 3 random minors, Black: -65% notes) - 10s duration, 30min cooldown";
            
            public const string Fate = "Fate Duet - Hourglass Reversal: All actions during first 10s replay during last 10s at X× effectiveness where X = clicks performed (20s duration, 8min cooldown)";
            
            public const string OdeToJoy = "Ode to Joy Duet - Crescendo Conductor: Build 16-note melodies (repeatable/stackable). Rewards at 4/8/12/16 notes (+25 fragments, minor score, major sheet, 5× NPS for 15s). On expiry: +3 Entropic Melody per completed section - 20s duration, 10min cooldown";
        }

        // Short Descriptions (for ArmoryOfForteScreen)
        public static class Short
        {
            public const string Moonlight = "Lunar Phases: Cycles through 4 moon phases every 5s (New Moon: 2x NPC, Crescent: 10% component drop per click, Full Moon: 3x NPS, Waning: Upgrades 50% off). When resonating with Moonlight Major, all phases active simultaneously! - 20s duration, 4min cooldown.";
            
            public const string DiesIrae = "Seven Seals: Each click places a seal. At 7 seals, grants 3 random minor scores without consuming components. Stacks during duration (15s duration, 3.5min cooldown).";
            
            public const string Winter = "Duet Resonance: Activate to convert your frozen NPS into a click multiplier. Each click extends duration by 0.5s (max +10s, 15s base duration, 5min cooldown).";
            
            public const string Eroica = "Victory March: Fill bar with 100 clicks for escalating rewards (25%: minor score, 50%: major score, 75%: +50 fragments, 100%: double all scores) - 5min cooldown.";
            
            public const string SwanLake = "Mirror Lake: Every action is reflected and happens again after 3 seconds (clicks, crafts, upgrades, fragments) - 20s duration, 4min cooldown.";
            
            public const string LaCampanella = "Chime Chain: First click creates a chime. Each click within 1 second extends the chain. Chain gives length² × NPS as instant notes. Chain breaks after 1s of no clicks - 10s duration, 10min cooldown.";
            
            public const string Enigma = "Mystery Clicks: Each click triggers 1 random effect (Red: 5× NPS notes, Blue: 10% NPS boost, Green: 30 Harmonious, Yellow: 30 Melodious, Purple: +1 major per owned, Orange: +1 minor component per owned, White: +1 to 3 random minors, Black: -65% notes) - 10s duration, 30min cooldown.";
            
            public const string Fate = "Hourglass Reversal: All actions during first 10s replay during last 10s at X× effectiveness where X = clicks performed (20s duration, 8min cooldown).";
            
            public const string OdeToJoy = "Crescendo Conductor: Build 16-note melodies (repeatable/stackable). Rewards at 4/8/12/16 notes (+25 fragments, minor score, major sheet, 5× NPS for 15s). On expiry: +3 Entropic Melody per completed section - 20s duration, 10min cooldown.";
        }

        // Duration Constants (in seconds)
        public static class Duration
        {
            public const int Moonlight = 20;
            public const int DiesIrae = 15;
            public const int Winter = 15;
            public const int SwanLake = 20;
            public const int LaCampanella = 10;
            public const int Enigma = 10;
            public const int Fate = 20;
            public const int OdeToJoy = 20;
        }

        // Cooldown Constants (in seconds)
        public static class Cooldown
        {
            public const int Moonlight = 240;      // 4 minutes
            public const int DiesIrae = 210;       // 3.5 minutes
            public const int Winter = 300;         // 5 minutes
            public const int Eroica = 300;         // 5 minutes
            public const int SwanLake = 240;       // 4 minutes
            public const int LaCampanella = 600;   // 10 minutes
            public const int Enigma = 1800;        // 30 minutes
            public const int Fate = 480;           // 8 minutes
            public const int OdeToJoy = 600;       // 10 minutes
        }
    }
}
