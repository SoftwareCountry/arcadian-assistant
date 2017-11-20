namespace Arcadia.Assistant.Server
{
    using Akka.Actor;

    using Arcadia.Assistant.Helpdesk;
    using Arcadia.Assistant.Organization;

    public class ActorSystemBuilder
    {
        private readonly IActorRefFactory actorSystem;

        public ActorSystemBuilder(IActorRefFactory actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public ServerActorsCollection AddRootActors()
        {
            var employees =this.actorSystem.ActorOf(Props.Create(() => new EmployeesActor()), "employees");
            var helpdesk = this.actorSystem.ActorOf(Props.Create(() => new HelpdeskActor()), "helpdesk");

            return new ServerActorsCollection(employees, helpdesk);
        }
    }
}