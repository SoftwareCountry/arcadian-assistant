namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    /// <summary>
    /// Makes a search across root and all child departments, 
    /// then sends <c>OrganizationRequests.RequestDepartments.Response</c> message to all <c>requesters</c>, 
    /// then kills itself.
    /// </summary>
    public class DepartmentsSearch : BaseSearch<
        FindDepartments.DepartmentFinding,
        DepartmentActor.GetDepartmentInfo,
        DepartmentActor.GetDepartmentInfo.Result,
        FindDepartments.Response>
    {
        private readonly string pattern;

        public DepartmentsSearch(IActorRef searchRootDepartment, IEnumerable<IActorRef> requesters, string pattern = null)
            : base(requesters, searchRootDepartment)
        {
        }

        protected override IEnumerable<FindDepartments.DepartmentFinding> ExtractFindings(DepartmentActor.GetDepartmentInfo.Result targetResponse)
        {
            yield return new FindDepartments.DepartmentFinding(targetResponse.Department, targetResponse.DepartmentActor, targetResponse.Employees);
        }

        protected override IEnumerable<IActorRef> GetAdditionalTargets(DepartmentActor.GetDepartmentInfo.Result targetResponse)
        {
            return targetResponse.Children;
        }

        protected override DepartmentActor.GetDepartmentInfo GetTargetQuery()
        {
            return DepartmentActor.GetDepartmentInfo.Instance;
        }

        protected override FindDepartments.Response FindingsToResponse(IReadOnlyCollection<FindDepartments.DepartmentFinding> findings)
        {
            return new FindDepartments.Response(findings);
        }
    }
}