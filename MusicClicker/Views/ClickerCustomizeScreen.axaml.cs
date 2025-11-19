using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class ClickerCustomizeScreen : UserControl
    {
        public ClickerCustomizeScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            
            // Initialize all clicker option buttons (placeholder for future functionality)
            InitializeClickerOptions();
        }

        private void InitializeClickerOptions()
        {
            // Placeholder click handlers for all 16 clicker options
            ClickerOption1.Click += (s, e) => HandleClickerSelection(1);
            ClickerOption2.Click += (s, e) => HandleClickerSelection(2);
            ClickerOption3.Click += (s, e) => HandleClickerSelection(3);
            ClickerOption4.Click += (s, e) => HandleClickerSelection(4);
            ClickerOption5.Click += (s, e) => HandleClickerSelection(5);
            ClickerOption6.Click += (s, e) => HandleClickerSelection(6);
            ClickerOption7.Click += (s, e) => HandleClickerSelection(7);
            ClickerOption8.Click += (s, e) => HandleClickerSelection(8);
            ClickerOption9.Click += (s, e) => HandleClickerSelection(9);
            ClickerOption10.Click += (s, e) => HandleClickerSelection(10);
            ClickerOption11.Click += (s, e) => HandleClickerSelection(11);
            ClickerOption12.Click += (s, e) => HandleClickerSelection(12);
            ClickerOption13.Click += (s, e) => HandleClickerSelection(13);
            ClickerOption14.Click += (s, e) => HandleClickerSelection(14);
            ClickerOption15.Click += (s, e) => HandleClickerSelection(15);
            ClickerOption16.Click += (s, e) => HandleClickerSelection(16);
        }

        private void HandleClickerSelection(int optionNumber)
        {
            // Placeholder for clicker button selection logic
            // This will update the main clicker button image
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show SymphonicGalleryScreen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var galleryScreen = parentWindow.FindControl<Views.SymphonicGalleryScreen>("SymphonicGalleryScreen");
                if (galleryScreen != null)
                    galleryScreen.IsVisible = true;
            }
        }
    }
}