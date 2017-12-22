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
    public class DepartmentsSearch : UntypedActor
    {
        private readonly DepartmentsQuery query;

        private readonly HashSet<IActorRef> requesters;

        private readonly HashSet<IActorRef> actorsToReply = new HashSet<IActorRef>();

        private readonly HashSet<DepartmentsQuery.DepartmentFinding> findings = new HashSet<DepartmentsQuery.DepartmentFinding>();

        public DepartmentsSearch(IActorRef searchRootDepartment, IEnumerable<IActorRef> requesters, DepartmentsQuery query)
        {
            this.query = query;

            this.requesters = new HashSet<IActorRef>(requesters);

            this.actorsToReply.Add(searchRootDepartment);

            if ((this.requesters.Count == 0) || (searchRootDepartment == null))
            {
                Context.Stop(this.Self);
            }

            this.Self.Tell(StartSearch.Instance);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartSearch _:
                    foreach (var actorRef in this.actorsToReply)
                    {
                        actorRef.Tell(DepartmentActor.GetDepartmentInfo.Instance);
                    }

                    this.Become(this.GatheringDepartments);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void GatheringDepartments(object message)
        {
            switch (message)
            {
                case DepartmentActor.GetDepartmentInfo.Result result:


                    this.findings.Add(new DepartmentsQuery.DepartmentFinding(result.Department, result.DepartmentActor));

                    foreach (var actor in result.Children)
                    {
                        this.actorsToReply.Add(actor);
                        actor.Tell(DepartmentActor.GetDepartmentInfo.Instance);
                    }

                    this.actorsToReply.Remove(this.Sender);

                    if (this.actorsToReply.Count == 0)
                    {
                        this.FilterFindings();
                        this.Self.Tell(SearchFinished.Instance);
                        this.Become(this.SearchCompleted);
                    }

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void FilterFindings()
        {
            if (!this.query.ShouldLoadAllDepartments)
            {
                this.findings.RemoveWhere(x => x.Department.DepartmentId != this.query.DepartmentId);
            }
        }

        private void SearchCompleted(object message)
        {
            switch (message)
            {
                case SearchFinished _:
                    //reply back to requester

                    foreach (var requester in this.requesters)
                    {
                        requester.Tell(new DepartmentsQuery.Response(this.findings));
                    }

                    Context.Stop(this.Self);

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private class StartSearch
        {
            public static readonly StartSearch Instance = new StartSearch();
        }

        private class SearchFinished
        {
            public static readonly SearchFinished Instance = new SearchFinished();
        }
    }
}