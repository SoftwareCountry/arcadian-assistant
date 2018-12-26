namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.InboxEmail;
    using Arcadia.Assistant.InboxEmail.Abstractions;

    public class VacationsEmailLoader : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private const string InboxEmailActorPath = @"/user/inbox-emails";

        private readonly VacationsEmailLoaderConfiguration configuration;
        private readonly ActorSelection inboxEmailActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private List<EmployeeVacationRecord> vacationsInfo = new List<EmployeeVacationRecord>();

        public static Props CreateProps() => Context.DI().Props<VacationsEmailLoader>();

        public VacationsEmailLoader(VacationsEmailLoaderConfiguration configuration)
        {
            this.configuration = configuration;
            this.inboxEmailActor = Context.ActorSelection(InboxEmailActorPath);

            this.Self.Tell(LoadInitialState.Instance);
        }

        public IStash Stash { get; set; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case LoadInitialState _:
                    var emailsQuery = EmailSearchQuery.Create()
                        .WithSender(this.configuration.Sender)
                        .WithSubject(this.configuration.Subject);
                    this.inboxEmailActor.Ask<GetInboxEmails.Response>(new GetInboxEmails(emailsQuery))
                        .PipeTo(
                            this.Self,
                            success: x =>
                            {
                                switch (x)
                                {
                                    case GetInboxEmails.Success msg:
                                        return new LoadInitialState.Success(msg.Emails);

                                    case GetInboxEmails.Error msg:
                                        return new LoadInitialState.Error(msg.Exception);

                                    default:
                                        return new LoadInitialState.Error(
                                            new Exception("Unexpected inbox emails response"));
                                }
                            },
                            failure: err => new LoadInitialState.Error(err)
                        );
                    break;

                case LoadInitialState.Success msg:
                    this.LoadVacationsFromEmails(msg.Emails);
                    this.BecomeAfterInitial();
                    break;

                case LoadInitialState.Error msg:
                    this.logger.Warning(msg.Exception.Message);
                    this.BecomeAfterInitial();
                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void BecomeAfterInitial()
        {
            this.Stash.UnstashAll();
            Context.System.EventStream.Subscribe<InboxEmailsEventBus>(this.Self);
            this.Become(this.AfterInitialState);
        }

        private void AfterInitialState(object message)
        {
            switch (message)
            {
                case GetVacationsInfo _:
                    var result = new GetVacationsInfo.Success(this.vacationsInfo);
                    this.Sender.Tell(result);
                    break;

                case InboxEmailsEventBus msg:
                    this.LoadVacationsFromEmails(msg.Emails);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void LoadVacationsFromEmails(IEnumerable<Email> emails)
        {
            var lastActualEmail = emails
                .Where(this.CheckEmail)
                .OrderByDescending(e => e.Date)
                .FirstOrDefault();
            if (lastActualEmail == null)
            {
                return;
            }

            this.logger.Debug(
                $"Received vacations email from {lastActualEmail.Sender} with subject {lastActualEmail.Subject}"
            );

            var vacationsAttachment = Encoding.UTF8.GetString(lastActualEmail.Attachments.First());
            this.vacationsInfo = this.ParseVacations(vacationsAttachment);
        }

        private List<EmployeeVacationRecord> ParseVacations(string vacationsAttachment)
        {
            var lines = vacationsAttachment.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var employeeLines = lines
                .Select(l => l.Split(new[] { "\t" }, StringSplitOptions.None))
                .Where(l => l.Length == 4)
                .Skip(2); // Headers line and next line with tabs

            var vacations = employeeLines
                .Select(l => new
                {
                    Name = l[0],
                    Email = l[1],
                    MainVacations = l[2],
                    TotalVacations = l[3]
                })
                .Where(v => !string.IsNullOrWhiteSpace(v.Email) && !string.IsNullOrWhiteSpace(v.MainVacations))
                .Select(v => new EmployeeVacationRecord
                {
                    Id = v.Email,
                    VacationDaysCount = double.Parse(v.MainVacations)
                })
                .ToList();
            return vacations;
        }

        private bool CheckEmail(Email email)
        {
            return
                email.Sender.Contains(this.configuration.Sender) &&
                email.Subject.Contains(this.configuration.Subject);
        }

        private class LoadInitialState
        {
            public static readonly LoadInitialState Instance = new LoadInitialState();

            public class Success
            {
                public Success(IEnumerable<Email> emails)
                {
                    this.Emails = emails;
                }

                public IEnumerable<Email> Emails { get; }
            }

            public class Error
            {
                public Error(Exception exception)
                {
                    this.Exception = exception;
                }

                public Exception Exception { get; }
            }
        }

        public class GetVacationsInfo
        {
            public static readonly GetVacationsInfo Instance = new GetVacationsInfo();

            public abstract class Response
            {
            }

            public class Success : Response
            {
                public Success(IEnumerable<EmployeeVacationRecord> employeeVacations)
                {
                    this.EmployeeVacations = employeeVacations;
                }

                public IEnumerable<EmployeeVacationRecord> EmployeeVacations { get; }
            }

            public class Error : Response
            {
                public Error(Exception exception)
                {
                    this.Exception = exception;
                }

                public Exception Exception { get; }
            }
        }

        public class EmployeeVacationRecord
        {
            public string Id { get; set; }

            public double VacationDaysCount { get; set; }
        }
    }
}