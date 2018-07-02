namespace Arcadia.Assistant.Feeds.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public abstract class EmployeesImportantDatesFeedActor : UntypedActor, ILogReceive
    {
        private readonly IActorRef organizationActor;

        public EmployeesImportantDatesFeedActor(IActorRef organizationActor)
        {
            this.organizationActor = organizationActor;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetMessages msg:
                    var today = DateTime.UtcNow.Date;

                    var toDate = msg.ToDate;
                    var fromDate = msg.FromDate;

                    if (toDate > today)
                    {
                        toDate = today;
                    }

                    var range = GetDateRange(fromDate, toDate);

                    this.GetMessages(range)
                        .PipeTo(this.Sender);

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract EmployeesQuery GetEmployeesQuery(DateTime date);

        protected abstract Message GetDateMessageForEmployee(EmployeeMetadata employee, DateTime date);

        private async Task<GetMessages.Response> GetMessages(IEnumerable<DateTime> dates)
        {
            var allMessages = new List<Message>();

            foreach (var date in dates)
            {
                var query = this.GetEmployeesQuery(date);
                var result = await this.organizationActor.Ask<EmployeesQuery.Response>(query);

                var messagesForDate = result
                    .Employees
                    .Select(x => this.GetDateMessageForEmployee(x.Metadata, date))
                    .Where(x => x != null)
                    .ToList();

                allMessages.AddRange(messagesForDate);
            }

            return new GetMessages.Response(allMessages);
        }

        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                yield break;
            }

            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }
    }
}