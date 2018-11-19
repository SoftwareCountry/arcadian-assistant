namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System;
    using System.Collections.Generic;

    public sealed class DepartmentsQuery : ICloneable
    {
        public string DepartmentId { get; private set; }

        public string AscendantDepartmentId { get; private set; }

        public string DepartmentHeadEmployeeId { get; private set; }


        public bool IncludeDirectDescendantDepartments { get; private set; }

        public static DepartmentsQuery Create()
        {
            return new DepartmentsQuery();
        }

        public DepartmentsQuery WithId(string departmentId)
        {
            var newObject = this.CloneTypewise();
            newObject.DepartmentId = departmentId;
            return newObject;
        }

        public DepartmentsQuery DescendantOf(string departmentId)
        {
            var newObject = this.CloneTypewise();
            newObject.AscendantDepartmentId = departmentId;
            return newObject;
        }

        public DepartmentsQuery WithHead(string employeeId)
        {
            var newObject = this.CloneTypewise();
            newObject.DepartmentHeadEmployeeId = employeeId;
            return newObject;
        }

        public DepartmentsQuery IncludeDirectDescendants()
        {
            var newObject = this.CloneTypewise();
            newObject.IncludeDirectDescendantDepartments = true;
            return newObject;
        }

        public object Clone()
        {
            return this.CloneTypewise();
        }

        private DepartmentsQuery CloneTypewise()
        {
            var copy = new DepartmentsQuery
            {
                DepartmentId = this.DepartmentId,
                AscendantDepartmentId = this.AscendantDepartmentId,
                DepartmentHeadEmployeeId = this.DepartmentHeadEmployeeId,
                IncludeDirectDescendantDepartments = this.IncludeDirectDescendantDepartments 
            };
            return copy;
        }

        public sealed class Response
        {
            public IReadOnlyCollection<DepartmentContainer> Departments { get; }

            public Response(IReadOnlyCollection<DepartmentContainer> departments)
            {
                this.Departments = departments;
            }
        }
    }
}
