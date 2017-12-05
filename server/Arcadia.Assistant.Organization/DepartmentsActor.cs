namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentsActor : UntypedActor
    {
        private readonly IActorRef departmentsQuery;

        private Dictionary<string, IActorRef> DepartmentsById { get; } = new Dictionary<string, IActorRef>();

        public DepartmentsActor()
        {
            this.departmentsQuery = Context.ActorOf(DepartmentsQuery.Props, "departments-loader");

            //TODO: make interval configurable
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(10),
                this.departmentsQuery,
                DepartmentsQuery.RequestAllDepartments.Instance,
                this.Self);

        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case DepartmentsQuery.RequestAllDepartments request:
                    this.departmentsQuery.Tell(request);
                    break;
                case DepartmentsQuery.RequestAllDepartments.Response response:
                    this.RecreateAgents(response.Departments);
                    break;
            }
        }

        private void RecreateAgents(Department[] responseDepartments)
        {
            var allDepartmentIds = responseDepartments.Select(x => x.DepartmentId).ToImmutableHashSet();

            var removedIds = this.DepartmentsById.Keys.Except(allDepartmentIds).ToImmutableList();
            var addedDepartments = responseDepartments.Where(x => !this.DepartmentsById.ContainsKey(x.DepartmentId)).ToImmutableList();

            foreach (var removedId in removedIds)
            {
                this.DepartmentsById[removedId].Tell(PoisonPill.Instance);
                this.DepartmentsById.Remove(removedId);
            }

            foreach (var addedDepartment in addedDepartments)
            {
                var department = Context.ActorOf(Props.Create(() => new DepartmentActor(addedDepartment)), Uri.EscapeDataString(addedDepartment.DepartmentId));
                this.DepartmentsById[addedDepartment.DepartmentId] = department;
            }
        }
    }
}