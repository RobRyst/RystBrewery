using RystBrewery.Software.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RystBrewery.Software.Services
{
    internal class WashingService : IWashingService
    {
        public event Action<string> WashingStepChanged;
        public event Action IsCompleted;

        private readonly ObservableCollection<int> _temperatureValues = new ObservableCollection<int>();
        public ObservableCollection<int> TemperatureValues => _temperatureValues;
        private int _currentTemperature = 55;

        private readonly ObservableCollection<int> _rinseValues = new ObservableCollection<int>();
        public ObservableCollection<int> RinseValues => _rinseValues;
        private int _currentRinsePower = 0;

        private readonly ObservableCollection<int> _detergentValues = new ObservableCollection<int>();
        public ObservableCollection<int> DetergentValues => _detergentValues;
        private int _currentDetergentValues = 0;

        private readonly ObservableCollection<int> _maltValues = new ObservableCollection<int>();
        public ObservableCollection<int> MaltValues => _maltValues;
        private int _currentMaltValues = 0;

        private DispatcherTimer _washingTimer;
        private WashProgram _washProgram;
        private int _washingStepIndex;
        private int _stepTimeElapsed;

        public bool IsRunning => _washingTimer?.IsEnabled == true;

        public WashingService()
        {
            _washingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _washingTimer.Tick += WashingTick;
            InitializeChartData();
        }

        private void InitializeChartData()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _temperatureValues.Add(_currentTemperature);
                _detergentValues.Add(_currentDetergentValues);
                _maltValues.Add(_currentMaltValues);
                _rinseValues.Add(_currentRinsePower);
            });
        }

        public void StartWashing(WashProgram program)
        {
            if (IsRunning) return;


            _washProgram = program;
            _washingStepIndex = 0;
            _stepTimeElapsed = 0;

            _temperatureValues.Clear();
            _detergentValues.Clear();
            _maltValues.Clear();
            _rinseValues.Clear();

            InitializeChartData();

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

            var step = _washProgram.Steps[_washingStepIndex];
            WashingStepChanged?.Invoke($"Step {_washingStepIndex + 1}/{_washProgram.Steps.Count}: {step.Description} ({step.Time}s)");

            ProcessWashingStep(step);
            Application.Current.Dispatcher.Invoke(() =>
            {
                _temperatureValues.Add(_currentTemperature);
                _detergentValues.Add(_currentDetergentValues);
                _maltValues.Add(_currentMaltValues);
                _rinseValues.Add(_currentRinsePower);
            });

            _stepTimeElapsed++;
            if (_stepTimeElapsed >= step.Time)
            {
                _washingStepIndex++;
                _stepTimeElapsed = 0;
            }
        }

        private void ProcessWashingStep(WashingSteps step)
        {
            if (_stepTimeElapsed == 0)
            {
                if (step.Description.Contains("Tømmer og renser tank", StringComparison.OrdinalIgnoreCase))
                {
                    _currentTemperature = 65;
                    _currentRinsePower = Math.Min(_currentTemperature + 35, 100);
                    _currentMaltValues = Math.Max(_currentMaltValues - 10, 0);
                    _currentDetergentValues = 0;
                }
                else if (step.Description.Contains("Vaskemiddel tilsettes", StringComparison.OrdinalIgnoreCase) ||
                         step.Description.Contains("Vaskemiddel", StringComparison.OrdinalIgnoreCase))
                {
                        _currentDetergentValues = 100;
                        _currentRinsePower = 0;
                }
            }

            if (step.Description.Contains("Renser tank for vaskemiddel", StringComparison.OrdinalIgnoreCase))
            {
                _currentRinsePower = Math.Min(_currentTemperature + 35, 100);
                _currentDetergentValues = Math.Max(_currentDetergentValues - 10, 0);
            }
        }
    }
}