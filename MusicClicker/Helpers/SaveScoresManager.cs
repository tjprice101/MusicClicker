using System;          // Random, Func<>, Action<>
using MusicClicker;    // Needed to reference MainWindow

public class SaveScoresManager
{
    private Random _random = new Random();

    // Notes reference
    private Func<double> _getNotes;
    private Action<double> _setNotes;

    // Save Scores state
    public int MoonlightMinorKeys, MoonlightMinorScales, MoonlightMinorProgressions, MoonlightMajorSheets;
    public int EroicaMinorKeys, EroicaMinorScales, EroicaMinorProgressions, EroicaMajorSheets;
    public int SwanLakeMinorKeys, SwanLakeMinorScales, SwanLakeMinorProgressions, SwanLakeMajorSheets;
    public int LaCampanellaMinorKeys, LaCampanellaMinorScales, LaCampanellaMinorProgressions, LaCampanellaMajorSheets;
    public int EnigmaMinorKeys, EnigmaMinorScales, EnigmaMinorProgressions, EnigmaMajorSheets;
    public int FateMinorKeys, FateMinorScales, FateMinorProgressions, FateMajorSheets;
    public int OdeToJoyMinorKeys, OdeToJoyMinorScales, OdeToJoyMinorProgressions, OdeToJoyMajorSheets;

    // Major abilities
    public bool LaCampanellaMajorAbility, EnigmaMajorAbility, FateMajorAbility, OdeToJoyMajorAbility;

    // Constructor
    public SaveScoresManager(Func<double> getNotes, Action<double> setNotes)
    {
        _getNotes = getNotes;
        _setNotes = setNotes;
    }

    // Wiring buttons
    public void RegisterButtons(MainWindow window)
    {
        window.MoonlightSonataButton.Click += (s, e) => HandleSaveScoreClick(window, ref MoonlightMinorKeys, ref MoonlightMinorScales, ref MoonlightMinorProgressions, ref MoonlightMajorSheets, 1000);
        window.EroicaButton.Click += (s, e) => HandleSaveScoreClick(window, ref EroicaMinorKeys, ref EroicaMinorScales, ref EroicaMinorProgressions, ref EroicaMajorSheets, 3000);
        window.SwanLakeButton.Click += (s, e) => HandleSaveScoreClick(window, ref SwanLakeMinorKeys, ref SwanLakeMinorScales, ref SwanLakeMinorProgressions, ref SwanLakeMajorSheets, 8000);

        window.LaCampanellaButton.Click += (s, e) => HandleSaveScoreClick(window, ref LaCampanellaMinorKeys, ref LaCampanellaMinorScales, ref LaCampanellaMinorProgressions, ref LaCampanellaMajorSheets, 12000);
        window.EnigmaButton.Click += (s, e) => HandleSaveScoreClick(window, ref EnigmaMinorKeys, ref EnigmaMinorScales, ref EnigmaMinorProgressions, ref EnigmaMajorSheets, 25000);
        window.FateButton.Click += (s, e) => HandleSaveScoreClick(window, ref FateMinorKeys, ref FateMinorScales, ref FateMinorProgressions, ref FateMajorSheets, 35000);
        window.OdeToJoyButton.Click += (s, e) => HandleSaveScoreClick(window, ref OdeToJoyMinorKeys, ref OdeToJoyMinorScales, ref OdeToJoyMinorProgressions, ref OdeToJoyMajorSheets, 55000);
    }

    // Click logic
    private void HandleSaveScoreClick(MainWindow window, ref int minorKeys, ref int minorScales, ref int minorProgressions, ref int majorSheets, int cost)
    {
        if (_getNotes() >= cost)
{
    _setNotes(_getNotes() - cost);

    double roll = _random.NextDouble(); // single roll

if (roll <= 0.20)           // 20%
    minorKeys++;
else if (roll <= 0.30)      // 10%
    minorScales++;
else if (roll <= 0.35)      // 5%
    minorProgressions++;
else if (roll <= 0.36)      // 1%
    majorSheets++;
}
UpdateUI(window, _getNotes());
    }

