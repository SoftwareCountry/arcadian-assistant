namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeSearch : UntypedActor
    {
        private readonly HashSet<IActorRef> requesters;

        private readonly HashSet<IActorRef> employeeActorsToReply;

        private readonly List<EmployeeContainer> results = new List<EmployeeContainer>();

        private const string StartSearch = "start";

        private const string SearchFinished = "finished";

        public EmployeeSearch(IEnumerable<EmployeeIndexEntry> allEmployees, IEnumerable<IActorRef> requesters, EmployeesQuery query)
        {
            this.employeeActorsToReply = GetInitialSearchSet(allEmployees, query);
            this.requesters = new HashSet<IActorRef>(requesters);

            this.Self.Tell(StartSearch);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartSearch:

                    foreach (var actorRef in this.employeeActorsToReply)
                    {
                        actorRef.Tell(EmployeeActor.GetEmployeeInfo.Instance);
                    }

                    if ((this.requesters.Count == 0) || (this.employeeActorsToReply.Count == 0))
                    {
                        this.Self.Tell(SearchFinished);
                    }

                    break;

                case EmployeeActor.GetEmployeeInfo.Response response:

                    this.results.Add(response.Employee);

                    this.employeeActorsToReply.Remove(response.Employee.Actor);

                    if (this.employeeActorsToReply.Count == 0)
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

        private static HashSet<IActorRef> GetInitialSearchSet(IEnumerable<EmployeeIndexEntry> employees, EmployeesQuery query)
        {
            return new HashSet<IActorRef>(
                employees
                    .Where(x => CheckFilter(x.Metadata, query))
                    .Select(x => x.EmployeeActor));
        }

        private static bool CheckFilter(EmployeeMetadata employee, EmployeesQuery query)
        {
            if ((query.DepartmentId != null) && (employee.DepartmentId != query.DepartmentId))
            {
                return false;
            }

            if ((query.EmployeeId != null) && (employee.EmployeeId != query.EmployeeId))
            {
                return false;
            }

            if ((query.RoomNumber != null) && (employee.RoomNumber != query.RoomNumber))
            {
                return false;
            }

            if ((query.Sid != null) && !string.Equals(employee.Sid, query.Sid, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if ((query.Email != null) && !string.Equals(employee.Email, query.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if ((query.HireDate != null) && !query.HireDate.Matches(employee.HireDate))
            {
                return false;
            }

            if ((query.BirthDate != null) && (!employee.BirthDate.HasValue || !query.BirthDate.Matches(employee.BirthDate.Value)))
            {
                return false;
            }

            return true;
        }
    }
}