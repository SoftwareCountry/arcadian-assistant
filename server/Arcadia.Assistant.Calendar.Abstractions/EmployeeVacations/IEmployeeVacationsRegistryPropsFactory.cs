namespace Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations
{
    using Akka.Actor;

    public interface IEmployeeVacationsRegistryPropsFactory
    {
        Props CreateProps(string employeeId);
    }
}