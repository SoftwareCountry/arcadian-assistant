namespace Arcadia.Assistant.Calendar.Vacations
{
    using Akka.Actor;
    using Arcadia.Assistant.Calendar.Abstractions;

    public class PersistentEmployeeVacationsSourceActorPropsFactory : IEmployeeVacationsSourceActorPropsFactory
    {
        public Props CreateProps(string employeeId)
        {
            return Props.Create(() => new PersistentEmployeeVacationsSourceActor(employeeId));
        }
    }
}