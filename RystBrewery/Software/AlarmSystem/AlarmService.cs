using Microsoft.Extensions.DependencyInjection;
using RystBrewery.Software.Services;
using System.IO;
using System.Windows;

namespace RystBrewery.Software.AlarmSystem
{
    public class AlarmService
    {

        private const int MaxTempThreshold = 80;
        private readonly string _alarmLogPath = "alarm_log.txt";
        private readonly string _allEventsPath = "all_events_log.txt";

        public event Action? AlarmTriggered;
        public event Action<string>? LogWritten;
        public event Action<string>? StatusChanged;

        public event Action<string>? ProcessLogged;
        public void CheckTemperature(double currentTemp, string selectedProgram, string tankName)
        {
            if (currentTemp >= MaxTempThreshold)
            {
                TriggerAlarm(selectedProgram, tankName, currentTemp);
            }
        }

        private void TriggerAlarm(string selectedProgram, string tank, double temp)
        {
            AlarmTriggered?.Invoke();
            StatusChanged?.Invoke("Error");
            MessageBox.Show("Error: Temperature went above 80°C. Process stopped.", "ALARM", MessageBoxButton.OK, MessageBoxImage.Error);
            LogAlarm(selectedProgram, tank, temp);
            StopAll();
        }

        private void StopAll()
        {
            try
            {
                var RystIpa = AppService.Services.GetRequiredService<RystIPABrewingService>();
                var RystEplecider = AppService.Services.GetRequiredService<RystEpleciderBrewingService>();
                var RystSommerØl = AppService.Services.GetRequiredService<RystSommerølBrewingService>();
                var RystIPAWashing = AppService.Services.GetRequiredService<RystIPAWashingService>();
                var RystEpleciderWashing = AppService.Services.GetRequiredService<RystEpleciderWashingService>();
                var RystSommerØlWashing = AppService.Services.GetRequiredService<RystSommerølWashingService>();

                if (RystIpa.IsRunning) RystIpa.StopBrewing();
                if (RystEplecider.IsRunning) RystEplecider.StopBrewing();
                if (RystSommerØl.IsRunning) RystSommerØl.StopBrewing();

                if (RystIPAWashing.IsRunning) RystIPAWashing.StopWashing();
                if (RystEpleciderWashing.IsRunning) RystEpleciderWashing.StopWashing();
                if (RystSommerØlWashing.IsRunning) RystSommerØlWashing.StopWashing();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to stop all processes: {ex.Message}");
            }
        }

        public void LogEvent(string message, string selectedProgram)
        {
            string logEntry = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss}: {selectedProgram}: {message}";
            try
            {
                File.AppendAllText(_allEventsPath, logEntry + Environment.NewLine);
                LogWritten?.Invoke(logEntry);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to write log: {ex.Message}");
            }
        }

        private void LogAlarm(string selectedProgram, string tank, double temp)
        {
            string logEntry = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss}: Alarm triggered for program '{selectedProgram}' on tank '{tank}' with temperature {temp}°C.";
            try
            {
                File.AppendAllText(_alarmLogPath, logEntry + Environment.NewLine);
                LogWritten?.Invoke(logEntry);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to write alarm log: {ex.Message}");
            }
        }

        public void LogProcessHistory(string selectedProgram)
        {
            var logEntry = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss}: {selectedProgram}";
            try
            {
                File.AppendAllText("process_history_log.txt", logEntry + Environment.NewLine);
                ProcessLogged?.Invoke(logEntry);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to write process history log: {ex.Message}");
            }
        }


        public void SetStatus(string status)
        {
            StatusChanged?.Invoke(status);
        }
    }
}