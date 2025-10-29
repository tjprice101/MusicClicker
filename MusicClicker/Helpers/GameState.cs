// GameState.cs
namespace MusicClicker
{
    public class GameState
    {
        // Base gameplay stats
        public double Notes = 0;
        public double NotesPerSecond = 0;
        public double NotesPerClick = 1;

        // Upgrade state
        public int ChordOwned = 0;
        public int ScaleOwned = 0;
        public int OrchestraOwned = 0;
        public int SymphonyOwned = 0;
        public int AriaOwned = 0;
        public int RequiemOwned = 0;
        public int OpusOwned = 0;
        public int MagnumOpusOwned = 0;

        // Base upgrade costs
        public double ChordBaseCost = 10;
        public double ScaleBaseCost = 50;
        public double OrchestraBaseCost = 100;
        public double SymphonyBaseCost = 350;
        public double AriaBaseCost = 350;
        public double RequiemBaseCost = 750;
        public double OpusBaseCost = 1500;
        public double MagnumOpusBaseCost = 3000;

        // Fragmentation state
        public int MelodiousOwned = 0;
        public int HarmoniousOwned = 0;

        // Save Scores state
        public int MoonlightMinorKeys = 0;
        public int MoonlightMinorScales = 0;
        public int MoonlightMinorProgressions = 0;
        public int MoonlightMajorSheets = 0;
        public int EroicaMinorKeys = 0;
        public int EroicaMinorScales = 0;
        public int EroicaMinorProgressions = 0;
        public int EroicaMajorSheets = 0;
        public int SwanLakeMinorKeys = 0;
        public int SwanLakeMinorScales = 0;
        public int SwanLakeMinorProgressions = 0;
        public int SwanLakeMajorSheets = 0;

        // Essence resources
        public int EssenceBeethoven = 0;
        public int EssencePyotr = 0;
        public int EssenceElgar = 0;
        public int EssenceLiszt = 0;

        // Craft Ownership (Unite the Symphony)
        public int MoonlightMinorOwned = 0;
        public int MoonlightMajorOwned = 0;
        public int EroicaMinorOwned = 0;
        public int EroicaMajorOwned = 0;
        public int SwanMinorOwned = 0;
        public int SwanMajorOwned = 0;
        public int LaCampanellaMinorOwned = 0;
        public int LaCampanellaMajorOwned = 0;
        public int EnigmaMinorOwned = 0;
        public int EnigmaMajorOwned = 0;
        public int FateMinorOwned = 0;
        public int FateMajorOwned = 0;
        public int OdeToJoyMinorOwned = 0;
        public int OdeToJoyMajorOwned = 0;

        // Ability unlock flags
        public bool MoonlightMajorAbility = false;
        public bool EroicaMajorAbility = false;
        public bool SwanMajorAbility = false;
        public bool LaCampanellaMajorAbility = false;
        public bool EnigmaMajorAbility = false;
        public bool FateMajorAbility = false;
        public bool OdeToJoyMajorAbility = false;

        // Major fragments for Heart of Harmony
        public int MoonlightMajorKeys = 0;
        public int MoonlightMajorScales = 0;
        public int MoonlightMajorProgressions = 0;
        public int EroicaMajorKeys = 0;
        public int EroicaMajorScales = 0;
        public int EroicaMajorProgressions = 0;
        public int SwanLakeMajorKeys = 0;
        public int SwanLakeMajorScales = 0;
        public int SwanLakeMajorProgressions = 0;

        // 🆕 New Save Scores entries
        public int LaCampanellaMinorKeys = 0;
        public int LaCampanellaMinorScales = 0;
        public int LaCampanellaMinorProgressions = 0;
        public int LaCampanellaMajorSheets = 0;
        public int EnigmaMinorKeys = 0;
        public int EnigmaMinorScales = 0;
        public int EnigmaMinorProgressions = 0;
        public int EnigmaMajorSheets = 0;
        public int FateMinorKeys = 0;
        public int FateMinorScales = 0;
        public int FateMinorProgressions = 0;
        public int FateMajorSheets = 0;
        public int OdeToJoyMinorKeys = 0;
        public int OdeToJoyMinorScales = 0;
        public int OdeToJoyMinorProgressions = 0;
        public int OdeToJoyMajorSheets = 0;
    }
}
