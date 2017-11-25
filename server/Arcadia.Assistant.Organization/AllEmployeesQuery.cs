namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;
    public class AllEmployeesQuery : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            //this.Become(this.WaitingForResults());
        }

        public UntypedReceive WaitingForResults()
        {
            return message =>
                       {
                           switch (message)
                           {
                               
                           }
                       };
        }
    }
}