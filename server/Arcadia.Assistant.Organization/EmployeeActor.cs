namespace Arcadia.Assistant.Organization
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar;
    using Arcadia.Assistant.Calendar.SickLeave;
    using Arcadia.Assistant.Calendar.Vacations;
    using Arcadia.Assistant.Calendar.WorkHours;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Images;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeActor : UntypedActor
    {
        private EmployeeMetadata employeeMetadata;

        private readonly IActorRef photo;

        private readonly IActorRef employeeFeed;

        private readonly EmployeeCalendarContainer calendar;

        public EmployeeActor(EmployeeStoredInformation storedInformation)
        {
            this.employeeMetadata = storedInformation.Metadata;

            this.photo = Context.ActorOf(Props.Create(() => new PhotoActor()), "photo");
            this.photo.Tell(new PhotoActor.SetSource(storedInformation.Photo));

            var employeeFeedId = $"employee-feed-{this.employeeMetadata.EmployeeId}";
            this.employeeFeed = Context.ActorOf(FeedActor.CreateProps(employeeFeedId), "feed");

            var vacationsActor = Context.ActorOf(EmployeeVacationsActor.CreateProps(this.employeeMetadata.EmployeeId), "vacations");
            var sickLeavesActor = Context.ActorOf(EmployeeSickLeaveActor.CreateProps(this.employeeMetadata.EmployeeId), "sick-leaves");
            var workHoursActor = Context.ActorOf(EmployeeWorkHoursActor.CreateProps(this.employeeMetadata.EmployeeId), "work-hours");

            var calendarActor = Context.ActorOf(EmployeeCalendarActor.CreateProps(vacationsActor, workHoursActor, sickLeavesActor));

            this.calendar = new EmployeeCalendarContainer(vacationsActor, workHoursActor, sickLeavesActor, calendarActor);
        }

        protected override void OnReceive(object message)
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
                    
                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void UpdateEmployeeMetadata(EmployeeMetadata informationMetadata)
        {
            if (informationMetadata.Position != this.employeeMetadata.Position)
            {
                var text = $"{informationMetadata.Name} is now {informationMetadata.Position}";
                this.employeeFeed.Tell(new FeedActor.PostMessage(new Message(Guid.NewGuid(), informationMetadata.EmployeeId, "Employee position has changed", text, DateTimeOffset.Now)));
            }

            if (informationMetadata.Name != this.employeeMetadata.Name)
            {
                var text = $"From now on, {this.employeeMetadata.Name} is to be known as {informationMetadata.Name}";
                this.employeeFeed.Tell(new FeedActor.PostMessage(new Message(Guid.NewGuid(), informationMetadata.EmployeeId, "Employee name has changed", text, DateTimeOffset.Now)));
            }

            //TODO: department id change handler

            this.employeeMetadata = informationMetadata;
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

        public static Props GetProps(EmployeeStoredInformation employeeStoredInformation) => Props.Create(() => new EmployeeActor(employeeStoredInformation));
    }
}