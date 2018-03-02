namespace Arcadia.Assistant.Feeds.Polls
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeBirthdaysPoll : EmployeesPoll
    {
        private readonly IActorRef feed;

        public EmployeeBirthdaysPoll(IActorRef organization, IActorRef feed)
            : base(organization)
        {
            this.feed = feed;
        }

        protected override void OnPollFinished(EmployeesQuery.Response employees)
        {
            foreach (var employee in employees.Employees)
            {
                var title = employee.Metadata.Name;
                var msg = $"{employee.Metadata.Name} turns {employee.Metadata.Age}! Happy Birthsday!";
                this.feed.Tell(new FeedActor.PostMessage(new Message(Guid.NewGuid(), employee.Metadata.EmployeeId, title, msg, DateTimeOffset.Now)));
            }
        }

        protected override EmployeesQuery GetQuery(DateTime date)
        {
            return new EmployeesQuery().WithBirthDate(new DateQuery { Day = date.Day, Month = date.Month });
        }
    }
}