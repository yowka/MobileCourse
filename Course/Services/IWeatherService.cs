using Course.Models;

namespace Course.Services
{
    public interface IWeatherService
    {
        Task<CurrentWeatherResponse> GetCurrentWeatherAsync(double lat, double lon);
        Task<List<CitySearchResponse>> SearchCitiesAsync(string query);
        Task<ForecastResponse> GetForecastAsync(double lat, double lon);
    }
}