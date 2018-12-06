namespace Arcadia.Assistant.Web.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;

    using NLog;

    public class AkkaTimeoutExceptionHandler : IMiddleware
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (AskTimeoutException ex)
            {
                Logger.Error(ex, "Akka timeout error occurred: {0}", context.Request.GetDisplayUrl());
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }
        }
    }

    public static class AkkaTimeoutExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseAkkaTimeoutExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AkkaTimeoutExceptionHandler>();
        }
    }
}