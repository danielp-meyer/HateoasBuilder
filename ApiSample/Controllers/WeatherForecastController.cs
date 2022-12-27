using MeyerCorp.HateoasBuilder;
using Microsoft.AspNetCore.Mvc;

namespace ApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public object Get()
    {
        var collection = Enumerable
            .Range(1, 5)
            .Select(index =>
            {
                var summary = Summaries[Random.Shared.Next(Summaries.Length)];

                return new
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summary,
                    Links = HttpContext
                       .AddRouteLink("item", "WeatherForecast", "item", "type", summary)
                       .Build()
                };
            })
            .ToArray();

        return new
        {
            Items = collection,
            Links = HttpContext
                .AddLink("self", "WeatherForecast")
                .AddQueryLink("previous", "WeatherForcast", "Page", 1)
                .AddQueryLink("next", "WeatherForecast", "Page", 3)
                .Build()
        };
    }
}
