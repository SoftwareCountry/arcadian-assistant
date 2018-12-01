namespace Arcadia.Assistant.Web.Employees
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.UserPreferences;

    public class PendingActionsRequest : UntypedActor, ILogReceive
    {
        private readonly ActorSelection organization;

        public PendingActionsRequest(ActorPathsBuilder actorsPathsBuilder)
        {
            this.organization = Context.ActorSelection(actorsPathsBuilder.Get(WellKnownActorPaths.Organization));
            // this.SetReceiveTimeout(timeoutSettings.Timeout);
        }

        public static Props CreateProps(ActorPathsBuilder actorsPathsBuilder)
            => Props.Create(() => new PendingActionsRequest(actorsPathsBuilder));

        private IActorRef requestor;

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetPendingActions request:
                    this.requestor = this.Sender;
                    this.Become(this.GatherEmployeeInfo(request.Approver, request.DependentDepartmentsPendingActions));
                    break;

                default:
                    this.DefaultMessageHandler(message);
                    break;
            }
        }

        private void DefaultMessageHandler(object message)
        {
            switch (message)
            {
                case ReceiveTimeout _:
                    Context.Stop(this.Self);
                    break;

                default:
                    this.Unhandled(this.requestor);
                    break;
            }
        }

        private UntypedReceive GatherEmployeeInfo(
            EmployeeContainer approver,
            DependentDepartmentsPendingActions dependentDepartmentsPendingActions)
        {
            this.organization.Tell(
                DepartmentsQuery.Create()
                    .WithHead(approver.Metadata.EmployeeId)
                    .IncludeDirectDescendants());

            void OnMessage(object message)
            {
                switch (message)
                {
                    case DepartmentsQuery.Response response:
                        var ownDepartmentsEmployees = response.Departments
                            .Where(d => d.Department.ChiefId == approver.Metadata.EmployeeId)
                            .SelectMany(d => d.Employees);

                        var otherDepartmentsEmployees = response.Departments
                            .Where(d => d.Department.ChiefId != approver.Metadata.EmployeeId)
                            .Select(d => new
                            {
                                d.Department.ChiefId,
                                Employees = d.Employees.Where(e =>
                                {
                                    if (dependentDepartmentsPendingActions == DependentDepartmentsPendingActions.All)
                                    {
                                        return true;
                                    }

                                    if (dependentDepartmentsPendingActions == DependentDepartmentsPendingActions.HeadsOnly)
                                    {
                                        return e.Metadata.EmployeeId == d.Department.ChiefId;
                                    }

                                    return false;
                                })
                            })
                            .SelectMany(x => x.Employees);

                        var subordinates = ownDepartmentsEmployees
                            .Union(otherDepartmentsEmployees)
                            .GroupBy(x => x.Metadata.EmployeeId)
                            .Select(x => x.First())
                            .ToList();

                        this.Become(this.GatherCalendarEvents(approver, subordinates));

                        break;

                    default:
                        this.DefaultMessageHandler(message);
                        break;
                }
            }

            return OnMessage;
        }

        private UntypedReceive GatherCalendarEvents(
            EmployeeContainer approver,
            IEnumerable<EmployeeContainer> subordinates)
        {
            var calendarActorsToRespond = new HashSet<IActorRef>();

            foreach (var subordinate in subordinates)
            {
                subordinate.Calendar.CalendarActor.Tell(GetCalendarEvents.Instance);
                calendarActorsToRespond.Add(subordinate.Calendar.CalendarActor);
            }

            approver.Calendar.VacationPendingActionsActor.Tell(GetEmployeePendingActions.Instance);
            calendarActorsToRespond.Add(approver.Calendar.VacationPendingActionsActor);

            var eventsByEmployeeId = new Dictionary<string, List<CalendarEvent>>();

            void OnMessage(object message)
            {
                switch (message)
                {
                    case GetCalendarEvents.Response response:
                        var pendingEvents = response.Events
                            .Where(x => x.IsPending && x.Type != CalendarEventTypes.Vacation)
                            .ToList();
                        if (pendingEvents.Count != 0)
                        {
                            eventsByEmployeeId.TryAdd(response.EmployeeId, new List<CalendarEvent>());
                            eventsByEmployeeId[response.EmployeeId].AddRange(pendingEvents);
                        }

                        calendarActorsToRespond.Remove(this.Sender);
                        if (calendarActorsToRespond.Count == 0)
                        {
                            this.FinishProcessing(eventsByEmployeeId);
                        }

                        break;

                    case GetEmployeePendingActions.Response response:
                        var pendingActions = response.PendingActions.ToList();
                        if (pendingActions.Count != 0)
                        {
                            var groupedActions = pendingActions.GroupBy(a => a.EmployeeId);
                            foreach (var grouping in groupedActions)
                            {
                                eventsByEmployeeId.TryAdd(grouping.Key, new List<CalendarEvent>());
                                eventsByEmployeeId[grouping.Key].AddRange(grouping);
                            }
                        }

                        calendarActorsToRespond.Remove(this.Sender);
                        if (calendarActorsToRespond.Count == 0)
                        {
                            this.FinishProcessing(eventsByEmployeeId);
                        }
                        break;

                    default:
                        this.DefaultMessageHandler(message);
                        break;
                }
            }

            return OnMessage;
        }

        private void FinishProcessing(Dictionary<string, List<CalendarEvent>> events)
        {
            var result = events.ToDictionary(x => x.Key, x => x.Value.AsEnumerable());
            this.requestor.Tell(new GetPendingActions.Response(result));
            Context.Stop(this.Self);
        }

        public class GetPendingActions
        {
            public GetPendingActions(EmployeeContainer approver, DependentDepartmentsPendingActions dependentDepartmentsPendingActions)
            {
                this.Approver = approver;
                this.DependentDepartmentsPendingActions = dependentDepartmentsPendingActions;
            }

            public EmployeeContainer Approver { get; }

            public DependentDepartmentsPendingActions DependentDepartmentsPendingActions { get; }

            public class Response
            {
                public Response(IDictionary<string, IEnumerable<CalendarEvent>> eventsByEmployeeId)
                {
                    this.EventsByEmployeeId = eventsByEmployeeId;
                }

                public IDictionary<string, IEnumerable<CalendarEvent>> EventsByEmployeeId { get; }
            }
        }
    }
}