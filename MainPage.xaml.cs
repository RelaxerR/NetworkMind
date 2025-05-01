using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace NetworkMind;

public partial class MainPage : ContentPage
{
    private readonly List<string> _randomTexts = new() { "GE0/1", "GE0/2", "FE0/1", "FE0/2", "S0/0/0", "S0/0/1", "V1", "L0", "E1/0", "E1/1" };
    private readonly Random _random = new();
    
    private Ellipse? _selectedPoint1;
    private Ellipse? _selectedPoint2;
    
    public MainPage()
    {
        InitializeComponent();
        AddTapGestureToPoints(Switch1ContainerDots);
        AddTapGestureToPoints(Switch2ContainerDots);
    }

    #region Check

    private bool CheckConnections()
    {
        bool allConnectionsCorrect = true;

        foreach (var connection in _pointLines)
        {
            var point1 = connection.Key;
            var (start, end) = connection.Value;

            // Получаем названия точек из Label над точками
            var label1 = GetLabelAbovePoint(point1);
            var label2 = GetLabelAbovePoint(_pointLines.FirstOrDefault(p => p.Value.Start == end).Key);
                
            Console.WriteLine($"label1: {label1?.Text}, label2: {label2?.Text}");
            
            if (label1 == null || label2 == null || label1.Text != label2.Text)
            {
                allConnectionsCorrect = false;
                break;
            }
        }

        if (allConnectionsCorrect)
        {
            Console.WriteLine("Все соединения корректны!");
        }
        else
        {
            Console.WriteLine("Есть некорректные соединения.");
        }

        return allConnectionsCorrect;
    }

    private Label? GetLabelAbovePoint(Ellipse point)
    {
        var parentGrid = point.Parent as Grid;
        if (parentGrid == null) return null;

        // Получаем колонку точки
        int column = Grid.GetColumn(point);

        // Ищем Label в этой колонке
        foreach (var child in parentGrid.Children)
        {
            if (child is Label label && Grid.GetColumn(label) == column)
            {
                return label;
            }
        }

        return null;
    }

    #endregion

    #region Select nodes

    private void AddTapGestureToPoints(Grid grid)
    {
        foreach (var child in grid.Children)
        {
            if (child is Ellipse ellipse)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += OnPointTapped;
                ellipse.GestureRecognizers.Add(tapGesture);
            }
        }
    }

    #endregion

    #region Level genertion
    
    public void UpdatePoints(Grid grid)
    {
        // Список колонок для скрытия
        var columnsToHide = new HashSet<int>();

        // Генерируем случайные колонки для скрытия
        for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
        {
            if (_random.Next(0, 2) == 0) // 50% шанс скрыть колонку
            {
                columnsToHide.Add(i);
            }
        }

        // Обходим все элементы в Grid
        foreach (var child in grid.Children)
        {
            if (child is Label label)
            {
                // Меняем текст на случайный из списка
                label.Text = _randomTexts[_random.Next(_randomTexts.Count)];
                Console.WriteLine($"Label updated: {label.Text}");

                // Скрываем, если колонка в списке скрытых
                if (columnsToHide.Contains(Grid.GetColumn(label)))
                {
                    label.IsVisible = false;
                    Console.WriteLine($"Label hidden: {Grid.GetColumn(label)}");
                }
                else
                {
                    label.IsVisible = true;
                }
            }
            else if (child is Ellipse ellipse)
            {
                // Скрываем, если колонка в списке скрытых
                if (columnsToHide.Contains(Grid.GetColumn(ellipse)))
                {
                    ellipse.IsVisible = false;
                    Console.WriteLine($"Ellipse hidden: {Grid.GetColumn(ellipse)}");
                }
                else
                {
                    ellipse.IsVisible = true;
                }
            }
        }
    }
    #endregion

    #region GetCoords

    public static IEnumerable<VisualElement> Ancestors(VisualElement element)
    {
        while(element != null)
        {
            yield return element;
            element = element.Parent as VisualElement;
        }
    }
    private static Point GetAbsolutePosition(VisualElement visualElement)
    {
        var ancestors = Ancestors(visualElement);
        var x = ancestors.Sum(ancestor => ancestor.X) - (visualElement.Width / 2);
        var y = ancestors.Sum(ancestor => ancestor.Y) - (visualElement.Height / 2);

        return new Point(x, y);
    }
    

    #endregion
    
    #region Btns

    private async void CheckBtn_Clicked(object? sender, EventArgs e)
    {
        Console.WriteLine("CheckBtn_Clicked");
        if (CheckConnections())
        {
            LineCanvas.ClearLines(); // Очищаем линии перед обновлением
            UpdatePoints(Switch1ContainerDots);
            UpdatePoints(Switch2ContainerDots);
            await DisplayAlert("Результат", "Все соединения корректны!", "OK");
        }
        else
        {
            await DisplayAlert("Результат", "Есть некорректные соединения.", "OK");
        }
    }
    
    private readonly Dictionary<Ellipse, (Point Start, Point End)> _pointLines = new();

    private void OnPointTapped(object? sender, EventArgs e)
    {
        if (sender is not Ellipse tappedPoint) return;

        // Удаление старой линии, если она существует
        if (_pointLines.ContainsKey(tappedPoint))
        {
            var lineToRemove = _pointLines[tappedPoint];
            LineCanvas.RemoveLine(lineToRemove.Start, lineToRemove.End); // Удаляем конкретную линию
            _pointLines.Remove(tappedPoint); // Удаляем из словаря
        }

        if (_selectedPoint1 == null)
        {
            _selectedPoint1 = tappedPoint;
            tappedPoint.Fill = new SolidColorBrush(Colors.Green); // Выделение точки
        }
        else if (_selectedPoint2 == null)
        {
            _selectedPoint2 = tappedPoint;
            tappedPoint.Fill = new SolidColorBrush(Colors.Green); // Выделение точки

            // Получение координат точек
            var startPoint = GetAbsolutePosition(_selectedPoint1);
            var endPoint = GetAbsolutePosition(_selectedPoint2);

            // Рисование линии
            if (startPoint != null && endPoint != null)
            {
                LineCanvas.SetPoints(startPoint, endPoint);

                // Сохранение линии в словарь
                _pointLines[_selectedPoint1] = (startPoint, endPoint);
                _pointLines[_selectedPoint2] = (endPoint, startPoint);
            }

            // Сброс выделения
            _selectedPoint1.Fill = new SolidColorBrush(Colors.Red);
            _selectedPoint2.Fill = new SolidColorBrush(Colors.Red);
            _selectedPoint1 = null;
            _selectedPoint2 = null;
        }
    }
    private void ClearBtn_OnClicked(object? sender, EventArgs e)
    {
        LineCanvas.ClearLines(); // Очищаем все линии
        _pointLines.Clear(); // Очищаем словарь линий
        Console.WriteLine("Все линии очищены.");
    }

    private async void BackBtn_OnClicked(object? sender, EventArgs e)
    {
        // Переход в меню
        Application.Current.MainPage = new NavigationPage(new MainMenu());
        Console.WriteLine("Возврат в меню.");
    }

    #endregion
}