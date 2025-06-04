using RystBrewery.Software.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RystBrewery.Software.Services
{
    internal class WashingService : IWashingService
    {
        public event Action<string> WashingStepChanged;
        public event Action IsCompleted;

        private DispatcherTimer _washingTimer;
        private WashProgram _washProgram;
        private int _washingStepIndex;
        private int _stepTimeElapsed;

        public bool IsRunning => _washingTimer?.IsEnabled == true;

        public WashingService()
        {
            _washingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _washingTimer.Tick += WashingTick;
        }

        public void StartWashing(WashProgram program)
        {
            if (IsRunning) return;

            _washProgram = program;
            _washingStepIndex = 0;
            _stepTimeElapsed = 0;
            _washingTimer.Start();
        }

        public void StopWashing()
        {
            _washingTimer.Stop();
        }

        private void WashingTick(object sender, EventArgs e)
        {
            if (_washProgram == null || _washingStepIndex >= _washProgram.Steps.Count)
            {
                WashingStepChanged?.Invoke("Washing complete.");
                IsCompleted?.Invoke();
                _washingTimer.Stop();
                return;
            }

            // Fixed syntax - removed incorrect * operators
            var step = _washProgram.Steps[_washingStepIndex];
            WashingStepChanged?.Invoke($"Step {_washingStepIndex + 1}/{_washProgram.Steps.Count}: {step.Description} ({step.Time}s)");

            _stepTimeElapsed++;
            if (_stepTimeElapsed >= step.Time)
            {
                _washingStepIndex++;
                _stepTimeElapsed = 0;
            }
        }
    }
}