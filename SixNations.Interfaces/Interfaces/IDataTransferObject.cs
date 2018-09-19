namespace SixNations.API.Interfaces
{
    public interface IDataTransferObject
    {
        object this[string key] { get; }

        void AddError(string error);
        bool ContainsKey(string key);
        bool IsEmpty();
        string ToString();
        void __SetIsLockedForEditing();
    }
}