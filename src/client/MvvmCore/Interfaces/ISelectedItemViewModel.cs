using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface ISelectedItemViewModel<T> 
        : IModelObjectViewModel<T>, IInitialised
        where T : IObservableModel, new()
    {
        void SeedModelObject(int modelId);

        void Seed(IObservableModel parent);

        void Seed(T modelObject, IObservableModel parent = null);

        void ResetModelObjectToHeader();

        T ModelObject { get; set; }

        bool IsItemSelected { get; }

        IModelService ModelService { get; }

        ICommand SubmitCmd { get; }
        ICommand ShowCmd { get; }
        ICommand EditCmd { get; }
        ICommand DeleteCmd { get; }

        bool CanSubmit { get; }
        bool CanShow { get; }
        bool CanEdit { get; }
    }
}