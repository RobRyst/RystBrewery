using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

internal class MainViewModel : INotifyPropertyChanged
{
    private readonly AlarmService _alarmService;

    private readonly RystIPABrewingService _ipaBrewing;
    private readonly RystEpleciderBrewingService _epleBrewing;
    private readonly RystSommerølBrewingService _sommerBrewing;

    private readonly RystIPAWashingService _ipaWashing;
    private readonly RystEpleciderWashingService _epleWashing;
    private readonly RystSommerølWashingService _sommerWashing;

    public ObservableCollection<string> ActiveProcesses { get; } = new();
    public ObservableCollection<string> LatestLogs { get; } = new();

    private string _statusLightLabel = "Stopped";
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

        _ipaBrewing = ipaBrewing;
        _epleBrewing = epleBrewing;
        _sommerBrewing = sommerBrewing;

        _ipaWashing = ipaWashing;
        _epleWashing = epleWashing;
        _sommerWashing = sommerWashing;

        _alarmService.StatusChanged += OnStatusChanged;

        LoadActiveProcesses();
        LoadLatestLogs();
    }

    private void OnStatusChanged(string status)
    {
        StatusLightLabel = status;
        OnPropertyChanged(nameof(StatusLightLabel));
    }

    public void LoadActiveProcesses()
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
    }

    public void LoadLatestLogs()
    {
        LatestLogs.Clear();

        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*_log.txt");
        foreach (var file in files)
        {
            var lastLines = File.ReadLines(file).Reverse().Take(5).Reverse();
            foreach (var line in lastLines)
                LatestLogs.Add($"{Path.GetFileName(file)}: {line}");
        }
    }

    public void Refresh()
    {
        LoadActiveProcesses();
        LoadLatestLogs();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
