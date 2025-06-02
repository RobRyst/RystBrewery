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
            _vm = new RystIPAViewModel();
            DataContext = _vm;
        }
    }
}