using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicClicker.Views
{
    public partial class BackgroundCustomizeScreen : UserControl
    {
        private GameState? _gameState;
        private MainWindow? _mainWindow;

        public BackgroundCustomizeScreen()
        {
            InitializeComponent();

            BackButton.Click += BackButton_Click;

            // Initialize all background option buttons
            InitializeBackgroundOptions();
        }

        public void SetGameState(GameState gameState, MainWindow mainWindow)
        {
            _gameState = gameState;
            _mainWindow = mainWindow;
            PopulateOptionThumbnails();
        }

        private void InitializeBackgroundOptions()
        {
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

        private void PopulateOptionThumbnails()
        {
            // Display thumbnails for option 1 (default background) and option 2 (customize screen background)
            try
            {
                // Default main background used in MainWindow.axaml
                var defaultUri = new System.Uri("avares://MusicClicker/Assets/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png");
                using var ds = Avalonia.Platform.AssetLoader.Open(defaultUri);
                var defaultBmp = new Avalonia.Media.Imaging.Bitmap(ds);
                BackgroundOption1.Content = new Image { Source = defaultBmp, Stretch = Avalonia.Media.Stretch.UniformToFill };

                // Customization screen background asset (sample)
                var customUri = new System.Uri("avares://MusicClicker/Assets/CustomizeScreenBackground.png");
                using var cs = Avalonia.Platform.AssetLoader.Open(customUri);
                var customBmp = new Avalonia.Media.Imaging.Bitmap(cs);
                BackgroundOption2.Content = new Image { Source = customBmp, Stretch = Avalonia.Media.Stretch.UniformToFill };
            }
            catch
            {
                // ignore asset loading failures silently (thumbnails are optional)
            }
        }

        private void HandleBackgroundSelection(int optionNumber)
        {
            if (_mainWindow == null)
                return;

            try
            {
                if (optionNumber == 1)
                {
                    var uri = new System.Uri("avares://MusicClicker/Assets/sacredtrevor_A_grand_musical_city_lights_everywhere_popular_shi_d84ff662-c87b-4630-9887-25228f42097b-min.png");
                    using var s = Avalonia.Platform.AssetLoader.Open(uri);
                    var bmp = new Avalonia.Media.Imaging.Bitmap(s);
                    _mainWindow.Background = new Avalonia.Media.ImageBrush { Source = bmp, Stretch = Avalonia.Media.Stretch.UniformToFill };
                }
                else if (optionNumber == 2)
                {
                    var uri = new System.Uri("avares://MusicClicker/Assets/CustomizeScreenBackground.png");
                    using var s = Avalonia.Platform.AssetLoader.Open(uri);
                    var bmp = new Avalonia.Media.Imaging.Bitmap(s);
                    _mainWindow.Background = new Avalonia.Media.ImageBrush { Source = bmp, Stretch = Avalonia.Media.Stretch.UniformToFill };
                }
            }
            catch
            {
                // ignore failures to change background
            }
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