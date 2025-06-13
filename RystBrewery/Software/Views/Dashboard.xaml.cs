using Microsoft.Extensions.DependencyInjection;
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

            _vm.LatestLogs.CollectionChanged += (s, e) =>
            {
                if (_vm.LatestLogs.Count > 0)
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        LogsListBox.ScrollIntoView(_vm.LatestLogs[^1]);
                    });
                }
            };
        }
    }
}