using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.News;
using MVS_Noticias_API.Models.Podcast;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;

namespace MVS_Noticias_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PodcastController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public PodcastController(IConfiguration configuration, ILogger<PodcastController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


        [HttpGet("allPrograms")]
        public async Task<ActionResult<List<ProgramPodcast>>> GetAllPrograms(int amount , int page)
        {
            _logger.LogInformation("Starting get all programs podcast.");

            try
            {
                var httpClient = new HttpClient();

                var apiOmnistudio = _configuration.GetSection("AppSettings:OmnistudioApi").Value;
                var keyOmnistudio = _configuration.GetSection("AppSettings:OmnistudioKey").Value;

                var responseOmnistudio = await httpClient.GetStringAsync(string.Format("{0}network/{1}/{2}/{3}", apiOmnistudio, keyOmnistudio, amount, page));
                var podcastData = JsonConvert.DeserializeObject<dynamic>(responseOmnistudio);

                var programList = new List<ProgramPodcast>();
                int indexCounter = 0;

                foreach (var program in podcastData)
                {
                    var programInfo = new ProgramPodcast
                    {
                        Index = indexCounter++,
                        Id = program.Id,
                        NetworkId = program.NetworkId,
                        Name = program.Name,
                        Slug = program.Slug,
                        HasImage = program.HasImage,
                        ImagePublicUrl = program.ImagePublicUrl,
                        Description = program.Description,
                    };
                    programList.Add(programInfo);
                }

                return Ok(programList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting podcast programs: " + ex.Message);
                return BadRequest("Error getting podcast programs: " + ex.Message);
            }
        }

        [HttpGet("programAudios")]
        public async Task<ActionResult<List<AudioPodcast>>> GetProgramAudios(string programId, int amount, int page)
        {
            _logger.LogInformation("Starting get all program audios.");

            try
            {
                var httpClient = new HttpClient();

                var apiOmnistudio = _configuration.GetSection("AppSettings:OmnistudioApi").Value;

                var responseOmnistudio = await httpClient.GetStringAsync(string.Format("{0}{1}/{2}/{3}", apiOmnistudio, programId, amount, page));
                var podcastData = JsonConvert.DeserializeObject<dynamic>(responseOmnistudio);

                var programList = new List<AudioPodcast>();
                int indexCounter = 0;

                foreach (var program in podcastData)
                {
                    var programInfo = new AudioPodcast
                    {
                        Index = indexCounter++,
                        Title = program.title,
                        Description = program.Description,
                        DurationSeconds = program.DurationSeconds,
                        PublishedDurationSeconds = program.PublishedDurationSeconds,
                        ImagePublicUrl = program.ImagePublicUrl,
                        AudioPublicUrl = program.AudioPublicUrl,
                        AudioPublicAdFreeUrl = program.AudioPublicAdFreeUrl,
                    };
                    programList.Add(programInfo);
                }

                return Ok(programList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting podcast audios: " + ex.Message);
                return BadRequest("Error getting podcast audios: " + ex.Message);
            }
        }
    }
}
