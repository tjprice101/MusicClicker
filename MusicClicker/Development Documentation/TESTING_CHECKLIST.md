# Testing Checklist for MusicClicker - All Major Scores & Weapons

## MOONLIGHT SONATA

### Crescendance: Eclipse of the Nocturne
- [ ] Every 20th click during nighttime (8PM-6AM) grants 1 Moonbeam Resonance stack
- [ ] No Moonbeam Resonance stacks gained during daytime (6AM-8PM)
- [ ] At 8 Moonbeam Resonance stacks, auto-consume triggers
- [ ] Auto-consume grants +100% of current notes
- [ ] Auto-consume grants 1 Harmonizing Moonlight stack
- [ ] Auto-consume reduces Moonbeam stacks by 8 (not to 0)
- [ ] Both weapon crescendance bonds are empowered when conditions met

### Incisor of Moonlight Weapon
- [ ] Passive: Every 4th click grants +3% notes (daytime)
- [ ] Passive: Every 4th click grants +10% notes (nighttime 8PM-6AM)
- [ ] Crescendance Bond (Piercing Radiance): Each Moonbeam Resonance stack gained grants +10% additional notes
- [ ] Crescendance Bond requires: Moonlight Major resonated + Incisor equipped
- [ ] Duet (Lunar Phases): New Moon phase grants 2× NPC
- [ ] Duet: Crescent phase grants 10% component drop per click
- [ ] Duet: Full Moon phase grants 3× NPS
- [ ] Duet: Waning phase grants 50% upgrade cost discount
- [ ] Duet: All phases activate simultaneously when Moonlight Major is resonated
- [ ] Duet: 20 second duration, 4 minute cooldown

### Eulogy of the Moon Weapon
- [ ] Passive (Nocturnal Refund): 25% refund chance on minor crafts (daytime)
- [ ] Passive: 50% refund chance on minor crafts (nighttime 8PM-6AM)
- [ ] Crescendance Bond (Requiem of Renewal): Consume 1 Harmonizing Moonlight grants 3 Moonbeam Resonance
- [ ] Crescendance Bond: Also grants 1 of each component for lowest owned minor
- [ ] Crescendance Bond requires: Moonlight Major resonated + Eulogy equipped
- [ ] Duet (Lunar Phases): Same as Incisor - all 4 phases tested

---

## EROICA (SYMPHONY NO. 3)

### Major Score Abilities
- [ ] Minor scores beyond the 10th grant double NPS each
- [ ] Verify calculation: 11th minor onwards should each add 2× their value to NPS

### Crescendance: Heroic Momentum
- [ ] Every minor craft grants +1 Heroic Resolve stack
- [ ] Can consume 1 Heroic Resolve + 25% notes for +1 Symphonic Catharsis stack
- [ ] Consume Symphonic Catharsis: Grants 10s of double NPC
- [ ] Consume Symphonic Catharsis: Grants 10s of +10% critical hit chance
- [ ] Triggers Sakura weapon bond (Crimson Requiem) when conditions met
- [ ] Triggers Funeral Prayer weapon bond (Testament of Harmony) when conditions met

### Sakura's Blossom Weapon
- [ ] Forte Resonance (Heroic Bounty): On Major acquisition, grants +1 of each component (Keys, Scales, Progressions)
- [ ] Components granted match the Major's score type
- [ ] Crescendance Bond (Crimson Requiem): Consuming Symphonic Catharsis triggers special burst
- [ ] Crescendance Bond: Next 30 clicks grant bonus = NPC + (NPS × NPC) each
- [ ] Crescendance Bond requires: Eroica Major resonated + Sakura equipped
- [ ] Duet (Victory March): 100-click progress bar fills correctly
- [ ] Duet: 25% progress grants random minor score
- [ ] Duet: 50% progress grants random major score
- [ ] Duet: 75% progress grants +50 fragments (Melodious & Harmonious)
- [ ] Duet: 100% progress doubles all owned scores
- [ ] Duet: 5 minute cooldown

