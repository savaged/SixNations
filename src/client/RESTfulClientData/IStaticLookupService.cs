using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IStaticLookupService : ILookupService
    {
        bool IsLoaded { get; }

        Task LoadAsync(IAuthUser user);

        ILookup Get(string lookupName);

        IDictionary<string, ILookup> Index { get; }
    }
}