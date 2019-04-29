namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.CSP.Sharepoint;
    using Arcadia.Assistant.CSP.SickLeaves;
    using Arcadia.Assistant.CSP.Vacations;
    using Arcadia.Assistant.Organization.Abstractions;

    using Microsoft.EntityFrameworkCore;

    public class CspDepartmentsStorage : DepartmentsStorage
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        private readonly CspConfiguration configuration;

        private string lastErrorMessage;

        public CspDepartmentsStorage(
            Func<ArcadiaCspContext> contextFactory,
            CspConfiguration configuration)
        {
            this.contextFactory = contextFactory;
            this.configuration = configuration;

            Context.ActorOf(CspVacationsRegistry.CreateProps(), "csp-vacations-registry");
            Context.ActorOf(VacationAccountingReadyReminderActor.CreateProps(), "csp-vacations-reminder");

            Context.ActorOf(CspSickLeavesRegistry.CreateProps(), "csp-sick-leaves-registry");
            Context.ActorOf(SickLeaveEndingReminderActor.CreateProps(), "csp-sick-leaves-reminder");

            Context.ActorOf(SharepointActor.CreateProps(), "csp-sharepoint-actor");
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

        private readonly Expression<Func<Department, IEnumerable<Employee>, DepartmentInfo>> mapDepartment =
            (x, chiefsGroup) =>
                new DepartmentInfo(
                    x.Id.ToString(),
                    x.Name,
                    x.Abbreviation,
                    (x.ParentDepartmentId == null) || (x.Id == x.ParentDepartmentId) ? null : x.ParentDepartmentId.ToString())
                { ChiefId = (x.ChiefId == null) || !chiefsGroup.Any() ? null : x.ChiefId.ToString() };

        private readonly Expression<Func<DepartmentInfo, IEnumerable<Employee>, DepartmentInfoWithPeopleCount>> countEmployees =
            (d, employees) =>
                new DepartmentInfoWithPeopleCount()
                {
                    Info = d,
                    PeopleCount = employees.Count()
                };

        protected override async Task<LoadAllDepartments.Response> GetAllDepartments()
        {
            try
            {
                var departments = await GetAllDepartmentsInternal();
                this.lastErrorMessage = null;
                return departments;
            }
            catch (Exception ex)
            {
                this.lastErrorMessage = ex.ToString();
                throw;
            }
        }

        private async Task<LoadAllDepartments.Response> GetAllDepartmentsInternal()
        {
            using (var context = this.contextFactory())
            {
                var arcEmployees = new CspEmployeeQuery(context, this.configuration).Get();

                var allDepartments = await context
                    .Department
                    .Where(x => (x.IsDelete != true) && (x.CompanyId == this.configuration.CompanyId))
                    .GroupJoin(arcEmployees, d => d.ChiefId, e => e.Id, this.mapDepartment)
                    .GroupJoin(arcEmployees, d => d.DepartmentId, e => e.DepartmentId.ToString(), this.countEmployees)
                    .ToListAsync();

                var head = allDepartments.FirstOrDefault(x => x.Info.IsHeadDepartment && (x.Info.Abbreviation == this.configuration.HeadDepartmentAbbreviation));

                if (head == null)
                {
                    return new LoadAllDepartments.Response(new DepartmentInfo[0]);
                }

                var processedIds = new HashSet<string>() { head.Info.DepartmentId };
                var tree = new DepartmentsTreeNode(head.Info, head.PeopleCount, this.CreateTree(allDepartments, head.Info.DepartmentId, processedIds));

                var departments = tree.AsEnumerable().Where(x => x.CountAllEmployees() != 0).Select(x => x.DepartmentInfo).ToList();

                return new LoadAllDepartments.Response(departments);
            }
        }

        private class DepartmentInfoWithPeopleCount
        {
            public DepartmentInfo Info { get; set; }

            public int PeopleCount { get; set; }
        }

        /// <param name="departmentId">Node for which descandants are requested</param>
        /// <param name="processedIds">a hashset with processed department ids to prevent stackoverflow</param>
        /// <param name="allDepartments">all possible departments</param>
        private List<DepartmentsTreeNode> CreateTree(IReadOnlyCollection<DepartmentInfoWithPeopleCount> allDepartments,
            string departmentId,
            HashSet<string> processedIds)
        {
            var childrenDepartments = allDepartments
                .Where(x => (x.Info.ParentDepartmentId == departmentId) && !processedIds.Contains(x.Info.DepartmentId))
                .ToList();

            processedIds.UnionWith(childrenDepartments.Select(x => x.Info.DepartmentId));

            var children = new List<DepartmentsTreeNode>();
            foreach (var department in childrenDepartments)
            {
                var child = new DepartmentsTreeNode(department.Info, department.PeopleCount, this.CreateTree(allDepartments, department.Info.DepartmentId, processedIds));
                children.Add(child);
            }

            return children;
        }
    }
}