using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Database;
using RystBrewery.Software.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;



namespace RystBrewery.Software.ViewModels
{
    internal class RystSommerØlViewModel : INotifyPropertyChanged
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

        private string _selectedBrewingProgram { get; set; } = string.Empty;
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

        private string _selectedWashingProgram = string.Empty;
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

        private string _currentBrewingStepDescription = string.Empty;
        public string CurrentBrewingStepDescription
        {
            get => _currentBrewingStepDescription;
            set
            {
                _currentBrewingStepDescription = value;
                OnPropertyChanged(nameof(CurrentBrewingStepDescription));
            }
        }
        private string _currentWashingStepDescription = string.Empty;
        public string CurrentWashingStepDescription
        {
            get => _currentWashingStepDescription;
            set
            {
                _currentWashingStepDescription = value;
                OnPropertyChanged(nameof(CurrentWashingStepDescription));
            }
        }

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

        private bool _isBrewingRunning;
        public bool IsBrewingRunning
        {
            get => _isBrewingRunning;
            set
            {
                if (_isBrewingRunning != value)
                {
                    _isBrewingRunning = value;
                    OnPropertyChanged(nameof(IsBrewingRunning));
                    OnPropertyChanged(nameof(CanStartBrewing));
                    OnPropertyChanged(nameof(CanStartWashing));
                }
            }
        }

        private bool _isWashingRunning;
        public bool IsWashingRunning
        {
            get => _isWashingRunning;
            set
            {
                if (_isWashingRunning != value)
                {
                    _isWashingRunning = value;
                    OnPropertyChanged(nameof(IsWashingRunning));
                    OnPropertyChanged(nameof(CanStartBrewing));
                    OnPropertyChanged(nameof(CanStartWashing));
                }
            }
        }

        public bool CanStartBrewing => !IsBrewingRunning && !IsWashingRunning && IsTankClean;
        public bool CanStartWashing => !IsBrewingRunning && !IsWashingRunning;

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

        private ISeries[] _washingSeries;
        public ISeries[] WashingSeries
        {
            get => _washingSeries;
            set
            {
                _washingSeries = value;
                OnPropertyChanged(nameof(WashingSeries));
            }
        }

        private ISeries[] _combinedSeries = Array.Empty<ISeries>();
        public ISeries[] CombinedSeries
        {
            get => _combinedSeries;
            set
            {
                _combinedSeries = value;
                OnPropertyChanged(nameof(CombinedSeries));
            }
        }

        public RystSommerØlViewModel(RystSommerølBrewingService brewingService, RystSommerølWashingService washingService)

        {
            _brewingRepo = new RecipeRepo();
            _washingRepo = new WashingRepo();
            _brewingService = brewingService;
            _washingService = washingService;

            _temperatureSeries = Array.Empty<ISeries>();
            _washingSeries = Array.Empty<ISeries>();


            ProgramBindings();
            ServiceEvents();
        }

        private void ProgramBindings()
        {
            var recipes = _brewingRepo.GetAllRecipes();
            foreach (var recipe in recipes)
                BrewingProgramOptions.Add(recipe.Name);

            var defaultRecipe = _brewingRepo.GetRecipeByName("Ryst Sommerøl");
            if (defaultRecipe != null)
                SelectedBrewingProgram = defaultRecipe.Name;

            var washPrograms = _washingRepo.GetAllWashPrograms();
            foreach (var wash in washPrograms)
                WashingProgramOptions.Add(wash.Name);

            var defaultWash = _washingRepo.GetWashProgramByName("Ryst Sommerøl Washing");
            if (defaultWash != null)
                SelectedWashingProgram = defaultWash.Name;

            UpdateCombinedSeries();
        }

        private void UpdateCombinedSeries()
        {
            CombinedSeries = new ISeries[]
            {
        new LineSeries<int>
        {
            Values = _brewingService.TemperatureValues,
            Name = "Brewing Temperature",
            Fill = null
        },
        new LineSeries<int>
        {
            Values = _brewingService.MaltValues,
            Name = "Malt",
            Fill = null
        },
        new LineSeries<int>
        {
            Values = _brewingService.JuniperValues,
            Name = "Juniper",
            Fill = null
        },
        new LineSeries<int>
        {
            Values = _washingService.TemperatureValues,
            Name = "Washing Temperature",
            Fill = null
        },
        new LineSeries<int>
        {
            Values = _washingService.RinseValues,
            Name = "Rinse Power",
            Fill = null
        },
        new LineSeries<int>
        {
            Values = _washingService.DetergentValues,
            Name = "Detergent",
            Fill = null
        },
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
                IsBrewingRunning = false;
                StatusChanged?.Invoke("Completed");
            };

            _washingService.WashingStepChanged += (msg) =>
            {
                CurrentWashingStepDescription = msg;
            };

            _washingService.IsCompleted += () =>
            {
                IsTankClean = true;
                IsWashingRunning = false;
                StatusChanged?.Invoke("Completed");
            };

            AlarmService.AlarmTriggered += OnAlarmTriggered;
        }


        public void ClearAllValues()
        {
            _washingService.TemperatureValues.Clear();
            _washingService.DetergentValues.Clear();
            _washingService.RinseValues.Clear();
            _brewingService.TemperatureValues.Clear();
            _brewingService.AppleJuiceValues.Clear();
            _brewingService.HopValues.Clear();
            _brewingService.JuniperValues.Clear();
            _brewingService.MaltValues.Clear();
        }

        public void StartBrewing()
        {
            ClearAllValues();
            CurrentWashingSteps.Clear();
            CurrentWashingStepDescription = string.Empty;

            var recipe = _brewingRepo.GetRecipeByName(SelectedBrewingProgram);
            if (recipe != null)
            {
                _brewingService.StartBrewing(recipe);
                IsTankClean = false;
                IsBrewingRunning = true;
                StatusChanged?.Invoke("Running");
            }
        }

        public void StartWashing()
        {
            ClearAllValues();
            CurrentBrewingSteps.Clear();
            CurrentBrewingStepDescription = string.Empty;

            var program = _washingRepo.GetWashProgramByName(SelectedWashingProgram);
            if (program != null)
            {
                _washingService.StartWashing(program);
                IsWashingRunning = true;
                StatusChanged?.Invoke("Running");
            }
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
            IsTankClean = false;
            IsWashingRunning = false;
            IsBrewingRunning = false;
        }

        private void OnAlarmTriggered()
        {
            StopAll();
            OnPropertyChanged(nameof(CanStartBrewing));
            OnPropertyChanged(nameof(CanStartWashing));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
