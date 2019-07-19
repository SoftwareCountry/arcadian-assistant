namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class DepartmentsActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly IActorRef departmentsStorage;

        private readonly Dictionary<string, IActorRef> departmentActorsById = new Dictionary<string, IActorRef>();

        private readonly IActorRef employees;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public DepartmentsActor(IActorRef employees)
        {
            this.employees = employees;
            this.departmentsStorage = Context.ActorOf(DepartmentsStorage.GetProps, "departments-storage"); ;
        }

        public static Props GetProps(IActorRef employees) => Props.Create(() => new DepartmentsActor(employees));

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshDepartments _:
                    this.departmentsStorage.Tell(DepartmentsStorage.LoadAllDepartments.Instance);
                    this.BecomeStacked(this.DepartmentsLoadRequested(this.Sender));
                    break;

                case DepartmentsQuery query:
                    Context.ActorOf(Props.Create(() => new DepartmentsSearch(query, this.departmentActorsById, this.Sender)));
                    break;

                case GetDepartmentFeatures msg:
                    if (this.departmentActorsById.TryGetValue(msg.DepartmentId, out var department))
                    {
                        department.Forward(msg);
                    }
                    else
                    {
                        this.Sender.Tell(GetDepartmentFeatures.NotFound.Instance);
                    }

                    break;

                case DepartmentActor.RefreshDepartmentInfo.Finished _:
                    //ignore for now
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private UntypedReceive DepartmentsLoadRequested(IActorRef initiator)
        {
            var actorsToRespondAboutRefreshing = new List<IActorRef> { initiator };
            return message => this.RefreshingDepartments(message, actorsToRespondAboutRefreshing);
        }

        private void RefreshingDepartments(object message, List<IActorRef> actorsToRespondAboutRefreshing)
        {
            void OnRefreshFinish(object onFinishMessage)
            {
                actorsToRespondAboutRefreshing.ForEach(x => x.Tell(onFinishMessage));
                this.Stash.UnstashAll();
                this.UnbecomeStacked();
            }

            switch (message)
            {
                case RefreshDepartments _:
                    actorsToRespondAboutRefreshing.Add(this.Sender);
                    break;

                case DepartmentsStorage.LoadAllDepartments.Response departments:
                    //TODO: assign departments actors
                    this.RecreateDepartments(departments.Departments);
                    OnRefreshFinish(RefreshDepartments.Finished.Instance);
                    break;

                case Status.Failure e:
                    OnRefreshFinish(e);
                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void RecreateDepartments(IReadOnlyCollection<DepartmentInfo> departments)
        {
            var allDepartmentIds = departments.Select(x => x.DepartmentId);
            var departmentsToDelete = this.departmentActorsById.Keys.Except(allDepartmentIds).ToList();

            foreach (var departmentToDeleteId in departmentsToDelete)
            {
                var department = this.departmentActorsById[departmentToDeleteId];
                department.Tell(PoisonPill.Instance);
                this.departmentActorsById.Remove(departmentToDeleteId);
            }

            var newDepartments = departments.Where(x => !this.departmentActorsById.ContainsKey(x.DepartmentId));

            foreach (var newDepartment in newDepartments)
            {
                var props = DepartmentActor.GetProps(newDepartment, this.employees);
                var departmentActor = Context.ActorOf(props, Uri.EscapeDataString(newDepartment.DepartmentId));
                this.departmentActorsById[newDepartment.DepartmentId] = departmentActor;
            }

            foreach (var department in departments)
            {
                if (!this.departmentActorsById.TryGetValue(department.DepartmentId, out var departmentActor))
                {
                    var props = DepartmentActor.GetProps(department, this.employees);
                    departmentActor = Context.ActorOf(props, Uri.EscapeDataString(department.DepartmentId));
                    this.departmentActorsById[department.DepartmentId] = departmentActor;
                }

                departmentActor.Tell(new DepartmentActor.RefreshDepartmentInfo(department));
            }
        }

        public class RefreshDepartments
        {
            public static readonly RefreshDepartments Instance = new RefreshDepartments();

            public class Finished
            {
                public static readonly Finished Instance = new Finished();
            }
        }

        public IStash Stash { get; set; }
    }
}