using Microsoft.EntityFrameworkCore;
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

                int idNota = newsData.notifications[0].data.idnota;

                var lastNotificationSent = await dataContext.LastNotificationSent.FirstOrDefaultAsync();

                // Si la noticia es la misma nos salimos del flujo
                if (lastNotificationSent!.NewsId == idNota)
                {
                    _logger.LogInformation($"Notification with idNota {idNota} has already been processed. Skipping.");
                    return;
                }

                string originalTitle = newsData.notifications[0].contents.en;

                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;
                var responseNewsMVS = await httpClient.GetStringAsync(string.Format("{0}noticias.asp?id_noticia={1}&contenido=si", apiEditor80, idNota));
                var newsDataDetail = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                TimeZoneInfo mexicoCityTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
                long unixDate = newsData.notifications[0].completed_at;
                DateTime dateUtc = DateTimeOffset.FromUnixTimeSeconds(unixDate).UtcDateTime;
                DateTime dateMexicoCity = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, mexicoCityTimeZone);
                string formattedDate = dateMexicoCity.ToString("dd/MM/yyyy HH:mm:ss");

                // Filtrar usuarios sin configuración en NotificationSettings
                var allUsers = await dataContext.Users.Select(ns => ns.Id).ToListAsync();
                var usersWithSettings = await dataContext.NotificationsSettings.Where(ns => allUsers.Contains(ns.UserId)).ToListAsync();
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

                    // Se guarda al usuario root para que exista un histórico
                    //var rootSavedNew = new UserNotifications
                    //{
                    //    UserId = 1,
                    //    NewsId = notification.id_noticia,
                    //    Title = notification.titulo,
                    //    Content = notification.descripcion,
                    //    Section = subseccion != ""
                    //                    ? subseccion
                    //                    : seccion,
                    //    RegisterDate = formattedDate,
                    //    SectionId = idSubseccion != "0"
                    //                    ? idSubseccion
                    //                    : idSeccion
                    //};
                    //savedNews.Add(rootSavedNew);

                    foreach (var userId in usersWithoutSettings)
                    {
                        var savedNew = new UserNotifications
                        {
                            UserId = userId,
                            NewsId = notification.id_noticia,
                            Title = originalTitle,
                            Content = notification.titulo,
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
                            NewsId = notification.id_noticia,
                            Title = originalTitle,
                            Content = notification.titulo,
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
                }

                // Inserciones con validación de duplicados y transacción
                const int batchSize = 100; // Tamaño del lote

                if (savedNews.Any())
                {
                    var uniqueNews = new List<UserNotifications>();

                    foreach (var news in savedNews)
                    {
                        bool exists = await dataContext.Notifications
                            .AnyAsync(n => n.UserId == news.UserId && n.NewsId == news.NewsId);

                        if (!exists)
                        {
                            uniqueNews.Add(news);
                        }
                    }

                    var executionStrategy = dataContext.Database.CreateExecutionStrategy();

                    await executionStrategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await dataContext.Database.BeginTransactionAsync();
                        try
                        {
                            // Inserciones en lotes
                            for (int i = 0; i < uniqueNews.Count; i += batchSize)
                            {
                                var batch = uniqueNews.Skip(i).Take(batchSize).ToList();
                                await dataContext.Notifications.AddRangeAsync(batch);
                                await dataContext.SaveChangesAsync();
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

                // Actualizamos la última notificación en la tabla de LastNotificationSent
                lastNotificationSent.NewsId = idNota;
                await dataContext.SaveChangesAsync();
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
