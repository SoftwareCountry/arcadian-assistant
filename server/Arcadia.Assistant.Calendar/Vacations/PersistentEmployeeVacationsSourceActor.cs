namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using Akka.Actor;
    using Akka.Persistence;

    public class PersistentEmployeeVacationsSourceActor : UntypedPersistentActor, ILogReceive
    {
        private readonly string employeeId;

        public PersistentEmployeeVacationsSourceActor(string employeeId)
        {
            this.employeeId = employeeId;
            this.PersistenceId = $"employee-vacations-{employeeId}";
        }

        public override string PersistenceId { get; }

        protected override void OnCommand(object message)
        {
            throw new NotImplementedException();
        }

        protected override void OnRecover(object message)
        {
            throw new NotImplementedException();
        }
    }
}