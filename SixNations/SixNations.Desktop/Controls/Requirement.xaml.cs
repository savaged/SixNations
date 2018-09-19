using SixNations.Desktop.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var i = e.Item as Data.Models.Requirement;
            if (i != null)
            {
                if (string.IsNullOrEmpty(StoryFilter.Text))
                {
                    e.Accepted = true;
                }
                else if (i.Story.Contains(StoryFilter.Text))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void StoryFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(DgRequirements.ItemsSource).Refresh();
        }
    }
}
