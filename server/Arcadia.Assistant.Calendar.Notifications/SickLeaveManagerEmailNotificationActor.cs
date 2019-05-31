﻿namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.UserPreferences;

    using EmailNotification = Arcadia.Assistant.Notifications.Email.EmailNotification;

    public class SickLeaveManagerEmailNotificationActor : UntypedActor
    {
        private readonly IEmailNotification createdEmailNotificationConfig;
        private readonly IEmailNotification prolongedEmailNotificationConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SickLeaveManagerEmailNotificationActor(
            IEmailNotification createdEmailNotificationConfig,
            IEmailNotification prolongedEmailNotificationConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor)
        {
            this.createdEmailNotificationConfig = createdEmailNotificationConfig;
            this.prolongedEmailNotificationConfig = prolongedEmailNotificationConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;

            Context.System.EventStream.Subscribe<CalendarEventCreated>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventCreated msg when
                    msg.Event.Type == CalendarEventTypes.Sickleave:

                    this.GetAdditionalData(msg.Event, false, msg.Event.EmployeeId)
                        .PipeTo(this.Self);
                    break;

                case CalendarEventChanged msg when
                    msg.NewEvent.Type == CalendarEventTypes.Sickleave &&
                    msg.NewEvent.Status == SickLeaveStatuses.Approved &&
                    msg.OldEvent.Status == SickLeaveStatuses.Approved &&
                    msg.OldEvent.Dates.EndDate != msg.NewEvent.Dates.EndDate:

                    this.GetAdditionalData(msg.NewEvent, true, msg.NewEvent.EmployeeId)
                        .PipeTo(this.Self);
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventWithAdditionalData msg when
                    msg.ManagerUserPreferences?.EmailNotifications == true:

                    this.logger.Debug($"Sending a sick leave {(msg.IsProlonged ? "prolonged" : "created")} email notification to manager {msg.Manager?.EmployeeId} for user {msg.Event.EmployeeId}");

                    var notificationConfiguration = msg.IsProlonged
                        ? this.prolongedEmailNotificationConfig
                        : this.createdEmailNotificationConfig;

                    this.SendNotification(msg, notificationConfiguration);

                    break;

                case CalendarEventWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendNotification(CalendarEventWithAdditionalData msg, IEmailNotification notificationConfiguration)
        {
            var templateExpressionContext = new Dictionary<string, string>
            {
                ["employee"] = msg.Owner.Name,
                ["startDate"] = msg.Event.Dates.StartDate.ToString("dd/MM/yyyy"),
                ["endDate"] = msg.Event.Dates.EndDate.ToString("dd/MM/yyyy")
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                msg.Event.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            var templateExpressionParser = new TemplateExpressionParser();

            var sender = notificationConfiguration.NotificationSender;
            var recipient = msg.Manager?.Email;
            var subject = notificationConfiguration.Subject;
            var body = templateExpressionParser.Parse(notificationConfiguration.Body, templateExpressionContext);

            Context.System.EventStream.Publish(
                new NotificationEventBusMessage(
                    new EmailNotification(sender, new[] { recipient }, subject, body)));
        }

        private async Task<CalendarEventWithAdditionalData> GetAdditionalData(CalendarEvent @event, bool isProlonged, string ownerEmployeeId)
        {
            var ownerTask = this.GetEmployee(ownerEmployeeId);
            var departmentsTask = this.GetDepartments();
            await Task.WhenAll(ownerTask, departmentsTask);

            var owner = ownerTask.Result;
            var departments = departmentsTask.Result;

            var ownDepartment = departments.First(d => d.DepartmentId == owner.DepartmentId);
            var isEmployeeChief = ownDepartment.ChiefId == owner.EmployeeId;
            var parentDepartment = departments.First(d => d.DepartmentId == ownDepartment.ParentDepartmentId);

            if (ownDepartment.IsHeadDepartment && isEmployeeChief)
            {
                return new CalendarEventWithAdditionalData(@event, isProlonged, owner);
            }

            var managerEmployeeId = !isEmployeeChief ? ownDepartment.ChiefId : parentDepartment?.ChiefId;

            var managerEmployeeTask = this.GetEmployee(managerEmployeeId);
            var managerPreferencesTask = this.GetUserPreferences(managerEmployeeId);
            await Task.WhenAll(managerEmployeeTask, managerPreferencesTask);

            var manager = managerEmployeeTask.Result;
            var managerPreferences = managerPreferencesTask.Result;

            return new CalendarEventWithAdditionalData(@event, isProlonged, owner, manager, managerPreferences);
        }

        private async Task<EmployeeMetadata> GetEmployee(string employeeId)
        {
            var employeesResponse = await this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(employeeId));
            return employeesResponse.Employees.FirstOrDefault()?.Metadata;
        }

        private async Task<DepartmentInfo[]> GetDepartments()
        {
            var departmentsResponse = await this.organizationActor.Ask<DepartmentsQuery.Response>(
                DepartmentsQuery.Create());
            return departmentsResponse.Departments
                .Select(d => d.Department)
                .ToArray();
        }

        private async Task<UserPreferences> GetUserPreferences(string managerEmployeeId)
        {
            var userPreferencesResponse = await this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(managerEmployeeId));
            return userPreferencesResponse.UserPreferences;
        }

        private class CalendarEventWithAdditionalData
        {
            public CalendarEventWithAdditionalData(
                CalendarEvent @event,
                bool isProlonged,
                EmployeeMetadata owner,
                EmployeeMetadata manager = null,
                UserPreferences managerUserPreferences = null)
            {
                this.Event = @event;
                this.IsProlonged = isProlonged;
                this.Owner = owner;
                this.Manager = manager;
                this.ManagerUserPreferences = managerUserPreferences;
            }

            public CalendarEvent Event { get; }

            public bool IsProlonged { get; }

            public EmployeeMetadata Owner { get; }

            public EmployeeMetadata Manager { get; }

            public UserPreferences ManagerUserPreferences { get; }
        }
    }
}