using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.CallRecords;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Migrations;
using MVS_Noticias_API.Models.Saved;
using MVS_Noticias_API.Models.Settings;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;

namespace MVS_Noticias_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IServiceProvider _serviceProvider;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public NotificationsController(IConfiguration configuration, ILogger<NotificationsController> logger, DataContext dataContext, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet("allNotifications")]
        public async Task<ActionResult> GetUserNotifications( string userEmail, int pageNumber = 1, int pageSize = 10, bool? isRead = null)
        {
            _logger.LogInformation("Starting getting user notifications process.");

            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var query = _dataContext.Notifications.Where(x => x.UserId == user.Id);

                if (isRead.HasValue)
                {
                    query = query.Where(x => x.IsRead == isRead.Value);
                }

                var rawNotifications = await query
                    .OrderByDescending(x => x.RegisterDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Realiza la conversión en memoria
                var notifications = rawNotifications.Select(x => new
                {
                    x.Id,
                    x.UserId,
                    x.NewsId,
                    x.Title,
                    x.Content,
                    x.Section,
                    x.SectionId,
                    x.IsRead,
                    x.IsNew,
                    RegisterDate = ConvertToISO(x.RegisterDate)
                }).ToList();


                var response = new
                {
                    Notifications = notifications
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user notifications: " + ex.Message);
                return BadRequest("Error getting user notifications: " + ex.Message);
            }
        }

        [HttpGet("allNotificationsOneSignal")]
        public async Task<ActionResult> GetNotificationsOneSignal(int limit)
        {
            _logger.LogInformation("Starting getting one signal notifications process.");

            try
            {

                using var httpClient = new HttpClient();
                using var scope = _serviceProvider.CreateScope();
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var apiOneSignal = _configuration.GetSection("AppSettings:OneSignalApiKey").Value;
                var appIdOneSignal = _configuration.GetSection("AppSettings:OneSignalAppId").Value;
                

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiOneSignal}");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var responseOneSignal = await httpClient.GetStringAsync
                    (string.Format($"https://onesignal.com/api/v1/notifications?limit={limit}&kind=1&app_id={appIdOneSignal}"));
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseOneSignal);

                var onesignalNotifications = new List<UserNotifications>();

                foreach ( var notification in newsData.notifications)
                {
                    TimeZoneInfo mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                    long unixDate = notification.completed_at;
                    DateTime dateUtc = DateTimeOffset.FromUnixTimeSeconds(unixDate).UtcDateTime;
                    DateTime dateMexicoCity = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, mexicoCityTimeZone);
                    string formattedDate = dateMexicoCity.ToString("dd/MM/yyyy HH:mm:ss");

                    var News = new UserNotifications
                    {
                        UserId = 0,
                        NewsId = notification.data.idnota,
                        Title = notification.contents.en,
                        Content = "",
                        Section = "",
                        RegisterDate = formattedDate,
                        SectionId = ""
                    };
                    onesignalNotifications.Add(News);
                }

                return Ok(onesignalNotifications);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting one signal notifications: " + ex.Message);
                return BadRequest("Error getting one signal notifications: " + ex.Message);
            }
        }

        [HttpPut("notification")]
        public async Task<ActionResult<List<UserNotifications>>> PutUserNotifications(string userEmail, int newsId)
        {
            _logger.LogInformation("Starting putting user notifications proccess.");

            try
            {

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var notification = await _dataContext.Notifications.FirstOrDefaultAsync(x => x.NewsId == newsId && x.UserId == user.Id);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                notification.IsRead = true;
                notification.IsNew = true;

                await _dataContext.SaveChangesAsync();

                return Ok("Updated notification");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error putting user notifications: " + ex.Message);
                return BadRequest("Error putting user notifications: " + ex.Message);
            }
        }

        [HttpDelete("notification")]
        public async Task<ActionResult<List<UserNotifications>>> DeleteUserNotifications(int newsId, string userEmail)
        {
            _logger.LogInformation("Starting delete notification proccess.");

            try
            {

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var notification = await _dataContext.Notifications.FirstOrDefaultAsync(x => x.NewsId == newsId && x.UserId == user.Id);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }


                _dataContext.Notifications.Remove(notification);
                await _dataContext.SaveChangesAsync();

                return Ok("Notification deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting notification: " + ex.Message);
                return BadRequest("Error deleting notification: " + ex.Message);
            }
        }

        /*  [HttpGet("notificationsOne")]
          public async Task<ActionResult<List<Notification>>> GetUserNotifications()
          {
              _logger.LogInformation("Starting getting user notifications proccess.");

              try
              {
                  using var httpClient = new HttpClient();
                  using var scope = _serviceProvider.CreateScope();
                  var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                  var apiOneSignal = _configuration.GetSection("AppSettings:OneSignalApiKey").Value;
                  var appIdOneSignal = _configuration.GetSection("AppSettings:OneSignalAppId").Value;

                  int limit = 1;

                  httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiOneSignal}");
                  httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                  var responseOneSignal = await httpClient.GetStringAsync
                      (string.Format($"https://onesignal.com/api/v1/notifications?limit={limit}&kind=1&app_id={appIdOneSignal}"));
                  var newsData = JsonConvert.DeserializeObject<dynamic>(responseOneSignal);

                  int idNota = newsData.notifications[0].data.idnota;

                  TimeZoneInfo mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                  long unixDate = newsData.notifications[0].completed_at;
                  DateTime dateUtc = DateTimeOffset.FromUnixTimeSeconds(unixDate).UtcDateTime;
                  DateTime dateMexicoCity = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, mexicoCityTimeZone);
                  string formattedDate = dateMexicoCity.ToString("dd/MM/yyyy HH:mm:ss");

                  var lastNotificationSent = await dataContext.LastNotificationSent.FirstOrDefaultAsync();

                  // Si la noticia es la misma nos salimos del flujo
                  if (lastNotificationSent!.NewsId == idNota && lastNotificationSent.RegisterDate == formattedDate)
                  {
                      _logger.LogInformation($"Notification with idNota {idNota} has already been processed. Skipping.");
                     // return;
                  }

                  string originalTitle = newsData.notifications[0].contents.en;

                  return Ok();
              }
              catch (Exception ex)
              {
                  _logger.LogError("Error getting user notifications: " + ex.Message);
                  return BadRequest("Error getting user notifications: " + ex.Message);
              }

          public enum NotificationSections
          {
              Tendencias,
              Entrevistas,
              Deportes,
              Nacional,
              Videos,
              CDMX,
              Entretenimiento,
              Opinion,
              Economia,
              Estados,
              Mundo,
              Mascotas,
              SaludBienestar,
              Policiaca,
              Programacion,
              CienciaTecnologia,
              Viral
          }*/

        private string ConvertToISO(string date)
        {
            var parsedDate = DateTime.ParseExact(date, "d/M/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            return parsedDate.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
