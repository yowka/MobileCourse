using Course.Models;
using Course.Services;
using System.Text.Json;

namespace Course.ViewModels
{
    public class MainViewModel
    {
        private readonly IWeatherService _weatherService;
        public CurrentWeatherResponse Weather { get; set; } = new();

        public bool HasWeather => Weather != null && !string.IsNullOrEmpty(Weather.Name);

        public MainViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task LoadWeather()
        {
            try
            {
                var cityJson = Preferences.Get("current_city", "");
                if (string.IsNullOrEmpty(cityJson)) return;

                var city = JsonSerializer.Deserialize<CitySearchResponse>(cityJson);
                if (city == null) return;

                Weather = await _weatherService.GetCurrentWeatherAsync(city.Latitude, city.Longitude);

                if (Weather != null && string.IsNullOrEmpty(Weather.Name))
                {
                    Weather.Name = city.Name;
                }
            }
            catch
            {
                Weather = new CurrentWeatherResponse();
            }
        }
    }
}