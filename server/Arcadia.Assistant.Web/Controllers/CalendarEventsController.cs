namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models.Calendar;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("/api/employees/{employeeId}/events/")]
    public class CalendarEventsController : Controller
    {
        private readonly IActorRefFactory actorSystem;

        private readonly ActorPathsBuilder pathsBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        public CalendarEventsController(IActorRefFactory actorSystem, ActorPathsBuilder pathsBuilder, ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.pathsBuilder = pathsBuilder;
            this.timeoutSettings = timeoutSettings;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventsWithIdModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(string employeeId, CancellationToken token)
        {
            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);

            if (employee == null)
            {
                return this.NotFound();
            }

            var events = await employee.Calendar.Ask<GetCalendarEvents.Response>(GetCalendarEvents.Instance, this.timeoutSettings.Timeout, token);
            var eventModels = events.Events
                .Select(x => new CalendarEventsWithIdModel(x.EventId, x.Type, x.Dates, x.Status));

            return this.Ok(eventModels);
        }

        [Route("{eventId}")]
        [HttpGet]
        [ProducesResponseType(typeof(CalendarEventsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string employeeId, string eventId, CancellationToken token)
        {
            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);

            if (employee == null)
            {
                return this.NotFound();
            }

            var requestedEvent = await this.GetCalendarEventOrDefaultAsync(employee.Calendar, eventId, token);

            if (requestedEvent == null)
            {
                return this.NotFound();
            }

            var eventModel = new CalendarEventsModel()
            {
                Dates = requestedEvent.Dates,
                Status = requestedEvent.Status,
                Type = requestedEvent.Type
            };

            return this.Ok(eventModel);
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(CalendarEventsWithIdModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(string employeeId, [FromBody] CalendarEventsModel model, CancellationToken token)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var newId = Guid.NewGuid().ToString();

            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);

            if (employee == null)
            {
                return this.NotFound();
            }

            var calendarEvent = new CalendarEvent(newId, model.Type, model.Dates, model.Status);
            var eventCreationResponse = await this.UpsertEventAsync(employee.Calendar, calendarEvent, token);

            var createdEvent = eventCreationResponse.Event; //TODO: add errors handling
            var responseObject = new CalendarEventsWithIdModel(createdEvent.EventId, createdEvent.Type, createdEvent.Dates, createdEvent.Status);

            return this.AcceptedAtAction(nameof(this.Get), new { eventId = responseObject.CalendarEventId }, responseObject);
        }

        [Route("{eventId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string employeeId, string eventId, [FromBody] CalendarEventsModel model, CancellationToken token)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);

            if (employee == null)
            {
                return this.NotFound();
            }

            var existingEvent = await this.GetCalendarEventOrDefaultAsync(employee.Calendar, eventId, token);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            var calendarEvent = new CalendarEvent(eventId, model.Type, model.Dates, model.Status);
            await this.UpsertEventAsync(employee.Calendar, calendarEvent, token);//TODO: add errors handling

            return this.NoContent();
        }

        private async Task<UpsertCalendarEvent.Response> UpsertEventAsync(IActorRef calendarActor, CalendarEvent calendarEvent, CancellationToken token)
        {
            var timeout = this.timeoutSettings.Timeout;
            var eventCreationResponse = await calendarActor.Ask<UpsertCalendarEvent.Response>(new UpsertCalendarEvent(calendarEvent), timeout, token);
            return eventCreationResponse;
        }

        private async Task<CalendarEvent> GetCalendarEventOrDefaultAsync(IActorRef calendarActor, string eventId, CancellationToken token)
        {
            var timeout = this.timeoutSettings.Timeout;
            var calendarEvent = await calendarActor.Ask<GetCalendarEvent.Response>(new GetCalendarEvent(eventId), timeout, token);
            switch (calendarEvent)
            {
                case GetCalendarEvent.Response.Found response:
                    return response.Event;

                case GetCalendarEvent.Response.NotFound _:
                    return null;

                default:
                    return null;
            }
        }

        private async Task<EmployeeContainer> GetEmployeeOrDefaultAsync(string employeeId, CancellationToken token)
        {
            var query = new EmployeesQuery().WithId(employeeId);

            var timeout = this.timeoutSettings.Timeout;

            //TODO: GET RID OF THAT COPY-PASTE!
            var organization = this.actorSystem.ActorSelection(this.pathsBuilder.Get("organization"));
            var response = await organization.Ask<EmployeesQuery.Response>(query, timeout, token);

            return response.Employees.SingleOrDefault();
        }
    }
}