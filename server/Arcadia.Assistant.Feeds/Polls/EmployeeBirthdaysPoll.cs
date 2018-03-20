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
                var msg = $"{employee.Metadata.Name} turns {employee.Metadata.Age}! Happy Birthday!";
                var hd = employee.Metadata.BirthDate.HasValue ? employee.Metadata.BirthDate.Value : DateTime.Now;
                var datePosted = new DateTimeOffset(DateTime.Now.Year, hd.Month, hd.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, new TimeSpan(0));
                this.feed.Tell(new FeedActor.PostMessage(new Message(Guid.NewGuid(), employee.Metadata.EmployeeId, title, msg, datePosted)));
            }
        }

        protected override EmployeesQuery GetQuery(DateTime date)
        {
            return new EmployeesQuery().WithBirthDate(new DateQuery { Day = date.Day, Month = date.Month });
        }
    }
}