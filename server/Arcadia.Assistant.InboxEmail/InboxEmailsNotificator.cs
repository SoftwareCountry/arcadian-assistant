namespace Arcadia.Assistant.InboxEmail
{
    using System;
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
                    this.BroadcastNewEmails().PipeTo(
                        this.Self,
                        success: () => LoadInboxEmailsFinishSuccess.Instance,
                        failure: err => new LoadInboxEmailsFinishError(err));
                    break;

                case LoadInboxEmailsFinishSuccess _:
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();

                    break;

                case LoadInboxEmailsFinishError msg:
                    this.logger.Warning(msg.Exception.Message);

                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();

                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private async Task BroadcastNewEmails()
        {
            var emailsSearchQuery = EmailSearchQuery.Create();

            if (this.lastEmailId != null)
            {
                emailsSearchQuery.FromId(this.lastEmailId.Value);
            }

            var message = new GetInboxEmails(emailsSearchQuery);
            var newEmailsResponse = await this.inboxEmailActor.Ask<GetInboxEmails.Response>(message);

            switch (newEmailsResponse)
            {
                case GetInboxEmails.Error error:
                    this.logger.Warning(error.Exception.Message);
                    break;

                case GetInboxEmails.Success success:
                    if (this.lastEmailId == null)
                    {
                        this.lastEmailId = success.Emails.Max(e => e.UniqueId);
                    }
                    else
                    {
                        var emailsEventBus = new InboxEmailsEventBus(success.Emails.ToList());
                        Context.System.EventStream.Publish(emailsEventBus);
                    }
                    break;

                default:
                    this.logger.Warning("Unexpected inbox emails response");
                    break;
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
            public static readonly LoadInboxEmailsFinishSuccess Instance = new LoadInboxEmailsFinishSuccess();
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