using System.Windows;

namespace RystBrewery.Software.Views
{
    public partial class PasswordPopUp : Window
    {
        public string InputPassword { get; set; } = string.Empty;

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
