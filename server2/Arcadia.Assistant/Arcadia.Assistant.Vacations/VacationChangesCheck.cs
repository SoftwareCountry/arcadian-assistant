namespace Arcadia.Assistant.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CSP.WebApi.Contracts.Models;

    using VacationsDictionary = System.Collections.Generic.Dictionary<Employees.Contracts.EmployeeId, System.Collections.Generic.Dictionary<int, CSP.WebApi.Contracts.Models.Vacation>>;

    public class VacationChangesCheck
    {
        //private readonly Func<Owned<ArcadiaCspContext>> dbFactory;
        private VacationsDictionary? vacations;

        public VacationChangesCheck(/*Func<Owned<ArcadiaCspContext>> dbFactory*/)
        {
            //this.dbFactory = dbFactory;
        }

        public async Task PerformAsync(CancellationToken cancellationToken)
        {
            VacationsDictionary databaseState = new VacationsDictionary();
            /*
            using (var db = this.dbFactory())
            {

                var allVacations = db.Value.Vacations
                    //.Include(x => x.VacationCancellations)
                    //.Include(x => x.VacationApprovals)
                    //.Include(x => x.VacationProcesses)
                    //.Include(x => x.VacationReadies)
                    //.AsNoTracking()
                    .ToList();

                databaseState = allVacations
                    .GroupBy(x => x.EmployeeId)
                    .ToDictionary(x => new EmployeeId(x.Key), x => x.ToDictionary(y => y.Id));
            }
            */
            if (this.vacations == null)
            {
                //Initial run, just assign values
                this.vacations = databaseState;
                return;
            }

            var removedEmployees = this.vacations.Keys.Except(databaseState.Keys);
            foreach (var removedEmployee in removedEmployees)
            {
                this.vacations.Remove(removedEmployee);
            }

            foreach (var databaseEmployeeState in databaseState)
            {
                if (!this.vacations.ContainsKey(databaseEmployeeState.Key))
                {
                    this.vacations[databaseEmployeeState.Key] = new Dictionary<int, Vacation>();
                }

                var localEmployeeState = this.vacations[databaseEmployeeState.Key];
                foreach (var removedVacation in localEmployeeState.Keys.Except(databaseEmployeeState.Value.Keys))
                {
                    localEmployeeState.Remove(removedVacation);
                }

                foreach (var databaseRecord in databaseEmployeeState.Value)
                {
                    var eventId = databaseRecord.Key;
                    var databaseVacation = databaseRecord.Value;
                    if (localEmployeeState.TryGetValue(eventId, out var localVacation))
                    {
                        if (localVacation.Start != databaseVacation.Start || localVacation.End != databaseVacation.End)
                        {
                            //TODO: dates changed
                        }

                        var statusConverter = new StatusConverter();

                        if (statusConverter.GetStatus(localVacation) != statusConverter.GetStatus(databaseVacation))
                        {
                            //TODO: status changed
                        }
                    }
                    else
                    {
                        // TODO: vacation added, notify
                    }

                    localEmployeeState[eventId] = databaseRecord.Value;
                }
            }
        }
    }
}