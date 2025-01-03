using System.Text.Json.Serialization;

namespace MVS_Noticias_API.Models.News
{
    public class LastNews
    {
        public int Id { get; set; }
        [JsonPropertyName("id_noticia")]
        public int IdNews { get; set; }

        [JsonPropertyName("titulo")]
        public string Title { get; set; }

        [JsonPropertyName("descripcion")]
        public string Description { get; set; }

        [JsonPropertyName("fecha")]
        public string Date { get; set; }

        [JsonPropertyName("seccion")]
        public string Section { get; set; }

        [JsonPropertyName("subseccion")]
        public string SubSection { get; set; }

        [JsonPropertyName("id_seccion")]
        public int IdSection { get; set; }

        [JsonPropertyName("id_subseccion")]
        public int IdSubSection { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("foto")]
        public string Photo { get; set; }

        [JsonPropertyName("foto_movil")]
        public string PhotoMobile { get; set; }

        [JsonPropertyName("foto_creditos")]
        public string PhotoCredits { get; set; }

        [JsonPropertyName("foto_descripcion")]
        public string PhotoDescription { get; set; }

        [JsonPropertyName("autor")]
        public string Author { get; set; }

        [JsonPropertyName("id_autor")]
        public int IdAuthor { get; set; }

        [JsonPropertyName("creador")]
        public string Creator { get; set; }

        [JsonPropertyName("id_creador")]
        public int IdCreator { get; set; }

        [JsonPropertyName("isVideo")]
        public bool IsVideo { get; set; }

        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; }

        [JsonPropertyName("isSound")]
        public bool IsSound { get; set; }

        [JsonPropertyName("SoundUrl")]
        public string SoundUrl { get; set; }

        [JsonPropertyName("Tipo")]
        public string Type { get; set; }

        [JsonPropertyName("numero")]
        public int Number { get; set; }
    }
}
