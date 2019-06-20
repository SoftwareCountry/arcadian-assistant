namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Avatars.Contracts;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    [Route("api/photo/employee")]

    public class EmployeePhotoController : Controller
    {
        private readonly IAvatars avatars;

        public EmployeePhotoController(IAvatars avatars)
        {
            this.avatars = avatars;
        }

        [Route("{employeeId}")]
        [HttpGet]
        public async Task<IActionResult> GetImage(string employeeId, CancellationToken token)
        {
            var actor = this.avatars.Get(employeeId);
            var photo = await actor.GetPhoto(token);
            if (photo == null)
            {
                return this.NotFound();
            }

            return this.File(photo.Bytes, photo.MimeType);
        }
    }
}