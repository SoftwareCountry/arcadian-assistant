namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class CachedEmployeesInfoStorage : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private const int DefaultCachePeriodInMinutes = 10;
        private const string EmployeesResponseCacheKey = "AllEmployeesResponse";
        private const string EmployeesStorageActorPath = @"/user/organization/employees/employees-storage";

        private readonly MemoryCache memoryCache;
        private readonly ActorSelection employeesStorageActor;

        public CachedEmployeesInfoStorage(MemoryCache memoryCache, TimeSpan cachePeriod, bool enablePeriodicalRefresh)
        {
            this.memoryCache = memoryCache;
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

        public IStash Stash { get; set; }

        public static Props CreateProps(MemoryCache memoryCache, TimeSpan? cachePeriod = null, bool enablePeriodicalRefresh = false)
        {
            cachePeriod = cachePeriod ?? TimeSpan.FromMinutes(DefaultCachePeriodInMinutes);
            return Props.Create(() => new CachedEmployeesInfoStorage(memoryCache, cachePeriod.Value, enablePeriodicalRefresh));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshEmployees _:
                    this.Become(this.OnRefreshReceive);

                    this.GetEmployeesResponse().PipeTo(
                        this.Self,
                        success: () => RefreshEmployees.Success.Instance);
                    break;

                case EmployeesInfoStorage.LoadAllEmployees _:
                    this.GetEmployeesResponse().PipeTo(this.Sender);
                    break;

                default:
                    this.employeesStorageActor.Tell(message, this.Sender);
                    break;
            }
        }

        private void OnRefreshReceive(object message)
        {
            switch (message)
            {
                case RefreshEmployees.Success _:
                    this.BecomeDefault();
                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void BecomeDefault()
        {
            this.Stash.UnstashAll();
            this.Become(this.OnReceive);
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
            this.memoryCache.Set(EmployeesResponseCacheKey, value);
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