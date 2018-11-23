namespace Arcadia.Assistant.CSP
{
    public class GetHealthCheckStatusMessage
    {
        public static readonly GetHealthCheckStatusMessage Instance = new GetHealthCheckStatusMessage();

        public class GetHealthCheckStatusResponse
        {
            public string Message { get; }

            public GetHealthCheckStatusResponse(string message)
            {
                Message = message;
            }
        }
    }
}