namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;

    using Arcadia.Assistant.Web.Models.Calendar;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("/api/employees/{employeeId}/work-time-changes/")]
    public class WorkTimeChangeController : Controller
    {
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<WorktimeChangeWithIdModel>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string employeeId)
        {
            return this.Ok(new[] { new WorktimeChangeWithIdModel() });
        }


        [HttpGet]
        [Route("{changeId}")]
        [ProducesResponseType(typeof(WorktimeChangeModel), StatusCodes.Status200OK)]
        public IActionResult GetById(string employeeId, string changeId)
        {
            return this.Ok(new WorktimeChangeModel()
                {
                    AdditionalWorkHours = 4,
                    Date = DateTime.Today.AddDays(2),
                    StartHour = 4,
                    Status = CalendarEventStatus.Approved
                });
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(WorktimeChangeWithIdModel), StatusCodes.Status201Created)]
        public IActionResult Create(string employeeId, [FromBody] WorktimeChangeModel model)
        {
            var copy = new WorktimeChangeWithIdModel
            {
                    AdditionalWorkHours = model.AdditionalWorkHours,
                    Date = model.Date,
                    StartHour = model.StartHour
                };

            copy.WorktimeChangeId = Guid.NewGuid().ToString();
            return this.CreatedAtAction(nameof(this.GetById), new { employeeId, changeId = copy.WorktimeChangeId }, copy);
        }

        [HttpPut]
        [Route("{changeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(string employeeId, string changeId, [FromBody] WorktimeChangeModel model)
        {
            return this.NoContent();
        }
    }
}