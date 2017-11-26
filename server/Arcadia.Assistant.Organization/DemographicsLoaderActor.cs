namespace Arcadia.Assistant.Organization
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DemographicsLoaderActor : UntypedActor
    {
        private readonly string employeeId;

        public DemographicsLoaderActor(string employeeId)
        {
            this.employeeId = employeeId;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case OrganizationRequests.RequestEmployeeInfo request when request.EmployeeId == this.employeeId:

                    this.Sender.Tell(
                        new EmployeeInfo(this.employeeId)
                        {
                            BirthDate = new DateTime(1980, 6, 1),
                            HireDate = new DateTime(2012, 10, 1),
                            Email = "test@example.com",
                            Name = "Test User 2",
                            PhotoBase64 = null,
                            Sex = Sex.Male
                        });
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}