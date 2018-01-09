namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private DepartmentInfo departmentInfo;

        private readonly IActorRef departmentsStorage;

        private readonly IActorRef employees;

        private readonly IDictionary<string, IActorRef> departmentsById = new Dictionary<string, IActorRef>();

        private readonly ILoggingAdapter logger = Context.GetLogger();

//        private EmployeeContainer headEmployee;

        public IStash Stash { get; set; }

        public DepartmentActor(DepartmentInfo departmentInfo, IActorRef departmentsStorage, IActorRef employees)
        {
            this.departmentInfo = departmentInfo;
            this.departmentsStorage = departmentsStorage;
            this.employees = employees;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshDepartmentInfo newInfo when newInfo.Department.DepartmentId == this.departmentInfo.DepartmentId:

                    if (this.departmentInfo.ChiefId != newInfo.Department.ChiefId)
                    {
                        //TODO record head change
                        //this.headEmployee = null;
                        //this.employees.Tell(new EmployeesActor.FindEmployee(newInfo.Department.ChiefId));
                    }

                    this.departmentInfo = newInfo.Department;
                    this.departmentsStorage.Tell(new DepartmentsStorage.LoadChildDepartments(this.departmentInfo.DepartmentId));
                    this.BecomeStacked(this.Refreshing);

                    //this.Self.Forward(ReportFinish.Instance);

                    break;

                case GetDepartmentInfo _:
                    this.Sender.Tell(new GetDepartmentInfo.Result(this.departmentInfo, this.Self, this.departmentsById.Values.ToList()));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void Refreshing(object message)
        {
            switch (message)
            {
                case DepartmentsStorage.LoadChildDepartments.Response children:
                    this.RefreshChildDepartments(children.Departments);
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();
                    break;

                case Status.Failure error:
                    this.logger.Error(error.Cause, $"Error occurred while refreshing child departments");
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();
                    break;
                
                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void RefreshChildDepartments(IReadOnlyCollection<DepartmentInfo> responseDepartments)
        {
            var allDepartmentIds = new HashSet<string>(responseDepartments.Select(x => x.DepartmentId));

            var removedIds = this.departmentsById.Keys.Except(allDepartmentIds).ToList();
            var addedDepartments = responseDepartments.Where(x => !this.departmentsById.ContainsKey(x.DepartmentId)).ToImmutableList();

            foreach (var removedId in removedIds)
            {
                this.departmentsById[removedId].Tell(PoisonPill.Instance);
                this.departmentsById.Remove(removedId);
            }

            foreach (var department in responseDepartments)
            {
                if (!this.departmentsById.TryGetValue(department.DepartmentId, out var departmentAgent))
                {
                    departmentAgent = Context.ActorOf(GetProps(department, this.departmentsStorage, this.employees), Uri.EscapeDataString(department.DepartmentId));
                    this.departmentsById[department.DepartmentId] = departmentAgent;
                }

                departmentAgent.Tell(new RefreshDepartmentInfo(department));
            }

            this.logger.Info($"Child departments are loaded for <{this.departmentInfo.DepartmentId}> : {this.departmentInfo.Name}." 
                + $"There are {this.departmentsById.Count} child departments. Removed {removedIds.Count}, added {addedDepartments.Count}");
        }

        public static Props GetProps(DepartmentInfo department, IActorRef departmentsStorage, IActorRef employees) =>
            Props.Create(() => new DepartmentActor(department, departmentsStorage, employees));

        public sealed class RefreshDepartmentInfo
        {
            public DepartmentInfo Department { get; }

            public RefreshDepartmentInfo(DepartmentInfo department)
            {
                this.Department = department;
            }
        }

        public sealed class GetDepartmentInfo
        {
            public static readonly GetDepartmentInfo Instance = new GetDepartmentInfo();

            public sealed class Result
            {
                public DepartmentInfo Department { get; }

                public IActorRef DepartmentActor { get; }

                public IReadOnlyCollection<IActorRef> Children { get; }

                public Result(DepartmentInfo department, IActorRef departmentActor, IReadOnlyCollection<IActorRef> children)
                {
                    this.Department = department;
                    this.Children = children;
                    this.DepartmentActor = departmentActor;
                }
            }
        }
    }
}