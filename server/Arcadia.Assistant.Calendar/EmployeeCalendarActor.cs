namespace Arcadia.Assistant.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    /// <summary>
    /// Aggregate calendar actor, just forwards calls to proper actors
    /// </summary>
    public class EmployeeCalendarActor : UntypedActor, ILogReceive
    {
        private readonly string employeeId;

        private readonly IActorRef vacationsActor;

        private readonly IActorRef workHoursActor;

        private readonly IActorRef sickLeavesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EmployeeCalendarActor(string employeeId, IActorRef vacationsActor, IActorRef workHoursActor, IActorRef sickLeavesActor)
        {
            this.employeeId = employeeId;
            this.vacationsActor = vacationsActor;
            this.workHoursActor = workHoursActor;
            this.sickLeavesActor = sickLeavesActor;
        }

        public static Props CreateProps(string employeeId, IActorRef vacationsActor, IActorRef workHoursActor, IActorRef sickLeavesActor)
        {
            return Props.Create(() => new EmployeeCalendarActor(employeeId, vacationsActor, workHoursActor, sickLeavesActor));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetCalendarEvents request:
                    this.logger.Debug("GetCalendarEvents message received in calendar actor");
                    this.FindAllCalendarEvents(request).PipeTo(this.Sender, this.Self);
                    break;

                case GetCalendarEvent request:
                    this.FindSpecificCalendarEvent(request).PipeTo(this.Sender, this.Self);
                    break;

                case UpsertCalendarEvent cmd when cmd.Event.EventId == null:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(new ArgumentNullException(nameof(cmd.Event.EventId)).ToString()));
                    break;

                case UpsertCalendarEvent cmd:
                    var actor = this.GetActorByEventType(cmd.Event.Type);
                    actor.Forward(cmd);
                    break;

                case GetCalendarEventApprovals cmd:
                    var getApprovalsActor = this.GetActorByEventType(cmd.Event.Type);
                    getApprovalsActor.Forward(cmd);
                    break;

                case ApproveCalendarEvent cmd:
                    var approveActor = this.GetActorByEventType(cmd.Event.Type);
                    approveActor.Forward(cmd);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<List<T>> GetActorResponses<T>(object request)
        {
            var responses = new List<T>();

            this.logger.Debug("Started loading of sick leaves");
            responses.Add(await this.sickLeavesActor.Ask<T>(request));
            this.logger.Debug("Sick leaves loaded");

            this.logger.Debug("Started loading of vacations");
            responses.Add(await this.vacationsActor.Ask<T>(request));
            this.logger.Debug("Vacations loaded");

            this.logger.Debug("Started loading of work hours");
            responses.Add(await this.workHoursActor.Ask<T>(request));
            this.logger.Debug("Work hours loaded");

            return responses;
        }

        private async Task<GetCalendarEvents.Response> FindAllCalendarEvents(GetCalendarEvents request)
        {
            this.logger.Debug("Started loading of all calendar events");
            var responses = await this.GetActorResponses<GetCalendarEvents.Response>(request);
            this.logger.Debug("All calendar events loaded");

            return new GetCalendarEvents.Response(this.employeeId, responses.SelectMany(x => x.Events).ToList());
        }

        private async Task<GetCalendarEvent.Response> FindSpecificCalendarEvent(GetCalendarEvent request)
        {
            var responses = await this.GetActorResponses<GetCalendarEvent.Response>(request);

            var result = responses.OfType<GetCalendarEvent.Response.Found>().FirstOrDefault()?.Event;
            if (result == null)
            {
                return new GetCalendarEvent.Response.NotFound();
            }
            else
            {
                return new GetCalendarEvent.Response.Found(result);
            }
        }


        private IActorRef GetActorByEventType(string calendarEventType)
        {
            switch (calendarEventType)
            {
                case CalendarEventTypes.Workout:
                case CalendarEventTypes.Dayoff:
                    return this.workHoursActor;

                case CalendarEventTypes.Vacation:
                    return this.vacationsActor;

                case CalendarEventTypes.Sickleave:
                    return this.sickLeavesActor;

                default:
                    return ActorRefs.Nobody;
            }
        }
    }
}