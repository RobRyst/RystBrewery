using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WPF;
using RystBrewery.Software;
using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Database;
using RystBrewery.Software.Services;
using SkiaSharp;
using SkiaSharp;
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

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                OnPropertyChanged(nameof(XAxes));
            }
        }

        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                OnPropertyChanged(nameof(YAxes));
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

        private ISeries[] _combinedSeries;
        public ISeries[] CombinedSeries
        {
            get => _combinedSeries;
            set
            {
                _combinedSeries = value;
                OnPropertyChanged(nameof(CombinedSeries));
            }
        }




        public RystEpleCiderViewModel(RystEpleciderBrewingService brewingService, RystEpleciderWashingService washingService)

        {

            var whitePaint = new SolidColorPaint(SKColors.White);
            _brewingRepo = new RecipeRepo();
            _washingRepo = new WashingRepo();
            _brewingService = brewingService;
            _washingService = washingService;


            ProgramBindings();
            ServiceEvents();
            ConfigureAxes();
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
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 1 }
        },
        new LineSeries<int>
        {
            Values = _brewingService.AppleJuiceValues,
            Name = "AppleJuice",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightGreen) { StrokeThickness = 1 }
        },
        new LineSeries<int>
        {
            Values = _washingService.TemperatureValues,
            Name = "Washing Temperature",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Cyan) { StrokeThickness = 1 }
        },
        new LineSeries<int>
        {
            Values = _washingService.RinseValues,
            Name = "Rinse Power",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.LightBlue) { StrokeThickness = 1 }
        },
        new LineSeries<int>
        {
            Values = _washingService.DetergentValues,
            Name = "Detergent",
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Yellow) { StrokeThickness = 1 }
        }
            };
        }

        private void ConfigureAxes()
        {
            var whitePaint = new SolidColorPaint(SKColors.White);

            XAxes = new Axis[]
            {
    new Axis
    {
        Name = "Time",
        NamePaint = whitePaint,
        NamePadding = new LiveChartsCore.Drawing.Padding(30), // More padding
        LabelsPaint = whitePaint,
        SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
        TicksPaint = whitePaint,
        TextSize = 14,       // Size of tick labels
        NameTextSize = 16    // 👈 Size of axis title
    }
};

            YAxes = new Axis[]
            {
        new Axis
        {
            Name = "Values",
            NamePaint = whitePaint,
            LabelsPaint = whitePaint,
            SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
            TicksPaint = whitePaint,
            NamePadding = new LiveChartsCore.Drawing.Padding(15), // << important!
            TextSize = 14
        }
            };
        }


        private void ServiceEvents()
        {
            AlarmService.AlarmTriggered += OnAlarmTriggered;
            _brewingService.BrewingStepChanged += (msg) =>
            {
                CurrentBrewingStepDescription = msg;
            };

            _brewingService.IsCompleted += () =>
            {
                IsTankClean = false;
                IsBrewingRunning = false;
                StatusChanged?.Invoke("Completed");
                AlarmService.LogEvent(" - Brewing completed", SelectedBrewingProgram);
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
                AlarmService.LogEvent("Washing completed", SelectedWashingProgram);
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
                AlarmService.LogEvent("Started brewing", SelectedBrewingProgram);
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
                AlarmService.LogEvent("Started Washing", SelectedWashingProgram);
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

            IsBrewingRunning = false;
            IsWashingRunning = false;

            StatusChanged?.Invoke("Stopped");
            AlarmService.LogEvent("Stopped all processes", SelectedBrewingProgram ?? SelectedWashingProgram);
        }


        private void OnAlarmTriggered()
        {
            StopAll();
            OnPropertyChanged(nameof(CanStartBrewing));
            OnPropertyChanged(nameof(CanStartWashing));
            AlarmService.LogEvent("Process paused", SelectedBrewingProgram ?? SelectedWashingProgram ?? "Unknown Program");

        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
