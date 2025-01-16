namespace MVS_Noticias_API.DTO.Settings
{
    public class NotificationSettingsDto
    {
        public string UserEmail { get; set; }
        public bool Tendencias { get; set; }
        public bool Entrevistas { get; set; }
        public bool MVSDeportes { get; set; }
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
        public bool Guardados { get; set; }
        public bool Viral { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Keywords { get; set; }
    }
}
