namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    public class EmployeesActor : UntypedActor
    {
        private Dictionary<string, IActorRef> EmployeesById { get; } = new Dictionary<string, IActorRef>();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
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