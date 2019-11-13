using System;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IDynamicLookupService : ILookupService
    {
        Task<ILookup> GetAsync<T>(IAuthUser user) where T : IDataModel;
        Task<ILookup> GetAsync(IAuthUser user, string lookupType);

        Task<ILookup> GetByRelationAsync(
            IAuthUser user, Type lookupType, IDataModel relation);

        Task<ILookup> GetByRelationAsync(
            IAuthUser user, string lookupName, IDataModel relation);

        Task<ILookup> GetByRelationAsync<T>(
            IAuthUser user, 
            Type relationType, 
            int relationId)
            where T : IDataModel;

        Task<ILookup> GetByRelationAsync<T>(
            IAuthUser user, IDataModel relation)
            where T : IDataModel;
    }
}