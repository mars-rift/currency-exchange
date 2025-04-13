# Currency Exchange Application

## Overview
The Currency Exchange Application is a WPF-based application that allows users to view currency data, convert between currencies, and manage currency information with enhanced features such as search functionality, sorting options, and data persistence.

## Features
- **Currency List**: View a list of available currencies with their names and exchange rates.
- **Search Functionality**: Easily search for specific currencies by code or name.
- **Sorting Options**: Sort the currency list based on different criteria.
- **Data Refresh**: Refresh currency data from external APIs to ensure up-to-date information.
- **Data Persistence**: Save and load currency data to and from CSV or JSON files for offline access.
- **Currency Conversion**: Convert amounts between different currencies using real-time exchange rates.
- **Improved Error Handling**: Robust error handling and logging mechanisms to manage API failures and user input errors.
- **User Interface Enhancements**: A modern and user-friendly interface for a better user experience.

## Project Structure
```
CurrencyExchangeApp
├── src
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── Models
│   │   ├── Currency.cs
│   │   └── CurrencyRate.cs
│   ├── Services
│   │   ├── CurrencyDataService.cs
│   │   ├── DataPersistenceService.cs
│   │   └── CurrencyConversionService.cs
│   ├── ViewModels
│   │   ├── BaseViewModel.cs
│   │   ├── MainViewModel.cs
│   │   └── CurrencyConverterViewModel.cs
│   ├── Views
│   │   ├── CurrencyListView.xaml
│   │   ├── CurrencyListView.xaml.cs
│   │   ├── CurrencyConverterView.xaml
│   │   └── CurrencyConverterView.xaml.cs
│   └── Utils
│       ├── ErrorHandler.cs
│       └── RelayCommand.cs
├── CurrencyExchangeApp.csproj
├── App.config
└── README.md
```

## Setup Instructions
1. Clone the repository to your local machine.
2. Open the solution in your preferred IDE.
3. Restore the NuGet packages required for the project.
4. Build the solution to ensure all dependencies are resolved.
5. Run the application to start using the Currency Exchange Application.

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.