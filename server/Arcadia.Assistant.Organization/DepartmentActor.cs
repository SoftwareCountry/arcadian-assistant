namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentActor : UntypedActor
    {
        private DepartmentInfo departmentInfo;

        private readonly IActorRef departmentsStorage;

        private readonly IActorRef employees;

        private readonly IDictionary<string, IActorRef> departmentsById = new Dictionary<string, IActorRef>();

        private IActorRef headEmployee;

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
                        this.headEmployee = null;
                        //this.employees.Tell(new EmployeesActor.FindEmployee(newInfo.Department.ChiefId));
                    }

                    this.departmentInfo = newInfo.Department;
                    this.departmentsStorage.Tell(new DepartmentsStorage.LoadChildDepartments(this.departmentInfo.DepartmentId));
                    
                    break;

                case DepartmentsStorage.LoadChildDepartments.Response response:
                    this.RefreshChildDepartments(response.Departments);
                    break;

                case GetDepartmentInfo _:
                    this.Sender.Tell(new GetDepartmentInfo.Result(this.departmentInfo, this.Self, this.departmentsById.Values.ToList()));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void RefreshChildDepartments(IReadOnlyCollection<DepartmentInfo> responseDepartments)
        {
            var allDepartmentIds = responseDepartments.Select(x => x.DepartmentId).ToImmutableHashSet();

            var removedIds = this.departmentsById.Keys.Except(allDepartmentIds).ToImmutableList();
            var addedDepartments = responseDepartments.Where(x => !this.departmentsById.ContainsKey(x.DepartmentId)).ToImmutableList();

            foreach (var removedId in removedIds)
            {
                this.departmentsById[removedId].Tell(PoisonPill.Instance);
                this.departmentsById.Remove(removedId);
            }

            foreach (var addedDepartment in addedDepartments)
            {
                var department = Context.ActorOf(Props(addedDepartment, this.departmentsStorage, this.employees), Uri.EscapeDataString(addedDepartment.DepartmentId));
                this.departmentsById[addedDepartment.DepartmentId] = department;
            }

            foreach (var department in responseDepartments)
            {
                this.departmentsById[department.DepartmentId].Tell(new RefreshDepartmentInfo(department));
            }
        }

        public static Props Props(DepartmentInfo department, IActorRef departmentsStorage, IActorRef employees) =>
            Akka.Actor.Props.Create(() => new DepartmentActor(department, departmentsStorage, employees));

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