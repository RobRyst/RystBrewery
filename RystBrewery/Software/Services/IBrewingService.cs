using RystBrewery.Software.Database;
using System.Collections.ObjectModel;

namespace RystBrewery.Software.Services
{
    public interface IBrewingService
    {
        event Action<string> BrewingStepChanged;
        event Action IsCompleted;
        bool IsRunning { get; }
        void StartBrewing(Recipe recipe);
        void StopBrewing();

        ObservableCollection<int> TemperatureValues { get; }
        ObservableCollection<int> MaltValues { get; }

        ObservableCollection<int> AppleJuiceValues { get; }

        ObservableCollection<int> HopValues { get; }
        ObservableCollection<int> JuniperValues { get; }

    }
}
