using RystBrewery.Software.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
