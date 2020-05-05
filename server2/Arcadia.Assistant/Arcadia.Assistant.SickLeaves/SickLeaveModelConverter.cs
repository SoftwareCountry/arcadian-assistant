namespace Arcadia.Assistant.SickLeaves
{
    using System.Linq;

    using Contracts;

    using CSP.Model;

    using Employees.Contracts;

    public class SickLeaveModelConverter
    {
        public SickLeaveDescription GetDescription(SickLeave model)
        {
            return new SickLeaveDescription
            {
                SickLeaveId = model.Id,
                EmployeeId = new EmployeeId(model.EmployeeId),
                StartDate = model.Start,
                EndDate = model.End,
                Status = this.GetStatus(model)
            };
        }

        public SickLeaveStatus GetStatus(SickLeave model)
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