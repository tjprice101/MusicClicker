// GameState holds the full, serializable snapshot of the player's progression.
// Design choices:
// - Many fields are exposed as public backing fields with thin properties. This allows
//   other parts of the code to pass fields by reference (ref) where necessary (e.g.
//   crafting and upgrade handlers) while still providing properties for serialization
//   systems that expect property access.
// - Keep this class lightweight: it is a plain data container without behavior.
using System;

namespace MusicClicker
{
    public class GameState
    {
        // Clair de Lune custom stacks and flags
        public int ShatteredMoonlightStacks { get; set; } = 0;
        public int ClockworkOfInfinityStacks { get; set; } = 0;
        public int TemporalFractureStacks { get; set; } = 0;
        public int InfiniteTemporalityCritsRemaining { get; set; } = 0;
        public int SurgeOfTimeFractalizationCritsRemaining { get; set; } = 0;
        public int ShatteredMoonlightConsumeCount { get; set; } = 0; // Tracks consumes for Celestial Horology bond
        public bool DuetChainOfTemporalityActive { get; set; } = false;
        public DateTime DuetChainOfTemporalityExpiry { get; set; } = DateTime.MinValue;
        public double DuetChainOfTemporalityNpcMultiplier { get; set; } = 1.0;
        public DateTime ClairDeLuneDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public int ClairDeLuneClickCounter { get; set; } = 0; // General click counter
        
        // Save format version - keep this field to support future migrations.
        public int SaveVersion = 2;

        // Base gameplay stats - public fields for ref support, properties for serialization
        public double _notes = 0;
        public double Notes { get => _notes; set => _notes = value; }
        
        public double _notesPerSecond = 0;
        public double NotesPerSecond { get => _notesPerSecond; set => _notesPerSecond = value; }
        
        public double _notesPerClick = 1;
        public double NotesPerClick { get => _notesPerClick; set => _notesPerClick = value; }

        // Upgrade state
        public int _chordOwned = 0;
        public int ChordOwned { get => _chordOwned; set => _chordOwned = value; }
        
        public int _scaleOwned = 0;
        public int ScaleOwned { get => _scaleOwned; set => _scaleOwned = value; }
        
        public int _orchestraOwned = 0;
        public int OrchestraOwned { get => _orchestraOwned; set => _orchestraOwned = value; }
        
        public int _symphonyOwned = 0;
        public int SymphonyOwned { get => _symphonyOwned; set => _symphonyOwned = value; }
        
        public int _ariaOwned = 0;
        public int AriaOwned { get => _ariaOwned; set => _ariaOwned = value; }
        
        public int _requiemOwned = 0;
        public int RequiemOwned { get => _requiemOwned; set => _requiemOwned = value; }
        
        public int _opusOwned = 0;
        public int OpusOwned { get => _opusOwned; set => _opusOwned = value; }
        
        public int _magnumOpusOwned = 0;
        public int MagnumOpusOwned { get => _magnumOpusOwned; set => _magnumOpusOwned = value; }

        // Base upgrade costs
        public double _chordBaseCost = 50;
        public double ChordBaseCost { get => _chordBaseCost; set => _chordBaseCost = value; }
        
        public double _scaleBaseCost = 500;
        public double ScaleBaseCost { get => _scaleBaseCost; set => _scaleBaseCost = value; }
        
        public double _orchestraBaseCost = 1000;
        public double OrchestraBaseCost { get => _orchestraBaseCost; set => _orchestraBaseCost = value; }
        
        public double _symphonyBaseCost = 10000;
        public double SymphonyBaseCost { get => _symphonyBaseCost; set => _symphonyBaseCost = value; }

        // Per-upgrade effect tuning (base effect and exponential growth factors)
        // NPS upgrades
        // Set to fixed values as requested: +5, +25, +125, +625 NPS respectively.
        public double ChordBaseNpsEffect { get; set; } = 5.0;
        public double ChordNpsGrowth { get; set; } = 1.0;

        public double ScaleBaseNpsEffect { get; set; } = 25.0;
        public double ScaleNpsGrowth { get; set; } = 1.0;

        public double OrchestraBaseNpsEffect { get; set; } = 125.0;
        public double OrchestraNpsGrowth { get; set; } = 1.0;

        public double SymphonyBaseNpsEffect { get; set; } = 625.0;
        public double SymphonyNpsGrowth { get; set; } = 1.0;

        // Click upgrades
        // NPC / click upgrade values as requested: +1, +5, +25, +125 clicks respectively.
        public double AriaBaseClickEffect { get; set; } = 1.0;
        public double AriaClickGrowth { get; set; } = 1.0;

        public double RequiemBaseClickEffect { get; set; } = 5.0;
        public double RequiemClickGrowth { get; set; } = 1.0;

        public double OpusBaseClickEffect { get; set; } = 25.0;
        public double OpusClickGrowth { get; set; } = 1.0;

        public double MagnumOpusBaseClickEffect { get; set; } = 125.0;
        public double MagnumOpusClickGrowth { get; set; } = 1.0;
        
        public double _ariaBaseCost = 500;
        public double AriaBaseCost { get => _ariaBaseCost; set => _ariaBaseCost = value; }
        
        public double _requiemBaseCost = 5000;
        public double RequiemBaseCost { get => _requiemBaseCost; set => _requiemBaseCost = value; }
        
        public double _opusBaseCost = 500000;
        public double OpusBaseCost { get => _opusBaseCost; set => _opusBaseCost = value; }
        
        public double _magnumOpusBaseCost = 5000000;
        public double MagnumOpusBaseCost { get => _magnumOpusBaseCost; set => _magnumOpusBaseCost = value; }

        // Fragmentation state
        public int _melodiousOwned = 0;
        public int MelodiousOwned { get => _melodiousOwned; set => _melodiousOwned = value; }
        
        public int _harmoniousOwned = 0;
        public int HarmoniousOwned { get => _harmoniousOwned; set => _harmoniousOwned = value; }

        // Save Scores state - Minor fragments
        public int _moonlightMinorKeys = 0;
        public int MoonlightMinorKeys { get => _moonlightMinorKeys; set => _moonlightMinorKeys = value; }
        
