namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    public class CachedOrganizationDepartments : IOrganizationDepartments
    {
        private readonly IReliableStateManager stateManager;
        private readonly Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery;
        private readonly ILogger logger;
        private const string ReliableDictionaryName = "departments-cache";
        private const string StoredKey = "departments";
        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(5); //TODO: configurable

        public CachedOrganizationDepartments(IReliableStateManager stateManager, Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery, ILogger logger)
        {
            this.stateManager = stateManager;
            this.allDepartmentsQuery = allDepartmentsQuery;
            this.logger = logger;
        }

        public async Task<DepartmentMetadata[]> GetAllAsync(CancellationToken cancellationToken)
        {
            var dictionary = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, OrganizationDepartmentsReliableState>>(ReliableDictionaryName);
            using var tx = this.stateManager.CreateTransaction();
            try
            {
                var storedDepartments = await dictionary.TryGetValueAsync(tx, StoredKey);
                if (storedDepartments.HasValue && storedDepartments.Value.Timestamp.Add(CacheTime) > DateTimeOffset.Now)
                {
                    return storedDepartments.Value.Data 
                        ?? throw new Exception($"{nameof(OrganizationDepartmentsReliableState.Data)} field is null");
                }
            }
            catch (Exception e)
            {
                this.logger.LogWarning(e, "Error occurred while reading cached departments");
            }

            using var departmentsQuery = this.allDepartmentsQuery();
            var loadedDepartments = (await departmentsQuery.Value.LoadAllAsync(cancellationToken)).ToArray();
            await dictionary.SetAsync(tx, StoredKey, new OrganizationDepartmentsReliableState() { Data = loadedDepartments, Timestamp = DateTimeOffset.Now });
            await tx.CommitAsync();
            return loadedDepartments;
        }
    }
}