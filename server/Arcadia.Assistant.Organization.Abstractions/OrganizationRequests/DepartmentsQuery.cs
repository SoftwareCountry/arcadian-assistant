namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;

    public sealed class DepartmentsQuery : ICloneable
    {
        public bool ShouldLoadAllDepartments => this.DepartmentId == null;

        public bool ShouldLoadEmployees { get; private set; }

        public string DepartmentId { get; private set; }

        public DepartmentsQuery WithId(string departmentId)
        {
            var newObject = this.CloneTypewise();
            newObject.DepartmentId = departmentId;
            return newObject;
        }

        public DepartmentsQuery WithEmployees()
        {
            var newObject = this.CloneTypewise();
            newObject.ShouldLoadEmployees = true;

            return newObject;
        }

        private DepartmentsQuery CloneTypewise()
        {
            var copy = new DepartmentsQuery();
            copy.DepartmentId = this.DepartmentId;
            copy.ShouldLoadEmployees = this.ShouldLoadEmployees;
            return copy;
        }

        public object Clone()
        {
            return this.CloneTypewise();
        }

        public sealed class Response
        {
            public IReadOnlyCollection<DepartmentFinding> Departments { get; }

            public Response(IReadOnlyCollection<DepartmentFinding> departments)
            {
                this.Departments = departments;
            }
        }

        public class DepartmentFinding
        {
            public DepartmentInfo Department { get; }

            public IActorRef DepartmentActor { get; }

            public DepartmentFinding(DepartmentInfo department, IActorRef departmentActor)
            {
                this.Department = department;
                this.DepartmentActor = departmentActor;
            }
        }
    }
}