        public int _moonlightMinorScales = 0;
        public int MoonlightMinorScales { get => _moonlightMinorScales; set => _moonlightMinorScales = value; }
        
        public int _moonlightMinorProgressions = 0;
        public int MoonlightMinorProgressions { get => _moonlightMinorProgressions; set => _moonlightMinorProgressions = value; }
        
        public int _moonlightMajorSheets = 0;
        public int MoonlightMajorSheets { get => _moonlightMajorSheets; set => _moonlightMajorSheets = value; }
        
        public int _eroicaMinorKeys = 0;
        public int EroicaMinorKeys { get => _eroicaMinorKeys; set => _eroicaMinorKeys = value; }
        
        public int _eroicaMinorScales = 0;
        public int EroicaMinorScales { get => _eroicaMinorScales; set => _eroicaMinorScales = value; }
        
        public int _eroicaMinorProgressions = 0;
        public int EroicaMinorProgressions { get => _eroicaMinorProgressions; set => _eroicaMinorProgressions = value; }
        
        public int _eroicaMajorSheets = 0;
        public int EroicaMajorSheets { get => _eroicaMajorSheets; set => _eroicaMajorSheets = value; }
        
        public int _swanLakeMinorKeys = 0;
        public int SwanLakeMinorKeys { get => _swanLakeMinorKeys; set => _swanLakeMinorKeys = value; }
        
        public int _swanLakeMinorScales = 0;
        public int SwanLakeMinorScales { get => _swanLakeMinorScales; set => _swanLakeMinorScales = value; }
        
        public int _swanLakeMinorProgressions = 0;
        public int SwanLakeMinorProgressions { get => _swanLakeMinorProgressions; set => _swanLakeMinorProgressions = value; }
        
        public int _swanLakeMajorSheets = 0;
        public int SwanLakeMajorSheets { get => _swanLakeMajorSheets; set => _swanLakeMajorSheets = value; }
        
        public int _laCampanellaMinorKeys = 0;
        public int LaCampanellaMinorKeys { get => _laCampanellaMinorKeys; set => _laCampanellaMinorKeys = value; }
        
        public int _laCampanellaMinorScales = 0;
        public int LaCampanellaMinorScales { get => _laCampanellaMinorScales; set => _laCampanellaMinorScales = value; }
        
        public int _laCampanellaMinorProgressions = 0;
        public int LaCampanellaMinorProgressions { get => _laCampanellaMinorProgressions; set => _laCampanellaMinorProgressions = value; }
        
        public int _laCampanellaMajorSheets = 0;
        public int LaCampanellaMajorSheets { get => _laCampanellaMajorSheets; set => _laCampanellaMajorSheets = value; }
        
        public int _enigmaMinorKeys = 0;
        public int EnigmaMinorKeys { get => _enigmaMinorKeys; set => _enigmaMinorKeys = value; }
        
        public int _enigmaMinorScales = 0;
        public int EnigmaMinorScales { get => _enigmaMinorScales; set => _enigmaMinorScales = value; }
        
        public int _enigmaMinorProgressions = 0;
        public int EnigmaMinorProgressions { get => _enigmaMinorProgressions; set => _enigmaMinorProgressions = value; }
        
        public int _enigmaMajorSheets = 0;
        public int EnigmaMajorSheets { get => _enigmaMajorSheets; set => _enigmaMajorSheets = value; }
        
        public int _fateMinorKeys = 0;
        public int FateMinorKeys { get => _fateMinorKeys; set => _fateMinorKeys = value; }
        
        public int _fateMinorScales = 0;
        public int FateMinorScales { get => _fateMinorScales; set => _fateMinorScales = value; }
        
        public int _fateMinorProgressions = 0;
        public int FateMinorProgressions { get => _fateMinorProgressions; set => _fateMinorProgressions = value; }
        
        public int _fateMajorSheets = 0;
        public int FateMajorSheets { get => _fateMajorSheets; set => _fateMajorSheets = value; }
        
        public int _odeToJoyMinorKeys = 0;
        public int OdeToJoyMinorKeys { get => _odeToJoyMinorKeys; set => _odeToJoyMinorKeys = value; }
        
        public int _odeToJoyMinorScales = 0;
        public int OdeToJoyMinorScales { get => _odeToJoyMinorScales; set => _odeToJoyMinorScales = value; }
        
        public int _odeToJoyMinorProgressions = 0;
        public int OdeToJoyMinorProgressions { get => _odeToJoyMinorProgressions; set => _odeToJoyMinorProgressions = value; }
        
        public int _odeToJoyMajorSheets = 0;
        public int OdeToJoyMajorSheets { get => _odeToJoyMajorSheets; set => _odeToJoyMajorSheets = value; }

        // Major Ability Counters
        public int _fateCounter = 0;
        public int FateCounter { get => _fateCounter; set => _fateCounter = value; }

        // Craft Ownership (Unite the Symphony) - Regular Scores
        public long _moonlightMinorOwned = 0;
        public long MoonlightMinorOwned { get => _moonlightMinorOwned; set => _moonlightMinorOwned = value; }
        
        public long _moonlightMajorOwned = 0;
        public long MoonlightMajorOwned { get => _moonlightMajorOwned; set => _moonlightMajorOwned = value; }
        
        public long _eroicaMinorOwned = 0;
        public long EroicaMinorOwned { get => _eroicaMinorOwned; set => _eroicaMinorOwned = value; }
        
        public long _eroicaMajorOwned = 0;
        public long EroicaMajorOwned { get => _eroicaMajorOwned; set => _eroicaMajorOwned = value; }
        
        public long _swanMinorOwned = 0;
        public long SwanMinorOwned { get => _swanMinorOwned; set => _swanMinorOwned = value; }
        
        public long _swanMajorOwned = 0;
        public long SwanMajorOwned { get => _swanMajorOwned; set => _swanMajorOwned = value; }
        
        public long _laCampanellaMinorOwned = 0;
        public long LaCampanellaMinorOwned { get => _laCampanellaMinorOwned; set => _laCampanellaMinorOwned = value; }
        
