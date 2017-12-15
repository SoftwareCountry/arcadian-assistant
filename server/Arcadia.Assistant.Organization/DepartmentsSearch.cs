namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    /// <summary>
    /// Makes a search across root and all child departments, 
    /// then sends <c>OrganizationRequests.RequestDepartments.Response</c> message to all <c>requesters</c>, 
    /// then kills itself.
    /// </summary>
    public class DepartmentsSearch : UntypedActor, IWithUnboundedStash
    {
        private readonly IActorRef searchRootDepartment;

        private readonly string pattern;

        private readonly HashSet<IActorRef> requesters;

        private readonly HashSet<IActorRef> actorsToReply = new HashSet<IActorRef>();

        private readonly HashSet<OrganizationRequests.RequestDepartments.DepartmentFinding> findings = new HashSet<OrganizationRequests.RequestDepartments.DepartmentFinding>();

        public IStash Stash { get; set; }

        public DepartmentsSearch(IActorRef searchRootDepartment, IEnumerable<IActorRef> requesters, string pattern = null)
        {
            this.searchRootDepartment = searchRootDepartment;
            this.requesters = new HashSet<IActorRef>(requesters);

            this.pattern = pattern;

            if (this.actorsToReply.Count == 0)
            {
                this.Self.Tell(PoisonPill.Instance);
            }

            foreach (var requester in this.requesters)
            {
                this.Self.Tell(GetResults.Instance, requester);
            }
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetResults _:
                    this.Stash.Stash();

                    this.actorsToReply.Add(this.searchRootDepartment);

                    this.searchRootDepartment.Tell(DepartmentActor.GetDepartmentInfo.Instance);
                    this.Become(this.GetheringDepartments);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void GetheringDepartments(object message)
        {
            switch (message)
            {
                case DepartmentActor.GetDepartmentInfo.Result result:

                    this.findings.Add(new OrganizationRequests.RequestDepartments.DepartmentFinding(result.Department, result.DepartmentActor, result.Employees));

                    foreach (var actor in result.Children)
                    {
                        this.actorsToReply.Add(actor);
                        actor.Tell(DepartmentActor.GetDepartmentInfo.Instance);
                    }

                    this.actorsToReply.Remove(this.Sender);

                    if (this.actorsToReply.Count == 0)
                    {
                        this.Stash.UnstashAll();
                        this.Become(this.SearchCompleted);
                    }

                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void SearchCompleted(object message)
        {
            switch (message)
            {
                case GetResults msg:
                    //reply back to requester
                    this.Sender.Tell(msg.Response(this.findings));

                    //if no requesters left, kill itself
                    this.requesters.Remove(this.Sender);
                    if (this.requesters.Count == 0)
                    {
                        this.Self.Tell(PoisonPill.Instance);
                    }

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private class GetResults
        {
            public static readonly GetResults Instance = new GetResults();

            public OrganizationRequests.RequestDepartments.Response Response(IReadOnlyCollection<OrganizationRequests.RequestDepartments.DepartmentFinding> findings)
            {
                return new OrganizationRequests.RequestDepartments.Response(findings);
            }
        }
    }
}