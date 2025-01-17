using MVS_Noticias_API.Models.Domain;
using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.Settings
{
    public class CustomSettings
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public int NacionalOrder { get; set; }
        public int CDMXOrder { get; set; }
        public int EstadosOrder { get; set; }
        public int PoliciacaOrder { get; set; }
        public int NuevoLeonOrder { get; set; }
        public int MundoOrder { get; set; }
        public int PodcastOrder { get; set; }
        public int EconomiaOrder { get; set; }
        public int EntretenimientoOrder { get; set; }
        public int TendenciasOrder { get; set; }
        public int ViralOrder { get; set; }
        public int SaludBienestarOrder { get; set; }
        public int CienciaTecnologiaOrder { get; set; }
        public int MascotasOrder { get; set; }
        public int OpinionOrder { get; set; }
        public int EntrevistasOrder { get; set; }
        public int VideosOrder { get; set; }
        public int MVSDeportesOrder { get; set; }
        public int ProgramacionOrder { get; set; }
        public int MasLeidasOrder { get; set; }
        public int GuardadosOrder { get; set; }
        public int MultimediaOrder { get; set; }
        public int UltimaHoraOrder {  get; set; }
        public bool? isDefaultOrder { get; set; } = false;

        [JsonIgnore]
        public User User { get; set; }
    }
}
