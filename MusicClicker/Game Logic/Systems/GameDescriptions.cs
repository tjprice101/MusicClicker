using System.Collections.Generic;

namespace MusicClicker
{
    /// <summary>
    /// CENTRAL SOURCE OF TRUTH for all in-game text descriptions.
    /// Update descriptions here and they will propagate to all screens automatically.
    /// </summary>
    public static class GameDescriptions
    {
        // ============================================================
        // MAJOR SCORES
        // ============================================================
        
        public static class MajorScores
        {
            public const string MoonlightSonata = @"Moonlight Sonata Major

Major Ability:
Doubles the power of all Moonlight-themed bonuses and effects.

Crescendance: Eclipse of the Nocturne
• Every 20th click during nighttime (8PM-6AM): Gain 1 stack of ""Moonbeam Resonance""
• At 5 stacks: Auto-consume for +250% of current notes and 1 ""Harmonizing Moonlight"" stack
• Empowers both Moonlight weapons with unique crescendance abilities";

            public const string Eroica = @"Eroica (Symphony No. 3)

Major Ability:
Minor scores beyond the 10th grant double NPS each.

Crescendance: Heroic Momentum
• Every minor craft: +1 Heroic Resolve stack
• Consume 1 Heroic Resolve + 25% notes: +1 Symphonic Catharsis stack
• Consume Symphonic Catharsis: 10s of double NPC + 10% crit chance
• Triggers Sakura weapon bond (Crimson Requiem: 30 clicks with bonus = NPC + (NPS × NPC) each)
• Triggers Funeral Prayer weapon bond (Testament of Harmony: +1 stack, consume for 100 Melodious + 100 Harmonious fragments)";

            public const string SwanLake = @"Swan Lake

Crescendance: Three Feather System
• Revered Feathers (every 10 clicks, 5%): Consume 5 for +20% notes
• Chromatic Feathers (every 15 clicks, 5%): Consume 10 for +2 all minors
• Polyphonic Feathers (every 30 clicks, 5%): Consume 1 for +250 entropic +75% notes";

            public const string LaCampanella = @"La Campanella

Crescendance: Grandiose Bell
• Bell cracks at 20/40/60 clicks, advancing stage (Intact → Crescending → Radiant → Harmonizing)
• Mend to collect rewards:
  - Crescending: +2 random owned minors
  - Radiant: Next 5 clicks are entropic crits + 5 Deafening Chime stacks
  - Harmonizing: Consume all Deafening Chime for 2^stacks multiplier to notes
• Symphony weapon bond: +75% notes when bell cracks
• Razer weapon bond: Mend grants entropic melodies equal to click count (max 250)";

            public const string EnigmaVariations = @"Enigma Variations

Crescendance: Resonate Mystery
• Every 10th click: +1 Resonate Mystery stack
• Every 25th click: +1 additional Resonate Mystery stack (with Creator of Mystery weapon)
• Consume 1 stack for:
  - +50% of current notes
  - +50 Entropic Melodies
  - +1 random owned minor (Truthseeker weapon bond grants 1 random minor per consumption)
• Bulk consume (10+ stacks): +25% notes per stack consumed + Truthseeker bonus minors";

            public const string Fate = @"Fate (Symphony No. 5)

Crescendance: Cosmic Modulation
• Every 5th click: +1 Cosmic Modulation stack + 10% notes bonus
• Tiers (every 5 stacks): Grant escalating passive effects
• Consume all Cosmic Modulation: +15 Entropic Melodies per stack
• Astral Chainripper weapon bond: Every 5 stacks gained → +1 Symphony of the Stars
• Symphony of the Stars: Consume 1 stack for +3 to lowest owned minor score
• Cosmic Weaver weapon bond: Each Symphony consumed → next 5 clicks are guaranteed entropic crits";

            public const string OdeToJoy = @"Ode to Joy (Symphony No. 9)

Crescendance: Petals of Life
• Minor craft: +1 Petal of Harmony
• Major craft: +1 Petal of Melody
• Consume Harmony: +250 Entropic Melodies
• Consume Melody: +10s full entropic crits (stackable)
• Combine (1 Harmony + 1 Melody + 50 Entropic): 1 Ode to Life
• Consume Ode to Life: Double all owned minors";

