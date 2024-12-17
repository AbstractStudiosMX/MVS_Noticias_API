using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.News;
using MVS_Noticias_API.Models.Statistics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml;
using static System.Collections.Specialized.BitVector32;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MostReadNewsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public MostReadNewsController(IConfiguration configuration, ILogger<MostReadNewsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("mostReadNews")]
        public async Task<ActionResult<List<CompleteNews>>> GetMostReadNews()
        {
            _logger.LogInformation("Starting most read news process.");

            try
            {

                using var httpClient = new HttpClient();

                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;
                var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}dinamico.asp?id_modulo=1", apiEditor80));
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                var mostReadNews = new List<CompleteNews>();
                
                foreach (var news in newsData.Noticias)
                {
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
                        IsVideo = news.isVideo,
                        VideoUrl = news.videoUrl,
                        IsSound = news.isSound,
                        SoundUrl = news.SoundUrl,
                        Type = news.Tipo,
                        Tags = "",
                        HiddenTags = "",
                        NewsQuantity = 0,
                        Number = news.numero
                    };
                    mostReadNews.Add(mostRead);
                }

                return Ok(mostReadNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting most read news: " + ex.Message);
                return BadRequest("Error getting most read news: " + ex.Message);
            }
        }
    }
}
