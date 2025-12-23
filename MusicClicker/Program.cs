/*
 * File: Program.cs
 * Summary: Application entry point for MusicClicker.
 * Purpose: Initializes and starts the Avalonia application host.
 * Notes: High-level program bootstrap. See Development Documentation/DeveloperGuide.txt for more.
 */

using Avalonia;
using System;

namespace MusicClicker;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            try
            {
                var logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_log.txt");
                System.IO.File.WriteAllText(logPath, $"Startup Exception: {ex}\n");
            }
            catch { /* Ignore logging errors */ }
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
