using System;
using System.ComponentModel;
using MahApps.Metro.Controls;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ISelectedIndexManager _selectedIndexManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            _selectedIndexManager = (ISelectedIndexManager)DataContext;
            _selectedIndexManager.PropertyChanged += DataContextPropertyChanged;
        }

        private void DataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISelectedIndexManager.SelectedIndex))
            {
                var selected = HamburgerNav.SelectedItem;
                if (selected == null)
                {
                    selected = HamburgerNav.SelectedOptionsItem;
                }
                if (selected != null)
                {
                    HamburgerNav.Content = ((HamburgerMenuItem)selected).Tag;
                }
            }
        }

        private void HamburgerNav_ItemClick(object sender, ItemClickEventArgs e)
        {
            // set the content
            this.HamburgerNav.Content = e.ClickedItem;
            // close the pane
            this.HamburgerNav.IsPaneOpen = false;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _selectedIndexManager.PropertyChanged -= DataContextPropertyChanged;
        }
    }
}
