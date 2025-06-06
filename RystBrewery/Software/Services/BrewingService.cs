using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Database;
using RystBrewery.Software.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RystBrewery.Software.Services
{
    internal class BrewingService : IBrewingService
    {
        public AlarmService AlarmService { get; } = new AlarmService();
        public ObservableCollection<string> BrewingProgramOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> WashingProgramOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BrewingSteps> CurrentBrewingSteps { get; set; } = new ObservableCollection<BrewingSteps>();
        public ObservableCollection<WashingSteps> CurrentWashingSteps { get; set; } = new ObservableCollection<WashingSteps>();

        private readonly ObservableCollection<int> _rinseValues = new ObservableCollection<int>();
        public ObservableCollection<int> RinseValues => _rinseValues;

        private readonly ObservableCollection<int> _temperatureValues = new ObservableCollection<int>();
        public ObservableCollection<int> TemperatureValues => _temperatureValues;

        private readonly ObservableCollection<int> _maltValues = new ObservableCollection<int>();
        public ObservableCollection<int> MaltValues => _maltValues;

        private readonly ObservableCollection<int> _detergentValues = new ObservableCollection<int>();
        public ObservableCollection<int> DetergentValues => _detergentValues;

        private readonly ObservableCollection<int> _appleJuiceValues = new ObservableCollection<int>();
        public ObservableCollection<int> AppleJuiceValues => _appleJuiceValues;

        private readonly ObservableCollection<int> _hopValues = new ObservableCollection<int>();
        public ObservableCollection<int> HopValues => _hopValues;

        private readonly ObservableCollection<int> _juniperValues = new ObservableCollection<int>();
        public ObservableCollection<int> JuniperValues => _juniperValues;

        public event Action<string>? BrewingStepChanged;
        public event Action IsCompleted;

        public string SelectedBrewingProgram { get; set; }
        private Recipe? _recipe;
        private int _brewingStepIndex;
        private int _stepTimeElapsed = 0;
        private DispatcherTimer? _brewingTimer;

        public ISeries[] TemperatureSeries { get; set; }
        public ISeries[] MaltSeries { get; set; }
        public ISeries[] HopSeries { get; set; }
        public ISeries[] AppleJuiceSeries { get; set; }
        public ISeries[] JuniperSeries { get; set; }

        private int _currentTemperature = 55;
        private int _currentMalt = 0;
        private int _currentAppleJuice = 0;
        private int _currentHop = 0;
        private int _currentJuniper = 0;

        public bool IsRunning => _brewingTimer?.IsEnabled == true;

        public string SelectedWashingProgram { get; private set; }

        public BrewingService()
        {
            _brewingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _brewingTimer.Tick += BrewingTick;
            InitializeChartData();
        }

        private void InitializeChartData()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _temperatureValues.Add(_currentTemperature);
                _maltValues.Add(_currentMalt);
                _appleJuiceValues.Add(_currentAppleJuice);
                _hopValues.Add(_currentHop);
                _juniperValues.Add(_currentJuniper);
            });
        }


        public void StartBrewing(Recipe recipe)
        {
            if (IsRunning)
                return;

            SelectedBrewingProgram = recipe.Name;
            _recipe = recipe;
            _brewingStepIndex = 0;
            _stepTimeElapsed = 0;


            _temperatureValues.Clear();
            _detergentValues.Clear();
            _maltValues.Clear();
            _rinseValues.Clear();
            _appleJuiceValues.Clear();
            _hopValues.Clear();
            _juniperValues.Clear();
            InitializeChartData();

            _brewingTimer.Start();
        }

        public void StopBrewing()
        {
            _brewingTimer?.Stop();
        }

        private void BrewingTick(object sender, EventArgs e)
        {
            if (_recipe == null || _brewingStepIndex >= _recipe.Steps.Count)
            {
                BrewingStepChanged?.Invoke("Brewing Complete");
                IsCompleted?.Invoke();
                _brewingTimer.Stop();
                return;
            }

            var step = _recipe.Steps[_brewingStepIndex];
            int remainingTime = step.Time - _stepTimeElapsed;
            BrewingStepChanged?.Invoke($"Step {_brewingStepIndex + 1}/{_recipe.Steps.Count}: {step.Description} - {remainingTime}s remaining");

            if (step.Description.Contains("Varm opp", StringComparison.OrdinalIgnoreCase))
            {
                _currentTemperature = Math.Min(_currentTemperature + 2, 65);
            }

            if (step.Description.Contains("Tilsett Malt og Einer", StringComparison.OrdinalIgnoreCase))
            {
                _currentMalt = 50;
                _currentJuniper = 20;
                AlarmService.LogEvent("Adding Malt and Juniper", SelectedBrewingProgram);
            }

            if (step.Description.Contains("Tilsett Malt og Humle", StringComparison.OrdinalIgnoreCase))
            {
                _currentMalt = 70;
                _currentHop = 30;
                AlarmService.LogEvent($"{SelectedBrewingProgram}Adding Malt: {_currentMalt}g and {_currentHop}g", SelectedBrewingProgram);
            }

            if (step.Description.Contains("Tilsett Eple Juice", StringComparison.OrdinalIgnoreCase))
            {
                _currentAppleJuice = 50;
                AlarmService.LogEvent("Adding Apple Juice", SelectedBrewingProgram);
            }

            _temperatureValues.Add(_currentTemperature);
            _maltValues.Add(_currentMalt);
            _hopValues.Add(_currentHop);
            _appleJuiceValues.Add(_currentAppleJuice);
            _juniperValues.Add(_currentJuniper);

            _stepTimeElapsed++;
            if (_stepTimeElapsed >= step.Time)
            {
                System.Diagnostics.Debug.WriteLine($"Completed step: {step.Description}");
                _brewingStepIndex++;
                _stepTimeElapsed = 0;
            }
        }
    }
}
