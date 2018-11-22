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
    using UserPreferences;

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
            EmployeeMetadata approver,
            DependentDepartmentsPendingActions dependentDepartmentsPendingActions)
        {
            this.organization.Tell(
                DepartmentsQuery.Create()
                    .WithHead(approver.EmployeeId)
                    .IncludeDirectDescendants());

            void OnMessage(object message)
            {
                switch (message)
                {
                    case DepartmentsQuery.Response response when response.Departments.Count == 0:
                        this.FinishProcessing();
                        break;

                    case DepartmentsQuery.Response response:
                        var ownDepartmentsEmployees = response.Departments
                            .Where(d => d.Department.ChiefId == approver.EmployeeId)
                            .SelectMany(d => d.Employees);

                        var otherDepartmentsEmployees = response.Departments
                            .Where(d => d.Department.ChiefId != approver.EmployeeId)
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

                        if (subordinates.Count == 0)
                        {
                            this.FinishProcessing();
                        }

                        this.Become(this.GatherCalendarEvents(subordinates));

                        break;

                    default:
                        this.DefaultMessageHandler(message);
                        break;
                }
            }

            return OnMessage;
        }

        private UntypedReceive GatherCalendarEvents(IEnumerable<EmployeeContainer> subordinates)
        {
            var calendarActorsToRespond = new HashSet<IActorRef>();

            foreach (var subordinate in subordinates)
            {
                subordinate.Calendar.CalendarActor.Tell(GetCalendarEvents.Instance);
                calendarActorsToRespond.Add(subordinate.Calendar.CalendarActor);
            }

            var eventsByEmployeeId = new Dictionary<string, IEnumerable<CalendarEvent>>();

            var statuses = new CalendarEventStatuses();

            void OnMessage(object message)
            {
                switch (message)
                {
                    case GetCalendarEvents.Response response:
                        var pendingEvents = response.Events.Where(x => x.IsPending).ToList();
                        if (pendingEvents.Count > 0)
                        {
                            eventsByEmployeeId[response.EmployeeId] = pendingEvents;
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

        private void FinishProcessing()
        {
            this.FinishProcessing(new Dictionary<string, IEnumerable<CalendarEvent>>());
        }

        private void FinishProcessing(IDictionary<string, IEnumerable<CalendarEvent>> events)
        {
            this.requestor.Tell(new GetPendingActions.Response(events));
            Context.Stop(this.Self);
        }

        public class GetPendingActions
        {
            public EmployeeMetadata Approver { get; }

            public DependentDepartmentsPendingActions DependentDepartmentsPendingActions { get; }

            public GetPendingActions(EmployeeMetadata approver, DependentDepartmentsPendingActions dependentDepartmentsPendingActions)
            {
                this.Approver = approver;
                DependentDepartmentsPendingActions = dependentDepartmentsPendingActions;
            }

            public class Response
            {
                public IDictionary<string, IEnumerable<CalendarEvent>> EventsByEmployeeId { get; }

                public Response(IDictionary<string, IEnumerable<CalendarEvent>> eventsByEmployeeId)
                {
                    this.EventsByEmployeeId = eventsByEmployeeId;
                }
            }
        }
    }
}