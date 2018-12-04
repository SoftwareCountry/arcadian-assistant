namespace Arcadia.Assistant.Organization.Abstractions
{
    using Akka.Actor;

    public class EmployeeCalendarContainer
    {
        public IActorRef CalendarActor { get; }

        public IActorRef VacationsActor { get; }

        public IActorRef WorkHoursActor { get; }

        public IActorRef SickLeavesActor { get; }

        public IActorRef PendingActionsActor { get; }

        public EmployeeCalendarContainer(
            IActorRef vacationsActor,
            IActorRef workHoursActor,
            IActorRef sickLeavesActor,
            IActorRef calendarActor,
            IActorRef pendingActionsActor)
        {
            this.VacationsActor = vacationsActor;
            this.WorkHoursActor = workHoursActor;
            this.SickLeavesActor = sickLeavesActor;
            this.CalendarActor = calendarActor;
            this.PendingActionsActor = pendingActionsActor;
        }
    }
}