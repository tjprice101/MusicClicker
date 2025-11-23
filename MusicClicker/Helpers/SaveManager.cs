using System;
using System.IO;
using System.Text.Json;
using MusicClicker;

namespace MusicClicker.Helpers
{
    // Simple file-based save/load manager for GameState.
    // Performance improvement: reuse JsonSerializerOptions to avoid allocating a new
    // options object on every save/load call.
    public static class SaveManager
    {
        private static readonly string SaveFolder = Path.Combine(Environment.CurrentDirectory, "Save");
        private static readonly string SaveFile = Path.Combine(SaveFolder, "SavedGameState");

        // Reused serializer options to avoid per-call allocation.
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

        public static bool Save(GameState state, out string error)
        {
            error = string.Empty;
            try
            {
                if (!Directory.Exists(SaveFolder))
                {
                    Directory.CreateDirectory(SaveFolder);
                }

                // Serialize the full GameState to disk. Keeping this simple and synchronous
                // is appropriate for a small single-file save system; reusing `_jsonOptions`
                // reduces GC pressure during frequent saves.
                string json = JsonSerializer.Serialize(state, _jsonOptions);
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
                var loaded = JsonSerializer.Deserialize<GameState>(json, _jsonOptions);
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
