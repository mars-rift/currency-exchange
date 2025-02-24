using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var pageSize = 15;
        var currencies = await FetchCurrencyData();
        var prices = await FetchCurrencyPrices();

        if (currencies == null || !currencies.Any() || prices == null)
        {
            Console.WriteLine("Error fetching data from the API.");
            return;
        }

        var currentPage = 0;
        var totalPages = (int)Math.Ceiling(currencies.Count / (double)pageSize);

        while (true)
        {
            Console.Clear();
            DisplayPage(currencies, prices, currentPage, pageSize, totalPages);

            Console.WriteLine("\nNavigation: [N]ext page, [P]revious page, [Q]uit");
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.N when currentPage < totalPages - 1:
                    currentPage++;
                    break;
                case ConsoleKey.P when currentPage > 0:
                    currentPage--;
                    break;
                case ConsoleKey.Q:
                    return;
            }
        }
    }

    static async Task<Dictionary<string, string>?> FetchCurrencyData()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies.json");
            return JsonSerializer.Deserialize<Dictionary<string, string>>(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching currency data: {ex.Message}");
            return null;
        }
    }

    static async Task<Dictionary<string, decimal>?> FetchCurrencyPrices()
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
                using var client = new HttpClient();
                var response = await client.GetStringAsync(endpoint);
                var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(response);

                if (result != null && result.ContainsKey("usd"))
                {
                    var prices = result["usd"].Deserialize<Dictionary<string, decimal>>();
                    if (prices != null && prices.Count > 0)
                    {
                        Console.WriteLine($"Successfully fetched data from: {endpoint}");
                        return prices;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching price data from {endpoint}: {ex.Message}");
                // Continue to next endpoint if this one fails
                continue;
            }
        }

        // If all endpoints fail, return empty dictionary
        Console.WriteLine("All endpoints failed. Returning empty dictionary.");
        return new Dictionary<string, decimal>();
    }

    static void DisplayPage(Dictionary<string, string> currencies, Dictionary<string, decimal> prices,
        int currentPage, int pageSize, int totalPages)
    {
        Console.WriteLine($"Currency List - Page {currentPage + 1} of {totalPages}\n");
        Console.WriteLine("Code".PadRight(10) + "Name".PadRight(40) + "Price (USD)");
        Console.WriteLine("".PadRight(65, '-'));

        var pageItems = currencies
            .Skip(currentPage * pageSize)
            .Take(pageSize);

        foreach (var currency in pageItems)
        {
            var price = prices.TryGetValue(currency.Key, out decimal value)
                ? $"1 USD = {value:F4}"
                : "N/A";

            Console.WriteLine($"{currency.Key.PadRight(10)}{currency.Value.PadRight(40)}{price}");
        }
    }
}
