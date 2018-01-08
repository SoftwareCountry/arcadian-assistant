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

        private void RecreateEmployeeAgents(IReadOnlyCollection<EmployeeStoredInformation> allEmployees)
        {
            var removedIds = this.EmployeesById.Keys.Except(allEmployees.Select(x => x.Metadata.EmployeeId)).ToList();
            

            foreach (var removedId in removedIds)
            {
                this.EmployeesById[removedId].Tell(PoisonPill.Instance);
                this.EmployeesById.Remove(removedId);
            }

            var newEmployeesCount = 0;
            foreach (var employeeNewInfo in allEmployees)
            {
                if (this.EmployeesById.TryGetValue(employeeNewInfo.Metadata.EmployeeId, out var employee))
                {
                    employee.Tell(new EmployeeActor.UpdateEmployeeInformation(employeeNewInfo));
                }
                else
                {
                    employee = Context.ActorOf(EmployeeActor.GetProps(employeeNewInfo), Uri.EscapeDataString(employeeNewInfo.Metadata.EmployeeId));
                    this.EmployeesById[employeeNewInfo.Metadata.EmployeeId] = employee;
                    newEmployeesCount++;
                }
            }

            this.logger.Debug($"Employees list is updated. There are {allEmployees.Count} at all, {removedIds.Count} got removed, {newEmployeesCount} were added");
        }

        public sealed class RefreshEmployees
        {
            public static readonly RefreshEmployees Instance = new RefreshEmployees();
        }

        public static Props GetProps() => Context.DI().Props<EmployeesActor>();
    }
}