### Funeral Prayer Weapon
- [ ] Forte Resonance (Prayer of Valor): Every 10th click grants +1 Prayer stack
- [ ] Forte Resonance: At 3 Prayer stacks, next 15 clicks gain +6× NPS each
- [ ] Crescendance Bond (Testament of Harmony): Consuming Symphonic Catharsis grants +1 Testament stack
- [ ] Crescendance Bond: Consume 1 Testament grants +100 Melodious + 100 Harmonious Fragments
- [ ] Crescendance Bond requires: Eroica Major resonated + Funeral Prayer equipped
- [ ] Duet (Victory March): Same as Sakura - all milestones tested

---

## SWAN LAKE

### Crescendance: Wings of Transcendence
- [ ] Revered Feathers: Every 10 clicks has 5% chance to grant 1 feather
- [ ] Revered Feathers: Consume 5 for +20% notes
- [ ] Chromatic Feathers: Every 15 clicks has 5% chance to grant 1 feather
- [ ] Chromatic Feathers: Consume 10 for +2 all minor scores
- [ ] Polyphonic Feathers: Every 30 clicks has 5% chance to grant 1 feather
- [ ] Polyphonic Feathers: Consume 1 for +250 entropic melodies
- [ ] Polyphonic Feathers: Consume also grants +75% notes

### Star-Scattered Wings Weapon
- [ ] Forte Resonance (Stellar Fragment Rain): Every 10th click grants +5 Melodious & +5 Harmonious Fragments
- [ ] Forte Resonance: Feather drops grant +2 components to random owned minor
- [ ] Duet (Feather Cascade): First 10 clicks each grant 1 of each feather type (Revered, Chromatic, Polyphonic)
- [ ] Duet: 5th click grants +25% notes bonus
- [ ] Duet: 10th click grants +25% notes bonus
- [ ] Duet: 20 second duration, 4 minute cooldown

### Thousand Winged Swan Weapon
- [ ] Forte Resonance (Wings of Fortune): On acquisition grants +50% notes instantly (one-time only)
- [ ] Forte Resonance: Polyphonic feather consumed grants 10s of NPS-to-NPC boost
- [ ] Forte Resonance: NPS-to-NPC adds 2× NPS to NPC during 10s window
- [ ] Duet (Feather Cascade): Same as Star-Scattered Wings - all feathers and bonuses tested

---

## LA CAMPANELLA

### Crescendance: Grandiose Bell
- [ ] Bell cracks at 20 clicks (Intact → Crescending)
- [ ] Bell cracks at 40 clicks (Crescending → Radiant)
- [ ] Bell cracks at 60 clicks (Radiant → Harmonizing)
- [ ] Mend at Crescending stage: Grants +2 random owned minors
- [ ] Mend at Radiant stage: Next 5 clicks are entropic crits
- [ ] Mend at Radiant stage: Grants 5 Deafening Chime stacks
- [ ] Mend at Harmonizing stage: Consumes all Deafening Chime stacks
- [ ] Mend at Harmonizing stage: Multiplies notes by 2^(Deafening Chime stacks)
- [ ] Symphony weapon bond: +75% notes when bell cracks (if equipped)
- [ ] Razer weapon bond: Mend grants entropic melodies = click count, max 250 (if equipped)

### Symphony of Bells Weapon
- [ ] Passive (Harmonic Duplication): On minor craft, duplicates that minor (+1 extra copy)
- [ ] Crescendance Bond (Resonant Crack Bonus): Bell crack grants +75% notes instantly
- [ ] Crescendance Bond requires: La Campanella Major resonated + Symphony equipped
- [ ] Duet (Chime Chain): Click chains build within 1-second windows
- [ ] Duet: Reward = (chain length)² × NPS as instant notes
- [ ] Duet: Chain breaks after 1 second without clicking
- [ ] Duet: 10 second duration, 10 minute cooldown

### Razer of Bell's Chimes Weapon
- [ ] Passive (Component Echo): On minor craft, refunds 2 random components for that minor
- [ ] Crescendance Bond (Entropic Resonance): Mend bell grants Entropic Melodies = click count
- [ ] Crescendance Bond: Max 250 Entropic Melodies per mend
- [ ] Crescendance Bond requires: La Campanella Major resonated + Razer equipped
- [ ] Duet (Chime Chain): Same as Symphony - chain mechanics tested

---

## ENIGMA VARIATIONS

