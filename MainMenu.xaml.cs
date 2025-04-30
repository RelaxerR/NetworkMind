using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMind;

public partial class MainMenu : ContentPage
{
    public MainMenu()
    {
        InitializeComponent();
    }

    private void StartBtn_Clicked(object? sender, EventArgs e)
    {
        // Перейти к игре
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }
}