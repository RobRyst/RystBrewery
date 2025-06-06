using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Threading;

internal class MainViewModel : INotifyPropertyChanged
{
    private readonly AlarmService _alarmService;
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly RystIPABrewingService _ipaBrewing;
    private readonly RystEpleciderBrewingService _epleBrewing;
    private readonly RystSommerølBrewingService _sommerBrewing;

    private readonly RystIPAWashingService _ipaWashing;
    private readonly RystEpleciderWashingService _epleWashing;
    private readonly RystSommerølWashingService _sommerWashing;
    private readonly DispatcherTimer _uiUpdateTimer = new DispatcherTimer();



    public ObservableCollection<string> ProcessHistory { get; } = new();
    public ObservableCollection<string> ActiveProcesses { get; } = new();
    public ObservableCollection<string> LatestLogs { get; } = new();

    private string _statusLightLabel = "Idle";
    public string StatusLightLabel
    {
        get => _statusLightLabel;
        set
        {
            _statusLightLabel = value;
            OnPropertyChanged(nameof(StatusLightLabel));
        }
    }

    public MainViewModel(
        AlarmService alarmService,
        RystIPABrewingService ipaBrewing,
        RystEpleciderBrewingService epleBrewing,
        RystSommerølBrewingService sommerBrewing,
        RystIPAWashingService ipaWashing,
        RystEpleciderWashingService epleWashing,
        RystSommerølWashingService sommerWashing)
    {
        _alarmService = alarmService;

        _alarmService.LogWritten += OnLogWritten;
        _alarmService.StatusChanged += OnStatusChanged;
        _alarmService.ProcessLogged += OnProcessLogged;

        _uiUpdateTimer.Interval = TimeSpan.FromSeconds(1);
        _uiUpdateTimer.Tick += (s, e) => LoadActiveProcesses();
        _uiUpdateTimer.Start();

        _ipaBrewing = ipaBrewing;

        _epleBrewing = epleBrewing;
        _sommerBrewing = sommerBrewing;

        _ipaWashing = ipaWashing;
        _epleWashing = epleWashing;
        _sommerWashing = sommerWashing;

        LoadActiveProcesses();
        LoadLatestLogs();
        LoadProcessHistory();

        _alarmService.LogEvent("System initialized", "SYSTEM");
    }

    private void OnStatusChanged(string status)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            StatusLightLabel = status;
        });
    }

    private void OnProcessLogged(string entry)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            ProcessHistory.Add(entry);
            while (ProcessHistory.Count > 100)
                ProcessHistory.RemoveAt(0);
        });
    }

    public void UpdateGlobalStatus()
    {
        if (_ipaBrewing.IsRunning || _epleBrewing.IsRunning || _sommerBrewing.IsRunning ||
            _ipaWashing.IsRunning || _epleWashing.IsRunning || _sommerWashing.IsRunning)
        {
            _alarmService.SetStatus("Running");
        }
        else
        {
            _alarmService.SetStatus("Completed");
        }
    }

    public void LoadActiveProcesses()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            ActiveProcesses.Clear();

            if (_ipaBrewing.IsRunning) ActiveProcesses.Add("Ryst IPA Brewing");
            if (_epleBrewing.IsRunning) ActiveProcesses.Add("Ryst EpleCider Brewing");
            if (_sommerBrewing.IsRunning) ActiveProcesses.Add("Ryst Sommerøl Brewing");

            if (_ipaWashing.IsRunning) ActiveProcesses.Add("Ryst IPA Washing");
            if (_epleWashing.IsRunning) ActiveProcesses.Add("Ryst EpleCider Washing");
            if (_sommerWashing.IsRunning) ActiveProcesses.Add("Ryst Sommerøl Washing");

            if (ActiveProcesses.Count == 0)
                ActiveProcesses.Add("No active processes");
        });
    }

    private void LoadProcessHistory()
    {
        try
        {
            if (File.Exists("process_history_log.txt"))
            {
                var lines = File.ReadLines("process_history_log.txt").Reverse().Take(100).Reverse();
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        ProcessHistory.Add(line);
                }
            }
        }
        catch (Exception ex)
        {
            ProcessHistory.Add($"Error loading process history: {ex.Message}");
        }
    }

    public void LoadLatestLogs()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            LatestLogs.Clear();

            try
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*_log.txt");
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        var lastLines = File.ReadLines(file).Reverse().Take(5).Reverse();
                        foreach (var line in lastLines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                                LatestLogs.Add($"{Path.GetFileName(file)}: {line}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LatestLogs.Add($"Error loading logs: {ex.Message}");
            }
        });
    }

    public void Refresh()
    {
        LoadActiveProcesses();
        LoadLatestLogs();
    }

    private void OnLogWritten(string logLine)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            while (LatestLogs.Count >= 50)
                LatestLogs.RemoveAt(0);

            LatestLogs.Add(logLine);
        });
    }
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}