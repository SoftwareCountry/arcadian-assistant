namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    public class CachedOrganizationDepartments : IOrganizationDepartments
    {
        private readonly IReliableStateManager stateManager;
        private readonly Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery;

        public CachedOrganizationDepartments(IReliableStateManager stateManager, Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery)
        {
            this.stateManager = stateManager;
            this.allDepartmentsQuery = allDepartmentsQuery;
        }

        public async Task<DepartmentMetadata[]> GetAllAsync(CancellationToken cancellationToken)
        {
            var dictionary = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, DepartmentMetadata[]>>("departments-temp");
            using (var tx = this.stateManager.CreateTransaction())
            {
                var storedDepartments = await dictionary.TryGetValueAsync(tx, "departments");
                if (storedDepartments.HasValue)
                {
                    return storedDepartments.Value;
                }

                using (var departmentsQuery = this.allDepartmentsQuery())
                {
                    var loadedDepartments = (await departmentsQuery.Value.LoadAllAsync(cancellationToken)).ToArray();
                    await dictionary.SetAsync(tx, "departments", loadedDepartments);
                    await tx.CommitAsync();
                    return loadedDepartments;
                }
            }
        }
    }
}