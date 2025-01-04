using System.Threading.Tasks;

namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IHealthCheckEvent
    {
        Task Execute();
        Task OnAuthExpire();
        Task OnAuthGet();
        Task OnAuthAsk(string email);
        Task OnCommandExecuted();
        Task OnDatabaseVersion();
        Task OnAppVersion();
        Task OnAppEnv();
        Task OnAppConfig();
        Task OnAppMapper();
        Task OnS3();
        Task OnCacheProvider();
        Task OnLocalCache();
        Task OnRedisCache();
        Task OnLocalCacheFlush();
        Task OnRedisCacheFlush();
        Task OnEmail();
        Task OnLine();
        Task OnSlack();
    }
}
