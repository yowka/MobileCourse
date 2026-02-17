using Microsoft.Maui.Controls;
using Course.Views;

namespace Course.Views
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
            _ = StartAnimationAndNavigate();
        }

        private async Task StartAnimationAndNavigate()
        {
            await Task.Delay(800);

            WeatherIcon.Opacity = 0;
            WeatherIcon.Scale = 1;

            await WeatherIcon.FadeTo(1, 800, Easing.SinOut); 


            await Task.Delay(800);

            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}