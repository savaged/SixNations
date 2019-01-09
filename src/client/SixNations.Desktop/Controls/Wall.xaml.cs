using SixNations.Desktop.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SixNations.Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Wall.xaml
    /// </summary>
    public partial class Wall : UserControl
    {
        public Wall()
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
