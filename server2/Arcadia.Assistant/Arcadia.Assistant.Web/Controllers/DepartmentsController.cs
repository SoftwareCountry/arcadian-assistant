﻿namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Authorization;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;

    using Organization.Contracts;

    [Route("/api/departments")]
    [ApiController]
    [Authorize(Policies.UserIsEmployee)]
    public class DepartmentsController : Controller
    {
        private readonly IOrganization organization;

        public DepartmentsController(IOrganization organization)
        {
            this.organization = organization;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DepartmentModel[]>> All(CancellationToken token)
        {
            var departments = await this.organization.GetDepartmentsAsync(token);
            return departments
                .OrderBy(x => x.DepartmentId)
                .Select(DepartmentModel.FromMetadata)
                .ToArray();
        }

        [Route("{departmentId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentModel>> Get(int departmentId, CancellationToken token)
        {
            var department = await this.organization.GetDepartmentAsync(new DepartmentId(departmentId), token);
            if (department == null)
            {
                return this.NotFound();
            }

            return DepartmentModel.FromMetadata(department);
        }
    }
}