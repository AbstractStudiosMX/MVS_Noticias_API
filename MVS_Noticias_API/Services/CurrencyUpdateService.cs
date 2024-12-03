using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using MVS_Noticias_API.Controllers;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.Currency;
using System.Text.Json;

namespace MVS_Noticias_API.Services
{
    public class CurrencyUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CurrencyUpdateService(ILogger<AuthenticationController> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Calcula el tiempo hasta las 9:00 a.m.
                var now = DateTime.Now;
                var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0);

                if (now > nextRunTime)
                {
                    nextRunTime = nextRunTime.AddDays(1);
                }

                var delay = nextRunTime - now;
                await Task.Delay(delay, stoppingToken); // Espera hasta las 9 a.m.

                await UpdateCurrencyRates();

                // Espera 24 horas para la siguiente ejecución
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task UpdateCurrencyRates()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var httpClient = new HttpClient();
                var apiKey = _configuration.GetSection("AppSettings:CurrencyApiKey").Value;

                var currencies = new List<string> { "JPY", "USD", "EUR", "MXN", "GBP" };
                foreach (var baseCurrency in currencies)
                {
                    foreach (var targetCurrency in currencies)
                    {
                        if ((baseCurrency == "CHF" && targetCurrency == "COP") || (baseCurrency == "COP" && targetCurrency == "CHF"))
                        {
                            continue;
                        }
                        if (baseCurrency == targetCurrency)
                        {
                            // Caso especial para monedas iguales (e.g., USD -> USD)
                            await HandleSameCurrencyCase(dbContext, baseCurrency, currencies, httpClient, apiKey);
                            continue;
                        }

                        // Generar el símbolo en el formato requerido por Finage (e.g., USD/MXN)
                        var symbol = $"{baseCurrency}/{targetCurrency}";

                        // Obtener tasa de cambio en tiempo real
                        var realTimeResponse = await httpClient.GetAsync($"https://api.finage.co.uk/convert/forex/{baseCurrency}/{targetCurrency}/1?apikey={apiKey}");
                        realTimeResponse.EnsureSuccessStatusCode();
                        var realTimeData = JsonSerializer.Deserialize<CurrencyResponse>(await realTimeResponse.Content.ReadAsStringAsync());
                        var currentRate = realTimeData?.value ?? 0;

                        // Obtener tasa de cierre previo
                        var previousCloseResponse = await httpClient.GetAsync($"https://api.finage.co.uk/agg/forex/prev-close/{baseCurrency}{targetCurrency}?apikey={apiKey}");
                        previousCloseResponse.EnsureSuccessStatusCode();
                        var previousCloseData = JsonSerializer.Deserialize<PreviousCloseResponse>(await previousCloseResponse.Content.ReadAsStringAsync());
                        var previousRate = previousCloseData?.results?.FirstOrDefault()?.c ?? 0;

                        if (currentRate <= 0 || previousRate <= 0)
                        {
                            _logger.LogError($"Invalid rate data for {baseCurrency} -> {targetCurrency}: currentRate={currentRate}, previousRate={previousRate}");
                            continue;
                        }

                        // Calcular AbsoluteChange y GrowthPercentage
                        var absoluteChange = currentRate - previousRate;
                        var growthPercentage = (absoluteChange / previousRate) * 100;

                        // Guardar la tasa directa
                        await UpdateOrAddCurrencyRate(dbContext, baseCurrency, targetCurrency, currentRate, absoluteChange, growthPercentage, realTimeData.timestamp);

                        // Calcular y guardar la tasa inversa
                        var inverseRate = 1 / currentRate;
                        var inverseAbsoluteChange = -absoluteChange;
                        var inverseGrowthPercentage = -growthPercentage;

                        await UpdateOrAddCurrencyRate(dbContext, targetCurrency, baseCurrency, inverseRate, inverseAbsoluteChange, inverseGrowthPercentage, realTimeData.timestamp);
                    }

                    await dbContext.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating currency rates: {ex.Message}");
            }
        }

        private async Task HandleSameCurrencyCase(DataContext dbContext, string currency, List<string> currencies, HttpClient httpClient, string apiKey)
        {
            // Usar la primera moneda distinta como referencia cruzada
            var referenceCurrency = "USD";

            if (currency == "USD")
            {
                referenceCurrency = "EUR";
            }

            // Obtener tasa cruzada actual
            var realTimeResponse = await httpClient.GetAsync($"https://api.finage.co.uk/convert/forex/{currency}/{referenceCurrency}/1?apikey={apiKey}");
            realTimeResponse.EnsureSuccessStatusCode();
            var realTimeData = JsonSerializer.Deserialize<CurrencyResponse>(await realTimeResponse.Content.ReadAsStringAsync());
            var currentRate = realTimeData?.value ?? 0;

            // Obtener tasa cruzada de cierre previo
            var previousCloseResponse = await httpClient.GetAsync($"https://api.finage.co.uk/agg/forex/prev-close/{currency}{referenceCurrency}?apikey={apiKey}");
            previousCloseResponse.EnsureSuccessStatusCode();
            var previousCloseData = JsonSerializer.Deserialize<PreviousCloseResponse>(await previousCloseResponse.Content.ReadAsStringAsync());
            var previousRate = previousCloseData?.results?.FirstOrDefault()?.c ?? 0;

            if (currentRate <= 0 || previousRate <= 0)
            {
                _logger.LogError($"Invalid cross rate data for {currency} -> {referenceCurrency}");
                return;
            }

            // Calcular valores basados en la tasa cruzada
            var absoluteChange = currentRate - previousRate;
            var growthPercentage = (absoluteChange / previousRate) * 100;

            // Guardar para moneda igual (e.g., USD -> USD)
            await UpdateOrAddCurrencyRate(dbContext, currency, currency, 1, absoluteChange, growthPercentage, realTimeData.timestamp);
        }

        private async Task UpdateOrAddCurrencyRate(DataContext dbContext, string from, string to, decimal exchangeRate, decimal absoluteChange, decimal growthPercentage, long timestamp)
        {
            try
            {
                var existingRate = await dbContext.Currencies.FirstOrDefaultAsync(r => r.From == from && r.To == to);

                if (existingRate != null)
                {
                    existingRate.ExchangeRate = exchangeRate;
                    existingRate.AbsoluteChange = absoluteChange;
                    existingRate.GrowthPercentage = growthPercentage;
                    existingRate.Timestamp = timestamp;
                    existingRate.UpdatedAt = DateTime.UtcNow;

                    dbContext.Entry(existingRate).State = EntityState.Modified;
                }
                else
                {
                    await dbContext.Currencies.AddAsync(new Currency
                    {
                        From = from,
                        To = to,
                        ExchangeRate = exchangeRate,
                        AbsoluteChange = absoluteChange,
                        GrowthPercentage = growthPercentage,
                        Timestamp = timestamp,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving rates: {ex.Message}");
                throw;
            }
            
        }

    }
}
