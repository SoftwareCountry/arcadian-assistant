namespace Arcadia.Assistant.Web.Authorization
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Configuration;
    using ZNetCS.AspNetCore.Authentication.Basic.Events;

    public class AuthenticationEvents : BasicAuthenticationEvents
    {
        private readonly IHealthEndpointAuthenticationSettings _healthEndpointAuthenticationSettings;

        public AuthenticationEvents(IHealthEndpointAuthenticationSettings healthEndpointAuthenticationSettings)
        {
            this._healthEndpointAuthenticationSettings = healthEndpointAuthenticationSettings;
        }

        public override Task ValidatePrincipalAsync(ValidatePrincipalContext context)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Test User", "Test Issuer") }));
            context.Principal = principal;

            return Task.CompletedTask;
        }
    }
}