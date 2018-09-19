using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using SixNations.Desktop.Interfaces;
using SixNations.API.Constants;

namespace SixNations.Desktop.Controls
{
    /// <summary>
    /// Interaction logic for Swimlane.xaml
    /// </summary>
    public partial class Swimlane : UserControl
    {
        public Swimlane()
        {
            InitializeComponent();
        }

        private void PostIt_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock postIt && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(postIt, postIt.Tag.ToString(), DragDropEffects.Move);
            }
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                var tag = (string)e.Data.GetData(DataFormats.StringFormat);
                var success = int.TryParse(tag, out int id);
                if (success)
                {
                    e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                }
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    var tag = (string)e.Data.GetData(DataFormats.StringFormat);
                    var success = int.TryParse(tag, out int id);
                    if (success)
                    {
                        var vm = DataContext as ISwimlaneDropTarget;
                        vm.OnDrop(id, (RequirementStatus)LvRequirements.Tag);
                    }
                }
            }
        }
    }
}
