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
            public const string MoonlightSonata = @"Moonlight Sonata

""Beneath the silent moon, shadows dance with luminous grace—nightfall becomes your sanctuary.""

Crescendance: Eclipse of the Nocturne
• Every 20th click during nighttime (8PM-6AM): Gain 1 stack of ""Moonbeam Resonance""
• At 8 stacks: Auto-consume for +100% of current notes and 1 ""Harmonizing Moonlight"" stack
• Empowers both Moonlight weapons with unique crescendance abilities";

            public const string Eroica = @"Eroica (Symphony No. 3)

""A hero's triumph echoes through eternity, where crimson sakura blooms amidst symphonic glory.""

Crescendance: Symphony of Triumph
• Every minor craft: +1 Heroic Resolve stack
• Consume 1 Heroic Resolve + 25% notes: +1 Symphonic Catharsis stack
• Consume Symphonic Catharsis: 10s of double NPC + 10% crit chance (stackable - each consumption adds 10s)";

            public const string SwanLake = @"Swan Lake

""Through graceful wings, fortune's feathers descend—each one a promise of transcendent beauty.""

Crescendance: Wings of Transcendence
• Revered Feathers (every 2 clicks): Consume 5 for +20% notes
• Chromatic Feathers (every 8 clicks): Consume 10 for +2 all owned minors
• Polyphonic Feathers (every 15 clicks): Consume 1 for +83 Entropic Melodies +75% notes
• Wings of Velocity weapon bond: Polyphonic feather → 7s of NPC = NPS × 10^stacks (max 8)
• Star Scattered Wings weapon bond: Any feather → +1 Scattered Starlight stack (max 5)";

            public const string LaCampanella = @"La Campanella

""Each chime resonates with destiny; as the bell cracks, fortune rings eternal.""

Crescendance: Grandiose Bell
• Bell cracks at 20/40/60 clicks, advancing stage (Intact → Crescending → Radiant → Harmonizing)
• Mend to collect rewards:
  - Crescending: +2 random owned minors
  - Radiant: +5 Deafening Chime stacks
  - Harmonizing: Consume all Deafening Chime to gain notes = stacks × NPS
• Deafening Chime: Max 15 stacks. Gained from Radiant mend (5 stacks) or Entropic Crescendo of Eternity critical hits (3 stacks per crit)
• Symphony weapon bond: +1 Deafening Chime when bell cracks
• Razer weapon bond: +2% critical click rate when equipped";

            public const string EnigmaVariations = @"Enigma Variations

""Within the veil of mystery lies infinite possibility—each riddle a key to untold power.""

Crescendance: Resonant Mystery
• Every 10th click: +1 Resonant Mystery stack
• Every 15th click (with Creator of Mystery): +1 additional Resonant Mystery stack
• Consume 1 stack for:
  - +50% of current notes
  - +17 Entropic Melodies
  - +1 random owned minor
• Bulk consume (10+ stacks): +25% notes per stack consumed
• Truthseeker weapon bond: +1 random minor per 2 stacks consumed";

            public const string Fate = @"Fate (Symphony No. 5)

""The stars themselves bend to your will, weaving cosmic threads into a tapestry of destiny.""

Crescendance: Cosmic Modulation
• Every 8th click: +1 Cosmic Modulation stack + 10% notes bonus
• Tier Effects (cumulative):
  - Tier 1 (1+ stacks): Every click grants +5 Entropic Melodies
  - Tier 2 (2+ stacks): Every click grants +5 Melodious and Harmonious Fragments
  - Tier 3 (3+ stacks): Each Cosmic stack gained grants +1 Stellar Cascade critical click
  - Tier 4 (4+ stacks): Consuming Cosmic stacks grants Symphony of the Stars (stacks consumed ÷ 2)
  - Tier 5 (5+ stacks): Guaranteed Entropic Crescendo of Eternity on every click
• Consume Cosmic Modulation: +5 Entropic Melodies per stack consumed";

            public const string OdeToJoy = @"Ode to Joy (Symphony No. 9)

""From petals of harmony and melody, life itself blooms—a symphony of boundless celebration.""

