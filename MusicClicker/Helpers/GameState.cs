// GameState.cs
namespace MusicClicker
{
    public class GameState
    {
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
        public double _chordBaseCost = 10;
        public double ChordBaseCost { get => _chordBaseCost; set => _chordBaseCost = value; }
        
        public double _scaleBaseCost = 50;
        public double ScaleBaseCost { get => _scaleBaseCost; set => _scaleBaseCost = value; }
        
        public double _orchestraBaseCost = 100;
        public double OrchestraBaseCost { get => _orchestraBaseCost; set => _orchestraBaseCost = value; }
        
        public double _symphonyBaseCost = 350;
        public double SymphonyBaseCost { get => _symphonyBaseCost; set => _symphonyBaseCost = value; }
        
        public double _ariaBaseCost = 350;
        public double AriaBaseCost { get => _ariaBaseCost; set => _ariaBaseCost = value; }
        
        public double _requiemBaseCost = 750;
        public double RequiemBaseCost { get => _requiemBaseCost; set => _requiemBaseCost = value; }
        
        public double _opusBaseCost = 1500;
        public double OpusBaseCost { get => _opusBaseCost; set => _opusBaseCost = value; }
        
        public double _magnumOpusBaseCost = 3000;
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
        public int _moonlightMinorOwned = 0;
        public int MoonlightMinorOwned { get => _moonlightMinorOwned; set => _moonlightMinorOwned = value; }
        
        public int _moonlightMajorOwned = 0;
        public int MoonlightMajorOwned { get => _moonlightMajorOwned; set => _moonlightMajorOwned = value; }
        
        public int _eroicaMinorOwned = 0;
        public int EroicaMinorOwned { get => _eroicaMinorOwned; set => _eroicaMinorOwned = value; }
        
        public int _eroicaMajorOwned = 0;
        public int EroicaMajorOwned { get => _eroicaMajorOwned; set => _eroicaMajorOwned = value; }
        
        public int _swanMinorOwned = 0;
        public int SwanMinorOwned { get => _swanMinorOwned; set => _swanMinorOwned = value; }
        
        public int _swanMajorOwned = 0;
        public int SwanMajorOwned { get => _swanMajorOwned; set => _swanMajorOwned = value; }
        
        public int _laCampanellaMinorOwned = 0;
        public int LaCampanellaMinorOwned { get => _laCampanellaMinorOwned; set => _laCampanellaMinorOwned = value; }
        
        public int _laCampanellaMajorOwned = 0;
        public int LaCampanellaMajorOwned { get => _laCampanellaMajorOwned; set => _laCampanellaMajorOwned = value; }
        
        public int _enigmaMinorOwned = 0;
        public int EnigmaMinorOwned { get => _enigmaMinorOwned; set => _enigmaMinorOwned = value; }
        
        public int _enigmaMajorOwned = 0;
        public int EnigmaMajorOwned { get => _enigmaMajorOwned; set => _enigmaMajorOwned = value; }
        
        public int _fateMinorOwned = 0;
        public int FateMinorOwned { get => _fateMinorOwned; set => _fateMinorOwned = value; }
        
        public int _fateMajorOwned = 0;
        public int FateMajorOwned { get => _fateMajorOwned; set => _fateMajorOwned = value; }
        
        public int _odeToJoyMinorOwned = 0;
        public int OdeToJoyMinorOwned { get => _odeToJoyMinorOwned; set => _odeToJoyMinorOwned = value; }
        
        public int _odeToJoyMajorOwned = 0;
        public int OdeToJoyMajorOwned { get => _odeToJoyMajorOwned; set => _odeToJoyMajorOwned = value; }

        // Event Score Ownership (no minor/major distinction)
        public int _diesIraeOwned = 0;
        public int DiesIraeOwned { get => _diesIraeOwned; set => _diesIraeOwned = value; }
        
        public int _winterOwned = 0;
        public int WinterOwned { get => _winterOwned; set => _winterOwned = value; }

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
        
