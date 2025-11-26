using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MusicClicker.Views
{
	public partial class WinterEventScreen : UserControl
	{
		private GameState? _gameState;

		public WinterEventScreen()
		{
			InitializeComponent();
			BackButton.Click += BackButton_Click;
			SoulOfWinterButton.Click += SoulOfWinterButton_Click;
			WinterMajorButton.Click += WinterMajorButton_Click;
		}

		public void SetGameState(GameState gameState)
		{
			_gameState = gameState;
			UpdateUI(gameState);
		}

		public void UpdateUI(GameState gameState)
		{
			_gameState = gameState;

			double cost = (gameState.NotesPerSecond * 3) + 100;
			SoulOfWinterCostText.Text = $"{Math.Round(cost, 1)} Notes for Soul Resonance";
			WinterMajorOwnedText.Text = $"Major Sheets of Winter Owned: {gameState.WinterMajorSheets}";

			if (this.FindControl<TextBlock>("NotesText") is TextBlock notes)
			{
				notes.Text = $"Notes: {MusicClicker.UIUpdater.FormatNotes(gameState.Notes)}";
			}

			try
			{
				WinterMajorOwnedRightText.Text = $"Number Owned: {gameState.WinterOwned}";
				WinterCrescendanceText.Text = "Crescendance Ability: <placeholder>";
			}
			catch { }
		}

		private void SoulOfWinterButton_Click(object? sender, RoutedEventArgs e)
		{
			var current = this.Parent;
			while (current != null && current is not Window) current = current.Parent;
			MainWindow? mw = current as MainWindow;

			GameState? gs = _gameState ?? mw?.GameState;
			if (gs == null) return;

			double cost = (gs.NotesPerSecond * 3) + 100;
			double before = MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes);
			if (before < cost) { UpdateUI(gs); return; }

			Console.WriteLine($"[WinterEvent] {DateTime.Now:HH:mm:ss.fff} Thread:{System.Threading.Thread.CurrentThread.ManagedThreadId} Attempt cost={cost}, beforeNotes={before}");
			double after = MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);
			double delta = after - before;
			Console.WriteLine($"[WinterEvent] {DateTime.Now:HH:mm:ss.fff} Thread:{System.Threading.Thread.CurrentThread.ManagedThreadId} After deduction notes={after}, delta={delta}");
			if (delta > 0) Console.WriteLine($"[WinterEvent] WARNING: net delta positive ({delta}) after supposed deduction of {cost}");

		var rng = new Random();
		// 1-in-3 chance
		if (rng.Next(1, 4) == 1)
		{
			gs.WinterMajorSheets++;
		}			if (mw != null)
			{
				Avalonia.Threading.Dispatcher.UIThread.Post(() =>
				{
					UpdateUI(gs);
					UIUpdater.UpdateUI(mw, gs);
				}, Avalonia.Threading.DispatcherPriority.Background);
			}
			else
			{
				UpdateUI(gs);
			}
		}

		private void WinterMajorButton_Click(object? sender, RoutedEventArgs e)
		{
			MainWindow? mwParent = null;
			var current = this.Parent;
			while (current != null && current is not Window) current = current.Parent;
			if (current is MainWindow mw) mwParent = mw;

			GameState? gs = _gameState ?? mwParent?.GameState;
			if (gs == null) return;

		if (gs.WinterMajorSheets > 0)
		{
			gs.WinterMajorSheets--;
			var rng = new Random();
			if (rng.Next(1, 251) == 1)
			{
				gs.WinterOwned++;
			}				if (mwParent != null)
				{
					Avalonia.Threading.Dispatcher.UIThread.Post(() =>
					{
						UpdateUI(gs);
						UIUpdater.UpdateUI(mwParent, gs);
					}, Avalonia.Threading.DispatcherPriority.Background);
				}
				else
				{
					UpdateUI(gs);
				}
			}
		}

		private async void BackButton_Click(object? sender, RoutedEventArgs e)
		{
			var current = this.Parent;
			while (current != null && current is not Window)
			{
				current = current.Parent;
			}

			if (current is MainWindow mw)
			{
				await mw.TransitionAsync(() =>
				{
					this.IsVisible = false;
					var eternalModulationScreen = mw.FindControl<UserControl>("EternalModulationScreen");
					if (eternalModulationScreen != null)
						eternalModulationScreen.IsVisible = true;
				});
			}
			else
			{
				this.IsVisible = false;
				if (current is Window parentWindow)
				{
					var eternalModulationScreen = parentWindow.FindControl<UserControl>("EternalModulationScreen");
					if (eternalModulationScreen != null)
						eternalModulationScreen.IsVisible = true;
				}
			}
		}
	}
}
