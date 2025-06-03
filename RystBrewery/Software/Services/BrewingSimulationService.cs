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
using System.Windows.Threading;

namespace RystBrewery.Software.Services
{
        internal class BrewingSimulationService : IBrewingService
        {

        public AlarmService AlarmService { get; } = new AlarmService();
        public ObservableCollection<string> BrewingProgramOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> WashingProgramOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BrewingSteps> CurrentBrewingSteps { get; set; } = new ObservableCollection<BrewingSteps>();
        public ObservableCollection<WashingSteps> CurrentWashingSteps { get; set; } = new ObservableCollection<WashingSteps>();
        public event Action<string>? StatusChanged;

        public string SelectedBrewingProgram { get; set; }


        private Recipe? _currentRecipe;
        private int BrewingStepChanged = 0;
        private string _currentBrewingStepDescription;
        private int _stepTimeElapsed = 0;
        private bool _isTankClean = true;

        private readonly RecipeRepo _brewingRepo;

        public ISeries[] TemperatureSeries { get; set; }
        public ISeries[] MaltSeries { get; set; }
        private int _currentTemperature = 55;
        private int _currentMaltInGrams = 0;
        private readonly ObservableCollection<int> _temperatureValues = new ObservableCollection<int>();
        private readonly ObservableCollection<int> _maltValues = new ObservableCollection<int>();

        private DispatcherTimer? _brewingSimulationTimer;


        public BrewingSimulationService()
        {
            _brewingRepo = new RecipeRepo();
            var recipes = _brewingRepo.GetAllRecipes();
            _currentRecipe = _brewingRepo.GetRecipeByName("Ryst Eplecider"); ;

            if (_currentRecipe != null)
            {
                SelectedBrewingProgram = _currentRecipe.Name;
            }

            foreach (var recipe in recipes)
            {
                BrewingProgramOptions.Add(recipe.Name);
            }

            TemperatureSeries = new ISeries[]
            {
               new LineSeries<int>
               {
                   Values = _temperatureValues,
                   Name = "Temperature"
               },
               new LineSeries<int>
               {
                   Values = _maltValues,
                   Name = "Malt"
               }
            };

            AlarmService.AlarmTriggered += OnAlarmTriggered;
        }

        public bool IsTankClean
        {
            get => _isTankClean;
            set
            {
                if (_isTankClean != value)
                {
                    _isTankClean = value;
                    OnPropertyChanged(nameof(IsTankClean));
                    OnPropertyChanged(nameof(CanStartBrewing));
                }
            }
        }

        public bool CanStartBrewing => IsTankClean;

        public void StopBrewing()
        {
            _brewingSimulationTimer?.Stop();

        }

        private void OnAlarmTriggered()
        {
            StopBrewing();
        }

        public string CurrentBrewingStepDescription
        {
            get => _currentBrewingStepDescription;
            set
            {
                _currentBrewingStepDescription = value;
                OnPropertyChanged(nameof(CurrentBrewingStepDescription));
            }
        }


        public void LoadBrewingSteps()
        {
            CurrentBrewingSteps.Clear();
            if (string.IsNullOrEmpty(SelectedBrewingProgram))
                return;

            var recipe = _brewingRepo
                .GetAllRecipes()
                .FirstOrDefault(recipe => recipe.Name == SelectedBrewingProgram);

            if (recipe == null)
                return;

            foreach (var brewingStep in recipe.Steps)
                CurrentBrewingSteps.Add(brewingStep);
        }

        public void StartBrewing()
        {
            if (_brewingSimulationTimer != null && _brewingSimulationTimer.IsEnabled)
                return;

            _currentRecipe = _brewingRepo
                .GetAllRecipes()
                .FirstOrDefault(recipe => recipe.Name == SelectedBrewingProgram);

            if (_currentRecipe == null || _currentRecipe.Steps.Count == 0)
                return;

            _currentBrewingStepIndex = 0;
            _stepTimeElapsed = 0;

            _brewingSimulationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _brewingSimulationTimer.Tick += BrewingSimulationTick;
            _brewingSimulationTimer?.Start();
        }

        private void BrewingSimulationTick(object? sender, EventArgs e)
        {
            try
            {
                if (_currentRecipe == null || _currentBrewingStepIndex >= _currentRecipe.Steps.Count)
                {
                    CurrentBrewingStepDescription = "Brewing complete.";
                    StatusChanged?.Invoke("Completed");
                    _brewingSimulationTimer?.Stop();
                    _isTankClean = false;
                    return;
                }

                var step = _currentRecipe.Steps[_currentBrewingStepIndex];
                CurrentBrewingStepDescription = $"Step {_currentBrewingStepIndex + 1}/{_currentRecipe.Steps.Count}: {step.Description} ({step.Time} sec)";

                if (step.Description.Contains("Varm opp", StringComparison.OrdinalIgnoreCase))
                {
                    if (_currentTemperature <= 65)
                    {
                        _currentTemperature += 2;
                        if (_currentTemperature >= 65)
                            _currentTemperature = 65;
                    }
                }

                if (step.Description.Contains("Tilsett malt", StringComparison.OrdinalIgnoreCase))
                {
                    if (_currentTemperature >= 65)
                    {
                        _currentMaltInGrams = 50;
                    }
                }

                _temperatureValues.Add(_currentTemperature);
                _maltValues.Add(_currentMaltInGrams);
                AlarmService.CheckTemperature(_currentTemperature, SelectedBrewingProgram, "Ryst Tank");

                _stepTimeElapsed++;

                if (_stepTimeElapsed >= step.Time)
                {
                    _currentBrewingStepIndex++;
                    _stepTimeElapsed = 0;
                }
            }
            catch (Exception ex)
            {
                _brewingSimulationTimer?.Stop();
                CurrentBrewingStepDescription = $"Simulation error: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

