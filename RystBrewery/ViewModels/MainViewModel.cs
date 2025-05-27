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
using System.Collections.ObjectModel;
using System.Windows.Threading;


namespace RystBrewery.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ProgramOptions { get; set; } = new() { "Brygg IPA", "Brygg Pilsner", "VaskeProgram" };
        public string SelectedProgram { get; set; }

        public ISeries[] TempSeries { get; set; }
        private double _currentTemp = 30.0;
        private readonly ObservableCollection<double> _tempValues = new();

        private DispatcherTimer? _simTimer;
        private readonly Random _random = new();

        public MainViewModel()
        {
           TempSeries = new ISeries[] 
           { 
               new LineSeries<double>
               {
                   Values = _tempValues,
                   Name= "Temperature"
               }
           };
        }

        public void StartTemperatureSimulation()
        {
            if (_simTimer != null && _simTimer.IsEnabled)
                return;

            _simTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _simTimer.Tick += (s, e) =>
            {
                double randomIncrease = 0.5 +- _random.NextDouble();
                _currentTemp += randomIncrease;
                _tempValues.Add(_currentTemp);
                _tempValues.Add(_currentTemp);
                OnPropertyChanged(nameof(TempSeries));
            };
            _simTimer.Start();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
