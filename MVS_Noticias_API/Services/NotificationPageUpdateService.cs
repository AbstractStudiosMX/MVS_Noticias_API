﻿using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.Saved;
using Newtonsoft.Json;

namespace MVS_Noticias_API.Services
{
    public class NotificationPageUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NewsUpdateService> _logger;
        public readonly IConfiguration _configuration;

        public NotificationPageUpdateService(IServiceProvider serviceProvider, ILogger<NewsUpdateService> logger, IConfiguration configuration) 
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SetUserNotifications();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during news update: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        public async Task SetUserNotifications()
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

                int idNote = newsData.notifications[0].data.idnota;
                string originalTitle = newsData.notifications[0].contents.en;

                TimeZoneInfo mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                long unixDate = newsData.notifications[0].completed_at;
                DateTime dateUtc = DateTimeOffset.FromUnixTimeSeconds(unixDate).UtcDateTime;
                DateTime dateMexicoCity = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, mexicoCityTimeZone);
                string formattedDate = dateMexicoCity.ToString("dd/MM/yyyy HH:mm:ss");

                var lastNotificationSent = await dataContext.LastNotificationSent.FirstOrDefaultAsync();

                if(lastNotificationSent != null)
                {
                    //Solo si la noticia es diferente a la guardada en la bd se hace el proces
                    if (lastNotificationSent.Title != originalTitle || lastNotificationSent.RegisterDate != formattedDate || lastNotificationSent.NewsId != idNote)
                    {
                
                        var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;
                        var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, idNote));
                        var newsDataDetail = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);
                        
                        if (newsDataDetail != null)
                        {
                            // Filtrar usuarios sin configuración en NotificationSettings
                            var allUsers = await dataContext.Users.Select(ns => ns.Id).ToListAsync();
                            var usersWithSettings = await dataContext.NotificationsSettings.Where(ns => allUsers.Contains(ns.UserId)).ToListAsync();
                            var usersWithoutSettings = allUsers.Where(userId => !usersWithSettings.Any(ns => ns.UserId == userId)).ToList();

                            var savedNews = new List<UserNotifications>();

                            string seccion = newsDataDetail.Noticias[0].seccion;
                            string subseccion = newsDataDetail.Noticias[0].subseccion;
                            string targetSection = !string.IsNullOrEmpty(subseccion) ? subseccion : seccion;
                            string idSubseccion = newsDataDetail.Noticias[0].id_subseccion;
                            string idSeccion = newsDataDetail.Noticias[0].id_seccion;

                            // Map section to enum
                            if (!Enum.TryParse(targetSection.Replace(" ", ""), true, out NotificationSections sectionEnum))
                            {
                                _logger.LogWarning($"Section '{targetSection}' not found in enum.");
                            }

                            foreach (var userId in usersWithoutSettings)
                            {
                                var savedNew = new UserNotifications
                                {
                                    UserId = userId,
                                    NewsId = newsDataDetail.Noticias[0].id_noticia,
                                    Title = originalTitle,
                                    Content = newsDataDetail.Noticias[0].titulo,
                                    Section = subseccion != ""
                                                ? subseccion
                                                : seccion,
                                    RegisterDate = formattedDate,
                                    SectionId = idSubseccion != "0"
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
                                    NewsId = newsDataDetail.Noticias[0].id_noticia,
                                    Title = originalTitle,
                                    Content = newsDataDetail.Noticias[0].titulo,
                                    Section = subseccion != ""
                                                ? subseccion
                                                : seccion,
                                    RegisterDate = formattedDate,
                                    SectionId = idSubseccion != "0"
                                                ? idSubseccion
                                                : idSeccion
                                };
                                savedNews.Add(savedNew);
                            }

                            await dataContext.Notifications.AddRangeAsync(savedNews);

                            // Actualizamos la última notificación en la tabla de LastNotificationSent
                            lastNotificationSent.NewsId = idNote;
                            lastNotificationSent.RegisterDate = formattedDate;
                            lastNotificationSent.Title = originalTitle;

                            await dataContext.SaveChangesAsync();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user notifications: " + ex.Message);
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
