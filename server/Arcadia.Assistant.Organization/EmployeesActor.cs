namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeesActor : UntypedActor, IWithUnboundedStash
    {
        private readonly IActorRef allEmployeesQuery;

        private Dictionary<string, IActorRef> EmployeesById { get; } = new Dictionary<string, IActorRef>();

        private readonly ILoggingAdapter logger = Context.GetLogger();

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

        public EmployeesActor()
        {
            this.allEmployeesQuery = Context.ActorOf(EmployeeIdsQuery.Props);

            //TODO: make interval configurable
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(10),
                this.allEmployeesQuery,
                EmployeeIdsQuery.RequestAllEmployeeIds.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case EmployeeIdsQuery.RequestAllEmployeeIds msg:
                    this.logger.Debug("Requesting employees list update...");
                    this.allEmployeesQuery.Tell(msg);
                    this.BecomeStacked(this.LoadingEmployees);
                    break;

                case FindEmployee request when this.EmployeesById.ContainsKey(request.EmployeeId):
                    this.Sender.Tell(new FindEmployee.Response(request.EmployeeId, this.EmployeesById[request.EmployeeId]));
                    break;
                case FindEmployee request:
                    this.Sender.Tell(new FindEmployee.Response(request.EmployeeId, Nobody.Instance));
                    break;

                case OrganizationRequests.RequestEmployeeInfo request when this.EmployeesById.ContainsKey(request.EmployeeId):
                    this.EmployeesById[request.EmployeeId].Forward(request);
                    break;

                case OrganizationRequests.RequestEmployeeInfo request:
                    this.Sender.Tell(new OrganizationRequests.RequestEmployeeInfo.EmployeeNotFound(request.EmployeeId));
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
                case EmployeeIdsQuery.RequestAllEmployeeIds _:
                    this.logger.Debug("Employees loading is requested while loading is still in progress, ignoring");
                    break;

                case EmployeeIdsQuery.RequestAllEmployeeIds.Error _:
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();
                    break;

                case EmployeeIdsQuery.RequestAllEmployeeIds.Response allEmployees:
                    this.RecreateEmployeeAgents(allEmployees.Ids);
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();
                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void RecreateEmployeeAgents(string[] allEmployees)
        {
            var removedIds = this.EmployeesById.Keys.Except(allEmployees).ToImmutableList();
            var addedIds = allEmployees.Except(this.EmployeesById.Keys).ToImmutableList();

            foreach (var removedId in removedIds)
            {
                this.EmployeesById[removedId].Tell(PoisonPill.Instance);
                this.EmployeesById.Remove(removedId);
            }

            foreach (var addedId in addedIds)
            {
                var employee = Context.ActorOf(Props.Create(() => new EmployeeActor(addedId)), Uri.EscapeDataString(addedId));
                this.EmployeesById[addedId] = employee;
            }

            this.logger.Debug("Employees list is updated");
        }

        public IStash Stash { get; set; }
    }
}