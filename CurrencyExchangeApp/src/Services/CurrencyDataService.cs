using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyExchangeApp.Utils;

namespace CurrencyExchangeApp.Services
{
    public class CurrencyDataService
    {
        private readonly HttpClient _httpClient;

        public CurrencyDataService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, string>?> FetchCurrencyDataAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies.json");
                return JsonSerializer.Deserialize<Dictionary<string, string>>(response);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError($"Error fetching currency data: {ex.Message}");
                return null;
            }
        }

        public async Task<Dictionary<string, decimal>?> FetchCurrencyPricesAsync()
        {
            var endpoints = new[]
            {
                "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json",
                "https://latest.currency-api.pages.dev/v1/currencies/usd.json"
            };

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
                    ErrorHandler.LogError($"Error fetching price data from {endpoint}: {ex.Message}");
                }
            }

            return new Dictionary<string, decimal>();
        }
    }
}