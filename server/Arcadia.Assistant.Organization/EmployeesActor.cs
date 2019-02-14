namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Routing;

    using Arcadia.Assistant.Images;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Patterns;

    public class EmployeesActor : UntypedActor, IWithUnboundedStash, ILogReceive
    {
        private static readonly int ResizersCount = (int)Math.Ceiling(Environment.ProcessorCount / 2.0);

        private readonly IActorRef employeesInfoStorage;

        private readonly IActorRef imageResizer;

        private readonly IActorRef vacationsCreditRegistry;

        private readonly Dictionary<string, EmployeeIndexEntry> employeesById = new Dictionary<string, EmployeeIndexEntry>();

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public IStash Stash { get; set; }

        public EmployeesActor()
        {
            this.employeesInfoStorage = Context.ActorOf(EmployeesInfoStorage.GetProps, "employees-storage");
            this.logger.Info($"Image resizers pool size: {ResizersCount}");
            this.imageResizer = Context.ActorOf(
                Props.Create(() => new ImageResizer()).WithRouter(new RoundRobinPool(ResizersCount)),
                "image-resizer");

            this.vacationsCreditRegistry = Context.ActorOf(VacationsCreditRegistry.GetProps, "vacations-credit-registry");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshEmployees _:
                    this.logger.Debug($"Requesting employees list update...");
                    this.employeesInfoStorage.Tell(EmployeesInfoStorage.LoadAllEmployees.Instance);
                    this.BecomeStacked(this.EmployeesLoadRequested(this.Sender));
                    break;

                case EmployeesQuery query:
                    var requesters = new[] { this.Sender };
                    Context.ActorOf(Props.Create(() => new EmployeeSearch(this.employeesById.Values.ToList(), requesters, query)));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private UntypedReceive EmployeesLoadRequested(IActorRef initiator)
        {
            var agentsToRespondAboutRefreshing = new List<IActorRef> { initiator };
            return message => this.LoadingEmployees(message, agentsToRespondAboutRefreshing);
        }

        private void LoadingEmployees(object message, List<IActorRef> actorsToRespondAboutRefreshing)
        {
            switch (message)
            {
                case RefreshEmployees _:
                    this.logger.Debug("Employees loading is requested while loading is still in progress, ignoring");
                    actorsToRespondAboutRefreshing.Add(this.Sender);
                    break;

                case Status.Failure e:
                    OnRefreshFinish(e);
                    break;

                case EmployeesInfoStorage.LoadAllEmployees.Response allEmployees:
                    this.RecreateEmployeeAgents(allEmployees.Employees);
                    OnRefreshFinish(RefreshEmployees.Finished.Instance);
                    break;

                default:
                    this.Stash.Stash();
                    break;
            }

            void OnRefreshFinish(object onFinishMessage)
            {
                actorsToRespondAboutRefreshing.ForEach(x => x.Tell(onFinishMessage));
                this.Stash.UnstashAll();
                this.UnbecomeStacked();
            }
        }

        private void RecreateEmployeeAgents(IReadOnlyCollection<EmployeeStoredInformation> allEmployees)
        {
            this.logger.Debug("Recreating employee agents...");
            var removedIds = this.employeesById.Keys.Except(allEmployees.Select(x => x.Metadata.EmployeeId)).ToList();

            foreach (var removedId in removedIds)
            {
                this.employeesById.Remove(removedId);
            }

            var persistenceSupervisorFactory = new PersistenceSupervisorFactory();

            var newEmployeesCount = 0;
            foreach (var employeeNewInfo in allEmployees)
            {
                IActorRef employeeActor;
                var employeeId = employeeNewInfo.Metadata.EmployeeId;

                if (this.employeesById.TryGetValue(employeeId, out var employee))
                {
                    employeeActor = employee.EmployeeActor;
                    employeeActor.Tell(new EmployeeActor.UpdateEmployeeInformation(employeeNewInfo));
                }
                else
                {
                    var employeeActorProps = EmployeeActor.GetProps(
                        employeeNewInfo,
                        this.imageResizer,
                        this.vacationsCreditRegistry);
                    employeeActor = Context.ActorOf(
                        persistenceSupervisorFactory.Get(employeeActorProps),
                        $"employee-{Uri.EscapeDataString(employeeId)}");

                    newEmployeesCount++;
                }

                this.employeesById[employeeId] = new EmployeeIndexEntry(employeeActor, employeeNewInfo.Metadata);
            }

            this.logger.Debug($"Employees list is updated. There are {allEmployees.Count} at all, {removedIds.Count} got removed, {newEmployeesCount} were added");
        }

        public sealed class RefreshEmployees
        {
            public static readonly RefreshEmployees Instance = new RefreshEmployees();

            public sealed class Finished
            {
                public static readonly Finished Instance = new Finished();
            }
        }

        public static Props GetProps() => Props.Create(() => new EmployeesActor());
    }
}