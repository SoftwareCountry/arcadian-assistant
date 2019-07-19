namespace Arcadia.Assistant.Web.DepartmentFeatures
{
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;

    public class DepartmentFeaturesService : IDepartmentFeaturesService
    {
        private readonly ITimeoutSettings timeoutSettings;

        public DepartmentFeaturesService(
            IActorRefFactory actorsFactory,
            ActorPathsBuilder actorPathsBuilder,
            ITimeoutSettings timeoutSettings)
        {
            this.timeoutSettings = timeoutSettings;
        }

        public async Task<DepartmentFeaturesModel> GetDepartmentFeatures(string departmentId, CancellationToken cancellationToken)
        {
            return new DepartmentFeaturesModel
            {
                Features = new[] { "dayoffs" }
            };
        }
    }
}