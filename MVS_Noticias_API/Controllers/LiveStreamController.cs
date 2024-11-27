using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVS_Noticias_API.Models.Livestream;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LiveStreamController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;


        public LiveStreamController(ILogger<ProgrammingController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        [HttpGet("streamUrl")]
        public async Task<ActionResult<List<StreamVideo>>> GetStreamUrl()
        {
            var streamUrls = new List<string>
            {
                "https://ts.mvsnoticias.com/720p.m3u8",
                "https://s5.mexside.net:1936/ejemplo/ejemplo/playlist.m3u8"
            };

            var availableStreams = new List<StreamVideo>();

            foreach (var url in streamUrls)
            {
                if (await IsUrlAvailable(url))
                {
                    availableStreams.Add(new StreamVideo { LivestreamUrl = url });

                    return Ok(availableStreams);
                }
                else
                {
                    _logger.LogWarning($"Stream URL not available: {url}");
                }
            }

            return NotFound("No streams are currently available.");
        }

        private async Task<bool> IsUrlAvailable(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                return response.IsSuccessStatusCode;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"Timeout while checking URL {url}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while checking URL {url}: {ex.Message}");
                return false;
            }
        }
    }
}
