namespace Arcadia.Assistant.EmailNotifications
{
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

        public EmailNotifications(
            StatelessServiceContext context,
            SmtpSettings smtpSettings,
            ILogger<EmailNotifications> logger)
            : base(context)
        {
            this.smtpSettings = smtpSettings;
            this.logger = logger;
        }

        public async Task SendEmailNotification(
            string[] recipients,
            EmailNotificationContent notificationContent,
            CancellationToken cancellationToken)
        {
            this.logger.LogDebug("Email notification message received");

            using var client = new SmtpClient();
            var emailMessage = this.CreateMimeMessage(recipients, notificationContent);
            await client.ConnectAsync(
                this.smtpSettings.Host,
                this.smtpSettings.Port,
                this.smtpSettings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);
            await client.AuthenticateAsync(this.smtpSettings.UserName, this.smtpSettings.Password, cancellationToken);
            await client.SendAsync(emailMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            this.logger.LogDebug("Email was successfully sent");
        }

        private MimeMessage CreateMimeMessage(
            IEnumerable<string> recipients, EmailNotificationContent notificationContent)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress(notificationContent.Sender));
            mimeMessage.Subject = notificationContent.Subject;
            mimeMessage.Body = new TextPart("plain") { Text = notificationContent.Body };
            mimeMessage.To.AddRange(recipients.Select(x => new MailboxAddress(x)));

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