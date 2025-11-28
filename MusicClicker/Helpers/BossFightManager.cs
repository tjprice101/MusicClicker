using System;
using System.Collections.Generic;

namespace MusicClicker.Helpers
{
    public class BossFightManager
    {
        // Boss definitions
        public enum BossType
        {
            Mercury,
            Tonality,
            Mars
        }

        // Boss fight state
        public class BossFight
        {
            public BossType Type { get; set; }
            public string Name { get; set; }
            public string FullName { get; set; }
            public double RecommendedNPS { get; set; }
            public double BossHealth { get; set; }
            public double MaxBossHealth { get; set; }
            public List<string> WeakTo { get; set; } = new();
            public List<string> ResistantTo { get; set; } = new();
            public string RewardName { get; set; }
            public double RewardChance { get; set; }
            public bool IsActive { get; set; }
            public DateTime? LastWeeklyReset { get; set; }
            public int WeeklyCompletions { get; set; }
            public int MaxWeeklyCompletions { get; set; } = 3;
            public double FightTimeRemaining { get; set; }
            public double MaxFightTime { get; set; } = 30.0; // 30 seconds
            public DateTime FightStartTime { get; set; }
            
            // Special mechanics state
            public bool TonalityFreezeActive { get; set; }
            public DateTime TonalityFreezeExpiry { get; set; }
            public bool TonalityFreezeTriggered { get; set; }
        }

        private static BossFightManager? _instance;
        public static BossFightManager Instance => _instance ??= new BossFightManager();

        public Dictionary<BossType, BossFight> Bosses { get; private set; }
        public BossFight? CurrentFight { get; private set; }

        private BossFightManager()
        {
            InitializeBosses();
        }

        private void InitializeBosses()
        {
            Bosses = new Dictionary<BossType, BossFight>
            {
                {
                    BossType.Mercury,
                    new BossFight
                    {
                        Type = BossType.Mercury,
                        Name = "Mercury",
                        FullName = "Planetary Discordance: Mercury",
                        RecommendedNPS = 1_000_000_000_000, // 1 Trillion
                        MaxBossHealth = 100,
                        BossHealth = 100,
                        WeakTo = new List<string> { "Swan Lake", "Ode to Joy" },
                        ResistantTo = new List<string> { "Moonlight", "Eroica", "Enigma" },
                        RewardName = "Mercury, \"Winged Messenger\" Major",
                        RewardChance = 0.01 // 1% drop chance
                    }
                },
                {
                    BossType.Tonality,
                    new BossFight
                    {
                        Type = BossType.Tonality,
                        Name = "Tonality",
                        FullName = "Eternal Discordance: Tonality",
                        RecommendedNPS = 100_000_000_000_000, // 100 Trillion
                        MaxBossHealth = 100,
                        BossHealth = 100,
                        WeakTo = new List<string> { "Moonlight Sonata", "La Campanella", "Fate" },
                        ResistantTo = new List<string> { "Eroica", "Enigma", "Ode to Joy" },
                        RewardName = "Clair De Lune Major",
                        RewardChance = 0.01 // 1% drop chance
                    }
                },
                {
                    BossType.Mars,
                    new BossFight
                    {
                        Type = BossType.Mars,
                        Name = "Mars",
                        FullName = "Planetary Discordance: Mars",
                        RecommendedNPS = 1_000_000_000_000_000, // 1 Quadrillion
                        MaxBossHealth = 100,
                        BossHealth = 100,
                        WeakTo = new List<string> { "Ode to Joy", "Fate", "Dies Irae" },
                        ResistantTo = new List<string> { "Swan Lake", "Moonlight", "Eroica", "Enigma", "La Campanella" },
                        RewardName = "Mars Major",
                        RewardChance = 0.01 // 1% drop chance
                    }
                }
            };
        }

