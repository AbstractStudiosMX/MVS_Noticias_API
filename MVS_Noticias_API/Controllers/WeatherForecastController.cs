using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DataService;
using MVS_Noticias_API.Models.Weather;
using Newtonsoft.Json;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public WeatherForecastController(IConfiguration configuration, ILogger<WeatherForecastController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("weatherforecast")]
        public async Task<ActionResult<WeatherForecast>> GetWeatherForecast(float latitude, float longuitude, string city)
        {
            _logger.LogInformation("Starting weather forecast process.");

            var APIkeyOP = _configuration.GetSection("AppSettings:OpenWeatherApiKey").Value;
            var APIkeyWA = _configuration.GetSection("AppSettings:WeatherApiKey").Value;
            var httpClient = new HttpClient();
            var responseWeather = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}", latitude, longuitude, APIkeyOP));
            var responseAirPollution = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/air_pollution?lat={0}&lon={1}&appid={2}", latitude, longuitude, APIkeyOP));
            var responseForecastDay = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&appid={2}", latitude, longuitude, APIkeyOP));
            var responseForecastHour = await httpClient.GetStringAsync(string.Format("https://api.weatherapi.com/v1/forecast.json?key={0}&q={1}", APIkeyWA, city));

            var weatherData = JsonConvert.DeserializeObject<dynamic>(responseWeather);
            var AirPollitionData = JsonConvert.DeserializeObject<dynamic>(responseAirPollution);
            var HourForecastData = JsonConvert.DeserializeObject<dynamic>(responseForecastHour);
            var DailyForecastData = JsonConvert.DeserializeObject<dynamic>(responseForecastDay);

            long unixTime = weatherData.dt;
            int timezoneOffset = weatherData.timezone;
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime.AddSeconds(timezoneOffset);
            string formattedDateTime = dateTime.ToString("ddd hh:mm tt", new System.Globalization.CultureInfo("es-ES"));

            int aqi = AirPollitionData.list[0].main.aqi;
            string airQuality = GetAirQualityDescription(aqi);
            float windSpeedKmH = weatherData.wind.speed * 3.6;
            string formattedWindSpeed = $"{windSpeedKmH} km/h";

            var forecast = new WeatherForecast
            {
                Condition = weatherData.weather[0].description,
                CurrentTemperature = weatherData.main.temp - 273.15f,
                RealFeelTemperature = weatherData.main.feels_like - 273.15f,
                City = weatherData.name,
                CurrentDateTime = formattedDateTime,
                MaxTemperature = weatherData.main.temp_max - 273.15f,
                MinTemperature = weatherData.main.temp_min - 273.15f,
                AirQuality = airQuality,
                WindSpeed = formattedWindSpeed,
                Humidity = weatherData.main.humidity,
            };

            foreach (var hour in HourForecastData.forecast.forecastday[0].hour)
            {
                DateTime hourFullDate = hour.time;
                string formattedHour = hourFullDate.ToString("hh:mm tt", new System.Globalization.CultureInfo("es-ES"));

                var hourForecast = new HourlyForecast
                {
                    Hour = formattedHour,
                    Condition = hour.condition.text,
                    Humidity = hour.humidity,
                    Temperature = hour.temp_c
                };
                forecast.HourlyForecasts.Add(hourForecast);
            }


            var dailyForecast = new List<DailyForecast>();
            bool isFirstDay = true;
            var seenDays = new HashSet<string>();

            foreach (var day in DailyForecastData.list)
            {
                DateTime dayFullDate = day.dt_txt;
                string formattedDay = dayFullDate.ToString("ddd", new System.Globalization.CultureInfo("es-ES"));
                string formattedDate = dayFullDate.ToString("M/d", new System.Globalization.CultureInfo("es-ES"));

                if (isFirstDay)
                {
                    formattedDay = "Hoy";
                    isFirstDay = false;
                }
                if (!seenDays.Contains(formattedDate))
                {
                    seenDays.Add(formattedDate);
                    var dayForecast = new DailyForecast
                    {
                        Day = formattedDay,
                        Date = formattedDate,
                        Condition = day.weather[0].description,
                        MaxTemperature = day.main.temp_max - 273.15f,
                        MinTemperature = day.main.temp_min - 273.15f,
                        PrecipitationChance = day.pop * 100f,
                    };
                    forecast.DailyForecasts.Add(dayForecast);
                }
                   
            }

            return Ok(forecast);
        }

        private string GetAirQualityDescription(int aqi) 
        {
            switch (aqi)
            {
                case 1:
                case 2:
                    return "Buena";
                case 3:
                    return "Moderado";
                case 4:
                    return "Pobre";
                case 5:
                    return "Insalubre";
                default:
                    return "Desconocido";
            }
        }
    }
}
