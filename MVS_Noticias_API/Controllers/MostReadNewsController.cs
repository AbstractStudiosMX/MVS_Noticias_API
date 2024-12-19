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
                var mvsWebUrl = _configuration.GetSection("AppSettings:MVSNoticiasWeb").Value;
                var newsIds = new List<int>();

                using var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(mvsWebUrl);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var sectionNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'contengo-mas-leidas-940')]");

                if (sectionNode != null)
                {
                    var linkNodes = sectionNode.SelectNodes(".//div[@class='item']/article/a");
                    foreach (var linkNode in linkNodes)
                    {
                        var href = linkNode.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrEmpty(href))
                        {
                            var idString = href.Split('-').LastOrDefault()?.Replace(".html", "");
                            if (int.TryParse(idString, out int id))
                            {
                                newsIds.Add(id);
                            }
                        }
                    }
                }

                var mostReadNews = new List<CompleteNews>();
                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;

                foreach (var id in newsIds)
                {

                    var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, id));
                    var newsData = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                    var news = new CompleteNews
                    {
                        IdNews = newsData.Noticias[0].id_noticia,
                        Title = newsData.Noticias[0].titulo,
                        Description = newsData.Noticias[0].descripcion,
                        Date = newsData.Noticias[0].fecha,
                        Section = newsData.Noticias[0].seccion,
                        SubSection = newsData.Noticias[0].subseccion,
                        IdSection = newsData.Noticias[0].id_seccion,
                        IdSubSection = newsData.Noticias[0].id_subseccion,
                        Url = newsData.Noticias[0].url,
                        Slug = newsData.Noticias[0].slug,
                        Photo = newsData.Noticias[0].foto,
                        PhotoMobile = newsData.Noticias[0].foto_movil,
                        PhotoCredits = newsData.Noticias[0].foto_creditos,
                        PhotoDescription = newsData.Noticias[0].foto_descripcion,
                        Author = newsData.Noticias[0].autor,
                        IdAuthor = newsData.Noticias[0].id_autor,
                        Creator = newsData.Noticias[0].creador,
                        IdCreator = newsData.Noticias[0].id_creador,
                        IsVideo = newsData.Noticias[0].isVideo,
                        VideoUrl = newsData.Noticias[0].videoUrl,
                        IsSound = newsData.Noticias[0].isSound,
                        SoundUrl = newsData.Noticias[0].SoundUrl,
                        Type = newsData.Noticias[0].Tipo,
                        Tags = newsData.Noticias[0].tags,
                        HiddenTags = newsData.Noticias[0].tags_ocultos,
                        NewsQuantity = newsData.Noticias[0].cantidad_noticias,
                        Number = newsData.Noticias[0].numero
                    };
                    mostReadNews.Add(news);
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

/*[HttpGet("mostReadNews")]
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
}*/
