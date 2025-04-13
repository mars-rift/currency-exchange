using System.Windows;
using System.Windows.Controls;
using CurrencyExchangeApp.ViewModels;

namespace CurrencyExchangeApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.RefreshData();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewModel?.SearchCommand != null && SearchTextBox != null)
            {
                ViewModel.SearchCommand.Execute(SearchTextBox.Text);
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel?.SortCommand != null && SortComboBox?.SelectedItem != null)
            {
                // Get the content of the selected ComboBoxItem
                var selectedItem = SortComboBox.SelectedItem as ComboBoxItem;
                string sortOption = selectedItem?.Content?.ToString();
                if (!string.IsNullOrEmpty(sortOption))
                {
                    ViewModel.SortCommand.Execute(sortOption);
                }
            }
        }
    }
}