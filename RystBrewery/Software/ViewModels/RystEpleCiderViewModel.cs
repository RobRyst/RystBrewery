using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using RystBrewery.Software;
using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Database;
using RystBrewery.Software.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;



namespace RystBrewery.Software.ViewModels
{
    internal class RystEpleCiderViewModel : INotifyPropertyChanged
    {

        private readonly RecipeRepo _brewingRepo;
        private readonly WashingRepo _washingRepo;
        private readonly IBrewingService _brewingService;
        private readonly IWashingService _washingService;

        public AlarmService AlarmService { get; } = new AlarmService();

        public ObservableCollection<string> BrewingProgramOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> WashingProgramOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BrewingSteps> CurrentBrewingSteps { get; set; } = new ObservableCollection<BrewingSteps>();
        public ObservableCollection<WashingSteps> CurrentWashingSteps { get; set; } = new ObservableCollection<WashingSteps>();
        public event Action<string>? StatusChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _selectedBrewingProgram { get; set; }
        public string SelectedBrewingProgram
        {
            get => _selectedBrewingProgram;
            set
            {
                _selectedBrewingProgram = value;
                OnPropertyChanged(nameof(SelectedBrewingProgram));
                LoadBrewingSteps();
            }
        }

        private string _selectedWashingProgram;
        public string SelectedWashingProgram
        {
            get => _selectedWashingProgram;
            set
            {
                _selectedWashingProgram = value;
                OnPropertyChanged(nameof(SelectedWashingProgram));
                LoadWashingSteps();
            }
        }


        private Recipe? _currentRecipe;
        private string _currentBrewingStepDescription;
        public string CurrentBrewingStepDescription
        {
            get => _currentBrewingStepDescription;
            set
            {
                _currentBrewingStepDescription = value;
                OnPropertyChanged(nameof(CurrentBrewingStepDescription));
            }
        }

        private WashProgram? _currentWashProgram;
        private string _currentWashingStepDescription;
        public string CurrentWashingStepDescription
        {
            get => _currentWashingStepDescription;
            set
            {
                _currentWashingStepDescription = value;
                OnPropertyChanged(nameof(CurrentWashingStepDescription));
            }
        }


        private int _stepTimeElapsed = 0;

        private bool _isTankClean = true;
        public bool IsTankClean
        {
            get => _isTankClean;
            set
            {
                _isTankClean = value;
                OnPropertyChanged(nameof(IsTankClean));
                OnPropertyChanged(nameof(CanStartBrewing));
            }
        }

        public bool CanStartBrewing => IsTankClean;

        private ISeries[] _temperatureSeries;
        public ISeries[] TemperatureSeries
        {
            get => _temperatureSeries;
            set
            {
                _temperatureSeries = value;
                OnPropertyChanged(nameof(TemperatureSeries));
            }
        }

        private readonly ObservableCollection<int> _temperatureValues = new ObservableCollection<int>();
        private readonly ObservableCollection<int> _maltValues = new ObservableCollection<int>();


        public RystEpleCiderViewModel(IBrewingService brewingService, IWashingService washingService)
        {
            _brewingRepo = new RecipeRepo();
            _washingRepo = new WashingRepo();
            _brewingService = brewingService;
            _washingService = washingService;


            ProgramBindings();
            ServiceEvents();
        }

        private void ProgramBindings()
        {
            var recipes = _brewingRepo.GetAllRecipes();
            foreach (var recipe in recipes)
                BrewingProgramOptions.Add(recipe.Name);

            var defaultRecipe = _brewingRepo.GetRecipeByName("Ryst Eplecider");
            if (defaultRecipe != null)
                SelectedBrewingProgram = defaultRecipe.Name;

            var washPrograms = _washingRepo.GetAllWashPrograms();
            foreach (var wash in washPrograms)
                WashingProgramOptions.Add(wash.Name);

            var defaultWash = _washingRepo.GetWashProgramByName("Ryst Eplecider Washing");
            if (defaultWash != null)
                SelectedWashingProgram = defaultWash.Name;

            TemperatureSeries = new ISeries[]
                {
                     new LineSeries<int> { Values = _brewingService.TemperatureValues, Name = "Temperature" },
                     new LineSeries<int> { Values = _brewingService.MaltValues, Name = "Malt" }
                };
        }

        private void ServiceEvents()
        {
            _brewingService.BrewingStepChanged += (msg) =>
            {
                CurrentBrewingStepDescription = msg;
            };

            _brewingService.IsCompleted += () =>
            {
                IsTankClean = false;
                StatusChanged?.Invoke("Completed");
            };

            _washingService.WashingStepChanged += (msg) =>
            {
                CurrentWashingStepDescription = msg;
            };

            _washingService.IsCompleted += () =>
            {
                IsTankClean = true;
                StatusChanged?.Invoke("Completed");
            };

            AlarmService.AlarmTriggered += OnAlarmTriggered;
        }

        public void StartBrewing()
        {
            var recipe = _brewingRepo.GetRecipeByName(SelectedBrewingProgram);
            if (recipe != null)
                _brewingService.StartBrewing(recipe);
        }

        public void StartWashing()
        {
            var program = _washingRepo.GetWashProgramByName(SelectedWashingProgram);
            if (program != null)
                _washingService.StartWashing(program);
        }

        public void LoadBrewingSteps()
        {
            CurrentBrewingSteps.Clear();
            var recipe = _brewingRepo.GetRecipeByName(SelectedBrewingProgram);
            if (recipe == null) return;

            foreach (var step in recipe.Steps)
                CurrentBrewingSteps.Add(step);
        }

        public void LoadWashingSteps()
        {
            CurrentWashingSteps.Clear();
            var program = _washingRepo.GetWashProgramByName(SelectedWashingProgram);
            if (program == null) return;

            foreach (var step in program.Steps)
                CurrentWashingSteps.Add(step);
        }

        public void StopAll()
        {
            _brewingService.StopBrewing();
            _washingService.StopWashing();
        }

        private void OnAlarmTriggered()
        {
            StopAll();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
