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

    public class OrganizationActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly IActorRef departmentsStorage;

        private readonly IActorRef employees;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private (string departmentId, IActorRef actor) headDepartment;

        public IStash Stash { get; set; }

        public OrganizationActor()
        {
            this.departmentsStorage = Context.ActorOf(DepartmentsStorage.GetProps, "departments-storage");
            this.employees = Context.ActorOf(EmployeesActor.GetProps(), "employees");

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
                    this.Become(this.RefreshingDepartments);
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

        private UntypedReceive DefaultState()
        {
            this.Stash.UnstashAll();
            return this.OnReceive;
        }

        private void RefreshingEmployees(object message)
        {
            switch (message)
            {
                case EmployeesActor.RefreshEmployees.Finished _:
                    this.logger.Info("Employees info is refreshed");
                    this.Become(this.DefaultState());
                    break;

                case Status.Failure error:
                    this.logger.Error(error.Cause, "Error occurred while loading employees information");
                    this.Become(this.DefaultState());
                    break;

                case RefreshOrganizationInformation _:
                    break; //ignore, it's already in progress

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void RefreshingDepartments(object message)
        {
            switch (message)
            {
                case DepartmentsStorage.LoadHeadDepartment.Response response:
                    this.RecreateHeadDepartment(response.Department);
                    this.headDepartment.actor.Tell(new DepartmentActor.RefreshDepartmentInfo(response.Department));
                    break;

                case DepartmentActor.RefreshDepartmentInfo.Finished _:
                    this.logger.Info("Organization structure is loaded, loading employees");
                    this.employees.Tell(EmployeesActor.RefreshEmployees.Instance);
                    this.Become(this.RefreshingEmployees);
                    break;

                case Status.Failure error:
                    this.logger.Error(error.Cause, "Error occurred while loading departments information");
                    this.Become(this.DefaultState());
                    break;

                case RefreshOrganizationInformation _:
                    break; //ignore, it's already in progress

                default:
                    this.Stash.Stash();
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

            if ((this.headDepartment.actor == null))
            {
                var props = DepartmentActor.GetProps(department, this.departmentsStorage, this.employees);
                var departmentActor = Context.ActorOf(props, Uri.EscapeDataString(department.DepartmentId));
                this.headDepartment = (department.DepartmentId, departmentActor);
            }
        }

        private class RefreshOrganizationInformation
        {
            public static readonly RefreshOrganizationInformation Instance = new RefreshOrganizationInformation();
        }
    }
}