Crescendance: Petals of Life
• Minor craft: +1 Petal of Harmony
• Major craft: +1 Petal of Melody
• Consume Harmony: +83 Entropic Melodies
• Consume Melody: +5s of guaranteed Entropic Crescendo of Eternity critical hits (stackable)
• Combine (1 Harmony + 1 Melody + 17 Entropic): 1 Ode to Life
• Consume Ode to Life: Double all owned minors
• Joyful Catharsis bond: Every petal gained → double NPS for 5s
• Ode to Creation bond: On Ode to Life consume → double petal generation for 25s";

            public const string DiesIrae = @"Dies Irae

""Wrath incarnate—where dissonant hatred forges an infernal symphony of unstoppable retribution.""

Major Ability:
Clicks build infernal power that multiplies damage.

Crescendance: Symphony of Hell's Retribution
• Every click: +1 Dissonant Hatred (up to 50), then +1 Discordant Malice
• Consume 5 Dissonant Hatred: +1 Cacophonic Oblivion
• Consume Discordant Malice: +X Entropic (X = current Dissonant Hatred stacks)
• Consume Cacophonic Oblivion: +20 Symphony of Hell's Retribution crits (STACKS)
• Hell crit formula: NPC × NPS × Dissonant Hatred stacks
• Seven Circles bond: Symphony crits grant Wrathful Seals
• Consume Wrathful Seal: +5 Seal-breaking Melody crits (STACKS, DOUBLE Symphony damage)
• Hell's Wrath bond: Symphony/Seal-breaking crits grant +50 Entropic each";

            public const string Winter = @"Winter

Major Ability: Glacial Dominion
Grants bonus effects when NPS is frozen.

