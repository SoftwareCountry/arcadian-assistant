namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions;

    public class OrganizationActor : UntypedActor
    {
        private readonly IActorRef departmentsStorage;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private (string departmentId, IActorRef actor) headDepartment;

        private Dictionary<string, IActorRef> DepartmentsById { get; } = new Dictionary<string, IActorRef>();

        public OrganizationActor()
        {
            this.departmentsStorage = Context.ActorOf(DepartmentsStorage.Props, "departments-storage");

            //TODO: make interval configurable
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(10),
                this.Self,
                DepartmentsStorage.LoadHeadDepartment.Instance,
                this.Self);

        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case DepartmentsStorage.LoadHeadDepartment request:
                    this.departmentsStorage.Tell(request);
                    break;

                case DepartmentsStorage.LoadHeadDepartment.Response response:
                    this.RecreateHeadDepartment(response.Department);
                    break;

                case OrganizationRequests.RequestDepartments _:
                    var requesters = new[] { this.Sender };
                    //TODO: null reference exception possible
                    Context.ActorOf(Props.Create(() => new DepartmentsSearch(this.headDepartment.actor, requesters, null)));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void RecreateHeadDepartment(Department department)
        {
            if ((this.headDepartment.actor != null) && (this.headDepartment.departmentId != department.DepartmentId))
            {
                this.logger.Info($"Head department changed. Recreating the hierarchy...");

                this.headDepartment.actor.Tell(PoisonPill.Instance);
                this.headDepartment.actor = null;
            }

            IActorRef CreateDepartment() => Context.ActorOf(DepartmentActor.Props(department, this.departmentsStorage), Uri.EscapeDataString(department.DepartmentId));

            if ((this.headDepartment.actor == null))
            {
                this.headDepartment = (department.DepartmentId, CreateDepartment());
            }

            this.headDepartment.actor.Tell(new DepartmentActor.RefreshDepartmentInfo(department));
        }
    }
}