namespace savaged.mvvm.Core.Interfaces
{
    public interface IBusyStateRegistry
    {
        bool IsBusy { get; }

        string DumpBusyRegistry();

        void ResetBusyRegistry();
    }
}
