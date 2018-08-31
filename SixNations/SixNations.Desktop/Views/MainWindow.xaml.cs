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
            var shellVM = (IShellViewModel)DataContext;
            _selectedIndexManager = shellVM.SelectedIndexManager;
            _selectedIndexManager.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = HamburgerNav.SelectedItem;
            if (selected == null)
            {
                selected = HamburgerNav.SelectedOptionsItem;
            }
            if (selected != null)
            {
                HamburgerNav.Content = (HamburgerMenuItem)selected;
                HamburgerNav.IsPaneOpen = false;
            }
        }

        private void HamburgerNav_ItemClick(object sender, ItemClickEventArgs e)
        {
            HamburgerNav.Content = e.ClickedItem;
            HamburgerNav.IsPaneOpen = false;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _selectedIndexManager.SelectedIndexChanged -= OnSelectedIndexChanged;
        }
    }
}
