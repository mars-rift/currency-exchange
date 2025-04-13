using System;

namespace CurrencyExchangeApp.Utils
{
    public static class ErrorHandler
    {
        public static void LogError(string message)
        {
            // Implement logging logic here (e.g., write to a file or logging service)
            Console.WriteLine($"Error: {message}");
        }

        public static void HandleException(Exception ex)
        {
            LogError(ex.Message);
            // Additional error handling logic can be added here (e.g., show a message box)
        }
    }
}