using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Programming
{
    public class BroadcastInfo
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int Weekday { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }

        [JsonIgnore]
        public int ProgrammingId { get; set; }
        [JsonIgnore]
        public Programming? Programming { get; set; }
    }
}
