namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Arcadia.Assistant.Web.Models.Calendar;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("/api/employees/{employeeId}/vacations/")]
    public class VacationsController : Controller
    {
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VacationModel.WithId>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string employeeId)
        {
            return this.Ok(Enumerable.Empty<VacationModel.WithId>());
        }

        [Route("{vacationId}")]
        [HttpGet]
        [ProducesResponseType(typeof(VacationModel.WithId), StatusCodes.Status200OK)]
        public IActionResult Get(string employeeId, string vacationId)
        {
            return this.Ok(new VacationModel.WithId()
                {
                    VacationId = Guid.NewGuid().ToString(),
                    Period = new DatesPeriod() { StartDate = DateTime.Now.AddDays(-7), EndDate = DateTime.Now.AddDays(4) },
                    Status = CalendarEventStatus.Approved
                });
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(VacationModel.WithId), StatusCodes.Status201Created)]
        public IActionResult Create(string employeeId, [FromBody] VacationModel model)
        {
            var newVacation = new VacationModel.WithId()
                {
                    VacationId = Guid.NewGuid().ToString(),
                    Period = model.Period,
                    Status = model.Status
                };

            return this.AcceptedAtAction(nameof(this.Get), new { vacationId = newVacation.VacationId }, newVacation);
        }

        [Route("{vacationId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(string employeeId, string vacationId, [FromBody] VacationModel model)
        {
            return this.NoContent();
        }
    }
}