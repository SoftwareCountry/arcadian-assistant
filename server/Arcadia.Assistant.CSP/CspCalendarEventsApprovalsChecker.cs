namespace Arcadia.Assistant.CSP
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions;

    public class CspCalendarEventsApprovalsChecker : CalendarEventsApprovalsChecker
    {
        private const string DepartmentsStorageActorPath = @"/user/organization/departments/departments-storage";
        private const string EmployeesStorageActorPath = @"/user/organization/employees/employees-storage";

        private readonly Regex sdoDepartmentRegex = new Regex(@"SDO\..+\..+");

        protected override async Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals, string eventType)
        {
            var employeesActor = Context.ActorSelection(EmployeesStorageActorPath);
            var departmentsActor = Context.ActorSelection(DepartmentsStorageActorPath);

            var allDepartments = await this.GetDepartments(departmentsActor);
            var employeeMetadata = await this.GetEmployee(employeesActor, employeeId);
            
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
            var employeeDepartment = departments.First(d => d.DepartmentId == employee.DepartmentId);
            var parentDepartment = departments.FirstOrDefault(d => d.DepartmentId == employeeDepartment.ParentDepartmentId);
            var isEmployeeChief = employeeDepartment.ChiefId == employee.EmployeeId;

            if (employeeDepartment.IsHeadDepartment && isEmployeeChief)
            {
                // No approvals required for Director General
                return null;
            }

            var approver = !isEmployeeChief ? employeeDepartment.ChiefId : parentDepartment?.ChiefId;

            return existingApprovals.Contains(approver) ? null : approver;
        }

        private string GetNextApproverHierarchyStrategy(
            EmployeeMetadata employee,
            List<DepartmentInfo> departments,
            IEnumerable<string> existingApprovals)
        {
            var employeeDepartment = departments.First(d => d.DepartmentId == employee.DepartmentId);
            var parentDepartment = departments.FirstOrDefault(d => d.DepartmentId == employeeDepartment.ParentDepartmentId);
            var isEmployeeChief = employeeDepartment.ChiefId == employee.EmployeeId;

            string finalApprover = null;
            string preliminaryApprover = null;

            if (this.sdoDepartmentRegex.Match(employeeDepartment.Name).Success)
            {
                if (!isEmployeeChief)
                {
                    preliminaryApprover = employeeDepartment.ChiefId;
                }

                finalApprover = parentDepartment?.ChiefId;
            }
            else if (employeeDepartment.IsHeadDepartment && isEmployeeChief)
            {
                // No approvals required for Director General
            }
            else
            {
                if (!isEmployeeChief)
                {
                    finalApprover = employeeDepartment.ChiefId;
                }
                else
                {
                    finalApprover = parentDepartment?.ChiefId;
                }
            }

            var existingApprovalsList = existingApprovals.ToList();

            if (existingApprovalsList.Contains(finalApprover))
            {
                return null;
            }

            if (existingApprovalsList.Contains(preliminaryApprover))
            {
                return finalApprover;
            }

            return preliminaryApprover ?? finalApprover;
        }

        private async Task<EmployeeMetadata> GetEmployee(ActorSelection employeesActor, string employeeId)
        {
            var allEmployees = await employeesActor.Ask<EmployeesInfoStorage.LoadAllEmployees.Response>(
                EmployeesInfoStorage.LoadAllEmployees.Instance
            );

            return allEmployees.Employees
                .FirstOrDefault(e => e.Metadata.EmployeeId == employeeId)
                ?.Metadata;
        }

        private async Task<List<DepartmentInfo>> GetDepartments(ActorSelection departmentsActor)
        {
            var allDepartmentsResponse = await departmentsActor.Ask<DepartmentsStorage.LoadAllDepartments.Response>(
                DepartmentsStorage.LoadAllDepartments.Instance
            );
            return allDepartmentsResponse.Departments.ToList();
        }
    }
}
