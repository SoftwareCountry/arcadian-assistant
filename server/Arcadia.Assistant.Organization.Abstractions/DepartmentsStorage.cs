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
                case LoadAllDepartments _:
                    this.GetAllDepartments().PipeTo(this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<LoadAllDepartments.Response> GetAllDepartments();

        public static Props GetProps => Context.DI().Props<DepartmentsStorage>();

        public class LoadAllDepartments
        {
            public static LoadAllDepartments Instance = new LoadAllDepartments();

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