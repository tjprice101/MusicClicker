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
• Every 20th click during nighttime (8PM to 6AM): Gain 1 Moonbeam Resonance stack.
• When you reach 8 Moonbeam Resonance stacks they auto-consume to:
    - Grant +100% of your current notes as an immediate bonus (adds to your notes total), and
    - Grant 1 Harmonizing Moonlight stack.

Notes: Moonbeam Resonance and Harmonizing Moonlight are crescendance resources used by Moonlight-specific bonds and consumptions.";

            public const string Eroica = @"Eroica (Symphony No. 3)

""A hero's triumph echoes through eternity—courage blooms like crimson sakura.""

Crescendance: Symphony of Triumph
• Each minor craft grants +1 Heroic Resolve stack.
• Spend 1 Heroic Resolve and 25% of your current notes to gain +1 Symphonic Catharsis stack.
• Consume 1 Symphonic Catharsis: Grants +10 seconds of doubled Notes Per Click (NPC) and +10% critical click chance. Effects stack in duration (each consumption adds +10s).";

            public const string SwanLake = @"Swan Lake

""Through graceful wings, fortune's feathers descend—each one a promise of transcendent beauty.""

Crescendance: Wings of Transcendence
• Every 2 clicks: gain 1 Revered Feather. Consume 5 Revered Feathers to grant +20% of your current notes.
• Every 8 clicks: gain 1 Chromatic Feather. Consume 10 Chromatic Feathers to grant +2 to all owned minor scores.
• Every 15 clicks: gain 1 Polyphonic Feather. Consume 1 Polyphonic Feather to gain +83 Entropic Melodies and +75% of your current notes.

Notes: Revered, Chromatic and Polyphonic Feathers are crescendance resources generated during Swan Lake. They can be consumed to trigger the effects listed above.";

                        public const string LaCampanella = @"La Campanella

""Each chime resonates with destiny; as the bell cracks, fortune rings eternal.""

Crescendance: Grandiose Bell
• The bell cracks when you reach 20, 40, and 60 clicks, advancing stages: Crescending (20) → Radiant (40) → Harmonizing (60).
• Major Ability (on crack): If you own La Campanella, each crack grants +5% of your current notes as an immediate bonus.
• Mend rewards by stage:
    - Crescending (20): Grant +2 random owned minor score(s).
    - Radiant (40): Grant +5 Deafening Chime stacks (applies up to the 15-stack cap).
    - Harmonizing (60): Consume all Deafening Chime stacks to gain immediate notes equal to (Deafening Chime stacks × Notes Per Second).
• Deafening Chime: Capped at 15 stacks. Can be gained from Radiant mend (+5) or certain Entropic Crescendo of Eternity critical hits (+3 per crit).";

            public const string EnigmaVariations = @"Enigma Variations

""Within the veil of mystery lies infinite possibility—each riddle a key to untold power.""

Crescendance: Resonant Mystery
• Every 10th click: +1 Resonant Mystery stack
• (Bonus) Bulk consume: special thresholds grant enhanced rewards
• Consume 1 stack for:
  - +50% of current notes
  - +17 Entropic Melodies
  - +1 random owned minor
 • Bulk consume (10+ stacks): +25% notes per stack consumed";

            

                        public const string Fate = @"Fate (Symphony No. 5)

""The stars themselves bend to your will, weaving cosmic threads into a tapestry of destiny.""

Crescendance: Cosmic Modulation
• Every 8th click: gain 1 Cosmic Modulation stack and immediately gain +10% of your current notes.
• Stack effects (cumulative):
    - 1+ stacks: Each click also awards +5 Entropic Melodies.
    - 2+ stacks: Each click also grants +5 Melodious and +5 Harmonious Fragments.
    - 3+ stacks: Each time you gain a Cosmic Modulation stack, also grant 1 Symphony of the Stars stack.
    - 4+ stacks: Consuming Cosmic Modulation converts (stacks consumed ÷ 2) into Symphony of the Stars (integer division).
    - 5+ stacks: While active, every click triggers an Entropic Crescendo of Eternity (guaranteed crit).
• Consume Cosmic Modulation: For each stack consumed, gain +5 Entropic Melodies.";

            public const string OdeToJoy = @"Ode to Joy (Symphony No. 9)

""From petals of harmony and melody, life itself blooms—a symphony of boundless celebration.""

Crescendance: Petals of Life
• Minor craft: Gain 1 Petal of Harmony.
• Major craft: Gain 1 Petal of Melody.
• Consume 1 Petal of Harmony: Gain +83 Entropic Melodies immediately.
• Consume 1 Petal of Melody: Grant +5 seconds of guaranteed Entropic Crescendo of Eternity critical hits (durations stack with multiple consumes).
• Combine (1 Harmony + 1 Melody + 17 Entropic Melodies): Create 1 Ode to Life.
• Consume Ode to Life: Double all owned minor scores.";

            public const string DiesIrae = @"Crescendance Ability: Wrathful Orchestration

