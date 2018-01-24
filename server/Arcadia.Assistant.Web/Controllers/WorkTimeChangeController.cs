namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Arcadia.Assistant.Web.Models.Calendar;

    using Microsoft.AspNetCore.Mvc;

    [Route("/api/employee/{employeeId}/work-time-changes/")]
    public class WorkTimeChangeController : Controller
    {
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<WorktimeChange.WithId>), (int)HttpStatusCode.OK)]
        public IActionResult GetAll(string employeeId)
        {
            return this.Ok(new[] { new WorktimeChange.WithId() });
        }


        [HttpGet]
        [Route("{changeId}")]
        [ProducesResponseType(typeof(WorktimeChange.WithId), 200)]
        public IActionResult GetById(string employeeId, string changeId)
        {
            return this.Ok(new WorktimeChange.WithId());
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(WorktimeChange), 201)]
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult Update(string employeeId, string changeId, [FromBody] WorktimeChange model)
        {
            return this.NoContent();
        }
    }
}