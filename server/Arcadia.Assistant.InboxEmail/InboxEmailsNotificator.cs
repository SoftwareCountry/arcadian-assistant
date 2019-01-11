namespace Arcadia.Assistant.InboxEmail
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.InboxEmail.Abstractions;

    public class InboxEmailsNotificator : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly IActorRef inboxEmailActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private uint? lastEmailId;

        public InboxEmailsNotificator(IImapSettings imapSettings, IActorRef inboxEmailActor)
        {
            this.inboxEmailActor = inboxEmailActor;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(imapSettings.RefreshIntervalMinutes),
                this.Self,
                LoadInboxEmails.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case LoadInboxEmails _:
                    this.Self.Tell(LoadInboxEmailsStart.Instance);
                    this.BecomeStacked(this.LoadingInboxEmails);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        public IStash Stash { get; set; }

        private void LoadingInboxEmails(object message)
        {
            switch (message)
            {
                case LoadInboxEmailsStart _:
                    this.GetNewEmails().PipeTo(
                        this.Self,
                        success: x => new LoadInboxEmailsFinishSuccess(x),
                        failure: err => new LoadInboxEmailsFinishError(err));
                    break;

                case LoadInboxEmailsFinishSuccess msg:
                    if (msg.Emails.Any())
                    {
                        var emailsEventBus = new EmailsReceivedEventBus(msg.Emails.ToList());
                        Context.System.EventStream.Publish(emailsEventBus);
                    }

                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();

                    break;

                case LoadInboxEmailsFinishError msg:
                    this.logger.Warning(msg.Exception.ToString());

                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();

                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private async Task<IEnumerable<Email>> GetNewEmails()
        {
            var emailsSearchQuery = EmailSearchQuery.Create();

            if (this.lastEmailId != null)
            {
                emailsSearchQuery.FromId(this.lastEmailId.Value);
            }
            else
            {
                emailsSearchQuery.TakeLastNEmails(1);
            }

            var message = new GetInboxEmails(emailsSearchQuery);
            var newEmailsResponse = await this.inboxEmailActor.Ask<GetInboxEmails.Response>(message);

            switch (newEmailsResponse)
            {
                case GetInboxEmails.Error error:
                    throw new Exception("Loading inbox emails error", error.Exception);

                case GetInboxEmails.Success success:
                    this.lastEmailId = success.Emails.Max(e => e.UniqueId);

                    var newEmails = this.lastEmailId == null
                        ? Enumerable.Empty<Email>()
                        : success.Emails;

                    return newEmails;

                default:
                    throw new Exception($"Unexpected inbox emails response: {newEmailsResponse.GetType()}");
            }
        }

        private class LoadInboxEmails
        {
            public static readonly LoadInboxEmails Instance = new LoadInboxEmails();
        }

        private class LoadInboxEmailsStart
        {
            public static readonly LoadInboxEmailsStart Instance = new LoadInboxEmailsStart();
        }

        private class LoadInboxEmailsFinishSuccess
        {
            public LoadInboxEmailsFinishSuccess(IEnumerable<Email> emails)
            {
                this.Emails = emails;
            }

            public IEnumerable<Email> Emails { get; }
        }

        private class LoadInboxEmailsFinishError
        {
            public LoadInboxEmailsFinishError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}