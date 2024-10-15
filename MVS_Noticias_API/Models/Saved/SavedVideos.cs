using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Saved
{
    public class SavedVideos
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ViewsNumber { get; set; }
        public string Photo { get; set; } = string.Empty;
        [JsonIgnore]
        public User User { get; set; }
    }
}
