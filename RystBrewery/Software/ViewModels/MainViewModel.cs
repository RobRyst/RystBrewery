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

        public ISeries[] TemperatureSeries { get; set; }
        private double _currentTemperature = 60.0;
        private readonly ObservableCollection<double> _temperatureValues = new ObservableCollection<double>();
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
               new LineSeries<double>
               {
                   Values = _temperatureValues,
                   Name= "Temperature"
               }
            };

            AlarmService.AlarmTriggered += OnAlarmTriggered;
        }



        public void LoadBrewingSteps()
        {
            CurrentBrewingSteps.Clear();
            CurrentBrewingSteps.Clear();
            if (string.IsNullOrEmpty(SelectedProgram)) return;

            var recipe = _repo
                .GetAllRecipes()
                .FirstOrDefault(r => r.Name == SelectedProgram);

            if (recipe == null) return;
            foreach (var step in recipe.Steps)
                CurrentBrewingSteps.Add(step);
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

            _simulationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _simulationTimer.Tick += (sender, args) =>
            {
                try
                {
                    double temperatureChanges = _random.NextDouble() * 2 - 1;
                    _currentTemperature += temperatureChanges;
                    _temperatureValues.Add(_currentTemperature);
                    AlarmService.CheckTemperature(_currentTemperature, SelectedProgram, "Tank1");
                    _currentTemperature = Math.Clamp(_currentTemperature, 0.0, 100.0);
                }
                catch (Exception ex)
                {
                    _simulationTimer?.Stop();
                    Console.WriteLine($"Simulation failed and the process stopped: {ex.Message}");
                }
            };
            _simulationTimer?.Start();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
