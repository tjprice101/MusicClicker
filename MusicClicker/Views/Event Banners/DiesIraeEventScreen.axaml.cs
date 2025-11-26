using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MusicClicker.Views
{
	public partial class DiesIraeEventScreen : UserControl
	{
		private GameState? _gameState;

		public DiesIraeEventScreen()
		{
			InitializeComponent();
			BackButton.Click += BackButton_Click;
			SoulOfDiesIraeButton.Click += SoulOfDiesIraeButton_Click;
			DiesIraeMajorButton.Click += DiesIraeMajorButton_Click;
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
		SoulOfDiesIraeCostText.Text = $"{Math.Round(cost, 1)} Notes for Soul Resonance";
		DiesIraeMajorOwnedText.Text = $"Major Sheets of Dies Irae Owned: {gameState.DiesIraeMajorSheets}";			if (this.FindControl<TextBlock>("NotesText") is TextBlock notes)
			{
				notes.Text = $"Notes: {Math.Round(gameState.Notes, 1)}";
			}
			if (this.FindControl<TextBlock>("NpsText") is TextBlock nps)
			{
				nps.Text = $"Notes Per Second: {Math.Round(gameState.NotesPerSecond, 1)}";
			}

			try
			{
				DiesIraeMajorOwnedRightText.Text = $"Number Owned: {gameState.DiesIraeOwned}";
				DiesIraeCrescendanceText.Text = "Crescendance Ability: <placeholder>";
			}
			catch { }
		}

	private void SoulOfDiesIraeButton_Click(object? sender, RoutedEventArgs e)
	{
		var current = this.Parent;
		while (current != null && current is not Window) current = current.Parent;
		MainWindow? mw = current as MainWindow;

		GameState? gs = _gameState ?? mw?.GameState;
		if (gs == null) return;

		double cost = (gs.NotesPerSecond * 3) + 100;
		double before = MusicClicker.Helpers.AtomicDouble.Read(ref gs._notes);
		if (before < cost) { UpdateUI(gs); return; }

		Console.WriteLine($"[DiesIraeEvent] {DateTime.Now:HH:mm:ss.fff} Thread:{System.Threading.Thread.CurrentThread.ManagedThreadId} Attempt cost={cost}, beforeNotes={before}");
		double after = MusicClicker.Helpers.AtomicDouble.Add(ref gs._notes, -cost);
		double delta = after - before;
		Console.WriteLine($"[DiesIraeEvent] {DateTime.Now:HH:mm:ss.fff} Thread:{System.Threading.Thread.CurrentThread.ManagedThreadId} After deduction notes={after}, delta={delta}");
		if (delta > 0) Console.WriteLine($"[DiesIraeEvent] WARNING: net delta positive ({delta}) after supposed deduction of {cost}");

		var rng = new Random();
		// 1-in-3 chance
		if (rng.Next(1, 4) == 1)
		{
			gs.DiesIraeMajorSheets++;
		}

		if (mw != null)
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
	}		private void DiesIraeMajorButton_Click(object? sender, RoutedEventArgs e)
		{
			MainWindow? mwParent = null;
			var current = this.Parent;
			while (current != null && current is not Window) current = current.Parent;
			if (current is MainWindow mw) mwParent = mw;

			GameState? gs = _gameState ?? mwParent?.GameState;
			if (gs == null) return;

		if (gs.DiesIraeMajorSheets > 0)
		{
			gs.DiesIraeMajorSheets--;
			var rng = new Random();
			if (rng.Next(1, 251) == 1)
			{
				gs.DiesIraeOwned++;
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
