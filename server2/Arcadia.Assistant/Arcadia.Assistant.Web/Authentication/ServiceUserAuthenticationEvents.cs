namespace Arcadia.Assistant.Web.Authentication
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Configuration;

    using ZNetCS.AspNetCore.Authentication.Basic;
    using ZNetCS.AspNetCore.Authentication.Basic.Events;

    public class ServiceUserAuthenticationEvents : BasicAuthenticationEvents
    {
        private readonly IBasicAuthenticationSettings authenticationSettings;

        public ServiceUserAuthenticationEvents(IBasicAuthenticationSettings authenticationSettings)
        {
            this.authenticationSettings = authenticationSettings;
        }

        public override Task ValidatePrincipalAsync(ValidatePrincipalContext context)
        {
            if (context.UserName == this.authenticationSettings.Login &&
                context.Password == this.authenticationSettings.Password)
            {
                var identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                    },
                    BasicAuthenticationDefaults.AuthenticationScheme);

                context.Principal = new ClaimsPrincipal(identity);
            }

            return Task.CompletedTask;
        }
    }
}