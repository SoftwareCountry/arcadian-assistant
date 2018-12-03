namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;
    using Akka.Routing;

    using Arcadia.Assistant.Images;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeesActor : UntypedActor, IWithUnboundedStash, ILogReceive
    {
        private static readonly int ResizersCount = (int)Math.Ceiling(Environment.ProcessorCount / 2.0);

        private readonly IActorRef employeesInfoStorage;

        private readonly IActorRef imageResizer;

        private readonly IActorRef vacationsRegistry;

        private Dictionary<string, IActorRef> EmployeesById { get; } = new Dictionary<string, IActorRef>();

        private readonly ILoggingAdapter logger = Context.GetLogger();
        private readonly IActorRef calendarEventsApprovalsChecker;
        private readonly TimeSpan timeoutSetting;

        public IStash Stash { get; set; }

        public EmployeesActor(IActorRef calendarEventsApprovalsChecker, TimeSpan timeoutSetting)
        {
            this.employeesInfoStorage = Context.ActorOf(EmployeesInfoStorage.GetProps, "employees-storage");
            this.logger.Info($"Image resizers pool size: {ResizersCount}");
            this.imageResizer = Context.ActorOf(
                Props.Create(() => new ImageResizer()).WithRouter(new RoundRobinPool(ResizersCount)),
                "image-resizer");

            this.vacationsRegistry = Context.ActorOf(VacationsRegistry.GetProps, "vacations-registry");

            this.calendarEventsApprovalsChecker = calendarEventsApprovalsChecker;
            this.timeoutSetting = timeoutSetting;
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
                    Context.ActorOf(Props.Create(() => new EmployeeSearch(this.EmployeesById.Values, requesters, query)));
                    break;

                case Terminated t:
                    this.logger.Debug($"Employee actor got terminated - {t.ActorRef}");
                    // unexpected employee actor termination
                    var deadEmployees = this.EmployeesById.Where(x => x.Value.Equals(t.ActorRef)).Select(x => x.Key).ToList();
                    foreach (var deadEmployee in deadEmployees)
                    {
                        this.logger.Warning($"Employee actor {deadEmployee} died unexpectedly");
                        this.EmployeesById.Remove(deadEmployee);
                    }
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
            var removedIds = this.EmployeesById.Keys.Except(allEmployees.Select(x => x.Metadata.EmployeeId)).ToList();


            foreach (var removedId in removedIds)
            {
                var actorToRemove = this.EmployeesById[removedId];
                actorToRemove.Tell(PoisonPill.Instance);
                Context.Unwatch(actorToRemove);
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
                    employee = Context.ActorOf(
                        EmployeeActor.GetProps(
                            employeeNewInfo,
                            this.imageResizer,
                            this.vacationsRegistry,
                            this.calendarEventsApprovalsChecker,
                            this.timeoutSetting),
                        $"employee-{Uri.EscapeDataString(employeeNewInfo.Metadata.EmployeeId)}");

                    this.EmployeesById[employeeNewInfo.Metadata.EmployeeId] = employee;
                    Context.Watch(employee);
                    newEmployeesCount++;
                }
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

        public static Props GetProps(IActorRef calendarEventsApprovalsChecker, TimeSpan timeoutSetting) =>
            Props.Create(() => new EmployeesActor(calendarEventsApprovalsChecker, timeoutSetting));
    }
}