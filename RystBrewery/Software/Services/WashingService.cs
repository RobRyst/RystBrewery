using Microsoft.Extensions.DependencyInjection;
using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Database;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace RystBrewery.Software.Services
{
    internal class WashingService : IWashingService
    {
        public event Action<string> WashingStepChanged = delegate { };
        public event Action IsCompleted = delegate { };
        private readonly AlarmService _alarmService;

        public string SelectedWashingProgram { get; set; } = string.Empty;

        private readonly ObservableCollection<int> _temperatureValues = new();
        public ObservableCollection<int> TemperatureValues => _temperatureValues;
        private int _currentTemperature = 55;

        private readonly ObservableCollection<int> _rinseValues = new();
        public ObservableCollection<int> RinseValues => _rinseValues;
        private int _currentRinsePower = 0;

        private readonly ObservableCollection<int> _detergentValues = new();
        public ObservableCollection<int> DetergentValues => _detergentValues;
        private int _currentDetergentValues = 0;

        private readonly ObservableCollection<int> _maltValues = new();
        public ObservableCollection<int> MaltValues => _maltValues;
        private int _currentMaltValues = 0;

        private DispatcherTimer _washingTimer;
        private WashProgram? _washProgram;
        private int _washingStepIndex;
        private int _stepTimeElapsed;

        private bool _loggedTargetRinse = false;
        private bool _loggedTargetDetergent = false;
        private bool _loggedTargetHeavyRinse = false;

        public bool IsRunning => _washingTimer?.IsEnabled == true;

        public WashingService(AlarmService alarmService)
        {
            _alarmService = alarmService ?? throw new ArgumentNullException(nameof(alarmService));
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
            SelectedWashingProgram = program.Name;
            _washingStepIndex = 0;
            _stepTimeElapsed = 0;

            _alarmService.LogEvent($"Starting washing process with program: {program.Name}", SelectedWashingProgram);
            _alarmService.SetStatus("Running");

            _washingTimer.Start();
            InitializeChartData();
        }

        public void ClearAllValues()
        {
            _temperatureValues.Clear();
            _detergentValues.Clear();
            _maltValues.Clear();
            _rinseValues.Clear();
        }

        public void StopWashing()
        {
            if (IsRunning)
            {
                _alarmService.LogEvent("Washing process stopped manually", SelectedWashingProgram);
                _alarmService.SetStatus("Stopped");
            }
            _washingTimer.Stop();
        }

        private void WashingTick(object? sender, EventArgs e)
        {
            if (_washProgram == null || _washingStepIndex >= _washProgram.Steps.Count)
            {
                WashingStepChanged?.Invoke("Washing complete.");
                IsCompleted?.Invoke();
                _washingTimer.Stop();
                _alarmService.LogEvent("Washing process completed successfully", SelectedWashingProgram);
                _alarmService.LogProcessHistory(SelectedWashingProgram);
                _alarmService.CheckTemperature(_currentTemperature, SelectedWashingProgram, "Washing Tank");
                AppService.Services.GetRequiredService<MainViewModel>().UpdateGlobalStatus();
                return;
            }

            var step = _washProgram.Steps[_washingStepIndex];
            int remaining = step.Time - _stepTimeElapsed;
            WashingStepChanged?.Invoke($"Step {_washingStepIndex + 1}/{_washProgram.Steps.Count}: {step.Description} - {remaining}s remaining");

            if (step.Description.Contains("Tømmer og renser tank", StringComparison.OrdinalIgnoreCase))
            {
                int prevTemp = _currentTemperature;
                int prevRinse = _currentRinsePower;

                _currentTemperature = Math.Min(_currentTemperature + 30, 60);
                _currentRinsePower = Math.Min(_currentRinsePower + 50, 100);

                if (_currentTemperature > prevTemp || _currentRinsePower > prevRinse)
                {
                    _alarmService.LogEvent($" - Rinsing in progress. Temp: {_currentTemperature}°C / 60°C, Rinse Power: {_currentRinsePower} / 100", SelectedWashingProgram);
                }
                else if (!_loggedTargetRinse)
                {
                    _alarmService.LogEvent($" - Target Temperature and Rinse Power reached: 60°C / 60°C, 100 / 100", SelectedWashingProgram);
                    _loggedTargetRinse = true;
                }
            }

            if (step.Description.Contains("Vaskemiddel tilsettes", StringComparison.OrdinalIgnoreCase))
            {
                int prevDetergent = _currentDetergentValues;

                _currentDetergentValues = Math.Min(_currentDetergentValues + 20, 60);
                _currentRinsePower = 0;

                if (_currentDetergentValues > prevDetergent)
                {
                   _alarmService.LogEvent($" - Water turned off, Adding Detergent: {_currentDetergentValues} / 60", SelectedWashingProgram);
                }
                else if (!_loggedTargetDetergent)
                {
                    _alarmService.LogEvent($" - Target amount of Detergent reached: 60 / 60", SelectedWashingProgram);
                    _loggedTargetDetergent = true;
                }
            }

            if (step.Description.Contains("Renser tank for vaskemiddel", StringComparison.OrdinalIgnoreCase))
            {
                int prevRinse = _currentRinsePower;

                _currentRinsePower = Math.Min(_currentRinsePower + 50, 150);
                _currentDetergentValues = Math.Max(_currentDetergentValues - 30, 0);

                if (_currentRinsePower > prevRinse)
                {
                    _alarmService.LogEvent($" - Heavy rinse in progress. Rinse Power: {_currentRinsePower} / 150 and detergant value left: {_currentDetergentValues}dl", SelectedWashingProgram);
                }
                else if (!_loggedTargetHeavyRinse)
                {
                    _alarmService.LogEvent($" - Target heavy Rinse Power reached: {_currentRinsePower} / 150 and no detergant left: {_currentDetergentValues}dl ", SelectedWashingProgram);
                    _loggedTargetHeavyRinse = true;
                }
            }

            _temperatureValues.Add(_currentTemperature);
            _detergentValues.Add(_currentDetergentValues);
            _maltValues.Add(_currentMaltValues);
            _rinseValues.Add(_currentRinsePower);

            _stepTimeElapsed++;
            if (_stepTimeElapsed >= step.Time)
            {
                System.Diagnostics.Debug.WriteLine($"Completed step: {step.Description}");
                _washingStepIndex++;
                _stepTimeElapsed = 0;
            }
        }
    }
}