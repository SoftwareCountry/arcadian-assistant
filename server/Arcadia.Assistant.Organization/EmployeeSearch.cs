namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeSearch : UntypedActor
    {
        private IActorRef department;

        //public 

        protected override void OnReceive(object message)
        {
            
        }

        public class EmployeeFinding
        {
            public EmployeeInfo EmployeeInfo { get; }

            public IActorRef Actor { get; }

            public EmployeeFinding(EmployeeInfo employeeInfo, IActorRef actor)
            {
                this.EmployeeInfo = employeeInfo;
                this.Actor = actor;
            }
        }
    }
}