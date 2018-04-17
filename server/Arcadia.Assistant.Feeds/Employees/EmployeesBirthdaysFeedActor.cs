namespace Arcadia.Assistant.Feeds.Employees
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeesBirthdaysFeedActor : EmployeesImportantDatesFeedActor
    {
        public EmployeesBirthdaysFeedActor(IActorRef organizationActor)
            : base(organizationActor)
        {
        }

        public static Props CreateProps(IActorRef organizationActor)
        {
            return Props.Create(() => new EmployeesBirthdaysFeedActor(organizationActor));
        }

        protected override EmployeesQuery GetEmployeesQuery(DateTime date)
        {
            return EmployeesQuery.Create().WithBirthDate(new DateQuery() { Day = date.Day, Month = date.Month });
        }

        protected override Message GetDateMessageForEmployee(EmployeeMetadata employee, DateTime date)
        {
            var title = employee.Name;
            var text = $"{employee.Name} turns {employee.AgeAt(date)}! Happy Birthday!";
            var message = new Message($"employee-birthday-{employee.EmployeeId}-at-{date}", employee.EmployeeId, title, text, date);

            return message;
        }
    }
}