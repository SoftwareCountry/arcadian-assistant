namespace Arcadia.Assistant.Web.Employees
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;

    public class EmployeesSearch : IEmployeesSearch
    {
        private readonly IActorRefFactory actorSystem;

        private readonly ActorPathsBuilder pathsBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        public EmployeesSearch(IActorRefFactory actorSystem, ActorPathsBuilder pathsBuilder, ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
            this.timeoutSettings = timeoutSettings;
        }

        public async Task<IReadOnlyCollection<EmployeeContainer>> Search(EmployeesQuery query, CancellationToken token)
        {
            var organization = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Organization));
            var response = await organization.Ask<EmployeesQuery.Response>(query, this.timeoutSettings.Timeout, token);
            return response.Employees;
        }
    }
}