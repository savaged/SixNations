using System.Collections.Generic;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IChildCollection<T> : IObservableModel
        where T : IObservableModel
    {
        IList<T> Children { get; set; }
    }
}