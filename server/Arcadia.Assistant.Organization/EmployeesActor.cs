namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeesActor : UntypedActor, IWithUnboundedStash
    {
        private readonly string departmentId;

        private readonly IActorRef employeesInfoStorage;

        private Dictionary<string, IActorRef> EmployeesById { get; } = new Dictionary<string, IActorRef>();

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public IStash Stash { get; set; }

        public EmployeesActor(string departmentId)
        {
            this.departmentId = departmentId;

            this.employeesInfoStorage = Context.ActorOf(EmployeesInfoStorage.Props, "employees-storage");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshEmployees _:
                    this.logger.Debug($"Requesting employees list update for ${this.departmentId}...");
                    this.employeesInfoStorage.Tell(new EmployeesInfoStorage.LoadDepartmentsEmployees(this.departmentId));
                    this.BecomeStacked(this.LoadingEmployees);
                    break;

                case FindEmployee request when this.EmployeesById.ContainsKey(request.EmployeeId):
                    this.Sender.Tell(new FindEmployee.Response(request.EmployeeId, this.EmployeesById[request.EmployeeId]));
                    break;
                case FindEmployee request:
                    this.Sender.Tell(new FindEmployee.Response(request.EmployeeId, Nobody.Instance));
                    break;

                case RequestEmployeeInfo request when this.EmployeesById.ContainsKey(request.EmployeeId):
                    this.EmployeesById[request.EmployeeId].Forward(request);
                    break;

                case RequestEmployeeInfo request:
                    this.Sender.Tell(new RequestEmployeeInfo.EmployeeNotFound(request.EmployeeId));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void LoadingEmployees(object message)
        {
            switch (message)
            {
                case RefreshEmployees _:
                    this.logger.Debug("Employees loading is requested while loading is still in progress, ignoring");
                    break;

                case EmployeesInfoStorage.LoadDepartmentsEmployees.Error _:
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();
                    break;

                case EmployeesInfoStorage.LoadDepartmentsEmployees.Response allEmployees:
                    this.RecreateEmployeeAgents(allEmployees.Employees);
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();
                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void RecreateEmployeeAgents(IReadOnlyCollection<EmployeeInfo> allEmployees)
        {
            var removedIds = this.EmployeesById.Keys.Except(allEmployees.Select(x => x.EmployeeId)).ToImmutableList();
            var addedEmployees = allEmployees.Where(x => !this.EmployeesById.ContainsKey(x.EmployeeId)).ToImmutableList();

            foreach (var removedId in removedIds)
            {
                this.EmployeesById[removedId].Tell(PoisonPill.Instance);
                this.EmployeesById.Remove(removedId);
            }

            foreach (var addedEmployee in addedEmployees)
            {
                var employee = Context.ActorOf(EmployeeActor.Props(addedEmployee), Uri.EscapeDataString(addedEmployee.EmployeeId));
                this.EmployeesById[addedEmployee.EmployeeId] = employee;
            }

            this.logger.Debug($"Employees list is updated for {this.departmentId} department");
        }

        public class FindEmployee
        {
            public string EmployeeId { get; }

            public FindEmployee(string employeeId)
            {
                this.EmployeeId = employeeId;
            }

            public class Response
            {
                public string EmployeeId { get; }

                public IActorRef Employee { get; }

                public Response(string employeeId, IActorRef employee)
                {
                    this.EmployeeId = employeeId;
                    this.Employee = employee;
                }
            }
        }

        public sealed class RefreshEmployees
        {
            public static readonly RefreshEmployees Instance = new RefreshEmployees();
        }

        public static Props Props(string departmentId) => Akka.Actor.Props.Create(() => new EmployeesActor(departmentId));
    }
}