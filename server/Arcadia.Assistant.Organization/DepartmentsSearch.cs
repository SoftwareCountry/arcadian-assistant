namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class DepartmentsSearch : UntypedActor, ILogReceive
    {
        private readonly DepartmentsQuery departmentsQuery;

        private readonly IReadOnlyDictionary<string, IActorRef> departments;

        private readonly IActorRef requestor;

        private readonly HashSet<IActorRef> actorsToRespond = new HashSet<IActorRef>();

        private readonly List<DepartmentsQuery.DepartmentFinding> departmentFindings = new List<DepartmentsQuery.DepartmentFinding>();

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

                case DepartmentActor.GetDepartmentInfo.Result info:
                    this.actorsToRespond.Remove(info.DepartmentActor);

                    this.departmentFindings.Add(new DepartmentsQuery.DepartmentFinding(info.Department, info.DepartmentActor));

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
                var filteredDepartments = this.PostfilterDepartments();
                this.FinishSearch(filteredDepartments);
            }
        }

        private IReadOnlyCollection<DepartmentsQuery.DepartmentFinding> PostfilterDepartments()
        {
            return this.departmentFindings;
        }

        private HashSet<IActorRef> PrefilterDepartments()
        {
            //first, id-based filter
            var prefilteredDepartments = new HashSet<IActorRef>(this.departments.Values);

            if (this.departmentsQuery.DepartmentId != null)
            {
                if (this.departments.TryGetValue(this.departmentsQuery.DepartmentId, out var department))
                {
                    prefilteredDepartments.IntersectWith(new [] { department });
                }
                else
                {
                    prefilteredDepartments.Clear();
                }
            }

            return prefilteredDepartments;
        }

        private void FinishSearch(IReadOnlyCollection<DepartmentsQuery.DepartmentFinding> findings)
        {
            this.requestor.Tell(new DepartmentsQuery.Response(findings));
            Context.Stop(this.Self);
        }

        /*
        private void GetDescendants(string departmentId)
        {
            var children = this.departments
                .Where(x => x.Department.ParentDepartmentId == node)
                .ToList();

            children.AddRange(children.SelectMany(child => GetDescendants(child.Department.DepartmentId, allDepartmentFindings)));

            return children;
        }*/

        private class StartSearch
        {
        }
    }
}