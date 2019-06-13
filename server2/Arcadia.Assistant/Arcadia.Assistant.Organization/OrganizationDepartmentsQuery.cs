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

    using CSP;

    using Microsoft.EntityFrameworkCore;

    public class OrganizationDepartmentsQuery
    {
        private readonly CspConfiguration cspConfiguration;
        private readonly CspDepartmentsQuery cspDepartmentsQuery;

        private readonly Expression<Func<CspDepartmentsQuery.DepartmentWithPeopleCount, DepartmentMetadata>> mapToDepartmentMetadata = x => new DepartmentMetadata(
            x.Department.Id.ToString(),
            x.Department.Name,
            x.Department.Abbreviation,
            x.Department.ParentDepartmentId == null || x.Department.ParentDepartmentId == x.Department.Id ? null : x.Department.ParentDepartmentId.ToString())
        {
            ChiefId = x.ActualChiefId == null ? null : x.ActualChiefId.ToString(),
            PeopleCount = x.PeopleCount
        };

        public OrganizationDepartmentsQuery(CspDepartmentsQuery cspDepartmentsQuery, CspConfiguration cspConfiguration)
        {
            this.cspDepartmentsQuery = cspDepartmentsQuery;
            this.cspConfiguration = cspConfiguration;
        }

        public async Task<IReadOnlyList<DepartmentMetadata>> LoadAllAsync(CancellationToken cancellationToken)
        {
            var departmentsQuery = this.cspDepartmentsQuery.Get();

            var allDepartments = await departmentsQuery
                .Select(this.mapToDepartmentMetadata)
                .ToListAsync(cancellationToken);

            var head = allDepartments.FirstOrDefault(x => x.IsHeadDepartment && x.Abbreviation == this.cspConfiguration.HeadDepartmentAbbreviation);

            if (head == null)
            {
                return new DepartmentMetadata[0];
            }

            var processedIds = new HashSet<string>
                { head.DepartmentId };
            var tree = new DepartmentsTreeNode(head, head.PeopleCount, this.CreateTree(allDepartments, head.DepartmentId, processedIds));

            var departments = tree.AsEnumerable().Where(x => x.CountAllEmployees() != 0).Select(x => x.DepartmentInfo).ToList();

            return departments;
        }

        /// <param name="departmentId">Node for which descandants are requested</param>
        /// <param name="processedIds">a hashset with processed department ids to prevent stackoverflow</param>
        /// <param name="allDepartments">all possible departments</param>
        private List<DepartmentsTreeNode> CreateTree(
            IReadOnlyCollection<DepartmentMetadata> allDepartments,
            string departmentId,
            HashSet<string> processedIds)
        {
            var childrenDepartments = allDepartments
                .Where(x => x.ParentDepartmentId == departmentId && !processedIds.Contains(x.DepartmentId))
                .ToList();

            processedIds.UnionWith(childrenDepartments.Select(x => x.DepartmentId));

            var children = new List<DepartmentsTreeNode>();
            foreach (var department in childrenDepartments)
            {
                var child = new DepartmentsTreeNode(department, department.PeopleCount, this.CreateTree(allDepartments, department.DepartmentId, processedIds));
                children.Add(child);
            }

            return children;
        }
    }
}