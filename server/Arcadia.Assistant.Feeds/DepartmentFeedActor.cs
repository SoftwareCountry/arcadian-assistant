namespace Arcadia.Assistant.Feeds
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentFeedActor : UntypedActor
    {
        private string feedName = string.Empty;

        private List<IActorRef> employeeFeeds = new List<IActorRef>();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetMessages request:
                    Context.ActorOf(AggregateMessagesActor.GetProps(this.employeeFeeds, request, this.Sender));
                    break;

                case AssignInformation information:
                    this.feedName = information.DepartmentInfo.Name;
                    this.employeeFeeds = information.EmployeesFeeds.ToList();
                    break;
            }
        }

        public class AssignInformation
        {
            public DepartmentInfo DepartmentInfo { get; }

            public IEnumerable<IActorRef> EmployeesFeeds { get; }

            public AssignInformation(IEnumerable<IActorRef> employeesFeeds, DepartmentInfo departmentInfo)
            {
                this.EmployeesFeeds = employeesFeeds;
                this.DepartmentInfo = departmentInfo;
            }
        }
    }
}