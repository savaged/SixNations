using System.Windows;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IDragAndDropable
    {
        void OnDrop(object sender, DragEventArgs e);
    }
}