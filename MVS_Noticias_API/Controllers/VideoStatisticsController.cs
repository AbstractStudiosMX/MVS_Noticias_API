using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.Statistics;
using MVS_Noticias_API.Models.Weather;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoStatisticsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public VideoStatisticsController(IConfiguration configuration, ILogger<VideoStatisticsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("videoStatistics")]
        public async Task<ActionResult<VideoStatistics>> GetVideoStatistics(int seccion, int limite, int pagina)
        {
            _logger.LogInformation("Starting video statistics process.");

            try
            {
                var APIkeyYoutube = _configuration.GetSection("AppSettings:YoutubeKey").Value;
                var httpClient = new HttpClient();
               
                var responseNewsMVS = await httpClient.GetStringAsync(string.Format("https://mvsnoticias.com/a/api/noticias.asp?id_seccion={0}&contenido=si&limite={1}&pagina={2}", seccion, limite, pagina));
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                var formattedNews = new List<VideoStatistics>();


                foreach (var news in newsData.Noticias)
                {
                    string ExtractYouTubeId(string text)
                    {
                        var youtubeMatch = Regex.Match(text, @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
                        return youtubeMatch.Success ? youtubeMatch.Groups[1].Value : null;
                    }

                    string videoId = "";
                    bool hasVideo = false;
                    string formattedVideoUrl = "";

                    if (news.videoUrl != "") 
                    {
                        videoId = ExtractYouTubeId(news.videoUrl);
                        formattedVideoUrl = news.videoUrl;
                        hasVideo = true;
                    }

                    if (news.contenido != "" && news.videoUrl == "")
                    {
                        videoId = ExtractYouTubeId(news.contenido);

                        if (videoId != "" && videoId != null)
                        {
                            formattedVideoUrl = "https://www.youtube.com/watch?v=" + videoId;
                            hasVideo = true;
                        }
                        else
                        {
                            hasVideo = false;
                        }
                    }

                    if (hasVideo) 
                    {
                        var responseYoutube = await httpClient.GetStringAsync(string.Format("https://www.googleapis.com/youtube/v3/videos?part=statistics&part=contentDetails&id={0}&key={1}", videoId, APIkeyYoutube));
                        var youtubeData = JsonConvert.DeserializeObject<dynamic>(responseYoutube);
                        string videoDuration = youtubeData.items[0].contentDetails.duration;
                        TimeSpan duration = XmlConvert.ToTimeSpan(videoDuration);

                        string formattedDuration = duration.ToString(@"h\:mm\:ss");

                        var newsTemp = new VideoStatistics
                        {
                            IdNews = news.id_noticia,
                            VideoURL = formattedVideoUrl,
                            ViewsNumber = youtubeData.items[0].statistics.viewCount,
                            VideoDuration = formattedDuration,
                            ImageURL = news.foto_movil
                        };
                        formattedNews.Add(newsTemp);
                    }
                    if (!hasVideo)
                    {
                        
                        var newsTemp = new VideoStatistics
                        {
                            IdNews = news.id_noticia,
                            VideoURL = "",
                            ViewsNumber = 0,
                            VideoDuration = "",
                            ImageURL = news.foto_movil,
                        };
                        formattedNews.Add(newsTemp);
                    }
                }

                return Ok(formattedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting video statistics: " + ex.Message);
                return BadRequest("Error getting video statistics.");
            }
        }
    }
}
