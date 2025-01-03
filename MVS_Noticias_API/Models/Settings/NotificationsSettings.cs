using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Settings
{
    public class NotificationsSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Tendencias { get; set; }
        public bool Entrevistas { get; set; }
        public bool Deportes { get; set; }
        public bool Nacional { get; set; }
        public bool Videos { get; set; }
        public bool CDMX { get; set; }
        public bool Entretenimiento { get; set; }
        public bool Opinion { get; set; }
        public bool Economia { get; set; }
        public bool Estados { get; set; }
        public bool Mundo { get; set; }
        public bool Mascotas { get; set; }
        public bool SaludBienestar { get; set; }
        public bool Policiaca { get; set; }
        public bool Programacion { get; set; }
        public bool CienciaTecnologia { get; set; }
        public bool Viral { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Keywords { get; set; } = string.Empty;
        [JsonIgnore]
        public User User { get; set; }
    }
}
