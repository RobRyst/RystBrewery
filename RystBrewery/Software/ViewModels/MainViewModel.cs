using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using System.Windows.Threading;
using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Database;



namespace RystBrewery.Software.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public AlarmService AlarmService { get; } = new AlarmService();
        public ObservableCollection<string> ProgramOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BrewingSteps> CurrentBrewingSteps { get; set; } = new ObservableCollection<BrewingSteps>();
        public string SelectedBrewingProgram { get; set; }
        public string SelectedWashingProgram { get; set; }


        private Recipe? _currentRecipe;
        private int _currentBrewingStepIndex = 0;
        private string _currentBrewingStepDescription;

        private WashProgram? _currentWashProgram;
        private int _currentWashingStepIndex = 0;
        private string _currentWashingStepDescription;
        //Testing multiple uses of _stepTimeElapsed - Otherwise change to BrewingStepTimeElapsed. 
        private int _stepTimeElapsed = 0;

        public ObservableCollection<WashingSteps> CurrentWashingSteps { get; set; } = new ObservableCollection<WashingSteps>();
        private readonly WashingRepo _washingRepo;



        public ISeries[] TemperatureSeries { get; set; }
        private int _currentTemperature = 53;
        private readonly ObservableCollection<int> _temperatureValues = new ObservableCollection<int>();
        private readonly RecipeRepo _brewingRepo;

        private DispatcherTimer? _simulationTimer;
        private readonly Random _random = new Random();

        public MainViewModel()
        {
            _brewingRepo = new RecipeRepo();
            var recipes = _brewingRepo.GetAllRecipes();

            foreach (var recipe in recipes)
            {
                ProgramOptions.Add(recipe.Name);
            }

            _washingRepo = new WashingRepo();
            var washPrograms = _washingRepo.GetAllWashPrograms();

            foreach (var washProgram in washPrograms)
            {
                ProgramOptions.Add(washProgram.Name);
            }


            TemperatureSeries = new ISeries[]
            {
               new LineSeries<int>
               {
                   Values = _temperatureValues,
                   Name= "Temperature"
               }
            };

            AlarmService.AlarmTriggered += OnAlarmTriggered;
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

        public string CurrectWashingStepDescription
        {
            get => _currentWashingStepDescription;
            set
            {
                _currentWashingStepDescription = value;
                OnPropertyChanged(nameof(CurrectWashingStepDescription));
            }
        }


        public void LoadBrewingSteps()
        {
            CurrentBrewingSteps.Clear();
            if (string.IsNullOrEmpty(SelectedBrewingProgram)) return;

            var recipe = _brewingRepo
                .GetAllRecipes()
                .FirstOrDefault(recipe => recipe.Name == SelectedBrewingProgram);

            if (recipe == null) return;
            foreach (var brewingStep in recipe.Steps)
                CurrentBrewingSteps.Add(brewingStep);
        }

        public void LoadWashingSteps()
        {
            CurrentWashingSteps.Clear();
            if (string.IsNullOrEmpty(SelectedWashingProgram)) return;

            var washProgram = _washingRepo
                .GetAllWashPrograms()
                .FirstOrDefault(washProgram => washProgram.Name == SelectedWashingProgram);

            if (washProgram == null) return;
            foreach (var washingSteps in washProgram.Steps)
                CurrentWashingSteps.Add(washingSteps);
        }

        public void StopSimulation()
        {
            _simulationTimer?.Stop();
        }

        private void OnAlarmTriggered()
        {
            StopSimulation();
        }

        public void StartTemperatureSimulation()
        {
            if (_simulationTimer != null && _simulationTimer.IsEnabled)
                return;

            _currentRecipe = _brewingRepo
                .GetAllRecipes()
                .FirstOrDefault(recipe => recipe.Name == SelectedBrewingProgram);

            if (_currentRecipe == null || _currentRecipe.Steps.Count == 0)
                return;

            _currentBrewingStepIndex = 0;
            _stepTimeElapsed = 0;

            _simulationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _simulationTimer.Tick += SimulationTick;
            {
                try
                {
                    if (_currentTemperature < 65) {
                        //double temperatureChange = _random.NextDouble() * 2 - 1;
                        //int temperatureChanges = _currentTemperature += 2;
                        //_currentTemperature += temperatureChanges;
                        _currentTemperature += 2;


                        if (_currentTemperature == 65)
                        {
                            _currentTemperature = 65;
                        }
                    }
                    _temperatureValues.Add(_currentTemperature);
                    AlarmService.CheckTemperature(_currentTemperature, SelectedBrewingProgram, "Ryst IPA Tank");
                    //_currentTemperature = Math.Clamp(_currentTemperature, 0, 100);
                }
                catch (Exception ex)
                {
                    _simulationTimer?.Stop();
                    Console.WriteLine($"Simulation failed and the process stopped: {ex.Message}");
                }
            };
            _simulationTimer?.Start();
        }

        private void SimulationTick(object? sender, EventArgs e)
        {
            try
            {
                if (_currentRecipe == null || _currentBrewingStepIndex >= _currentRecipe.Steps.Count)
                {
                    CurrentBrewingStepDescription = "Brewing complete.";
                    _simulationTimer?.Stop();
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

                _temperatureValues.Add(_currentTemperature);
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
                _simulationTimer?.Stop();
                _currentBrewingStepDescription = $"Simulation error: {ex.Message}";
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
