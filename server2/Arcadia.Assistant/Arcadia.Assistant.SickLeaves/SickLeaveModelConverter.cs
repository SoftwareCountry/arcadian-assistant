namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Contracts;

    using Employees.Contracts;

    public class SickLeaveModelConverter
    {
        public Expression<Func<CSP.Model.SickLeave, SickLeaveDescription>> ToDescription { get; }
            = model => new SickLeaveDescription()
            {
                SickLeaveId = model.Id,
                EmployeeId = new EmployeeId(model.EmployeeId),
                StartDate = model.Start,
                EndDate = model.End,
                Status = model.SickLeaveCancellations.Any() ? SickLeaveStatus.Cancelled
                    : model.SickLeaveCompletes.Any() ? SickLeaveStatus.Completed
                    : SickLeaveStatus.Requested
            };
    }
}