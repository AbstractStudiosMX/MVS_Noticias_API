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
        private readonly DataContext _dbContext;

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

                // Espera un intervalo configurable (e.g., cada 5 minutos)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckForNewsUpdates(DataContext dbContext)
        {
            var httpClient = new HttpClient();
            var apiUrl = "https://mvsnoticias.com/a/API/dinamico.asp?id_modulo=54"; // URL de la API de noticias
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<NoticiasResponse>(jsonString);

            if (apiResponse?.Noticias == null || !apiResponse.Noticias.Any())
            {
                _logger.LogWarning("No news found in the API response.");
                return;
            }

            var latestNewsFromApi = apiResponse.Noticias.First();
            var latestStoredNews = await dbContext.LastNews
                .OrderByDescending(n => n.Date)
                .FirstOrDefaultAsync();

            if (latestStoredNews == null || latestNewsFromApi.IdNews != latestStoredNews.IdNews)
            {
                // Agrega la nueva noticia
                dbContext.LastNews.Add(latestNewsFromApi);

                // Limita a las últimas 10 noticias
                var allNews = await dbContext.LastNews.OrderByDescending(n => n.Date).ToListAsync();
                if (allNews.Count > 10)
                {
                    var newsToRemove = allNews.Skip(10);
                    dbContext.LastNews.RemoveRange(newsToRemove);
                }

                await dbContext.SaveChangesAsync();

                // Notifica a los clientes vía WebSocket
                await NotifyFrontend(latestNewsFromApi);
            }
        }


        public async Task SaveNews(List<LastNews> newsList)
        {
            var dbNews = await _dbContext.LastNews.OrderByDescending(n => n.Date).ToListAsync();
            bool newNewsAdded = false;

            foreach (var news in newsList)
            {
                if (!dbNews.Any(n => n.IdNews == news.IdNews))
                {
                    // Agregar noticia si no existe
                    _dbContext.LastNews.Add(news);
                    newNewsAdded = true;
                }
            }

            // Mantener solo las últimas 10 noticias
            var excessNews = dbNews.Skip(10).ToList();
            if (excessNews.Any())
            {
                _dbContext.LastNews.RemoveRange(excessNews);
            }

            await _dbContext.SaveChangesAsync();

            // Notificar a través de WebSocket si hay noticias nuevas
            if (newNewsAdded)
            {
                var latestNews = await _dbContext.LastNews
                    .OrderByDescending(n => n.Date)
                    .Take(1)
                    .FirstOrDefaultAsync();

                if (latestNews != null)
                {
                    await WebSocketService.NotifyClientsAsync(latestNews);
                }
            }
        }


        private async Task NotifyFrontend(LastNews news)
        {
           await WebSocketService.NotifyClientsAsync(news);
        }
    }


}
