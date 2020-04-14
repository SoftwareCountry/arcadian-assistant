using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.CSP.WebApi.Processors
{
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts.Models;

    using Microsoft.Extensions.Logging;

    public sealed class EmployeeCspProcessor
    {
        private readonly HttpClient httpClient;
        private readonly string serverUrl;
        private readonly ILogger logger;

        public EmployeeCspProcessor(HttpClient httpClient, string serverUrl, ILogger logger)
        {
            this.httpClient = httpClient;
            this.serverUrl = $"{serverUrl}EmployeeApi/";
            this.logger = logger;
        }

        public async Task<Employee[]> Get(CancellationToken cancellationToken)
        {
            var requestUrl = $"{this.serverUrl}GetEmployees";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            //request.Content.Headers.Remove("Content-Type");
            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            //request.Content.Headers.ContentLength = requestData.Length;

            try
            {
                var response = await this.httpClient.GetAsync(new Uri(requestUrl), cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Access token response has failed");
                }

                var responseContent = await response.Content.ReadAsStreamAsync();
                this.logger.LogInformation("Read employee collection stream");
                var employeeArray = await JsonSerializer.DeserializeAsync<Employee[]>(responseContent, default, cancellationToken);
                this.logger.LogDebug($"Read {employeeArray.Length} employees.");
                return employeeArray;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Get employee list error");
            }

            return new Employee[0];
        }
    }
}
