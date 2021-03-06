﻿namespace Arcadia.Assistant.Web.Controllers
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
    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Authorization.Requirements;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Employees;
    using Arcadia.Assistant.Web.Models.Calendar;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using NLog;

    [Route("/api/employees/{employeeId}/events/")]
    [Authorize(Policies.UserIsEmployee)]
    public class CalendarEventsController : Controller
    {
        private readonly ITimeoutSettings timeoutSettings;
        private readonly IEmployeesRegistry employeesRegistry;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserEmployeeSearch userEmployeeSearch;

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public CalendarEventsController(
            ITimeoutSettings timeoutSettings,
            IEmployeesRegistry employeesRegistry,
            IAuthorizationService authorizationService,
            IUserEmployeeSearch userEmployeeSearch)
        {
            this.timeoutSettings = timeoutSettings;
            this.employeesRegistry = employeesRegistry;
            this.authorizationService = authorizationService;
            this.userEmployeeSearch = userEmployeeSearch;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CalendarEventsWithIdModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(string employeeId, CancellationToken token)
        {
            this.logger.Trace($"Start loading of calendar events for user {this.User.Identity.Name}");

            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);

            this.logger.Trace($"Employee is loaded for user {this.User.Identity.Name}");

            if (employee == null)
            {
                return this.NotFound();
            }

            this.logger.Trace($"Start checking permissions for user {this.User.Identity.Name}");

            var authorizationResult = await this.authorizationService.AuthorizeAsync(this.User, employee, new ReadCalendarEvents());

            this.logger.Trace($"Permissions checked for user {this.User.Identity.Name}");

            if (!authorizationResult.Succeeded)
            {
                return this.Ok(new CalendarEventsWithIdModel[0]);
            }

            this.logger.Trace($"Start loading calendar events for user {this.User.Identity.Name}");

            var events = await employee.Calendar.CalendarActor.Ask<GetCalendarEvents.Response>(GetCalendarEvents.Instance, this.timeoutSettings.Timeout, token);

            this.logger.Trace($"Calendar events loaded for user {this.User.Identity.Name}");

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

            if (!(await this.authorizationService.AuthorizeAsync(this.User, employee, new ReadCalendarEvents())).Succeeded)
            {
                return this.Ok(new CalendarEventsWithIdModel[0]);
            }

            var requestedEvent = await this.GetCalendarEventOrDefaultAsync(employee.Calendar.CalendarActor, eventId, token);

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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


            if (!(await this.authorizationService.AuthorizeAsync(this.User, employee, new CreateCalendarEvents())).Succeeded)
            {
                return this.Forbid();
            }

            var calendarEvent = new CalendarEvent(
                newId,
                model.Type,
                model.Dates,
                model.Status,
                employee.Metadata.EmployeeId);
            var eventCreationResponse = await this.UpsertEventAsync(employee.Calendar.CalendarActor, calendarEvent, token);

            switch (eventCreationResponse)
            {
                case UpsertCalendarEvent.Success success:
                    var createdEvent = success.Event;
                    var responseObject = new CalendarEventsWithIdModel(createdEvent.EventId, createdEvent.Type, createdEvent.Dates, createdEvent.Status);

                    return this.AcceptedAtAction(nameof(this.Get), new { eventId = responseObject.CalendarEventId }, responseObject);

                case UpsertCalendarEvent.Error error:
                    return this.BadRequest(error.Message);

                default:
                    return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("{eventId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

            var existingEvent = await this.GetCalendarEventOrDefaultAsync(employee.Calendar.CalendarActor, eventId, token);
            if (existingEvent == null)
            {
                return this.NotFound();
            }

            var calendarEventsRequirement = existingEvent.IsPending
                ? (IAuthorizationRequirement)new EditPendingCalendarEvents()
                : new EditCalendarEvents(existingEvent, model);

            var hasPermissions = (await this.authorizationService.AuthorizeAsync(this.User, employee, calendarEventsRequirement)).Succeeded;
            if (!hasPermissions)
            {
                return this.Forbid();
            }

            if (existingEvent.Type != model.Type)
            {
                return this.StatusCode(StatusCodes.Status409Conflict, "Calendar types are not compatible");
            }

            var calendarEvent = new CalendarEvent(
                eventId,
                existingEvent.Type,
                model.Dates,
                model.Status,
                existingEvent.EmployeeId);

            var response = await this.UpsertEventAsync(employee.Calendar.CalendarActor, calendarEvent, token);

            switch (response)
            {
                case UpsertCalendarEvent.Success _:
                    return this.NoContent();

                case UpsertCalendarEvent.Error error:
                    return this.BadRequest(error.Message);

                default:
                    return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        private async Task<UpsertCalendarEvent.Response> UpsertEventAsync(IActorRef calendarActor, CalendarEvent calendarEvent, CancellationToken token)
        {
            var userEmployee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, token);
            var timeout = this.timeoutSettings.Timeout;
            var timestamp = DateTimeOffset.Now;
            var eventCreationResponse = await calendarActor.Ask<UpsertCalendarEvent.Response>(
                new UpsertCalendarEvent(calendarEvent, userEmployee.Metadata.EmployeeId, timestamp),
                timeout,
                token);
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
            var employees = await this.employeesRegistry.SearchAsync(query, token);

            return employees.SingleOrDefault();
        }
    }
}