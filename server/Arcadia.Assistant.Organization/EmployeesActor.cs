namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeesActor : UntypedActor
    {
        private readonly IActorRef allEmployeesQuery;

        private Dictionary<string, IActorRef> EmployeesById { get; } = new Dictionary<string, IActorRef>();

        public EmployeesActor()
        {
            this.allEmployeesQuery = Context.ActorOf(AllEmployeesQuery.Props);

            //Start loading
            this.Self.Tell(AllEmployeesQuery.RequestAllEmployeeIds.Instance);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AllEmployeesQuery.RequestAllEmployeeIds _:
                    {
                        this.allEmployeesQuery.Tell(AllEmployeesQuery.RequestAllEmployeeIds.Instance);
                        break;
                    }
                case AllEmployeesQuery.RequestAllEmployeeIds.Response allEmployees:
                    {
                        this.RecreateEmployeeAgents(allEmployees.Ids);
                        break;
                    }

                case OrganizationRequests.RequestEmployeeInfo request:
                    if (!this.EmployeesById.TryGetValue(request.EmployeeId, out var employee))
                    {
                        this.Sender.Tell(new OrganizationRequests.RequestEmployeeInfo.EmployeeNotFound(request.EmployeeId));
                    }
                    else
                    {
                        employee.Forward(request);
                    }
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void RecreateEmployeeAgents(string[] allEmployees)
        {
            var removedIds = this.EmployeesById.Keys.Except(allEmployees).ToImmutableList();
            var addedIds = allEmployees.Except(this.EmployeesById.Keys).ToImmutableList();

            foreach (var removedId in removedIds)
            {
                this.EmployeesById[removedId].Tell(PoisonPill.Instance);
                this.EmployeesById.Remove(removedId);
            }

            foreach (var addedId in addedIds)
            {
                var employee = Context.ActorOf(Props.Create(() => new EmployeeActor(addedId)), Uri.EscapeDataString(addedId));
                this.EmployeesById[addedId] = employee;
            }
        }
    }
}