using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static string apiKey = "0ec53e1eb998c64df6b63a279e226a7a";
    static string apiUrl = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric&lang=ru";

    static async Task Main()
    {
        Console.Write("Введите города через запятую: ");
        var cities = Console.ReadLine()?.Split(",").Select(c => c.Trim()).ToList();
        if (cities == null || cities.Count == 0) return;

        var tasks = cities.Select(GetWeatherAsync);
        var results = await Task.WhenAll(tasks);

        Console.WriteLine("\nРезультаты:");
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        await SaveResultsAsync(results);
    }

    static async Task<string> GetWeatherAsync(string city)
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(string.Format(apiUrl, city, apiKey));
            if (!response.IsSuccessStatusCode)
                return $"Ошибка получения данных для {city}: {response.StatusCode}";

            var responseBody = await response.Content.ReadAsStringAsync();
            var weather = JsonSerializer.Deserialize<WeatherResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return $"{city}: {weather.Main.Temp}°C, {weather.Main.Humidity}% влажность, {weather.Weather[0].Description}";

        }
        catch
        {
            return $"Ошибка получения данных для {city}";
        }
    }

    static async Task SaveResultsAsync(IEnumerable<string> results)
    {
        var json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync("weather_results.json", json);
    }
}

class WeatherResponse
{
    public List<WeatherInfo> Weather { get; set; }
    public MainInfo Main { get; set; }
}

class WeatherInfo
{
    public string Description { get; set; }
}

class MainInfo
{
    public float Temp { get; set; }
    public int Humidity { get; set; }
}
