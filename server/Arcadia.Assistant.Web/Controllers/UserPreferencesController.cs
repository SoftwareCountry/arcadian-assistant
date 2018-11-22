namespace Arcadia.Assistant.Web.Controllers
{
    using System;
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
        private readonly IUserEmployeeSearch userEmployeeSearch;

        public UserPreferencesController(IUserPreferencesService userPreferencesService, IUserEmployeeSearch userEmployeeSearch)
        {
            this.userPreferencesService = userPreferencesService;
            this.userEmployeeSearch = userEmployeeSearch;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserPreferencesModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserPreferences(CancellationToken cancellationToken)
        {
            var user = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);
            if (user == null)
            {
                return this.Forbid();
            }

            var userPreferences = await this.userPreferencesService.GetUserPreferences(
                user.Metadata.EmployeeId,
                cancellationToken);
            return this.Ok(userPreferences);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveUserPreferences(
            UserPreferencesModel userPreferencesModel,
            CancellationToken cancellationToken)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);
            if (user == null)
            {
                return this.Forbid();
            }

            var response = await this.userPreferencesService.SaveUserPreferences(
                user.Metadata.EmployeeId,
                userPreferencesModel,
                cancellationToken);

            switch (response)
            {
                case SaveUserPreferencesMessage.Success _:
                    return this.NoContent();

                case SaveUserPreferencesMessage.Error error:
                    return this.BadRequest(error.ErrorMessage);

                default:
                    return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}