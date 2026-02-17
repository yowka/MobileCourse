using Course.Models;
using Course.Services;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Course.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IWeatherService _weatherService;
        private CurrentWeatherResponse _weather = new();

        public CurrentWeatherResponse Weather
        {
            get => _weather;
            set
            {
                _weather = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasWeather));
                OnPropertyChanged(nameof(Temp));
                OnPropertyChanged(nameof(TempDisplay));
            }
        }

        public bool HasWeather => Weather != null && !string.IsNullOrEmpty(Weather.Name);

        public double Temp => Weather?.Main?.Temp ?? 0;
        public string TempDisplay => Math.Round(Temp).ToString();

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}