using Course.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Course.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "13c8283217070e7b9f6d5de5946cae0f";
    private const string BaseUrl = "http://api.openweathermap.org";

    public WeatherService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "WeatherApp-MAUI");
    }

    // Существующие методы
    public async Task<List<CitySearchResponse>> SearchCitiesAsync(string cityName)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/geo/1.0/direct?q={Uri.EscapeDataString(cityName)}&limit=5&appid={ApiKey}"
            );

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var cities = JsonSerializer.Deserialize<List<CitySearchResponse>>(json, options);
                return cities ?? new List<CitySearchResponse>();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка поиска: {ex.Message}");
        }

        return new List<CitySearchResponse>();
    }

    public async Task<CurrentWeatherResponse> GetCurrentWeatherAsync(double lat, double lon)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/data/2.5/weather?lat={lat}&lon={lon}&appid={ApiKey}&units=metric&lang=ru"
            );

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<CurrentWeatherResponse>(json, options);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка погоды: {ex.Message}");
        }

        return null;
    }

    // НОВЫЙ МЕТОД: Прогноз на 5 дней (с интервалом 3 часа, всего 40 записей)
    public async Task<ForecastResponse> Get5DayForecastAsync(double lat, double lon)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/data/2.5/forecast?lat={lat}&lon={lon}&appid={ApiKey}&units=metric&lang=ru&cnt=40"
            );

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ForecastResponse>(json, options);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка прогноза: {ex.Message}");
        }

        return null;
    }

    // НОВЫЙ МЕТОД: Прогноз на 24 часа (первые 8 записей)
    public async Task<List<ForecastItem>> Get24HourForecastAsync(double lat, double lon)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/data/2.5/forecast?lat={lat}&lon={lon}&appid={ApiKey}&units=metric&lang=ru&cnt=8"
            );

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var forecast = JsonSerializer.Deserialize<ForecastResponse>(json, options);
                return forecast?.List?.Take(8).ToList() ?? new List<ForecastItem>();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка прогноза 24ч: {ex.Message}");
        }

        return new List<ForecastItem>();
    }
}