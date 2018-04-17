namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using Akka.Actor;

    public sealed class GetDepartmentInfo
    {
        public static readonly GetDepartmentInfo Instance = new GetDepartmentInfo();

        public sealed class Result
        {
            public DepartmentInfo Department { get; }

            public IActorRef DepartmentActor { get; }

            public Result(DepartmentInfo department, IActorRef departmentActor)
            {
                this.Department = department;
                this.DepartmentActor = departmentActor;
            }
        }
    }
}