        public void StartFight(BossType bossType)
        {
            if (Bosses.TryGetValue(bossType, out var boss))
            {
                CurrentFight = boss;
                CurrentFight.BossHealth = CurrentFight.MaxBossHealth;
                CurrentFight.IsActive = true;
                CurrentFight.FightTimeRemaining = CurrentFight.MaxFightTime;
                CurrentFight.FightStartTime = DateTime.UtcNow;
                
                // Reset special mechanics
                CurrentFight.TonalityFreezeActive = false;
                CurrentFight.TonalityFreezeTriggered = false;
            }
        }

        public void EndFight()
        {
            if (CurrentFight != null)
            {
                CurrentFight.IsActive = false;
                CurrentFight = null;
            }
        }

        // Calculate damage multiplier based on current major score and weapons
        public double CalculateDamageMultiplier(string currentMajorScore, List<string> equippedWeapons)
        {
            if (CurrentFight == null) return 1.0;

            double multiplier = 1.0;

            // Normalize score name (remove "Major" suffix if present)
            string normalizedScore = currentMajorScore.Replace(" Major", "").Trim();

            // Check if current major score is weak/resistant
            if (CurrentFight.WeakTo.Contains(normalizedScore))
            {
                multiplier *= 2.0; // 2x damage if weak to current score
            }
            else if (CurrentFight.ResistantTo.Contains(normalizedScore))
            {
                multiplier *= 0.25; // 1/4 damage if resistant (significant penalty)
            }

            // Check weapons - weapon names in GameState don't have a consistent suffix, so check partial matches
            foreach (var weapon in equippedWeapons)
            {
                foreach (var weakScore in CurrentFight.WeakTo)
                {
                    if (weapon.Contains(weakScore, StringComparison.OrdinalIgnoreCase))
                    {
                        multiplier *= 1.5; // 1.5x damage for each weak weapon
                        break;
                    }
                }
                
                foreach (var resistantScore in CurrentFight.ResistantTo)
                {
                    if (weapon.Contains(resistantScore, StringComparison.OrdinalIgnoreCase))
                    {
                        multiplier *= 0.5; // 1/2 damage for each resistant weapon
                        break;
                    }
                }
            }

            return multiplier;
        }

        // Process a click during boss fight - reduces boss health
        public double ProcessClick(double baseNPS, double damageMultiplier)
        {
            if (CurrentFight == null || !CurrentFight.IsActive) return 0;

            // Check if Tonality freeze is active
            if (CurrentFight.Type == BossType.Tonality && CurrentFight.TonalityFreezeActive)
            {
                if (DateTime.UtcNow < CurrentFight.TonalityFreezeExpiry)
                {
                    return 0; // Can't damage during freeze
                }
                else
                {
                    CurrentFight.TonalityFreezeActive = false;
                }
            }

            // Mercury: Harder to damage early, easier later (inverse difficulty curve)
            double mercuryModifier = 1.0;
            if (CurrentFight.Type == BossType.Mercury)
            {
                double timeProgress = 1.0 - (CurrentFight.FightTimeRemaining / CurrentFight.MaxFightTime);
                mercuryModifier = 0.4 + (timeProgress * 0.6); // 0.4x at start, 1.0x at end
            }

            // Damage calculation - significantly reduced to require high NPS or weakness exploitation
            double baseDamage = (baseNPS / CurrentFight.RecommendedNPS) * 100.0; // Base damage per click
            double damage = baseDamage * damageMultiplier * mercuryModifier * 0.03; // Much lower multiplier (was 0.15)
            
            CurrentFight.BossHealth -= damage;
            if (CurrentFight.BossHealth < 0)
            {
                CurrentFight.BossHealth = 0;
            }

            return damage;
        }

