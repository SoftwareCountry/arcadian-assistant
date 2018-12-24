namespace Arcadia.Assistant.InboxEmail
{
    using System;
    using System.Threading.Tasks;

    using MailKit.Net.Imap;
    using MailKit.Security;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;

    public class InboxEmailActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly IImapSettings imapSettings;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public InboxEmailActor(IImapSettings imapSettings)
        {
            this.imapSettings = imapSettings;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(imapSettings.RefreshIntervalMinutes),
                this.Self,
                LoadInboxEmails.Instance,
                this.Self);
        }

        public IStash Stash { get; set; }

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

        private void LoadingInboxEmails(object message)
        {
            switch (message)
            {
                case LoadInboxEmailsStart _:
                    this.GetEmails().PipeTo(
                        this.Self,
                        success: () => LoadInboxEmailsFinish.Instance,
                        failure: err => new LoadInboxEmailsFinish(err.Message));
                    break;


                case LoadInboxEmailsFinish msg:
                    if (msg.Message != null)
                    {
                        this.logger.Warning(msg.Message);
                    }

                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();

                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private async Task GetEmails()
        {
            this.logger.Debug("Loading inbox emails started");

            using (var client = new ImapClient())
            {
                await client.ConnectAsync(
                    this.imapSettings.Host,
                    this.imapSettings.Port,
                    this.imapSettings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                await client.AuthenticateAsync(this.imapSettings.User, this.imapSettings.Password);

                client.Disconnect(true);
            }

            this.logger.Debug("Loading inbox emails finished");
        }

        private class LoadInboxEmails
        {
            public static readonly LoadInboxEmails Instance = new LoadInboxEmails();
        }

        private class LoadInboxEmailsStart
        {
            public static readonly LoadInboxEmailsStart Instance = new LoadInboxEmailsStart();
        }

        private class LoadInboxEmailsFinish
        {
            public static readonly LoadInboxEmailsFinish Instance = new LoadInboxEmailsFinish();

            public LoadInboxEmailsFinish(string message = null)
            {
                this.Message = message;
            }

            public string Message { get; }
        }
    }
}