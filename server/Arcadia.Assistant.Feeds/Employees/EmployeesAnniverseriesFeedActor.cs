namespace Arcadia.Assistant.Feeds.Employees
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeesAnniverseriesFeedActor : EmployeesImportantDatesFeedActor
    {
        public EmployeesAnniverseriesFeedActor(IActorRef organizationActor)
            : base(organizationActor)
        {
        }

        public static Props CreateProps(IActorRef organizationActor)
        {
            return Props.Create(() => new EmployeesAnniverseriesFeedActor(organizationActor));
        }

        protected override EmployeesQuery GetEmployeesQuery(DateTime date)
        {
            return EmployeesQuery.Create().WithHireDate(new DateQuery() { Day = date.Day, Month = date.Month });
        }

        protected override Message GetDateMessageForEmployee(EmployeeMetadata employee, DateTime date)
        {
            var title = employee.Name;

            var yearsServed = employee.YearsServedAt(date);
            if (yearsServed <= 0)
            {
                return null;
            }

            var msg = $"Congratulations with Anniversery! {employee.YearsServedAt(date)} years served!";
            return new Message(Guid.NewGuid(), employee.EmployeeId, title, msg, date);
        }
    }
}