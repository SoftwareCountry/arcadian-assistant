namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;
    using Akka.DI.Core;

    public class EmployeeActor : UntypedActor
    {
        private readonly IActorRef demographicsLoader;

        public EmployeeActor(string employeeId)
        {
            this.EmployeeId = employeeId;
            this.demographicsLoader = Context.ActorOf(Props.Create(() => new DemographicsLoaderActor(employeeId)), "demographics-loader");
        }

        private string EmployeeId { get; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestDemographics request when this.EmployeeId == request.EmployeeId:
                    this.demographicsLoader.Forward(request);
                    break;
            }
        }
    }
}