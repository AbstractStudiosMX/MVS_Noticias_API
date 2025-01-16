using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Settings
{
    public class NotificationsSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Tendencias { get; set; } = true;
        public bool Entrevistas { get; set; } = true;
        public bool Deportes { get; set; } = true;
        public bool Nacional { get; set; } = true;
        public bool Videos { get; set; } = true;
        public bool CDMX { get; set; } = true;
        public bool Entretenimiento { get; set; } = true;
        public bool Opinion { get; set; } = true;
        public bool Economia { get; set; } = true;
        public bool Estados { get; set; } = true;
        public bool Mundo { get; set; } = true;
        public bool Mascotas { get; set; } = true;
        public bool SaludBienestar { get; set; } = true;
        public bool Policiaca { get; set; } = true;
        public bool Programacion { get; set; } = true;
        public bool CienciaTecnologia { get; set; } = true;
        public bool Viral { get; set; } = true;
        public bool Guardados { get; set; } = true;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Keywords { get; set; } = string.Empty;
        [JsonIgnore]
        public User User { get; set; }
    }
}
