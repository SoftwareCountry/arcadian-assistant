namespace Arcadia.Assistant.Organization
{
    using System.Fabric;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Runtime;

    public interface IEmployees : IService
    {
        Task<string> HelloWorldAsync();
    }

    public class Employees : StatelessService, IEmployees
    {
        public Employees(StatelessServiceContext serviceContext) : base(serviceContext)
        {
        }

        public Task<string> HelloWorldAsync()
        {
            return Task.FromResult("Hello!");
        }
    }
}