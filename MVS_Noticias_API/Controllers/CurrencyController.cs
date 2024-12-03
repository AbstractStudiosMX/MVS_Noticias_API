using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.DTO.Currency;
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
        private readonly DataContext _dataContext;
        private readonly ILogger _logger;

        public CurrencyController(DataContext dataContext, ILogger<AuthenticationController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet("convert")]
        public ActionResult<CurrencyDto> ConvertCurrency(string from, string to)
        {
            try
            {
                var currency = _dataContext.Currencies
                    .FirstOrDefault(c => c.From == from && c.To == to);
                if (currency == null)
                {
                    return NotFound("Conversion rate not found.");
                }

                var currencyDto = new CurrencyDto
                {
                    From = from,
                    To = to,
                    ExchangeRate = Math.Round(currency.ExchangeRate,4),
                    AbsoluteChange = currency.AbsoluteChange,
                    GrowthPercentage = Math.Round(currency.GrowthPercentage,4),
                    UpdatedAt = currency.UpdatedAt
                };

                return Ok(currencyDto);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error converting currency: " + ex.Message);
                return BadRequest("Error converting currency.");
            }
        }
    }
}
