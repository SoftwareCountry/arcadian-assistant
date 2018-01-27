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
        [ProducesResponseType(typeof(IEnumerable<WorktimeChange.WithId>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string employeeId)
        {
            return this.Ok(new[] { new WorktimeChange.WithId() });
        }


        [HttpGet]
        [Route("{changeId}")]
        [ProducesResponseType(typeof(WorktimeChange.WithId), StatusCodes.Status200OK)]
        public IActionResult GetById(string employeeId, string changeId)
        {
            return this.Ok(new WorktimeChange.WithId());
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(WorktimeChange), StatusCodes.Status201Created)]
        public IActionResult Create(string employeeId, [FromBody] WorktimeChange model)
        {
            var copy = new WorktimeChange.WithId()
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
        public IActionResult Update(string employeeId, string changeId, [FromBody] WorktimeChange model)
        {
            return this.NoContent();
        }
    }
}