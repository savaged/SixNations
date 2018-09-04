using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using SixNations.Desktop.Models;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Constants;
using System.Collections.ObjectModel;

namespace SixNations.Desktop.ViewModels
{
    public class SwimlaneViewModel : ViewModelBase
    {
        private readonly RequirementStatus _requirementStatus;

        public SwimlaneViewModel(RequirementStatus requirementStatus)
        {
            _requirementStatus = requirementStatus;
            Index = new ObservableCollection<Requirement>();
            RequirementDragHandler = new RequirementDragHandler();
            RequirementDragHandler.DroppedHandler += OnDropped;
        }

        public ObservableCollection<Requirement> Index { get; }

        public RequirementDragHandler RequirementDragHandler { get; }

        public void DragOver(IDropInfo di)
        {
            if (di != null && di.Data != null && di.TargetItem != null)
            {
                di.DropTargetAdorner = DropTargetAdorners.Highlight;
                di.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo di)
        {
            var source = (Requirement)di.Data;
            if (source.Status != (int)_requirementStatus && !Index.Contains(source))
            {
                source.Status = (int)_requirementStatus;
                Index.Add(source);
            }
        }

        private void OnDropped(object sender, DropEventArgs e)
        {
            var source = (Requirement)e.DropInfo.Data;
            if (source.Status != (int)_requirementStatus && Index.Contains(source))
            {
                Index.Remove(source);
            }
        }
    }
}
