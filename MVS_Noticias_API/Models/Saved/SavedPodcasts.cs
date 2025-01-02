using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Saved
{
    public class SavedPodcasts
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float DurationSeconds { get; set; }
        public float PublishedDurationSeconds { get; set; }
        public string ImagePublicUrl { get; set; } = string.Empty;
        public string AudioPublicUrl { get; set; } = string.Empty;
        public string AudioPublicAdFreeUrl { get; set; } = string.Empty;

        [JsonIgnore]
        public User User { get; set; }
    }
}
