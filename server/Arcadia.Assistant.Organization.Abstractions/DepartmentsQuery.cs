namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class DepartmentsQuery : UntypedActor
    {
        public class RequestAllDepartments
        {
            public static readonly RequestAllDepartments Instance = new RequestAllDepartments();

            public class Response
            {
                public Response(Department[] departments)
                {
                    this.Departments = departments;
                }

                public Department[] Departments { get; }
            }
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestAllDepartments _:
                    this.GetAllDepartments().PipeTo(this.Sender);
                    break;
                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<RequestAllDepartments.Response> GetAllDepartments();

        public static Props Props => Context.DI().Props<DepartmentsQuery>();
    }
}