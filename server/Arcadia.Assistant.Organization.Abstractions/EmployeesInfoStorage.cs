namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class EmployeesInfoStorage : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case LoadAllEmployees _:
                    this.GetAllEmployees().PipeTo(this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<LoadAllEmployees.Response> GetAllEmployees();

        public static Props Props => Context.DI().Props<EmployeesInfoStorage>();

        public sealed class LoadAllEmployees
        {
            public static readonly LoadAllEmployees Instance = new LoadAllEmployees();

            private LoadAllEmployees()
            {
            }

            public sealed class Response
            {
                public IReadOnlyCollection<EmployeeInfo> Employees { get; }

                public Response(IReadOnlyCollection<EmployeeInfo> employees)
                {
                    this.Employees = employees;
                }
            }

            public sealed class Error
            {
                public Exception Exception { get; }

                public Error(Exception exception)
                {
                    this.Exception = exception;
                }
            }
        }
    }
}