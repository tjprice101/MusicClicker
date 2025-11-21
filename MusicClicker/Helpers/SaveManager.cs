using System;
using System.IO;
using System.Text.Json;
using MusicClicker;

namespace MusicClicker.Helpers
{
    public static class SaveManager
    {
        private static readonly string SaveFolder = Path.Combine(Environment.CurrentDirectory, "Save");
        private static readonly string SaveFile = Path.Combine(SaveFolder, "SavedGameState");

        public static bool Save(GameState state, out string error)
        {
            error = string.Empty;
            try
            {
                if (!Directory.Exists(SaveFolder))
                {
                    Directory.CreateDirectory(SaveFolder);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(state, options);

                File.WriteAllText(SaveFile, json);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool Exists()
        {
            return File.Exists(SaveFile);
        }

        public static string GetPath() => SaveFile;

        public static bool TryLoad(out GameState state, out string error)
        {
            state = new GameState();
            error = string.Empty;
            try
            {
                if (!File.Exists(SaveFile))
                {
                    return false;
                }

                string json = File.ReadAllText(SaveFile);
                var loaded = JsonSerializer.Deserialize<GameState>(json);
                if (loaded == null)
                {
                    error = "Failed to deserialize save file.";
                    return false;
                }

                state = loaded;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
