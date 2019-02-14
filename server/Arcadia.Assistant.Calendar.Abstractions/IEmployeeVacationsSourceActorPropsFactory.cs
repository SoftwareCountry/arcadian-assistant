namespace Arcadia.Assistant.Calendar.Abstractions
{
    using Akka.Actor;

    public interface IEmployeeVacationsSourceActorPropsFactory
    {
        Props CreateProps(string employeeId);
    }
}