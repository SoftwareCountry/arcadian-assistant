namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeVacationsActor : UntypedActor, ILogReceive
    {
        private readonly string employeeId;
        private readonly IActorRef employeeFeed;
        private readonly IActorRef vacationsCreditRegistry;
        private readonly IActorRef vacationsRegistry;

        public EmployeeVacationsActor(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsCreditRegistry,
            IEmployeeVacationsRegistryPropsFactory vacationsRegistryPropsFactory
        )
        {
            this.employeeId = employeeId;
            this.employeeFeed = employeeFeed;
            this.vacationsCreditRegistry = vacationsCreditRegistry;
            this.vacationsRegistry = Context.ActorOf(
                vacationsRegistryPropsFactory.CreateProps(employeeId),
                "vacations-registry");

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        public static Props CreateProps(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsCreditRegistry,
            IEmployeeVacationsRegistryPropsFactory vacationsRegistryPropsFactory)
        {
            return Props.Create(() => new EmployeeVacationsActor(
                employeeId,
                employeeFeed,
                vacationsCreditRegistry,
                vacationsRegistryPropsFactory)
            );
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg when
                    msg.NewEvent.Status == VacationStatuses.Approved &&
                    msg.NewEvent.EmployeeId == this.employeeId:

                    var text = $"Vacation approved from {msg.NewEvent.Dates.StartDate.ToLongDateString()} to {msg.NewEvent.Dates.EndDate.ToLongDateString()}";
                    var feedMessage = new Message(Guid.NewGuid().ToString(), this.employeeId, "Vacation", text, msg.Timestamp.Date);
                    this.employeeFeed.Tell(new PostMessage(feedMessage));

                    break;

                case GetVacationsCredit _:
                    this.vacationsCreditRegistry
                        .Ask<VacationsCreditRegistry.GetVacationInfo.Response>(new VacationsCreditRegistry.GetVacationInfo(this.employeeId))
                        .ContinueWith(x => new GetVacationsCredit.Response(x.Result.VacationsCredit))
                        .PipeTo(this.Sender);
                    break;

                case GetCalendarEvents msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case GetCalendarEvent msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case UpsertCalendarEvent msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case GetCalendarEventApprovals msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case ApproveCalendarEvent msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}