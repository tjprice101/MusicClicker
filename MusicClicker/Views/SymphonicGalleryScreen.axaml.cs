using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class SymphonicGalleryScreen : UserControl
    {
        public SymphonicGalleryScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            ClickerCustomizeButton.Click += ClickerCustomizeButton_Click;
            BackgroundCustomizeButton.Click += BackgroundCustomizeButton_Click;
        }

        private void ClickerCustomizeButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show ClickerCustomizeScreen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var clickerScreen = parentWindow.FindControl<ClickerCustomizeScreen>("ClickerCustomizeScreen");
                if (clickerScreen != null)
                    clickerScreen.IsVisible = true;
            }
        }

        private void BackgroundCustomizeButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show BackgroundCustomizeScreen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var backgroundScreen = parentWindow.FindControl<BackgroundCustomizeScreen>("BackgroundCustomizeScreen");
                if (backgroundScreen != null)
                    backgroundScreen.IsVisible = true;
            }
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show the main screen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var mainScreen = parentWindow.FindControl<Grid>("MainScreen");
                if (mainScreen != null)
                    mainScreen.IsVisible = true;
            }
        }
    }
}