namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.Extensions.Caching.Memory;

    public class CachedDepartmentsStorage : CachedActor
    {
        private const string DepartmentsResponseCacheKey = "AllDepartmentsResponse";
        private const string DepartmentsStorageActorPath = @"/user/organization/departments/departments-storage";

        private readonly ActorSelection departmentsActor;

        public CachedDepartmentsStorage(IMemoryCache memoryCache, TimeSpan cachePeriod)
            : base(memoryCache, cachePeriod)
        {
            this.departmentsActor = Context.ActorSelection(DepartmentsStorageActorPath);
        }

        public static Props CreateProps(IMemoryCache memoryCache, TimeSpan cachePeriod)
        {
            return Props.Create(() => new CachedDepartmentsStorage(memoryCache, cachePeriod));
        }

        protected override string CacheKey => DepartmentsResponseCacheKey;

        protected override void OnReceive(object message)
        {
            if (base.CanReceive(message))
            {
                base.OnReceive(message);
                return;
            }

            switch (message)
            {
                case DepartmentsStorage.LoadAllDepartments _:
                    this.GetDepartmentsResponse().PipeTo(this.Sender);
                    break;

                default:
                    this.departmentsActor.Tell(message, this.Sender);
                    break;
            }
        }

        protected override async Task<object> LoadValue()
        {
            var departmentsResponse = await this.departmentsActor.Ask<DepartmentsStorage.LoadAllDepartments.Response>(
                DepartmentsStorage.LoadAllDepartments.Instance);
            return departmentsResponse;
        }

        private async Task<DepartmentsStorage.LoadAllDepartments.Response> GetDepartmentsResponse()
        {
            var departmentsResponse = this.GetFromCache();

            if (departmentsResponse == null)
            {
                departmentsResponse = await this.LoadValue();
                this.SetToCache(departmentsResponse);
            }

            return departmentsResponse as DepartmentsStorage.LoadAllDepartments.Response;
        }
    }
}