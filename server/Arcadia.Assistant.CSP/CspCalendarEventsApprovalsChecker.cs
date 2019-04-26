namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP.Cache;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.Extensions.Caching.Memory;

    public class CspCalendarEventsApprovalsChecker : CalendarEventsApprovalsChecker
    {
        private readonly Regex sdoDepartmentRegex = new Regex(@"SDO\..+\..+");

        private readonly IActorRef departmentsActor;
        private readonly IActorRef employeesActor;

        public CspCalendarEventsApprovalsChecker(IMemoryCache memoryCache, IRefreshInformation refreshInformation)
        {
            this.departmentsActor = Context.ActorOf(CachedDepartmentsStorage.CreateProps(memoryCache, TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes), true));
            this.employeesActor = Context.ActorOf(CachedEmployeesInfoStorage.CreateProps(memoryCache, TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes), true));
        }

        protected override async Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals, string eventType)
        {
            var allDepartments = await this.GetDepartments();
            var employeeMetadata = await this.GetEmployee(employeeId);

            if (employeeMetadata == null)
            {
                return null;
            }

            switch (eventType)
            {
                case CalendarEventTypes.Vacation:
                    return this.GetNextApproverHierarchyStrategy(employeeMetadata, allDepartments, existingApprovals);

                default:
                    return this.GetNextApproverOnlyHeadStrategy(employeeMetadata, allDepartments, existingApprovals);
            }
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

            var existingApprovalsList = existingApprovals.ToList();
            var nextApprover = !isEmployeeChief ? ownDepartment.ChiefId : parentDepartment?.ChiefId;
            var acceptedApprovers = parentDepartments.Select(d => d.ChiefId);

            return existingApprovalsList.Contains(nextApprover) || existingApprovalsList.Intersect(acceptedApprovers).Count() != 0
                ? null
                : nextApprover;
        }

        private string GetNextApproverHierarchyStrategy(
            EmployeeMetadata employee,
            List<DepartmentInfo> departments,
            IEnumerable<string> existingApprovals)
        {
            var ownDepartment = departments.First(d => d.DepartmentId == employee.DepartmentId);
            var isEmployeeChief = ownDepartment.ChiefId == employee.EmployeeId;
            var parentDepartment = departments.FirstOrDefault(d => d.DepartmentId == ownDepartment.ParentDepartmentId);
            var parentDepartments = this.GetParentDepartments(ownDepartment, departments);

            string finalApprover = null;
            string preliminaryApprover = null;

            if (this.sdoDepartmentRegex.Match(ownDepartment.Name).Success)
            {
                if (!isEmployeeChief)
                {
                    preliminaryApprover = ownDepartment.ChiefId;
                }

                finalApprover = parentDepartment?.ChiefId;
            }
            else if (ownDepartment.IsHeadDepartment && isEmployeeChief)
            {
                // No approvals required for Director General
            }
            else
            {
                if (!isEmployeeChief)
                {
                    finalApprover = ownDepartment.ChiefId;
                }
                else
                {
                    finalApprover = parentDepartment?.ChiefId;
                }
            }

            var existingApprovalsList = existingApprovals.ToList();
            var acceptedFinalApprovers = parentDepartments.Select(d => d.ChiefId);

            if (existingApprovalsList.Contains(finalApprover) ||
                existingApprovalsList.Intersect(acceptedFinalApprovers).Count() != 0)
            {
                return null;
            }

            if (existingApprovalsList.Contains(preliminaryApprover))
            {
                return finalApprover;
            }

            return preliminaryApprover ?? finalApprover;
        }

        private async Task<EmployeeMetadata> GetEmployee(string employeeId)
        {
            var allEmployees = await employeesActor.Ask<EmployeesInfoStorage.LoadAllEmployees.Response>(
                EmployeesInfoStorage.LoadAllEmployees.Instance
            );

            return allEmployees.Employees
                .FirstOrDefault(e => e.Metadata.EmployeeId == employeeId)
                ?.Metadata;
        }

        private async Task<List<DepartmentInfo>> GetDepartments()
        {
            var allDepartmentsResponse = await departmentsActor.Ask<DepartmentsStorage.LoadAllDepartments.Response>(
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
