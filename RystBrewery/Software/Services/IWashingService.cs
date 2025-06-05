using RystBrewery.Software.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RystBrewery.Software.Services
{
    public interface IWashingService
    {
        event Action<string> WashingStepChanged;
        ObservableCollection<int> RinseValues { get; }
        ObservableCollection<int> DetergentValues { get; }
        ObservableCollection<int> MaltValues { get; }
        ObservableCollection<int> TemperatureValues { get; }

        event Action IsCompleted;
        bool IsRunning { get; }
        void StartWashing(WashProgram program);
        void StopWashing();
    }
}
