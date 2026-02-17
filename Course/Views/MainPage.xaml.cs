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
    private const string HOURLY_CACHE_KEY = "hourly_cache";
    private const string DAILY_CACHE_KEY = "daily_cache";
    private const string CACHE_TIMESTAMP_KEY = "cache_timestamp";

    public string City { get; set; } = "Ч";
    public string Temp { get; set; } = "Ч";
    public string Description { get; set; } = "Ч";
    public string FeelsLike { get; set; } = "Ч";
    public string Humidity { get; set; } = "Ч";
    public string Pressure { get; set; } = "Ч";
    public string WindSpeed { get; set; } = "Ч";
    public string WindDirection { get; set; } = "Ч";
    public string Sunrise { get; set; } = "Ч";
    public string Sunset { get; set; } = "Ч";

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
        BindingContext = this;
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
                ClearAllData();
                return;
            }

            var city = JsonSerializer.Deserialize<CitySearchResponse>(cityJson);

            LoadCachedForecasts();

            await RefreshWeatherData(city);
        }
        catch (Exception ex)
        {
            await DisplayAlert("ќшибка", $"Ќе удалось загрузить погоду: {ex.Message}", "OK");
        }
    }

    private void LoadCachedForecasts()
    {
        try
        {
            var lastCacheTime = Preferences.Get(CACHE_TIMESTAMP_KEY, 0L);
            var cacheAge = DateTime.Now - DateTime.FromBinary(lastCacheTime);

            if (cacheAge.TotalHours < 1) 
            {
                var hourlyJson = Preferences.Get(HOURLY_CACHE_KEY, null);
                if (!string.IsNullOrEmpty(hourlyJson))
                {
                    var hourly = JsonSerializer.Deserialize<List<ForecastItem>>(hourlyJson);
                    if (hourly != null)
                    {
                        Hourly24.Clear();
                        foreach (var item in hourly)
                            Hourly24.Add(item);
                    }
                }

                var dailyJson = Preferences.Get(DAILY_CACHE_KEY, null);
                if (!string.IsNullOrEmpty(dailyJson))
                {
                    var daily = JsonSerializer.Deserialize<List<ForecastItem>>(dailyJson);
                    if (daily != null)
                    {
                        Daily5.Clear();
                        foreach (var item in daily)
                            Daily5.Add(item);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ќшибка загрузки кэша: {ex.Message}");
        }
    }

    private async Task RefreshWeatherData(CitySearchResponse city)
    {
        try
        {
            var current = await _weatherService.GetCurrentWeatherAsync(city.Latitude, city.Longitude);
            if (current != null)
            {
                UpdateCurrentWeather(current);
            }

            var (hourlyList, dailyList) = await _weatherService.GetForecastsAsync(city.Latitude, city.Longitude);

            UpdateForecasts(hourlyList, dailyList);

            SaveForecastsToCache(hourlyList, dailyList);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ќшибка обновлени€ данных: {ex.Message}");
        }
    }

    private void UpdateCurrentWeather(CurrentWeatherResponse current)
    {
        City = current.Name;
        Temp = Math.Round(current.Main.Temp).ToString();
        Description = current.Weather.FirstOrDefault()?.Description ?? "";
        FeelsLike = $"{Math.Round(current.Main.Feels_like)}∞C";
        Humidity = $"{current.Main.Humidity}%";
        Pressure = $"{current.Main.Pressure} гѕа";
        WindSpeed = $"{current.Wind.Speed} м/с";

        var dirs = new[] { "—", "—¬", "¬", "ё¬", "ё", "ё«", "«", "—«" };
        var idx = (int)Math.Round((current.Wind.Deg % 360) / 45.0) % 8;
        WindDirection = dirs[idx];

        Sunrise = UnixToTime(current.Sys.Sunrise);
        Sunset = UnixToTime(current.Sys.Sunset);

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

    private void UpdateForecasts(List<ForecastItem> hourly, List<ForecastItem> daily)
    {
        Hourly24.Clear();
        foreach (var item in hourly)
            Hourly24.Add(item);

        Daily5.Clear();
        foreach (var item in daily)
            Daily5.Add(item);
    }

    private void SaveForecastsToCache(List<ForecastItem> hourly, List<ForecastItem> daily)
    {
        try
        {
            var hourlyJson = JsonSerializer.Serialize(hourly);
            var dailyJson = JsonSerializer.Serialize(daily);

            Preferences.Set(HOURLY_CACHE_KEY, hourlyJson);
            Preferences.Set(DAILY_CACHE_KEY, dailyJson);
            Preferences.Set(CACHE_TIMESTAMP_KEY, DateTime.Now.ToBinary());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ќшибка сохранени€ кэша: {ex.Message}");
        }
    }

    private void ClearAllData()
    {
        City = "Ч";
        Temp = "Ч";
        Description = "Ч";
        FeelsLike = "Ч";
        Humidity = "Ч";
        Pressure = "Ч";
        WindSpeed = "Ч";
        WindDirection = "Ч";
        Sunrise = "Ч";
        Sunset = "Ч";

        Hourly24.Clear();
        Daily5.Clear();

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