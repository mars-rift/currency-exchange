using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyExchangeApp.Utils;

namespace CurrencyExchangeApp.Services
{
    public class DataPersistenceService
    {
        private readonly string _dataFilePath;

        public DataPersistenceService(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public async Task SaveDataAsync<T>(List<T> data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                await File.WriteAllTextAsync(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError($"Error saving data: {ex.Message}");
            }
        }

        public async Task<List<T>> LoadDataAsync<T>()
        {
            if (!File.Exists(_dataFilePath))
            {
                return new List<T>();
            }

            try
            {
                var json = await File.ReadAllTextAsync(_dataFilePath);
                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError($"Error loading data: {ex.Message}");
                return new List<T>();
            }
        }
    }
}