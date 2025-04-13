using System;
using System.Windows;
using System.Windows.Controls;
using CurrencyExchangeApp.ViewModels;

namespace CurrencyExchangeApp.Views
{
    // Changed from UserControl to Window since there appears to be another partial declaration
    // that uses Window as its base class
    public partial class CurrencyConverterView
    {
        private CurrencyConverterViewModel _viewModel;

        public CurrencyConverterView()
        {
            InitializeComponent();
            _viewModel = new CurrencyConverterViewModel();
            this.DataContext = _viewModel; // Added 'this.' to access DataContext property
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null && !string.IsNullOrEmpty(AmountTextBox?.Text))
            {
                if (decimal.TryParse(AmountTextBox.Text, out decimal amount))
                {
                    _viewModel.AmountToConvert = amount;
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.RefreshCommand != null)
            {
                _viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
