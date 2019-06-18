namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP.Cache;
    using Arcadia.Assistant.Organization.Abstractions;

    public class CspCalendarEventsApprovalsChecker : CalendarEventsApprovalsChecker
    {
        private readonly IActorRef departmentsActor;
        private readonly IActorRef employeesActor;

        public CspCalendarEventsApprovalsChecker(MemoryCache memoryCache, IRefreshInformation refreshInformation)
        {
            this.departmentsActor = Context.ActorOf(CachedDepartmentsStorage.CreateProps(memoryCache, TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes), true));
            this.employeesActor = Context.ActorOf(CachedEmployeesInfoStorage.CreateProps(memoryCache, TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes), true));
        }

        protected override async Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals, string eventType)
        {
            // No approval required for sick leaves
            if (eventType == CalendarEventTypes.Sickleave)
            {
                return null;
            }

            var allDepartments = await this.GetDepartments();
            var employeeMetadata = await this.GetEmployee(employeeId);

            if (employeeMetadata == null)
            {
                return null;
            }

            return this.GetNextApproverOnlyHeadStrategy(employeeMetadata, allDepartments, existingApprovals);
        }

        private string GetNextApproverOnlyHeadStrategy(
            EmployeeMetadata employee,
            List<DepartmentInfo> departments,
            IEnumerable<string> existingApprovals)
        {
            var ownDepartment = departments.First(d => d.DepartmentId == employee.DepartmentId);
            var isEmployeeChief = ownDepartment.ChiefId == employee.EmployeeId;
            var parentDepartment = departments.First(d => d.DepartmentId == ownDepartment.ParentDepartmentId);
            var parentDepartments = this.GetParentDepartments(ownDepartment, departments);

            if (ownDepartment.IsHeadDepartment && isEmployeeChief)
            {
                // No approvals required for Director General
                return null;
            }

            var nextApprover = !isEmployeeChief ? ownDepartment.ChiefId : parentDepartment?.ChiefId;
            var acceptedApprovers = parentDepartments
                .Select(d => d.ChiefId)
                .Append(nextApprover);

            return existingApprovals.Intersect(acceptedApprovers).Count() != 0
                ? null
                : nextApprover;
        }

        private async Task<EmployeeMetadata> GetEmployee(string employeeId)
        {
            var allEmployees = await this.employeesActor.Ask<EmployeesInfoStorage.LoadAllEmployees.Response>(
                EmployeesInfoStorage.LoadAllEmployees.Instance
            );

            return allEmployees.Employees
                .FirstOrDefault(e => e.Metadata.EmployeeId == employeeId)
                ?.Metadata;
        }

        private async Task<List<DepartmentInfo>> GetDepartments()
        {
            var allDepartmentsResponse = await this.departmentsActor.Ask<DepartmentsStorage.LoadAllDepartments.Response>(
                DepartmentsStorage.LoadAllDepartments.Instance
            );
            return allDepartmentsResponse.Departments.ToList();
        }

        private List<DepartmentInfo> GetParentDepartments(
            DepartmentInfo childDepartment,
            List<DepartmentInfo> allDepartments)
        {
            if (childDepartment.IsHeadDepartment)
            {
                return new List<DepartmentInfo>();
            }

            var parentDepartment = allDepartments.First(d => d.DepartmentId == childDepartment.ParentDepartmentId);

            var result = new List<DepartmentInfo> { parentDepartment };
            result.AddRange(this.GetParentDepartments(parentDepartment, allDepartments));

            return result;
        }
    }
}