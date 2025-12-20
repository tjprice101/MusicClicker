/*
 * File: Views/UpgradeScreen.axaml.cs
 * Summary: Code-behind for the Upgrades screen UI.
 * Purpose: Handles upgrade button wiring and reflects upgrade-related GameState in the UI.
 */

using Avalonia.Controls;
using MusicClicker;

namespace MusicClicker.Views
{
    public partial class UpgradeScreen : UserControl
    {
        public UpgradeScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Refreshes the owned counters in the Upgrade screen to match the provided
        /// window's GameState. This is a small helper used by other systems when
        /// weapon abilities or programmatic changes modify owned counts.
        /// </summary>
        public void RefreshOwnedTexts(MainWindow window)
        {
            if (window == null) return;

            var gs = window.GameState;

            ChordOwnedTextUpgrade.Text = $"Number Owned: {gs.ChordOwned} - Boosts NPS by 5%.";
            ScaleOwnedTextUpgrade.Text = $"Number Owned: {gs.ScaleOwned} - Increases crit chance by 10%.";
            OrchestraOwnedTextUpgrade.Text = $"Number Owned: {gs.OrchestraOwned} - Enhances click damage by 15%.";
            SymphonyOwnedTextUpgrade.Text = $"Number Owned: {gs.SymphonyOwned} - Grants +20% to all buffs.";

            AriaOwnedTextUpgrade.Text = $"Number Owned: {gs.AriaOwned}";
            RequiemOwnedTextUpgrade.Text = $"Number Owned: {gs.RequiemOwned}";
            OpusOwnedTextUpgrade.Text = $"Number Owned: {gs.OpusOwned}";
            MagnumOpusOwnedTextUpgrade.Text = $"Number Owned: {gs.MagnumOpusOwned}";
        }
    }
}