            public const string DiesIrae = @"Dies Irae

Crescendance: Symphony of Hell's Retribution
• Every click: +1 Burning Hatred (up to 50), then +1 Discordant Malice
• Consume 5 Burning Hatred: +1 Dissonant Oblivion
• Consume Discordant Malice: +X Entropic (X = Burning Hatred stacks)
• Consume Dissonant Oblivion: Next 20 clicks are Symphony of Hell's Retribution
• Hell crit formula: NPC × NPS × Burning Hatred stacks";

            public const string Winter = @"Winter

Crescendance: Eternal Frost
Freezes NPS and converts it into click multiplier. Clicks extend duration.";
        }

        // ============================================================
        // WEAPONS - FULL DESCRIPTIONS (for Tempo Resonate tooltips)
        // ============================================================
        
        public static class Weapons
        {
            // Moonlight Weapons
            public const string IncisorOfMoonlight = @"Incisor of Moonlight

Passive: Moonlight Strike (while equipped)
Every 4th click grants bonus notes based on current total. Doubles during nighttime (8PM-6AM).
• +5% bonus notes of current total (+20% at night)
• Trigger: Every 4th click

Crescendance Bond: Piercing Radiance
Moonbeam Resonance stacks amplify your note gains when using Moonlight Sonata Major.
• Each stack gained: +20% additional notes of current notes
• Requires: Moonlight Sonata Major resonated + Incisor equipped

Duet (with Eulogy): Lunar Phases
Cycles through moon phases granting different bonuses. With Moonlight Major, all phases activate simultaneously!
• New Moon: 2× NPC | Crescent: 10% component drop | Full Moon: 3× NPS | Waning: 50% upgrade discount
• Duration: 20s | Cooldown: 4min";

            public const string EulogyOfTheMoon = @"Eulogy of the Moon

Passive: Nocturnal Refund (while equipped)
Crafting minor scores has a chance to refund all materials. Stronger during nighttime.
• 25% refund chance (50% during nighttime 8PM-6AM)
• Trigger: On minor score craft

Crescendance Bond: Requiem of Renewal
Convert Harmonizing Moonlight stacks into Moonbeam Resonance plus crafting materials.
• Consume 1 Harmonizing Moonlight: Gain 3 Moonbeam Resonance + 1 of each component for lowest owned minor
• Requires: Moonlight Sonata Major resonated + Eulogy equipped

Duet (with Incisor): Lunar Phases
Cycles through moon phases granting different bonuses. With Moonlight Major, all phases activate simultaneously!
• New Moon: 2× NPC | Crescent: 10% component drop | Full Moon: 3× NPS | Waning: 50% upgrade discount
• Duration: 20s | Cooldown: 4min";

            // Eroica Weapons
            public const string SakurasBlossom = @"Sakura's Blossom

Forte Resonance: Heroic Bounty (while equipped)
Acquiring Majors (upgrades) rewards you with crafting materials for that score type.
• On Major acquisition: +1 of each component (Keys, Scales, Progressions) for that Major
• Trigger: When purchasing upgrades

Crescendance Bond: Crimson Requiem
Consuming Symphonic Catharsis triggers a special burst where clicks are supercharged.
• Next 30 clicks: Bonus notes = NPC + (NPS × NPC) each
• Requires: Eroica Major resonated + Sakura equipped
• Trigger: When Symphonic Catharsis is consumed

Duet (with Funeral Prayer): Victory March
Fill a 100-click bar to earn escalating rewards, culminating in doubling all scores!
• 25%: Random minor | 50%: Random major | 75%: +50 fragments | 100%: Double all scores
• Cooldown: 5min";

