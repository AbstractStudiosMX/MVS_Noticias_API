using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.News;
using Newtonsoft.Json;
using Ganss.Xss; // Para sanitización

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public NewsController(IConfiguration configuration, ILogger<NewsController> logger, DataContext dataContext)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet("detailNews")]
        public async Task<ActionResult<List<CompleteNews>>> GetDetailNews(int idNews)
        {
            _logger.LogInformation("Starting getting news formatted details.");

            try
            {
                using var httpClient = new HttpClient();

                // Obtener URL desde configuración
                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;
                var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, idNews));
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                var mostReadNews = new List<CompleteNews>();

                foreach (var news in newsData.Noticias)
                {

                    string content = news.contenido;
                    var sanitizedContent = SanitizeHtmlContent(content);

                    var mostRead = new CompleteNews
                    {
                        IdNews = news.id_noticia,
                        Title = news.titulo,
                        Description = news.descripcion,
                        Date = news.fecha,
                        Section = news.seccion,
                        SubSection = news.subseccion,
                        IdSection = news.id_seccion,
                        IdSubSection = news.id_subseccion,
                        Url = news.url,
                        Slug = news.slug,
                        Photo = news.foto,
                        PhotoMobile = news.foto_movil,
                        PhotoCredits = news.foto_creditos,
                        PhotoDescription = news.foto_descripcion,
                        Author = news.autor,
                        IdAuthor = news.id_autor,
                        Creator = news.creador,
                        IdCreator = news.id_creador,
                        Content = sanitizedContent,
                        IsVideo = news.isVideo,
                        VideoUrl = news.videoUrl,
                        IsSound = news.isSound,
                        SoundUrl = news.SoundUrl,
                        Type = news.Tipo,
                        Tags = "",
                        HiddenTags = "",
                        NewsQuantity = 0,
                        Number = news.numero,
                    };
                    mostReadNews.Add(mostRead);
                }

                return Ok(mostReadNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting saved news: " + ex.Message);
                return BadRequest("Error getting saved news: " + ex.Message);
            }
        }

        private string SanitizeHtmlContent(string htmlContent)
        {
            var sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("em");
            sanitizer.AllowedTags.Add("b");
            sanitizer.AllowedTags.Add("i");
            sanitizer.AllowedTags.Add("u");
            sanitizer.AllowedTags.Add("br");
            sanitizer.AllowedTags.Add("a");
            sanitizer.AllowedTags.Add("img");
            sanitizer.AllowedTags.Add("iframe");
            sanitizer.AllowedTags.Add("ul");
            sanitizer.AllowedTags.Add("li");
            sanitizer.AllowedTags.Add("div");
            sanitizer.AllowedTags.Add("aside");

            sanitizer.AllowedAttributes.Add("href"); 
            sanitizer.AllowedAttributes.Add("src"); 
            sanitizer.AllowedAttributes.Add("alt"); 
            sanitizer.AllowedAttributes.Add("title");
            sanitizer.AllowedAttributes.Add("width"); 
            sanitizer.AllowedAttributes.Add("height"); 
            sanitizer.AllowedAttributes.Add("allowfullscreen");

            return sanitizer.Sanitize(htmlContent);
        }
    }
}
