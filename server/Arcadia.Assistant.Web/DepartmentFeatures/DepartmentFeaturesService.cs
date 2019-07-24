namespace Arcadia.Assistant.Web.DepartmentFeatures
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;

    public class DepartmentFeaturesService : IDepartmentFeaturesService
    {
        private readonly ITimeoutSettings timeoutSettings;
        private readonly ActorSelection organizationActor;

        public DepartmentFeaturesService(
            IActorRefFactory actorsFactory,
            ActorPathsBuilder actorPathsBuilder,
            ITimeoutSettings timeoutSettings)
        {
            this.organizationActor = actorsFactory.ActorSelection(
                actorPathsBuilder.Get(WellKnownActorPaths.Organization));
            this.timeoutSettings = timeoutSettings;
        }

        public async Task<DepartmentFeaturesModel> GetDepartmentFeatures(string departmentId, CancellationToken cancellationToken)
        {
            var response = await this.organizationActor.Ask<GetDepartmentFeatures.Response>(
                new GetDepartmentFeatures(departmentId),
                this.timeoutSettings.Timeout,
                cancellationToken);

            switch (response)
            {
                case GetDepartmentFeatures.Success success:
                    return new DepartmentFeaturesModel
                    {
                        Features = success.Features
                    };

                case GetDepartmentFeatures.NotFound _:
                    return new DepartmentFeaturesModel
                    {
                        Features = Enumerable.Empty<string>()
                    };

                default:
                    throw new Exception("Not supported department features response type");
            }
        }
    }
}