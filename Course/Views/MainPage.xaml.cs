using Course.Models;
using Course.Services;
using System.Text.Json;

namespace Course.Views;

public partial class MainPage : ContentPage
{
    private readonly WeatherService _weatherService = new();

    public string City { get; set; } = "—";
    public string Temp { get; set; } = "—";
    public string Description { get; set; } = "—";
    public string FeelsLike { get; set; } = "—";
    public string Humidity { get; set; } = "—";
    public string Pressure { get; set; } = "—";
    public string WindSpeed { get; set; } = "—";
    public string WindDirection { get; set; } = "—";
    public string Sunrise { get; set; } = "—";
    public string Sunset { get; set; } = "—";

    public List<ForecastItem> Hourly24 { get; set; } = new();
    public List<ForecastItem> Daily5 { get; set; } = new();
    public MainPage()
    {
        InitializeComponent();
        
    _ = LoadWeather(); 
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadWeather();
    }
    private async void OnAddCityClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddCityPage());
    }

    private async Task LoadWeather()
    {
        try
        {
            var cityJson = Preferences.Get("current_city", null);
            if (string.IsNullOrEmpty(cityJson)) return;

            var city = JsonSerializer.Deserialize<CitySearchResponse>(cityJson);
            var weatherService = new WeatherService();

            // Текущая погода
            var current = await weatherService.GetCurrentWeatherAsync(city.Latitude, city.Longitude);
            if (current != null)
            {
                City = current.Name;
                Temp = Math.Round(current.Main.Temp).ToString();
                Description = current.Weather.FirstOrDefault()?.Description ?? "";
                FeelsLike = $"Ощущается: {Math.Round(current.Main.Feels_like)}°C";
                Humidity = $"Влажность: {current.Main.Humidity}%";
                Pressure = $"Давление: {current.Main.Pressure} гПа";
                WindSpeed = $"Ветер: {current.Wind.Speed} м/с";

                var dirs = new[] { "С", "СВ", "В", "ЮВ", "Ю", "ЮЗ", "З", "СЗ" };
                var idx = (int)Math.Round((current.Wind.Deg % 360) / 45.0) % 8;
                WindDirection = $"Направление: {dirs[idx]}";

                Sunrise = $"Восход: {UnixToTime(current.Sys.Sunrise)}";
                Sunset = $"Закат: {UnixToTime(current.Sys.Sunset)}";

                OnPropertyChanged(nameof(City));
                OnPropertyChanged(nameof(Temp));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(FeelsLike));
                OnPropertyChanged(nameof(Humidity));
                OnPropertyChanged(nameof(Pressure));
                OnPropertyChanged(nameof(WindSpeed));
                OnPropertyChanged(nameof(WindDirection));
                OnPropertyChanged(nameof(Sunrise));
                OnPropertyChanged(nameof(Sunset));
            }

            // Прогнозы
            Hourly24 = await weatherService.GetHourly24ForecastAsync(city.Latitude, city.Longitude);
            Daily5 = await weatherService.GetDaily5ForecastAsync(city.Latitude, city.Longitude);

            OnPropertyChanged(nameof(Hourly24));
            OnPropertyChanged(nameof(Daily5));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private string UnixToTime(long unix)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime.ToString("HH:mm");
    }


}