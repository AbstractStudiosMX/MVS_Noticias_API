using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.News
{
    public class NoticiasResponse
    {
        [JsonPropertyName("otrosdatos")]
        public OtrosDatos OtrosDatos { get; set; }

        [JsonPropertyName("Noticias")]
        public List<LastNews> Noticias { get; set; }
    }

    public class OtrosDatos
    {
        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("Volanta_1")]
        public string Volanta1 { get; set; }

        [JsonPropertyName("Link_2")]
        public string Link2 { get; set; }

        [JsonPropertyName("Opciones")]
        public string Opciones { get; set; }
    }

}
