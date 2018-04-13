namespace Arcadia.Assistant.Feeds.Employees
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeesAnniversariesFeedActor : EmployeesImportantDatesFeedActor
    {
        public EmployeesAnniversariesFeedActor(IActorRef organizationActor)
            : base(organizationActor)
        {
        }

        public static Props CreateProps(IActorRef organizationActor)
        {
            return Props.Create(() => new EmployeesAnniversariesFeedActor(organizationActor));
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

            var msg = $"Congratulations with Anniversary! {employee.YearsServedAt(date)} years served!";
            return new Message($"employee-anniversary-{employee.EmployeeId}-at-{date}", employee.EmployeeId, title, msg, date);
        }
    }
}