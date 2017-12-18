namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeSearch : UntypedActor, IWithUnboundedStash
    {
        private IActorRef department;

        //public 

        protected override void OnReceive(object message)
        {
            
        }

        public IStash Stash { get; set; }
    }
}