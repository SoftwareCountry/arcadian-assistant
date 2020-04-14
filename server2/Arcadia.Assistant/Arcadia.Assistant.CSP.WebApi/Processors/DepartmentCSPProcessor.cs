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

    public sealed class DepartmentCspProcessor
    {
        private readonly HttpClient httpClient;
        private readonly string serverUrl;
        private readonly ILogger logger;

        public DepartmentCspProcessor(HttpClient httpClient, string serverUrl, ILogger logger)
        {
            this.httpClient = httpClient;
            this.serverUrl = serverUrl;
            this.logger = logger;
        }

        public async Task<Department[]> Get(CancellationToken cancellationToken)
        {
            var requestUrl = $"{this.serverUrl}GetDistinctDepartments";
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
                var departmentsArray =
                    await JsonSerializer.DeserializeAsync<Department[]>(responseContent, default, cancellationToken);
                this.logger.LogDebug($"Read {departmentsArray.Length} departments.");
                return departmentsArray;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Get employee list error");
            }

            return new Department[0];
        }

        public async Task<DepartmentWithPeopleCount[]> GetDepartmentWithPeople(CancellationToken cancellationToken)
        {
            var requestUrl = $"{this.serverUrl}GetDistinctDepartments";
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
                var departmentsArray = await JsonSerializer.DeserializeAsync<Department[]>(responseContent, default, cancellationToken);
                this.logger.LogDebug($"Read {departmentsArray.Length} departments.");
                return departmentsArray
                    .Select(x => new DepartmentWithPeopleCount()
                    {
                        Department =  x,
                        ActualChiefId = x.ChiefId,
                        PeopleCount = 0
                    })
                    .ToArray();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Get employee list error");
            }

            return new DepartmentWithPeopleCount[0];
        }
    }
}
