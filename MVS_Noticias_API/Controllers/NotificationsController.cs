﻿using FirebaseAdmin.Messaging;
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
        public async Task<ActionResult<List<Notification>>> GetUserNotifications(string userEmail)
        {
            _logger.LogInformation("Starting getting user notifications proccess.");

            try
            {

                var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var notifications = await _dataContext.Notifications.Where(x => x.UserId == user.Id).ToListAsync();

                return Ok(notifications);
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

                return Ok("Updated notification");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error putting user notifications: " + ex.Message);
                return BadRequest("Error putting user notifications: " + ex.Message);
            }
        }

        [HttpDelete("notification")]
        public async Task<ActionResult<List<Notification>>> DeleteUserNotifications(int newsId, string userEmail)
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

                int limit = 5;//el limite tendria que servir para validar las otras noticias pasadas y ver si esta la nueva o no, creo que con 2 seria suficiente

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiOneSignal}");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var responseOneSignal = await httpClient.GetStringAsync
                    (string.Format($"https://onesignal.com/api/v1/notifications?limit={limit}&kind=1&app_id={appIdOneSignal}"));
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseOneSignal);

                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;
                var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, newsData.notifications[0].data.idnota));
                var newsDataDetail = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                var savedNews = new List<UserNotifications>();

                //var usersList = await _dataContext.Users.ToListAsync();//este sirve para mandarle a absolutamebte todos los usuario las notificaciones

                var notificationSettings = await _dataContext.NotificationsSettings.ToListAsync();

                foreach (var notification in newsDataDetail.Noticias)
                {
                    string seccion = notification.seccion;
                    string subseccion = notification.subseccion;
                    string targetSection = !string.IsNullOrEmpty(subseccion) ? subseccion : seccion;

                    // Map section to enum
                    if (!Enum.TryParse(targetSection.Replace(" ", ""), true, out NotificationSections sectionEnum))
                    {
                        _logger.LogWarning($"Section '{targetSection}' not found in enum.");
                        continue;
                    }

                    // Filtrar usuarios con preferencias activas
                    var targetUsers = notificationSettings
                        .Where(ns =>
                        {
                            var property = ns.GetType().GetProperty(sectionEnum.ToString());
                            if (property == null) return false; // Si la propiedad no existe, no se incluye
                            var value = property.GetValue(ns) as bool?;
                            return value == true; // Solo incluye configuraciones activas
                        })
                        .ToList();

                    foreach (var targetUser in targetUsers)
                    {
                        var savedNew = new UserNotifications
                        {

                            UserId = targetUser.UserId,
                            NewsId = notification.id_noticia,
                            Title = notification.titulo,
                            Content = notification.descripcion,
                            Section = subseccion != ""
                                        ? subseccion
                                        : seccion,
                            RegisterDate = notification.fecha

                        };
                        savedNews.Add(savedNew);
                    }
                }

                // Lista de usuarios - revisar preferencias - [usuarios sin registro en la tabla preferencias (todos)] - [usuarios con preferencias]

                // Automatizar el chequeo de la notificacion (CRON) solo con el ID de la noticia

                // Revisar si ya tiene la notificación

                // Mandarsela a todos


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
                }

                return Ok(savedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user notifications: " + ex.Message);
                return BadRequest("Error getting user notifications: " + ex.Message);
            }
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
