using Microsoft.Maui.Graphics;
using System.Collections.Generic;

namespace NetworkMind;

public class LineDrawingView : GraphicsView, IDrawable
{
    private readonly List<(Point Start, Point End, Color LineColor, float StrokeSize)> _lines = new();
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
        var color = _colors[_random.Next(_colors.Count)];
        var strokeSize = _random.Next(minStrokeSize, maxStrokeSize);
        _lines.Add((start, end, color, strokeSize)); // Добавляем новую линию с цветом и размером
        Invalidate(); // Перерисовать
    }

    public void ClearLines()
    {
        _lines.Clear(); // Очищаем список линий
        Invalidate(); // Перерисовать
    }

    public void RemoveLine(Point start, Point end)
    {
        _lines.RemoveAll(line => line.Start == start && line.End == end); // Удаляем линию из списка
        Invalidate(); // Перерисовать
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Рисуем все линии из списка
        foreach (var line in _lines)
        {
            canvas.StrokeColor = line.LineColor; // Используем сохранённый цвет
            canvas.StrokeSize = line.StrokeSize; // Используем сохранённый размер
            canvas.DrawLine((float)line.Start.X, (float)line.Start.Y, (float)line.End.X, (float)line.End.Y);
        }
    }
}