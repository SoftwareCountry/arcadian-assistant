namespace Arcadia.Assistant.CSP.Cache
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.Extensions.Caching.Memory;

    public class CachedEmployeesInfoStorage : CachedActor
    {
        private const string EmployeesResponseCacheKey = "AllEmployeesResponse";
        private const string EmployeesStorageActorPath = @"/user/organization/employees/employees-storage";

        private readonly ActorSelection employeesStorageActor;

        public CachedEmployeesInfoStorage(IMemoryCache memoryCache, TimeSpan cachePeriod)
            : base(memoryCache, cachePeriod)
        {
            this.employeesStorageActor = Context.ActorSelection(EmployeesStorageActorPath);
        }

        public static Props CreateProps(IMemoryCache memoryCache, TimeSpan cachePeriod)
        {
            return Props.Create(() => new CachedEmployeesInfoStorage(memoryCache, cachePeriod));
        }

        protected override string CacheKey => EmployeesResponseCacheKey;

        protected override void OnReceive(object message)
        {
            if (base.CanReceive(message))
            {
                base.OnReceive(message);
                return;
            }

            switch (message)
            {
                case EmployeesInfoStorage.LoadAllEmployees _:
                    this.GetEmployeesResponse().PipeTo(this.Sender);
                    break;

                default:
                    this.employeesStorageActor.Tell(message, this.Sender);
                    break;
            }
        }

        protected override async Task<object> LoadValue()
        {
            var employeesResponse = await this.employeesStorageActor.Ask<EmployeesInfoStorage.LoadAllEmployees.Response>(
                EmployeesInfoStorage.LoadAllEmployees.Instance);
            return employeesResponse;
        }

        private async Task<EmployeesInfoStorage.LoadAllEmployees.Response> GetEmployeesResponse()
        {
            var employeesResponse = this.GetFromCache();

            if (employeesResponse == null)
            {
                employeesResponse = await this.LoadValue();
                this.SetToCache(employeesResponse);
            }

            return employeesResponse as EmployeesInfoStorage.LoadAllEmployees.Response;
        }
    }
}