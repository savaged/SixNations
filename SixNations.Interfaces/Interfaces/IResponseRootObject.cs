namespace SixNations.API.Interfaces
{
    public interface IResponseRootObject
    {
        IDataTransferObject[] Data { get; set; }
        string Error { get; set; }
        bool Success { get; set; }

        bool IsEmpty();
        string ToString();
        void __SetIsLockedForEditing();
    }
}