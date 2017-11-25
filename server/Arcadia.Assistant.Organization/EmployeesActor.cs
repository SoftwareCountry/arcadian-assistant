namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeesActor : UntypedActor
    {
        private readonly IAllEmployeesQuery allEmployeesQuery;

        private Dictionary<string, IActorRef> EmployeesById { get; } = new Dictionary<string, IActorRef>();

        public EmployeesActor(IAllEmployeesQuery allEmployeesQuery)
        {
            this.allEmployeesQuery = allEmployeesQuery;
            this.Self.Tell("LOAD_ALL");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "LOAD_ALL":
                    {
                        this.allEmployeesQuery.GetAllEmployeeIds().PipeTo(this.Self);
                        break;
                    }
                case string[] employeeIds:
                    {
                        break;
                    }

                case RequestDemographics request:

                    if (!this.EmployeesById.TryGetValue(request.EmployeeId, out var employee))
                    {
                        employee = Context.ActorOf(Props.Create(() => new EmployeeActor(request.EmployeeId)));
                        this.EmployeesById.Add(request.EmployeeId, employee);
                    }

                    employee.Forward(request);
                    break;
            }
        }
    }
}