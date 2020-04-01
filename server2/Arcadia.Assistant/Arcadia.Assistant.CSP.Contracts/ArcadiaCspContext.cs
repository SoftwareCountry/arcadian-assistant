namespace Arcadia.Assistant.CSP
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using CspdbConnector.AzureBus;
    using CspdbConnector.Contracts;
    using CspdbConnector.Contracts.Configuration;

    using Microsoft.Extensions.Logging;

    using Models;

    public class ArcadiaCspContext
    {
        private readonly ICspdbConnector cspdbConnector;
        private readonly ILogger logger;

        public ArcadiaCspContext(MassTransitBusService busService, ICspdbConnector cspdbConnector, ILogger<ArcadiaCspContext> logger)
        {
            this.logger = logger;
            this.cspdbConnector = cspdbConnector;
            busService.StartAsync(CancellationToken.None).Wait();
        }

        public async Task<byte[]> GetEmployeesImageAsync(int id)
        {
            return await cspdbConnector.GetEmployeeImageAsync(id);
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return (await cspdbConnector.ListEmployeesAsync(""))
                .Select(x => new Employee()
                {
                    Id = x.Id.GetValueOrDefault(),
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    LoginName = x.LoginName,
                    Email = x.Email,
                    Birthday = x.BirthDay,
                    RoomNumber = x.RoomNumber,
                    Department = new Department()
                    {
                        Abbreviation = x.Department
                    },
                    IsDelete = x.IsDeleted,
                    Position =  new EmployeePosition()
                    {
                        Title = x.Position
                    }
                })
                .ToList();
            /*
            foreach (var employee in result)
            {
                employee.IsPublishDeny = await cspdbConnector.GetEmployeeImageAsync(employee.Id ?? 0);
                logger.LogInformation($"<< {employee}, {image?.Length ?? 0} bytes");
            }
            */
        }

        public List<Department> Departments { get; } = new List<Department>();

        public List<SickLeave> SickLeaves { get; } = new List<SickLeave>();

        public List<Vacation> Vacations { get; } = new List<Vacation>();
    }
}