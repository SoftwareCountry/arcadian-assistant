namespace Arcadia.Assistant.CSP
{
    public class GetVacationRegistryStatusMessage
    {
        public static readonly GetVacationRegistryStatusMessage Instance = new GetVacationRegistryStatusMessage();

        public class GetVacationRegistryStatusResponse
        {
            public string Message { get; }

            public GetVacationRegistryStatusResponse(string message)
            {
                Message = message;
            }
        }
    }
}