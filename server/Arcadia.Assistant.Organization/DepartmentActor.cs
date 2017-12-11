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
        private Department departmentInfo;

        private readonly IActorRef departmentsStorage;

        private readonly IActorRef employees;

        private readonly IDictionary<string, IActorRef> departmentsById = new Dictionary<string, IActorRef>();

        private IActorRef headEmployee;

        public DepartmentActor(Department departmentInfo, IActorRef departmentsStorage)
        {
            this.departmentInfo = departmentInfo;
            this.departmentsStorage = departmentsStorage;
            this.employees = Context.ActorOf(EmployeesActor.Props(departmentInfo.DepartmentId));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshDepartmentInfo newInfo when newInfo.Department.DepartmentId == this.departmentInfo.DepartmentId:

                    if (this.departmentInfo.ChiefId != newInfo.Department.ChiefId)
                    {
                        this.headEmployee = null;
                        this.employees.Tell(new EmployeesActor.FindEmployee(newInfo.Department.ChiefId));
                    }

                    this.departmentInfo = newInfo.Department;
                    this.departmentsStorage.Tell(new DepartmentsStorage.LoadChildDepartments(this.departmentInfo.DepartmentId));
                    
                    this.employees.Tell(EmployeesActor.RefreshEmployees.Instance);
                    break;

                case EmployeesActor.FindEmployee.Response response:
                    this.headEmployee = response.Employee;
                    break;

                case DepartmentsStorage.LoadChildDepartments.Response response:
                    this.RefreshChildDepartments(response.Departments);
                    break;

                case GetDepartmentInfo _:
                    this.Sender.Tell(new GetDepartmentInfo.Result(this.departmentInfo, this.Self, this.departmentsById.Values.ToList(), this.employees));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void RefreshChildDepartments(IReadOnlyCollection<Department> responseDepartments)
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
                var department = Context.ActorOf(Props(addedDepartment, this.departmentsStorage), Uri.EscapeDataString(addedDepartment.DepartmentId));
                this.departmentsById[addedDepartment.DepartmentId] = department;
            }

            foreach (var department in responseDepartments)
            {
                this.departmentsById[department.DepartmentId].Tell(new RefreshDepartmentInfo(department));
            }
        }

        public static Props Props(Department department, IActorRef departmentsStorage) =>
            Akka.Actor.Props.Create(() => new DepartmentActor(department, departmentsStorage));

        public sealed class RefreshDepartmentInfo
        {
            public Department Department { get; }

            public RefreshDepartmentInfo(Department department)
            {
                this.Department = department;
            }
        }

        public sealed class GetDepartmentInfo
        {
            public static readonly GetDepartmentInfo Instance = new GetDepartmentInfo();

            public sealed class Result
            {
                public Department Department { get; }

                public IActorRef DepartmentActor { get; }

                public IReadOnlyCollection<IActorRef> Children { get; }

                public IActorRef Employees { get; }

                public Result(Department department, IActorRef departmentActor, IReadOnlyCollection<IActorRef> children, IActorRef employees)
                {
                    this.Department = department;
                    this.Children = children;
                    this.Employees = employees;
                    this.DepartmentActor = departmentActor;
                }
            }
        }
    }
}