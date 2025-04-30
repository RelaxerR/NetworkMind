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

    #region Btns

    private void CheckBtn_Clicked(object? sender, EventArgs e)
    {
        Console.WriteLine("CheckBtn_Clicked");
        UpdatePoints(Switch1ContainerDots);
        UpdatePoints(Switch2ContainerDots);
    }
    
    private void OnPointTapped(object? sender, EventArgs e)
    {
        if (sender is not Ellipse tappedPoint) return;

        if (_selectedPoint1 == null)
        {
            _selectedPoint1 = tappedPoint;
            tappedPoint.Fill = new SolidColorBrush(Colors.Green); // Выделение точки
        }
        else if (_selectedPoint2 == null)
        {
            _selectedPoint2 = tappedPoint;
            tappedPoint.Fill = new SolidColorBrush(Colors.Green); // Выделение точки

            // TODO: draw line
        }
    }
    

    #endregion
}