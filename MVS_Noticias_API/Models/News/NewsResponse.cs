namespace MVS_Noticias_API.Models.News
{
    public class NoticiasResponse
    {
        public OtrosDatos OtrosDatos { get; set; }
        public List<LastNews> Noticias { get; set; }
    }

    public class OtrosDatos
    {
        public string Titulo { get; set; }
        public string Volanta1 { get; set; }
        public string Link2 { get; set; }
        public string Opciones { get; set; }
    }
}
