using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IIndexFilters : INotifyPropertyChanged
    {
        IDictionary<string, object> Args { get; }
        string ColumnFilter { get; set; }
        PaymentDateFilterOptions DateFilterOptionId { get; set; }
        string DateFilterOption { get; }
        DateTime FromDateFilter { get; set; }
        bool IsAnySet { get; }
        bool IsAssignedToMe { get; set; }
        bool IsDateFilterVisible { get; set; }
        bool IsLogged { get; set; }
        bool IsNotPaid { get; set; }
        DateTime ToDateFilter { get; set; }

        void MergeArgs(IDictionary<string, object> argsToMerge);
        IDictionary<string, object> MergedArgs(IDictionary<string, object> argsToMerge);

        void ApplyDefaults();

        void Clear();
    }
}