using Course.Models;
using System.Text.Json;

namespace Course.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "f554b93c8b7cb90e4bf0a8e5a346dd2b";
        private const string BaseUrl = "https://api.openweathermap.org";
        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<CurrentWeatherResponse> GetCurrentWeatherAsync(double lat, double lon)
        {
            try
            {
                var url = $"{BaseUrl}/data/2.5/weather?lat={lat}&lon={lon}&appid={ApiKey}&units=metric&lang=ru";
                Console.WriteLine($"Запрос погоды: {url}");

                var response = await _httpClient.GetStringAsync(url);
                Console.WriteLine($"Ответ API: {response}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<CurrentWeatherResponse>(response, options);
                Console.WriteLine($"Десериализовано: Name={result?.Name}, Temp={result?.Main?.Temp}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetCurrentWeatherAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CitySearchResponse>> SearchCitiesAsync(string query)
        {
            try
            {
                var url = $"{BaseUrl}/geo/1.0/direct?q={query}&limit=10&appid={ApiKey}";
                var response = await _httpClient.GetStringAsync(url);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<CitySearchResponse>>(response, options) ?? new List<CitySearchResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в SearchCitiesAsync: {ex.Message}");
                return new List<CitySearchResponse>();
            }
        }

        public async Task<ForecastResponse> GetForecastAsync(double lat, double lon)
        {
            try
            {
                var url = $"{BaseUrl}/data/2.5/forecast?lat={lat}&lon={lon}&appid={ApiKey}&units=metric&lang=ru";
                var response = await _httpClient.GetStringAsync(url);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ForecastResponse>(response, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetForecastAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ForecastItem>> GetHourly24ForecastAsync(double lat, double lon)
        {
            var forecast = await GetForecastAsync(lat, lon);
            var list = forecast?.List;
            if (list == null || !list.Any())
                return new List<ForecastItem>();

            return list.Take(8).ToList();
        }

        public async Task<List<ForecastItem>> GetDaily5ForecastAsync(double lat, double lon)
        {
            var forecast = await GetForecastAsync(lat, lon);
            var list = forecast?.List;
            if (list == null || !list.Any())
                return new List<ForecastItem>();

            var today = DateTime.UtcNow.Date;
            return list
                .Where(item => DateTimeOffset.FromUnixTimeSeconds(item.Timestamp).UtcDateTime.Date != today)
                .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds(item.Timestamp).UtcDateTime.Date)
                .Take(5)
                .Select(g => g.First())
                .ToList();
        }
    }
}