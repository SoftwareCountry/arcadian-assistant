namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class OrganizationActor : UntypedActor, ILogReceive
    {
        private readonly IActorRef departmentsStorage;

        private readonly IActorRef employees;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private (string departmentId, IActorRef actor) headDepartment;

        public OrganizationActor()
        {
            this.departmentsStorage = Context.ActorOf(DepartmentsStorage.Props, "departments-storage");
            this.employees = Context.ActorOf(EmployeesActor.Props());

            //TODO: make interval configurable
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(10),
                this.Self,
                RefreshOrganizationInformation.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshOrganizationInformation _:
                    this.departmentsStorage.Tell(DepartmentsStorage.LoadHeadDepartment.Instance);
                    this.employees.Tell(EmployeesActor.RefreshEmployees.Instance);
                    break;

                case DepartmentsStorage.LoadHeadDepartment request:
                    this.departmentsStorage.Tell(request);
                    break;

                case DepartmentsStorage.LoadHeadDepartment.Response response:
                    this.RecreateHeadDepartment(response.Department);
                    break;

                case DepartmentsQuery query:
                    var requesters = new[] { this.Sender };
                    //TODO: null reference exception possible
                    Context.ActorOf(Props.Create(() => new DepartmentsSearch(this.headDepartment.actor, requesters, query)));
                    break;

                case EmployeesQuery query:
                    this.employees.Forward(query);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void RecreateHeadDepartment(DepartmentInfo department)
        {
            if ((this.headDepartment.actor != null) && (this.headDepartment.departmentId != department.DepartmentId))
            {
                this.logger.Info($"Head department changed. Recreating the hierarchy...");

                this.headDepartment.actor.Tell(PoisonPill.Instance);
                this.headDepartment.actor = null;
            }

            IActorRef CreateDepartment() => Context.ActorOf(DepartmentActor.Props(department, this.departmentsStorage, this.employees), Uri.EscapeDataString(department.DepartmentId));

            if ((this.headDepartment.actor == null))
            {
                this.headDepartment = (department.DepartmentId, CreateDepartment());
            }

            this.headDepartment.actor.Tell(new DepartmentActor.RefreshDepartmentInfo(department));
        }

        private class RefreshOrganizationInformation
        {
            public static readonly RefreshOrganizationInformation Instance = new RefreshOrganizationInformation();
        }
    }
}