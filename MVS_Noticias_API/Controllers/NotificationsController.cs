using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
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
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public NotificationsController(IConfiguration configuration, ILogger<NotificationsController> logger, DataContext dataContext)
        {
            _configuration = configuration;
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

        [HttpGet("notificationsOne")]
        public async Task<ActionResult<List<Notification>>> GetUserNotifications()
        {
            _logger.LogInformation("Starting getting user notifications proccess.");

            try
            {
                using var httpClient = new HttpClient();
                var apiOneSignal = _configuration.GetSection("AppSettings:OneSignalApiKey").Value;
                var appIdOneSignal = _configuration.GetSection("AppSettings:OneSignalAppId").Value;

                int limit = 1;

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiOneSignal}");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var responseOneSignal = await httpClient.GetStringAsync
                    (string.Format($"https://onesignal.com/api/v1/notifications?limit={limit}&kind=1&app_id={appIdOneSignal}"));
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseOneSignal);

                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;
                var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, newsData.notifications[0].data.idnota));
                var newsDataDetail = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                // Filtrar usuarios sin configuración en NotificationSettings
                var allUsers = await _dataContext.Users.Select(ns => ns.Id).ToListAsync();
                var usersWithSettings = await _dataContext.NotificationsSettings.Where(ns => allUsers.Contains(ns.UserId)).ToListAsync();
                var usersWithoutSettings = allUsers.Where(userId => !usersWithSettings.Any(ns => ns.UserId == userId)).ToList();

                var savedNews = new List<UserNotifications>();

                foreach (var notification in newsDataDetail.Noticias)
                {
                    string seccion = notification.seccion;
                    string subseccion = notification.subseccion;
                    string targetSection = !string.IsNullOrEmpty(subseccion) ? subseccion : seccion;
                    string idSubseccion = notification.id_subseccion;
                    string idSeccion = notification.id_seccion;

                    // Map section to enum
                    if (!Enum.TryParse(targetSection.Replace(" ", ""), true, out NotificationSections sectionEnum))
                    {
                        _logger.LogWarning($"Section '{targetSection}' not found in enum.");
                        continue;
                    }

                    foreach (var userId in usersWithoutSettings)
                    {
                        var savedNew = new UserNotifications
                        {
                            UserId = userId,
                            NewsId = notification.id_noticia,
                            Title = notification.titulo,
                            Content = notification.descripcion,
                            Section = subseccion != ""
                                        ? subseccion
                                        : seccion,
                            RegisterDate = notification.fecha,
                            SectionId = idSubseccion != ""
                                        ? idSubseccion
                                        : idSeccion
                        };
                        savedNews.Add(savedNew);
                    }

                    // Filtrar usuarios con preferencias activas
                    var targetUsersWithSettings = usersWithSettings
                        .Where(ns =>
                        {
                            var property = ns.GetType().GetProperty(sectionEnum.ToString());
                            if (property == null) return false; // Si la propiedad no existe, no se incluye
                            var value = property.GetValue(ns) as bool?;
                            return value == true; // Solo incluye configuraciones activas
                        })
                        .ToList();

                    // Lista de usuarios que no tienen preferencias de notificaciones
                    foreach (var user in targetUsersWithSettings)
                    {
                        var savedNew = new UserNotifications
                        {
                            UserId = user.UserId,
                            NewsId = notification.id_noticia,
                            Title = notification.titulo,
                            Content = notification.descripcion,
                            Section = subseccion != ""
                                        ? subseccion
                                        : seccion,
                            RegisterDate = notification.fecha,
                            SectionId = idSubseccion != "0"
                                        ? idSubseccion
                                        : idSeccion
                        };
                        savedNews.Add(savedNew);
                    }
                }

                // Inserciones con validación de duplicados y transacción
                const int batchSize = 100; // Tamaño del lote

                if (savedNews.Any())
                {
                    var uniqueNews = new List<UserNotifications>();

                    foreach (var news in savedNews)
                    {
                        bool exists = await _dataContext.Notifications
                            .AnyAsync(n => n.UserId == news.UserId && n.NewsId == news.NewsId);

                        if (!exists)
                        {
                            uniqueNews.Add(news);
                        }
                    }

                    var executionStrategy = _dataContext.Database.CreateExecutionStrategy();

                    await executionStrategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _dataContext.Database.BeginTransactionAsync();
                        try
                        {
                            // Inserciones en lotes
                            for (int i = 0; i < uniqueNews.Count; i += batchSize)
                            {
                                var batch = uniqueNews.Skip(i).Take(batchSize).ToList();
                                await _dataContext.Notifications.AddRangeAsync(batch);
                                await _dataContext.SaveChangesAsync();
                            }

                            await transaction.CommitAsync();
                            _logger.LogInformation($"Saved {uniqueNews.Count} unique notifications to the database.");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError("Error while saving notifications: " + ex.Message);
                            throw;
                        }
                    });
                }

                return Ok(savedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user notifications: " + ex.Message);
                return BadRequest("Error getting user notifications: " + ex.Message);
            }
        }

        private string ConvertToISO(string date)
        {
            var parsedDate = DateTime.ParseExact(date, "d/M/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            return parsedDate.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
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
        }
    }
}