Crescendance: Requiem of the Frozen Choir
• Freezing Harmony (3s cooldown): Freeze NPS for 5s, +1 Frigid Melody
• Ignite the Blizzard: Convert Frigid Melody → Eternal Frost OR Regal Snowlight
• Eternal Frost: Consume for NPS freeze + enhanced 'Blizzard's Command' clicks (15 per stack)
• Regal Snowlight: Consume for +50 Melodious, +50 Harmonious, +25 Entropic per stack
• Cacophonic Blizzard bond: Eternal Frost consume grants +2 Snow's Oblivion
• Consume Snow's Oblivion: +X Entropic (X = total upgrades owned)
• The Snow's Desire bond: Frigid Melody gains doubled";
        }

        // ============================================================
        // WEAPONS - FULL DESCRIPTIONS (for Tempo Resonate tooltips)
        // ============================================================
        
        public static class Weapons
        {
            // Moonlight Weapons
            public const string IncisorOfMoonlight = @"Incisor of Moonlight

Passive: Moonlight Strike - Every 4th click grants +1% bonus notes (+5% during nighttime 8PM-6AM).

Crescendance Bond: Piercing Radiance - Each Moonbeam Resonance stack gained grants +10% additional notes of current notes.

Duet: Lunar Phases - Cycles through moon phases granting bonuses. All phases active with Moonlight Major resonated. 20s duration, 4min cooldown.";

            public const string EulogyOfTheMoon = @"Eulogy of the Moon

Passive: Nocturnal Refund - On minor score craft: 25% chance to refund all materials (50% during nighttime 8PM-6AM).

Crescendance Bond: Requiem of Renewal - Consume 1 Harmonizing Moonlight stack to gain 3 Moonbeam Resonance stacks plus components.

Duet: Lunar Phases - Cycles through moon phases granting bonuses. All phases active with Moonlight Major resonated. 20s duration, 4min cooldown.";

            // Eroica Weapons
            public const string SakurasBlossom = @"Sakura's Blossom

Passive: Heroic Bounty - On Major craft (Unite the Symphony screen), grants 1 of each component type (Keys, Scales, Progressions) for that score's associated minor.

Crescendance Bond: Crimson Requiem - When Symphonic Catharsis is consumed (stackable), next 30 clicks are 'Blossom's Blooming in Crimson Light' crits (NPC + NPS × NPC).

Duet: Victory March - Fill 100-click bar for escalating rewards. 5min cooldown.";

            public const string FuneralPrayer = @"Funeral Prayer

Passive: Prayer of Valor - Every 10th click grants 1 Prayer stack. At 3 Prayer stacks: next 15 clicks each gain +6× Notes Per Second as 'Retribution of the Symphonic Sakura' crits.

Crescendance Bond: Testament of Harmony - When Symphonic Catharsis is consumed (stackable), grants +1 Testament of Harmony (consume for +100 Melodious + 100 Harmonious Fragments + NPS×10 notes).

Duet: Victory March - Fill 100-click bar for escalating rewards. 5min cooldown.";

            // Swan Lake Weapons
            public const string StarScatteredWings = @"Star-Scattered Wings

Passive: Stellar Fragment Rain - Every 10th click grants +5 Melodious and +5 Harmonious Fragments.

Crescendance Bond: Stellar Component Rain - When Revered or Chromatic Feather obtained during Crescendance: grants +2 components to random owned minor score.

Duet: Feather Cascade - First 10 clicks grant all feather types, with milestone clicks granting note bonuses. 20s duration, 4min cooldown.";

            public const string ThousandWingedSwan = @"Thousand Winged Swan

Passive: Wings of Fortune - Every minor score acquisition grants +33 Entropic Melodies.

Crescendance Bond: Wings of Velocity - Polyphonic Feather consumption replaces NPC with NPS × 10^stacks (max 8 stacks). 7s duration. Dark pink outline.

Duet: Feather Cascade - First 10 clicks grant all feather types, with milestone clicks granting note bonuses. 20s duration, 4min cooldown.";

            // La Campanella Weapons
            public const string SymphonyOfBells = @"Symphony of Bells

Passive: Harmonic Duplication - On minor score craft: duplicate that minor score (+1 extra copy).

Crescendance Bond: Resonant Crack Bonus - When Grandiose Bell cracks (at 20/40/60 clicks): gain +1 Deafening Chime stack (max 15).

Duet: Chime Chain - Build click chains within 1s windows for exponential rewards. 10s duration, 10min cooldown.";

            public const string RazerOfBellsChimes = @"Razer of Bell's Chimes

Passive: Component Echo - On minor score craft: refund 2 random components for that minor.

Crescendance Bond: Precision Chimes - While equipped: +2% critical click rate.

Duet: Chime Chain - Build click chains within 1s windows for exponential rewards. 10s duration, 10min cooldown.";

            // Enigma Weapons  
            public const string CreatorOfMystery = @"Creator of Mystery

Passive: Chaotic Flux - Every 3rd click: ±25% notes (60% chance positive, 40% chance negative).

Crescendance Bond: Accelerated Mystery - Every 15th click: gain +1 additional Resonant Mystery stack (on top of base every-10th).

Duet: Mystery Clicks - Each click triggers random powerful effect. 10s duration, 30min cooldown.";

            public const string Truthseeker = @"Truthseeker

Passive: Revelation Burst - On upgrade purchase: gain +5 Resonant Mystery stacks.

Crescendance Bond: Knowledge Harvest - For every 2 Resonant Mystery stacks consumed: gain +1 random owned minor score.

Duet: Mystery Clicks - Each click triggers random powerful effect. 10s duration, 30min cooldown.";

            // Fate Weapons
            public const string AstralChainripper = @"Astral Chainripper

Passive: Temporal Surge - On Fate minor craft: 5× Notes Per Second for 10 seconds.

Crescendance Bond: Stellar Convergence - Every 5 Cosmic Modulation stacks gained: automatically grants +1 Symphony of the Stars stack.

Duet: Hourglass Reversal - First 10s banks actions, last 10s replays at X× effectiveness (X = clicks during banking phase). 20s duration, 8min cooldown.";

            public const string CosmicWeaver = @"Cosmic Weaver

Passive: Harmonic Multiplication - On minor craft: If you own +1 of that major, gain +3 of that major score.

Crescendance Bond: Stellar Cascade - Each Symphony of the Stars consumed: Next 5 clicks are guaranteed Stellar Cascade critical hits (1700× multiplier).

Duet: Hourglass Reversal - First 10s banks actions, last 10s replays at X× effectiveness (X = clicks during banking phase). 20s duration, 8min cooldown.";

            // Ode to Joy Weapons
            public const string JoyfulCatharsis = @"Joyful Catharsis

Passive: Harmonic Entropy - Every 50th click: +15 Entropic Melodies.

Crescendance Bond: Euphoric Resonance - Every time you gain any Petal: double Notes Per Second for 5 seconds (effect stacks).

Duet: Crescendo Conductor - Build melodies for escalating rewards. 20s duration, 10min cooldown.";

            public const string OdeToCreation = @"Ode to Creation

Passive: Life Genesis - Every 20th click: generate random Petal (50% Harmony, 50% Melody).

Crescendance Bond: Life Amplification - When Ode to Life is consumed: double petal generation for 25 seconds (generates petals every 10th click instead).

Duet: Crescendo Conductor - Build melodies for escalating rewards. 20s duration, 10min cooldown.";

            // Dies Irae Weapons
            public const string SevenCircles = @"Seven Circles

Passive: Wrathful Amplification - Every critical hit in the game (all types) grants +20% of your current notes as bonus notes.

Crescendance Bond: Seal Resonance - 'Symphony of Hell's Retribution' crits grant +1 Wrathful Seal. Consume Wrathful Seals for 'Seal-breaking Melody of Oblivion' crits:
• Each Wrathful Seal consumed: +5 Seal-breaking crits (STACKS)
• Seal-breaking damage: 2 × NPC × NPS × Dissonant Hatred (DOUBLE Symphony of Hell's Retribution power)
• Effects stack if multiple Seals consumed

Duet: Infernal Symphony - During duet, Seal-breaking crits also grant +1 Wrathful Seal. Consume 15+ Wrathful Seals for next 5 clicks that double your current notes. 20s duration, 10min cooldown.";

            public const string HellsWrath = @"Hell's Wrath

Passive: Infernal Bounty - Every 20th click grants +2 to your 2 lowest owned minor scores (tied lowest scores chosen randomly).

Crescendance Bond: Entropic Fury - Dies Irae crits grant Entropic Melodies:
• Each 'Symphony of Hell's Retribution' crit: +50 Entropic Melodies
• Each 'Seal-breaking Melody of Oblivion' crit: +50 Entropic Melodies

Duet: Infernal Symphony - During duet, Seal-breaking crits also grant +1 Wrathful Seal. Consume 15+ Wrathful Seals for next 5 clicks that double your current notes. 20s duration, 10min cooldown.";

            // Winter Weapons
            public const string CacophonicBlizzard = @"Cacophonic Blizzard

Passive: Permafrost Strikes - Every 20th click freezes NPS for 5 seconds and grants your next 20 clicks +50% notes each.

Crescendance Bond: Snow's Oblivion - When you consume Eternal Frost stacks, gain +2 Snow's Oblivion. Consume 1 Snow's Oblivion for +X Entropic Melodies (X = total upgrades owned).

Duet: Symphony of Absolute Zero - Freezes NPS for 20s. Every click grants +1 Frigid Melody + Blizzard's Command crit, extends duration by +0.5s (max +10s). 20s duration, 4min cooldown.";

            public const string TheSnowsDesire = @"The Snow's Desire

Passive: Accelerating Flurry - Every click grants +1% NPC (max +50%). Bonus decays to 0% after 5 seconds of no clicking.

Crescendance Bond: Frigid Resonance - Freezing Harmony grants +2 Frigid Melody stacks instead of +1 (doubling effect).

Duet: Symphony of Absolute Zero - Freezes NPS for 20s. Every click grants +1 Frigid Melody + Blizzard's Command crit, extends duration by +0.5s (max +10s). 20s duration, 4min cooldown.";
        }

        // ============================================================
        // FORTE RESONANCE (Armory screen - individual weapon effects)
        // ============================================================
        
        public static class ForteResonance
        {
            // Moonlight
            public const string IncisorOfMoonlight = "Passive: Moonlight Strike - Every 4th click grants +1% bonus notes (+5% during nighttime 8PM-6AM).\n\nCrescendance Bond: Piercing Radiance - Each Moonbeam Resonance stack gained grants +10% additional notes of current notes.\n\nDuet: Lunar Phases - Cycles through moon phases granting bonuses. All phases active with Moonlight Major resonated. 20s duration, 4min cooldown.";
            public const string EulogyOfTheMoon = "Passive: Nocturnal Refund - On minor score craft: 25% chance to refund all materials (50% during nighttime 8PM-6AM).\n\nCrescendance Bond: Requiem of Renewal - Consume 1 Harmonizing Moonlight stack to gain 3 Moonbeam Resonance stacks plus components.\n\nDuet: Lunar Phases - Cycles through moon phases granting bonuses. All phases active with Moonlight Major resonated. 20s duration, 4min cooldown.";
            
            // Eroica
            public const string SakurasBlossom = "Passive: Heroic Bounty - On Major craft (Unite the Symphony screen), grants 1 of each component type (Keys, Scales, Progressions) for that score's associated minor.\n\nCrescendance Bond: Crimson Requiem - When Symphonic Catharsis is consumed (stackable), next 30 clicks are 'Blossom's Blooming in Crimson Light' crits (NPC + NPS × NPC).\n\nDuet: Victory March - Fill 100-click bar for escalating rewards. 5min cooldown.";
            public const string FuneralPrayer = "Passive: Prayer of Valor - Every 10th click grants 1 Prayer stack. At 3 Prayer stacks: next 15 clicks each gain +6× Notes Per Second as 'Retribution of the Symphonic Sakura' crits.\n\nCrescendance Bond: Testament of Harmony - When Symphonic Catharsis is consumed (stackable), grants +1 Testament of Harmony (consume for +100 Melodious + 100 Harmonious Fragments + NPS×10 notes).\n\nDuet: Victory March - Fill 100-click bar for escalating rewards. 5min cooldown.";
            
            // Swan Lake
            public const string StarScatteredWings = "Passive: Stellar Fragment Rain - Every 10th click grants +5 Melodious and +5 Harmonious Fragments.\n\nCrescendance Bond: Stellar Component Rain - When Revered or Chromatic Feather obtained during Crescendance: grants +2 components to random owned minor score.\n\nDuet: Feather Cascade - First 10 clicks grant all feather types, with milestone clicks granting note bonuses. 20s duration, 4min cooldown.";
            public const string ThousandWingedSwan = "Passive: Wings of Fortune - Every minor score acquisition grants +33 Entropic Melodies.\n\nCrescendance Bond: Wings of Velocity - Polyphonic Feather consumption replaces NPC with NPS × 10^stacks (max 8 stacks). 7s duration. Dark pink outline.\n\nDuet: Feather Cascade - First 10 clicks grant all feather types, with milestone clicks granting note bonuses. 20s duration, 4min cooldown.";
            
            // La Campanella
            public const string SymphonyOfBells = "Passive: Harmonic Duplication - On minor score craft: duplicate that minor score (+1 extra copy).\n\nCrescendance Bond: Resonant Crack Bonus - When Grandiose Bell cracks (at 20/40/60 clicks): instantly gain +75% notes.\n\nDuet: Chime Chain - Build click chains within 1s windows for exponential rewards. 10s duration, 10min cooldown.";
            public const string RazerOfBellsChimes = "Passive: Component Echo - On minor score craft: refund 2 random components for that minor.\n\nCrescendance Bond: Entropic Resonance - On Grandiose Bell mend: gain Entropic Melodies equal to click count × 2/3 (maximum 83).\n\nDuet: Chime Chain - Build click chains within 1s windows for exponential rewards. 10s duration, 10min cooldown.";
            
            // Fate
            public const string AstralChainripper = "Passive: Temporal Surge - On Fate minor craft: 5× Notes Per Second for 10 seconds.\n\nCrescendance Bond: Stellar Convergence - Every 5 Cosmic Modulation stacks gained: automatically grants +1 Symphony of the Stars stack.\n• Symphony of the Stars: Consume 1 stack for +3 to lowest owned minor score.\n\nDuet: Hourglass Reversal - First 10s banks actions, last 10s replays at X× effectiveness (X = clicks during banking phase). 20s duration, 8min cooldown.";
            public const string CosmicWeaver = "Passive: Harmonic Multiplication - On minor craft: If you own +1 of that major, gain +3 of that major score.\n\nCrescendance Bond: Stellar Cascade - Each Symphony of the Stars consumed: Grants 5 stackable Stellar Cascade critical clicks (1700× multiplier).\n\nDuet: Hourglass Reversal - First 10s banks actions, last 10s replays at X× effectiveness (X = clicks during banking phase). 20s duration, 8min cooldown.";
            
            // Enigma
            public const string CreatorOfMystery = "Passive: Chaotic Flux - Every 3rd click: ±25% notes (60% chance positive, 40% chance negative).\n\nCrescendance Bond: Accelerated Mystery - Every 15th click: gain +1 additional Resonant Mystery stack (on top of base every-10th).\n\nDuet: Mystery Clicks - Each click triggers random powerful effect. 10s duration, 30min cooldown.";
            public const string Truthseeker = "Passive: Revelation Burst - On upgrade purchase: gain +5 Resonant Mystery stacks.\n\nCrescendance Bond: Knowledge Harvest - For every 2 Resonant Mystery stacks consumed: gain +1 random owned minor score.\n\nDuet: Mystery Clicks - Each click triggers random powerful effect. 10s duration, 30min cooldown.";
            
            // Ode to Joy
            public const string JoyfulCatharsis = "Passive: Harmonic Entropy - Every 50th click: +15 Entropic Melodies.\n\nCrescendance Bond: Euphoric Resonance - Every time you gain any Petal: double Notes Per Second for 5 seconds (effect stacks).\n\nDuet: Crescendo Conductor - Build melodies for escalating rewards. 20s duration, 10min cooldown.";
            public const string OdeToCreation = "Passive: Life Genesis - Every 20th click: generate random Petal (50% Harmony, 50% Melody).\n\nCrescendance Bond: Life Amplification - When Ode to Life is consumed: double petal generation for 25 seconds (generates petals every 10th click instead).\n\nDuet: Crescendo Conductor - Build melodies for escalating rewards. 20s duration, 10min cooldown.";
            
            // Dies Irae
            public const string SevenCircles = "Passive: Wrathful Amplification - Every critical hit (all types) grants +20% of your current notes as bonus notes.\n\nCrescendance Bond: Seal Resonance - 'Symphony of Hell's Retribution' crits grant +1 Wrathful Seal. Consume Seals for next 5 clicks as 'Seal-breaking Melody of Oblivion' (DOUBLE Symphony damage: 2 × NPC × NPS × Dissonant Hatred). Effect stacks if multiple Seals consumed.\n\nDuet: Infernal Symphony - Seal-breaking crits grant +1 Wrathful Seal. Consume 15+ Seals for next 5 note-doubling clicks. 20s duration, 10min cooldown.";
            public const string HellsWrath = "Passive: Infernal Bounty - Every 20th click grants +2 to your 2 lowest owned minor scores (tied lowest chosen randomly).\n\nCrescendance Bond: Entropic Fury - Dies Irae crits grant Entropic Melodies:\n• Symphony of Hell's Retribution: +50 Entropic\n• Seal-breaking Melody of Oblivion: +50 Entropic\n\nDuet: Infernal Symphony - For 10 seconds, every click is a guaranteed Seal-breaking crit that returns +1 Wrathful Seal. During duet, consume 15+ Seals for next 5 note-doubling clicks. 10s duration, 10min cooldown.";
            
            // Winter
            public const string CacophonicBlizzard = "Passive: Permafrost Strikes - Every 20th click freezes NPS for 5 seconds and grants your next 20 clicks +50% notes each.\n\nCrescendance Bond: Snow's Oblivion - When you consume Eternal Frost stacks, gain +2 Snow's Oblivion. Consume 1 Snow's Oblivion for +X Entropic Melodies (X = total upgrades owned).\n\nDuet: Symphony of Absolute Zero - Freezes NPS for 20s. Every click grants +1 Frigid Melody + Blizzard's Command crit, extends duration by +0.5s (max +10s). 20s duration, 4min cooldown.";
            public const string TheSnowsDesire = "Passive: Accelerating Flurry - Every click grants +1% Notes Per Click (max +50%). Decays to 0% after 5 seconds of no clicking. Current bonus displayed in Crescendance panel.\n\nCrescendance Bond: Frigid Resonance - Whenever you gain 1 Frigid Melody stack, gain +1 bonus stack (doubling effect).\n\nDuet: Symphony of Absolute Zero - Freezes NPS for 20s. Every click grants +1 Frigid Melody + Blizzard's Command crit, extends duration by +0.5s (max +10s). 20s duration, 4min cooldown.";
        }

        // ============================================================
        // DUET ABILITIES
        // ============================================================
        
        public static class Duets
        {
            public static class Names
            {
                public const string Moonlight = "Moonlight Sonata:\nLunar Phases";
                public const string DiesIrae = "Damnation Requiem:\nInfernal Symphony";
                public const string Winter = "Requiem of the Frozen Choir:\nSymphony of Absolute Zero";
                public const string Eroica = "Symphony of Heroes:\nVictory March";
                public const string SwanLake = "Wings of Transcendence:\nFeather Cascade";
                public const string LaCampanella = "Bell Resonance:\nChime Chain";
                public const string Enigma = "Enigmatic Puzzle:\nMystery Clicks";
                public const string Fate = "Threads of Destiny:\nHourglass Reversal";
                public const string OdeToJoy = "Symphony of Celebration:\nCrescendo Conductor";
            }

            public static class Full
            {
                public const string Moonlight = "Cycles through 4 moon phases every 5 seconds:\n• New Moon: 2× Notes Per Click\n• Crescent: 10% component drop chance per click\n• Full Moon: 3× Notes Per Second\n• Waning: Upgrades cost 50% less\n\nSPECIAL: All phases active simultaneously when resonating with Moonlight Sonata Major!\n\n20 second duration | 4 minute cooldown";
                public const string DiesIrae = "For 10 seconds, every click is a guaranteed Seal-breaking Melody of Oblivion critical hit that returns +1 Wrathful Seal.\n\nDuring the duet, when you have 15+ Wrathful Seals, you can consume them to grant your next 5 clicks the power to double your current notes.\n\n10 second duration | 10 minute cooldown";
                public const string Winter = "Freezes Notes Per Second for 20 seconds. Every click during freeze:\n• Grants +1 Frigid Melody stack\n• Applies 'Blizzard's Command of Eternal Ice' critical hit (base notes + NPC × NPS)\n• Extends duration by +0.5 seconds (max +10s extension = 30s total)\n\n20 second base duration | 4 minute cooldown";
                public const string Eroica = "Fill progress bar with 100 clicks:\n• 25% progress: Random minor score\n• 50% progress: Random major score\n• 75% progress: +50 Melodious and Harmonious Fragments\n• 100% complete: Double all owned scores\n\n5 minute cooldown";
                public const string SwanLake = "Each of your first 10 clicks grants 1 Revered Feather, 1 Chromatic Feather, and 1 Polyphonic Feather. Every 5th click (5th and 10th) increases current notes by +25%.\n\n20 second duration | 4 minute cooldown";
                public const string LaCampanella = "Click to create chimes. Clicking within 1 second extends chain. Chain reward equals (chain length)² × Notes Per Second as instant notes. Chain breaks after 1 second of no clicks.\n\n10 second duration | 10 minute cooldown";
                public const string Enigma = "Each click triggers 1 random effect:\nRed: 5× Notes Per Second as instant notes | Blue: +10% Notes Per Second boost | Green: +30 Harmonious Fragments | Yellow: +30 Melodious Fragments | Purple: +1 major score per owned type | Orange: +1 minor component per owned minor type | White: +1 to 3 random minor scores | Black: -65% current notes\n\n10 second duration | 30 minute cooldown";
                public const string Fate = "All actions performed during the first 10 seconds will be replayed during the last 10 seconds at X× effectiveness, where X equals the number of clicks you performed during the first half.\n\nDuration: 20 seconds | Cooldown: 8 minutes";
                public const string OdeToJoy = "Build 16-note melodies by clicking (repeatable). Every 16 notes resets counter and grants rewards:\n• 4 notes: +25 Melodious and Harmonious Fragments\n• 8 notes: +5 Petals of Harmony\n• 12 notes: +5 Petals of Melody\n• 16 notes: +1 Ode to Life\nOn ability expiry: +3 Entropic Melodies per completed 4-note section\n\n20 second duration | 10 minute cooldown";
            }

            public static class Short
            {
                public const string Moonlight = "Lunar Phases: Cycles through 4 moon phases every 5 seconds (New Moon: 2× Notes Per Click, Crescent: 10% component drop per click, Full Moon: 3× Notes Per Second, Waning: Upgrades 50% off). When resonating with Moonlight Major, all phases active simultaneously! - 20 second duration, 4 minute cooldown.";
                public const string DiesIrae = "Infernal Symphony: For 10 seconds, every click is a guaranteed Seal-breaking Melody crit that returns +1 Wrathful Seal. During duet, consume 15+ Wrathful Seals for next 5 clicks that double your current notes (10 second duration, 10 minute cooldown).";
                public const string Winter = "Duet Resonance: Activate to convert your frozen Notes Per Second into a click multiplier. Each click extends duration by 0.5 seconds (max +10 seconds, 15 second base duration, 5 minute cooldown).";
                public const string Eroica = "Victory March: Fill bar with 100 clicks for escalating rewards (25%: minor score, 50%: major score, 75%: +50 fragments, 100%: double all scores) - 5 minute cooldown.";
                public const string SwanLake = "Feather Cascade: Each of your first 10 clicks grants 1 of each feather (Revered, Chromatic, Polyphonic). Every 5th click grants +25% notes - 20 second duration, 4 minute cooldown.";
                public const string LaCampanella = "Chime Chain: First click creates a chime. Each click within 1 second extends the chain. Chain gives length² × Notes Per Second as instant notes. Chain breaks after 1 second of no clicks - 10 second duration, 10 minute cooldown.";
                public const string Enigma = "Mystery Clicks: Each click triggers 1 random effect (Red: 5× Notes Per Second notes, Blue: 10% Notes Per Second boost, Green: 30 Harmonious Fragments, Yellow: 30 Melodious Fragments, Purple: +1 major per owned, Orange: +1 minor component per owned, White: +1 to 3 random minors, Black: -65% notes) - 10 second duration, 30 minute cooldown.";
                public const string Fate = "Hourglass Reversal: All actions during first 10 seconds replay during last 10 seconds at X× effectiveness where X = clicks performed (20 second duration, 8 minute cooldown).";
                public const string OdeToJoy = "Crescendo Conductor: Build 16-note melodies (repeatable/stackable). Rewards at 4/8/12/16 notes (+25 fragments, +5 Petals of Harmony, +5 Petals of Melody, +1 Ode to Life). On expiry: +3 Entropic Melodies per completed section - 20 second duration, 10 minute cooldown.";
            }

            // Duration and cooldown constants
            public static class Duration
            {
                public const int Moonlight = 20;
                public const int DiesIrae = 10;
                public const int Winter = 20;
                public const int SwanLake = 20;
                public const int LaCampanella = 10;
                public const int Enigma = 10;
                public const int Fate = 20;
                public const int OdeToJoy = 20;
            }

            public static class Cooldown
            {
                public const int Moonlight = 240;      // 4 minutes
                public const int DiesIrae = 600;       // 10 minutes
                public const int Winter = 240;         // 4 minutes
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
                "Seven Circles" => Weapons.SevenCircles,
                "Hell's Wrath" => Weapons.HellsWrath,
                "Cacophonic Blizzard" => Weapons.CacophonicBlizzard,
                "The Snow's Desire" => Weapons.TheSnowsDesire,
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
