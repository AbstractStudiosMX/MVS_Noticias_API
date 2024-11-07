using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Programming
{
    public class BroadcastInfo
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int Weekday { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }

        [JsonIgnore]
        public int ProgrammingId { get; set; }
        [JsonIgnore]
        public Programming Programming { get; set; }
    }
}
