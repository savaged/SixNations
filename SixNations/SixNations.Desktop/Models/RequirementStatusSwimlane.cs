using System.Windows;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Constants;

namespace SixNations.Desktop.Models
{
    public class RequirementStatusSwimlane : IDropTarget
    {
        private readonly RequirementStatus _requirementStatus;

        public RequirementStatusSwimlane(RequirementStatus requirementStatus)
        {
            _requirementStatus = requirementStatus;
            Index = new ObservableCollection<Requirement>();
            RequirementDragHandler = new RequirementDragHandler();
            RequirementDragHandler.DroppedHandler += OnDropped;
        }

        private void OnDropped(object sender, DropEventArgs e)
        {
            var source = (Requirement)e.DropInfo.Data;
            Index.Remove(source);
        }

        public ObservableCollection<Requirement> Index { get; }

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
            source.Status = (int)_requirementStatus;
            Index.Add(source);
        }

        public RequirementDragHandler RequirementDragHandler { get; }
    }
}
