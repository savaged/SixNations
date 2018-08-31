using System;
using System.Windows;
using System.Windows.Input;

namespace SixNations.Desktop.Views
{
    /// <summary>
    /// Interaction logic for WallDialog.xaml
    /// </summary>
    public partial class WallDialog
    {
        public WallDialog()
        {
            InitializeComponent();
        }

        private void OnFullScreenExit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