            public const string FuneralPrayer = @"Funeral Prayer

Forte Resonance: Prayer of Valor (while equipped)
Build Prayer stacks through clicking, then unleash them for massively boosted clicks.
• Every 10th click: +1 Prayer stack
• At 3 stacks: Next 15 clicks gain +6× NPS each
• Trigger: Every 10th click

Crescendance Bond: Testament of Harmony
Consuming Symphonic Catharsis grants Testament stacks that can be redeemed for fragments.
• When Symphonic Catharsis consumed: +1 Testament of Harmony
• Consume 1 Testament: +100 Melodious + 100 Harmonious Fragments
• Requires: Eroica Major resonated + Funeral Prayer equipped

Duet (with Sakura's Blossom): Victory March
Fill a 100-click bar to earn escalating rewards, culminating in doubling all scores!
• 25%: Random minor | 50%: Random major | 75%: +50 fragments | 100%: Double all scores
• Cooldown: 5min";

            // Swan Lake Weapons
            public const string StarScatteredWings = @"Star-Scattered Wings

Forte Resonance: Stellar Fragment Rain (while equipped)
Regular clicks generate crafting fragments. Feather collection grants bonus components.
• Every 10th click: +5 Melodious & +5 Harmonious Fragments
• Feather bonus: +2 components to random owned minor when feathers drop
• Trigger: Every 10th click, feather acquisitions

Duet (with Thousand Winged Swan): Feather Cascade
First clicks rain all feather types, with milestone clicks granting note bonuses.
• First 10 clicks: Each grants 1 Revered, 1 Chromatic, 1 Polyphonic Feather
• Every 5th click (5th & 10th): +25% notes bonus
• Duration: 20s | Cooldown: 4min";

            public const string ThousandWingedSwan = @"Thousand Winged Swan

Forte Resonance: Wings of Fortune (while equipped)
Immediate note boost when acquired. Polyphonic feathers temporarily convert NPS to NPC.
• On acquisition: +50% notes instantly (one-time)
• Polyphonic feather consumed: 10s of NPS-to-NPC boost (adds 2× NPS to NPC)
• Trigger: On weapon acquisition, Polyphonic feather consumption

Duet (with Star-Scattered Wings): Feather Cascade
First clicks rain all feather types, with milestone clicks granting note bonuses.
• First 10 clicks: Each grants 1 Revered, 1 Chromatic, 1 Polyphonic Feather
• Every 5th click (5th & 10th): +25% notes bonus
• Duration: 20s | Cooldown: 4min";

            // La Campanella Weapons
            public const string SymphonyOfBells = @"Symphony of Bells

Passive: Harmonic Duplication (while equipped)
Crafting minor scores creates duplicates, doubling your gain.
• On minor craft: Duplicate that minor (+1 extra copy)
• Trigger: Every minor score craft

Crescendance Bond: Resonant Crack Bonus
Grandiose Bell cracking releases burst damage as instant notes.
• When bell cracks (20/40/60 clicks): +75% notes instantly
• Requires: La Campanella Major resonated + Symphony equipped
• Trigger: Bell crack events

Duet (with Razer of Bell's Chimes): Chime Chain
Build click chains within 1-second windows for exponentially scaling note rewards!
• Reward: (chain length)² × NPS as instant notes
• Chain breaks after 1s without clicking
• Duration: 10s | Cooldown: 10min";

            public const string RazerOfBellsChimes = @"Razer of Bell's Chimes

Passive: Component Echo (while equipped)
Crafting minor scores refunds some materials, reducing resource costs.
• On minor craft: Refund 2 random components for that minor
• Trigger: Every minor score craft

Crescendance Bond: Entropic Resonance
Mending the Grandiose Bell converts accumulated clicks into Entropic Melodies.
• On bell mend: Gain Entropic Melodies = click count (max 250)
• Requires: La Campanella Major resonated + Razer equipped
• Trigger: Mend bell action

Duet (with Symphony of Bells): Chime Chain
Build click chains within 1-second windows for exponentially scaling note rewards!
• Reward: (chain length)² × NPS as instant notes
• Chain breaks after 1s without clicking
• Duration: 10s | Cooldown: 10min";

            // Enigma Weapons  
            public const string CreatorOfMystery = @"Creator of Mystery

Passive: Chaotic Flux (while equipped)
Clicks randomly add or subtract notes, favoring positive outcomes.
• Every 3rd click: ±25% notes (60% chance positive, 40% chance negative)
• Trigger: Every 3rd click

Crescendance Bond: Accelerated Mystery
Additional mystery stack generation from frequent clicking accelerates your power growth.
• Every 25th click: +1 additional Resonate Mystery stack (on top of base every-10th)
• Requires: Enigma Major resonated + Creator equipped
• Trigger: Every 25th click

Duet (with Truthseeker): Mystery Clicks
Each click triggers a random powerful effect - risk and reward in every tap!
• Red: 5× NPS notes | Blue: 10% NPS boost | Green: 30 Harmonious | Yellow: 30 Melodious
• Purple: +1 major per owned | Orange: +1 component per owned minor | White: +1-3 random minors | Black: -65% notes
• Duration: 10s | Cooldown: 30min";

            public const string Truthseeker = @"Truthseeker

Passive: Revelation Burst (while equipped)
Acquiring Majors (upgrades) reveals hidden knowledge, granting extra fragments.
• On Major acquisition: +10% of total fragments
• Trigger: When purchasing upgrades

Crescendance Bond: Knowledge Harvest
Every mystery you solve grants additional minor scores, building your collection.
• Each Resonate Mystery consumed: +1 random owned minor score
• Requires: Enigma Major resonated + Truthseeker equipped
• Trigger: Resonate Mystery consumption

Duet (with Creator of Mystery): Mystery Clicks
Each click triggers a random powerful effect - risk and reward in every tap!
• Red: 5× NPS notes | Blue: 10% NPS boost | Green: 30 Harmonious | Yellow: 30 Melodious
• Purple: +1 major per owned | Orange: +1 component per owned minor | White: +1-3 random minors | Black: -65% notes
• Duration: 10s | Cooldown: 30min";

            // Fate Weapons
            public const string AstralChainripper = @"Astral Chainripper

Passive: Temporal Surge (while equipped)
Crafting Fate minors triggers a temporal surge, massively boosting NPS temporarily.
• On Fate minor craft: 5× NPS for 10s
• Trigger: Crafting Fate minor scores

Crescendance Bond: Stellar Convergence
Cosmic Modulation stacks converge into Symphony of the Stars at regular intervals.
• Every 5 Cosmic Modulation stacks gained: +1 Symphony of the Stars
• Requires: Fate Major resonated + Astral Chainripper equipped
• Trigger: Gaining Cosmic Modulation stacks

Duet (with Cosmic Weaver): Hourglass Reversal
Actions in the first half replay in the second half, multiplied by your click count!
• First 10s: Bank all actions
• Last 10s: Replay at X× effectiveness (X = number of clicks performed)
• Duration: 20s | Cooldown: 8min";

            public const string CosmicWeaver = @"Cosmic Weaver

Passive: Harmonic Multiplication (while equipped)
Crafting minors when you own their major score grants bonus major scores.
• On any minor craft: If you own 1+ of that major, gain +3 of that major score
• Trigger: Crafting minor scores

Crescendance Bond: Entropic Starfall
Symphony consumption triggers guaranteed critical streaks for massive burst damage.
• Each Symphony of the Stars consumed: Next 5 clicks are guaranteed entropic crits
• Requires: Fate Major resonated + Cosmic Weaver equipped
• Trigger: Symphony of the Stars consumption

Duet (with Astral Chainripper): Hourglass Reversal
Actions in the first half replay in the second half, multiplied by your click count!
• First 10s: Bank all actions
• Last 10s: Replay at X× effectiveness (X = number of clicks performed)
• Duration: 20s | Cooldown: 8min";

            // Ode to Joy Weapons
            public const string JoyfulCatharsis = @"Joyful Catharsis

Passive: Harmonic Entropy (while equipped)
Frequent clicks build up entropic energy, released based on your critical performance.
• Every 50th click: +X Entropic Melodies (X = critical notes gotten × 3)
• Trigger: Every 50th click

Crescendance Bond: Euphoric Resonance
Gaining Petals of Life creates euphoric bursts that temporarily double your passive income.
• Every time you gain any Petal: Double NPS for 5s (effect stacks with multiple petals)
• Requires: Ode to Joy Major resonated + Joyful Catharsis equipped
• Trigger: Petal acquisition

Duet (with Ode to Creation): Crescendo Conductor
Build musical melodies through clicking, earning escalating rewards and entropic bonuses!
• 4 notes: +25 fragments | 8 notes: Random minor | 12 notes: Major sheet | 16 notes: 5× NPS for 15s (repeatable)
• On expiry: +3 Entropic Melody per completed 4-note section
• Duration: 20s | Cooldown: 10min";

            public const string OdeToCreation = @"Ode to Creation

Passive: Rhythmic Genesis (while equipped)
Regular clicking creates Petals of Life randomly, fueling the Crescendance system.
• Every 20th click: Generate random Petal of Harmony or Petal of Melody
• Trigger: Every 20th click

Crescendance Bond: Life Amplification
Consuming Ode to Life supercharges petal generation for a limited time.
• On Ode to Life consume: Double passive effect for 25s (generates petals every 10th click instead)
• Requires: Ode to Joy Major resonated + Ode to Creation equipped
• Trigger: Ode to Life consumption

Duet (with Joyful Catharsis): Crescendo Conductor
Build musical melodies through clicking, earning escalating rewards and entropic bonuses!
• 4 notes: +25 fragments | 8 notes: Random minor | 12 notes: Major sheet | 16 notes: 5× NPS for 15s (repeatable)
• On expiry: +3 Entropic Melody per completed 4-note section
• Duration: 20s | Cooldown: 10min";
        }

        // ============================================================
        // FORTE RESONANCE (Armory screen - individual weapon effects)
        // ============================================================
        
        public static class ForteResonance
        {
            // Moonlight
            public const string IncisorOfMoonlight = "Passive: Moonlight Strike - Every 4th click grants +5% bonus notes (+20% during nighttime 8PM-6AM). Crescendance Bond: Piercing Radiance - Each Moonbeam Resonance stack gained grants +20% additional notes of current notes.";
            public const string EulogyOfTheMoon = "Passive: Nocturnal Refund - On minor score craft: 25% chance to refund all materials (50% during nighttime 8PM-6AM). Crescendance Bond: Requiem of Renewal - Consume 1 Harmonizing Moonlight stack to gain 3 Moonbeam Resonance stacks plus components.";
            
            // Eroica
            public const string SakurasBlossom = "Forte Resonance: Heroic Bounty - On Major (upgrade) acquisition, grants 1 of each component type (Keys, Scales, Progressions) for that Major score. Crescendance Bond: Crimson Requiem - When Symphonic Catharsis is consumed, next 30 clicks gain bonus notes.";
            public const string FuneralPrayer = "Forte Resonance: Prayer of Valor - Every 10th click grants 1 Prayer stack. At 3 Prayer stacks: next 15 clicks each gain +6× Notes Per Second. Crescendance Bond: Testament of Harmony - When Symphonic Catharsis is consumed, grants +1 Testament of Harmony (consume for +100 Melodious + 100 Harmonious Fragments).";
            
            // Swan Lake
            public const string StarScatteredWings = "Forte Resonance: Stellar Fragment Rain - Every 10th click grants +5 Melodious Fragments and +5 Harmonious Fragments. Feather acquisitions grant +2 components to random owned minor score.";
            public const string ThousandWingedSwan = "Forte Resonance: Wings of Fortune - On weapon acquisition grants +50% notes instantly (one-time). When Polyphonic Feather is consumed: gain 10 seconds of Notes Per Second-to-Notes Per Click boost.";
            
            // La Campanella
            public const string SymphonyOfBells = "Passive: Harmonic Duplication - On minor score craft: duplicate that minor score (+1 extra copy). Crescendance Bond: Resonant Crack Bonus - When Grandiose Bell cracks (at 20/40/60 clicks): instantly gain +75% notes.";
            public const string RazerOfBellsChimes = "Passive: Component Echo - On minor score craft: refund 2 random components for that minor. Crescendance Bond: Entropic Resonance - On Grandiose Bell mend: gain Entropic Melodies equal to click count (maximum 250).";
            
            // Enigma
            public const string CreatorOfMystery = "Passive: Chaotic Flux - Every 3rd click: randomly +25% or -25% notes (60% chance positive). Crescendance Bond: Accelerated Mystery - Every 25th click: gain +1 additional Resonate Mystery stack (on top of base every-10th).";
            public const string Truthseeker = "Passive: Revelation Burst - On Major (upgrade) acquisition: gain +10% of total fragments. Crescendance Bond: Knowledge Harvest - Each Resonate Mystery stack consumed: gain +1 random owned minor score.";
            
            // Fate
            public const string AstralChainripper = "Passive: Temporal Surge - On Fate minor score craft: multiply Notes Per Second by 5× for 10 seconds. Crescendance Bond: Stellar Convergence - Every 5 Cosmic Modulation stacks gained: automatically grants +1 Symphony of the Stars stack.";
            public const string CosmicWeaver = "Passive: Harmonic Multiplication - On any minor score craft: If you own 1+ of that major score, gain +3 of that major score. Crescendance Bond: Entropic Starfall - Each Symphony of the Stars stack consumed: next 5 clicks are guaranteed Entropic Melody critical hits.";
            
            // Ode to Joy
            public const string JoyfulCatharsis = "Passive: Harmonic Entropy - Every 50th click: gain Entropic Melodies equal to (critical notes gotten × 3). Crescendance Bond: Euphoric Resonance - Every time you gain any Petal: double Notes Per Second for 5 seconds (effect stacks).";
            public const string OdeToCreation = "Passive: Rhythmic Genesis - Every 20th click: generate random Petal of Harmony or Petal of Melody. Crescendance Bond: Life Amplification - When Ode to Life is consumed: double passive effect for 25 seconds (generates petals every 10th click instead).";
            
            // Dies Irae
            public const string SevenCircles = "Passive: Wrathful Amplification - Every critical hit: gain +20% notes. Crescendance Bond: Seal Resonance - Symphony of Hell's Retribution critical hits grant +1 Wrathful Seal. Consume Wrathful Seal for 5 Seal-breaking clicks.";
            public const string HellsWrath = "Passive: Infernal Bounty - Every 50th click: grant +2 to your 2 lowest owned minor scores. Crescendance Bond: Entropic Fury - Hell's Retribution critical hits grant +200 Entropic Melodies.";
            
            // Winter
            public const string CacophonicBlizzard = "Crystalline Shatter - Every 10th click: instantly gain 10 seconds worth of Notes Per Second as notes.";
            public const string TheSnowsDesire = "Blizzard's Bounty - On Harmonious Fragment purchase: gain +2% Notes Per Second for 30 seconds (effect stacks).";
        }

        // ============================================================
        // DUET ABILITIES
        // ============================================================
        
        public static class Duets
        {
            public static class Names
            {
                public const string Moonlight = "Moonlight Sonata:\nLunar Phases";
                public const string DiesIrae = "Descending Judgment:\nSeven Seals";
                public const string Winter = "Eternal Frost:\nAbsolute Zero";
                public const string Eroica = "Symphony of Heroes:\nVictory March";
                public const string SwanLake = "Swan Lake Crescendance:\nFeather Cascade";
                public const string LaCampanella = "Bell Resonance:\nChime Chain";
                public const string Enigma = "Enigmatic Puzzle:\nMystery Clicks";
                public const string Fate = "Threads of Destiny:\nHourglass Reversal";
                public const string OdeToJoy = "Symphony of Celebration:\nCrescendo Conductor";
            }

            public static class Full
            {
                public const string Moonlight = "Cycles through 4 moon phases every 5 seconds:\n• New Moon: 2× Notes Per Click\n• Crescent: 10% component drop chance per click\n• Full Moon: 3× Notes Per Second\n• Waning: Upgrades cost 50% less\n\nSPECIAL: All phases active simultaneously when resonating with Moonlight Sonata Major!\n\n20 second duration | 4 minute cooldown";
                public const string DiesIrae = "Each click places a seal. At 7 seals placed, grants 3 random minor scores without consuming components. Effect stacks during duration.\n\n15 second duration | 3.5 minute cooldown";
                public const string Winter = "Converts frozen Notes Per Second into click multiplier. Each click extends duration by 0.5 seconds (maximum +10 seconds total extension).\n\n15 second base duration | 5 minute cooldown";
                public const string Eroica = "Fill progress bar with 100 clicks:\n• 25% progress: Random minor score\n• 50% progress: Random major score\n• 75% progress: +50 Melodious and Harmonious Fragments\n• 100% complete: Double all owned scores\n\n5 minute cooldown";
                public const string SwanLake = "Each of your first 10 clicks grants 1 Revered Feather, 1 Chromatic Feather, and 1 Polyphonic Feather. Every 5th click (5th and 10th) increases current notes by +25%.\n\n20 second duration | 4 minute cooldown";
                public const string LaCampanella = "Click to create chimes. Clicking within 1 second extends chain. Chain reward equals (chain length)² × Notes Per Second as instant notes. Chain breaks after 1 second of no clicks.\n\n10 second duration | 10 minute cooldown";
                public const string Enigma = "Each click triggers 1 random effect:\nRed: 5× Notes Per Second as instant notes | Blue: +10% Notes Per Second boost | Green: +30 Harmonious Fragments | Yellow: +30 Melodious Fragments | Purple: +1 major score per owned type | Orange: +1 minor component per owned minor type | White: +1 to 3 random minor scores | Black: -65% current notes\n\n10 second duration | 30 minute cooldown";
                public const string Fate = "All actions performed during the first 10 seconds will be replayed during the last 10 seconds at X× effectiveness, where X equals the number of clicks you performed during the first half.\n\nDuration: 20 seconds | Cooldown: 8 minutes";
                public const string OdeToJoy = "Build 16-note melodies by clicking (repeatable). Every 16 notes resets counter and grants rewards:\n• 4 notes: +25 Melodious and Harmonious Fragments\n• 8 notes: Random minor score\n• 12 notes: Random major score sheet\n• 16 notes: 5× Notes Per Second for 15 seconds\nOn ability expiry: +3 Entropic Melodies per completed 4-note section\n\n20 second duration | 10 minute cooldown";
            }

            public static class Short
            {
                public const string Moonlight = "Lunar Phases: Cycles through 4 moon phases every 5 seconds (New Moon: 2× Notes Per Click, Crescent: 10% component drop per click, Full Moon: 3× Notes Per Second, Waning: Upgrades 50% off). When resonating with Moonlight Major, all phases active simultaneously! - 20 second duration, 4 minute cooldown.";
                public const string DiesIrae = "Seven Seals: Each click places a seal. At 7 seals, grants 3 random minor scores without consuming components. Stacks during duration (15 second duration, 3.5 minute cooldown).";
                public const string Winter = "Duet Resonance: Activate to convert your frozen Notes Per Second into a click multiplier. Each click extends duration by 0.5 seconds (max +10 seconds, 15 second base duration, 5 minute cooldown).";
                public const string Eroica = "Victory March: Fill bar with 100 clicks for escalating rewards (25%: minor score, 50%: major score, 75%: +50 fragments, 100%: double all scores) - 5 minute cooldown.";
                public const string SwanLake = "Feather Cascade: Each of your first 10 clicks grants 1 of each feather (Revered, Chromatic, Polyphonic). Every 5th click grants +25% notes - 20 second duration, 4 minute cooldown.";
                public const string LaCampanella = "Chime Chain: First click creates a chime. Each click within 1 second extends the chain. Chain gives length² × Notes Per Second as instant notes. Chain breaks after 1 second of no clicks - 10 second duration, 10 minute cooldown.";
                public const string Enigma = "Mystery Clicks: Each click triggers 1 random effect (Red: 5× Notes Per Second notes, Blue: 10% Notes Per Second boost, Green: 30 Harmonious Fragments, Yellow: 30 Melodious Fragments, Purple: +1 major per owned, Orange: +1 minor component per owned, White: +1 to 3 random minors, Black: -65% notes) - 10 second duration, 30 minute cooldown.";
                public const string Fate = "Hourglass Reversal: All actions during first 10 seconds replay during last 10 seconds at X× effectiveness where X = clicks performed (20 second duration, 8 minute cooldown).";
                public const string OdeToJoy = "Crescendo Conductor: Build 16-note melodies (repeatable/stackable). Rewards at 4/8/12/16 notes (+25 fragments, minor score, major sheet, 5× Notes Per Second for 15 seconds). On expiry: +3 Entropic Melodies per completed section - 20 second duration, 10 minute cooldown.";
            }

            // Duration and cooldown constants
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

        // ============================================================
        // LOOKUP HELPERS
        // ============================================================

        /// <summary>
        /// Get major score description by name
        /// </summary>
        public static string GetMajorScoreDescription(string scoreName)
        {
            return scoreName switch
            {
                "Moonlight Sonata" => MajorScores.MoonlightSonata,
                "Eroica" => MajorScores.Eroica,
                "Swan Lake" => MajorScores.SwanLake,
                "La Campanella" => MajorScores.LaCampanella,
                "Enigma Variations" => MajorScores.EnigmaVariations,
                "Fate" => MajorScores.Fate,
                "Ode to Joy" => MajorScores.OdeToJoy,
                "Dies Irae" => MajorScores.DiesIrae,
                "Winter" => MajorScores.Winter,
                _ => $"No description available for: {scoreName}"
            };
        }

        /// <summary>
        /// Get weapon description by name (for TempoResonate tooltips)
        /// </summary>
        public static string GetWeaponDescription(string weaponName)
        {
            return weaponName switch
            {
                "Incisor of Moonlight" => Weapons.IncisorOfMoonlight,
                "Eulogy of the Moon" => Weapons.EulogyOfTheMoon,
                "Sakura's Blossom" => Weapons.SakurasBlossom,
                "Funeral Prayer" => Weapons.FuneralPrayer,
                "Star-Scattered Wings" => Weapons.StarScatteredWings,
                "Thousand Winged Swan" => Weapons.ThousandWingedSwan,
                "Symphony of Bells" => Weapons.SymphonyOfBells,
                "Razer of Bell's Chimes" => Weapons.RazerOfBellsChimes,
                "Creator of Mystery" => Weapons.CreatorOfMystery,
                "Truthseeker" => Weapons.Truthseeker,
                "Astral Chainripper" => Weapons.AstralChainripper,
                "Cosmic Weaver" => Weapons.CosmicWeaver,
                "Joyful Catharsis" => Weapons.JoyfulCatharsis,
                "Ode to Creation" => Weapons.OdeToCreation,
                _ => $"No description available for: {weaponName}"
            };
        }

        /// <summary>
        /// Get Forte Resonance description by weapon name
        /// </summary>
        public static string GetForteDescription(string weaponName)
        {
            return weaponName switch
            {
                "Incisor of Moonlight" => ForteResonance.IncisorOfMoonlight,
                "Eulogy of the Moon" => ForteResonance.EulogyOfTheMoon,
                "Sakura's Blossom" => ForteResonance.SakurasBlossom,
                "Funeral Prayer" => ForteResonance.FuneralPrayer,
                "Star-Scattered Wings" => ForteResonance.StarScatteredWings,
                "Thousand Winged Swan" => ForteResonance.ThousandWingedSwan,
                "Symphony of Bells" => ForteResonance.SymphonyOfBells,
                "Razer of Bell's Chimes" => ForteResonance.RazerOfBellsChimes,
                "Creator of Mystery" => ForteResonance.CreatorOfMystery,
                "Truthseeker" => ForteResonance.Truthseeker,
                "Astral Chainripper" => ForteResonance.AstralChainripper,
                "Cosmic Weaver" => ForteResonance.CosmicWeaver,
                "Joyful Catharsis" => ForteResonance.JoyfulCatharsis,
                "Ode to Creation" => ForteResonance.OdeToCreation,
                "Seven Circles" => ForteResonance.SevenCircles,
                "Hell's Wrath" => ForteResonance.HellsWrath,
                "Cacophonic Blizzard" => ForteResonance.CacophonicBlizzard,
                "The Snow's Desire" => ForteResonance.TheSnowsDesire,
                _ => "Forte Resonance: <placeholder>"
            };
        }

        /// <summary>
        /// Get unified description (major score or weapon) for TempoResonate screen
        /// </summary>
        public static string GetTempoResonateDescription(string itemName)
        {
            // Try major scores first
            var majorDesc = GetMajorScoreDescription(itemName);
            if (!majorDesc.StartsWith("No description"))
                return majorDesc;
            
            // Try weapons
            return GetWeaponDescription(itemName);
        }
    }
}
