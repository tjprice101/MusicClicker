using Avalonia.Controls;

namespace MusicClicker
{
    public static class UniteTheSymphonyManager
    {
        // TryCraft for Moonlight Sonata Minor
        public static bool TryCraftMoonlightMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if resources are available
            if (gameState.MoonlightMinorKeys >= 1 &&
                gameState.MoonlightMinorScales >= 1 &&
                gameState.MoonlightMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 5 &&
                gameState.MelodiousOwned >= 10)
            {
                // Deduct the resources
                gameState.MoonlightMinorKeys--;
                gameState.MoonlightMinorScales--;
                gameState.MoonlightMinorProgressions--;
                gameState.EssenceBeethoven -= 5;
                gameState.MelodiousOwned -= 10;

                owned++;
                gameState.NotesPerSecond += 3000;

                canCraft = true;
            }

            // Update UI if crafting is successful
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // TryCraft for Eroica Minor
        public static bool TryCraftEroicaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if resources are available
            if (gameState.EroicaMinorKeys >= 1 &&
                gameState.EroicaMinorScales >= 1 &&
                gameState.EroicaMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 10 &&
                gameState.MelodiousOwned >= 15)
            {
                // Deduct the resources
                gameState.EroicaMinorKeys--;
                gameState.EroicaMinorScales--;
                gameState.EroicaMinorProgressions--;
                gameState.EssenceBeethoven -= 10;
                gameState.MelodiousOwned -= 15;

                owned++;
                gameState.NotesPerSecond += 8000;

                canCraft = true;
            }

            // Update UI if crafting is successful
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // TryCraft for Swan Lake Minor
        public static bool TryCraftSwanLakeMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if resources are available
            if (gameState.SwanLakeMinorKeys >= 1 &&
                gameState.SwanLakeMinorScales >= 1 &&
                gameState.SwanLakeMinorProgressions >= 1 &&
                gameState.EssencePyotr >= 5 &&
                gameState.MelodiousOwned >= 20)
            {
                // Deduct the resources
                gameState.SwanLakeMinorKeys--;
                gameState.SwanLakeMinorScales--;
                gameState.SwanLakeMinorProgressions--;
                gameState.EssencePyotr -= 5;
                gameState.MelodiousOwned -= 20;

                owned++;
                gameState.NotesPerSecond += 15000;

                canCraft = true;
            }

            // Update UI if crafting is successful
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // TryCraft for La Campanella Minor
        public static bool TryCraftLaCampanellaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if resources are available
            if (gameState.LaCampanellaMinorKeys >= 1 &&
                gameState.LaCampanellaMinorScales >= 1 &&
                gameState.LaCampanellaMinorProgressions >= 1 &&
                gameState.EssenceLiszt >= 10 &&
                gameState.MelodiousOwned >= 25)
            {
                // Deduct the resources
                gameState.LaCampanellaMinorKeys--;
                gameState.LaCampanellaMinorScales--;
                gameState.LaCampanellaMinorProgressions--;
                gameState.EssenceLiszt -= 10;
                gameState.MelodiousOwned -= 25;

                owned++;
                gameState.NotesPerSecond += 35000;

                canCraft = true;
            }

            // Update UI if crafting is successful
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // TryCraft for Enigma Minor
        public static bool TryCraftEnigmaMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if resources are available
            if (gameState.EnigmaMinorKeys >= 1 &&
                gameState.EnigmaMinorScales >= 1 &&
                gameState.EnigmaMinorProgressions >= 1 &&
                gameState.EssenceElgar >= 10 &&
                gameState.MelodiousOwned >= 30)
            {
                // Deduct the resources
                gameState.EnigmaMinorKeys--;
                gameState.EnigmaMinorScales--;
                gameState.EnigmaMinorProgressions--;
                gameState.EssenceElgar -= 10;
                gameState.MelodiousOwned -= 30;

                owned++;
                gameState.NotesPerSecond += 75000;

                canCraft = true;
            }

            // Update UI if crafting is successful
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // TryCraft for Fate Minor
        public static bool TryCraftFateMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if resources are available
            if (gameState.FateMinorKeys >= 1 &&
                gameState.FateMinorScales >= 1 &&
                gameState.FateMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 10 &&
                gameState.MelodiousOwned >= 35)
            {
                // Deduct the resources
                gameState.FateMinorKeys--;
                gameState.FateMinorScales--;
                gameState.FateMinorProgressions--;
                gameState.EssenceBeethoven -= 10;
                gameState.MelodiousOwned -= 35;

                owned++;
                gameState.NotesPerSecond += 135000;

                canCraft = true;
            }

