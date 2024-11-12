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

                decimal currentRate;

                // Caso especial: conversión de la misma moneda (por ejemplo, USD a USD o EUR a EUR)
                if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
                {
                    // Tipo de cambio actual es ficticiamente 1 porque es la misma moneda
                    currentRate = 1;

                    // Usa la moneda frente al USD como referencia (por ejemplo, EUR/USD si from es EUR)
                    var referenceSymbol = $"{from}USD";
                    if (from == "USD")
                    {
                        referenceSymbol = "USDEUR";
                    }

                    // Solicitud del precio de cierre del día anterior
                    var responsePreviousClose = await httpClient.GetAsync($"https://api.finage.co.uk/agg/forex/prev-close/{referenceSymbol}?apikey={apiKey}");
                    responsePreviousClose.EnsureSuccessStatusCode();
                    var previousCloseContent = await responsePreviousClose.Content.ReadAsStringAsync();
                    var previousCloseResponse = JsonSerializer.Deserialize<PreviousCloseResponse>(previousCloseContent);
                    var previousClose = previousCloseResponse.results[0].c;

                    // Calcular el cambio absoluto y el porcentaje de crecimiento
                    var absoluteChange = currentRate - previousClose;
                    var growthPercentage = ((currentRate - previousClose) / previousClose) * 100;

                    var currencyData = new Currency
                    {
                        From = from,
                        To = to,
                        ExchangeRate = currentRate,
                        AbsoluteChange = absoluteChange,
                        GrowthPercentage = growthPercentage,
                        Timestamp = previousCloseResponse.results[0].t
                    };

                    return Ok(currencyData);
                }
                else
                {
                    // Solicitud de conversión de divisas actual
                    var responseCurrencyConversion = await httpClient.GetAsync($"https://api.finage.co.uk/convert/forex/{from}/{to}/{amount}?apikey={apiKey}");
                    responseCurrencyConversion.EnsureSuccessStatusCode();
                    var currencyConversionContent = await responseCurrencyConversion.Content.ReadAsStringAsync();
                    var currencyConversion = JsonSerializer.Deserialize<CurrencyResponse>(currencyConversionContent);
                    currentRate = currencyConversion.value;

                    // Solicitud de precio de cierre del día anterior
                    var responsePreviousClose = await httpClient.GetAsync($"https://api.finage.co.uk/agg/forex/prev-close/{from}{to}?apikey={apiKey}");
                    responsePreviousClose.EnsureSuccessStatusCode();
                    var previousCloseContent = await responsePreviousClose.Content.ReadAsStringAsync();
                    var previousCloseResponse = JsonSerializer.Deserialize<PreviousCloseResponse>(previousCloseContent);
                    var previousClose = previousCloseResponse.results[0].c;

                    // Calcular el cambio absoluto y el porcentaje de crecimiento
                    var absoluteChange = currentRate - previousClose;
                    var growthPercentage = ((currentRate - previousClose) / previousClose) * 100;

                    var currencyData = new Currency
                    {
                        From = from,
                        To = to,
                        ExchangeRate = currentRate,
                        AbsoluteChange = absoluteChange,
                        GrowthPercentage = growthPercentage,
                        Timestamp = currencyConversion.timestamp
                    };

                    return Ok(currencyData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting currency data: " + ex.Message);
                return BadRequest("Error getting currency data.");
            }
        }
    }
}
