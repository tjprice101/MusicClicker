using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MusicClicker
{
    public partial class MainWindow : Window
    {
        private int _harmony = 0;

        public MainWindow()
        {
            InitializeComponent(); // MUST be first

            // Wire up click AFTER InitializeComponent
            ClickButton.Click += ClickButton_Click;
        }

        private void ClickButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            _harmony++;
            HarmonyText.Text = $"Harmony: {_harmony}";
        }
    }
}