        public long _laCampanellaMajorOwned = 0;
        public long LaCampanellaMajorOwned { get => _laCampanellaMajorOwned; set => _laCampanellaMajorOwned = value; }
        
        public long _enigmaMinorOwned = 0;
        public long EnigmaMinorOwned { get => _enigmaMinorOwned; set => _enigmaMinorOwned = value; }
        
        public long _enigmaMajorOwned = 0;
        public long EnigmaMajorOwned { get => _enigmaMajorOwned; set => _enigmaMajorOwned = value; }
        
        public long _fateMinorOwned = 0;
        public long FateMinorOwned { get => _fateMinorOwned; set => _fateMinorOwned = value; }
        
        public long _fateMajorOwned = 0;
        public long FateMajorOwned { get => _fateMajorOwned; set => _fateMajorOwned = value; }
        
        public long _odeToJoyMinorOwned = 0;
        public long OdeToJoyMinorOwned { get => _odeToJoyMinorOwned; set => _odeToJoyMinorOwned = value; }
        
        public long _odeToJoyMajorOwned = 0;
        public long OdeToJoyMajorOwned { get => _odeToJoyMajorOwned; set => _odeToJoyMajorOwned = value; }

        // Event Score Ownership (no minor/major distinction)
        public int _diesIraeOwned = 0;
        public int DiesIraeOwned { get => _diesIraeOwned; set => _diesIraeOwned = value; }
        
        public int _winterOwned = 0;
        public int WinterOwned { get => _winterOwned; set => _winterOwned = value; }

        // Boss Fight Score Ownership (Cacophonic Dreams)
        public int _mercuryMajorOwned = 0;
        public int MercuryMajorOwned { get => _mercuryMajorOwned; set => _mercuryMajorOwned = value; }
        
        public int _clairDeLuneMajorOwned = 0;
        public int ClairDeLuneMajorOwned { get => _clairDeLuneMajorOwned; set => _clairDeLuneMajorOwned = value; }
        
        public int _marsMajorOwned = 0;
        public int MarsMajorOwned { get => _marsMajorOwned; set => _marsMajorOwned = value; }

        // ==================== CLAIR DE LUNE CRESCENDANCE: CLOCKWORK SYMPHONY ====================
        // New time-based system - no hour/minute hands, uses real-world time
        // Variables already defined at top of class
        
        // Clair De Lune major ability flag
        public bool ClairDeLuneMajorAbility { get; set; } = false;

        // ==================== MERCURY CRESCENDANCE: THE WINGED MESSENGER ====================
        // Mercury custom stacks
        public int SwiftResonanceStacks { get; set; } = 0;
        public int HeraldsMessageStacks { get; set; } = 0;
        public int PlanetaryAlignmentStacks { get; set; } = 0;
        
        // Mercury ability cooldowns
        public DateTime MercuryHeraldDescentCooldownExpiry { get; set; } = DateTime.MinValue;
        public DateTime MercuryDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        
        // Mercury Duet: Swift Delivery state
        public bool MercuryDuetActive { get; set; } = false;
        public DateTime MercuryDuetExpiry { get; set; } = DateTime.MinValue;
        public int MercuryDuetStartingStacks { get; set; } = 0; // Stacks at start of duet for end bonus calc
        
        // Mercury click counter
        public int MercuryCrescendanceClickCounter { get; set; } = 0;
        
        // Mercury critical hit counters
        public bool MercuryWeapon1NextClickBonus { get; set; } = false; // For every-3rd-click passive
        public int MercuryWeapon1ClickCounter { get; set; } = 0;
        public int MercuryWeapon2ClickCounter { get; set; } = 0;
        public int MessengerOfPlanetaryResonanceCritsRemaining { get; set; } = 0;
        public int ThousandWingedSymphonyCritsRemaining { get; set; } = 0;
        public int MessengerOfFateMelodyCritsRemaining { get; set; } = 0;
        
        // Mercury major ability flag
        public bool MercuryMajorAbility { get; set; } = false;

        // Event Score Major Sheets
        public int _diesIraeMajorSheets = 0;
        public int DiesIraeMajorSheets { get => _diesIraeMajorSheets; set => _diesIraeMajorSheets = value; }
        
        public int _winterMajorSheets = 0;
        public int WinterMajorSheets { get => _winterMajorSheets; set => _winterMajorSheets = value; }

        // Ability unlock flags - Regular Scores
        public bool _moonlightMajorAbility = false;
        public bool MoonlightMajorAbility { get => _moonlightMajorAbility; set => _moonlightMajorAbility = value; }
        
        public bool _eroicaMajorAbility = false;
        public bool EroicaMajorAbility { get => _eroicaMajorAbility; set => _eroicaMajorAbility = value; }
        
        public bool _swanMajorAbility = false;
        public bool SwanMajorAbility { get => _swanMajorAbility; set => _swanMajorAbility = value; }
        
        public bool _laCampanellaMajorAbility = false;
        public bool LaCampanellaMajorAbility { get => _laCampanellaMajorAbility; set => _laCampanellaMajorAbility = value; }
        
        public bool _enigmaMajorAbility = false;
        public bool EnigmaMajorAbility { get => _enigmaMajorAbility; set => _enigmaMajorAbility = value; }
        
        public bool _fateMajorAbility = false;
        public bool FateMajorAbility { get => _fateMajorAbility; set => _fateMajorAbility = value; }
        
        public bool _odeToJoyMajorAbility = false;
        public bool OdeToJoyMajorAbility { get => _odeToJoyMajorAbility; set => _odeToJoyMajorAbility = value; }

        // Event Score Ability Flags
        public bool _diesIraeAbility = false;
        public bool DiesIraeAbility { get => _diesIraeAbility; set => _diesIraeAbility = value; }
        
        public bool _winterAbility = false;
        public bool WinterAbility { get => _winterAbility; set => _winterAbility = value; }

        // Major fragments for Heart of Harmony
        public int _moonlightMajorKeys = 0;
        public int MoonlightMajorKeys { get => _moonlightMajorKeys; set => _moonlightMajorKeys = value; }
        
        public int _moonlightMajorScales = 0;
        public int MoonlightMajorScales { get => _moonlightMajorScales; set => _moonlightMajorScales = value; }
        
