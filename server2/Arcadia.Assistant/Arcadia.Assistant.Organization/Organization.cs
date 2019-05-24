namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Organization : StatefulService, IOrganization
    {
        public Organization(StatefulServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle
        ///     client or user requests.
        /// </summary>
        /// <remarks>
        ///     For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public Task<EmployeeMetadata> FindByIdAsync(string employeeId, CancellationToken cancellationToken)
        {
            return Task.FromResult(new EmployeeMetadata(employeeId, "John Doe", "john.doe@aaa.com"));
        }
    }
}