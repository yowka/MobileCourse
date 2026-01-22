using System.Text.Json.Serialization;

namespace Course.Models
{
    public class CurrentWeatherResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("main")]
        public CurrentWeatherMain Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<CurrentWeatherInfo> Weather { get; set; } = new();

        [JsonPropertyName("wind")]
        public CurrentWeatherWind Wind { get; set; } = new();

        [JsonPropertyName("clouds")]
        public CurrentWeatherClouds Clouds { get; set; } = new();

        [JsonPropertyName("sys")]
        public CurrentWeatherSys Sys { get; set; } = new();
    }

    public class CurrentWeatherMain
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double Feels_like { get; set; }

        [JsonPropertyName("temp_min")]
        public double Temp_min { get; set; }

        [JsonPropertyName("temp_max")]
        public double Temp_max { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    public class CurrentWeatherInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("main")]
        public string Main { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = "";
    }

    public class CurrentWeatherWind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }

        [JsonPropertyName("gust")]
        public double Gust { get; set; }
    }

    public class CurrentWeatherClouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }

    public class CurrentWeatherSys
    {
        [JsonPropertyName("country")]
        public string Country { get; set; } = "";

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }
    }
}