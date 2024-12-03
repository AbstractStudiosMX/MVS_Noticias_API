using Microsoft.EntityFrameworkCore;

namespace MVS_Noticias_API.Models.Currency
{
    public class Currency
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        [Precision(19, 6)]
        public decimal ExchangeRate { get; set; }
        [Precision(19, 6)]
        public decimal AbsoluteChange { get; set; }
        [Precision(19, 6)]
        public decimal GrowthPercentage { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public long Timestamp { get; set; }
    }

    public class CurrencyResponse
    {
        public string from { get; set; }
        public string to { get; set; }
        public decimal value { get; set; }
        public long timestamp { get; set; }
    }

    public class PreviousCloseResponse
    {
        public List<PreviousCloseResult> results { get; set; }
    }

    public class PreviousCloseResult
    {
        public decimal o { get; set; }  // Open
        public decimal h { get; set; }  // High
        public decimal l { get; set; }  // Low
        public decimal c { get; set; }  // Close
        public int v { get; set; }      // Volume
        public long t { get; set; }     // Timestamp
    }
}
