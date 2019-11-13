using System;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class LookupServiceLocator : ILookupServiceLocator
    {
        private readonly IStaticLookupService _staticLookupService;
        private readonly IDynamicLookupService _dynamicLookupService;
        private readonly IUserLookupService _userLookupService;
        private readonly IUserInputLookupService _userInputLookupService;
        private readonly ILookupAdminService _lookupAdminService;

        public LookupServiceLocator(
            IStaticLookupService staticLookupService,
            IDynamicLookupService dynamicLookupService,
            IUserLookupService userLookupService,
            IUserInputLookupService userInputLookupService,
            ILookupAdminService lookupAdminService)
        {
            _staticLookupService = staticLookupService ??
                throw new ArgumentNullException(
                    nameof(staticLookupService));

            _dynamicLookupService = dynamicLookupService ??
                throw new ArgumentNullException(
                    nameof(dynamicLookupService));

            _userLookupService = userLookupService ??
                throw new ArgumentNullException(
                    nameof(userLookupService));

            _userInputLookupService = userInputLookupService ??
                throw new ArgumentNullException(
                    nameof(userInputLookupService));

            _lookupAdminService = lookupAdminService ??
                throw new ArgumentNullException(
                    nameof(lookupAdminService));
        }

        public async Task<IStaticLookupService> GetStaticLookupServiceAsync(
            IAuthUser user)
        {
            if (!_staticLookupService.IsLoaded)
            {
                await _staticLookupService.LoadAsync(user);
            }
            return _staticLookupService;
        }

        public IStaticLookupService GetStaticLookupService()
        {
            if (!_staticLookupService.IsLoaded)
            {
                throw new InvalidOperationException(
                    "This service must be loaded prior to being fetched. " +
                    "Either load this as the client system starts up or " +
                    "call the alternative Async Get method.");
            }
            return _staticLookupService;
        }

        public IDynamicLookupService GetDynamicLookupService()
        {
            return _dynamicLookupService;
        }

        public IUserLookupService GetUserLookupService()
        {
            return _userLookupService;
        }

        public IUserInputLookupService GetUserInputLookupService()
        {
            return _userInputLookupService;
        }

        public ILookupAdminService GetLookupAdminService()
        {
            return _lookupAdminService;
        }

    }
}
