namespace MVS_Noticias_API.DTO.Currency
{
    public class CurrencyDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal AbsoluteChange { get; set; }
        public decimal GrowthPercentage { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
