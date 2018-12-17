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

        private readonly EmployeesQuery query;

        private const string StartSearch = "start";

        private const string SearchFinished = "finished";

        public EmployeeSearch(IDictionary<string, IActorRef> allEmployees, IEnumerable<IActorRef> requesters, EmployeesQuery query)
        {
            this.employeeActorsToReply = GetInitialSearchSet(allEmployees, query);
            this.requesters = new HashSet<IActorRef>(requesters);
            this.query = query;

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
                    else
                    {
                        if (this.query.AscendantDepartmentId != null)
                        {
                            var departmentQuery = new DepartmentsQuery();
                            //TODO: work in progress
                        }
                    }

                    break;

                case EmployeeActor.GetEmployeeInfo.Response response:

                    if (this.CheckFilter(response.Employee.Metadata))
                    {
                        this.results.Add(response.Employee);
                    }

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

        private bool CheckFilter(EmployeeMetadata employee)
        {
            if ((this.query.DepartmentId != null) && (employee.DepartmentId != this.query.DepartmentId))
            {
                return false;
            }

            if ((this.query.EmployeeId != null) && (employee.EmployeeId != this.query.EmployeeId))
            {
                return false;
            }

            if ((this.query.RoomNumber != null) && (employee.RoomNumber != this.query.RoomNumber))
            {
                return false;
            }

            if ((this.query.Sid != null) && !string.Equals(employee.Sid, this.query.Sid, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if ((this.query.Email != null) && !string.Equals(employee.Email, this.query.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if ((this.query.HireDate != null) && !this.query.HireDate.Matches(employee.HireDate))
            {
                return false;
            }

            if ((this.query.BirthDate != null) && (!employee.BirthDate.HasValue || !this.query.BirthDate.Matches(employee.BirthDate.Value)))
            {
                return false;
            }

            return true;
        }

        private static HashSet<IActorRef> GetInitialSearchSet(IDictionary<string, IActorRef> employeesById, EmployeesQuery query)
        {
            if (query.EmployeeId != null)
            {
                return employeesById.TryGetValue(query.EmployeeId, out var actor)
                    ? new HashSet<IActorRef>() { actor }
                    : new HashSet<IActorRef>();
            }

            return new HashSet<IActorRef>(employeesById.Values);
        }
    }
}