﻿namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using CSP.WebApi.Contracts;
    using CSP.WebApi.Contracts.Models;

    using Employees.Contracts;

    public class OrganizationDepartmentsQuery
    {
        private readonly CspConfiguration cspConfiguration;
        //private readonly CspDepartmentsQuery cspDepartmentsQuery;
        private readonly ICspApi csp;

        private DepartmentMetadata MapToDepartmentMetadata(DepartmentWithPeopleCount x)
        {
            return new DepartmentMetadata(
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
        }

        public OrganizationDepartmentsQuery(ICspApi csp, CspConfiguration cspConfiguration)
        {
            this.csp = csp;
            this.cspConfiguration = cspConfiguration;
        }

        public async Task<IReadOnlyList<DepartmentMetadata>> LoadAll()
        {
            var departmentsQuery = await this.csp.GetDepartmentWithPeople(CancellationToken.None);

            var allDepartments = departmentsQuery
                .Select(this.MapToDepartmentMetadata)
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