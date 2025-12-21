namespace MusicClicker
{
    /// <summary>
    /// Legacy wrapper - delegates to GameDescriptions (central source of truth)
    /// </summary>
    public static class TempoResonateDescriptions
    {
        // Delegate to central GameDescriptions
        public static string GetDescription(string itemName)
        {
            return GameDescriptions.GetTempoResonateDescription(itemName);
        }
    }
}
