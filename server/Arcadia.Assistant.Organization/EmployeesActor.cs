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

    public class EmployeesActor : UntypedActor, IWithUnboundedStash, ILogReceive
    {
        private readonly IActorRef employeesInfoStorage;

        private Dictionary<string, IActorRef> EmployeesById { get; } = new Dictionary<string, IActorRef>();

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public IStash Stash { get; set; }

        public EmployeesActor()
        {
            this.employeesInfoStorage = Context.ActorOf(EmployeesInfoStorage.Props, "employees-storage");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshEmployees _:
                    this.logger.Debug($"Requesting employees list update...");
                    this.employeesInfoStorage.Tell(EmployeesInfoStorage.LoadAllEmployees.Instance);
                    this.BecomeStacked(this.LoadingEmployees);
                    break;

                case EmployeesQuery query:
                    var requesters = new[] { this.Sender };
                    Context.ActorOf(Akka.Actor.Props.Create(() => new EmployeeSearch(this.EmployeesById.Values, requesters, query)));
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

                case Status.Failure _:
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();
                    break;

                case EmployeesInfoStorage.LoadAllEmployees.Response allEmployees:
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

            this.logger.Debug($"Employees list is updated. There are {allEmployees.Count} at all, {removedIds.Count} got removed, {addedEmployees.Count} were added");
        }

        public sealed class RefreshEmployees
        {
            public static readonly RefreshEmployees Instance = new RefreshEmployees();
        }

        public static Props Props() => Akka.Actor.Props.Create(() => new EmployeesActor());
    }
}