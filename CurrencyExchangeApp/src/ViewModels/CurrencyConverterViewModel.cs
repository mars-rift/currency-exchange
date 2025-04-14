using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CurrencyExchangeApp.Utils;

namespace CurrencyExchangeApp.ViewModels
{
    public class CurrencyConverterViewModel : BaseViewModel
    {
        private decimal _amountToConvert;
        private string _selectedFromCurrency = string.Empty; // Initialize to avoid nullability warnings
        private string _selectedToCurrency = string.Empty;   // Initialize to avoid nullability warnings
        private decimal _convertedAmount;

        public decimal AmountToConvert
        {
            get => _amountToConvert;
            set
            {
                _amountToConvert = value;
                OnPropertyChanged(nameof(AmountToConvert));
                ConvertAmount();
            }
        }

        public string SelectedFromCurrency
        {
            get => _selectedFromCurrency;
            set
            {
                _selectedFromCurrency = value;
                OnPropertyChanged(nameof(SelectedFromCurrency));
                ConvertAmount();
            }
        }

        public string SelectedToCurrency
        {
            get => _selectedToCurrency;
            set
            {
                _selectedToCurrency = value;
                OnPropertyChanged(nameof(SelectedToCurrency));
                ConvertAmount();
            }
        }

        public decimal ConvertedAmount
        {
            get => _convertedAmount;
            private set
            {
                _convertedAmount = value;
                OnPropertyChanged(nameof(ConvertedAmount));
            }
        }

        public ObservableCollection<string> CurrencyList { get; set; }

        public ICommand RefreshCommand { get; }

        public CurrencyConverterViewModel()
        {
            CurrencyList = new ObservableCollection<string>();
            RefreshCommand = new RelayCommand(RefreshData);
            LoadCurrencies();
        }

        private void LoadCurrencies()
        {
            // Logic to load currencies from the data service
        }

        private void ConvertAmount()
        {
            if (string.IsNullOrEmpty(SelectedFromCurrency) || string.IsNullOrEmpty(SelectedToCurrency) || AmountToConvert <= 0)
            {
                ConvertedAmount = 0;
                return;
            }

            // Logic to convert the amount using the CurrencyConversionService
        }

        private void RefreshData()
        {
            // Logic to refresh currency data
        }
    }
}