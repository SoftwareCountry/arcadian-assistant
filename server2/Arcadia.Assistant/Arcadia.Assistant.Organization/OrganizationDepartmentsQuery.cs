namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using CSP.Contracts;

    using Employees.Contracts;

    public class OrganizationDepartmentsQuery
    {
        private readonly CspConfiguration cspConfiguration;
        private readonly CspDepartmentsQuery cspDepartmentsQuery;

        private readonly Expression<Func<CspDepartmentsQuery.DepartmentWithPeopleCount, DepartmentMetadata>> mapToDepartmentMetadata = x => new DepartmentMetadata(
            new DepartmentId(x.Department.Id),
            x.Department.Name,
            x.Department.Abbreviation,
            x.Department.ParentDepartmentId == null || x.Department.ParentDepartmentId == x.Department.Id 
                ? (DepartmentId?)null 
                : new DepartmentId(x.Department.ParentDepartmentId.Value))
        {
            ChiefId = x.ActualChiefId == null ? (EmployeeId?)(null) : new EmployeeId(x.ActualChiefId.Value),
            PeopleCount = x.PeopleCount
        };

        public OrganizationDepartmentsQuery(CspDepartmentsQuery cspDepartmentsQuery, CspConfiguration cspConfiguration)
        {
            this.cspDepartmentsQuery = cspDepartmentsQuery;
            this.cspConfiguration = cspConfiguration;
        }

        public IReadOnlyList<DepartmentMetadata> LoadAll()
        {
            var departmentsQuery = this.cspDepartmentsQuery.Get();

            var allDepartments = departmentsQuery
                .Select(this.mapToDepartmentMetadata)
                .ToList();

            var head = allDepartments.FirstOrDefault(x => x.IsHeadDepartment && x.Abbreviation == this.cspConfiguration.HeadDepartmentAbbreviation);

            if (head == null)
            {
                return new DepartmentMetadata[0];
            }

            var treeBuilder = new DepartmentsTreeBuilder(allDepartments);
            var tree = treeBuilder.Build(head.DepartmentId);
            if (tree == null)
            {
                return new DepartmentMetadata[0];
            }

            var departments = tree.AsEnumerable().Where(x => x.CountAllEmployees() != 0).Select(x => x.DepartmentInfo).ToList();
            return departments;
        }
    }
}