• Dissonant Hatred is a click-generated resource (each click grants +1, capped at 50). After the 50 cap, additional clicks grant Discordant Malice stacks.
• Spend 5 Dissonant Hatred to gain 1 Cacophonic Oblivion.
• Consuming a Cacophonic Oblivion converts your next 20 clicks into guaranteed 'Symphony of Hell's Retribution' critical clicks; each such critical scales with NotesPerClick × NotesPerSecond × current Dissonant Hatred stacks.
• Symphony crits can generate Wrathful Seals via Crescendance Bonds; consuming Wrathful Seals triggers higher-power effects defined by those bonds (for example, Seal-breaking Melody or duet-specific bonuses).";

            // Winter major description updated below; removed duplicate older definition.
            public const string Winter = @"Crescendance Ability: Requiem of the Frozen Choir

• Frigid Melody stacks are gained by Crescendance actions and are consumed to produce Eternal Frost or Regal Snowlight.
• Eternal Frost grants a fixed number of enhanced offensive critical clicks when consumed; these criticals scale with NotesPerSecond.
• Regal Snowlight grants immediate resource and fragment bonuses when consumed; stacking increases potency.
• Crescendance Bonds (for example, The Snow's Desire) alter how Frigid Melody is gained or consumed and change the exact numerical effects of Eternal Frost/Regal Snowlight.";

            public const string ClairDeLune = @"Clair de Lune

""Time bends and fractures in moonlight's embrace—each broken moment holds infinite possibility.""

Crescendance Passive: Temporal Acceleration
Your Notes Per Click is permanently increased by (Notes Per Second)^5 while resonating with Clair de Lune.

Crescendance: Clockwork Symphony
Your clicks resonate with the real-world clock, granting different effects based on the current hour.

TIME-BASED CLICK EFFECTS (automatically changes each hour):
• 12:00-3:00 (AM/PM): Every 12th click grants ×12,000 Notes Per Click bonus notes
• 3:00-6:00 (AM/PM): Every 6th click grants +1 Shattered Moonlight stack
• 6:00-9:00 (AM/PM): Every 9th click grants +1 Clockwork of Infinity stack
• 9:00-12:00 (AM/PM): Every 12th click grants +20% of your current notes

CONSUMABLE STACKS:
• Shattered Moonlight: Consume for +100 Entropic Melodies
• Clockwork of Infinity: Consume for 5 Infinite Temporality critical hits (NPC^12)
• Temporal Fracture: Consume for +15% current notes (grants Surge crits if bonded with Clockwork Finality)

The flow of time itself bends to your will—each hour brings new temporal power.
";

            public const string Mercury = @"Mercury: The Winged Messenger

""Swift as thought, bearing fate's proclamations across the cosmos.""

Crescendance: Herald of Fate
Passively gain 1 Swift Resonance every 5 clicks.

ABILITIES:
• Herald's Descent: Double your current notes and gain 5 Swift Resonance (15s cooldown)
• Resonant Transformation: Convert 1 Swift Resonance → 1 Herald's Message + 250 Entropic Melodies
• Symphony's Bounty: Consume 3 Herald's Message for (minors × 5) Entropic Melodies + NPS^25 notes

CUSTOM STACKS:
• Swift Resonance: Gained passively from clicks and Herald's Descent
• Herald's Message: Transformed from Swift Resonance, consumed for massive rewards
• Planetary Alignment: Gained from weapon bonds, grants powerful critical hits

Transform speed into power—let the messenger's swiftness guide your ascension.
";

            public const string Mars = @"Mars, Bringer of War

""The blade sings with each strike—a symphony of war echoing through eternity.""

Crescendance: Blade of Symphonic War
• Every click: charge Blade by 1% (0-300% max)
• At 100%+: each 1% charge generates 1 Resolute Fractal stack
• Quadruple charge rate during Annihilation Nocturne duet

STACK CONVERSIONS:
• Consume 1 Resolute Fractal → Quintuple current notes (5× multiplier)
  - Every 5th consumption: grants 20 'Infinite War of Harmony!!!' critical hits
  - Critical Formula: NPC × (Blade charge %)
  - After all Infinite War crits end: Blade fully depletes to 0%
• Consume 5 Oblivion's Destruction → +30 to ALL owned minor scores

CUSTOM STACKS:
• Blade Charge: 0-300%, generates Resolute Fractal above 100%
• Resolute Fractal: Consume for 5× notes, every 5th triggers Infinite War
• Oblivion's Destruction: From Fractal of War bond (each Infinite War crit)
• Infinite War of Harmony: 20 crits granted every 5 Resolute Fractal consumed";
        }

        // ============================================================
        // WEAPONS - FULL DESCRIPTIONS (for Tempo Resonate tooltips)
        // ============================================================
        
        public static class Weapons
        {
            // Moonlight Weapons
            public const string IncisorOfMoonlight = @"Incisor of Moonlight

Passive: Moonlight Strike — Every 4th click grants bonus notes equal to 1% of your current notes. During nighttime (8PM to 6AM) this increases to 5%.

Upgrade Note: Purchasing the Incisor upgrade also grants an immediate 5% increase of your current notes.

Crescendance Bond: Piercing Radiance — Each Moonbeam Resonance stack you gain increases the incidental bonus amount by an additional 10% of your current notes (applied when stacks are consumed).

Duet: Lunar Phases — Activates a 20 second duet that cycles through four moon phases every 5 seconds (New Moon: 2× Notes Per Click; Crescent: component drops; Full Moon: 3× Notes Per Second; Waning: 50% upgrade cost). 4 minute cooldown. When resonating with Moonlight Sonata Major, all phases become active simultaneously for the duet duration.";


                        public const string EulogyOfTheMoon = @"Eulogy of the Moon

Passive: Nocturnal Refund — When you craft a minor score, there is a 25% chance to refund all materials used for that craft. During nighttime (8PM to 6AM) the refund chance increases to 50%.

Crescendance Bond: Requiem of Renewal — Consuming one Harmonizing Moonlight stack will:
    • Grant 3 Moonbeam Resonance stacks immediately.
    • If Moonbeam stacks reach 8, they auto-convert (in blocks of 8) into 1 Harmonizing Moonlight and each conversion grants +100% of your current notes.
    • Grant +1 Key, +1 Scale, and +1 Progression to the lowest-owned minor score.

Duet: Lunar Phases — Activates a 20 second duet that cycles through four moon phases every 5 seconds (see Moonlight duet summary). 4 minute cooldown. When resonating with Moonlight Sonata Major, all phases become active simultaneously for the duet duration.";

            // Eroica Weapons
            
            public const string SakurasBlossom = @"Sakura's Blossom

Passive: Heroic Bounty — When you complete a Major craft in Unite the Symphony, gain 1 of each component type (Key, Scale, Progression) for that score's associated minor.

Crescendance Bond: Crimson Requiem — Whenever a Symphonic Catharsis is consumed, the next 30 clicks become Blossom Crits. Each Blossom Crit awards immediate notes equal to NPC + (NPS × NPC).

Duet: Victory March — Fill a 100-click progress bar for escalating rewards (see Victory March duet). 5 minute cooldown.";

            public const string FuneralPrayer = @"Funeral Prayer

Passive: Prayer of Valor — Every 10th click grants 1 Prayer stack. When you reach 3 Prayer stacks they are consumed to trigger 15 Retribution Crits: the next 15 clicks each grant an additional +6 × Notes Per Second as instant notes.

Crescendance Bond: Testament of Harmony — When a Symphonic Catharsis is consumed, gain +1 Testament of Harmony. Consuming 1 Testament grants +100 Melodious, +100 Harmonious, and immediate notes equal to NPS × 10.

Duet: Victory March — Fill a 100-click progress bar for escalating rewards (see Victory March duet). 5 minute cooldown.";

            // Swan Lake Weapons
            public const string StarScatteredWings = @"Star-Scattered Wings

Passive: Stellar Fragment Rain — Every 10th click grants +5 Melodious and +5 Harmonious Fragments.

Crescendance Bond: Scattered Starlight — While equipped, obtaining any Feather during Swan Lake crescendance grants +1 Scattered Starlight stack (max 5). Scattered Starlight stacks are tracked separately and consumed by other Swan Lake effects.

Duet: Feather Cascade — First 10 clicks of the duet grant all three feather types (Revered, Chromatic, Polyphonic). Milestone clicks within the duet also grant immediate note bonuses. 20s duration | 4 minute cooldown.";
            

            public const string ThousandWingedSwan = @"Thousand Winged Swan

Passive: Wings of Fortune — Each time you acquire a new minor score, gain +33 Entropic Melodies.

Crescendance Bond: Wings of Velocity — Consuming Polyphonic Feathers grants a temporary Notes Per Click override for 7 seconds. While active, Notes Per Click is set to NPS × 10^stacks (10 to the power of stacks), where stacks is the number of Polyphonic Feather stacks used (maximum 8 stacks).

Duet: Feather Cascade — First 10 clicks of the duet grant all three feather types (Revered, Chromatic, Polyphonic). Milestone clicks within the duet grant immediate note bonuses. 20s duration | 4 minute cooldown.";

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

Passive: Chaotic Flux — Every 3rd click: ±25% of current notes (60% chance positive, 40% chance negative).

Crescendance Bond: Accelerated Mystery — While equipped, every 15th click grants +1 additional Resonant Mystery stack (in addition to the base every-10th stack).

Duet: Mystery Clicks — Each click during the duet triggers one random powerful effect (see duet details). 10s duration | 30 minute cooldown.";

            public const string Truthseeker = @"Truthseeker

Passive: Revelation Burst — When you purchase a weapon upgrade, immediately gain +5 Resonant Mystery stacks.

Crescendance Bond: Knowledge Harvest — For every 2 Resonant Mystery stacks you consume, gain +1 random owned minor score.

Duet: Mystery Clicks — Each click during the duet triggers one random powerful effect (see duet details). 10s duration | 30 minute cooldown.";

            public const string AstralChainripper = @"Astral Chainripper

Passive: Temporal Surge — When you craft a Fate minor score, temporarily set Notes Per Second to 5× its current value for 10 seconds (overrides base NPS).

Crescendance Bond: Stellar Convergence — For every 5 Cosmic Modulation stacks you gain, automatically grant +1 Symphony of the Stars stack.
• Symphony of the Stars (consumable): Consume 1 stack to instantly add +3 to your lowest-owned minor score.

Duet: Hourglass Reversal — First 10 seconds: bank the clicks/actions you perform. Last 10 seconds: replay the banked actions at X× effectiveness, where X equals the total clicks performed during the banking phase. 20s duration | 8 minute cooldown.";

            

            public const string CosmicWeaver = @"Cosmic Weaver

Passive: Harmonic Multiplication — When you craft a Fate minor score and you already own at least one extra copy of that major, grant +3 additional copies of that minor score (applies per craft).

Crescendance Bond: Stellar Cascade — Consuming a Symphony of the Stars stack grants a guaranteed set of Stellar Cascade critical clicks (high multiplier). Each Symphony consumed grants 5 Stellar Cascade crits.

Duet: Hourglass Reversal — First 10 seconds: bank actions; last 10 seconds: replay them at X× effectiveness (X = clicks performed during the banking phase). 20s duration | 8 minute cooldown.";


            public const string JoyfulCatharsis = @"Joyful Catharsis

Passive: Harmonic Entropy — Every 50th click grants +15 Entropic Melodies.

Crescendance Bond: Euphoric Resonance — While equipped, every Petal you gain (Harmony or Melody) immediately grants a 5 second double Notes Per Second effect; multiple grants stack their durations.

Duet: Crescendo Conductor — Build 16-note melodies by clicking during the duet for escalating rewards (rewards paid at 4/8/12/16 notes). 20s duration | 10 minute cooldown.";

            
            

            public const string OdeToCreation = @"Ode to Creation

Passive: Life Genesis — Every 20th click generates a random Petal (50% chance Harmony, 50% chance Melody).

Crescendance Bond: Life Amplification — While equipped, when an Ode to Life is consumed, petal generation is doubled for 25 seconds (petals are generated every 10th click during this period).

Duet: Crescendo Conductor — Build 16-note melodies by clicking during the duet for escalating rewards. 20s duration | 10 minute cooldown.";

            // Dies Irae Weapons
            public const string SevenCircles = @"Seven Circles

Passive: Wrathful Amplification — Every critical hit grants bonus notes equal to 20% of your current notes.

Crescendance Bond: Seal Resonance — Symphony of Hell's Retribution critical hits generate Wrathful Seals. Consuming 1 Wrathful Seal converts into 5 Seal-breaking Melody critical clicks (these crits use a heavy damage multiplier: 2 × NPC × NPS × Dissonant Hatred).

Duet: Infernal Symphony — During the duet, Seal-breaking Melody crits also grant Wrathful Seals. If you have 15 or more Wrathful Seals you may consume them to make your next 5 clicks double your current notes. 20s duration | 10 minute cooldown.";

            public const string HellsWrath = @"Hell's Wrath

Passive: Infernal Bounty — Every 20th click grants +2 to each of your two lowest-owned minor scores (ties chosen randomly).

Crescendance Bond: Entropic Fury — Dies Irae critical hits grant Entropic Melodies:
• Each Symphony of Hell's Retribution critical hit: +50 Entropic Melodies.
• Each Seal-breaking Melody critical hit: +50 Entropic Melodies.

Duet: Infernal Symphony — During duet, Seal-breaking Melody crits return Wrathful Seals; consuming 15+ Seals lets you convert them to five clicks that double your current notes. 20s duration | 10 minute cooldown.";

            // Winter Weapons
            public const string CacophonicBlizzard = @"Cacophonic Blizzard

Passive: Permafrost Strikes — Every 20th click freezes Notes Per Second (NPS) for 5 seconds and grants your next 20 clicks +50% notes each (applied immediately as bonus notes).

Crescendance Bond: Snow's Oblivion — When Eternal Frost is consumed, gain +2 Snow's Oblivion stacks. Consume 1 Snow's Oblivion to gain Entropic Melodies equal to your total upgrades owned.

Duet: Symphony of Absolute Zero — Freezes NPS for 20 seconds. Every click during the duet grants +1 Frigid Melody and extends the duet duration by +0.5 seconds (up to +10s total extension). 20s base duration | 4 minute cooldown.";

            public const string TheSnowsDesire = @"The Snow's Desire

Passive: Accelerating Flurry — Each click grants +1% Notes Per Click (stacking), up to a maximum of +50%. The accumulated bonus decays to 0% after 5 seconds without clicking.

Crescendance Bond: Frigid Resonance — While equipped and resonating with Winter, effects that grant Frigid Melody grant +2 Frigid Melody instead of +1 (doubling Frigid Melody gain).

Duet: Symphony of Absolute Zero — Freezes NPS for 20 seconds. Each click during the duet grants +1 Frigid Melody and extends duration by +0.5 seconds (max +10s). 20s base duration | 4 minute cooldown.";

            // Clair de Lune Weapons
            public const string CelestialHorology = @"Celestial Horology
Passive: Chrono-Entropic Refund
• Every 5th click grants +3% Entropic Melodies
• Anytime you spend Entropic Melodies, refund 50% of them

Crescendance Bond: Celestial Convergence
• Every 5th Shattered Moonlight consumed grants +1 Temporal Fracture stack

Duet: Chain of Temporality
For 12 seconds, each click doubles your Notes Per Click multiplier (1× → 2× → 4× → 8×...), stacking infinitely. 30 min cooldown.
";

            public const string MetronomicDissonance = @"Clockwork Finality
Passive: Temporal Dissolution
• When dissolving major scores, gain double the Entropic Melodies

Crescendance Bond: Surge Amplification
• Consuming Temporal Fracture grants 10 Surge of Time's Fractalization critical hits (NPC^144)
• Only active when weapon ability is equipped

Duet: Chain of Temporality
For 12 seconds, each click doubles your Notes Per Click multiplier (1× → 2× → 4× → 8×...), stacking infinitely. 30 min cooldown.
";

            // Mercury Weapons
            public const string MercurialOverture = @"Mercurial Overture

Passive: Mercurial Strike — Every 3rd click grants bonus notes equal to (NPC × NPC) + (Entropic Melodies × 1,000).

Crescendance Bond: Planetary Resonance — Every Herald's Message consumed grants +1 Planetary Alignment stack and your next 5 clicks become Messenger of Planetary Resonance crits: (NPS × 5,000) + (NPC × log₁₀(NPC+1)⁸ × Planetary Alignment stacks).

Duet: Swift Delivery — Activate Mercury's Haste for 30 seconds. Every click grants +1 Swift Resonance and deals (NPC × NPS × log₁₀(NPC+1)^stacks) crit damage. Ends with NPS×stacks bonus. 3 min cooldown.";

            public const string WingOfTheMessenger = @"Wing of the Messenger

Passive: Messenger's Gift — Every 50th click grants (Entropic Melodies ÷ 5) to your lowest owned minor score.

Crescendance Bond: A Thousand Wings — Consume Planetary Alignment stacks to grant your next 5 clicks the critical 'A Thousand Winged Symphony for the Universe!!!' (NPC × NPS × log₁₀(NPC+1)^20 × stacks).

Duet: Swift Delivery — Activate Mercury's Haste for 30 seconds. Every click grants +1 Swift Resonance and deals (NPC × NPS × log₁₀(NPC+1)^stacks) crit damage. Ends with NPS×stacks bonus. 3 min cooldown.";

            // Mars Weapons
            public const string FractalOfWar = @"Fractal of War

Passive: Recursive Devastation — Every 25th click doubles your Entropic Melodies and grants +10 to ALL 8 upgrades (Chord, Scale, Orchestra, Symphony, Aria, Requiem, Opus, Magnum Opus).

Crescendance Bond: Fractal Armament — Each 'Infinite War of Harmony!!!' critical hit grants +1 Oblivion's Destruction stack. Consume 5 Oblivion's Destruction for +30 to all owned minor scores.

Duet: Annihilation Nocturne — For 12 seconds, every click grants NPC^(NPC/NPS) notes and +2 to all owned Major Scores. Instantly sets Blade to 300% charge. 10 min cooldown.";

            public const string ConsonanceRequiemicWar = @"Consonance's Requiemic War

Passive: Requiem's Crescendo — Whenever you purchase ANY upgrade (Chord, Scale, etc.), instantly gain +5% of current notes.

Crescendance Bond: Requiemic Resonance — When you consume Resolute Fractal or Oblivion's Destruction stacks, your next 3 clicks receive NPC^1.56 bonus damage.

Duet: Annihilation Nocturne — For 12 seconds, every click grants NPC^(NPC/NPS) notes and +2 to all owned Major Scores. Instantly sets Blade to 300% charge. 10 min cooldown.";
        }

        // ============================================================
        // FORTE RESONANCE (Armory screen - individual weapon effects)
        // ============================================================
        
        public static class ForteResonance
        {
            // Moonlight
            public const string IncisorOfMoonlight = "Passive: Moonlight Strike — Every 4th click grants bonus notes equal to 1% of your current notes (5% during nighttime 8PM to 6AM).\n\nUpgrade Note: Purchasing the Incisor upgrade grants an immediate 5% increase of current notes.\n\nCrescendance Bond: Piercing Radiance — Each Moonbeam Resonance stack increases the bonus amount by +10% of current notes when stacks are consumed.";
            public const string EulogyOfTheMoon = "Passive: Nocturnal Refund — On minor score craft: 25% chance to refund all crafting materials (50% during nighttime 8PM to 6AM).\n\nCrescendance Bond: Requiem of Renewal — Consuming one Harmonizing Moonlight stack grants 3 Moonbeam Resonance stacks. If Moonbeam stacks reach 8, they auto-convert (in blocks of 8) into 1 Harmonizing Moonlight and each conversion grants +100% of your current notes. Also grants +1 Key, +1 Scale, and +1 Progression to the lowest-owned minor score.";
            
            // Eroica
            public const string SakurasBlossom = "Passive: Heroic Bounty — On Major craft: grant 1 Key, 1 Scale, and 1 Progression for that score's associated minor.\n\nCrescendance Bond: Crimson Requiem — When a Symphonic Catharsis is consumed, the next 30 clicks are Blossom Crits that award immediate notes equal to Notes Per Click + (Notes Per Second × Notes Per Click).";
            public const string FuneralPrayer = "Passive: Prayer of Valor — Every 10th click grants 1 Prayer stack. At 3 Prayer stacks, they are consumed to trigger 15 Retribution Crits: the next 15 clicks each grant an additional +6 × Notes Per Second as instant notes.\n\nCrescendance Bond: Testament of Harmony — When a Symphonic Catharsis is consumed, gain +1 Testament of Harmony. Consuming 1 Testament grants +100 Melodious, +100 Harmonious, and immediate notes equal to Notes Per Second × 10.";
            
            // Swan Lake
            public const string StarScatteredWings = "Passive: Stellar Fragment Rain — Every 10th click grants +5 Melodious and +5 Harmonious Fragments.\n\nCrescendance Bond: Scattered Starlight — While equipped, obtaining any Feather during Swan Lake crescendance grants +1 Scattered Starlight stack (max 5). Scattered Starlight stacks are tracked separately and consumed by other Swan Lake effects.";
            public const string ThousandWingedSwan = "Passive: Wings of Fortune — Each time you acquire a new minor score, gain +33 Entropic Melodies.\n\nCrescendance Bond: Wings of Velocity — Consuming Polyphonic Feathers grants a temporary Notes Per Click override for 7 seconds. While active, Notes Per Click is set to Notes Per Second × 10^stacks (where stacks is the number of Polyphonic Feather stacks used, max 8).";
            
            // La Campanella
            public const string SymphonyOfBells = "Passive: Harmonic Duplication — On minor score craft: duplicate that minor (+1 extra copy).\n\nCrescendance Bond: Resonant Crack Bonus — When the Grandiose Bell cracks (20/40/60 clicks), gain +1 Deafening Chime stack (applies up to the 15-stack cap).";
            public const string RazerOfBellsChimes = "Passive: Component Echo — On minor score craft: refund 2 random components for that minor.\n\nCrescendance Bond: Entropic Resonance — If mended at Radiant stage, gain +5 Deafening Chime stacks; additionally, when mended at Radiant while this weapon is equipped, grant +50 Entropic Melodies.";
            
            // Fate
            public const string AstralChainripper = "Passive: Temporal Surge — On crafting a Fate minor score: temporarily set Notes Per Second to 5× for 10 seconds.\n\nCrescendance Bond: Stellar Convergence — Every 5 Cosmic Modulation stacks gained: grant +1 Symphony of the Stars stack.\n• Symphony of the Stars: Consume 1 stack to add +3 to your lowest-owned minor score.";
            public const string CosmicWeaver = "Passive: Harmonic Multiplication — On Fate minor craft: if you own an extra copy of that major, gain +3 additional copies of that minor score.\n\nCrescendance Bond: Stellar Cascade — Each Symphony of the Stars consumed grants 5 Stellar Cascade critical clicks (very high multiplier).";
            
            // Enigma
            public const string CreatorOfMystery = "Passive: Chaotic Flux — Every 3rd click: ±25% of current notes (60% chance positive, 40% chance negative).\n\nCrescendance Bond: Accelerated Mystery — While equipped, every 15th click grants +1 additional Resonant Mystery stack.";
            public const string Truthseeker = "Passive: Revelation Burst — On weapon upgrade purchase, immediately gain +5 Resonant Mystery stacks.\n\nCrescendance Bond: Knowledge Harvest — For every 2 Resonant Mystery stacks consumed, gain +1 random owned minor score.";
            
            // Ode to Joy
            public const string JoyfulCatharsis = "Passive: Harmonic Entropy — Every 50th click grants +15 Entropic Melodies.\n\nCrescendance Bond: Euphoric Resonance — Every time you gain any Petal (Harmony or Melody), immediately grants a 5 second double Notes Per Second effect; multiple grants stack their durations.";
            public const string OdeToCreation = "Passive: Life Genesis — Every 20th click generates a random Petal (50% Harmony, 50% Melody).\n\nCrescendance Bond: Life Amplification — When Ode to Life is consumed, petal generation is doubled for 25 seconds (petals are generated every 10th click during this period).";
            
            // Dies Irae
            public const string SevenCircles = "Passive: Wrathful Amplification — Every critical hit grants bonus notes equal to 20% of your current notes.\n\nCrescendance Bond: Seal Resonance — Symphony of Hell's Retribution critical hits generate Wrathful Seals. Consuming 1 Wrathful Seal converts into 5 Seal-breaking Melody critical clicks (these crits use a heavy damage multiplier: 2 × Notes Per Click × Notes Per Second × Dissonant Hatred).";
            public const string HellsWrath = "Passive: Infernal Bounty — Every 20th click grants +2 to each of your two lowest-owned minor scores (ties chosen randomly).\n\nCrescendance Bond: Entropic Fury — Dies Irae critical hits grant Entropic Melodies:\n• Each Symphony of Hell's Retribution critical hit: +50 Entropic Melodies.\n• Each Seal-breaking Melody critical hit: +50 Entropic Melodies.";
            
            // Winter
            public const string CacophonicBlizzard = "Passive: Permafrost Strikes — Every 20th click freezes Notes Per Second for 5 seconds and grants your next 20 clicks +50% notes each (applied immediately as bonus notes).\n\nCrescendance Bond: Snow's Oblivion — When Eternal Frost is consumed, gain +2 Snow's Oblivion stacks. Consume 1 Snow's Oblivion to gain Entropic Melodies equal to your total upgrades owned.";
            public const string TheSnowsDesire = "Passive: Accelerating Flurry — Every click grants +1% Notes Per Click (max +50%). Decays to 0% after 5 seconds of no clicking. Current bonus displayed in Crescendance panel.\n\nCrescendance Bond: Frigid Resonance — Whenever you gain 1 Frigid Melody stack, gain +1 bonus stack (doubling effect).";
            
            // Clair de Lune
            public const string CelestialHorology = "Passive: Chrono-Entropic Refund — Every 5th click grants +3% Entropic Melodies. Anytime you spend Entropic Melodies, refund 50% of them.\n\nCrescendance Bond: Celestial Convergence — Every 5th Shattered Moonlight consumed grants +1 Temporal Fracture stack.";
            public const string MetronomicDissonance = "Passive: Temporal Dissolution — When dissolving major scores, gain double the Entropic Melodies.\n\nCrescendance Bond: Surge Amplification — Consuming Temporal Fracture grants 10 Surge of Time's Fractalization critical hits (NPC^144). Only active when weapon ability is equipped.";
            
            // Mercury
            public const string MercurialOverture = "Passive: Mercurial Strike — Every 3rd click grants bonus notes equal to (Notes Per Click × Notes Per Click) + (Entropic Melodies × 1,000).\n\nCrescendance Bond: Planetary Resonance — Every Herald's Message consumed grants +1 Planetary Alignment stack and your next 5 clicks become Messenger of Planetary Resonance crits: (Notes Per Second × 5,000) + (Notes Per Click × log₁₀(NPC+1)⁸ × Planetary Alignment stacks).";
            public const string WingOfTheMessenger = "Passive: Messenger's Gift — Every 50th click grants (Entropic Melodies ÷ 5) to your lowest owned minor score.\n\nCrescendance Bond: A Thousand Wings — Consume Planetary Alignment stacks to grant your next 5 clicks the critical 'A Thousand Winged Symphony for the Universe!!!' with damage equal to NPC × NPS × log₁₀(NPC+1)^20 × stacks.";
            
            // Mars
            public const string FractalOfWar = "Passive: Recursive Devastation — Every 25th click doubles your Entropic Melodies and grants +10 to ALL 8 upgrades (Chord, Scale, Orchestra, Symphony, Aria, Requiem, Opus, Magnum Opus).\n\nCrescendance Bond: Fractal Armament — Each 'Infinite War of Harmony!!!' critical hit grants +1 Oblivion's Destruction stack. Consume 5 Oblivion's Destruction for +30 to all owned minor scores.";
            public const string ConsonanceRequiemicWar = "Passive: Requiem's Crescendo — Whenever you purchase ANY upgrade (Chord, Scale, etc.), instantly gain +5% of current notes.\n\nCrescendance Bond: Requiemic Resonance — When you consume Resolute Fractal or Oblivion's Destruction stacks, your next 3 clicks receive NPC^1.56 bonus damage.";
        }

        // ============================================================
        // DUET ABILITIES
        // ============================================================
        
        public static class Duets
        {
            public static class Names
            {
                public const string Moonlight = "Moonlight Sonata:\nLunar Phases";
                public const string DiesIrae = "Wrathful Orchestration:\nInfernal Symphony";
                public const string Winter = "Requiem of the Frozen Choir:\nSymphony of Absolute Zero";
                public const string Eroica = "Symphony of Heroes:\nVictory March";
                public const string SwanLake = "Wings of Transcendence:\nFeather Cascade";
                public const string LaCampanella = "Bell Resonance:\nChime Chain";
                public const string Enigma = "Enigmatic Puzzle:\nMystery Clicks";
                public const string Fate = "Threads of Destiny:\nHourglass Reversal";
                public const string OdeToJoy = "Symphony of Celebration:\nCrescendo Conductor";
                public const string ClairDeLune = "Chain of Temporality";
                public const string Mercury = "Celestial Messengers";
                public const string Mars = "Annihilation Nocturne";
            }

            public static class Full
            {
                public const string Moonlight = "Cycles through 4 moon phases every 5 seconds:\n• New Moon: 2× Notes Per Click\n• Crescent: 10% component drop chance per click\n• Full Moon: 3× Notes Per Second\n• Waning: Upgrades cost 50% less\n\nSPECIAL: All phases active simultaneously when resonating with Moonlight Sonata Major!\n\n20 second duration | 4 minute cooldown";
                public const string DiesIrae = "For 10 seconds, every click is a guaranteed Seal-breaking Melody of Oblivion critical hit that returns +1 Wrathful Seal.\n\nDuring the duet, when you have 15+ Wrathful Seals, you can consume them to grant your next 5 clicks the power to double your current notes.\n\n10 second duration | 10 minute cooldown";
                public const string Winter = "Freezes Notes Per Second for 20 seconds. Every click during freeze:\n• Grants +1 Frigid Melody stack\n• Extends duration by +0.5 seconds (max +10s extension = 30s total)\n\n20 second base duration | 4 minute cooldown";
                public const string Eroica = "Fill progress bar with 100 clicks:\n• 25% progress: Random minor score\n• 50% progress: Random major score\n• 75% progress: +50 Melodious and Harmonious Fragments\n• 100% complete: Double all owned scores\n\n5 minute cooldown";
                public const string SwanLake = "Each of your first 10 clicks grants 1 Revered Feather, 1 Chromatic Feather, and 1 Polyphonic Feather. Every 5th click (5th and 10th) increases current notes by +25%.\n\n20 second duration | 4 minute cooldown";
                public const string LaCampanella = "Click to create chimes. Clicking within 1 second extends chain. Chain reward equals (chain length)² × Notes Per Second as instant notes. Chain breaks after 1 second of no clicks.\n\n10 second duration | 10 minute cooldown";
                public const string Enigma = "Each click triggers 1 random effect:\nRed: 5× Notes Per Second as instant notes | Blue: +10% Notes Per Second boost | Green: +30 Harmonious Fragments | Yellow: +30 Melodious Fragments | Purple: +1 major score per owned type | Orange: +1 minor component per owned minor type | White: +1 to 3 random minor scores | Black: -65% current notes\n\n10 second duration | 30 minute cooldown";
                public const string Fate = "All actions performed during the first 10 seconds will be replayed during the last 10 seconds at X× effectiveness, where X equals the number of clicks you performed during the first half.\n\nDuration: 20 seconds | Cooldown: 8 minutes";
                public const string OdeToJoy = "Build 16-note melodies by clicking (repeatable). Every 16 notes resets counter and grants rewards:\n• 4 notes: +25 Melodious and Harmonious Fragments\n• 8 notes: +5 Petals of Harmony\n• 12 notes: +5 Petals of Melody\n• 16 notes: +1 Ode to Life\nOn ability expiry: +3 Entropic Melodies per completed 4-note section\n\n20 second duration | 10 minute cooldown";
                public const string ClairDeLune = "For 12 seconds, each click doubles your Notes Per Click multiplier (1× → 2× → 4× → 8× → 16×...). The multiplier is applied before calculating notes gained from each click, allowing for exponentially scaling power.\n\n12 second duration | 30 minute cooldown\n\n✦ RECOMMENDED: Resonating with Clair de Lune Major enhances the power of time itself!";
                public const string Mercury = "Swift Delivery: Activate Mercury's Haste for 30 seconds (3 min cooldown).\n\nDuring Mercury's Haste:\n• Every click grants +1 Swift Resonance\n• Every click is an 'Alignment of Mercurial Judgement' crit: ((NPC × NPS)^stacks) notes\n\nWhen the effect ends: Consume all Swift Resonance for NPS × stacks bonus notes.\n\n✦ RECOMMENDED: Resonating with Mercury Major amplifies the messenger's swift delivery!";
                public const string Mars = "For 12 seconds, every click becomes an Annihilation Nocturne critical:\n\n• Each click grants notes equal to NPC^(NPC/NPS)\n• Each click grants +2 to every owned Major Score\n• Instantly sets Blade of Symphonic War to 300% charge\n\n12 second duration | 10 minute cooldown\n\n✦ RECOMMENDED: Resonating with Mars Major unleashes the full fury of war!\n\nThe blade sings its final song—let annihilation's nocturne echo through eternity.";
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
                public const string ClairDeLune = "Chain of Temporality: For 12 seconds, each click doubles your Notes Per Click multiplier (1× → 2× → 4× → 8×...), stacking infinitely. ✦ Resonating with Clair de Lune Major recommended! - 30 minute cooldown.";
                public const string OdeToJoy = "Crescendo Conductor: Build 16-note melodies (repeatable/stackable). Rewards at 4/8/12/16 notes (+25 fragments, +5 Petals of Harmony, +5 Petals of Melody, +1 Ode to Life). On expiry: +3 Entropic Melodies per completed section - 20 second duration, 10 minute cooldown.";
                public const string Mercury = "Swift Delivery: 30s of Mercury's Haste where every click grants +1 Swift Resonance and deals ((NPC×NPS)^stacks) crit damage. Ends with NPS×stacks bonus notes. ✦ Resonating with Mercury Major recommended! - 3 min cooldown.";
                public const string Mars = "Annihilation Nocturne: For 12 seconds, every click grants NPC^(NPC/NPS) notes and +2 to all owned Major Scores. Instantly sets Blade to 300% charge. ✦ Resonating with Mars Major recommended! - 10 minute cooldown.";
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
                public const int ClairDeLune = 12; // Chain of Temporality duration
                public const int Mercury = 30;     // Swift Delivery duration
                public const int Mars = 12;        // Annihilation Nocturne duration
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
                public const int ClairDeLune = 1800;   // 30 minutes (Chain of Temporality)
                public const int Mercury = 180;        // 3 minutes (Swift Delivery)
                public const int Mars = 600;           // 10 minutes (Annihilation Nocturne)
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
                "Clair de Lune" => MajorScores.ClairDeLune,
                "Mercury" => MajorScores.Mercury,
                "Mars" => MajorScores.Mars,
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
                "Mercurial Overture" => Weapons.MercurialOverture,
                "Wing of the Messenger" => Weapons.WingOfTheMessenger,
                "Celestial Horology" => Weapons.CelestialHorology,
                "Clockwork Finality" => Weapons.MetronomicDissonance,
                "Fractal of War" => Weapons.FractalOfWar,
                "Consonance's Requiemic War" => Weapons.ConsonanceRequiemicWar,
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
                "Mercurial Overture" => ForteResonance.MercurialOverture,
                "Wing of the Messenger" => ForteResonance.WingOfTheMessenger,
                "Celestial Horology" => ForteResonance.CelestialHorology,
                "Clockwork Finality" => ForteResonance.MetronomicDissonance,
                "Fractal of War" => ForteResonance.FractalOfWar,
                "Consonance's Requiemic War" => ForteResonance.ConsonanceRequiemicWar,
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
