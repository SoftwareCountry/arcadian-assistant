namespace Arcadia.Assistant.SickLeaves
{
    using Contracts;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class SickLeaveModelConverter
    {
        public Expression<Func<CSP.Model.SickLeave, SickLeaveDescription>> ToDescription { get; }
            = model => new SickLeaveDescription()
            {
                SickLeaveId = model.Id,
                StartDate = model.Start,
                EndDate = model.End,
                Status = model.SickLeaveCancellations.Any() ? SickLeaveStatus.Cancelled
                    : model.SickLeaveCompletes.Any() ? SickLeaveStatus.Completed
                    : SickLeaveStatus.Requested
            };
    }
}