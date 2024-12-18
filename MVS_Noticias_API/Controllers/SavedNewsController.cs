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
    public class SavedNewsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public SavedNewsController(IConfiguration configuration, ILogger<SavedNewsController> logger, DataContext dataContext)
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

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == request.userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var savedNews = new SavedNews 
                {
                    UserId = user.Id,
                    IdNews = request.IdNews,
                    Title = request.Title,
                    Description = request.Description,
                    Date = request.Date,
                    Section = request.Section,
                    SubSection = request.SubSection,
                    IdSection = request.IdSection,
                    IdSubSection = request.IdSubSection,
                    Url = request.Url,
                    Slug = request.Slug,
                    Photo = request.Photo,
                    PhotoMobile = request.PhotoMobile,
                    PhotoDescription = request.PhotoDescription,
                    Author = request.Author,
                    IdAuthor = request.IdAuthor,
                    Creator = request.Creator,
                    IdCreator = request.IdCreator,
                    Content = request.Content,
                    IsVideo = request.IsVideo,
                    VideoUrl = request.VideoUrl,
                    IsSound = request.IsSound,
                    SoundUrl = request.SoundUrl,
                    Type = request.Type,
                    Tags = request.Tags,
                    HiddenTags = request.HiddenTags,
                    NewsQuantity = request.NewsQuantity,
                    Number = request.Number,
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

                var savedNews = await _dataContext.SavedNews.FirstOrDefaultAsync(x => x.Id == newsId);

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

    }
}