        public int _moonlightMajorProgressions = 0;
        public int MoonlightMajorProgressions { get => _moonlightMajorProgressions; set => _moonlightMajorProgressions = value; }
        
        public int _eroicaMajorKeys = 0;
        public int EroicaMajorKeys { get => _eroicaMajorKeys; set => _eroicaMajorKeys = value; }
        
        public int _eroicaMajorScales = 0;
        public int EroicaMajorScales { get => _eroicaMajorScales; set => _eroicaMajorScales = value; }
        
        public int _eroicaMajorProgressions = 0;
        public int EroicaMajorProgressions { get => _eroicaMajorProgressions; set => _eroicaMajorProgressions = value; }
        
        public int _swanLakeMajorKeys = 0;
        public int SwanLakeMajorKeys { get => _swanLakeMajorKeys; set => _swanLakeMajorKeys = value; }
        
        public int _swanLakeMajorScales = 0;
        public int SwanLakeMajorScales { get => _swanLakeMajorScales; set => _swanLakeMajorScales = value; }
        
        public int _swanLakeMajorProgressions = 0;
        public int SwanLakeMajorProgressions { get => _swanLakeMajorProgressions; set => _swanLakeMajorProgressions = value; }
        
        public int _laCampanellaMajorKeys = 0;
        public int LaCampanellaMajorKeys { get => _laCampanellaMajorKeys; set => _laCampanellaMajorKeys = value; }
        
        public int _laCampanellaMajorScales = 0;
        public int LaCampanellaMajorScales { get => _laCampanellaMajorScales; set => _laCampanellaMajorScales = value; }
        
        public int _laCampanellaMajorProgressions = 0;
        public int LaCampanellaMajorProgressions { get => _laCampanellaMajorProgressions; set => _laCampanellaMajorProgressions = value; }
        
        public int _enigmaMajorKeys = 0;
        public int EnigmaMajorKeys { get => _enigmaMajorKeys; set => _enigmaMajorKeys = value; }
        
        public int _enigmaMajorScales = 0;
        public int EnigmaMajorScales { get => _enigmaMajorScales; set => _enigmaMajorScales = value; }
        
        public int _enigmaMajorProgressions = 0;
        public int EnigmaMajorProgressions { get => _enigmaMajorProgressions; set => _enigmaMajorProgressions = value; }

        public int _fateMajorKeys = 0;
        public int FateMajorKeys { get => _fateMajorKeys; set => _fateMajorKeys = value; }
        
        public int _fateMajorScales = 0;
        public int FateMajorScales { get => _fateMajorScales; set => _fateMajorScales = value; }
        
        public int _fateMajorProgressions = 0;
        public int FateMajorProgressions { get => _fateMajorProgressions; set => _fateMajorProgressions = value; }

        // Ode to Joy major fragments (may be referenced by other systems)
        public int _odeToJoyMajorKeys = 0; 
        public int OdeToJoyMajorKeys { get => _odeToJoyMajorKeys; set => _odeToJoyMajorKeys = value; }

        public int _odeToJoyMajorScales = 0; 
        public int OdeToJoyMajorScales { get => _odeToJoyMajorScales; set => _odeToJoyMajorScales = value; }

        public int _odeToJoyMajorProgressions = 0;
        public int OdeToJoyMajorProgressions { get => _odeToJoyMajorProgressions; set => _odeToJoyMajorProgressions = value; }
        
        // Currency gained by dissolving major scores
        public int _entropicMelodies = 0;
        public int EntropicMelodies { get => _entropicMelodies; set => _entropicMelodies = value; }
            // Tracks total Entropic Melodies spent (for refund logic)
            public int EntropicMelodiesSpent { get; set; } = 0;
        // Armory of Forte - Weapon Ownership booleans (true = owned)
        // Naming uses the weapon identifier (no spaces/special chars) matching displayed names.
        public bool _eulogyOfTheMoon = false;
        public bool EulogyOfTheMoon { get => _eulogyOfTheMoon; set => _eulogyOfTheMoon = value; }

        public bool _incisorOfMoonlight = false;
        public bool IncisorOfMoonlight { get => _incisorOfMoonlight; set => _incisorOfMoonlight = value; }

        public bool _sakurasBlossom = false;
        public bool SakurasBlossom { get => _sakurasBlossom; set => _sakurasBlossom = value; }

        public bool _funeralPrayer = false;
        public bool FuneralPrayer { get => _funeralPrayer; set => _funeralPrayer = value; }

        public bool _starScatteredWings = false;
        public bool StarScatteredWings { get => _starScatteredWings; set => _starScatteredWings = value; }

        public bool _thousandWingedSwan = false;
        public bool ThousandWingedSwan { get => _thousandWingedSwan; set => _thousandWingedSwan = value; }

        public bool _symphonyOfBells = false;
        public bool SymphonyOfBells { get => _symphonyOfBells; set => _symphonyOfBells = value; }

        // Entropic Armory Weapon ownership flags
        public bool _phasesOfInfinity = false;
        public bool PhasesOfInfinity { get => _phasesOfInfinity; set => _phasesOfInfinity = value; }

        public bool _moonlitRevelation = false;
        public bool MoonlitRevelation { get => _moonlitRevelation; set => _moonlitRevelation = value; }

        public bool _rebornInSakurasEmbrace = false;
        public bool RebornInSakurasEmbrace { get => _rebornInSakurasEmbrace; set => _rebornInSakurasEmbrace = value; }

        public bool _aThousandPetals = false;
        public bool AThousandPetals { get => _aThousandPetals; set => _aThousandPetals = value; }

        public bool _swansFinality = false;
        public bool SwansFinality { get => _swansFinality; set => _swansFinality = value; }

        public bool _starPiercingSwanSong = false;
        public bool StarPiercingSwanSong { get => _starPiercingSwanSong; set => _starPiercingSwanSong = value; }

        public bool _heavensChime = false;
        public bool HeavensChime { get => _heavensChime; set => _heavensChime = value; }

