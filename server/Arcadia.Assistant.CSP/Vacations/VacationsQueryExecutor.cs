namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Arcadia.Assistant.CSP.Model;

    using Microsoft.EntityFrameworkCore;

    public class VacationsQueryExecutor
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        private readonly Remote1CConfiguration bkConfiguration;

        public VacationsQueryExecutor(Func<ArcadiaCspContext> contextFactory, Remote1CConfiguration bkConfiguration)
        {
            this.contextFactory = contextFactory;
            this.bkConfiguration = bkConfiguration;
        }

        public async Task<Dictionary<string, double>> Fetch()
        {
            var bookkeepingInfo = await this.Get1CInformation();
            var cspRecords = await this.GetCspRecords();

            var daysCounters = cspRecords
                .GroupJoin(
                    bookkeepingInfo,
                    x => new RegistryRecordKey(x.NameRus, x.Birthday),
                    x => new RegistryRecordKey(x.NameRus, x.BirthDay),
                    (x, y) => new { x.Id, DaysLeft = y.Select(v => v.DaysLeft).FirstOrDefault() },
                    RegistryRecordKey.NameBirthdayComparer
                )
                .ToDictionary(x => x.Id.ToString(), x => x.DaysLeft);

            return daysCounters;
        }

        private async Task<List<VacationInfo1C>> Get1CInformation()
        {
            var uriBuilder = new UriBuilder(this.bkConfiguration.Url);
            var client = new EnterpriseV8(uriBuilder.Uri);
            client.Credentials = new NetworkCredential() { UserName = this.bkConfiguration.Username, Password = this.bkConfiguration.Password };

            var taskFactory = new TaskFactory();

            var personsQuery = GetPersonsQuery(client);
            var employeesQuery = GetEmployeesQuery(client);
            var vacQuery = GetEarnedVacationsQuery(client);
            var vacUsedQuery = GetUsedVacationsQuery(client);

            var persons = await taskFactory.FromAsync(personsQuery.BeginExecute, personsQuery.EndExecute, new object());
            var employees = await taskFactory.FromAsync(employeesQuery.BeginExecute, employeesQuery.EndExecute, new object());
            var allEarnedVacations = await taskFactory.FromAsync(vacQuery.BeginExecute, vacQuery.EndExecute, new object());
            var vacationsUsed = await taskFactory.FromAsync(vacUsedQuery.BeginExecute, vacUsedQuery.EndExecute, new object());

            var currentDate = DateTime.UtcNow.Date;

            var records = persons
                .Join(employees, x => x.PersonKey, x => x.PersonKey, (p, e) => new
                {
                    p.NameRus,
                    e.EmployeeKey,
                    p.Birthday
                })
                .GroupJoin(allEarnedVacations, x => x.EmployeeKey, x => x.EmployeeKey, (p, v) => new
                {
                    p.EmployeeKey,
                    p.Birthday,
                    p.NameRus,
                    VacationRights = v
                        .Where(x => x.Period <= currentDate)
                        .DefaultIfEmpty(new EarnedVacation1CRecord())
                        .Aggregate((i, j) => i.Period > j.Period ? i : j)
                        .DaysEarned ?? 0
                })
                .GroupJoin(vacationsUsed, x => x.EmployeeKey, x => x.EmployeeKey, (p, v) => new
                {
                    p.Birthday,
                    p.NameRus,
                    DaysLeft = p.VacationRights - v.Sum(x => x.DaysUsed ?? 0)
                })
                .Select(x => new VacationInfo1C()
                {
                    NameRus = x.NameRus,
                    BirthDay = x.Birthday,
                    DaysLeft = x.DaysLeft
                })
                .ToList();

            return records;
        }

        private static DataServiceQuery<UsedVacation1CRecord> GetUsedVacationsQuery(EnterpriseV8 client)
        {
            return (DataServiceQuery<UsedVacation1CRecord>)client.AccumulationRegister_ФактическиеОтпуска_RecordType
                .Select(x => new UsedVacation1CRecord()
                {
                    DaysUsed = x.Количество,
                    EmployeeKey = x.Сотрудник_Key.Value
                });
        }

        private static DataServiceQuery<EarnedVacation1CRecord> GetEarnedVacationsQuery(EnterpriseV8 client)
        {
            return (DataServiceQuery<EarnedVacation1CRecord>)client.InformationRegister_ЗаработанныеПраваНаОтпуска
                .Where(x => x.КоличествоДней > 0)
                .Select(x => new EarnedVacation1CRecord()
                {
                    DaysEarned = x.КоличествоДней,
                    EmployeeKey = x.Сотрудник_Key,
                    Period = x.Period
                });
        }

        private static DataServiceQuery<Employee1CRecord> GetEmployeesQuery(EnterpriseV8 client)
        {
            return (DataServiceQuery<Employee1CRecord>)client.Catalog_Сотрудники
                .Select(x => new Employee1CRecord()
                {
                    EmployeeKey = x.Ref_Key,
                    PersonKey = x.ФизическоеЛицо_Key.Value
                });
        }

        private static DataServiceQuery<Person1CRecord> GetPersonsQuery(EnterpriseV8 client)
        {
            return (DataServiceQuery<Person1CRecord>)client.Catalog_ФизическиеЛица
                            .Select(x => new Person1CRecord()
                            {
                                Birthday = x.ДатаРождения.Value,
                                PersonKey = x.Ref_Key,
                                NameRus = x.Description
                            });
        }

        private async Task<List<CspEmployeeRecord>> GetCspRecords()
        {
            using (var ctx = this.contextFactory())
            {
                var arcEmployees = new CspEmployeeQuery(ctx)
                    .Get()
                    .Where(x => x.Birthday != null);
                return await arcEmployees.Select(
                    x =>
                        new CspEmployeeRecord()
                        {
                            Id = x.Id,
                            NameRus = $"{x.LastNameRus} {x.FirstNameRus} {x.MiddleNameRus}",
                            Birthday = x.Birthday.Value
                        }).ToListAsync();
            }
        }

        private class Person1CRecord
        {
            public DateTime Birthday { get; set; }

            public Guid PersonKey { get; set; }

            public string NameRus { get; set; }
        }

        private class Employee1CRecord
        {
            public Guid PersonKey { get; set; }

            public Guid EmployeeKey { get; set; }
        }

        private class EarnedVacation1CRecord
        {
            public Guid EmployeeKey { get; set; }

            public DateTime Period { get; set; }

            public double? DaysEarned { get; set; }
        }

        private class UsedVacation1CRecord
        {
            public Guid EmployeeKey { get; set; }

            public double? DaysUsed { get; set; }
        }

        private class CspEmployeeRecord
        {
            public int Id { get; set; }

            public string NameRus { get; set; }

            public DateTime Birthday { get; set; }
        }
    }
}