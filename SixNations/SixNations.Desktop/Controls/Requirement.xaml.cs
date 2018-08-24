using SixNations.Desktop.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SixNations.Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Requirement.xaml
    /// </summary>
    public partial class Requirement : UserControl
    {
        public Requirement()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (IAsyncViewModel)DataContext;
            await vm.LoadAsync();
        }
    }
}
