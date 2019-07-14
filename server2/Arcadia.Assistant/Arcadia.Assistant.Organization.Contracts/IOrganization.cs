﻿using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Organization.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IOrganization : IService
    {
        Task<DepartmentMetadata> GetDepartmentAsync(string departmentId, CancellationToken cancellationToken);

        Task<DepartmentMetadata[]> GetDepartmentsAsync(CancellationToken cancellationToken);

        Task<EmployeeMetadata> FindEmployeeSupervisorAsync(string employeeId, CancellationToken cancellationToken);

        Task<DepartmentMetadata[]> GetSupervisedDepartmentsAsync(string employeeId, CancellationToken cancellationToken);
    }
}