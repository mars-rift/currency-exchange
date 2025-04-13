using System;
using System.Windows;
using CurrencyExchangeApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyExchangeApp
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        // Entry point for the application  
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure services  
            var services = new ServiceCollection();

            // Register your services  
            services.AddSingleton<ICurrencyService, CurrencyService>();
            services.AddSingleton<CurrencyDataService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Perform any cleanup before the application exits  
            base.OnExit(e);
        }
    }
}
