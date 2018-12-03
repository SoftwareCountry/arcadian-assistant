namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class CalendarEventAssignedToApproverEventBusMessage
    {
        public CalendarEventAssignedToApproverEventBusMessage(CalendarEvent @event, string approverId)
        {
            this.Event = @event;
            this.ApproverId = approverId;
        }

        public CalendarEvent Event { get; }

        public string ApproverId { get; }
    }
}