namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventAddToPendingActions
    {
        public CalendarEventAddToPendingActions(CalendarEvent @event, string approverId)
        {
            this.Event = @event;
            this.ApproverId = approverId;
        }

        public CalendarEvent Event { get; }

        public string ApproverId { get; }
    }
}