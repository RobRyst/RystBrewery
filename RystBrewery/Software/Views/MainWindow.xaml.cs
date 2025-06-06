using RystBrewery.Software.Database;
using RystBrewery.Software.Views;
using System.Windows;
using System.Windows.Controls;

namespace RystBrewery
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PasswordSettings.Password();
            MainContentFrame.Navigate(new Dashboard());
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new Dashboard());
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
    }
}
