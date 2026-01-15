using Course.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Course.Services
{
    public interface IWeatherService
    {
        Task<List<CitySearchResponse>> SearchCitiesAsync(string cityName);
        Task<CurrentWeatherResponse> GetCurrentWeatherAsync(double lat, double lon);
        Task<ForecastResponse> Get5DayForecastAsync(double lat, double lon);
        Task<List<ForecastItem>> Get24HourForecastAsync(double lat, double lon);
    }
}