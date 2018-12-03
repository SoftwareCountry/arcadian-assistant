namespace Arcadia.Assistant.Organization
{
    using System;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class OrganizationActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IActorRef employeesActor;
        private readonly IActorRef departmentsActor;

        public IStash Stash { get; set; }

        public OrganizationActor(IRefreshInformation refreshInformation)
        {
            var vacationApprovalsCheckerActor = Context.ActorOf(VacationApprovalsChecker.GetProps(), "vacation-approvals-checker");

            this.employeesActor = Context.ActorOf(EmployeesActor.GetProps(vacationApprovalsCheckerActor), "employees");
            this.departmentsActor = Context.ActorOf(DepartmentsActor.GetProps(this.employeesActor), "departments");

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes),
                this.Self,
                RefreshOrganizationInformation.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshOrganizationInformation _:
                    this.employeesActor.Tell(EmployeesActor.RefreshEmployees.Instance);
                    this.Become(this.RefreshingEmployees);

                    break;

                case DepartmentsQuery query:
                    this.departmentsActor.Forward(query);
                    break;

                case EmployeesQuery query:
                    this.employeesActor.Forward(query);
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
                    //this.departmentsStorage.Tell(DepartmentsStorage.LoadAllDepartments.Instance);

                    this.departmentsActor.Tell(DepartmentsActor.RefreshDepartments.Instance);

                    //this.departmentsStorage.Tell(DepartmentsStorage.LoadHeadDepartment.Instance);
                    this.Become(this.RefreshingDepartments);

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
                case DepartmentsActor.RefreshDepartments.Finished _:
                    this.logger.Info("Organization structure is loaded");
                    this.Become(this.DefaultState());

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

        private class RefreshOrganizationInformation
        {
            public static readonly RefreshOrganizationInformation Instance = new RefreshOrganizationInformation();
        }
    }
}