        public int _odeToJoyMajorKeys = 0;
        public int OdeToJoyMajorKeys { get => _odeToJoyMajorKeys; set => _odeToJoyMajorKeys = value; }
        
        public int _odeToJoyMajorScales = 0;
        public int OdeToJoyMajorScales { get => _odeToJoyMajorScales; set => _odeToJoyMajorScales = value; }
        
        public int _odeToJoyMajorProgressions = 0;
        public int OdeToJoyMajorProgressions { get => _odeToJoyMajorProgressions; set => _odeToJoyMajorProgressions = value; }

        // Armor of Forte - Weapon Ownership (0 = not owned, 1 = owned)
        public int _moonlightBladeIOwned = 0;
        public int MoonlightBladeIOwned { get => _moonlightBladeIOwned; set => _moonlightBladeIOwned = value; }
        
        public int _moonlightBladeIIOwned = 0;
        public int MoonlightBladeIIOwned { get => _moonlightBladeIIOwned; set => _moonlightBladeIIOwned = value; }
        
        public int _eroicaSwordIOwned = 0;
        public int EroicaSwordIOwned { get => _eroicaSwordIOwned; set => _eroicaSwordIOwned = value; }
        
        public int _eroicaSwordIIOwned = 0;
        public int EroicaSwordIIOwned { get => _eroicaSwordIIOwned; set => _eroicaSwordIIOwned = value; }
        
        public int _swanLanceIOwned = 0;
        public int SwanLanceIOwned { get => _swanLanceIOwned; set => _swanLanceIOwned = value; }
        
        public int _swanLanceIIOwned = 0;
        public int SwanLanceIIOwned { get => _swanLanceIIOwned; set => _swanLanceIIOwned = value; }
        
        public int _campanellaDaggerIOwned = 0;
        public int CampanellaDaggerIOwned { get => _campanellaDaggerIOwned; set => _campanellaDaggerIOwned = value; }
        
        public int _campanellaDaggerIIOwned = 0;
        public int CampanellaDaggerIIOwned { get => _campanellaDaggerIIOwned; set => _campanellaDaggerIIOwned = value; }
        
        public int _enigmaStaffIOwned = 0;
        public int EnigmaStaffIOwned { get => _enigmaStaffIOwned; set => _enigmaStaffIOwned = value; }
        
        public int _enigmaStaffIIOwned = 0;
        public int EnigmaStaffIIOwned { get => _enigmaStaffIIOwned; set => _enigmaStaffIIOwned = value; }
        
        public int _fateAxeIOwned = 0;
        public int FateAxeIOwned { get => _fateAxeIOwned; set => _fateAxeIOwned = value; }
        
        public int _fateAxeIIOwned = 0;
        public int FateAxeIIOwned { get => _fateAxeIIOwned; set => _fateAxeIIOwned = value; }
        
        public int _joyHammerIOwned = 0;
        public int JoyHammerIOwned { get => _joyHammerIOwned; set => _joyHammerIOwned = value; }
        
        public int _joyHammerIIOwned = 0;
        public int JoyHammerIIOwned { get => _joyHammerIIOwned; set => _joyHammerIIOwned = value; }
        
        public int _diesIraeScytheIOwned = 0;
        public int DiesIraeScytheIOwned { get => _diesIraeScytheIOwned; set => _diesIraeScytheIOwned = value; }
        
        public int _diesIraeScytheIIOwned = 0;
        public int DiesIraeScytheIIOwned { get => _diesIraeScytheIIOwned; set => _diesIraeScytheIIOwned = value; }
        
        public int _winterBowIOwned = 0;
        public int WinterBowIOwned { get => _winterBowIOwned; set => _winterBowIOwned = value; }
        
        public int _winterBowIIOwned = 0;
        public int WinterBowIIOwned { get => _winterBowIIOwned; set => _winterBowIIOwned = value; }
    }
}