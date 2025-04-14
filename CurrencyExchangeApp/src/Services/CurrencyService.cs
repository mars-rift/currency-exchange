using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyExchangeApp.Models;

namespace CurrencyExchangeApp.Services
{
    public class CurrencyService : ICurrencyService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;
        private List<Currency> _cachedCurrencies = new List<Currency>();

        public CurrencyService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        /// <summary>
        /// Fetches a list of all available currencies with their current prices
        /// </summary>
        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            // Check cache first
            if (_cachedCurrencies != null && _cachedCurrencies.Any())
            {
                return _cachedCurrencies;
            }

            // If no cached data, refresh from API
            await RefreshDataAsync();
            return _cachedCurrencies;
        }

        /// <summary>
        /// Gets currencies with an option to force refresh from the API
        /// </summary>
        /// <param name="forceRefresh">If true, forces a refresh from the API even if cached data exists</param>
        public async Task<List<Currency>> GetCurrenciesWithRefreshAsync(bool forceRefresh)
        {
            if (forceRefresh)
            {
                await RefreshDataAsync();
            }
            return _cachedCurrencies.Any() ? _cachedCurrencies : await GetCurrenciesAsync();
        }

        /// <summary>
        /// Converts an amount from one currency to another
        /// </summary>
        public async Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            if (string.IsNullOrEmpty(fromCurrency))
                throw new ArgumentException("From currency cannot be empty", nameof(fromCurrency));

            if (string.IsNullOrEmpty(toCurrency))
                throw new ArgumentException("To currency cannot be empty", nameof(toCurrency));

            var prices = await FetchCurrencyPrices();

            if (prices == null)
                throw new Exception("Unable to fetch currency rates");

            if (!prices.ContainsKey(fromCurrency.ToLower()))
                throw new Exception($"Currency '{fromCurrency}' not found");

            if (!prices.ContainsKey(toCurrency.ToLower()))
                throw new Exception($"Currency '{toCurrency}' not found");

            // Convert through USD
            var usdAmount = amount / prices[fromCurrency.ToLower()];
            return usdAmount * prices[toCurrency.ToLower()];
        }

        /// <summary>
        /// Refreshes the currency data from the API
        /// </summary>
        public async Task RefreshDataAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RefreshDataAsync called at " + DateTime.Now);

                var currencies = await FetchCurrencyData();
                var prices = await FetchCurrencyPrices();

                if (currencies == null || !currencies.Any())
                {
                    throw new Exception("Failed to fetch currency names from API");
                }

                if (prices == null || !prices.Any())
                {
                    throw new Exception("Failed to fetch currency prices from API");
                }

                _cachedCurrencies = currencies.Select(c => new Currency
                {
                    Code = c.Key,
                    Name = c.Value,
                    Price = prices.TryGetValue(c.Key, out decimal value) ? value : 0
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"Refreshed {_cachedCurrencies.Count} currencies");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshDataAsync: {ex.Message}");
                throw new Exception($"Error retrieving currency data: {ex.Message}", ex);
            }
        }

        private async Task<Dictionary<string, string>?> FetchCurrencyData()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies.json");
                return JsonSerializer.Deserialize<Dictionary<string, string>>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error fetching currency data: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error parsing currency data: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error fetching currency data: {ex.Message}", ex);
            }
        }

        private async Task<Dictionary<string, decimal>> FetchCurrencyPrices()
        {
            var endpoints = new[]
            {
                "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json",
                "https://latest.currency-api.pages.dev/v1/currencies/usd.json"
            };

            Exception? lastException = null;

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var response = await _httpClient.GetStringAsync(endpoint);
                    var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(response);

                    if (result != null && result.ContainsKey("usd"))
                    {
                        var prices = result["usd"].Deserialize<Dictionary<string, decimal>>();
                        if (prices != null && prices.Count > 0)
                        {
                            return prices;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Store the last exception but continue trying other endpoints
                    lastException = ex;
                }
            }

            // If we get here, all endpoints failed
            throw new Exception($"All API endpoints failed: {lastException?.Message}", lastException);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
