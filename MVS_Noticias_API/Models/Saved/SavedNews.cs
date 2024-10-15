using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Saved
{
    public class SavedNews
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Image {  get; set; } = string.Empty;
        public string ShareLink { get; set; } = string.Empty;
        [JsonIgnore]
        public User User { get; set; }
    }
}
