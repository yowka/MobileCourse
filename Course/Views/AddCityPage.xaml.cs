using Course.Models;
using Course.Services;
using System.Text.Json;

namespace Course.Views
{
    public partial class AddCityPage : ContentPage
    {
        private WeatherService _weatherService = new();

        public AddCityPage()
        {
            InitializeComponent();
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            var cityName = SearchEntry.Text;
            if (!string.IsNullOrWhiteSpace(cityName))
            {
                var cities = await _weatherService.SearchCitiesAsync(cityName);
                CitiesList.ItemsSource = cities;
            }
        }

        private async void OnCitySelected(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                var city = button.BindingContext as CitySearchResponse;
                if (city != null)
                {
                    var json = JsonSerializer.Serialize(city);
                    Preferences.Set("current_city", json);
                    await Navigation.PopModalAsync();
                }
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}