using RystBrewery.Software.Database;
using RystBrewery.Software.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;


namespace RystBrewery.Software.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RystIPA : UserControl
    {
        private readonly RystIPAViewModel _vm;
        public RystIPA()
        {
            InitializeComponent();
            _vm = AppService.Services.GetRequiredService<RystIPAViewModel>();
            DataContext = _vm;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.StatusChanged += Vm_StatusChanged;
            UpdateLampStatusFromCurrentState();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _vm.StatusChanged -= Vm_StatusChanged;
        }

        private void Vm_StatusChanged(string status)
        {
            Dispatcher.Invoke(() => UpdateLampStatus(status));
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

        private void UpdateLampStatusFromCurrentState()
        {
            if (_vm == null)
                return;

            if (_vm.IsBrewingRunning || _vm.IsWashingRunning)
            {
                UpdateLampStatus("Running");
            }
            else if (!_vm.IsTankClean)
            {
                UpdateLampStatus("Completed");
            }
            else
            {
                UpdateLampStatus("Stopped");
            }
        }

        private void Start_Brewing_Click(object sender, RoutedEventArgs e)
        {
            var popup = new PasswordPopUp();
            bool? dialogResult = popup.ShowDialog();

            if (dialogResult != true) return;

            string inputPassword = popup.InputPassword;
            var authenticatePassword = new PasswordAuth();

            if (!authenticatePassword.AuthPassword(inputPassword))
            {
                MessageBox.Show("Incorrect Password. Access Denied");
                return;
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
            _vm.StartBrewing();
            UpdateLampStatus("Running");
        }

        private void Start_Washing_Click(object sender, RoutedEventArgs e)
        {
            var popup = new PasswordPopUp();
            bool? dialogResult = popup.ShowDialog();

            if (dialogResult != true) return;

            string inputPassword = popup.InputPassword;
            var authenticatePassword = new PasswordAuth();

            if (!authenticatePassword.AuthPassword(inputPassword))
            {
                MessageBox.Show("Incorrect Password. Access Denied");
                return;
            }

            if (string.IsNullOrEmpty(_vm.SelectedWashingProgram))
            {
                MessageBox.Show("Select a program to run");
                return;
            }

            MessageBox.Show($"Starter program: {_vm.SelectedWashingProgram}");
            _vm.StartWashing();
            UpdateLampStatus("Running");
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            _vm.StopAll();
            UpdateLampStatus("Paused");
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _vm.StopAll();
            UpdateLampStatus("Stopped");
        }
    }
}
