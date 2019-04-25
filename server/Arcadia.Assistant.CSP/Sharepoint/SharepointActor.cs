namespace Arcadia.Assistant.CSP.Sharepoint
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class SharepointActor : UntypedActor, ILogReceive
    {
        private const string OrganizationActorPath = @"/user/organization";

        private readonly ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings;
        private readonly ActorSelection organizationActor;
        private IActorRef sharepointStorageActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SharepointActor(ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings)
        {
            this.departmentsCalendarsSettings = departmentsCalendarsSettings;
            this.organizationActor = Context.ActorSelection(OrganizationActorPath);

            Context.System.EventStream.Subscribe<CalendarEventRecoverComplete>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemoved>(this.Self);

            this.sharepointStorageActor = Context.ActorOf(SharepointStorageActor.CreateProps(), "csp-sharepoint-storage-actor");
        }

        public static Props CreateProps()
        {
            return Context.DI().Props<SharepointActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventRecoverComplete msg:
                    this.OnReceiveEvent(msg.Event);
                    break;

                case CalendarEventChanged msg:
                    this.OnReceiveEvent(msg.NewEvent);
                    break;

                case CalendarEventRemoved msg:
                    this.OnReceiveEvent(msg.Event, true);
                    break;

                case StoreCalendarEventToSharepoint msg:
                    this.sharepointStorageActor.Forward(msg);
                    break;

                case RemoveCalendarEventFromSharepoint msg:
                    this.sharepointStorageActor.Forward(msg);
                    break;

                case CalendarEventEmployeeMetadataFailed msg:
                    this.logger.Error(msg.Exception, $"Error occurred when trying to get employee with id {msg.Event.EmployeeId}.");
                    break;

                case NoSharepointDepartmentMapping msg:
                    this.logger.Warning($"No Sharepoint calendar mapping is defined for department with id {msg.DepartmentId}.");
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void OnReceiveEvent(CalendarEvent @event, bool isRemove = false)
        {
            this.GetEmployeeMetadata(@event.EmployeeId)
                .PipeTo(
                    this.Self,
                    success: employeeMetadata =>
                    {
                        var departmentCalendars = this.GetSharepointCalendarsByDepartment(employeeMetadata.DepartmentId);
                        if (departmentCalendars.Length == 0)
                        {
                            return new NoSharepointDepartmentMapping(employeeMetadata.DepartmentId);
                        }

                        if (!isRemove)
                        {
                            return new StoreCalendarEventToSharepoint(@event, employeeMetadata);
                        }

                        return new RemoveCalendarEventFromSharepoint(@event, employeeMetadata);
                    },
                    failure: err => new CalendarEventEmployeeMetadataFailed(@event, err));
        }

        private async Task<EmployeeMetadata> GetEmployeeMetadata(string employeeId)
        {
            var employeeResponse = await this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(employeeId));
            return employeeResponse.Employees.First().Metadata;
        }

        private string[] GetSharepointCalendarsByDepartment(string departmentId)
        {
            return this.departmentsCalendarsSettings.DepartmentsCalendars
                .Where(x => x.DepartmentId == departmentId)
                .Select(x => x.Calendar)
                .ToArray();
        }

        private class CalendarEventEmployeeMetadataFailed
        {
            public CalendarEventEmployeeMetadataFailed(CalendarEvent @event, Exception exception)
            {
                this.Event = @event;
                this.Exception = exception;
            }

            public CalendarEvent Event { get; }

            public Exception Exception { get; }
        }

        private class NoSharepointDepartmentMapping
        {
            public NoSharepointDepartmentMapping(string departmentId)
            {
                this.DepartmentId = departmentId;
            }

            public string DepartmentId { get; }
        }
    }
}