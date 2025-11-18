using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class EternalModulationScreen : UserControl
    {
        public EternalModulationScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;
            DiesIraeBannerButton.Click += DiesIraeBannerButton_Click;
            WinterBannerButton.Click += WinterBannerButton_Click;
        }

        private void DiesIraeBannerButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show Dies Irae event screen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var diesIraeScreen = parentWindow.FindControl<DiesIraeEventScreen>("DiesIraeEventScreen");
                if (diesIraeScreen != null)
                {
                    // Get MainWindow to access GameState
                    var mainWindow = parentWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        diesIraeScreen.UpdateUI(mainWindow.GameState);
                    }
                    diesIraeScreen.IsVisible = true;
                }
            }
        }

        private void WinterBannerButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show Winter event screen
            this.IsVisible = false;

            var current = this.Parent;
            while (current != null && current is not Window)
            {
                current = current.Parent;
            }

            if (current is Window parentWindow)
            {
                var winterScreen = parentWindow.FindControl<WinterEventScreen>("WinterEventScreen");
                if (winterScreen != null)
                {
                    // Get MainWindow to access GameState
                    var mainWindow = parentWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        winterScreen.UpdateUI(mainWindow.GameState);
                    }
                    winterScreen.IsVisible = true;
                }
            }
        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            // Hide this screen and show main screen
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