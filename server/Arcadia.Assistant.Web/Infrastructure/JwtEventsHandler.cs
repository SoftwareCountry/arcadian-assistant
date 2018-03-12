namespace Arcadia.Assistant.Web.Infrastructure
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using NLog;

    public class JwtEventsHandler : JwtBearerEvents
    {
        private readonly ILogger logger = LogManager.GetLogger("Auth");

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            this.logger.Error(context.Exception, "JWT Error occurred");

            return base.AuthenticationFailed(context);
        }
    }
}