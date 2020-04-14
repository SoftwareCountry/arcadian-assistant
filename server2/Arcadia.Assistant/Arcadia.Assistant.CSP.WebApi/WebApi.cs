using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.CSP.WebApi
{
    using System.Net.Http;
    using Contracts.Models;

    using Contracts;

    using Microsoft.Extensions.Logging;

    using Processors;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class WebApi : StatelessService, ICspApi
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;

        private readonly EmployeeCspProcessor employeeCsp;
        private readonly DepartmentCspProcessor departmentCsp;
        private readonly CspConfiguration cspConfiguration;

        public WebApi(
            StatelessServiceContext context, 
            IHttpClientFactory httpClientFactory, 
            CspConfiguration configuration,
            ILogger<WebApi> logger)
            : base(context)
        {
            this.logger = logger;
            this.cspConfiguration = configuration;
            this.httpClient = httpClientFactory.CreateClient();
            this.employeeCsp = new EmployeeCspProcessor(this.httpClient, this.cspConfiguration.ConnectionString, this.logger);
            this.departmentCsp = new DepartmentCspProcessor(this.httpClient, this.cspConfiguration.ConnectionString, this.logger);

        }

        public Task<Employee[]> GetEmployees(CancellationToken cancellationToken)
        {
            return this.employeeCsp.Get(cancellationToken);
        }

        public Task<Department[]> GetDepartments(CancellationToken cancellationToken)
        {
            return this.departmentCsp.Get(cancellationToken);
        }

        public Task<DepartmentWithPeopleCount[]> GetDepartmentWithPeople(CancellationToken cancellationToken)
        {
            return this.departmentCsp.GetDepartmentWithPeople(cancellationToken);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
            }
        }
    }
}
