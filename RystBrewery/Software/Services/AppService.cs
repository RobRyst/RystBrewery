using Microsoft.Extensions.DependencyInjection;
using RystBrewery.Software.Services;
using RystBrewery.Software.ViewModels;

public static class AppService
{
    public static ServiceProvider Services { get; private set; }

    public static void Init()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<RystIPABrewingService>();
        serviceCollection.AddSingleton<RystEpleciderBrewingService>();
        serviceCollection.AddSingleton<RystSommerølBrewingService>();

        serviceCollection.AddSingleton<RystIPAWashingService>();
        serviceCollection.AddSingleton<RystEpleciderWashingService>();
        serviceCollection.AddSingleton<RystSommerølWashingService>();

        serviceCollection.AddSingleton<RystIPAViewModel>();
        serviceCollection.AddSingleton<RystEpleCiderViewModel>();
        serviceCollection.AddSingleton<RystSommerØlViewModel>();

        Services = serviceCollection.BuildServiceProvider();
    }

}

