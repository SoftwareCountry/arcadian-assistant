﻿namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class CspEmployeesInfoStorage : EmployeesInfoStorage
    {
        private readonly Func<ArcadiaCspContext> contextFactory;
        private readonly CspConfiguration cspConfiguration;
        private string lastErrorMessage;

        public CspEmployeesInfoStorage(Func<ArcadiaCspContext> contextFactory, CspConfiguration cspConfiguration)
        {
            this.contextFactory = contextFactory;
            this.cspConfiguration = cspConfiguration;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetHealthCheckStatusMessage _:
                    this.Sender.Tell(new GetHealthCheckStatusMessage.GetHealthCheckStatusResponse(this.lastErrorMessage));
                    break;

                default:
                    base.OnReceive(message);
                    break;
            }
        }

        protected override async Task<LoadAllEmployees.Response> GetAllEmployees()
        {
            try
            {
                var employees = await GetAllEmployeesInternal();
                this.lastErrorMessage = null;
                return employees;
            }
            catch (Exception ex)
            {
                this.lastErrorMessage = ex.ToString();
                throw;
            }
        }

        private async Task<LoadAllEmployees.Response> GetAllEmployeesInternal()
        {
            var userIdentityDomain = this.cspConfiguration.UserIdentityDomain;

            using (var context = this.contextFactory())
            {
                var employees = await new CspEmployeeQuery(context, this.cspConfiguration)
                    .Get()
                    .Select(x => new EmployeeStoredInformation(
                        new EmployeeMetadata(
                            x.Id.ToString(),
                            $"{x.LastName} {x.FirstName}".Trim(),
                            x.Email,
                            new[]
                            {
                                x.Email,
                                $"{x.LoginName}@{userIdentityDomain}"
                            }
                        )
                        {
                            BirthDate = x.Birthday,
                            HireDate = x.HiringDate,
                            FireDate = x.FiringDate,
                            MobilePhone = x.MobilePhone,
                            RoomNumber = x.RoomNumber != null ? x.RoomNumber.Trim() : null,
                            Position = x.Position.Title,
                            Sid = x.Sid.HasValue ? x.Sid.Value.ToString() : null,
                            DepartmentId = x.DepartmentId.HasValue ? x.DepartmentId.Value.ToString() : null,
                            Sex = x.Gender == "M"
                                ? Sex.Male
                                : x.Gender == "F"
                                    ? Sex.Female
                                    : Sex.Undefined
                        }
                    )
                    {
                        Photo = x.Image
                    })
                    .ToListAsync();
                return new LoadAllEmployees.Response(employees);
            }
        }
    }
}