namespace Arcadia.Assistant.Organization
{
    using System;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar;
    using Arcadia.Assistant.Calendar.SickLeave;
    using Arcadia.Assistant.Calendar.Vacations;
    using Arcadia.Assistant.Calendar.WorkHours;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Images;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Organization.Events;

    public class EmployeeActor : UntypedPersistentActor, ILogReceive
    {
        private EmployeeMetadata employeeMetadata;

        private readonly IActorRef photo;

        private readonly IActorRef employeeFeed;

        private readonly EmployeeCalendarContainer calendar;

        public EmployeeActor(EmployeeStoredInformation storedInformation, IActorRef imageResizer, IActorRef vacationsRegistry)
        {
            this.employeeMetadata = storedInformation.Metadata;
            this.PersistenceId = $"employee-info-{Uri.EscapeDataString(this.employeeMetadata.EmployeeId)}";

            this.photo = Context.ActorOf(Props.Create(() => new PhotoActor(imageResizer)), "photo");
            this.photo.Tell(new PhotoActor.SetSource(storedInformation.Photo));

            this.employeeFeed = Context.ActorOf(FeedActor.GetProps(), "feed");

            var vacationsActor = Context.ActorOf(EmployeeVacationsActor.CreateProps(this.employeeMetadata.EmployeeId, this.employeeFeed, vacationsRegistry), "vacations");
            var sickLeavesActor = Context.ActorOf(EmployeeSickLeaveActor.CreateProps(this.employeeMetadata), "sick-leaves");
            var workHoursActor = Context.ActorOf(EmployeeWorkHoursActor.CreateProps(this.employeeMetadata.EmployeeId), "work-hours");
            Context.Watch(vacationsActor);
            Context.Watch(sickLeavesActor);
            Context.Watch(workHoursActor);

            var calendarActor = Context.ActorOf(EmployeeCalendarActor.CreateProps(this.employeeMetadata.EmployeeId, vacationsActor, workHoursActor, sickLeavesActor), "all-calendar-events");

            this.calendar = new EmployeeCalendarContainer(vacationsActor, workHoursActor, sickLeavesActor, calendarActor);
        }

        public override string PersistenceId { get; }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetEmployeeInfo _:
                    var container = new EmployeeContainer(this.employeeMetadata, this.Self, this.employeeFeed, this.calendar);
                    this.Sender.Tell(new GetEmployeeInfo.Response(container));
                    break;

                case GetPhoto _:
                    this.photo.Forward(message);
                    break;

                case UpdateEmployeeInformation newInfo when newInfo.Information.Metadata.EmployeeId == this.employeeMetadata.EmployeeId:
                    this.photo.Tell(new PhotoActor.SetSource(newInfo.Information.Photo));
                    this.UpdateEmployeeMetadata(newInfo.Information.Metadata);
                    break;

                //TODO: get rid of it somehow, now it monitors that actors were able to restore.
                case Terminated t:
                    Context.Stop(this.Self);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case EmployeeChangedDepartment ev:
                    break;

                case EmployeeChangedName ev:
                    this.OnEmployeeNameChange(ev);
                    break;

                case EmployeeChangedPosition ev:
                    this.OnEmployeePositionChange(ev);
                    break;
            }
        }

        private void UpdateEmployeeMetadata(EmployeeMetadata informationMetadata)
        {
            if (informationMetadata.Position != this.employeeMetadata.Position)
            {
                var ev = new EmployeeChangedPosition()
                {
                    EmployeeId = this.employeeMetadata.EmployeeId,
                    NewPosition = informationMetadata.Position,
                    OldPosition = this.employeeMetadata.Position,
                    TimeStamp = DateTimeOffset.UtcNow
                };
                this.Persist(ev, this.OnEmployeePositionChange);
                this.employeeMetadata.Position = ev.NewPosition;
            }

            if (informationMetadata.Name != this.employeeMetadata.Name)
            {
                var ev = new EmployeeChangedName()
                {
                    EmployeeId = this.employeeMetadata.EmployeeId,
                    NewName = informationMetadata.Name,
                    OldName = this.employeeMetadata.Name,
                    TimeStamp = DateTimeOffset.UtcNow
                };

                this.Persist(ev, this.OnEmployeeNameChange);
                this.employeeMetadata.Name = informationMetadata.Name;
            }

            if (informationMetadata.DepartmentId != this.employeeMetadata.DepartmentId)
            {
                //TODO: department id change handler
                this.employeeMetadata.DepartmentId = informationMetadata.DepartmentId;
            }

            this.employeeMetadata = informationMetadata;
        }

        private void OnEmployeePositionChange(EmployeeChangedPosition ev)
        {
            var messageId = $"employee-position-change-{ev.EmployeeId}-{ev.TimeStamp}";
            var text = $"{this.employeeMetadata.Name} is now {ev.NewPosition} (previously, {ev.OldPosition})";
            this.employeeFeed.Tell(new PostMessage(new Message(messageId, this.employeeMetadata.EmployeeId, "Employee position has changed", text, ev.TimeStamp.UtcDateTime)));
        }

        private void OnEmployeeNameChange(EmployeeChangedName ev)
        {
            var messageId = $"employee-name-change-{ev.EmployeeId}-{ev.TimeStamp}";
            var text = $"From now on, {ev.OldName} is to be known as {ev.NewName}";
            this.employeeFeed.Tell(new PostMessage(new Message(messageId, this.employeeMetadata.EmployeeId, "Employee name has changed", text, ev.TimeStamp.UtcDateTime)));
        }

        public class GetEmployeeInfo
        {
            public static readonly GetEmployeeInfo Instance = new GetEmployeeInfo();

            private GetEmployeeInfo() { }

            public class Response
            {
                public Response(EmployeeContainer employee)
                {
                    this.Employee = employee;
                }

                public EmployeeContainer Employee { get; }
            }
        }

        public sealed class UpdateEmployeeInformation
        {
            public EmployeeStoredInformation Information { get; }

            public UpdateEmployeeInformation(EmployeeStoredInformation information)
            {
                this.Information = information;
            }
        }

        public static Props GetProps(EmployeeStoredInformation employeeStoredInformation, IActorRef imageResizer, IActorRef vacationsRegistry) =>
            Props.Create(() => new EmployeeActor(employeeStoredInformation, imageResizer, vacationsRegistry));
    }
}