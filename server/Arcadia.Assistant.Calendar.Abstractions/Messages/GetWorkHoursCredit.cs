namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public sealed class GetWorkHoursCredit
    {
        public static readonly GetWorkHoursCredit Instance = new GetWorkHoursCredit();

        public sealed class Response
        {
            public int WorkHoursCredit { get; }

            public Response(int workHoursCredit)
            {
                this.WorkHoursCredit = workHoursCredit;
            }
        }
    }
}