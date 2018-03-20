namespace Arcadia.Assistant.Feeds.Polls
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeAnniverseryPoll : EmployeesPoll
    {
        private readonly IActorRef feed;

        public EmployeeAnniverseryPoll(IActorRef organization, IActorRef feed)
            : base(organization)
        {
            this.feed = feed;
        }

        protected override void OnPollFinished(EmployeesQuery.Response employees)
        {
            foreach (var employee in employees.Employees)
            {
                var title = employee.Metadata.Name;
                var msg = $"Congratulations with Anniversery! {employee.Metadata.YearsServed} years served!";
                var hd = employee.Metadata.HireDate;
                var datePosted = new DateTimeOffset(DateTime.Now.Year, hd.Month, hd.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, new TimeSpan(0));
                this.feed.Tell(new FeedActor.PostMessage(new Message(Guid.NewGuid(), employee.Metadata.EmployeeId, title, msg, datePosted)));
            }
        }

        protected override EmployeesQuery GetQuery(DateTime date)
        {
            return new EmployeesQuery().WithHireDate(new DateQuery { Day = date.Day, Month = date.Month });
        }
    }
}