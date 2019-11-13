using System;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Core;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class IndexFilters : ObservableObject, IIndexFilters
    {
        private readonly IDictionary<string, object> _args;
        private readonly IDictionary<string, object> _defaults;
        private bool _defaultsApplied;
        private bool _isDateFilterVisible;

        public IndexFilters()
        {
            _args = new Dictionary<string, object>();

            _defaults = new Dictionary<string, object>
            {
                { nameof(IsLogged), false },
                { nameof(IsNotPaid), false },
                { nameof(IsAssignedToMe), false },
                { nameof(ColumnFilter), string.Empty },
                { nameof(DateFilterOptionId), PaymentDateFilterOptions.None },
                { nameof(FromDateFilter), DateTime.MinValue },
                { nameof(ToDateFilter), DateTime.MinValue }
            };
        }

        public IDictionary<string, object> Args
        {
            get
            {
                var dateFilterOptionIdKey = nameof(DateFilterOptionId);
                if (_args.ContainsKey(dateFilterOptionIdKey))
                {
                    var dateFilterOptionIdValue = 
                        (PaymentDateFilterOptions)_args[dateFilterOptionIdKey];
                    var dateFilterOptionKvp = new KeyValuePair<string, object>(
                        nameof(DateFilterOption), 
                        dateFilterOptionIdValue.ToString());
                    _args.AddOrUpdate(dateFilterOptionKvp);
                }
                return _args;
            }
        }

        public void Clear()
        {
            _args.Clear();
            ApplyDefaults();
            RaisePropertyChanged(nameof(IsLogged));
            RaisePropertyChanged(nameof(IsNotPaid));
            RaisePropertyChanged(nameof(IsAssignedToMe));
            RaisePropertyChanged(nameof(ColumnFilter));
            RaisePropertyChanged(nameof(DateFilterOptionId));
            RaisePropertyChanged(nameof(DateFilterOption));
            RaisePropertyChanged(nameof(FromDateFilter));
            RaisePropertyChanged(nameof(ToDateFilter));
        }

        public void ApplyDefaults()
        {
            foreach (var kvp in _defaults)
            {
                _args.AddOrUpdate(kvp);
            }
            _defaultsApplied = true;
        }

        public void MergeArgs(IDictionary<string, object> argsToMerge)
        {
            var merged = MergedArgs(argsToMerge);
            _args.Clear();
            foreach (var kvp in merged)
            {
                _args.Add(kvp);
            }
        }

        public IDictionary<string, object> MergedArgs(
            IDictionary<string, object> argsToMerge)
        {
            if (!IsAnySet)
            {
                return argsToMerge;
            }
            IDictionary<string, object> merged = null;
            try
            {
                merged = argsToMerge.Union(_args)
                    .ToDictionary(d => d.Key, d => d.Value);
            }
            catch (ArgumentException ex) 
            when (ex.Message == 
                "An item with the same key has already been added.")
            {
                foreach (var kvp in argsToMerge)
                {
                    merged.AddOrUpdate(kvp);
                }
            }
            return merged;
        }

        public bool IsAnySet
        {
            get
            {
                bool value;
                if (_defaultsApplied)
                {
                    value = IsLogged
                        || IsNotPaid
                        || IsAssignedToMe
                        || !string.IsNullOrEmpty(ColumnFilter)
                        || DateFilterOptionId != PaymentDateFilterOptions.None
                        || FromDateFilter > DateTime.MinValue
                        || ToDateFilter > DateTime.MinValue
                        || _args.Count > _defaults.Count;
                }
                else
                {
                    value = _args.Count > 0;
                }
                return value;
            }
        }

        public bool IsLogged
        {
            get
            {
                var value = false;
                if (_args.Keys.Contains(nameof(IsLogged)))
                {
                    value = (bool)_args[nameof(IsLogged)];
                }
                return value;
            }
            set
            {
                if (_args.Keys.Contains(nameof(IsLogged)))
                {
                    _args[nameof(IsLogged)] = value;
                }
                else
                {
                    _args.Add(nameof(IsLogged), value);
                }
                RaisePropertyChanged();
            }
        }

        public bool IsNotPaid
        {
            get
            {
                var value = false;
                if (_args.Keys.Contains(nameof(IsNotPaid)))
                {
                    value = (bool)_args[nameof(IsNotPaid)];
                }
                return value;
            }
            set
            {
                if (_args.Keys.Contains(nameof(IsNotPaid)))
                {
                    _args[nameof(IsNotPaid)] = value;
                }
                else
                {
                    _args.Add(nameof(IsNotPaid), value);
                }
                RaisePropertyChanged();
            }
        }

        public bool IsAssignedToMe
        {
            get
            {
                var value = false;
                if (_args.Keys.Contains(nameof(IsAssignedToMe)))
                {
                    value = (bool)_args[nameof(IsAssignedToMe)];
                }
                return value;
            }
            set
            {
                if (_args.Keys.Contains(nameof(IsAssignedToMe)))
                {
                    _args[nameof(IsAssignedToMe)] = value;
                }
                else
                {
                    _args.Add(nameof(IsAssignedToMe), value);
                }
                RaisePropertyChanged();
            }
        }

        public string ColumnFilter
        {
            get
            {
                string value = null;
                if (_args.Keys.Contains(nameof(ColumnFilter)))
                {
                    value = _args[nameof(ColumnFilter)].ToString();
                }
                return value;
            }
            set
            {
                if (_args.Keys.Contains(nameof(ColumnFilter)))
                {
                    _args[nameof(ColumnFilter)] = value;
                }
                else
                {
                    _args.Add(nameof(ColumnFilter), value);
                }
                RaisePropertyChanged();
            }
        }

        public PaymentDateFilterOptions DateFilterOptionId
        {
            get
            {
                var value = PaymentDateFilterOptions.None;
                if (_args.Keys.Contains(nameof(DateFilterOptionId)))
                {
                    value = (PaymentDateFilterOptions)_args[nameof(DateFilterOptionId)];
                }
                return value;
            }
            set
            {
                if (_args.Keys.Contains(nameof(DateFilterOptionId)))
                {
                    _args[nameof(DateFilterOptionId)] = value;
                }
                else
                {
                    _args.Add(nameof(DateFilterOptionId), value);
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DateFilterOption));
                IsDateFilterVisible = value != 
                    PaymentDateFilterOptions.None;
            }
        }
        public string DateFilterOption => DateFilterOptionId.ToString();

        public DateTime FromDateFilter
        {
            get
            {
                var value = DateTime.MinValue;
                if (_args.Keys.Contains(nameof(FromDateFilter)))
                {
                    value = (DateTime)_args[nameof(FromDateFilter)];
                }
                return value;
            }
            set
            {
                if (_args[nameof(FromDateFilter)] == null ||
                    (DateTime)_args[nameof(FromDateFilter)] != value)
                {
                    if (_args.Keys.Contains(nameof(FromDateFilter)))
                    {
                        _args[nameof(FromDateFilter)] = value;
                    }
                    else
                    {
                        _args.Add(nameof(FromDateFilter), value);
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public DateTime ToDateFilter
        {
            get
            {
                var value = DateTime.MinValue;
                if (_args.Keys.Contains(nameof(ToDateFilter)))
                {
                    value = (DateTime)_args[nameof(ToDateFilter)];
                }
                return value;
            }
            set
            {
                if (_args[nameof(ToDateFilter)] == null ||
                    (DateTime)_args[nameof(ToDateFilter)] != value)
                {
                    if (_args.Keys.Contains(nameof(ToDateFilter)))
                    {
                        _args[nameof(ToDateFilter)] = value;
                    }
                    else
                    {
                        _args.Add(nameof(ToDateFilter), value);
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsDateFilterVisible
        {
            get => _isDateFilterVisible;
            set => Set(ref _isDateFilterVisible, value);
        }

    }
}
