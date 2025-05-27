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



namespace RystBrewery.Software.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly AlarmService _alarmService = new();
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

            _alarmService.AlarmTriggered += OnAlarmTriggered;
        }

        private void OnAlarmTriggered()
        {
            _simTimer?.Stop();
        }

        public void StartTemperatureSimulation()
        {
            if (_simTimer != null && _simTimer.IsEnabled)
                return;

            _simTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _simTimer.Tick += (s, e) =>
            {
                double randomIncrease = 2 +- _random.NextDouble();
                _currentTemp += randomIncrease;
                _tempValues.Add(_currentTemp);
                _alarmService.CheckTemperature(_currentTemp, SelectedProgram, "Tank1");
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
