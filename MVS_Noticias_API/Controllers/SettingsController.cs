using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DTO.Settings;
using MVS_Noticias_API.Models.Domain;
using MVS_Noticias_API.Models.Settings;

namespace MVS_Noticias_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public SettingsController(IConfiguration configuration, DataContext dataContext, ILogger<AuthenticationController> logger)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _logger = logger;
        }

        #region Accessibility Settings Endpoints

        [HttpGet("accesibility_settings")]
        public async Task<ActionResult<AccessibilitySettings>> GetAccesibilitySettingsByUser(string userEmail)
        {
            _logger.LogInformation("Get accesibility settings by user.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var accesibilitySettings = await _dataContext.AccessibilitySettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (accesibilitySettings == null)
                {
                    return NotFound("Accesibility settings not found.");
                }

                return Ok(accesibilitySettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting accesibility settings: " + ex.Message);
                return BadRequest("Error getting accesibility settings.");
            }
        }

        [HttpPost("accesibility_settings")]
        public async Task<ActionResult<AccessibilitySettings>> RegisterAccessibilitySettings(AccessibilitySettingsDto accessibilitySettingsDto)
        {
            _logger.LogInformation("Register accessibility settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == accessibilitySettingsDto.UserEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var accessibilitySettings = await _dataContext.AccessibilitySettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (accessibilitySettings != null)
                {
                    return BadRequest("Accessibility settings already exists.");
                }

                accessibilitySettings = new AccessibilitySettings
                {
                    UserId = user.Id,
                    FontSize = accessibilitySettingsDto.FontSize,
                    ApareanceMode = accessibilitySettingsDto.ApareanceMode
                };

                await _dataContext.AccessibilitySettings.AddAsync(accessibilitySettings);
                await _dataContext.SaveChangesAsync();

                return Ok(accessibilitySettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting accessibility settings: " + ex.Message);
                return BadRequest("Error getting accessibility settings.");
            }
        }

        [HttpPut("accesibility_settings")]
        public async Task<ActionResult<AccessibilitySettings>> UpdateAccessibilitySettings(AccessibilitySettingsDto accessibilitySettingsDto)
        {
            _logger.LogInformation("Update accessibility settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == accessibilitySettingsDto.UserEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var accessibilitySettings = await _dataContext.AccessibilitySettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (accessibilitySettings == null)
                {
                    return NotFound("Accessibility settings not found.");
                }

                accessibilitySettings.FontSize = accessibilitySettingsDto.FontSize;
                accessibilitySettings.ApareanceMode = accessibilitySettingsDto.ApareanceMode;

                await _dataContext.SaveChangesAsync();

                return Ok(accessibilitySettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating accessibility settings: " + ex.Message);
                return BadRequest("Error updating accessibility settings.");
            }
        }

        [HttpDelete("accesibility_settings")]
        public async Task<ActionResult<AccessibilitySettings>> DeleteAccessibilitySettings(string userEmail)
        {
            _logger.LogInformation("Delete accessibility settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var accessibilitySettings = await _dataContext.AccessibilitySettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (accessibilitySettings == null)
                {
                    return NotFound("Accessibility settings not found.");
                }

                _dataContext.AccessibilitySettings.Remove(accessibilitySettings);
                await _dataContext.SaveChangesAsync();

                return Ok("Accessibility settings deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting accessibility settings: " + ex.Message);
                return BadRequest("Error deleting accessibility settings.");
            }
        }

        #endregion

        #region Custom Settings Endpoints

        [HttpGet("custom_settings")]
        public async Task<ActionResult<CustomSettings>> GetCustomSettingsByUser(string userEmail)
        {
            _logger.LogInformation("Custom settings news by user.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var customSettings = await _dataContext.CustomSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (customSettings == null)
                {
                    var settings = new CustomSettings
                    {
                        NacionalOrder = 0,
                        CDMXOrder = 1,
                        EstadosOrder = 2,
                        PoliciacaOrder = 3,
                        NuevoLeonOrder = 4,
                        MundoOrder = 5,
                        PodcastOrder = 6,
                        EconomiaOrder = 7,
                        EntretenimientoOrder = 8,
                        TendenciasOrder = 9,
                        ViralOrder = 10,
                        SaludBienestarOrder = 11,
                        CienciaTecnologiaOrder = 12,
                        MascotasOrder = 13,
                        OpinionOrder = 14,
                        EntrevistasOrder = 15,
                        VideosOrder = 16,
                        MVSDeportesOrder = 17,
                        ProgramacionOrder = 18,
                        MasLeidasOrder = 19,
                        isDefaultOrder = true,
                    };
                    return Ok(settings);
                }

                return Ok(customSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting custom settings: " + ex.Message);
                return BadRequest("Error getting custom settings.");
            }
        }

        [HttpPost("custom_settings")]
        public async Task<ActionResult<CustomSettings>> RegisterCustomSettings(CustomSettingsDto customSettingsDto)
        {
            _logger.LogInformation("Register custom settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == customSettingsDto.UserEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var customSettings = await _dataContext.CustomSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (customSettings != null)
                {
                    return BadRequest("Custom settings already exists.");
                }

                customSettings = new CustomSettings
                {
                    UserId = user.Id,
                    NacionalOrder = customSettingsDto.NacionalOrder,
                    CDMXOrder = customSettingsDto.CDMXOrder,
                    EstadosOrder = customSettingsDto.EstadosOrder,
                    PoliciacaOrder = customSettingsDto.PoliciacaOrder,
                    NuevoLeonOrder = customSettingsDto.NuevoLeonOrder,
                    MundoOrder = customSettingsDto.MundoOrder,
                    PodcastOrder = customSettingsDto.PodcastOrder,
                    EconomiaOrder = customSettingsDto.EconomiaOrder,
                    EntretenimientoOrder = customSettingsDto.EntretenimientoOrder,
                    TendenciasOrder = customSettingsDto.TendenciasOrder,
                    ViralOrder = customSettingsDto.ViralOrder,
                    SaludBienestarOrder = customSettingsDto.SaludBienestarOrder,
                    CienciaTecnologiaOrder = customSettingsDto.CienciaTecnologiaOrder,
                    MascotasOrder = customSettingsDto.MascotasOrder,
                    OpinionOrder = customSettingsDto.OpinionOrder,
                    EntrevistasOrder = customSettingsDto.EntrevistasOrder,
                    VideosOrder = customSettingsDto.VideosOrder,
                    MVSDeportesOrder = customSettingsDto.MVSDeportesOrder,
                    ProgramacionOrder = customSettingsDto.ProgramacionOrder,
                    isDefaultOrder = true
                };

                await _dataContext.CustomSettings.AddAsync(customSettings);
                await _dataContext.SaveChangesAsync();

                return Ok(customSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting custom settings: " + ex.Message);
                return BadRequest("Error getting custom settings." + ex.Message);
            }
        }

        [HttpPut("custom_settings")]
        public async Task<ActionResult<CustomSettings>> UpdateCustomSettings(CustomSettingsDto customSettingsDto)
        {
            _logger.LogInformation("Update custom settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == customSettingsDto.UserEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var customSettings = await _dataContext.CustomSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (customSettings == null)
                {
                    return NotFound("Custom settings not found.");
                }

                customSettings.NacionalOrder = customSettingsDto.NacionalOrder;
                customSettings.CDMXOrder = customSettingsDto.CDMXOrder;
                customSettings.EstadosOrder = customSettingsDto.EstadosOrder;
                customSettings.ProgramacionOrder = customSettingsDto.ProgramacionOrder;
                customSettings.NuevoLeonOrder = customSettingsDto.NuevoLeonOrder;
                customSettings.MundoOrder = customSettingsDto.MundoOrder;
                customSettings.PodcastOrder = customSettingsDto.PodcastOrder;
                customSettings.EconomiaOrder = customSettingsDto.EconomiaOrder;
                customSettings.EntretenimientoOrder = customSettingsDto.EntretenimientoOrder;
                customSettings.TendenciasOrder = customSettingsDto.TendenciasOrder;
                customSettings.VideosOrder = customSettingsDto.VideosOrder;
                customSettings.SaludBienestarOrder = customSettingsDto.SaludBienestarOrder;
                customSettings.CienciaTecnologiaOrder = customSettingsDto.CienciaTecnologiaOrder;
                customSettings.MascotasOrder = customSettingsDto.MascotasOrder;
                customSettings.OpinionOrder = customSettingsDto.OpinionOrder;
                customSettings.EntrevistasOrder = customSettingsDto.EntrevistasOrder;
                customSettings.VideosOrder = customSettingsDto.VideosOrder;
                customSettings.MVSDeportesOrder = customSettingsDto.MVSDeportesOrder;
                customSettings.ProgramacionOrder = customSettingsDto.ProgramacionOrder;
                customSettings.MasLeidasOrder = customSettingsDto.MasLeidasOrder;
                customSettings.isDefaultOrder = false;

                await _dataContext.SaveChangesAsync();

                return Ok(customSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating custom settings: " + ex.Message);
                return BadRequest("Error updating custom settings.");
            }
        }

        [HttpDelete("custom_settings")]
        public async Task<ActionResult<CustomSettings>> DeleteCustomSettings(string userEmail)
        {
            _logger.LogInformation("Delete custom settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                var customSettings = await _dataContext.CustomSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (customSettings == null)
                {
                    return NotFound("Custom settings not found.");
                }

                _dataContext.CustomSettings.Remove(customSettings);
                await _dataContext.SaveChangesAsync();

                return Ok("Custom settings deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting custom settings: " + ex.Message);
                return BadRequest("Error deleting custom settings.");
            }
        }

        #endregion

        #region Notification Settings Endpoints

        [HttpGet("notification_settings")]
        public async Task<ActionResult<NotificationsSettings>> GetNotificationSettingsByUser(string userEmail)
        {
            _logger.LogInformation("Get notification settings by user.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var notificationSettings = await _dataContext.NotificationsSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (notificationSettings == null)
                {
                    return NotFound("Notification settings not found.");
                }

                return Ok(notificationSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting notification settings: " + ex.Message);
                return BadRequest("Error getting notification settings.");
            }
        }

        [HttpPost("notification_settings")]
        public async Task<ActionResult<NotificationsSettings>> RegisterNotificationSettings(NotificationSettingsDto notificationsSettingsDto)
        {
            _logger.LogInformation("Register notification settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == notificationsSettingsDto.UserEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var notificationSettings = await _dataContext.NotificationsSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (notificationSettings != null)
                {
                    return BadRequest("Notification settings already exists.");
                }

                notificationSettings = new NotificationsSettings
                {
                    UserId = user.Id,
                    Tendencias = notificationsSettingsDto.Tendencias,
                    Entrevistas = notificationsSettingsDto.Entrevistas,
                    Deportes = notificationsSettingsDto.MVSDeportes,
                    Nacional = notificationsSettingsDto.Nacional,
                    Videos = notificationsSettingsDto.Videos,
                    CDMX = notificationsSettingsDto.CDMX,
                    Entretenimiento = notificationsSettingsDto.Entretenimiento,
                    Opinion = notificationsSettingsDto.Opinion,
                    Economia = notificationsSettingsDto.Economia,
                    Estados = notificationsSettingsDto.Estados,
                    Mundo = notificationsSettingsDto.Mundo,
                    Mascotas = notificationsSettingsDto.Mascotas,
                    SaludBienestar = notificationsSettingsDto.SaludBienestar,
                    Policiaca = notificationsSettingsDto.Policiaca,
                    Programacion = notificationsSettingsDto.Programacion,
                    CienciaTecnologia = notificationsSettingsDto.CienciaTecnologia,
                    Viral = notificationsSettingsDto.Viral,
                    StartTime = notificationsSettingsDto.StartTime,
                    EndTime = notificationsSettingsDto.EndTime,
                    Keywords = notificationsSettingsDto.Keywords
                };

                await _dataContext.NotificationsSettings.AddAsync(notificationSettings);
                await _dataContext.SaveChangesAsync();

                return Ok(notificationSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error registering notification settings: " + ex.Message);
                return BadRequest("Error registering notification settings.");
            }
        }

        [HttpPut("notification_settings")]
        public async Task<ActionResult<NotificationsSettings>> UpdateNotificationSettings(NotificationSettingsDto notificationsSettingsDto)
        {
            _logger.LogInformation("Update notification settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == notificationsSettingsDto.UserEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var notificationSettings = await _dataContext.NotificationsSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (notificationSettings == null)
                {
                    return NotFound("Notification settings not found.");
                }

                notificationSettings.Tendencias = notificationsSettingsDto.Tendencias;
                notificationSettings.Entrevistas = notificationsSettingsDto.Entrevistas;
                notificationSettings.Deportes = notificationsSettingsDto.MVSDeportes;
                notificationSettings.Nacional = notificationsSettingsDto.Nacional;
                notificationSettings.Videos = notificationsSettingsDto.Videos;
                notificationSettings.CDMX = notificationsSettingsDto.CDMX;
                notificationSettings.Entretenimiento = notificationsSettingsDto.Entretenimiento;
                notificationSettings.Opinion = notificationsSettingsDto.Opinion;
                notificationSettings.Economia = notificationsSettingsDto.Economia;
                notificationSettings.Estados = notificationsSettingsDto.Estados;
                notificationSettings.Mundo = notificationsSettingsDto.Mundo;
                notificationSettings.Mascotas = notificationsSettingsDto.Mascotas;
                notificationSettings.SaludBienestar = notificationsSettingsDto.SaludBienestar;
                notificationSettings.Policiaca = notificationsSettingsDto.Policiaca;
                notificationSettings.Programacion = notificationsSettingsDto.Programacion;
                notificationSettings.CienciaTecnologia = notificationsSettingsDto.CienciaTecnologia;
                notificationSettings.Viral = notificationsSettingsDto.Viral;
                notificationSettings.StartTime = notificationsSettingsDto.StartTime;
                notificationSettings.EndTime = notificationsSettingsDto.EndTime;
                notificationSettings.Keywords = notificationsSettingsDto.Keywords;

                await _dataContext.SaveChangesAsync();

                return Ok(notificationSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating notification settings: " + ex.Message);
                return BadRequest("Error updating notification settings.");
            }
        }

        [HttpDelete("notification_settings")]
        public async Task<ActionResult<NotificationsSettings>> DeleteNotificationSettings(string userEmail)
        {
            _logger.LogInformation("Delete notification settings.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var notificationSettings = await _dataContext.NotificationsSettings.FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (notificationSettings == null)
                {
                    return NotFound("Notification settings not found.");
                }

                _dataContext.NotificationsSettings.Remove(notificationSettings);
                await _dataContext.SaveChangesAsync();

                return Ok("Notification settings deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting notification settings: " + ex.Message);
                return BadRequest("Error deleting notification settings.");
            }
        }
        #endregion
      
        #region User data Endpoint
        [HttpGet("user_data")]
        public async Task<ActionResult<User>> GetUserData(string userEmail)
        {
            _logger.LogInformation("Get user data proccess.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user data: " + ex.Message);
                return BadRequest("Error getting user data: " + ex.Message);
            }
        }

        [HttpPut("user_data")]
        public async Task<ActionResult<User>> UpadateUserData(DataUserDto userData)
        {
            _logger.LogInformation("Put user data proccess.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userData.email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                user.FullName = userData.FullName;
                user.Username = userData.Username;
                user.PhoneNumber = userData.PhoneNumber;
                user.BirthDate = userData.BirthDate;
                user.Gender = userData.Gender;
                user.City = userData.City;
                user.ImageUrl = userData.ImageUrl;

                await _dataContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error putting user data: " + ex.Message);
                return BadRequest("Error putting user data: " + ex.Message);
            }
        }
        #endregion
    }
}
