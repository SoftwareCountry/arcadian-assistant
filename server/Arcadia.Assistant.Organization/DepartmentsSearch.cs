namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class DepartmentsSearch : UntypedActor, ILogReceive
    {
        private readonly DepartmentsQuery departmentsQuery;

        private readonly IReadOnlyDictionary<string, IActorRef> departments;

        private readonly IActorRef requestor;

        private readonly HashSet<IActorRef> actorsToRespond = new HashSet<IActorRef>();

        private readonly List<DepartmentContainer> departmentFindings = new List<DepartmentContainer>();

        public DepartmentsSearch(DepartmentsQuery departmentsQuery, IReadOnlyDictionary<string, IActorRef> departments, IActorRef requestor)
        {
            this.departmentsQuery = departmentsQuery;
            this.departments = departments;
            this.requestor = requestor;
            this.Self.Tell(new StartSearch());
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartSearch _:
                    var prefilteredDepartments = this.PrefilterDepartments();
                    this.RequestDepartmentsInfo(prefilteredDepartments);
                    break;

                case DepartmentActor.GetDepartmentInfo.Result result:
                    this.actorsToRespond.Remove(result.Department.DepartmentActor);

                    this.departmentFindings.Add(result.Department);

                    this.CheckIfSearchFinished();

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void RequestDepartmentsInfo(IEnumerable<IActorRef> prefilteredDepartments)
        {
            this.actorsToRespond.UnionWith(prefilteredDepartments);

            foreach (var actorRef in this.actorsToRespond)
            {
                actorRef.Tell(DepartmentActor.GetDepartmentInfo.Instance);
            }

            this.CheckIfSearchFinished();
        }

        private void CheckIfSearchFinished()
        {
            if (this.actorsToRespond.Count == 0)
            {
                var filteredDepartments = this.PostfilterDepartments().ToList();
                this.FinishSearch(filteredDepartments);
            }
        }

        private IEnumerable<DepartmentContainer> PostfilterDepartments()
        {
            IEnumerable<DepartmentContainer> result = this.departmentFindings;

            if (this.departmentsQuery.DepartmentId != null)
            {
                result = result.Where(x => x.Department.DepartmentId == this.departmentsQuery.DepartmentId);
            }

            if (this.departmentsQuery.DepartmentHeadEmployeeId != null)
            {
                result = result.Where(x => x.Department.ChiefId == this.departmentsQuery.DepartmentHeadEmployeeId);
            }

            var resultList = result.ToList();

            if (this.departmentsQuery.IncludeDirectDescendantDepartments)
            {
                var descendantDepartments = resultList
                    .SelectMany(d => GetDescendants(d.Department.DepartmentId, this.departmentFindings, 1))
                    .ToList();
                resultList.AddRange(descendantDepartments);
            }

            if (this.departmentsQuery.IncludeAllDescendantDepartments)
            {
                var descendantDepartments = resultList
                    .SelectMany(d => GetDescendants(d.Department.DepartmentId, this.departmentFindings))
                    .ToList();
                resultList.AddRange(descendantDepartments);
            }

            return resultList
                .GroupBy(d => d.Department.DepartmentId)
                .Select(g => g.First());
        }

        private HashSet<IActorRef> PrefilterDepartments()
        {
            var prefilteredDepartments = new HashSet<IActorRef>(this.departments.Values);

            if (this.departmentsQuery.DepartmentId != null)
            {
                if (this.departments.TryGetValue(this.departmentsQuery.DepartmentId, out var department))
                {
                    prefilteredDepartments.IntersectWith(new[] { department });
                }
                else
                {
                    prefilteredDepartments.Clear();
                }
            }

            return prefilteredDepartments;
        }

        private void FinishSearch(IReadOnlyCollection<DepartmentContainer> findings)
        {
            this.requestor.Tell(new DepartmentsQuery.Response(findings));
            Context.Stop(this.Self);
        }

        private static List<DepartmentContainer> GetDescendants(
            string departmentId,
            IEnumerable<DepartmentContainer> allDepartments,
            int maxNestingLevel = 0,
            int nestingLevel = 0)
        {
            if (maxNestingLevel != 0 && nestingLevel >= maxNestingLevel)
            {
                return new List<DepartmentContainer>();
            }

            var children = allDepartments
                .Where(x => x.Department.ParentDepartmentId == departmentId)
                .ToList();

            var descendants = children
                .SelectMany(child => GetDescendants(child.Department.DepartmentId, allDepartments, maxNestingLevel, nestingLevel + 1))
                .ToList();

            return children
                .Union(descendants)
                .ToList();
        }

        private class StartSearch
        {
        }
    }
}