using LiveChartsCore;
using Microsoft.Extensions.DependencyInjection;
using RystBrewery.Software.Services;
using RystBrewery.Software.ViewModels;

namespace RystBrewery.Software
{
    public static class AppService
    {
        public static ServiceProvider Services { get; private set; }

        public static void Init()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IBrewingService, BrewingService>();
            serviceCollection.AddSingleton<IWashingService, WashingService>();
            serviceCollection.AddTransient<RystEpleCiderViewModel>();
            serviceCollection.AddSingleton<RystEpleCiderViewModel>();

            Services = serviceCollection.BuildServiceProvider();
        }
    }
}