        public bool _bellOfChromaticFate = false;
        public bool BellOfChromaticFate { get => _bellOfChromaticFate; set => _bellOfChromaticFate = value; }

        public bool _everBurningMystery = false;
        public bool EverBurningMystery { get => _everBurningMystery; set => _everBurningMystery = value; }

        public bool _prismaticEnigmas = false;
        public bool PrismaticEnigmas { get => _prismaticEnigmas; set => _prismaticEnigmas = value; }

        public bool _finalityOfTheCosmos = false;
        public bool FinalityOfTheCosmos { get => _finalityOfTheCosmos; set => _finalityOfTheCosmos = value; }

        public bool _destinysConductor = false;
        public bool DestinysConductor { get => _destinysConductor; set => _destinysConductor = value; }

        public bool _catharticDivination = false;
        public bool CatharticDivination { get => _catharticDivination; set => _catharticDivination = value; }

        public bool _dreamsOfHarmony = false;
        public bool DreamsOfHarmony { get => _dreamsOfHarmony; set => _dreamsOfHarmony = value; }

        // Weapon ability flags (true if weapon's resonance ability is currently active)
        public bool _eulogyOfTheMoonAbility = false;
        public bool EulogyOfTheMoonAbility { get => _eulogyOfTheMoonAbility; set => _eulogyOfTheMoonAbility = value; }

        public bool _incisorOfMoonlightAbility = false;
        public bool IncisorOfMoonlightAbility { get => _incisorOfMoonlightAbility; set => _incisorOfMoonlightAbility = value; }

        public bool _sakurasBlossomAbility = false;
        public bool SakurasBlossomAbility { get => _sakurasBlossomAbility; set => _sakurasBlossomAbility = value; }

        public bool _funeralPrayerAbility = false;
        public bool FuneralPrayerAbility { get => _funeralPrayerAbility; set => _funeralPrayerAbility = value; }

        public bool _starScatteredWingsAbility = false;
        public bool StarScatteredWingsAbility { get => _starScatteredWingsAbility; set => _starScatteredWingsAbility = value; }

        public bool _thousandWingedSwanAbility = false;
        public bool ThousandWingedSwanAbility { get => _thousandWingedSwanAbility; set => _thousandWingedSwanAbility = value; }

        public bool _symphonyOfBellsAbility = false;
        public bool SymphonyOfBellsAbility { get => _symphonyOfBellsAbility; set => _symphonyOfBellsAbility = value; }

        public bool _razerOfBellsChimesAbility = false;
        public bool RazerOfBellsChimesAbility { get => _razerOfBellsChimesAbility; set => _razerOfBellsChimesAbility = value; }

        public bool _creatorOfMysteryAbility = false;
        public bool CreatorOfMysteryAbility { get => _creatorOfMysteryAbility; set => _creatorOfMysteryAbility = value; }

        public bool _truthseekerAbility = false;
        public bool TruthseekerAbility { get => _truthseekerAbility; set => _truthseekerAbility = value; }

        public bool _astralChainripperAbility = false;
        public bool AstralChainripperAbility { get => _astralChainripperAbility; set => _astralChainripperAbility = value; }

        public bool _cosmicWeaverAbility = false;
        public bool CosmicWeaverAbility { get => _cosmicWeaverAbility; set => _cosmicWeaverAbility = value; }

        public bool _joyfulCatharsisAbility = false;
        public bool JoyfulCatharsisAbility { get => _joyfulCatharsisAbility; set => _joyfulCatharsisAbility = value; }

        public bool _odeToCreationAbility = false;
        public bool OdeToCreationAbility { get => _odeToCreationAbility; set => _odeToCreationAbility = value; }

        public bool _sevenCirclesAbility = false;
        public bool SevenCirclesAbility { get => _sevenCirclesAbility; set => _sevenCirclesAbility = value; }

        public bool _hellsWrathAbility = false;
        public bool HellsWrathAbility { get => _hellsWrathAbility; set => _hellsWrathAbility = value; }

        public bool _cacophonicBlizzardAbility = false;
        public bool CacophonicBlizzardAbility { get => _cacophonicBlizzardAbility; set => _cacophonicBlizzardAbility = value; }

        public bool _theSnowsDesireAbility = false;
        public bool TheSnowsDesireAbility { get => _theSnowsDesireAbility; set => _theSnowsDesireAbility = value; }

        // Duet click counters
        public int FateDuetClickCounter { get; set; } = 0;

        // Individual weapon click counters
        public int OdeToCreationClickCounter { get; set; } = 0;

        // Fate weapon temporary boosts
        public bool AstralChainripperNpsBoostActive { get; set; } = false;
        public DateTime AstralChainripperNpsBoostExpiry { get; set; } = DateTime.MinValue;

        // Razer of Bell's Chimes temporary NPS boost
        public bool RazerNpsBoostActive { get; set; } = false;
        public DateTime RazerNpsBoostExpiry { get; set; } = DateTime.MinValue;

        // Ode to Creation next click bonus
        public bool OdeToCreationNextClickBonus { get; set; } = false;

        // ==================== WINTER CRESCENDANCE: REQUIEM OF THE FROZEN CHOIR ====================
        
        // Core stack system
        public int FrigidMelodyStacks { get; set; } = 0;          // Primary resource (from Freezing Harmony)
        public int EternalFrostStacks { get; set; } = 0;          // Offensive branch (enhanced clicks + crits)
        public int RegalSnowlightStacks { get; set; } = 0;        // Utility branch (fragments + entropic)
        
        // Freezing Harmony cooldown
        public DateTime FreezingHarmonyCooldownExpiry { get; set; } = DateTime.MinValue;
        
        // NPS Freeze mechanic (shared by Freezing Harmony, Eternal Frost, Cacophonic Blizzard passive, and Duet)
        public bool NpsFrozen { get; set; } = false;
        public double FrozenNpsValue { get; set; } = 0;
        public DateTime NpsFreezeExpiry { get; set; } = DateTime.MinValue;
        
        // Eternal Frost: Enhanced clicks ("Blizzard's Command of Eternal Ice" crits)
        public int BlizzardCommandClicksRemaining { get; set; } = 0;
        
