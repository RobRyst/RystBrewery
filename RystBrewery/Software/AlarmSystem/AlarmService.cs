using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace RystBrewery.Software.AlarmSystem
{
    class AlarmService
    {
        private const double MaxTempThreshold = 80;
        private readonly string _alarmLogPath = "alarm_log.txt";
        private readonly string _allEventsPath = "all_events_log.txt";

        public event Action AlarmTriggered;
        public event Action<string>? StatusChanged;

        public void CheckTemperature(double currentTemp, string selectedProgram, string tankName)
        {
            if (currentTemp >= MaxTempThreshold)
            {
                TriggerAlarm(selectedProgram, tankName, currentTemp);
            }
        }

        private void TriggerAlarm(string program, string tank, double temp)
        {
            AlarmTriggered?.Invoke();
            StatusChanged?.Invoke("Error");

            MessageBox.Show("Error: Temperature went above 80°C. Process stopped.", "ALARM", MessageBoxButton.OK, MessageBoxImage.Error);
            LogAlarm(program, tank, temp);

        }

        public void LogEvent(string message, string program)
        {
            string logEntry = $"{DateTime.Now:u}: {message}";
            File.AppendAllText(_allEventsPath, logEntry + Environment.NewLine);
        }

        private void LogAlarm(string program, string tank, double temp)
        {
            string logEntry = $"{DateTime.Now:u}: Alarm triggered for program '{program}' on tank '{tank}' with temperature {temp}°C.";
            File.AppendAllText(_alarmLogPath, logEntry + Environment.NewLine);
        }
    }
}
