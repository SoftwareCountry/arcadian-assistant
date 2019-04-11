namespace Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves
{
    using Akka.Actor;

    public interface IEmployeeSickLeavesRegistryPropsFactory
    {
        Props CreateProps(string employeeId);
    }
}