        // Weapon: Cacophonic Blizzard
        public int CacophonicBlizzardClickCounter { get; set; } = 0;       // Tracks toward 20th click
        public int CacophonicBlizzardBonusClicksRemaining { get; set; } = 0; // Next 20 clicks gain +50% notes
        public int SnowsOblivionStacks { get; set; } = 0;                  // Crescendance Bond stack
        
        // Weapon: The Snow's Desire
        public double AcceleratingFlurryBonus { get; set; } = 0;           // Current +% NPC (up to 50%)
        public DateTime AcceleratingFlurryLastClickTime { get; set; } = DateTime.MinValue; // For 5s decay
        
        // Winter Duet: Symphony of Absolute Zero
        public bool WinterDuetActive { get; set; } = false;
        public DateTime WinterDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime WinterDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public double WinterDuetExtensionTime { get; set; } = 0; // Tracks how much time has been added (max 10s)

        // Dies Irae weapon mechanics - Descending Judgment
        public bool DiesIraeDuetActive { get; set; } = false;
        public DateTime DiesIraeDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime DiesIraeDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public int SevenSealsCounter { get; set; } = 0; // Counts clicks toward next seal trigger (resets at 7)

        // Moonlight Duet: Lunar Phases
        public bool MoonlightDuetActive { get; set; } = false;
        public DateTime MoonlightDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime MoonlightDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public DateTime MoonlightPhaseChangeTime { get; set; } = DateTime.MinValue; // When current phase started
        public int MoonlightCurrentPhase { get; set; } = 0; // 0=New Moon, 1=Crescent, 2=Full Moon, 3=Waning

        // Eroica Duet: Victory March
        public bool EroicaDuetActive { get; set; } = false;
        public DateTime EroicaDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public int VictoryMarchClicks { get; set; } = 0; // Progress toward 100 clicks
        public bool VictoryMarch25Claimed { get; set; } = false;
        public bool VictoryMarch50Claimed { get; set; } = false;
        public bool VictoryMarch75Claimed { get; set; } = false;
        public bool VictoryMarch100Claimed { get; set; } = false;

        // ==================== EROICA CRESCENDANCE ====================
        // Eroica Major Crescendance: Heroic Resolve → Symphonic Catharsis
        public int HeroicResolveStacks { get; set; } = 0;
        public int SymphonicCatharsisStacks { get; set; } = 0;
        
        // Symphonic Catharsis buff: 10s double NPC + 10% crit
        public bool SymphonicCatharsisActive { get; set; } = false;
        public DateTime SymphonicCatharsisExpiry { get; set; } = DateTime.MinValue;
        
        // Crescendance Bond - Sakura's Blossom: Crimson Requiem
        public int CrimsonRequiemClicksRemaining { get; set; } = 0;
        
        // Crescendance Bond - Funeral Prayer: Testament of Harmony
        public int TestamentOfHarmonyStacks { get; set; } = 0;
        
        // Funeral Prayer Forte Resonance: Prayer stacks and empowered clicks
        public int FuneralPrayerClickCounter { get; set; } = 0;
        public int FuneralPrayerStacks { get; set; } = 0;
        public int FuneralPrayerEmpoweredClicksRemaining { get; set; } = 0;
        
        // Eroica Major: Triumph of Heroes
        public bool TriumphOfHeroesAutoClickActive { get; set; } = false;
        public DateTime TriumphOfHeroesAutoClickExpiry { get; set; } = DateTime.MinValue;
        public DateTime TriumphOfHeroesCooldownExpiry { get; set; } = DateTime.MinValue;

        // Swan Lake Duet: Feather Cascade
        public bool SwanLakeDuetActive { get; set; } = false;
        public DateTime SwanLakeDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime SwanLakeDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public int SwanLakeDuetClickCounter { get; set; } = 0; // Tracks clicks during duet for feather grants
        
        // Swan Lake Crescendance: Feather System
        public int ReveredFeathers { get; set; } = 0; // Common feathers (5 stacks → +20% notes)
        public int ChromaticFeathers { get; set; } = 0; // Rare feathers (10 stacks → 2 of every minor)
        public int PolyphonicFeathers { get; set; } = 0; // Epic feathers (1 stack → 83 entropic + 75% notes)
        public int SwanLakeClickCounter { get; set; } = 0; // Tracks clicks for feather drop intervals
        
        // Moonlight Sonata Crescendance: Eclipse of the Nocturne
        public int MoonbeamResonanceStacks { get; set; } = 0; // Stacks gained every 20th click at night (8 stacks → +100% notes + 1 Harmonizing Moonlight)
        public int HarmonizingMoonlightStacks { get; set; } = 0; // Consumed by Eulogy weapon for 3 Moonbeam stacks + components
        public int MoonlightCrescendanceClickCounter { get; set; } = 0; // Tracks clicks toward next Moonbeam Resonance (every 20th click at night)
        public int IncisorClickCounter { get; set; } = 0; // Tracks every 4th click for Incisor of Moonlight passive
        
        // La Campanella Crescendance: Grandiose Bell
        public int GrandioseBellClickCounter { get; set; } = 0; // Counts clicks toward crack thresholds (20/40/60)
        public int GrandioseBellStage { get; set; } = 0; // 0=intact, 1=Crescending(20), 2=Radiant(40), 3=Harmonizing(60)
        public int DeafeningChimeStacks { get; set; } = 0; // Stacks from Radiant mend (max 15), consumed on Harmonizing mend for notes = stacks × NPS
        public DateTime DeafeningChimeExpiry { get; set; } = DateTime.MinValue; // Expiry for Deafening Chime stacks
        public int CosmicWeaverEntropicCritClicks { get; set; } = 0; // Remaining Stellar Cascade crit clicks from Cosmic Weaver's Symphony consume (5 per Symphony) or Tier 3 Cosmic Modulation (+1 per stack gained)
        
        // Enigma Crescendance: Resonant Mystery
        public int ResonateMysteryStacks { get; set; } = 0; // Every 10th click (+15th when crescendance active)
        public int EnigmaClickCounter { get; set; } = 0; // Tracks clicks for 10th/25th intervals
        public int EnigmaPassiveClickCounter { get; set; } = 0; // Creator of Mystery: every 3rd click ±25% notes
        public int TruthseekerSingleConsumeCount { get; set; } = 0; // Truthseeker: Track single mystery consumptions for "1 minor per 2 consumed"
        
