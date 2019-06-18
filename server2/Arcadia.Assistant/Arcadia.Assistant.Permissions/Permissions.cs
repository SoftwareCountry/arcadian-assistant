namespace Arcadia.Assistant.Permissions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Fabric;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using Contracts;

    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Permissions : StatelessService, IPermissions
    {
        public Permissions(StatelessServiceContext context)
            : base(context)
        {
        }

        public async Task<UserPermissionsCollection> GetPermissionsAsync(string identity)
        {
            var user = identity;
            return new UserPermissionsCollection(EmployeePermissionsEntry.None, new Dictionary<string, EmployeePermissionsEntry>(), new Dictionary<string, EmployeePermissionsEntry>());
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}