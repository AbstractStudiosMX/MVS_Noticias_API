using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.Interviews;
using MVS_Noticias_API.Models.News;
using Newtonsoft.Json;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InterviewsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public InterviewsController(IConfiguration configuration, ILogger<InterviewsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("interviews")]
        public async Task<ActionResult<List<Interviews>>> GetMostReadNews()
        {
            _logger.LogInformation("Starting interviews process.");

            try
            {
                var mvsWebUrl = _configuration.GetSection("AppSettings:MVSNoticiasWeb").Value;
                var newsIds = new List<int>();

                using var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(mvsWebUrl + "/entrevistas.html");

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var sectionNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'contengo-mas-leidas-940')]");

                var linkNodes = htmlDoc.DocumentNode.SelectNodes("//h2[@class='titulo']/a");
                if (linkNodes != null)
                {
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

                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;

                var authors = new List<(string Name, string Picture)>
                {
                    ("Juan Manuel Jiménez", "https://www.omnycontent.com/d/programs/a586ab2b-f2d1-4bc7-abd8-affd00c083a1/8e9461e1-de62-48c7-ae64-b1310002cc6b/image.jpg?t=1710202290&size=Medium"),
                    ("Luis Cárdenas", "https://www.omnycontent.com/d/programs/a586ab2b-f2d1-4bc7-abd8-affd00c083a1/6bdf152e-80c9-4923-bcbc-b00b0116f040/image.jpg?t=1707519338&size=Medium"),
                    ("Manuel López San Martín", "https://www.omnycontent.com/d/programs/a586ab2b-f2d1-4bc7-abd8-affd00c083a1/7b3e67db-4500-4f9c-81f2-b00b01189f54/image.jpg?t=1684861445&size=Medium"),
                    ("Ana Francisca Vega", "https://www.omnycontent.com/d/programs/a586ab2b-f2d1-4bc7-abd8-affd00c083a1/2235f17b-ec5e-4f93-aac3-b00b0119055d/image.jpg?t=1711566954&size=Medium"),
                    ("Pamela Cerdeira", "https://www.omnycontent.com/d/programs/a586ab2b-f2d1-4bc7-abd8-affd00c083a1/b0cadfd1-433b-40f0-b9c6-b00b011abbe1/image.jpg?t=1707519491&size=Medium")
                };

                var interviewsList = new List<Interviews>();

                for (int i = 0; i < authors.Count; i++)
                {
                    var author = authors[i];
                    var authorIds = newsIds.Skip(i * 4).Take(4).ToList();

                    var authorInterviews = new Interviews
                    {
                        Id = i,
                        AuthorName = author.Name,
                        AuthorPicture = author.Picture
                    };

                    foreach (var id in authorIds)
                    {
                        var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, id));
                        var newsData = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                        var newsInfo = new Interviews.NewsInfo
                        {
                            Header = newsData.Noticias[0].seccion + " " + author.Name,
                            Title = newsData.Noticias[0].titulo,
                            Description = newsData.Noticias[0].descripcion,
                            ImageUrl = newsData.Noticias[0].foto_movil
                        };

                        authorInterviews.News.Add(newsInfo);
                    }

                    interviewsList.Add(authorInterviews);
                }

                return Ok(interviewsList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting interviews: {ex.Message}");
                return BadRequest($"Error getting interviews: {ex.Message}");
            }
        }
    }
}
