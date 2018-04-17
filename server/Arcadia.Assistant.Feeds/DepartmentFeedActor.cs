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

        private List<IActorRef> employees = new List<IActorRef>();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetMessages request:
                    Context.ActorOf(AggregateMessagesActor.GetProps(this.employees, request, this.Sender));
                    break;

                case AssignInformation information:
                    this.feedName = information.DepartmentInfo.Name;
                    this.employees = information.EmployeesActors.ToList();
                    break;
            }
        }

        public class AssignInformation
        {
            public DepartmentInfo DepartmentInfo { get; }

            public IEnumerable<IActorRef> EmployeesActors { get; }

            public AssignInformation(IEnumerable<IActorRef> employeesActors, DepartmentInfo departmentInfo)
            {
                this.EmployeesActors = employeesActors;
                this.DepartmentInfo = departmentInfo;
            }
        }
    }
}