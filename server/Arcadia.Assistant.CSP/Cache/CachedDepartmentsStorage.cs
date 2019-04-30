namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class CachedDepartmentsStorage : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private const int DefaultCachePeriodInMinutes = 10;
        private const string DepartmentsResponseCacheKey = "AllDepartmentsResponse";
        private const string DepartmentsStorageActorPath = @"/user/organization/departments/departments-storage";

        private readonly MemoryCache memoryCache;
        private readonly ActorSelection departmentsActor;

        public CachedDepartmentsStorage(MemoryCache memoryCache, TimeSpan cachePeriod, bool enablePeriodicalRefresh)
        {
            this.memoryCache = memoryCache;
            this.departmentsActor = Context.ActorSelection(DepartmentsStorageActorPath);

            if (enablePeriodicalRefresh)
            {
                Context.System.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.Zero,
                    cachePeriod,
                    this.Self,
                    RefreshDepartments.Instance,
                    this.Self);
            }
        }

        public IStash Stash { get; set; }

        public static Props CreateProps(MemoryCache memoryCache, TimeSpan? cachePeriod = null, bool enablePeriodicalRefresh = false)
        {
            cachePeriod = cachePeriod ?? TimeSpan.FromMinutes(DefaultCachePeriodInMinutes);
            return Props.Create(() => new CachedDepartmentsStorage(memoryCache, cachePeriod.Value, enablePeriodicalRefresh));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshDepartments _:
                    this.Become(this.OnRefreshReceive);

                    this.GetDepartmentsResponse().PipeTo(
                        this.Self,
                        success: () => RefreshDepartments.Success.Instance);
                    break;

                case DepartmentsStorage.LoadAllDepartments _:
                    this.GetDepartmentsResponse().PipeTo(this.Sender);
                    break;

                default:
                    this.departmentsActor.Tell(message, this.Sender);
                    break;
            }
        }

        private void OnRefreshReceive(object message)
        {
            switch (message)
            {
                case RefreshDepartments.Success _:
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

        private async Task<DepartmentsStorage.LoadAllDepartments.Response> GetDepartmentsResponse()
        {
            var departmentsResponse = this.GetFromCache();

            if (departmentsResponse == null)
            {
                departmentsResponse = await this.departmentsActor.Ask<DepartmentsStorage.LoadAllDepartments.Response>(
                    DepartmentsStorage.LoadAllDepartments.Instance
                );
                this.SetToCache(departmentsResponse);
            }

            return departmentsResponse;
        }

        private DepartmentsStorage.LoadAllDepartments.Response GetFromCache()
        {
            if (this.memoryCache.TryGetValue<DepartmentsStorage.LoadAllDepartments.Response>(DepartmentsResponseCacheKey, out var value))
            {
                return value;
            }

            return null;
        }

        private void SetToCache(DepartmentsStorage.LoadAllDepartments.Response value)
        {
            this.memoryCache.Set(DepartmentsResponseCacheKey, value);
        }

        private class RefreshDepartments
        {
            public static readonly RefreshDepartments Instance = new RefreshDepartments();

            public class Success
            {
                public static readonly Success Instance = new Success();
            }
        }
    }
}