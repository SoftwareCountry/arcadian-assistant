namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Assistant.UserPreferences;
    using Authorization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using UserPreferences;
    using Users;

    [Authorize(Policies.UserIsEmployee)]
    [Route("/api/user-preferences")]
    public class UserPreferencesController : Controller
    {
        private readonly IUserPreferencesService userPreferencesService;

        public UserPreferencesController(IUserPreferencesService userPreferencesService)
        {
            this.userPreferencesService = userPreferencesService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserPreferencesModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserPreferences(CancellationToken cancellationToken)
        {
            var userPreferences = await this.userPreferencesService.GetUserPreferences(
                this.User.Identity.Name,
                cancellationToken);
            return this.Ok(userPreferences);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveUserPreferences(
            UserPreferencesModel userPreferencesModel,
            CancellationToken cancellationToken)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var response = await this.userPreferencesService.SaveUserPreferences(
                this.User.Identity.Name,
                userPreferencesModel,
                cancellationToken);

            switch (response)
            {
                case SaveUserPreferencesMessage.Response _:
                    return this.NoContent();

                default:
                    return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}