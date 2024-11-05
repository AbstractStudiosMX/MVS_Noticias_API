using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
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
        public async Task<ActionResult<WeatherForecast>> GetWeatherForecast(float? latitude, float? longuitude, string? city)
        {
            _logger.LogInformation("Starting weather forecast process.");

            try
            {
                var APIkeyOP = _configuration.GetSection("AppSettings:OpenWeatherApiKey").Value;
                var APIkeyWA = _configuration.GetSection("AppSettings:WeatherApiKey").Value;
                var httpClient = new HttpClient();
                string language = "es";

                dynamic weatherData = null;
                dynamic AirPollutionData = null;
                dynamic HourForecastData = null;
                dynamic DailyForecastData = null;

                if (latitude != null && longuitude != null && city == null) 
                {
                    var responseWeather = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&lang={3}", latitude, longuitude, APIkeyOP, language));
                    var responseAirPollution = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/air_pollution?lat={0}&lon={1}&appid={2}&lang={3}&lang={3}", latitude, longuitude, APIkeyOP, language));
                    var responseForecastDay = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&appid={2}&lang={3}", latitude, longuitude, APIkeyOP, language));
                    
                    weatherData = JsonConvert.DeserializeObject<dynamic>(responseWeather);
                    AirPollutionData = JsonConvert.DeserializeObject<dynamic>(responseAirPollution);
                    DailyForecastData = JsonConvert.DeserializeObject<dynamic>(responseForecastDay);

                    var responseForecastHour = await httpClient.GetStringAsync(string.Format("https://api.weatherapi.com/v1/forecast.json?key={0}&q={1}&lang={2}", APIkeyWA, weatherData.name, language));
                    HourForecastData = JsonConvert.DeserializeObject<dynamic>(responseForecastHour);

                }
                if (latitude == null && longuitude == null && city != null) 
                {
                    var responseForecastHour = await httpClient.GetStringAsync(string.Format("https://api.weatherapi.com/v1/forecast.json?key={0}&q={1}&lang={2}", APIkeyWA, city, language));
                    HourForecastData = JsonConvert.DeserializeObject<dynamic>(responseForecastHour);
                    
                    var responseWeather = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&appid={2}&lang={3}", HourForecastData.location.lat, HourForecastData.location.lon, APIkeyOP, language));
                    var responseAirPollution = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/air_pollution?lat={0}&lon={1}&appid={2}&lang={3}&lang={3}", HourForecastData.location.lat, HourForecastData.location.lon, APIkeyOP, language));
                    var responseForecastDay = await httpClient.GetStringAsync(string.Format("https://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&appid={2}&lang={3}", HourForecastData.location.lat, HourForecastData.location.lon, APIkeyOP, language));
                    
                    weatherData = JsonConvert.DeserializeObject<dynamic>(responseWeather);
                    AirPollutionData = JsonConvert.DeserializeObject<dynamic>(responseAirPollution);
                    DailyForecastData = JsonConvert.DeserializeObject<dynamic>(responseForecastDay);
                }

                long unixTime = weatherData.dt;
                int timezoneOffset = weatherData.timezone;
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime.AddSeconds(timezoneOffset);
                string formattedDateTime = dateTime.ToString("ddd hh:mm tt", new System.Globalization.CultureInfo("es-ES"));
                formattedDateTime = char.ToUpper(formattedDateTime[0]) + formattedDateTime.Substring(1);

                int aqi = AirPollutionData.list[0].main.aqi;
                string airQuality = GetAirQualityDescription(aqi);
                float windSpeedKmH = weatherData.wind.speed * 3.6;
                string formattedWindSpeed = $"{windSpeedKmH} km/h";

                string description = weatherData.weather[0].description;
                string formattedDescription = char.ToUpper(description[0]) + description.Substring(1).ToLower();
                string formattedIcon = weatherData.weather[0].icon;
                //formattedIcon = formattedIcon.Remove(2, 1);

                var forecast = new WeatherForecast
                {
                    Condition = formattedDescription,
                    ConditionIcon = formattedIcon,
                    CurrentTemperature = weatherData.main.temp - 273.15f,
                    RealFeelTemperature = weatherData.main.feels_like - 273.15f,
                    City = weatherData.name,
                    CurrentDateTime = formattedDateTime,
                    MaxTemperature = HourForecastData.forecast.forecastday[0].day.maxtemp_c,
                    MinTemperature = HourForecastData.forecast.forecastday[0].day.mintemp_c,
                    AirQuality = airQuality,
                    WindSpeed = formattedWindSpeed,
                    Humidity = HourForecastData.current.humidity,
                };

                foreach (var hour in HourForecastData.forecast.forecastday[0].hour)
                {
                    DateTime hourFullDate = hour.time;
                    string formattedHour = hourFullDate.ToString("hh:mm tt", new System.Globalization.CultureInfo("es-ES"));
                    string IconHour = hour.condition.icon;
                    string formattedIconHour = SetWeatherIconCode(IconHour,formattedHour);

                    var hourForecast = new HourlyForecast
                    {
                        Hour = formattedHour,
                        Condition = hour.condition.text,
                        ConditionIcon = formattedIconHour,
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
                    formattedDay = char.ToUpper(formattedDay[0]) + formattedDay.Substring(1);
                    string formattedDate = dayFullDate.ToString("M/d", new System.Globalization.CultureInfo("es-ES"));
                    string dayDescription = day.weather[0].description;
                    string formattedDayDescription = char.ToUpper(dayDescription[0]) + dayDescription.Substring(1).ToLower();
                    string formattedIconDay = day.weather[0].icon;
                    //formattedIconDay = formattedIconDay.Remove(2, 1);
                    var maxTemp = day.main.temp_max - 273.15f;
                    var minTemp = day.main.temp_min - 273.15f;

                    if (isFirstDay)
                    {
                        formattedDay = "Hoy";
                        isFirstDay = false;
                        maxTemp = HourForecastData.forecast.forecastday[0].day.maxtemp_c;
                        minTemp = HourForecastData.forecast.forecastday[0].day.mintemp_c;
                    }
                    if (!seenDays.Contains(formattedDate))
                    {
                        seenDays.Add(formattedDate);
                        var dayForecast = new DailyForecast
                        {
                            Day = formattedDay,
                            Date = formattedDate,
                            Condition = formattedDayDescription,
                            ConditionIcon = formattedIconDay,
                            MaxTemperature = maxTemp,
                            MinTemperature = minTemp,
                            PrecipitationChance = day.pop * 100f,
                        };
                        forecast.DailyForecasts.Add(dayForecast);
                    }

                }

                return Ok(forecast);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting weather forecast: " + ex.Message);
                return BadRequest("Error getting weather forecas.");
            }
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

        private string SetWeatherIconCode(string iconURL, string date)
        {
            int lastSlashIndex = iconURL.LastIndexOf('/') + 1;
            int dotPngIndex = iconURL.LastIndexOf('.');

            string resultURL = iconURL.Substring(lastSlashIndex, dotPngIndex - lastSlashIndex);

            DateTime dateFormatted = DateTime.ParseExact(date, "hh:mm tt", new System.Globalization.CultureInfo("es-ES"));

            string dayTime = "";

            if (dateFormatted.Hour >= 19 || dateFormatted.Hour < 6) 
            {
                dayTime = "n";
            }

            switch (resultURL)
            {
                case "113":
                    return "01" + dayTime;
                case "116":
                    return "02" + dayTime;
                case "119":
                    return "03" + dayTime;
                case "122":
                    return "04" + dayTime;
                case "296":
                case "302":
                case "308":
                    return "09" + dayTime;
                case "176":
                case "293":
                case "299":
                case "305":
                case "353":
                case "356":
                case "362":
                case "365":
                    return "10" + dayTime;
                case "200":
                case "359":
                case "386":
                case "389":
                case "392":
                    return "11" + dayTime;
                case "179":
                case "182":
                case "185":
                case "227":
                case "230":
                case "260":
                case "263":
                case "266":
                case "281":
                case "284":
                case "311":
                case "314":
                case "317":
                case "320":
                case "323":
                case "326":
                case "329":
                case "332":
                case "335":
                case "338":
                case "350":
                case "368":
                case "371":
                case "374":
                case "377":
                case "395":
                    return "13" + dayTime;
                case "143":
                case "248":
                    return "50" + dayTime;
                default:
                    return "Desconocido";
            }
        }
    }
}
