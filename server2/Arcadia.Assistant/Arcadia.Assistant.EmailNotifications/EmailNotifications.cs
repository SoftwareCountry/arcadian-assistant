namespace Arcadia.Assistant.EmailNotifications
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;
    using Contracts.Models;

    using MailKit.Net.Smtp;
    using MailKit.Security;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using MimeKit;

    using Models;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class EmailNotifications : StatelessService, IEmailNotifications
    {
        private readonly ILogger logger;
        private readonly SmtpSettings smtpSettings;
        private readonly EmailNotificationSettings emailNotificationSettings;

        public EmailNotifications(
            StatelessServiceContext context,
            EmailNotificationSettings emailNotificationSettings,
            SmtpSettings smtpSettings,
            ILogger<EmailNotifications> logger)
            : base(context)
        {
            this.smtpSettings = smtpSettings;
            this.emailNotificationSettings = emailNotificationSettings;
            this.logger = logger;
        }

        public async Task SendEmailNotification(
            string[] emailAddresses,
            EmailNotificationContent notificationContent,
            CancellationToken cancellationToken)
        {
            if (!emailAddresses.Any())
            {
                this.logger.LogWarning("No one email addresses for email notification found.");
                return;
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(
                this.smtpSettings.Host,
                this.smtpSettings.Port,
                this.smtpSettings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);
            if (!string.IsNullOrEmpty(this.smtpSettings.UserName))
            {
                await client.AuthenticateAsync(this.smtpSettings.UserName, this.smtpSettings.Password,
                    cancellationToken);
            }
            else
            {
                this.logger.LogInformation("Smtp client authentication skipped (user name is empty).");
            }

            foreach (var emailAddress in emailAddresses)
            {
                await this.SendMessage(client, emailAddress, notificationContent, cancellationToken);
            }

            await client.DisconnectAsync(true, cancellationToken);

            this.logger.LogDebug("Email for {EmailsCount} addresses was sent", emailAddresses.Length);
        }

        private async Task SendMessage(SmtpClient client, string emailAddress, EmailNotificationContent notificationContent, CancellationToken cancellationToken)
        {
            try
            {
                var emailMessage = this.CreateMimeMessage(emailAddress, notificationContent);
                await client.SendAsync(emailMessage, cancellationToken);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Email notification send to address '{EmailAddress}' error.", emailAddress);
            }
        }

        private MimeMessage CreateMimeMessage(
            string emailAddress, 
            EmailNotificationContent notificationContent)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress(this.emailNotificationSettings.ArcadiaAssistantFrom));
            mimeMessage.Subject = notificationContent.Subject;
            mimeMessage.Body = new TextPart("plain") { Text = notificationContent.Body };
            mimeMessage.To.Add(new MailboxAddress(emailAddress));

            return mimeMessage;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}