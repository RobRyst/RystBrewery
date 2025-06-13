using Microsoft.Extensions.DependencyInjection;
using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Services;
using RystBrewery.Software.ViewModels;

public static class AppService
{
    public static ServiceProvider Services { get; private set; } = null!;

    public static void Init()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<AlarmService>();

        serviceCollection.AddSingleton<RystIPABrewingService>();
        serviceCollection.AddSingleton<RystEpleciderBrewingService>();
        serviceCollection.AddSingleton<RystSommerølBrewingService>();

        serviceCollection.AddSingleton<RystIPAWashingService>();
        serviceCollection.AddSingleton<RystEpleciderWashingService>();
        serviceCollection.AddSingleton<RystSommerølWashingService>();

        serviceCollection.AddSingleton<RystIPAViewModel>();
        serviceCollection.AddSingleton<RystEpleCiderViewModel>();
        serviceCollection.AddSingleton<RystSommerØlViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();

        serviceCollection.AddSingleton<IBrewingService>(provider => provider.GetRequiredService<RystEpleciderBrewingService>());
        serviceCollection.AddSingleton<IWashingService>(provider => provider.GetRequiredService<RystEpleciderWashingService>());

        Services = serviceCollection.BuildServiceProvider();
    }
}