namespace Arcadia.Assistant.Server.Interop
{
    public static class Messages
    {
        public class Connect
        {
            public static readonly Connect Instance = new Connect();

            private Connect()
            {
            }

            public class Response
            {
                public ServerActorsCollection ServerActors { get; }

                public Response(ServerActorsCollection serverActors)
                {
                    this.ServerActors = serverActors;
                }
            }
        }
    }
}