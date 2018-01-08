namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class DepartmentsStorage : UntypedActor, ILogReceive
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case LoadHeadDepartment _:
                    this.GetHeadDepartment().PipeTo(this.Sender);
                    break;

                case LoadChildDepartments request:
                    this.GetChildDepartments(request.DepartmentId).PipeTo(this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<LoadHeadDepartment.Response> GetHeadDepartment();

        protected abstract Task<LoadChildDepartments.Response> GetChildDepartments(string departmentId);

        public static Props GetProps => Context.DI().Props<DepartmentsStorage>();

        public class LoadHeadDepartment
        {
            public static readonly LoadHeadDepartment Instance = new LoadHeadDepartment();

            public class Response
            {
                public DepartmentInfo Department { get; }

                public Response(DepartmentInfo department)
                {
                    this.Department = department;
                }
            }
        }

        public class LoadChildDepartments
        {
            public string DepartmentId { get; }

            public LoadChildDepartments(string departmentId)
            {
                this.DepartmentId = departmentId;
            }

            public class Response
            {
                public IReadOnlyCollection<DepartmentInfo> Departments { get; }

                public Response(IReadOnlyCollection<DepartmentInfo> departments)
                {
                    this.Departments = departments;
                }
            }
        }
    }
}