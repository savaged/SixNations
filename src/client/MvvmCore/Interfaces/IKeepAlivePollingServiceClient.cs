using System.Threading.Tasks;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IKeepAlivePollingServiceClient
    {
        Task ServiceCallToKeepAliveAsync();
    }
}