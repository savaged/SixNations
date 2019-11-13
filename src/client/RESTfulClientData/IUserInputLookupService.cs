namespace savaged.mvvm.Data
{
    public interface IUserInputLookupService
    {
        IUserInputLookup Get(string lookupName, string selectedItem);
        bool Update(string lookupName, IUserInputLookup lookup);
    }
}