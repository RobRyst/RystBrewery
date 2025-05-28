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
        public string SelectedProgram { get; set; }

        private Recipe? _currentRecipe;
        private int _currentStepIndex = 0;
        private int _stepTimeElapsed = 0;
        private string _currentStepDescription;

        public ISeries[] TemperatureSeries { get; set; }
        private int _currentTemperature = 53;
        private readonly ObservableCollection<int> _temperatureValues = new ObservableCollection<int>();
        private readonly RecipeRepo _repo;

        private DispatcherTimer? _simulationTimer;
        private readonly Random _random = new Random();

        public MainViewModel()
        {
            _repo = new RecipeRepo();
            var recipes = _repo.GetAllRecipes();

            foreach (var recipe in recipes)
            {
                ProgramOptions.Add(recipe.Name);
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

        public string CurrentStepDescription
        {
            get => _currentStepDescription;
            set
            {
                _currentStepDescription = value;
                OnPropertyChanged(nameof(CurrentStepDescription));
            }
        }



        public void LoadBrewingSteps()
        {
            CurrentBrewingSteps.Clear();
            if (string.IsNullOrEmpty(SelectedProgram)) return;

            var recipe = _repo
                .GetAllRecipes()
                .FirstOrDefault(recipe => recipe.Name == SelectedProgram);

            if (recipe == null) return;
            foreach (var brewingStep in recipe.Steps)
                CurrentBrewingSteps.Add(brewingStep);
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

            _currentRecipe = _repo
                .GetAllRecipes()
                .FirstOrDefault(recipe => recipe.Name == SelectedProgram);

            if (_currentRecipe == null || _currentRecipe.Steps.Count == 0)
                return;

            _currentStepIndex = 0;
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
                    AlarmService.CheckTemperature(_currentTemperature, SelectedProgram, "Ryst IPA Tank");
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
                if (_currentRecipe == null || _currentStepIndex >= _currentRecipe.Steps.Count)
                {
                    CurrentStepDescription = "Brewing complete.";
                    _simulationTimer?.Stop();
                    return;
                }

                var step = _currentRecipe.Steps[_currentStepIndex];
                CurrentStepDescription = $"Step {_currentStepIndex + 1}/{_currentRecipe.Steps.Count}: {step.Description} ({step.Time} sec)";

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
                AlarmService.CheckTemperature(_currentTemperature, SelectedProgram, "Ryst Tank");

                _stepTimeElapsed++;

                if (_stepTimeElapsed >= step.Time)
                {
                    _currentStepIndex++;
                    _stepTimeElapsed = 0;
                }
            }
            catch (Exception ex)
            {
                _simulationTimer?.Stop();
                CurrentStepDescription = $"Simulation error: {ex.Message}";
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