    // Update the Save Scores UI
    public void UpdateUI(MainWindow window, double notes)
    {
        window.MoonlightMinorKeysText.Text = $"{MoonlightMinorKeys} Minor Keys of Moonlight Sonata Owned";
        window.MoonlightMinorScalesText.Text = $"{MoonlightMinorScales} Minor Scales of Moonlight Sonata Owned";
        window.MoonlightMinorProgressionsText.Text = $"{MoonlightMinorProgressions} Minor Progressions of Moonlight Sonata Owned";
        window.MoonlightMajorSheetsText.Text = $"{MoonlightMajorSheets} Major Sheets of Moonlight Sonata Owned";

        window.EroicaMinorKeysText.Text = $"{EroicaMinorKeys} Minor Keys of Eroica Owned";
        window.EroicaMinorScalesText.Text = $"{EroicaMinorScales} Minor Scales of Eroica Owned";
        window.EroicaMinorProgressionsText.Text = $"{EroicaMinorProgressions} Minor Progressions of Eroica Owned";
        window.EroicaMajorSheetsText.Text = $"{EroicaMajorSheets} Major Sheets of Eroica Owned";

        window.SwanLakeMinorKeysText.Text = $"{SwanLakeMinorKeys} Minor Keys of Swan Lake Owned";
        window.SwanLakeMinorScalesText.Text = $"{SwanLakeMinorScales} Minor Scales of Swan Lake Owned";
        window.SwanLakeMinorProgressionsText.Text = $"{SwanLakeMinorProgressions} Minor Progressions of Swan Lake Owned";
        window.SwanLakeMajorSheetsText.Text = $"{SwanLakeMajorSheets} Major Sheets of Swan Lake Owned";

        window.LaCampanellaMinorKeysText.Text = $"{LaCampanellaMinorKeys} Minor Keys of La Campanella Owned";
        window.LaCampanellaMinorScalesText.Text = $"{LaCampanellaMinorScales} Minor Scales of La Campanella Owned";
        window.LaCampanellaMinorProgressionsText.Text = $"{LaCampanellaMinorProgressions} Minor Progressions of La Campanella Owned";
        window.LaCampanellaMajorSheetsText.Text = $"{LaCampanellaMajorSheets} Major Sheets of La Campanella Owned";

        window.EnigmaMinorKeysText.Text = $"{EnigmaMinorKeys} Minor Keys of Enigma Owned";
        window.EnigmaMinorScalesText.Text = $"{EnigmaMinorScales} Minor Scales of Enigma Owned";
        window.EnigmaMinorProgressionsText.Text = $"{EnigmaMinorProgressions} Minor Progressions of Enigma Owned";
        window.EnigmaMajorSheetsText.Text = $"{EnigmaMajorSheets} Major Sheets of Enigma Owned";

        window.FateMinorKeysText.Text = $"{FateMinorKeys} Minor Keys of Fate Owned";
        window.FateMinorScalesText.Text = $"{FateMinorScales} Minor Scales of Fate Owned";
        window.FateMinorProgressionsText.Text = $"{FateMinorProgressions} Minor Progressions of Fate Owned";
        window.FateMajorSheetsText.Text = $"{FateMajorSheets} Major Sheets of Fate Owned";

        window.OdeToJoyMinorKeysText.Text = $"{OdeToJoyMinorKeys} Minor Keys of Ode to Joy Owned";
        window.OdeToJoyMinorScalesText.Text = $"{OdeToJoyMinorScales} Minor Scales of Ode to Joy Owned";
        window.OdeToJoyMinorProgressionsText.Text = $"{OdeToJoyMinorProgressions} Minor Progressions of Ode to Joy Owned";
        window.OdeToJoyMajorSheetsText.Text = $"{OdeToJoyMajorSheets} Major Sheets of Ode to Joy Owned";

        window.SaveScoresNotesText.Text = $"Notes: {Math.Round(notes, 1)}";
    }
}
