using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CurrencyExchangeApp.Models;
using CurrencyExchangeApp.Services;
using CurrencyExchangeApp.Utils;

namespace CurrencyExchangeApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ICurrencyService _currencyService;
        private ObservableCollection<Currency> _allCurrencies;
        private ObservableCollection<Currency> _filteredCurrencies;
        private string _searchText = string.Empty;
        private bool _isLoading;
        private string _errorMessage = string.Empty; // Initialize to avoid CS8618
        private bool _isSortedByCode;
        private bool _isSortedByName;
        private bool _isSortedByPrice;

        public ObservableCollection<Currency> FilteredCurrencies
        {
            get => _filteredCurrencies;
            set => SetProperty(ref _filteredCurrencies, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand SearchCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand RefreshCommand { get; }
        public MainViewModel() : this(new CurrencyService())
        {
        }

        public MainViewModel(ICurrencyService currencyService)
        {
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _allCurrencies = new ObservableCollection<Currency>();
            _filteredCurrencies = new ObservableCollection<Currency>(); // Initialize to avoid CS8618

            SearchCommand = new RelayCommand<string>(Search);
            SortCommand = new RelayCommand<string>(Sort);
            ExportCommand = new RelayCommand(ExportData, () => FilteredCurrencies.Any());
            RefreshCommand = new RelayCommand(RefreshData);
            Task.Run(() => LoadDataAsync());
        }

        public async void RefreshData()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                System.Diagnostics.Debug.WriteLine("MainViewModel.RefreshData called");

                // Force refresh by calling RefreshDataAsync directly
                await _currencyService.RefreshDataAsync();

                // Now get the refreshed data
                var currencyData = await _currencyService.GetCurrenciesAsync();

                _allCurrencies.Clear();
                foreach (var currency in currencyData)
                {
                    _allCurrencies.Add(currency);
                }

                ApplyFilterAndSort();
                System.Diagnostics.Debug.WriteLine($"Refresh complete, loaded {_allCurrencies.Count} currencies");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshData: {ex.Message}");
                ErrorMessage = $"Error refreshing data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }


        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var currencyData = await _currencyService.GetCurrenciesAsync();

                _allCurrencies.Clear();
                foreach (var currency in currencyData)
                {
                    _allCurrencies.Add(currency);
                }

                ApplyFilterAndSort();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Search(string searchText)
        {
            _searchText = searchText?.ToLower() ?? string.Empty;
            ApplyFilterAndSort();
        }

        private void Sort(string sortOption)
        {
            switch (sortOption)
            {
                case "Code":
                    _isSortedByCode = true;
                    _isSortedByName = false;
                    _isSortedByPrice = false;
                    break;
                case "Name":
                    _isSortedByCode = false;
                    _isSortedByName = true;
                    _isSortedByPrice = false;
                    break;
                case "Price":
                    _isSortedByCode = false;
                    _isSortedByName = false;
                    _isSortedByPrice = true;
                    break;
            }

            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            var filtered = _allCurrencies.Where(c =>
                string.IsNullOrEmpty(_searchText) ||
                c.Code.ToLower().Contains(_searchText) ||
                c.Name.ToLower().Contains(_searchText)).ToList();

            if (_isSortedByCode)
                filtered = filtered.OrderBy(c => c.Code).ToList();
            else if (_isSortedByName)
                filtered = filtered.OrderBy(c => c.Name).ToList();
            else if (_isSortedByPrice)
                filtered = filtered.OrderByDescending(c => c.Price).ToList();

            FilteredCurrencies = new ObservableCollection<Currency>(filtered);
        }

        private void ExportData()
        {
            // Implement CSV export
            try
            {
                // Placeholder for export implementation
                // Will be implemented when we add the DataExportService
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error exporting data: {ex.Message}";
            }
        }

        // These methods should be implemented properly or removed
        // They're currently throwing NotImplementedException
        internal void FilterCurrencies(string text)
        {
            Search(text); // Use existing search functionality instead of throwing exception
        }

        internal void SortCurrencies(string selectedSort)
        {
            Sort(selectedSort); // Use existing sort functionality instead of throwing exception
        }

        internal void SaveData()
        {
            ExportData(); // Use existing export functionality instead of throwing exception
        }
    }
}
