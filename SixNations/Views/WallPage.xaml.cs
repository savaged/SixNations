using System;

using SixNations.ViewModels;

using Windows.UI.Xaml.Controls;

namespace SixNations.Views
{
    public sealed partial class WallPage : Page
    {
        private WallViewModel ViewModel
        {
            get { return DataContext as WallViewModel; }
        }

        public WallPage()
        {
            InitializeComponent();
        }
    }
}
