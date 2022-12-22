using MeyerCorp.HateoasBuilder;
using Microsoft.AspNetCore.Mvc;

namespace WebSample.Controllers
{
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

        [HttpGet("all")]
        public object GetAll()
        {
            return new
            {
                Links = HttpContext.AddFormattedLink("self", "WeatherForecast").Build(),
                Results = Enumerable.Range(1, 6).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                    Links = HttpContext.AddFormattedLink("self", "WeatherForecast/{0}", index).Build(),
                })
                .ToArray()
            };
        }

        [HttpGet]
        public object GetPage([FromQuery] int page = 0)
        {
            // Hardcode our total and page size and calculate the page amount
            int pagecount = 6 / 2;

            return new
            {
                Links = HttpContext
                    .AddFormattedLink("self", $"WeatherForecast?page={page}")
                    .AddFormattedLinkIf(page - 1 > -1, "previous", "WeatherForecast?page={0}", page - 1)
                    .AddFormattedLinkIf(page < pagecount, "next", "WeatherForecast?page={0}", page + 1)
                    .Build(),
                Results = Enumerable
                    .Range(1, 6)
                    .Skip(page)
                    .Take(2)
                    .Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                        Links = HttpContext.AddFormattedLink("self", "WeatherForecast/{0}", index).Build(),
                    })
                .ToArray()
            };
        }

        [HttpGet("{id}")]
        public WeatherForecast Get(int id)
        {
            return new WeatherForecast
            {
                Date = DateTime.Now.AddDays(id),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[id],
                Links = HttpContext
                    .AddFormattedLink("self", "WeatherForecast/{0}", id)
                    .AddFormattedLink("detail", "WeatherForecast/{0}/detail/{1}?param={2}", id, "x0x0x0", "value")
                    .AddLinkExternal("http://meyer.com", "external", String.Empty)
                    .AddLinkExternal("http://meyer.com", "externaldetail", "products/accent-nonstick-frypan")
                    .Build(),
            };
        }
    }
}