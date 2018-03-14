namespace Arcadia.Assistant.Feeds.Polls
{
    using System;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public abstract class EmployeesPoll : UntypedActor, ILogReceive
    {
        private readonly IActorRef organization;

        protected readonly ILoggingAdapter logger = Context.GetLogger();

        protected EmployeesPoll(IActorRef organization)
        {
            this.organization = organization;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case PerformPoll cmd:
                    this.organization.Tell(this.GetQuery(cmd.Date));
                    break;

                case EmployeesQuery.Response response:
                    this.OnPollFinished(response);
                    Context.Stop(this.Self);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract EmployeesQuery GetQuery(DateTime date);

        protected abstract void OnPollFinished(EmployeesQuery.Response employees);
    }
}