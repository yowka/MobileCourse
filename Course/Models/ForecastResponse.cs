using System.Text.Json.Serialization;

namespace Course.Models
{
    public class ForecastResponse
    {
        [JsonPropertyName("cod")]
        public string? Cod { get; set; }

        [JsonPropertyName("message")]
        public int Message { get; set; }

        [JsonPropertyName("cnt")]
        public int Count { get; set; }

        [JsonPropertyName("list")]
        public List<ForecastItem>? List { get; set; }

        [JsonPropertyName("city")]
        public CityInfo? City { get; set; }
    }

    public class ForecastItem
    {
        [JsonPropertyName("dt")]
        public long Timestamp { get; set; }

        [JsonPropertyName("main")]
        public ForecastMain? Main { get; set; }

        [JsonPropertyName("weather")]
        public List<ForecastWeather>? Weather { get; set; }

        [JsonPropertyName("clouds")]
        public ForecastClouds? Clouds { get; set; }

        [JsonPropertyName("wind")]
        public ForecastWind? Wind { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("pop")]
        public double PrecipitationProbability { get; set; }

        [JsonPropertyName("rain")]
        public ForecastRain? Rain { get; set; }

        [JsonPropertyName("snow")]
        public ForecastSnow? Snow { get; set; }

        [JsonPropertyName("dt_txt")]
        public string? DateText { get; set; }

        // Свойство для удобства - преобразованная дата
        public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).LocalDateTime;
    }

    public class ForecastMain
    {
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }

        [JsonPropertyName("sea_level")]
        public int SeaLevel { get; set; }

        [JsonPropertyName("grnd_level")]
        public int GroundLevel { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("temp_kf")]
        public double TempKF { get; set; }
    }

    public class ForecastWeather
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("main")]
        public string? Main { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    public class ForecastClouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }

    public class ForecastWind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Direction { get; set; }

        [JsonPropertyName("gust")]
        public double Gust { get; set; }
    }

    public class ForecastRain
    {
        [JsonPropertyName("3h")]
        public double Volume3h { get; set; }
    }

    public class ForecastSnow
    {
        [JsonPropertyName("3h")]
        public double Volume3h { get; set; }
    }

    public class CityInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("coord")]
        public CityCoord? Coord { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("population")]
        public int Population { get; set; }

        [JsonPropertyName("timezone")]
        public int Timezone { get; set; }

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }
    }

    public class CityCoord
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}