using Course.Models;
using Course.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Course.Views;

public partial class MainPage : ContentPage, INotifyPropertyChanged
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

    private ObservableCollection<ForecastItem> _hourly24 = new();
    public ObservableCollection<ForecastItem> Hourly24
    {
        get => _hourly24;
        set
        {
            _hourly24 = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<ForecastItem> _daily5 = new();
    public ObservableCollection<ForecastItem> Daily5
    {
        get => _daily5;
        set
        {
            _daily5 = value;
            OnPropertyChanged();
        }
    }

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this; // Важно!
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadWeather();
    }

    private async Task LoadWeather()
    {
        try
        {
            var cityJson = Preferences.Get("current_city", null);
            if (string.IsNullOrEmpty(cityJson))
            {
                return;
            }

            var city = JsonSerializer.Deserialize<CitySearchResponse>(cityJson);

            var current = await _weatherService.GetCurrentWeatherAsync(city.Latitude, city.Longitude);
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

            var hourlyList = await _weatherService.GetHourly24ForecastAsync(city.Latitude, city.Longitude);
            var dailyList = await _weatherService.GetDaily5ForecastAsync(city.Latitude, city.Longitude);

            Hourly24.Clear();
            foreach (var item in hourlyList)
                Hourly24.Add(item);

            Daily5.Clear();
            foreach (var item in dailyList)
                Daily5.Add(item);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить погоду: {ex.Message}", "OK");
        }
    }

    private string UnixToTime(long unix)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime.ToString("HH:mm");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void OnAddCityClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddCityPage());
    }
}