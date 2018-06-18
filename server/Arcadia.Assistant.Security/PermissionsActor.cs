namespace Arcadia.Assistant.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class PermissionsActor : UntypedActor, ILogReceive
    {
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(1);
        private readonly Dictionary<string, EmployeePermissionsEntry> permissionsForDepartments = new Dictionary<string, EmployeePermissionsEntry>();
        private readonly Dictionary<string, EmployeePermissionsEntry> permissionsForEmployees = new Dictionary<string, EmployeePermissionsEntry>();

        private EmployeePermissionsEntry defaultEmployeePermission = EmployeePermissionsEntry.None;

        private readonly ActorSelection organizationActor;

        private IActorRef originalSender;
        private string userEmail;

        public PermissionsActor(ActorSelection organizationActor)
        {
            this.organizationActor = organizationActor;
            Context.SetReceiveTimeout(this.timeout);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetPermissions p:
                    this.originalSender = this.Sender;
                    this.userEmail = p.User.Identity.Name;
                    if (this.userEmail == null)
                    {
                        this.ReplyAndStop();
                    }

                    this.Become(this.GetUserEmployee());
                    
                    break;
            }
        }

        private UntypedReceive GetUserEmployee()
        {
            void OnMessage(object message)
            {
                switch (message)
                {
                    case ReceiveTimeout _:
                    case EmployeesQuery.Response userEmployee when userEmployee.Employees.Count == 0:
                        this.ReplyAndStop();
                        break;

                    case EmployeesQuery.Response userEmployee:
                        this.defaultEmployeePermission = ExistingEmployeeDefaultPermission;
                        BulkBumpPermissions(userEmployee.Employees.Select(x => x.Metadata.EmployeeId), SelfPermissions, this.permissionsForEmployees);

                        //that can be fixed (as array, not First(), when DepartmentsQuery starts to 
                        //support arrays for Heads and DepartmentIds
                        this.Become(this.GetOwnDepartments(userEmployee.Employees.First().Metadata.EmployeeId)); 
                        break;

                    default:
                        this.Unhandled(message);
                        break;
                }
            }

            this.organizationActor.Tell(EmployeesQuery.Create().WithEmail(this.userEmail));
            return OnMessage;
        }

        private UntypedReceive GetOwnDepartments(string employeeId)
        {
            void OnMessage(object message)
            {
                switch (message)
                {
                    case DepartmentsQuery.Response response:
                        BulkBumpPermissions(
                            response.Departments.Select(x => x.Department.DepartmentId),
                            OwnDepartmentPermissions,
                            this.permissionsForDepartments);

                        this.Become(this.LoadSupervisedDepartmentsPermissions(employeeId));

                        break;

                    case ReceiveTimeout _:
                        this.ReplyAndStop();
                        break;
                    default:
                        this.Unhandled(message);
                        break;
                }
            }

            this.organizationActor.Tell(DepartmentsQuery.Create().WithHead(employeeId));
            return OnMessage;
        }

        private UntypedReceive LoadSupervisedDepartmentsPermissions(string employeeId)
        {
            void OnMessage(object message)
            {
                switch (message)
                {
                    case DepartmentsQuery.Response r when r.Departments.Count == 0:
                    case ReceiveTimeout _:
                        this.ReplyAndStop();
                        break;

                    case DepartmentsQuery.Response response:

                        //setup permissions for directly supervised departments
                        BulkBumpPermissions(response.Departments.Select(x => x.Department.DepartmentId), SupervisedPermissions, this.permissionsForDepartments);

                        var tasks = response
                            .Departments
                            .Select(x => this.organizationActor.Ask<DepartmentsQuery.Response>(DepartmentsQuery.Create().DescendantOf(employeeId), this.timeout));

                        //setup permissions for branch-like supervised departments
                        Task.WhenAll(tasks).PipeTo(this.Self);
                        break;

                    //child departments
                    case DepartmentsQuery.Response[] responses:
                        var departmentsIds = responses.SelectMany(r => r.Departments.Select(d => d.Department.DepartmentId));
                        BulkBumpPermissions(departmentsIds, SupervisedPermissions, this.permissionsForDepartments);
                        this.ReplyAndStop();
                        break;

                    default:
                        this.Unhandled(message);
                        break;
                }
            }

            this.organizationActor.Tell(DepartmentsQuery.Create().WithHead(employeeId));
            return OnMessage;
        }

        private void ReplyAndStop()
        {
            var permissions = new Permissions(this.defaultEmployeePermission, this.permissionsForDepartments, this.permissionsForEmployees);
            this.originalSender.Tell(new GetPermissions.Response(permissions));
            Context.Stop(this.Self);
        }

        private static void BulkBumpPermissions(IEnumerable<string> entriesIds,
            EmployeePermissionsEntry permissionSet,
            Dictionary<string, EmployeePermissionsEntry> targetPermissionsEntries)
        {
            foreach (var entryId in entriesIds)
            {
                if (targetPermissionsEntries.ContainsKey(entryId))
                {
                    targetPermissionsEntries[entryId] |= permissionSet;
                }
                else
                {
                    targetPermissionsEntries[entryId] = permissionSet;
                }
            }
        }

        private static EmployeePermissionsEntry ExistingEmployeeDefaultPermission =
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents;

        private static readonly EmployeePermissionsEntry OwnDepartmentPermissions =
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeePhone;

        private static readonly EmployeePermissionsEntry SelfPermissions =
            EmployeePermissionsEntry.CreateCalendarEvents |
            EmployeePermissionsEntry.CompleteCalendarEvents |
            EmployeePermissionsEntry.ProlongCalendarEvents |
            EmployeePermissionsEntry.CancelCalendarEvents |
            EmployeePermissionsEntry.EditPendingCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeePhone |            
            EmployeePermissionsEntry.ReadEmployeeDayoffsCounter |
            EmployeePermissionsEntry.ReadEmployeeVacationsCounter;

        private static readonly EmployeePermissionsEntry SupervisedPermissions =
            EmployeePermissionsEntry.CreateCalendarEvents |
            EmployeePermissionsEntry.EditCalendarEvents |
            EmployeePermissionsEntry.EditPendingCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeePhone |
            EmployeePermissionsEntry.ReadEmployeeDayoffsCounter |
            EmployeePermissionsEntry.ReadEmployeeVacationsCounter;

        public class GetPermissions
        {
            public GetPermissions(ClaimsPrincipal user)
            {
                this.User = user;
            }

            public ClaimsPrincipal User { get; }

            public class Response
            {
                public Response(Permissions permissions)
                {
                    this.Permissions = permissions;
                }

                public Permissions Permissions { get; }
            }
        }
    }
}