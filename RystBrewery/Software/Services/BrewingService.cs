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
        private int _currentRinsePower = 0;

        private readonly ObservableCollection<int> _detergentValues = new ObservableCollection<int>();
        public ObservableCollection<int> DetergentValues => _detergentValues;
        private int _currentDetergentValues = 0;

        public event Action<string>? BrewingStepChanged;
        public event Action IsCompleted;

        public string SelectedBrewingProgram { get; set; }
        private Recipe? _recipe;
        private int _brewingStepIndex;
        private string _currentBrewingStepDescription;
        private int _stepTimeElapsed = 0;
        private DispatcherTimer? _brewingTimer;
        private bool _isTankClean = true;
        private readonly RecipeRepo _brewingRepo;

        public ISeries[] TemperatureSeries { get; set; }
        public ISeries[] MaltSeries { get; set; }

        private int _currentTemperature = 55;
        private int _currentMaltInGrams = 0;
        private readonly ObservableCollection<int> _temperatureValues = new ObservableCollection<int>();
        private readonly ObservableCollection<int> _maltValues = new ObservableCollection<int>();

        public ObservableCollection<int> TemperatureValues => _temperatureValues;
        public ObservableCollection<int> MaltValues => _maltValues;

        public bool IsRunning => _brewingTimer?.IsEnabled == true;

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
                _maltValues.Add(_currentMaltInGrams);
            });
        }

        public void StartBrewing(Recipe recipe)
        {
            if (IsRunning)
                return;

            _recipe = recipe;
            _brewingStepIndex = 0;
            _stepTimeElapsed = 0;


            _temperatureValues.Clear();
            _detergentValues.Clear();
            _maltValues.Clear();
            _rinseValues.Clear();
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


            if ((step.Description.Contains("Tilsett malt", StringComparison.OrdinalIgnoreCase) ||
                 step.Description.Contains("Tilsett", StringComparison.OrdinalIgnoreCase)) &&
                 _currentTemperature >= 65)
            {
                _currentMaltInGrams = 50;
            }

            _temperatureValues.Add(_currentTemperature);
            _maltValues.Add(_currentMaltInGrams);


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