        // Fate Crescendance: Cosmic Modulation
        public int CosmicModulationStacks { get; set; } = 0; // Every 8th click grants +1 stack + 10% notes
        public int SymphonyOfTheStarsStacks { get; set; } = 0; // Secondary stacks (1 per 5 Cosmic Modulation)
        public int FateClickCounter { get; set; } = 0; // Tracks every-5th-click intervals
        public DateTime FateCraftNpsBoostExpiry { get; set; } = DateTime.MinValue; // 5x NPS boost from Fate minor craft
        
        // Ode to Joy Crescendance: Petals of Harmony and Melody
        public int PetalsOfHarmony { get; set; } = 0; // Gained on minor craft
        public int PetalsOfMelody { get; set; } = 0; // Gained on major craft
        public int OdeToLifeStacks { get; set; } = 0; // Combined petals + entropic
        public int EntropicCritClicksRemaining { get; set; } = 0; // From consuming Petal of Melody
        public DateTime EntropicCritExpiry { get; set; } = DateTime.MinValue; // Tracks 10s duration per petal
        public int JoyfulCatharsisClickCounter { get; set; } = 0; // Every 50th click for entropic melodies
        public bool OdeToCreationDoubleActive { get; set; } = false; // Doubled passive after Ode to Life consume
        public DateTime OdeToCreationDoubleExpiry { get; set; } = DateTime.MinValue; // 25s duration
        public DateTime JoyfulCatharsisNpsBoostExpiry { get; set; } = DateTime.MinValue; // 5s NPS boost on petal gain
        
        // Dies Irae Crescendance: Dissonant Hatred and Discordant Malice
        public int DissonantHatredStacks { get; set; } = 0; // Every click before 50
        public int DiscordantMaliceStacks { get; set; } = 0; // Every click after 50 Dissonant Hatred
        public int CacophonicOblivionStacks { get; set; } = 0; // Consume 5 Dissonant Hatred
        public int SymphonyOfHellClicks { get; set; } = 0; // Remaining clicks for 'Symphony of Hell's Retribution' crit
        public int WrathfulSealStacks { get; set; } = 0; // Gained from Symphony of Hell's Retribution crits (Seven Circles)
        public int SealBreakingMelodyClicks { get; set; } = 0; // Remaining clicks for Seal-breaking Melody
        public int HellsWrathClickCounter { get; set; } = 0; // Every 20th click for passive
        public int DiesIraeDuetNoteDoublingClicks { get; set; } = 0; // Remaining clicks that double notes (from 15+ Wrathful Seals)
        
        // Star-Scattered Wings (Swan I) passive tracking
        public int StarScatteredWingsClickCounter { get; set; } = 0; // Every 10th click grants fragments
        
        // Thousand Winged Swan (Swan II) - Polyphonic NPS boost
        public bool ThousandWingedSwanNpsBoostActive { get; set; } = false;
        public DateTime ThousandWingedSwanNpsBoostExpiry { get; set; } = DateTime.MinValue;
        public int ThousandWingedSwanFeathersConsumed { get; set; } = 0; // Stacks 1-10, resets on expiry

        // La Campanella Duet: Chime Chain
        public bool LaCampanellaDuetActive { get; set; } = false;
        public DateTime LaCampanellaDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime LaCampanellaDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public int ChimeChainLength { get; set; } = 0; // Current chain length
        public DateTime LastChimeClickTime { get; set; } = DateTime.MinValue; // Last click time for 1-second window

        // Enigma Duet: Mystery Clicks (RNG effects on click)
        public bool EnigmaDuetActive { get; set; } = false;
        public DateTime EnigmaDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime EnigmaDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public int EnigmaMysteryClickCount { get; set; } = 0; // Track clicks during Enigma activation

        // Fate Duet: Hourglass Reversal
        public bool FateDuetActive { get; set; } = false;
        public DateTime FateDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime FateDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public DateTime FateDuetFlipTime { get; set; } = DateTime.MinValue; // When hourglass flips (10s mark)
        public bool FateDuetHasFlipped { get; set; } = false; // Whether we're in replay phase
        public int FateDuetClickCount { get; set; } = 0; // Number of clicks performed during banking phase
        public System.Collections.Generic.List<(string actionType, object actionData, DateTime recordTime)> HourglassActionBank { get; set; } = new();

        // Ode to Joy Duet: Crescendo Conductor (Repeatable 16-note cycles)
        public bool OdeDuetActive { get; set; } = false;
        public DateTime OdeDuetExpiry { get; set; } = DateTime.MinValue;
        public DateTime OdeDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public int CrescendoNotesPlaced { get; set; } = 0; // Number of notes in current cycle (0-15, resets at 16)
        public int CrescendoCompletedSections { get; set; } = 0; // Number of completed 16-note sections
        public bool Crescendo4Claimed { get; set; } = false; // 4-note phrase reward claimed (per cycle)
        public bool Crescendo8Claimed { get; set; } = false; // 8-note phrase reward claimed (per cycle)
        public bool Crescendo12Claimed { get; set; } = false; // 12-note phrase reward claimed (per cycle)
        public bool Crescendo16Claimed { get; set; } = false; // 16-note symphony reward claimed (per cycle)
        public bool OdeDuetNpsBoostActive { get; set; } = false; // 5x NPS boost from completing 16 notes
        public DateTime OdeDuetNpsBoostExpiry { get; set; } = DateTime.MinValue;

        // Currently resonated weapons (up to two slots)
        public string CurrentResonatedWeapon1 { get; set; } = "None";
        public string CurrentResonatedWeapon2 { get; set; } = "None";

        public bool _razerOfBellsChimes = false;
        public bool RazerOfBellsChimes { get => _razerOfBellsChimes; set => _razerOfBellsChimes = value; }

        public bool _creatorOfMystery = false;
        public bool CreatorOfMystery { get => _creatorOfMystery; set => _creatorOfMystery = value; }

