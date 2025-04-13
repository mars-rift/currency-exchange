using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchangeApp.Models;

namespace CurrencyExchangeApp.Services
{
    public interface ICurrencyService
    {
        /// <summary>
        /// Fetches a list of all available currencies with their current prices
        /// </summary>
        Task<List<Currency>> GetCurrenciesAsync();
        
        /// <summary>
        /// Converts an amount from one currency to another
        /// </summary>
        /// <param name="fromCurrency">Source currency code</param>
        /// <param name="toCurrency">Target currency code</param>
        /// <param name="amount">Amount to convert</param>
        /// <returns>Converted amount in target currency</returns>
        Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount);
        
        /// <summary>
        /// Refreshes the currency data from the API
        /// </summary>
        Task RefreshDataAsync();
    }
}