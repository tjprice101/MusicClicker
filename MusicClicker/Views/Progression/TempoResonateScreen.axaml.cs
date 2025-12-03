/*
 * File: Views/TempoResonateScreen.axaml.cs
 * Summary: Code-behind for the Tempo Resonate screen (music scores management).
 * Purpose: Manages equipping and resonating musical scores and updates related UI elements, including hover tooltips.
 */

using Avalonia.Controls;
using Avalonia.Input;

namespace MusicClicker.Views
{
    public partial class TempoResonateScreen : UserControl
    {
        private Border? _tooltipPanel;
        private TextBlock? _tooltipTitle;
        private TextBlock? _tooltipDescription;

        public TempoResonateScreen()
        {
            InitializeComponent();
            InitializeTooltips();
        }

        private void InitializeTooltips()
        {
            // Get tooltip panel references
            _tooltipPanel = this.FindControl<Border>("DescriptionTooltipPanel");
            _tooltipTitle = this.FindControl<TextBlock>("TooltipTitle");
            _tooltipDescription = this.FindControl<TextBlock>("TooltipDescription");

            if (_tooltipPanel != null)
            {
                _tooltipPanel.IsVisible = true; // Always visible
            }
        }

        // Attach hover handlers to an item (called when items are added to drawers)
        public void AttachTooltipHandlers(Control itemControl, string itemName)
        {
            if (itemControl == null) return;

            // Use PointerMoved for more reliable hover detection that doesn't interfere with clicks
            itemControl.PointerMoved += (sender, e) =>
            {
                ShowTooltip(itemName);
            };
            
            // Don't hide on exit - let it persist until next hover
        }

        private void ShowTooltip(string itemName)
        {
            if (_tooltipDescription == null || _tooltipTitle == null) return;

            string description = TempoResonateDescriptions.GetDescription(itemName);
            _tooltipDescription.Text = description;
            _tooltipTitle.Text = itemName; // Set title to the item name
        }

        private void HideTooltip()
        {
            // Don't hide the panel anymore - keep showing default text
            // Content persists until another item is hovered
        }

        // Public method for TempoResonateManager to attach tooltips when adding items
        public void RegisterTooltipItem(Control control, string name)
        {
            AttachTooltipHandlers(control, name);
        }
    }
}