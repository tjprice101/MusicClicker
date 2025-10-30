using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Avalonia;
using System;

namespace MusicClicker
{
    public class TempoResonateManager
    {
        private Canvas _tileCanvas;
        private StackPanel _slotPanel;
        private ComboBox _tileSelector;

        private Rectangle? _draggedTile;
        private Point _dragStart;

        public TempoResonateManager(Canvas tileCanvas, StackPanel slotPanel, ComboBox tileSelector)
        {
            _tileCanvas = tileCanvas;
            _slotPanel = slotPanel;
            _tileSelector = tileSelector;

            // Hook up events
            _tileCanvas.PointerPressed += TileCanvas_PointerPressed;
            _tileCanvas.PointerMoved += TileCanvas_PointerMoved;
            _tileCanvas.PointerReleased += TileCanvas_PointerReleased;
        }

        private void TileCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_tileSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string colorName = selectedItem.Content.ToString()!;
                IBrush brush = colorName switch
                {
                    "Red" => Brushes.Red,
                    "Green" => Brushes.Green,
                    "Blue" => Brushes.Blue,
                    "Yellow" => Brushes.Yellow,
                    _ => Brushes.Gray
                };

                _draggedTile = new Rectangle
                {
                    Width = 80,
                    Height = 80,
                    Fill = brush,
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };

                _dragStart = e.GetPosition(_tileCanvas);
                Canvas.SetLeft(_draggedTile, _dragStart.X - 40);
                Canvas.SetTop(_draggedTile, _dragStart.Y - 40);
                _tileCanvas.Children.Add(_draggedTile);
            }
        }

        private void TileCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_draggedTile != null && e.GetCurrentPoint(_tileCanvas).Properties.IsLeftButtonPressed)
            {
                Point pos = e.GetPosition(_tileCanvas);
                Canvas.SetLeft(_draggedTile, pos.X - 40);
                Canvas.SetTop(_draggedTile, pos.Y - 40);
            }
        }

        private void TileCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_draggedTile != null)
            {
                // Check for drop on slots
                foreach (var child in _slotPanel.Children)
                {
                    if (child is Rectangle slot)
                    {
                        Rect slotRect = new Rect(
                            slot.TranslatePoint(new Point(0,0), _tileCanvas)!.Value,
                            new Size(slot.Width, slot.Height));

                        Point tilePos = _draggedTile.TranslatePoint(new Point(0, 0), _tileCanvas)!.Value;

                        if (slotRect.Contains(tilePos))
                        {
                            slot.Fill = _draggedTile.Fill;
                            break;
                        }
                    }
                }

                _tileCanvas.Children.Remove(_draggedTile);
                _draggedTile = null;
            }
        }
    }
}
