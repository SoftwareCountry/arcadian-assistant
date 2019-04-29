namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.Extensions.Caching.Memory;

    public class CachedEmployeesInfoStorage : UntypedActor, ILogReceive
    {
        private const int DefaultCachePeriodInMinutes = 10;
        private const string EmployeesResponseCacheKey = "AllEmployeesResponse";
        private const string EmployeesStorageActorPath = @"/user/organization/employees/employees-storage";

        private readonly IMemoryCache memoryCache;
        private readonly TimeSpan cachePeriod;
        private readonly ActorSelection employeesStorageActor;

        public CachedEmployeesInfoStorage(IMemoryCache memoryCache, TimeSpan cachePeriod, bool enablePeriodicalRefresh)
        {
            this.memoryCache = memoryCache;
            this.cachePeriod = cachePeriod;
            this.employeesStorageActor = Context.ActorSelection(EmployeesStorageActorPath);

            if (enablePeriodicalRefresh)
            {
                Context.System.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.Zero,
                    cachePeriod,
                    this.Self,
                    RefreshEmployees.Instance,
                    this.Self);
            }
        }

        public static Props CreateProps(IMemoryCache memoryCache, TimeSpan? cachePeriod = null, bool enablePeriodicalRefresh = false)
        {
            cachePeriod = cachePeriod ?? TimeSpan.FromMinutes(DefaultCachePeriodInMinutes);
            return Props.Create(() => new CachedEmployeesInfoStorage(memoryCache, cachePeriod.Value, enablePeriodicalRefresh));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshEmployees _:
                    this.GetEmployeesResponse().PipeTo(
                        this.Self,
                        success: () => RefreshEmployees.Success.Instance);
                    break;

                case RefreshEmployees.Success _:
                    break;

                case EmployeesInfoStorage.LoadAllEmployees _:
                    this.GetEmployeesResponse().PipeTo(this.Sender);
                    break;

                default:
                    this.employeesStorageActor.Tell(message, this.Sender);
                    break;
            }
        }

        private async Task<EmployeesInfoStorage.LoadAllEmployees.Response> GetEmployeesResponse()
        {
            var employeesResponse = this.GetFromCache();

            if (employeesResponse == null)
            {
                employeesResponse = await this.employeesStorageActor.Ask<EmployeesInfoStorage.LoadAllEmployees.Response>(
                    EmployeesInfoStorage.LoadAllEmployees.Instance
                );
                this.SetToCache(employeesResponse);
            }

            return employeesResponse;
        }

        private EmployeesInfoStorage.LoadAllEmployees.Response GetFromCache()
        {
            if (this.memoryCache.TryGetValue<EmployeesInfoStorage.LoadAllEmployees.Response>(EmployeesResponseCacheKey, out var value))
            {
                return value;
            }

            return null;
        }

        private void SetToCache(EmployeesInfoStorage.LoadAllEmployees.Response value)
        {
            this.memoryCache.Set(EmployeesResponseCacheKey, value, this.cachePeriod);
        }

        private class RefreshEmployees
        {
            public static readonly RefreshEmployees Instance = new RefreshEmployees();

            public class Success
            {
                public static readonly Success Instance = new Success();
            }
        }
    }
}