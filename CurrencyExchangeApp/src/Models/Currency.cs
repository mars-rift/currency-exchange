using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CurrencyExchangeApp.Models
{
    public class Currency : INotifyPropertyChanged
    {
        private string _code;
        private string _name;
        private decimal _price;

        public string Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PriceFormatted));
                }
            }
        }

        public string PriceFormatted => $"1 USD = {Price:F4}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}