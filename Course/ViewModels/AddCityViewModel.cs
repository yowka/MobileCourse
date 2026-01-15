using Course.Models;
using Course.Services;
using System.Collections.ObjectModel;

namespace Course.ViewModels
{
    public class AddCityViewModel
    {
        public string SearchText { get; set; }
        public ObservableCollection<CitySearchResponse> FoundCities { get; } = new ObservableCollection<CitySearchResponse>();

        private WeatherService _weatherService;

        public AddCityViewModel()
        {
            _weatherService = new WeatherService();
        }

        public async Task SearchCities()
        {
            var cities = await _weatherService.SearchCitiesAsync(SearchText);

            FoundCities.Clear();
            foreach (var city in cities)
            {
                FoundCities.Add(city);
            }
        }

        public async Task SaveCity(CitySearchResponse city)
        {
            // Сохраняем город в Preferences или базу данных
            // Preferences.Set("saved_cities", ...);
        }
    }
}