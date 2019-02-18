namespace Arcadia.Assistant.Calendar.Abstractions
{
    using Akka.Actor;

    public interface IEmployeeVacationsRegistryPropsFactory
    {
        Props CreateProps(string employeeId);
    }
}