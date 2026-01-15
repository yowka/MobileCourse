using System.Text.Json.Serialization;

namespace Course.Models;

public class CurrentWeatherResponse
{
    [JsonPropertyName("main")]
    public MainData? Main { get; set; }

    [JsonPropertyName("weather")]
    public List<WeatherInfo>? Weather { get; set; }

    [JsonPropertyName("wind")]
    public WindData? Wind { get; set; }

    [JsonPropertyName("dt")]
    public long Timestamp { get; set; }

    [JsonPropertyName("name")]
    public string? CityName { get; set; }

    // ВОТ ЭТА СТРОКА ДОЛЖНА БЫТЬ:
    [JsonPropertyName("sys")]
    public SysData? Sys { get; set; }
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }
}

public class WeatherInfo
{
    [JsonPropertyName("main")]
    public string? Main { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Direction { get; set; }
}

// И ЭТОТ КЛАСС ТОЖЕ ДОЛЖЕН БЫТЬ:
public class SysData
{
    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}