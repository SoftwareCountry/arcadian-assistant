namespace Arcadia.Assistant.NotificationTemplates
{
    using System.Fabric.Description;
    using Arcadia.Assistant.NotificationTemplates.Configuration;
    using Arcadia.Assistant.NotificationTemplates.Interfaces;
    using Autofac;

    using Employees.Contracts;

    using NotificationMasters;

    using Organization.Contracts;

    using SickLeaveCreatedNotificationMaster = Sharepoint.NotificationTemplates.SickLeaveCreatedNotificationMaster;

    public static class NotificationTemplatesRegistrationExtensions
    {
        public static void RegisterNotificationTemplates(
            this ContainerBuilder builder,
            ConfigurationSettings configurationSettings)
        {
            builder.RegisterModule<EmployeesModule>();
            builder.RegisterModule<OrganizationModule>();

            // -- SickLeaveCreated
            builder.RegisterInstance(
                NotificationConfigurationLoader.Load<ISickLeaveCreatedConfiguration>(
                    configurationSettings.Sections["SickLeaveCreated"]));
            builder.RegisterType<SickLeaveCreatedNotificationMaster>();
            // -- SickLeaveProlonged
            builder.RegisterInstance(
                NotificationConfigurationLoader.Load<ISickLeaveProlongedConfiguration>(
                    configurationSettings.Sections["SickLeaveProlonged"]));
            builder.RegisterType<SickLeaveProlongedNotificationMaster>();
            // -- SickLeaveCancelled
            builder.RegisterInstance(
                NotificationConfigurationLoader.Load<ISickLeaveCancelledConfiguration>(
                    configurationSettings.Sections["SickLeaveCancelled"]));
            builder.RegisterType<SickLeaveCancelledNotificationMaster>();
            // -- EventAssignedToApprover
            builder.RegisterInstance(
                NotificationConfigurationLoader.Load<IEventAssignedToApproverConfiguration>(
                    configurationSettings.Sections["EventAssignedToApprover"]));
            builder.RegisterType<EventAssignedToApproverNotificationMaster>();
            // -- EventStatusChanged
            builder.RegisterInstance(
                NotificationConfigurationLoader.Load<IEventStatusChangedConfiguration>(
                    configurationSettings.Sections["EventStatusChanged"]));
            builder.RegisterType<EventStatusChangedNotificationMaster>();
            // -- EventUserGrantedApproval
            builder.RegisterInstance(
                NotificationConfigurationLoader.Load<IEventUserGrantedApprovalConfiguration>(
                    configurationSettings.Sections["EventUserGrantedApproval"]));
            builder.RegisterType<EventUserGrantedApprovalNotificationMaster>();
        }
    }
}