namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Diagnostics;

    using Akka.Actor;

    [DebuggerDisplay("{Metadata}")]
    public class EmployeeContainer
    {
        public EmployeeMetadata Metadata { get; }

        public IActorRef Actor { get; }

        public IActorRef Feed { get; }

        public EmployeeCalendarContainer Calendar { get; }

        public EmployeeContainer(EmployeeMetadata metadata, IActorRef actor, IActorRef feed, EmployeeCalendarContainer calendar)
        {
            this.Metadata = metadata;
            this.Actor = actor;
            this.Feed = feed;
            this.Calendar = calendar;
        }
    }
}