using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Controllers;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.Currency;
using MVS_Noticias_API.Models.News;
using System.Text.Json;

namespace MVS_Noticias_API.Services
{
    public class NewsUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NewsUpdateService> _logger;

        public NewsUpdateService(IServiceProvider serviceProvider, ILogger<NewsUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                    await CheckForNewsUpdates(dbContext);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during news update: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckForNewsUpdates(DataContext dbContext)
        {
            try
            {
                var httpClient = new HttpClient();
                var apiUrl = "https://mvsnoticias.com/a/API/dinamico.asp?id_modulo=54";
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                jsonString = jsonString.Replace("\"isVideo\": 0", "\"isVideo\": false")
                                       .Replace("\"isVideo\": 1", "\"isVideo\": true")
                                       .Replace("\"isSound\": 0", "\"isSound\": false")
                                       .Replace("\"isSound\": 1", "\"isSound\": true");

                var apiResponse = JsonSerializer.Deserialize<NoticiasResponse>(jsonString);

                if (apiResponse?.Noticias == null || !apiResponse.Noticias.Any())
                {
                    _logger.LogWarning("No news found in the API response.");
                    return;
                }

                var exists = await dbContext.LastNews.AnyAsync(n => n.IdNews == apiResponse.Noticias[0].IdNews);
                if (!exists)
                {
                    var latestNewsFromApi = new LastNews
                    {
                        IdNews = apiResponse.Noticias[0].IdNews,
                        Title = apiResponse.Noticias[0].Title,
                        Description = apiResponse.Noticias[0].Description,
                        Date = apiResponse.Noticias[0].Date,
                        Section = apiResponse.Noticias[0].Section,
                        SubSection = apiResponse.Noticias[0].Section,
                        IdSection = apiResponse.Noticias[0].IdSection,
                        IdSubSection = apiResponse.Noticias[0].IdSubSection,
                        Url = apiResponse.Noticias[0].Url,
                        Slug = apiResponse.Noticias[0].Slug,
                        Photo = apiResponse.Noticias[0].Photo,
                        PhotoMobile = apiResponse.Noticias[0].PhotoMobile,
                        PhotoCredits = apiResponse.Noticias[0].PhotoCredits,
                        PhotoDescription = apiResponse.Noticias[0].PhotoDescription,
                        Author = apiResponse.Noticias[0].Author,
                        IdAuthor = apiResponse.Noticias[0].Number,
                        Creator = apiResponse.Noticias[0].Creator,
                        IdCreator = apiResponse.Noticias[0].IdCreator,
                        IsVideo = apiResponse.Noticias[0].IsVideo,
                        VideoUrl = apiResponse.Noticias[0].VideoUrl,
                        IsSound = apiResponse.Noticias[0].IsSound,
                        SoundUrl = apiResponse.Noticias[0].SoundUrl,
                        Type = apiResponse.Noticias[0].Type,
                        Number = apiResponse.Noticias[0].Number
                    };

                    dbContext.LastNews.Add(latestNewsFromApi);

                    // Limita a las últimas 10 noticias
                    var allNews = await dbContext.LastNews.OrderByDescending(n => n.Date).ToListAsync();
                    if (allNews.Count > 10)
                    {
                        var newsToRemove = allNews.Skip(10);
                        dbContext.LastNews.RemoveRange(newsToRemove);
                    }

                    await dbContext.SaveChangesAsync();

                    // Notificar al frontend
                    await NotifyFrontend(latestNewsFromApi);

                    _logger.LogInformation($"Nueva noticia agregada: {latestNewsFromApi.Title}");
                }
                else
                {
                    _logger.LogInformation($"La noticia con ID {apiResponse.Noticias[0].IdNews} ya existe en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing JSON: {ex.Message}");
                throw;
            }
        }

        private async Task NotifyFrontend(LastNews news)
        {
            await WebSocketService.NotifyClientsAsync(news);
        }
    }
}
