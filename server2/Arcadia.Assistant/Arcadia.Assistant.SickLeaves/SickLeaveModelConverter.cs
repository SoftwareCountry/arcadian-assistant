namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Contracts;

    using Employees.Contracts;

    public class SickLeaveModelConverter
    {
        public SickLeaveDescription GetDescription(CSP.Model.SickLeave model)
        {
            return new SickLeaveDescription()
            {
                SickLeaveId = model.Id,
                EmployeeId = new EmployeeId(model.EmployeeId),
                StartDate = model.Start,
                EndDate = model.End,
                Status = GetStatus(model),
            };
        }

        public SickLeaveStatus GetStatus(CSP.Model.SickLeave model)
        {
            return model.SickLeaveCancellations.Select(x => new
                {
                    DateTime = x.At, Status = SickLeaveStatus.Cancelled
                }).Union(model.SickLeaveCompletes.Select(x => new
                {
                    DateTime = x.At, Status = SickLeaveStatus.Completed
                }))
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefault()
                ?.Status ?? SickLeaveStatus.Requested;

        }
    }
}