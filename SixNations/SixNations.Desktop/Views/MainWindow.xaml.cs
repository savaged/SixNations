using System;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private IShellViewModel _shell;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            _shell = (IShellViewModel)DataContext;
            _shell.SelectedIndexManager.SelectedIndexChanged += OnSelectedIndexChanged;
            _shell.IsFullScreenChanged += OnIsFullScreenChanged;
        }

        private void OnIsFullScreenChanged(object sender, IIsFullScreenChangedEventArgs e)
        {
            // TODO hide / unhide window and task bar entry
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
            _shell.SelectedIndexManager.SelectedIndexChanged -= OnSelectedIndexChanged;
            _shell.IsFullScreenChanged -= OnIsFullScreenChanged;
        }
    }
}