### Crescendance: Resonate Mystery
- [ ] Every 10th click grants +1 Resonate Mystery stack
- [ ] With Creator of Mystery weapon: Every 15th click grants +1 additional stack
- [ ] Consume 1 stack grants +50% of current notes
- [ ] Consume 1 stack grants +50 Entropic Melodies
- [ ] Consume 1 stack grants +1 random owned minor
- [ ] With Truthseeker weapon bond: For every 2 stacks consumed, grants 1 random owned minor
- [ ] Bulk consume (10+ stacks): Grants +25% notes per stack consumed
- [ ] Bulk consume: Truthseeker bonus (1 minor per 2 stacks) still applies

### Creator of Mystery Weapon
- [ ] Passive (Chaotic Flux): Every 3rd click triggers ±25% notes
- [ ] Passive: 60% chance positive, 40% chance negative
- [ ] Crescendance Bond (Accelerated Mystery): Every 15th click grants +1 additional Resonate Mystery stack
- [ ] Crescendance Bond requires: Enigma Major resonated + Creator equipped
- [ ] Duet (Mystery Clicks): Red effect grants 5× NPS as instant notes
- [ ] Duet: Blue effect grants +10% NPS boost
- [ ] Duet: Green effect grants +30 Harmonious Fragments
- [ ] Duet: Yellow effect grants +30 Melodious Fragments
- [ ] Duet: Purple effect grants +1 major per owned type
- [ ] Duet: Orange effect grants +1 component per owned minor type
- [ ] Duet: White effect grants +1 to 3 random minors
- [ ] Duet: Black effect removes -65% current notes
- [ ] Duet: 10 second duration, 30 minute cooldown

### Truthseeker Weapon
- [ ] Passive (Revelation Burst): On upgrade purchase, grants +5 Resonant Mystery stacks
- [ ] Crescendance Bond (Knowledge Harvest): For every 2 Resonate Mystery stacks consumed, grants +1 random owned minor
- [ ] Crescendance Bond requires: Enigma Major resonated + Truthseeker equipped
- [ ] Duet (Mystery Clicks): Same as Creator - all 8 random effects tested

---

## FATE (SYMPHONY NO. 5)

### Crescendance: Cosmic Modulation
- [ ] Every 8th click grants +1 Cosmic Modulation stack
- [ ] Every 8th click grants +10% notes bonus
- [ ] Tiers activate at every 5 stacks (5, 10, 15, 20, etc.) with escalating passive effects
- [ ] Consume Cosmic Modulation grants +15 Entropic Melodies per stack consumed
- [ ] Astral Chainripper bond: Every 5 stacks gained grants +1 Symphony of the Stars
- [ ] Symphony of the Stars: Consume 1 stack grants +3 to lowest owned minor score
- [ ] Cosmic Weaver bond: Each Symphony consumed grants next 5 clicks as guaranteed entropic crits

### Astral Chainripper Weapon
- [ ] Passive (Temporal Surge): Crafting Fate minor grants 5× NPS for 10s
- [ ] Crescendance Bond (Stellar Convergence): Every 5 Cosmic Modulation stacks grants +1 Symphony of the Stars
- [ ] Crescendance Bond requires: Fate Major resonated + Astral Chainripper equipped
- [ ] Duet (Hourglass Reversal): First 10 seconds banks all actions
- [ ] Duet: Last 10 seconds replays actions at X× effectiveness (X = click count)
- [ ] Duet: 20 second duration, 8 minute cooldown

### Cosmic Weaver Weapon
- [ ] Passive (Harmonic Multiplication): On minor craft, if you own 1+ of that major, gain +3 of that major score
- [ ] Crescendance Bond (Entropic Starfall): Each Symphony of the Stars consumed grants next 5 clicks as guaranteed entropic crits
- [ ] Crescendance Bond requires: Fate Major resonated + Cosmic Weaver equipped
- [ ] Duet (Hourglass Reversal): Same as Astral Chainripper - action replay tested

---

## ODE TO JOY (SYMPHONY NO. 9)

### Crescendance: Petals of Life
- [ ] Minor craft grants +1 Petal of Harmony
- [ ] Major craft grants +1 Petal of Melody
- [ ] Consume Harmony grants +250 Entropic Melodies
- [ ] Consume Melody grants +10s full entropic crits (stackable)
- [ ] Combine (1 Harmony + 1 Melody + 50 Entropic) creates 1 Ode to Life
- [ ] Consume Ode to Life doubles all owned minor scores

