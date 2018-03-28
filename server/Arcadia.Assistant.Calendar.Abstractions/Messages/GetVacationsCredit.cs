namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public sealed class GetVacationsCredit
    {
        public static readonly GetVacationsCredit Instance = new GetVacationsCredit();

        public sealed class Response
        {
            public int VacationsCredit { get; }

            public Response(int vacationsCredit)
            {
                this.VacationsCredit = vacationsCredit;
            }
        }
    }
}