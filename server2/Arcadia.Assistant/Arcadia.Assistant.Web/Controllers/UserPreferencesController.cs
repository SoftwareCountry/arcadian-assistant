using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    using Models;

    using UserPreferences.Contracts;

    [Authorize]
    [Route("/api/user-preferences")]
    public class UserPreferencesController : Controller
    {
        private readonly IEmployees employees;
        private readonly IUsersPreferencesStorage usersPreferencesStorage;

        public UserPreferencesController(IEmployees employees, IUsersPreferencesStorage usersPreferencesStorage)
        {
            this.employees = employees;
            this.usersPreferencesStorage = usersPreferencesStorage;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserPreferencesModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserPreferencesModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserPreferences(CancellationToken cancellationToken)
        {
            var employee = (await this.employees
                .FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity.Name), cancellationToken))
                .FirstOrDefault();

            if (employee == null)
            {
                return this.NotFound();
            }

            var userPreferences = await this.usersPreferencesStorage.ForEmployee(employee.EmployeeId).Get(cancellationToken);
            var model = new UserPreferencesModel()
            {
                EmailNotifications = userPreferences.EmailNotifications,
                PushNotifications = userPreferences.PushNotifications
            };

            return this.Ok(model);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SaveUserPreferences(UserPreferencesModel userPreferencesModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var employee = (await this.employees
                    .FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity.Name), CancellationToken.None)).FirstOrDefault();

            if (employee == null)
            {
                return this.NotFound();
            }

            await this.usersPreferencesStorage.ForEmployee(employee.EmployeeId).Set(new UserPreferences()
            {
                EmailNotifications = userPreferencesModel.EmailNotifications,
                PushNotifications = userPreferencesModel.PushNotifications
            }, CancellationToken.None);
            return this.NoContent();
        }
    }
}