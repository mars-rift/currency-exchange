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
        private List<Currency> _cachedCurrencies = new List<Currency>(); // Initialize to fix CS8618

        public CurrencyService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15); // Set a reasonable timeout
        }

        // Implement the ICurrencyService interface directly - no explicit implementation
        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            if (_cachedCurrencies != null && _cachedCurrencies.Any())
            {
                return _cachedCurrencies;
            }

            await RefreshDataAsync(); // Changed to match return type
            return _cachedCurrencies;
        }

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

        // Changed to return Task instead of Task<List<Currency>> to match interface
        public async Task RefreshDataAsync()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                // Log the exception details here
                throw new Exception($"Error retrieving currency data: {ex.Message}", ex);
            }
        }

        private async Task<Dictionary<string, string>?> FetchCurrencyData() // Changed return type to nullable
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

            Exception? lastException = null; // Added ? to make nullable

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
