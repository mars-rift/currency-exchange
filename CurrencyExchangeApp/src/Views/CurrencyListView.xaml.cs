using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CurrencyExchangeApp.ViewModels;

namespace CurrencyExchangeApp.Views
{
    public partial class CurrencyListView : Window
    {
        private readonly MainViewModel _viewModel;

        public CurrencyListView(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox) // Cast sender to TextBox to access the Text property
            {
                _viewModel.FilterCurrencies(textBox.Text);
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem is string selectedSort)
            {
                _viewModel.SortCurrencies(selectedSort);
            }
        }

       

        private void CurrencyListView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.SaveData();
        }
    }
}
