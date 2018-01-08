namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class EmployeesInfoStorage : UntypedActor, ILogReceive
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case LoadAllEmployees request:
                    this.GetAllEmployees().PipeTo(this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<LoadAllEmployees.Response> GetAllEmployees();

        public static Props GetProps => Context.DI().Props<EmployeesInfoStorage>();

        public sealed class LoadAllEmployees
        {
            public static readonly LoadAllEmployees Instance = new LoadAllEmployees();

            private LoadAllEmployees()
            {
            }

            public sealed class Response
            {
                public IReadOnlyCollection<EmployeeStoredInformation> Employees { get; }

                public Response(IReadOnlyCollection<EmployeeStoredInformation> employees)
                {
                    this.Employees = employees;
                }
            }
        }
    }
}