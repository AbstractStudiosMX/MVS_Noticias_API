using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DataService;
using MVS_Noticias_API.DTO.Programming;
using MVS_Noticias_API.DTO.Settings;
using MVS_Noticias_API.Models.Programming;
using MVS_Noticias_API.Models.Settings;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProgrammingController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public ProgrammingController(IConfiguration configuration, DataContext dataContext, ILogger<ProgrammingController> logger)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet("allPrograms")]
        public async Task<ActionResult<AccessibilitySettings>> GetAllPrograms()
        {
            _logger.LogInformation("Started getting all programs proccess.");

            try
            {
                var programs = await _dataContext.Programs.Include(p => p.BroadcastDates).ToListAsync();
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting programming: " + ex.Message);
                return BadRequest("Error getting programming.");
            }
        }

        [HttpPost("program")]
        public async Task<ActionResult<Programming>> PostProgramming(ProgrammingDto request)
        {
            _logger.LogInformation("Starting post programming process.");

            try
            {
                Programming program = new Programming();

                program.Title = request.Title;
                program.BroadcastHour = request.BroadcastHour;
                program.BroadcastDay = request.BroadcastDay;
                program.UrlImage = request.UrlImage;
                program.UrlPersonalSite = request.UrlPersonalSite;

                foreach (var date in request.BroadcastDates)
                {
                    program.BroadcastDates.Add(date);
                }

                _dataContext.Programs.Add(program);
                await _dataContext.SaveChangesAsync();
                _logger.LogInformation("Program registered successfully.");
                return Ok(request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has ocurred.");
                return BadRequest("Could not register program.");
            }
        }

        [HttpPut("program")]
        public async Task<ActionResult<Programming>> PutProgramming(ProgrammingDto request)
        {
            _logger.LogInformation("Starting put programming process.");

            try
            {
                var program = await _dataContext.Programs.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (program == null)
                {
                    return NotFound("Program not found.");
                }

                program.Title = request.Title;
                program.BroadcastHour = request.BroadcastHour;
                program.BroadcastDay = request.BroadcastDay;
                program.UrlImage = request.UrlImage;
                program.UrlPersonalSite = request.UrlPersonalSite;

                for (int i = 0; i < request.BroadcastDates.Count; i++) 
                {
                    program.BroadcastDates[i] = request.BroadcastDates[i];
                }

                await _dataContext.SaveChangesAsync();
                _logger.LogInformation("Program updated successfully.");
                return Ok(program);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has ocurred.");
                return BadRequest("Could not update program.");
            }
        }

        [HttpDelete("program")]
        public async Task<ActionResult<Programming>> DeleteProgramming(int programId)
        {
            _logger.LogInformation("Starting delete programming process.");

            try
            {
                var program = await _dataContext.Programs.FirstOrDefaultAsync(x => x.Id == programId);

                if (program == null)
                {
                    return NotFound("Program not found.");
                }

                _dataContext.Programs.Remove(program);
                await _dataContext.SaveChangesAsync();

                return Ok("Program successfully deleted");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has ocurred.");
                return BadRequest("Could not delete program.");
            }
        }
    }
}
