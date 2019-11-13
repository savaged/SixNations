using savaged.mvvm.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IObservableModel : IDataModel, IObservableObject
    {
        bool IsHeader { get; set; }

        bool IsUnlocked { get; set; }

        string Identifier { get; }

        int ParentId { get; set; }

        string Error { get; set; }

        [SuppressMessage("NamingRuleViolation", "IDE1006")]
        DateTime updated_at { get; set; }

        IDictionary<string, object> GetData(
            bool forDisplay = false, 
            bool withDisplayNames = false,
            bool ignoreAttributes = false);

        void CopyTo<T>(ref T instance) where T : IObservableModel, new();

        IObservableModel Template();

    }
}
