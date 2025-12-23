namespace MusicClicker
{
    /// <summary>
    /// Legacy wrapper - delegates to GameDescriptions (central source of truth)
    /// </summary>
    public static class TempoResonateDescriptions
    {
        // All tempo resonate descriptions are now updated in GameDescriptions
        public static string GetDescription(string itemName)
        {
            return GameDescriptions.GetTempoResonateDescription(itemName);
        }
    }
}
