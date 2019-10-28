﻿namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Threading.Tasks;

    using Contracts;

    using CSP.Model;

    using Employees.Contracts;

    public class SickLeaveCreationStep
    {
        private readonly ArcadiaCspContext context;
        private readonly SickLeaveModelConverter converter = new SickLeaveModelConverter();

        public SickLeaveCreationStep(ArcadiaCspContext context)
        {
            this.context = context;
        }

        public async Task<SickLeaveDescription> InvokeAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate)
        {
            var newSickLeave = new CSP.Model.SickLeave()
            {
                EmployeeId = employeeId.Value,
                Start = startDate,
                End = endDate,
                RaisedAt = DateTimeOffset.Now
            };

            var sickLeave = this.context.SickLeaves.Add(newSickLeave);
            await this.context.SaveChangesAsync();

            return this.converter.ToDescription.Compile()(sickLeave.Entity);
        }
    }
}