        public bool _truthseeker = false;
        public bool Truthseeker { get => _truthseeker; set => _truthseeker = value; }

        public bool _astralChainripper = false;
        public bool AstralChainripper { get => _astralChainripper; set => _astralChainripper = value; }

        public bool _cosmicWeaver = false;
        public bool CosmicWeaver { get => _cosmicWeaver; set => _cosmicWeaver = value; }

        public bool _joyfulCatharsis = false;
        public bool JoyfulCatharsis { get => _joyfulCatharsis; set => _joyfulCatharsis = value; }

        public bool _odeToCreation = false;
        public bool OdeToCreation { get => _odeToCreation; set => _odeToCreation = value; }

        // Renamed event weapon ownership fields to match display names.
        // Note: renaming these fields/properties will change serialized names in saves.
        public bool _sevenCircles = false;
        public bool SevenCircles { get => _sevenCircles; set => _sevenCircles = value; }

        public bool _hellsWrath = false;
        public bool HellsWrath { get => _hellsWrath; set => _hellsWrath = value; }

        public bool _cacophonicBlizzard = false;
        public bool CacophonicBlizzard { get => _cacophonicBlizzard; set => _cacophonicBlizzard = value; }

        public bool _theSnowsDesire = false;
        public bool TheSnowsDesire { get => _theSnowsDesire; set => _theSnowsDesire = value; }

        // Clair De Lune weapons (Boss Fight - Tonality)
        public bool _celestialHorology = false;
        public bool CelestialHorology { get => _celestialHorology; set => _celestialHorology = value; }

        public bool _metronomicDissonance = false;
        public bool MetronomicDissonance { get => _metronomicDissonance; set => _metronomicDissonance = value; }

        // Clair De Lune weapon ability flags
        public bool _celestialHorologyAbility = false;
        public bool CelestialHorologyAbility { get => _celestialHorologyAbility; set => _celestialHorologyAbility = value; }

        public bool _metronomicDissonanceAbility = false;
        public bool MetronomicDissonanceAbility { get => _metronomicDissonanceAbility; set => _metronomicDissonanceAbility = value; }

        // Mercury weapons (Boss Fight - Cacophonic Dreams)
        public bool _mercurialOverture = false;
        public bool MercurialOverture { get => _mercurialOverture; set => _mercurialOverture = value; }

        public bool _wingOfTheMessenger = false;
        public bool WingOfTheMessenger { get => _wingOfTheMessenger; set => _wingOfTheMessenger = value; }

        // Mercury weapon ability flags
        public bool _mercurialOvertureAbility = false;
        public bool MercurialOvertureAbility { get => _mercurialOvertureAbility; set => _mercurialOvertureAbility = value; }

        public bool _wingOfTheMessengerAbility = false;
        public bool WingOfTheMessengerAbility { get => _wingOfTheMessengerAbility; set => _wingOfTheMessengerAbility = value; }

        // ==================== MARS MAJOR CRESCENDANCE ====================
        // Mars, Bringer of War - Blade of Symphonic War
        public bool MarsMajorAbility { get; set; } = false; // True when Mars Major is resonated
        
        // Blade of Symphonic War charging system
        public double BladeOfSymphonicWarCharge { get; set; } = 0;  // Charges 0-300%, generates stacks above 100%
        public int ResoluteFractalStacks { get; set; } = 0;         // Generated above 100% charge (1 per 1%)
        public int ResoluteFractalConsumed { get; set; } = 0;       // Tracks total consumed (every 5th triggers Infinite War)
        public int InfiniteWarOfHarmonyCritsRemaining { get; set; } = 0; // 5 crits granted every 5th Resolute Fractal (4x effective)
        public int OblivionsDestructionStacks { get; set; } = 0;    // From Fractal of War bond
        
        // Mars Weapon 1: Fractal of War
        public bool _fractalOfWar = false;
        public bool FractalOfWar { get => _fractalOfWar; set => _fractalOfWar = value; }
        public bool _fractalOfWarAbility = false;
        public bool FractalOfWarAbility { get => _fractalOfWarAbility; set => _fractalOfWarAbility = value; }
        public int FractalOfWarClickCounter { get; set; } = 0;      // Tracks every 25th click
        
        // Mars Weapon 2: Consonance's Requiemic War
        public bool _consonanceRequiemicWar = false;
        public bool ConsonanceRequiemicWar { get => _consonanceRequiemicWar; set => _consonanceRequiemicWar = value; }
        public bool _consonanceRequiemicWarAbility = false;
        public bool ConsonanceRequiemicWarAbility { get => _consonanceRequiemicWarAbility; set => _consonanceRequiemicWarAbility = value; }
        public int ConsonanceRequiemBonusClicksRemaining { get; set; } = 0; // NPC^1.56 bonus clicks
        
        // Mars Duet: Annihilation Nocturne (time-based)
        public DateTime MarsDuetCooldownExpiry { get; set; } = DateTime.MinValue;
        public DateTime MarsDuetExpiry { get; set; } = DateTime.MinValue; // 12 second duration
        public bool MarsDuetActive => DateTime.Now <= MarsDuetExpiry;

        public string CurrentClickerImage { get; set; } = "avares://MusicClicker/Gameplay Components/Resources/Assets/Backgrounds & Generic/Music Game Assets [A961E2A]-min.png";
        public string CurrentBackgroundImage { get; set; } = "avares://MusicClicker/Gameplay Components/Resources/Assets/Backgrounds & Generic/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png";
        public string CurrentResonatedScore { get; set; } = "None";

        // Cached upgrade costs for performance optimization (avoids recalculating Math.Pow every frame)
        // These are invalidated when purchases are made
        public double CachedChordCost { get; set; } = 0;
        public double CachedScaleCost { get; set; } = 0;
        public double CachedOrchestraCost { get; set; } = 0;
        public double CachedSymphonyCost { get; set; } = 0;
        public double CachedAriaCost { get; set; } = 0;
        public double CachedRequiemCost { get; set; } = 0;
        public double CachedOpusCost { get; set; } = 0;
        public double CachedMagnumOpusCost { get; set; } = 0;
    }
}