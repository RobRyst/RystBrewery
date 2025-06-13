using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly AlarmService _alarmService;

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
        public event Action IsCompleted = delegate { };

        public string SelectedBrewingProgram { get; set; } = string.Empty;
        private Recipe? _recipe;
        private int _brewingStepIndex;
        private int _stepTimeElapsed = 0;
        private DispatcherTimer? _brewingTimer;

        public required ISeries[] TemperatureSeries { get; set; }
        public required ISeries[] MaltSeries { get; set; }
        public required ISeries[] HopSeries { get; set; }
        public required ISeries[] AppleJuiceSeries { get; set; }
        public required ISeries[] JuniperSeries { get; set; }

        private int _currentTemperature = 0;
        private int _currentMalt = 0;
        private int _currentAppleJuice = 0;
        private int _currentHop = 0;
        private int _currentJuniper = 0;
        private bool _loggedCooking = false;

        public bool IsRunning => _brewingTimer?.IsEnabled == true;

        public string SelectedWashingProgram { get; set; } = string.Empty;


        public BrewingService(AlarmService alarmService)
        {
            _alarmService = alarmService ?? throw new ArgumentNullException(nameof(alarmService));
            _brewingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _brewingTimer.Tick += BrewingTick;
            _alarmService.SetStatus("Brewing");
            InitializeChartData();
            _alarmService.LogEvent("BrewingService initialized", "BREWING_SERVICE");
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

        public void ClearAllValues()
        {
            _temperatureValues.Clear();
            _detergentValues.Clear();
            _maltValues.Clear();
            _rinseValues.Clear();
            _appleJuiceValues.Clear();
            _hopValues.Clear();
            _juniperValues.Clear();
        }

        public void StartBrewing(Recipe recipe)
        {
            if (IsRunning)
            {
                _alarmService.LogEvent("Brewing already in progress - ignoring start request", "BREWING_SERVICE");
                return;
            }

            SelectedBrewingProgram = recipe.Name;
            _recipe = recipe;
            _brewingStepIndex = 0;
            _stepTimeElapsed = 0;
            _loggedCooking = false;

            ClearAllValues();
            InitializeChartData();

            _brewingTimer?.Start();
            _alarmService.LogEvent($"Started brewing: {recipe.Name}", "BREWING_SERVICE");
            _alarmService.SetStatus("Running");
        }

        public void StopBrewing()
        {
            if (IsRunning)
            {
                _brewingTimer?.Stop();
                _alarmService.LogEvent($"Stopped brewing: {SelectedBrewingProgram}", "BREWING_SERVICE");
                _alarmService.SetStatus("Stopped");
            }
        }

        private void BrewingTick(object? sender, EventArgs e)
        {
            if (_recipe == null || _brewingStepIndex >= _recipe.Steps.Count)
            {
                BrewingStepChanged?.Invoke(" - Brewing Complete");
                IsCompleted?.Invoke();
                _brewingTimer?.Stop();
                _alarmService.LogEvent($"Brewing completed: {SelectedBrewingProgram}", "BREWING_SERVICE");
                AppService.Services.GetRequiredService<MainViewModel>().UpdateGlobalStatus();
                _alarmService.LogProcessHistory(SelectedBrewingProgram);
                return;
            }

            var step = _recipe.Steps[_brewingStepIndex];
            int remainingTime = step.Time - _stepTimeElapsed;
            BrewingStepChanged?.Invoke($"Step {_brewingStepIndex + 1}/{_recipe.Steps.Count}: {step.Description} - {remainingTime}s remaining");


            _alarmService.CheckTemperature(_currentTemperature, SelectedBrewingProgram, "Main Tank");

            if (step.Description.Contains("Varm opp", StringComparison.OrdinalIgnoreCase))
            {
                int prevTemperature = _currentTemperature;
                _currentTemperature = Math.Min(_currentTemperature + 30, 60);

                if (_currentTemperature > prevTemperature)
                {
                    if (_currentTemperature < 60)
                    {
                        _alarmService.LogEvent($"Heating in progress: {_currentTemperature}℃ / 60℃", SelectedBrewingProgram);
                    }
                    else
                    {
                        _alarmService.LogEvent($"Target temperature reached: 60℃", SelectedBrewingProgram);
                    }
                }
            }

            if (step.Description.Contains("Tilsett Malt og Einer", StringComparison.OrdinalIgnoreCase))
            {
                int prevMalt = _currentMalt;
                int prevJuniper = _currentJuniper;

                _currentMalt = Math.Min(_currentMalt + 20, 80);
                _currentJuniper = Math.Min(_currentJuniper + 20, 40);

                if (_currentMalt > prevMalt || _currentJuniper > prevJuniper)
                {
                    _alarmService.LogEvent($"Adding - Malt: {_currentMalt}g / 80g, Juniper: {_currentJuniper}g / 40g", SelectedBrewingProgram);
                }
                else
                {
                    _alarmService.LogEvent($"Target reached - Malt: {_currentMalt}g / 80g, Juniper: {_currentJuniper}g / 40g", SelectedBrewingProgram);
                }
            }

            if (step.Description.Contains("Tilsett Malt og Humle", StringComparison.OrdinalIgnoreCase))
            {
                int prevMalt = _currentMalt;
                int prevHop = _currentHop;

                _currentMalt = Math.Min(_currentMalt + 20, 80);
                _currentHop = Math.Min(_currentHop + 20, 40);

                if (_currentMalt > prevMalt || _currentHop > prevHop)
                {
                    _alarmService.LogEvent($"Adding - Malt: {_currentMalt}g / 80g, Hop: {_currentHop}g / 40g", SelectedBrewingProgram);
                }
                else
                {
                    _alarmService.LogEvent($"Target reached - Malt: {_currentMalt}g / 80g, Hop: {_currentHop}g / 40g", SelectedBrewingProgram);
                }
            }

            if (step.Description.Contains("Tilsett Eple Juice", StringComparison.OrdinalIgnoreCase))
            {
                int prevAppleJuice = _currentAppleJuice;
                _currentAppleJuice = Math.Min(_currentAppleJuice + 40, 80);

                if (_currentAppleJuice > prevAppleJuice)
                {
                    if (_currentAppleJuice < 80)
                    {
                        _alarmService.LogEvent($"Adding Apple Juice: {_currentAppleJuice}dl / 80dl", SelectedBrewingProgram);
                    }
                    else
                    {
                        _alarmService.LogEvent($"Target Apple Juice added: {_currentAppleJuice}dl / 80dl", SelectedBrewingProgram);
                    }
                }
            }

            if (step.Description.Contains("Kok i 15 min", StringComparison.OrdinalIgnoreCase) && !_loggedCooking)
            {
                _alarmService.LogEvent($"Cooking for 15min", SelectedBrewingProgram);
                _loggedCooking = true;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                _temperatureValues.Add(_currentTemperature);
                _maltValues.Add(_currentMalt);
                _hopValues.Add(_currentHop);
                _appleJuiceValues.Add(_currentAppleJuice);
                _juniperValues.Add(_currentJuniper);
            });

            _stepTimeElapsed++;
            if (_stepTimeElapsed >= step.Time)
            {
                _alarmService.LogEvent($"Completed step: {step.Description}", SelectedBrewingProgram);
                _brewingStepIndex++;
                _stepTimeElapsed = 0;
            }
        }
    }
}