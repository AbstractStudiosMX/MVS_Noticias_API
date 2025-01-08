using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DTO.Saved;
using MVS_Noticias_API.Models.News;
using MVS_Noticias_API.Models.Saved;
using MVS_Noticias_API.Services;
using Newtonsoft.Json;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LastNewsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public LastNewsController(DataContext dataContext, IConfiguration configuration, ILogger<LastNews> logger)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("lastTenNews")]
        public async Task<ActionResult<List<LastNews>>> GetLastTenNews()
        {
            _logger.LogInformation("Starting getting last news process");

            try
            {
                var lastNews = await _dataContext.LastNews.ToListAsync();

                if (lastNews == null)
                {
                    return NotFound("Last news not found");
                }

                lastNews.Reverse();

                return Ok(lastNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting last news: " + ex.Message);
                return BadRequest("Error getting last news: " + ex.Message);
            }
        }

        [HttpGet("simulateWebSocket")]
        public async Task<ActionResult<LastNews>> SimulateWebSocket()
        {
            _logger.LogInformation("Starting getting last news process");

            try
            {
                var lastNews = await _dataContext.LastNews.FirstOrDefaultAsync();

                if (lastNews == null)
                {
                    lastNews = new LastNews();
                }

                await WebSocketService.NotifyClientsAsync(lastNews);

                return Ok(lastNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error simulating WebSocket: " + ex.Message);
                return BadRequest("Error simulating WebSocket: " + ex.Message);
            }
        }
    }
}
