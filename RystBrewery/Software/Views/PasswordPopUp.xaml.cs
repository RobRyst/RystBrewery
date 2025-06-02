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

namespace RystBrewery.Software.Views
{
    public partial class PasswordPopUp : Window
    {
        public string InputPassword { get; set; }

        public PasswordPopUp()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            InputPassword = PasswordInput.Password;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
