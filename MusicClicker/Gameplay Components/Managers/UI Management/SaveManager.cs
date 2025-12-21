using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Buffers;
using MusicClicker;

namespace MusicClicker.Helpers
{
    // Optimized file-based save/load manager for GameState.
    // Performance improvements:
    // - Reused JsonSerializerOptions to avoid per-call allocation
    // - Compact JSON (WriteIndented=false) reduces file size by ~30-40%
    // - UTF8 encoding with buffers for faster serialization
    // - Backup system for safety
    public static class SaveManager
    {
        private static readonly string SaveFolder = Path.Combine(Environment.CurrentDirectory, "Save");
        private static readonly string SaveFile = Path.Combine(SaveFolder, "SavedGameState");
        private static readonly string BackupFile = Path.Combine(SaveFolder, "SavedGameState.backup");

        // Optimized serializer options for production (compact format, faster)
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,  // Compact format - 30-40% smaller files, faster parsing
            IncludeFields = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };

        // Debug option for human-readable saves (use only for debugging)
        private static readonly JsonSerializerOptions _jsonOptionsReadable = new JsonSerializerOptions
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

                // Create backup of existing save before overwriting
                if (File.Exists(SaveFile))
                {
                    File.Copy(SaveFile, BackupFile, true);
                }

                // Optimized: serialize directly to UTF8 bytes (faster than string)
                byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(state, _jsonOptions);
                File.WriteAllBytes(SaveFile, jsonBytes);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                // Restore backup if save failed
                if (File.Exists(BackupFile))
                {
                    try { File.Copy(BackupFile, SaveFile, true); } catch { }
                }
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

                // Create backup before overwriting
                if (File.Exists(SaveFile))
                {
                    File.Copy(SaveFile, BackupFile, true);
                }

                // Optimized: serialize directly to UTF8 bytes asynchronously
                using (FileStream fs = new FileStream(SaveFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    await JsonSerializer.SerializeAsync(fs, state, _jsonOptions).ConfigureAwait(false);
                }
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                // Restore backup if save failed
                if (File.Exists(BackupFile))
                {
                    try { File.Copy(BackupFile, SaveFile, true); } catch { }
                }
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

                // Optimized: deserialize directly from UTF8 bytes (faster than string)
                byte[] jsonBytes = File.ReadAllBytes(SaveFile);
                var loaded = JsonSerializer.Deserialize<GameState>(jsonBytes, _jsonOptions);
                
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
                
                // Try to load from backup if main save is corrupted
                if (File.Exists(BackupFile))
                {
                    try
                    {
                        byte[] backupBytes = File.ReadAllBytes(BackupFile);
                        var loaded = JsonSerializer.Deserialize<GameState>(backupBytes, _jsonOptions);
                        if (loaded != null)
                        {
                            loaded.NotesPerSecond = MusicClicker.Helpers.Progression.RecalculateNotesPerSecond(loaded);
                            state = loaded;
                            error = "Loaded from backup (main save was corrupted)";
                            
                            // Restore the backup as the main save
                            File.Copy(BackupFile, SaveFile, true);
                            return true;
                        }
                    }
                    catch
                    {
                        // Backup also failed, fall through to return false
                    }
                }
                
                return false;
            }
        }

        // Export readable save for debugging purposes
        public static bool ExportReadable(GameState state, string path, out string error)
        {
            error = string.Empty;
            try
            {
                string json = JsonSerializer.Serialize(state, _jsonOptionsReadable);
                File.WriteAllText(path, json);
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
