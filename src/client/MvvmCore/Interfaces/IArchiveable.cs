using System;
using System.Diagnostics.CodeAnalysis;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IArchiveable : IObservableModel
    {
        [SuppressMessage("NamingRuleViolation", "IDE1006")]
        DateTime? archived_at { get; set; }

        bool IsArchived { get; }

        void SetToRestore();
    }
}
