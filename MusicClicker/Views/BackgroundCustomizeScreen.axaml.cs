using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class BackgroundCustomizeScreen : UserControl
    {
        public BackgroundCustomizeScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            
            // Initialize all background option buttons (placeholder for future functionality)
            InitializeBackgroundOptions();
        }

        private void InitializeBackgroundOptions()
        {
            // Placeholder click handlers for all 10 background options
            BackgroundOption1.Click += (s, e) => HandleBackgroundSelection(1);
            BackgroundOption2.Click += (s, e) => HandleBackgroundSelection(2);
            BackgroundOption3.Click += (s, e) => HandleBackgroundSelection(3);
            BackgroundOption4.Click += (s, e) => HandleBackgroundSelection(4);
            BackgroundOption5.Click += (s, e) => HandleBackgroundSelection(5);
            BackgroundOption6.Click += (s, e) => HandleBackgroundSelection(6);
            BackgroundOption7.Click += (s, e) => HandleBackgroundSelection(7);
            BackgroundOption8.Click += (s, e) => HandleBackgroundSelection(8);
            BackgroundOption9.Click += (s, e) => HandleBackgroundSelection(9);
            BackgroundOption10.Click += (s, e) => HandleBackgroundSelection(10);
        }

        private void HandleBackgroundSelection(int optionNumber)
        {
            // Placeholder for background selection logic
            // This will update the main menu background
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
                var galleryScreen = parentWindow.FindControl<SymphonicGalleryScreen>("SymphonicGalleryScreen");
                if (galleryScreen != null)
                    galleryScreen.IsVisible = true;
            }
        }
    }
}