using System;

namespace CurrencyExchangeApp.Models
{
    public class CurrencyRate
    {
        public string CurrencyCode { get; set; }
        public decimal Rate { get; set; }

        public CurrencyRate(string currencyCode, decimal rate)
        {
            CurrencyCode = currencyCode;
            Rate = rate;
        }

        public override string ToString()
        {
            return $"{CurrencyCode}: {Rate:F4}";
        }
    }
}