### Joyful Catharsis Weapon
- [ ] Passive (Harmonic Entropy): Every 50th click grants +X Entropic Melodies (X = critical notes gotten × 3)
- [ ] Crescendance Bond (Euphoric Resonance): Every Petal gained doubles NPS for 5s
- [ ] Crescendance Bond: Effect stacks with multiple petals gained
- [ ] Crescendance Bond requires: Ode to Joy Major resonated + Joyful Catharsis equipped
- [ ] Duet (Crescendo Conductor): 4 notes grants +25 fragments
- [ ] Duet: 8 notes grants +5 Petals of Harmony
- [ ] Duet: 12 notes grants +5 Petals of Melody
- [ ] Duet: 16 notes grants +1 Ode to Life (repeatable/stackable)
- [ ] Duet: On expiry, grants +3 Entropic Melodies per completed 4-note section
- [ ] Duet: 20 second duration, 10 minute cooldown

### Ode to Creation Weapon
- [ ] Passive (Rhythmic Genesis): Every 20th click generates random Petal (Harmony or Melody)
- [ ] Crescendance Bond (Life Amplification): On Ode to Life consume, doubles passive effect for 25s
- [ ] Crescendance Bond: During doubled effect, generates petals every 10th click instead of 20th
- [ ] Crescendance Bond requires: Ode to Joy Major resonated + Ode to Creation equipped
- [ ] Duet (Crescendo Conductor): Same as Joyful Catharsis - all milestones tested

---

## DIES IRAE

### Crescendance: Symphony of Hell's Retribution
- [ ] Every click grants +1 Burning Hatred (up to 50 max)
- [ ] After 50 Burning Hatred, clicks grant +1 Discordant Malice instead
- [ ] Consume 5 Burning Hatred grants +1 Dissonant Oblivion
- [ ] Consume Discordant Malice grants +X Entropic Melodies (X = current Burning Hatred stacks)
- [ ] Consume Dissonant Oblivion grants next 20 clicks as Symphony of Hell's Retribution
- [ ] Hell crit formula: NPC × NPS × Burning Hatred stacks

### Duet: Seven Seals
- [ ] Each click during duet places 1 seal
- [ ] At 7 seals placed, grants 3 random minor scores without consuming components
- [ ] Effect stacks during 15 second duration (can trigger multiple times)
- [ ] 15 second duration, 3.5 minute cooldown

---

## WINTER

### Crescendance: Eternal Frost
- [ ] Activating freezes NPS (stops passive note generation)
- [ ] Frozen NPS converts to click multiplier
- [ ] Each click extends duration by +0.5 seconds
- [ ] Maximum extension is +10 seconds total
- [ ] 15 second base duration, 5 minute cooldown

### Duet: Absolute Zero
- [ ] Same mechanics as Crescendance tested
- [ ] NPS properly frozen and converted to click power
- [ ] Click extension works correctly

---

## GENERAL TESTING

### UI & Display
- [ ] All tooltips display correctly without abbreviations (NPS → Notes Per Second, NPC → Notes Per Click)
- [ ] No emojis appear in any text fields
- [ ] Crescendance panels display correctly for each major score on main screen
- [ ] Crescendance panel sits flush at bottom (Margin 0,0,0,0)
- [ ] Event screen crescendance descriptions wrap correctly (MaxWidth 525)

### Equipment System
- [ ] Weapon passives only work while equipped (not permanently on unlock)
- [ ] Equipping weapon activates passive abilities
- [ ] Unequipping weapon deactivates passive abilities
- [ ] Crescendance bonds only trigger when correct weapon is equipped AND major is resonated

### Saving & Loading
- [ ] All crescendance stacks save correctly
- [ ] All weapon states save correctly
- [ ] Duet cooldowns persist through save/load
- [ ] Active effects resume correctly after loading

### Performance
- [ ] No lag during rapid clicking with multiple effects active
- [ ] Crescendance calculations don't cause framerate drops
- [ ] Large stack numbers (100+) display and calculate correctly
