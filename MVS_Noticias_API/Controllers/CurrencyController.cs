using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVS_Noticias_API.Models.Currency;
using System.Net.Http;
using System.Text.Json;

namespace MVS_Noticias_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class CurrencyController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CurrencyController(IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("convert")]
        public async Task<ActionResult<Currency>> ConvertCurrency(string from, string to)
        {
            try
            {
                var apiKey = _configuration.GetSection("AppSettings:CurrencyApiKey").Value;
                var amount = 1; // amount to convert (default 1)

                var httpClient = new HttpClient();

                // Solicitud de conversión de divisas actual
                var responseCurrencyConvertion = await httpClient.GetAsync($"https://api.finage.co.uk/convert/forex/{from}/{to}/{amount}?apikey={apiKey}");
                responseCurrencyConvertion.EnsureSuccessStatusCode();
                var currencyConvertionContent = await responseCurrencyConvertion.Content.ReadAsStringAsync();
                var currencyConvertion = JsonSerializer.Deserialize<CurrencyResponse>(currencyConvertionContent);

                // Solicitud de precio de cierre del día anterior
                var responsePreviousClose = await httpClient.GetAsync($"https://api.finage.co.uk/agg/forex/prev-close/{from}{to}?apikey={apiKey}");
                responsePreviousClose.EnsureSuccessStatusCode();
                var previousCloseContent = await responsePreviousClose.Content.ReadAsStringAsync();
                var previousCloseResponse = JsonSerializer.Deserialize<PreviousCloseResponse>(previousCloseContent);
                var previousClose = previousCloseResponse.results[0].c;

                // Calcular cambio absoluto
                var absoluteChange = currencyConvertion.value - previousClose;

                // Calcular porcentaje de crecimiento
                var growthPercentage = ((currencyConvertion.value - previousClose) / previousClose) * 100;

                var currencyData = new Currency
                {
                    From = from,
                    To = to,
                    ExchangeRate = currencyConvertion.value,
                    AbsoluteChange = absoluteChange,
                    GrowthPercentage = growthPercentage,
                    Timestamp = currencyConvertion.timestamp,
                };

                return Ok(currencyData);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting currency data: " + ex.Message);
                return BadRequest("Error getting currency data.");
            }
        }
    }
}
