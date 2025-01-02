using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DTO.Saved;
using MVS_Noticias_API.Models.Saved;
using Newtonsoft.Json;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SavedController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public SavedController(IConfiguration configuration, ILogger<SavedController> logger, DataContext dataContext)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet("savedNews")]
        public async Task<ActionResult<List<SavedNews>>> GetSavedNews(string userEmail)
        {
            _logger.LogInformation("Starting getting save news proccess.");

            try
            {

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var savedNews = await _dataContext.SavedNews.Where(x => x.UserId == user.Id).ToListAsync();

                savedNews.Reverse();

                return Ok(savedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting saved news: " + ex.Message);
                return BadRequest("Error getting saved news: " + ex.Message);
            }
        }

        [HttpPost("savedNews")]
        public async Task<ActionResult<List<SavedNews>>> PostSavedNews(SavedNewsDto request)
        {
            _logger.LogInformation("Starting post save news proccess.");

            try
            {
                using var httpClient = new HttpClient();
                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;

                var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, request.IdNews));
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == request.userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var savedNews = new SavedNews 
                {
                    UserId = user.Id,
                    IdNews = request.IdNews,
                    Title = newsData.Noticias[0].titulo,
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
                    Content = newsData.Noticias[0].contenido,
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

                await _dataContext.SavedNews.AddAsync(savedNews);
                await _dataContext.SaveChangesAsync();

                return Ok(savedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error posting saved news: " + ex.Message);
                return BadRequest("Error posting saved news: " + ex.Message);
            }
        }

        [HttpDelete("savedNews")]
        public async Task<ActionResult<List<SavedNews>>> DeleteSavedNews(int newsId, string userEmail)
        {
            _logger.LogInformation("Starting delete save news proccess.");

            try
            {

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var savedNews = await _dataContext.SavedNews.FirstOrDefaultAsync(x => x.IdNews == newsId);

                if (savedNews == null) 
                {
                    return NotFound("saved News not found.");
                }
               

                _dataContext.SavedNews.Remove(savedNews);
                await _dataContext.SaveChangesAsync();

                return Ok("Saved news deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting saved news: " + ex.Message);
                return BadRequest("Error deleting saved news: " + ex.Message);
            }
        }

        [HttpGet("savedPodcast")]
        public async Task<ActionResult<List<SavedPodcasts>>> GetSavedPodcasts(string userEmail)
        {
            _logger.LogInformation("Starting saved podcast proccess.");

            try
            {

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var savedNews = await _dataContext.savedPodcasts.Where(x => x.UserId == user.Id).ToListAsync();

                savedNews.Reverse();

                return Ok(savedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting saved podcasts: " + ex.Message);
                return BadRequest("Error getting saved podcasts: " + ex.Message);
            }
        }

    }
}
