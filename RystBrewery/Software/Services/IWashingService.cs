using RystBrewery.Software.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RystBrewery.Software.Services
{
    public interface IWashingService
    {
        event Action<string> WashingStepChanged;
        event Action IsCompleted;
        bool IsRunning { get; }
        void StartWashing(WashProgram program);
        void StopWashing();
    }
}
