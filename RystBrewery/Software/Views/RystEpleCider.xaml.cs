using RystBrewery.Software.AlarmSystem;
using RystBrewery.Software.Database;
using RystBrewery.Software.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace RystBrewery.Software.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RystEpleCider : UserControl
    {
        private readonly RystEpleCiderViewModel _vm;
        public RystEpleCider()
        {
            InitializeComponent();
            _vm = new RystEpleCiderViewModel();
            DataContext = _vm;

            _vm.AlarmService.StatusChanged += (status) =>
            {
                Dispatcher.Invoke(() => UpdateLampStatus(status));
            };
        }

        public void UpdateLampStatus(string status)
        {
            switch (status)
            {
                case "Running":
                    StatusLight.Fill = Brushes.Yellow;
                    break;

                case "Completed":
                    StatusLight.Fill = Brushes.Green;
                    break;

                case "Paused":
                    StatusLight.Fill = Brushes.Black;
                    break;

                case "Stopped":
                    StatusLight.Fill = Brushes.Blue;
                    break;

                case "Error":
                    StatusLight.Fill = Brushes.Red;
                    break;
            }
        }

        private void Start_Brewing_Click(object sender, RoutedEventArgs e)
        {

            var popup = new PasswordPopUp();
            if (popup.ShowDialog() == true)
            {
                string inputPassword = popup.InputPassword;

                var authenticatePassword = new PasswordAuth();
                if (!authenticatePassword.AuthPassword(inputPassword))
                {
                    MessageBox.Show("Incorrect Password. Access Denied");
                    return;
                }
            }

            if (!_vm.CanStartBrewing)
            {
                MessageBox.Show("You must clean the tank before brewing");
                return;
            }

            if (string.IsNullOrEmpty(_vm.SelectedBrewingProgram))
            {
                MessageBox.Show("Select a program to run");
                return;
            }
            MessageBox.Show($"Starter program: {_vm.SelectedBrewingProgram}");
            _vm.StartBrewingSimulation();
            UpdateLampStatus("Running");
        }

        private void Start_Washing_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_vm.SelectedWashingProgram))
            {
                MessageBox.Show("Select a program to run");
                return;
            }

            MessageBox.Show($"Starter program: {_vm.SelectedWashingProgram}");
            _vm.StartWashingSimulation();
            UpdateLampStatus("Running");
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            _vm.StopSimulation();
            UpdateLampStatus("Paused");
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _vm.StopSimulation();
            UpdateLampStatus("Stopped");
        }
    }
}
