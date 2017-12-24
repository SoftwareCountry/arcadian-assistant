namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeSearch : UntypedActor
    {
        private readonly HashSet<IActorRef> requesters;

        private readonly HashSet<IActorRef> actorsToReply;

        private readonly List<EmployeeContainer> results = new List<EmployeeContainer>();

        private readonly EmployeesQuery query;

        private const string StartSearch = "start";

        private const string SearchFinished = "finished";

        public EmployeeSearch(IEnumerable<IActorRef> allEmployees, IEnumerable<IActorRef> requesters, EmployeesQuery query)
        {
            this.actorsToReply = new HashSet<IActorRef>(allEmployees);
            this.requesters = new HashSet<IActorRef>(requesters);
            this.query = query;

            this.Self.Tell(StartSearch);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartSearch:
                    foreach (var actorRef in this.actorsToReply)
                    {
                        actorRef.Tell(EmployeeActor.GetEmployeeInfo.Instance);
                    }

                    if ((this.requesters.Count == 0) || (this.actorsToReply.Count == 0))
                    {
                        this.Self.Tell(SearchFinished);
                    }

                    break;

                case EmployeeActor.GetEmployeeInfo.Response response:

                    if (this.CheckFilter(response.Employee.EmployeeStoredInformation))
                    {
                        this.results.Add(response.Employee);
                    }

                    this.actorsToReply.Remove(response.Employee.Actor);

                    if (this.actorsToReply.Count == 0)
                    {
                        this.Self.Tell(SearchFinished);
                    }

                    break;

                case SearchFinished:
                    foreach (var requester in this.requesters)
                    {
                        requester.Tell(new EmployeesQuery.Response(this.results));
                    }

                    Context.Stop(this.Self);

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private bool CheckFilter(EmployeeStoredInformation employee)
        {
            if ((this.query.DepartmentId != null) && (employee.DepartmentId != this.query.DepartmentId))
            {
                return false;
            }

            if ((this.query.EmployeeId != null) && (employee.EmployeeId != this.query.EmployeeId))
            {
                return false;
            }

            return true;
        }
    }
}