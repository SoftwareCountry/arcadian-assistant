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
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.UserPreferences;

    using PushNotification = Arcadia.Assistant.Notifications.Push.PushNotification;

    public class SickLeaveManagerPushNotificationActor : UntypedActor
    {
        private readonly IPushNotification createdPushNotificationConfig;
        private readonly IPushNotification prolongedPushNotificationConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;
        private readonly IActorRef pushDevicesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SickLeaveManagerPushNotificationActor(
            IPushNotification createdPushNotificationConfig,
            IPushNotification prolongedPushNotificationConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor,
            IActorRef pushDevicesActor)
        {
            this.createdPushNotificationConfig = createdPushNotificationConfig;
            this.prolongedPushNotificationConfig = prolongedPushNotificationConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;
            this.pushDevicesActor = pushDevicesActor;

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
                    msg.ManagerUserPreferences?.PushNotifications == true:

                    this.logger.Debug($"Sending a sick leave {(msg.IsProlonged ? "prolonged" : "created")} push notification to manager {msg.Manager?.EmployeeId} for user {msg.Event.EmployeeId}");

                    var notificationConfiguration = msg.IsProlonged
                        ? this.prolongedPushNotificationConfig
                        : this.createdPushNotificationConfig;

                    this.SendNotification(msg, notificationConfiguration);

                    break;

                case CalendarEventWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendNotification(CalendarEventWithAdditionalData message, IPushNotification notificationConfiguration)
        {
            var templateExpressionContext = new Dictionary<string, string>
            {
                ["employee"] = message.Owner.Name,
                ["startDate"] = message.Event.Dates.StartDate.ToString("dd/MM/yyyy")
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                message.Event.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            var content = new PushNotificationContent
            {
                Title = notificationConfiguration.Title,
                Body = new TemplateExpressionParser().Parse(notificationConfiguration.Body, templateExpressionContext),
                CustomData = new
                {
                    message.Event.EventId,
                    message.Owner.EmployeeId,
                    ManagerId = message.Manager.EmployeeId,
                    Type = message.IsProlonged
                        ? CalendarEventPushNotificationTypes.SickLeaveProlongedManager
                        : CalendarEventPushNotificationTypes.SickLeaveCreatedManager
                }
            };

            Context.System.EventStream.Publish(new NotificationEventBusMessage(
                new PushNotification(content, message.ManagerPushTokens)));
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
            var managerPushTokensTask = this.GetDevicePushTokens(managerEmployeeId);
            await Task.WhenAll(managerEmployeeTask, managerPreferencesTask, managerPushTokensTask);

            var manager = managerEmployeeTask.Result;
            var managerPreferences = managerPreferencesTask.Result;
            var managerPushTokens = managerPushTokensTask.Result;

            return new CalendarEventWithAdditionalData(@event, isProlonged, owner, manager, managerPreferences, managerPushTokens);
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

        private async Task<DevicePushToken[]> GetDevicePushTokens(string managerEmployeeId)
        {
            var pushTokensResponse = await this.pushDevicesActor.Ask<GetDevicePushTokens.Success>(
                new GetDevicePushTokens(managerEmployeeId));
            return pushTokensResponse.DevicePushTokens.ToArray();
        }

        private class CalendarEventWithAdditionalData
        {
            public CalendarEventWithAdditionalData(
                CalendarEvent @event,
                bool isProlonged,
                EmployeeMetadata owner,
                EmployeeMetadata manager = null,
                UserPreferences managerUserPreferences = null,
                IEnumerable<DevicePushToken> managerPushTokens = null)
            {
                this.Event = @event;
                this.IsProlonged = isProlonged;
                this.Owner = owner;
                this.Manager = manager;
                this.ManagerUserPreferences = managerUserPreferences;
                this.ManagerPushTokens = managerPushTokens;
            }

            public CalendarEvent Event { get; }

            public bool IsProlonged { get; }

            public EmployeeMetadata Owner { get; }

            public EmployeeMetadata Manager { get; }

            public UserPreferences ManagerUserPreferences { get; }

            public IEnumerable<DevicePushToken> ManagerPushTokens { get; }
        }
    }
}