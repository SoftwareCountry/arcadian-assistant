namespace Arcadia.Assistant.Organization.Contracts
{
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IOrganization : IService
    {
        Task<string> HelloWorldAsync();
    }
}