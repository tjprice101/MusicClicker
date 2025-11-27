using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
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

                // Synchronous save remains available for callers that expect it.
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

        // Async save method for background saving to avoid blocking UI thread.
        public static async Task<(bool success, string error)> SaveAsync(GameState state)
        {
            try
            {
                if (!Directory.Exists(SaveFolder))
                {
                    Directory.CreateDirectory(SaveFolder);
                }

                string json = JsonSerializer.Serialize(state, _jsonOptions);
                await File.WriteAllTextAsync(SaveFile, json).ConfigureAwait(false);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Fire-and-forget background save. Errors are logged to console.
        public static void SaveBackground(GameState state)
        {
            _ = Task.Run(async () =>
            {
                var (success, error) = await SaveAsync(state).ConfigureAwait(false);
                if (!success)
                {
                    Console.WriteLine($"Background save failed: {error}");
                }
            });
        }

        public static bool Exists()
        {
            return File.Exists(SaveFile);
        }

        public static string GetPath() => SaveFile;

        public static bool TryLoad(out GameState state, out string error)
        {
            state = null;
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

                // Recalculate NPS from owned items to ensure accuracy
                loaded.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(loaded);

                // Optional migration hook: if you add migrations later, handle them here.
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
