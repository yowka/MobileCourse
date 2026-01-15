using Course.Models;
using Course.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Course.ViewModels
{
    public class MainViewModel
    {
        private WeatherService _weatherService;
        public ObservableCollection<CityWeather> CitiesWeather { get; } = new ObservableCollection<CityWeather>();

        public MainViewModel()
        {
            _weatherService = new WeatherService();
        }

        public async Task LoadCitiesWeather()
        {
            CitiesWeather.Clear();

            // Загружаем сохраненные города
            var savedCities = LoadSavedCities();

            foreach (var city in savedCities)
            {
                var weather = await _weatherService.GetCurrentWeatherAsync(city.Latitude, city.Longitude);
                if (weather != null)
                {
                    CitiesWeather.Add(new CityWeather
                    {
                        Name = weather.CityName ?? city.Name,
                        Temperature = Math.Round(weather.Main?.Temperature ?? 0),
                        WeatherDescription = weather.Weather?[0]?.Description ?? "Нет данных"
                    });
                }
            }
        }

        // Сохраняем город
        public void SaveCity(CitySearchResponse city)
        {
            var savedCities = LoadSavedCities();

            // Проверяем, нет ли уже такого города
            if (!savedCities.Any(c => c.Name == city.Name && c.Country == city.Country))
            {
                savedCities.Add(new SavedCity
                {
                    Name = city.Name,
                    Country = city.Country,
                    Latitude = city.Latitude,
                    Longitude = city.Longitude
                });

                SaveCitiesToStorage(savedCities);
            }
        }

        // Загружаем сохраненные города
        private List<SavedCity> LoadSavedCities()
        {
            try
            {
                var json = Preferences.Get("saved_cities", "[]");
                return JsonSerializer.Deserialize<List<SavedCity>>(json) ?? new List<SavedCity>();
            }
            catch
            {
                return new List<SavedCity>();
            }
        }

        // Сохраняем в Preferences
        private void SaveCitiesToStorage(List<SavedCity> cities)
        {
            var json = JsonSerializer.Serialize(cities);
            Preferences.Set("saved_cities", json);
        }
    }

    public class CityWeather
    {
        public string Name { get; set; }
        public double Temperature { get; set; }
        public string WeatherDescription { get; set; }
    }
}