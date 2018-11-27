namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Configuration.Configuration;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Organization.Abstractions;

    public class CspVacationApprovalsChecker : VacationApprovalsChecker
    {
        private const string DepartmentsStorageActorPath = @"/user/organization/departments/departments-storage";

        private readonly Func<ArcadiaCspContext> contextFactory;
        private readonly AppSettings settings;

        private readonly Regex sdoDepartmentRegex = new Regex(@"SDO\..+\..+");

        public CspVacationApprovalsChecker(Func<ArcadiaCspContext> contextFactory, AppSettings settings)
        {
            this.contextFactory = contextFactory;
            this.settings = settings;
        }

        protected override async Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals)
        {
            var allDepartments = await this.GetDepartments();

            var employeeDepartmentId = await this.GetEmployeeDepartmentId(employeeId);
            var employeeDepartment = allDepartments.First(d => d.DepartmentId == employeeDepartmentId);
            var parentDepartment = allDepartments.FirstOrDefault(d => d.DepartmentId == employeeDepartment.ParentDepartmentId);
            var isEmployeeChief = employeeDepartment.ChiefId == employeeId;

            var neededApprovals = new List<string>();

            if (this.sdoDepartmentRegex.Match(employeeDepartment.Name).Success)
            {
                if (!isEmployeeChief)
                {
                    neededApprovals.Add(employeeDepartment.ChiefId);
                }

                neededApprovals.Add(parentDepartment?.ChiefId);
            }
            else if (employeeDepartment.IsHeadDepartment && employeeDepartment.ChiefId == employeeId)
            {
                // No approvals required for Director General
            }
            else
            {
                if (!isEmployeeChief)
                {
                    neededApprovals.Add(employeeDepartment.ChiefId);
                }
                else
                {
                    neededApprovals.Add(parentDepartment?.ChiefId);
                }
            }

            return neededApprovals.FirstOrDefault(i => !existingApprovals.Contains(i));
        }

        private async Task<string> GetEmployeeDepartmentId(string employeeId)
        {
            using (var context = this.contextFactory())
            {
                var employeeDepartmentId = await new CspEmployeeQuery(context)
                    .Get()
                    .Where(e => e.Id.ToString() == employeeId)
                    .Select(x => x.DepartmentId.HasValue ? x.DepartmentId.Value.ToString() : null)
                    .FirstAsync();
                return employeeDepartmentId;
            }
        }

        private async Task<List<DepartmentInfo>> GetDepartments()
        {
            var departmentsActor = Context.ActorSelection(DepartmentsStorageActorPath);

            var allDepartmentsResponse = await departmentsActor.Ask<DepartmentsStorage.LoadAllDepartments.Response>(
                DepartmentsStorage.LoadAllDepartments.Instance,
                this.settings.Timeout);
            return allDepartmentsResponse.Departments.ToList();
        }
    }
}