            // Update UI if crafting is successful
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // TryCraft for Ode to Joy Minor
        public static bool TryCraftOdeToJoyMinor(MainWindow window, ref int owned)
        {
            var gameState = window.GameState;
            bool canCraft = false;

            // Check if resources are available
            if (gameState.OdeToJoyMinorKeys >= 1 &&
                gameState.OdeToJoyMinorScales >= 1 &&
                gameState.OdeToJoyMinorProgressions >= 1 &&
                gameState.EssenceBeethoven >= 10 &&
                gameState.MelodiousOwned >= 40)
            {
                // Deduct the resources
                gameState.OdeToJoyMinorKeys--;
                gameState.OdeToJoyMinorScales--;
                gameState.OdeToJoyMinorProgressions--;
                gameState.EssenceBeethoven -= 10;
                gameState.MelodiousOwned -= 40;

                owned++;
                gameState.NotesPerSecond += 255000;

                canCraft = true;
            }

            // Update UI if crafting is successful
            if (canCraft)
            {
                UIUpdater.UpdateUnitySymphonyUI(window, gameState);
                UIUpdater.UpdateEssenceUI(window, gameState);
                UIUpdater.UpdateFragmentationUI(window, gameState);
            }

            return canCraft;
        }

        // TryCraft for Moonlight Sonata Major
public static bool TryCraftMoonlightSonataMajor(MainWindow window, ref int owned)
{
    var gameState = window.GameState;
    bool canCraft = false;

    // Check if resources are available
    if (gameState.MoonlightMajorKeys >= 1 &&
        gameState.MoonlightMajorScales >= 1 &&
        gameState.MoonlightMajorProgressions >= 1 &&
        gameState.EssenceBeethoven >= 10 &&
        gameState.HarmoniousOwned >= 10)
    {
        // Deduct the resources
        gameState.MoonlightMajorKeys--;
        gameState.MoonlightMajorScales--;
        gameState.MoonlightMajorProgressions--;
        gameState.EssenceBeethoven -= 10;
        gameState.HarmoniousOwned -= 10;

        owned++; // Increment the owned count for this major score
        gameState.MoonlightMajorAbility = true; // Enable Crescendance ability for Moonlight Sonata

        canCraft = true; // Successfully crafted
    }

    // Update UI if crafting is successful
    if (canCraft)
    {
        UIUpdater.UpdateUnitySymphonyUI(window, gameState);
        UIUpdater.UpdateEssenceUI(window, gameState);
        UIUpdater.UpdateFragmentationUI(window, gameState);
    }

    return canCraft;
}

// TryCraft for Eroica Major
public static bool TryCraftEroicaMajor(MainWindow window, ref int owned)
{
    var gameState = window.GameState;
    bool canCraft = false;

    // Check if resources are available
    if (gameState.EroicaMajorKeys >= 1 &&
        gameState.EroicaMajorScales >= 1 &&
        gameState.EroicaMajorProgressions >= 1 &&
        gameState.EssenceBeethoven >= 15 &&
        gameState.HarmoniousOwned >= 15)
    {
        // Deduct the resources
        gameState.EroicaMajorKeys--;
        gameState.EroicaMajorScales--;
        gameState.EroicaMajorProgressions--;
        gameState.EssenceBeethoven -= 15;
        gameState.HarmoniousOwned -= 15;

        owned++; // Increment the owned count for this major score
        gameState.EroicaMajorAbility = true; // Enable Crescendance ability for Eroica

        canCraft = true; // Successfully crafted
    }

    // Update UI if crafting is successful
    if (canCraft)
    {
        UIUpdater.UpdateUnitySymphonyUI(window, gameState);
        UIUpdater.UpdateEssenceUI(window, gameState);
        UIUpdater.UpdateFragmentationUI(window, gameState);
    }

    return canCraft;
}

// TryCraft for Swan Lake Major
public static bool TryCraftSwanLakeMajor(MainWindow window, ref int owned)
{
    var gameState = window.GameState;
    bool canCraft = false;

    // Check if resources are available
    if (gameState.SwanLakeMajorKeys >= 1 &&
        gameState.SwanLakeMajorScales >= 1 &&
        gameState.SwanLakeMajorProgressions >= 1 &&
        gameState.EssencePyotr >= 10 &&
        gameState.HarmoniousOwned >= 20)
    {
        // Deduct the resources
        gameState.SwanLakeMajorKeys--;
        gameState.SwanLakeMajorScales--;
        gameState.SwanLakeMajorProgressions--;
        gameState.EssencePyotr -= 10;
        gameState.HarmoniousOwned -= 20;

        owned++; // Increment the owned count for this major score
        gameState.SwanMajorAbility = true; // Enable Crescendance ability for Swan Lake

        canCraft = true; // Successfully crafted
    }

    // Update UI if crafting is successful
    if (canCraft)
    {
        UIUpdater.UpdateUnitySymphonyUI(window, gameState);
        UIUpdater.UpdateEssenceUI(window, gameState);
        UIUpdater.UpdateFragmentationUI(window, gameState);
    }

    return canCraft;
}

// TryCraft for La Campanella Major
public static bool TryCraftLaCampanellaMajor(MainWindow window, ref int owned)
{
    var gameState = window.GameState;
    bool canCraft = false;

    // Check if resources are available
    if (gameState.LaCampanellaMajorKeys >= 1 &&
        gameState.LaCampanellaMajorScales >= 1 &&
        gameState.LaCampanellaMajorProgressions >= 1 &&
        gameState.EssenceLiszt >= 15 &&
        gameState.HarmoniousOwned >= 25)
    {
        // Deduct the resources
        gameState.LaCampanellaMajorKeys--;
        gameState.LaCampanellaMajorScales--;
        gameState.LaCampanellaMajorProgressions--;
        gameState.EssenceLiszt -= 15;
        gameState.HarmoniousOwned -= 25;

        owned++; // Increment the owned count for this major score
        gameState.LaCampanellaMajorAbility = true; // Enable Crescendance ability for La Campanella

        canCraft = true; // Successfully crafted
    }

    // Update UI if crafting is successful
    if (canCraft)
    {
        UIUpdater.UpdateUnitySymphonyUI(window, gameState);
        UIUpdater.UpdateEssenceUI(window, gameState);
        UIUpdater.UpdateFragmentationUI(window, gameState);
    }

    return canCraft;
}

// TryCraft for Enigma Major
public static bool TryCraftEnigmaMajor(MainWindow window, ref int owned)
{
    var gameState = window.GameState;
    bool canCraft = false;

    // Check if resources are available
    if (gameState.EnigmaMajorKeys >= 1 &&
        gameState.EnigmaMajorScales >= 1 &&
        gameState.EnigmaMajorProgressions >= 1 &&
        gameState.EssenceElgar >= 15 &&
        gameState.HarmoniousOwned >= 30)
    {
        // Deduct the resources
        gameState.EnigmaMajorKeys--;
        gameState.EnigmaMajorScales--;
        gameState.EnigmaMajorProgressions--;
        gameState.EssenceElgar -= 15;
        gameState.HarmoniousOwned -= 30;

        owned++; // Increment the owned count for this major score
        gameState.EnigmaMajorAbility = true; // Enable Crescendance ability for Enigma

        canCraft = true; // Successfully crafted
    }

    // Update UI if crafting is successful
    if (canCraft)
    {
        UIUpdater.UpdateUnitySymphonyUI(window, gameState);
        UIUpdater.UpdateEssenceUI(window, gameState);
        UIUpdater.UpdateFragmentationUI(window, gameState);
    }

    return canCraft;
}

// TryCraft for Fate Major
public static bool TryCraftFateMajor(MainWindow window, ref int owned)
{
    var gameState = window.GameState;
    bool canCraft = false;

    // Check if resources are available
    if (gameState.FateMajorKeys >= 1 &&
        gameState.FateMajorScales >= 1 &&
        gameState.FateMajorProgressions >= 1 &&
        gameState.EssenceBeethoven >= 15 &&
        gameState.HarmoniousOwned >= 35)
    {
        // Deduct the resources
        gameState.FateMajorKeys--;
        gameState.FateMajorScales--;
        gameState.FateMajorProgressions--;
        gameState.EssenceBeethoven -= 15;
        gameState.HarmoniousOwned -= 35;

        owned++; // Increment the owned count for this major score
        gameState.FateMajorAbility = true; // Enable Crescendance ability for Fate

        canCraft = true; // Successfully crafted
    }

    // Update UI if crafting is successful
    if (canCraft)
    {
        UIUpdater.UpdateUnitySymphonyUI(window, gameState);
        UIUpdater.UpdateEssenceUI(window, gameState);
        UIUpdater.UpdateFragmentationUI(window, gameState);
    }

    return canCraft;
}

// TryCraft for Ode to Joy Major
public static bool TryCraftOdeToJoyMajor(MainWindow window, ref int owned)
{
    var gameState = window.GameState;
    bool canCraft = false;

    // Check if resources are available
    if (gameState.OdeToJoyMajorKeys >= 1 &&
        gameState.OdeToJoyMajorScales >= 1 &&
        gameState.OdeToJoyMajorProgressions >= 1 &&
        gameState.EssenceBeethoven >= 15 &&
        gameState.HarmoniousOwned >= 40)
    {
        // Deduct the resources
        gameState.OdeToJoyMajorKeys--;
        gameState.OdeToJoyMajorScales--;
        gameState.OdeToJoyMajorProgressions--;
        gameState.EssenceBeethoven -= 15;
        gameState.HarmoniousOwned -= 40;

        owned++; // Increment the owned count for this major score
        gameState.OdeToJoyMajorAbility = true; // Enable Crescendance ability for Ode to Joy

        canCraft = true; // Successfully crafted
    }

    // Update UI if crafting is successful
    if (canCraft)
    {
        UIUpdater.UpdateUnitySymphonyUI(window, gameState);
        UIUpdater.UpdateEssenceUI(window, gameState);
        UIUpdater.UpdateFragmentationUI(window, gameState);
    }

    return canCraft;
}
    }
}
