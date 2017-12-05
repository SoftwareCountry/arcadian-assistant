namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentActor : UntypedActor
    {
        private readonly Department department;

        public DepartmentActor(Department department)
        {
            this.department = department;
        }

        protected override void OnReceive(object message)
        {
            
        }
    }
}