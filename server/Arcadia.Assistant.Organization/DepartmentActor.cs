﻿namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Util.Internal;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class DepartmentActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private DepartmentInfo departmentInfo;
        private readonly IEnumerable<string> departmentFeatures;

        private readonly IActorRef organizationEmployeesActor;

        private readonly List<EmployeeContainer> employees = new List<EmployeeContainer>();

        private readonly IActorRef feed;

        private EmployeeContainer head;

        //        private EmployeeContainer headEmployee;

        public IStash Stash { get; set; }

        public DepartmentActor(
            DepartmentInfo departmentInfo,
            IActorRef organizationEmployeesActor,
            IEnumerable<DepartmentFeaturesMapping> departmentFeaturesSettings)
        {
            this.departmentInfo = departmentInfo;

            var features = departmentFeaturesSettings.FirstOrDefault(x => x.DepartmentId == departmentInfo.DepartmentId);
            this.departmentFeatures = features?.Features ?? Enumerable.Empty<string>();

            this.organizationEmployeesActor = organizationEmployeesActor;
            this.feed = Context.ActorOf(Props.Create(() => new DepartmentFeedActor(departmentInfo)), "feed");

            this.RefreshFeedsInformation();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshDepartmentInfo newInfo when newInfo.Department.DepartmentId == this.departmentInfo.DepartmentId:
                    this.StartRefreshing(newInfo.Department, this.Sender);

                    break;

                case GetDepartmentInfo _:
                    var container = new DepartmentContainer(this.departmentInfo, this.Self, this.head, this.employees.ToList(), this.feed);
                    this.Sender.Tell(new GetDepartmentInfo.Result(container));
                    break;

                case GetDepartmentFeatures msg:
                    if (this.departmentInfo.DepartmentId == msg.DepartmentId)
                    {
                        this.Sender.Tell(new GetDepartmentFeatures.Success(this.departmentFeatures.ToArray()));
                    }
                    else
                    {
                        this.Sender.Tell(GetDepartmentFeatures.NotFound.Instance);
                    }

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void StartRefreshing(DepartmentInfo newDepartmentInfo, IActorRef requestor)
        {
            this.departmentInfo = newDepartmentInfo;

            if (this.departmentInfo.ChiefId != newDepartmentInfo.ChiefId)
            {
                //TODO record head change
                //this.headEmployee = null;
                //this.employees.Tell(new EmployeesActor.FindEmployee(newInfo.Department.ChiefId));
            }

            this.RefreshHead(requestor);
        }

        private void RefreshHead(IActorRef requestor)
        {
            var requestors = new List<IActorRef>() { requestor };

            void RefreshingHead(object message)
            {
                switch (message)
                {
                    case EmployeesQuery.Response queryResult:
                        this.head = queryResult.Employees.FirstOrDefault();
                        this.RefreshEmployees(requestors);
                        break;

                    case RefreshDepartmentInfo newInfo when newInfo.Department.DepartmentId == this.departmentInfo.DepartmentId:
                        requestors.Add(this.Sender);
                        break;

                    default:
                        this.Stash.Stash();
                        break;
                }
            }

            this.organizationEmployeesActor.Tell(EmployeesQuery.Create().WithId(this.departmentInfo.ChiefId));
            this.Become(RefreshingHead);
        }

        private void RefreshEmployees(List<IActorRef> requestors)
        {
            void RefreshingEmployees(object message)
            {
                switch (message)
                {
                    case EmployeesQuery.Response queryResult:
                        this.employees.Clear();
                        this.employees.AddRange(queryResult.Employees);

                        this.Become(this.RefreshFinished(requestors));
                        break;

                    case RefreshDepartmentInfo newInfo when newInfo.Department.DepartmentId == this.departmentInfo.DepartmentId:
                        requestors.Add(this.Sender);
                        break;

                    default:
                        this.Stash.Stash();
                        break;
                }
            }

            this.organizationEmployeesActor.Tell(EmployeesQuery.Create().ForDepartment(this.departmentInfo.DepartmentId));
            this.Become(RefreshingEmployees);
        }

        private UntypedReceive RefreshFinished(IEnumerable<IActorRef> refreshRequesters)
        {
            refreshRequesters.ForEach(x => x.Tell(RefreshDepartmentInfo.Finished.Instance));

            this.RefreshFeedsInformation();

            this.Stash.UnstashAll();
            return this.OnReceive;
        }

        private void RefreshFeedsInformation()
        {
            this.feed.Tell(new DepartmentFeedActor.AssignInformation(this.employees.Select(x => x.Feed), this.departmentInfo));
        }

        public static Props GetProps(DepartmentInfo department, IActorRef organizationEmployeesActor, IEnumerable<DepartmentFeaturesMapping> departmentFeaturesSettings) =>
            Props.Create(() => new DepartmentActor(department, organizationEmployeesActor, departmentFeaturesSettings));

        public sealed class RefreshDepartmentInfo
        {
            public DepartmentInfo Department { get; }

            public RefreshDepartmentInfo(DepartmentInfo department)
            {
                this.Department = department;
            }

            public sealed class Finished
            {
                public static readonly Finished Instance = new Finished();
            }
        }


        public sealed class GetDepartmentInfo
        {
            public static readonly GetDepartmentInfo Instance = new GetDepartmentInfo();

            public sealed class Result
            {
                public DepartmentContainer Department { get; }

                public Result(DepartmentContainer department)
                {
                    this.Department = department;
                }
            }
        }
    }
}