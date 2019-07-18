namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Contracts;

    public class SickLeaveModelConverter
    {
        public Expression<Func<CSP.Model.SickLeaves, SickLeaveDescription>> ToDescription { get; }
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