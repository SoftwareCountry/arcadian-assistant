namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentsSearch : UntypedActor, IWithUnboundedStash
    {
        private readonly IActorRef searchRootDepartment;

        private readonly string pattern;

        private readonly HashSet<IActorRef> actorsToReply = new HashSet<IActorRef>();

        private readonly HashSet<OrganizationRequests.RequestDepartments.DepartmentFinding> findings = new HashSet<OrganizationRequests.RequestDepartments.DepartmentFinding>();

        public IStash Stash { get; set; }

        public DepartmentsSearch(IActorRef searchRootDepartment, string pattern = null)
        {
            this.searchRootDepartment = searchRootDepartment;
            this.pattern = pattern;
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
                    this.Sender.Tell(msg.Response(this.findings));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        public class GetResults
        {
            public static readonly GetResults Instance = new GetResults();

            public OrganizationRequests.RequestDepartments.Response Response(IReadOnlyCollection<OrganizationRequests.RequestDepartments.DepartmentFinding> findings)
            {
                return new OrganizationRequests.RequestDepartments.Response(findings);
            }
        }
    }
}