using Microsoft.Maui.Graphics;
using System.Collections.Generic;

namespace NetworkMind;

public class LineDrawingView : GraphicsView, IDrawable
{
    private readonly List<(Point Start, Point End)> _lines = new();
    private readonly Random _random;
    private readonly List<Color> _colors = new()
    {
        Colors.Blue,
        Colors.Green,
        Colors.Yellow,
    };
    private readonly int minStrokeSize = 5;
    private readonly int maxStrokeSize = 10;

    public LineDrawingView()
    {
        Drawable = this;
        _random = new Random();
    }

    public void SetPoints(Point start, Point end)
    {
        _lines.Add((start, end)); // Добавляем новую линию в список
        Invalidate(); // Перерисовать
    }

    public void ClearLines()
    {
        _lines.Clear(); // Очищаем список линий
        Invalidate(); // Перерисовать
    }
    public void RemoveLine(Point start, Point end)
    {
        _lines.Remove((start, end)); // Удаляем линию из списка
        Invalidate(); // Перерисовать
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = _colors[_random.Next(_colors.Count)];
        canvas.StrokeSize = _random.Next(minStrokeSize, maxStrokeSize);

        // Рисуем все линии из списка
        foreach (var line in _lines)
        {
            canvas.DrawLine((float)line.Start.X, (float)line.Start.Y, (float)line.End.X, (float)line.End.Y);
        }
    }
}