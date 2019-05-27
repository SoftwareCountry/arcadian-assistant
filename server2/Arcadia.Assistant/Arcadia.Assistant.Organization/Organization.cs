namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using CSP.Model;

    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class Organization : StatefulService, IOrganization
    {
        private readonly Func<ArcadiaCspContext> cspContextFactory;

        public Organization(StatefulServiceContext context, Func<ArcadiaCspContext> cspContextFactory)
            : base(context)
        {
            this.cspContextFactory = cspContextFactory;
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

        public async Task<EmployeeMetadata> FindByIdAsync(string employeeId, CancellationToken cancellationToken)
        {
            using (var db = this.cspContextFactory())
            {
                var employee = await db.Employee.FindAsync(int.Parse(employeeId));
                return new EmployeeMetadata(employeeId, employee.LastName, employee.Email);
            }
        }
    }
}