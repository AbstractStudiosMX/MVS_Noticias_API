using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Settings
{
    public class AccessibilitySettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FontSize { get; set; }
        public string ApareanceMode { get; set; } = string.Empty;
        [JsonIgnore]
        public User User { get; set; }
    }
}
