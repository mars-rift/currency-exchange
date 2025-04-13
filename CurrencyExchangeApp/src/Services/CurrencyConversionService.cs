using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyExchangeApp.Services
{
    public class CurrencyConversionService
    {
        private readonly Dictionary<string, decimal> _currencyRates;

        public CurrencyConversionService(Dictionary<string, decimal> currencyRates)
        {
            _currencyRates = currencyRates;
        }

        public decimal Convert(string fromCurrency, string toCurrency, decimal amount)
        {
            if (!_currencyRates.ContainsKey(fromCurrency) || !_currencyRates.ContainsKey(toCurrency))
            {
                throw new ArgumentException("Invalid currency code.");
            }

            var fromRate = _currencyRates[fromCurrency];
            var toRate = _currencyRates[toCurrency];

            // Convert the amount to USD first, then to the target currency
            var amountInUSD = amount / fromRate;
            var convertedAmount = amountInUSD * toRate;

            return convertedAmount;
        }

        public List<string> GetAvailableCurrencies()
        {
            return _currencyRates.Keys.ToList();
        }
    }
}