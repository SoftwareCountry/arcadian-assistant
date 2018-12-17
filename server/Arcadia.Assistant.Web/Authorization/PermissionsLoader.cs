namespace Arcadia.Assistant.Web.Authorization
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Security;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;

    public class PermissionsLoader : IPermissionsLoader
    {
        private readonly IActorRefFactory actorSystem;

        private readonly ActorPathsBuilder pathsBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        public PermissionsLoader(
            IActorRefFactory actorSystem,
            ActorPathsBuilder pathsBuilder,
            ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
            this.timeoutSettings = timeoutSettings;
        }

        public async Task<Permissions> LoadAsync(ClaimsPrincipal user)
        {
            var organization = this.actorSystem.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Organization));
            var actor = this.actorSystem.ActorOf(Props.Create(() => new PermissionsActor(organization, this.timeoutSettings.Timeout)));

            var permissionsResponse = await actor.Ask<PermissionsActor.GetPermissions.Response>
                (new PermissionsActor.GetPermissions(user), this.timeoutSettings.Timeout);

            return permissionsResponse.Permissions;
        }
    }
}