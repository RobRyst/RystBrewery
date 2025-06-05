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
using RystBrewery.Software.ViewModels;
using RystBrewery.Software.AlarmSystem;
using System.Windows.Threading;
using RystBrewery.Software.Views;
using RystBrewery.Software.Database;


namespace RystBrewery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            PasswordSettings.Password();

            _vm = new MainViewModel();
            DataContext = _vm;

            _vm.AlarmService.StatusChanged += (status) =>
            {
                Dispatcher.Invoke(() => UpdateLampStatus(status));
            };
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = null;
        }
        private void TankOneBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new RystIPA());
        }
        private void TankTwoBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new RystEpleCider());
        }
        private void TankThreeBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new RystSommerØl());
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

        private void Stop_Click(object sender, RoutedEventArgs e) {
            _vm.StopSimulation();
            UpdateLampStatus("Stopped");
        }
    }
}