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
        event Action<string> StepChanged;
        event Action Completed;
        bool IsRunning { get; }
        void Start(WashProgram program);
        void Stop();
    }
}
