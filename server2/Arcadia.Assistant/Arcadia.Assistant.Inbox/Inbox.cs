namespace Arcadia.Assistant.Inbox
{
    using Contracts;
    using MailKit;
    using MailKit.Net.Imap;
    using MailKit.Search;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using MimeKit;
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Inbox : StatelessService, IInbox
    {
        private readonly ImapConfiguration imapConfiguration;

        public Inbox(StatelessServiceContext context, ImapConfiguration imapConfiguration)
            : base(context)
        {
            this.imapConfiguration = imapConfiguration;
        }

        public async Task<Email[]> GetEmailsAsync(EmailSearchQuery query, CancellationToken cancellationToken)
        {
            //this.logger.Debug("Loading inbox emails started");

            //this.logger.Debug($"Inbox emails query: {query}");

            using (var client = new ImapClient())
            {
                await client.ConnectAsync(
                    this.imapConfiguration.Host,
                    this.imapConfiguration.Port,
                    cancellationToken: cancellationToken);
                await client.AuthenticateAsync(this.imapConfiguration.User, this.imapConfiguration.Password, cancellationToken);

                await client.Inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken);

                var inboxQuery = new SearchQuery();

                if (!string.IsNullOrWhiteSpace(query.Subject))
                {
                    inboxQuery = inboxQuery.And(SearchQuery.SubjectContains(query.Subject));
                }

                var ids = await client.Inbox.SearchAsync(inboxQuery, cancellationToken);

                if (query.MinId != null)
                {
                    ids = ids.Where(x => x.Id > query.MinId).ToList();
                }

                var messages = (await client.Inbox
                        .FetchAsync(ids, MessageSummaryItems.BodyStructure | MessageSummaryItems.Envelope, cancellationToken))
                    .OrderByDescending(m => m.Date)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(query.Sender))
                {
                    messages = messages
                        .Where(m => m.Envelope.From.Any(f => f.ToString().Contains(query.Sender)))
                        .ToList();
                }

                if (query.LastNEmails != null)
                {
                    messages = messages
                        .Take((int)query.LastNEmails.Value)
                        .ToList();
                }

                //   this.logger.Debug($"Total messages loaded: {messages.Count}");

                var emails = this.ConvertMessages(client, messages);

                await client.DisconnectAsync(true, cancellationToken);

                return emails;
            }
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        private Email[] ConvertMessages(ImapClient client, IEnumerable<IMessageSummary> messages)
        {
            var emails = messages
                .Select(m =>
                {
                    var date = m.Envelope.Date ?? DateTimeOffset.Now;
                    var sender = m.Envelope.From.ToString();
                    var subject = m.NormalizedSubject;

                    var textPart = (TextPart)client.Inbox.GetBodyPart(m.UniqueId, m.TextBody);
                    var text = textPart.Text;

                    var attachments = m.Attachments
                        .Select(a =>
                        {
                            var attachmentPart = client.Inbox.GetBodyPart(m.UniqueId, a);
                            var stream = new MemoryStream();

                            if (attachmentPart is MessagePart messagePart)
                            {
                                messagePart.Message.WriteTo(stream);
                            }
                            else
                            {
                                var mimePart = (MimePart)attachmentPart;
                                mimePart.Content.DecodeTo(stream);
                            }

                            return stream.ToArray();
                        })
                        .ToArray();

                    return new Email(m.UniqueId.Id, date, sender, subject, text, attachments);
                })
                .ToArray();

            return emails;
        }
    }
}