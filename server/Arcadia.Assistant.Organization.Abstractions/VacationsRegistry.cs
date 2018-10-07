namespace Arcadia.Assistant.Organization.Abstractions
{
    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class VacationsRegistry : UntypedActor
    {
        public static Props GetProps => Context.DI().Props<VacationsRegistry>();

        public class GetVacationInfo
        {
            public string EmployeeId { get; }

            public GetVacationInfo(string employeeId)
            {
                this.EmployeeId = employeeId;
            }

            public class Response
            {
                public int VacationsCredit { get; }

                public Response(int vacationsCredit)
                {
                    this.VacationsCredit = vacationsCredit;
                }
            }
        }
    }
}