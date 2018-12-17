namespace Arcadia.Assistant.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    using NLog;

    public class PermissionsActor : UntypedActor, ILogReceive
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan timeout;
        private readonly Dictionary<string, EmployeePermissionsEntry> permissionsForDepartments = new Dictionary<string, EmployeePermissionsEntry>();
        private readonly Dictionary<string, EmployeePermissionsEntry> permissionsForEmployees = new Dictionary<string, EmployeePermissionsEntry>();

        private EmployeePermissionsEntry defaultEmployeePermission = EmployeePermissionsEntry.None;

        private readonly ActorSelection organizationActor;

        private IActorRef originalSender;
        private string userEmail;

        public PermissionsActor(ActorSelection organizationActor, TimeSpan timeout)
        {
            this.timeout = timeout;
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
                        Log.Debug("Passed user identity is null.");
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
                        Log.Debug($"Cannot get permissions for '${this.userEmail}' due to timeout");
                        this.ReplyAndStop();
                        break;

                    case EmployeesQuery.Response userEmployee when userEmployee.Employees.Count == 0:
                        Log.Debug($"Cannot find an employee for '{this.userEmail}'");
                        this.ReplyAndStop();
                        break;

                    case EmployeesQuery.Response userEmployee:
                        this.defaultEmployeePermission = ExistingEmployeeDefaultPermission;
                        BulkBumpPermissions(userEmployee.Employees.Select(x => x.Metadata.EmployeeId), SelfPermissions, this.permissionsForEmployees);

                        //that can be fixed (as array, not First(), when DepartmentsQuery starts to 
                        //support arrays for Heads and DepartmentIds
                        this.Become(this.GetOwnDepartments(userEmployee.Employees.First().Metadata));
                        break;

                    default:
                        this.Unhandled(message);
                        break;
                }
            }

            this.organizationActor.Tell(EmployeesQuery.Create().WithEmail(this.userEmail));
            return OnMessage;
        }

        private UntypedReceive GetOwnDepartments(EmployeeMetadata employee)
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

                        this.Become(this.LoadSupervisedDepartmentsPermissions(employee.EmployeeId));

                        break;

                    case ReceiveTimeout _:
                        this.ReplyAndStop();
                        break;
                    default:
                        this.Unhandled(message);
                        break;
                }
            }

            this.organizationActor.Tell(DepartmentsQuery.Create().WithId(employee.DepartmentId));
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
                        BulkBumpPermissions(
                            response.Departments.Select(x => x.Department.DepartmentId),
                            SupervisedPermissions,
                            this.permissionsForDepartments);

                        this.ReplyAndStop();

                        break;

                    default:
                        this.Unhandled(message);
                        break;
                }
            }

            var departmentsQuery = DepartmentsQuery.Create()
                .WithHead(employeeId)
                .IncludeAllDescendants();
            this.organizationActor.Tell(departmentsQuery);

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
            EmployeePermissionsEntry.CompleteSickLeave |
            EmployeePermissionsEntry.ProlongSickLeave |
            EmployeePermissionsEntry.CancelPendingCalendarEvents |
            EmployeePermissionsEntry.EditPendingCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeePhone |
            EmployeePermissionsEntry.ReadEmployeeDayoffsCounter |
            EmployeePermissionsEntry.ReadEmployeeVacationsCounter;

        private static readonly EmployeePermissionsEntry SupervisedPermissions =
            EmployeePermissionsEntry.CreateCalendarEvents |
            EmployeePermissionsEntry.ApproveCalendarEvents |
            EmployeePermissionsEntry.RejectCalendarEvents |
            EmployeePermissionsEntry.CompleteSickLeave |
            EmployeePermissionsEntry.ProlongSickLeave |
            EmployeePermissionsEntry.CancelPendingCalendarEvents |
            EmployeePermissionsEntry.EditPendingCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeePhone |
            EmployeePermissionsEntry.ReadEmployeeDayoffsCounter |
            EmployeePermissionsEntry.ReadEmployeeVacationsCounter |
            EmployeePermissionsEntry.CancelApprovedCalendarEvents;

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