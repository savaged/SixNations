namespace SixNations.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HamburgerNav_ItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs e)
        {
            // set the content
            this.HamburgerNav.Content = e.ClickedItem;
            // close the pane
            this.HamburgerNav.IsPaneOpen = false;

        }
    }
}
