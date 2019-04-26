namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.Extensions.Caching.Memory;

    public class CachedDepartmentsStorage : UntypedActor, ILogReceive
    {
        private const int DefaultCachePeriodInMinutes = 10;
        private const string DepartmentsResponseCacheKey = "AllDepartmentsResponse";
        private const string DepartmentsStorageActorPath = @"/user/organization/departments/departments-storage";

        private readonly IMemoryCache memoryCache;
        private readonly int cachePeriodInMinutes;
        private readonly ActorSelection departmentsActor;

        public CachedDepartmentsStorage(IMemoryCache memoryCache, int cachePeriodInMinutes)
        {
            this.memoryCache = memoryCache;
            this.cachePeriodInMinutes = cachePeriodInMinutes;
            this.departmentsActor = Context.ActorSelection(DepartmentsStorageActorPath);
        }

        public static Props CreateProps(IMemoryCache memoryCache, int cachePeriodInMinutes = DefaultCachePeriodInMinutes)
        {
            return Props.Create(() => new CachedDepartmentsStorage(memoryCache, cachePeriodInMinutes));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case DepartmentsStorage.LoadAllDepartments msg:
                    this.GetDepartmentsResponse().PipeTo(this.Sender);
                    break;

                default:
                    this.departmentsActor.Anchor.Forward(message);
                    break;
            }
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
            this.memoryCache.Set(DepartmentsResponseCacheKey, value, TimeSpan.FromMinutes(this.cachePeriodInMinutes));
        }
    }
}