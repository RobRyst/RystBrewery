using Microsoft.Extensions.DependencyInjection;
using RystBrewery.Software.ViewModels;
using System.Windows.Controls;

namespace RystBrewery.Software.Views
{
    public partial class Dashboard : UserControl
    {
        private readonly MainViewModel _vm;

        public Dashboard()
        {
            InitializeComponent();
            _vm = AppService.Services.GetRequiredService<MainViewModel>();
            DataContext = _vm;
            _vm.Refresh();
        }
    }
}