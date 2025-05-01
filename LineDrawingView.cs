using Microsoft.Maui.Graphics;

namespace NetworkMind;

public class LineDrawingView : GraphicsView, IDrawable
{
    private Point? _startPoint;
    private Point? _endPoint;

    public LineDrawingView()
    {
        Drawable = this;
    }

    public void SetPoints(Point start, Point end)
    {
        _startPoint = start;
        _endPoint = end;
        Invalidate(); // Перерисовать
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_startPoint == null || _endPoint == null) return;

        canvas.StrokeColor = Colors.Blue;
        canvas.StrokeSize = 10;

        // Рисуем линию
        canvas.DrawLine((float)_startPoint.Value.X, (float)_startPoint.Value.Y, (float)_endPoint.Value.X, (float)_endPoint.Value.Y);
    }
}