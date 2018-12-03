namespace Arcadia.Assistant.CSP
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class CspCalendarEventsApprovalsChecker : CalendarEventsApprovalsChecker
    {
        private const string DepartmentsStorageActorPath = @"/user/organization/departments/departments-storage";
        private const string EmployeesStorageActorPath = @"/user/organization/employees/employees-storage";

        private readonly Regex sdoDepartmentRegex = new Regex(@"SDO\..+\..+");

        protected override async Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals)
        {
            var employeesActor = Context.ActorSelection(EmployeesStorageActorPath);
            var departmentsActor = Context.ActorSelection(DepartmentsStorageActorPath);

            var allDepartments = await this.GetDepartments(departmentsActor);
            var employeeDepartmentId = await this.GetEmployeeDepartmentId(employeesActor, employeeId);

            var employeeDepartment = allDepartments.First(d => d.DepartmentId == employeeDepartmentId);
            var parentDepartment = allDepartments.FirstOrDefault(d => d.DepartmentId == employeeDepartment.ParentDepartmentId);
            var isEmployeeChief = employeeDepartment.ChiefId == employeeId;

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
            else if (employeeDepartment.IsHeadDepartment && employeeDepartment.ChiefId == employeeId)
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

        private async Task<string> GetEmployeeDepartmentId(ActorSelection employeesActor, string employeeId)
        {
            var allEmployees = await employeesActor.Ask<EmployeesInfoStorage.LoadAllEmployees.Response>(
                EmployeesInfoStorage.LoadAllEmployees.Instance
            );

            return allEmployees.Employees
                .FirstOrDefault(e => e.Metadata.EmployeeId == employeeId)
                ?.Metadata.DepartmentId;
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