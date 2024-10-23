namespace MVS_Noticias_API.Models.Weather
{
    public class WeatherForecast
    {
        public string Condition { get; set; } = string.Empty;
        public string ConditionIcon { get; set; } = string.Empty;
        public int CurrentTemperature { get; set; }
        public int RealFeelTemperature { get; set; }
        public string City { get; set; } = string.Empty;
        public string CurrentDateTime { get; set; } = string.Empty;
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public string AirQuality { get; set; } = string.Empty;
        public string WindSpeed {  get; set; } = string.Empty;
        public int Humidity { get; set; }
        public List<HourlyForecast> HourlyForecasts { get; set; } = new List<HourlyForecast>();
        public List<DailyForecast> DailyForecasts { get; set; } = new List<DailyForecast> ();
    }

    public class HourlyForecast
    {
        public string Hour { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string ConditionIcon { get; set; } = string.Empty;
        public string Humidity { get; set; } = string.Empty;
        public int Temperature { get; set; }
    }
    
    public class DailyForecast
    { 
        public string Day { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string ConditionIcon { get; set; } = string.Empty;
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public float PrecipitationChance { get;  set; }
    }
}
