using System.Windows;
using CurrencyRpcTestClient;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            var channel = GrpcChannel.ForAddress("https://localhost:7085");
            var client = new Currency.CurrencyClient(channel);
            services.AddSingleton(client);
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}