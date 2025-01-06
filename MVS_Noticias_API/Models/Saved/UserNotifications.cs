using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Saved
{
    public class UserNotifications
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Section {  get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public bool IsNew { get; set; } = false ;
        public string RegisterDate { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;

        [JsonIgnore]
        public User User { get; set; }

    }
}