        // Update fight timer and apply boss pushback
        public void UpdateFightTimer(double deltaTime)
        {
            if (CurrentFight == null || !CurrentFight.IsActive) return;

            // Update timer
            CurrentFight.FightTimeRemaining -= deltaTime;

            // Check if Tonality freeze is active
            if (CurrentFight.Type == BossType.Tonality && CurrentFight.TonalityFreezeActive)
            {
                return; // No pushback during freeze
            }

            // Boss pushes back (increases their health)
            double pushbackRate = 5.0; // Increased base pushback per second (was 2.0)
            
            // Mars: Becomes harder over time (stronger pushback)
            if (CurrentFight.Type == BossType.Mars)
            {
                double timeProgress = 1.0 - (CurrentFight.FightTimeRemaining / CurrentFight.MaxFightTime);
                pushbackRate *= (1.0 + timeProgress * 3.0); // 1x at start, 4x at end (was 3x)
            }

            double pushback = pushbackRate * deltaTime;
            CurrentFight.BossHealth += pushback;
            
            if (CurrentFight.BossHealth > CurrentFight.MaxBossHealth)
            {
                CurrentFight.BossHealth = CurrentFight.MaxBossHealth;
            }

            // Tonality special: Freeze at <50% health
            if (CurrentFight.Type == BossType.Tonality && 
                !CurrentFight.TonalityFreezeTriggered && 
                CurrentFight.BossHealth < CurrentFight.MaxBossHealth * 0.5)
            {
                CurrentFight.TonalityFreezeActive = true;
                CurrentFight.TonalityFreezeExpiry = DateTime.UtcNow.AddSeconds(5);
                CurrentFight.TonalityFreezeTriggered = true;
            }
        }

        public bool IsFightWon()
        {
            return CurrentFight != null && CurrentFight.BossHealth <= 0;
        }

        public bool IsFightLost()
        {
            return CurrentFight != null && CurrentFight.FightTimeRemaining <= 0;
        }

        // Check and reset weekly completions (Sunday 12:00 PM Eastern Time)
        public void CheckWeeklyReset()
        {
            DateTime now = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime nowEastern = TimeZoneInfo.ConvertTimeFromUtc(now, easternZone);
            
            foreach (var boss in Bosses.Values)
            {
                if (!boss.LastWeeklyReset.HasValue)
                {
                    boss.WeeklyCompletions = 0;
                    boss.LastWeeklyReset = now;
                    continue;
                }
                
                DateTime lastResetEastern = TimeZoneInfo.ConvertTimeFromUtc(boss.LastWeeklyReset.Value, easternZone);
                
                // Check if we've passed a Sunday 12:00 PM ET since last reset
                DateTime nextReset = GetNextSundayNoon(lastResetEastern);
                
                if (nowEastern >= nextReset)
                {
                    boss.WeeklyCompletions = 0;
                    boss.LastWeeklyReset = now;
                }
            }
        }
        
        private DateTime GetNextSundayNoon(DateTime fromDate)
        {
            // Find next Sunday at 12:00 PM from the given date
            DateTime candidate = fromDate.Date.AddHours(12);
            
            // Move to next Sunday
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)candidate.DayOfWeek + 7) % 7;
            if (daysUntilSunday == 0 && fromDate >= candidate)
            {
                daysUntilSunday = 7; // If already past noon Sunday, go to next Sunday
            }
            
            return candidate.AddDays(daysUntilSunday);
        }

        public bool CanFight(BossType bossType)
        {
            CheckWeeklyReset();
            if (Bosses.TryGetValue(bossType, out var boss))
            {
                return boss.WeeklyCompletions < boss.MaxWeeklyCompletions;
            }
            return false;
        }
        
        public int GetRemainingWeeklyCompletions(BossType bossType)
        {
            CheckWeeklyReset();
            if (Bosses.TryGetValue(bossType, out var boss))
            {
                return boss.MaxWeeklyCompletions - boss.WeeklyCompletions;
            }
            return 0;
        }

        public void IncrementWeeklyCompletion(BossType bossType)
        {
            if (Bosses.TryGetValue(bossType, out var boss))
            {
                boss.WeeklyCompletions++;
            }
        }

        public bool RollForReward()
        {
            if (CurrentFight == null) return false;
            
            Random random = new Random();
            return random.NextDouble() < CurrentFight.RewardChance;
        }
    }
}
