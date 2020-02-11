namespace Arcadia.Assistant.WorkHoursCredit
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using Employees.Contracts;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Model;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class WorkHoursCredit : StatelessService, IWorkHoursCredit
    {
        private readonly Func<Owned<WorkHoursCreditContext>> dbFactory;
        private readonly ILogger logger;

        public WorkHoursCredit(StatelessServiceContext context, Func<Owned<WorkHoursCreditContext>> dbFactory, ILogger logger)
            : base(context)
        {
            this.dbFactory = dbFactory;
            this.logger = logger;
        }

        public async Task<Dictionary<EmployeeId, int>> GetAvailableHoursAsync(EmployeeId[] employeeIds, CancellationToken cancellationToken)
        {
            if (employeeIds.Length == 0)
            {
                return new Dictionary<EmployeeId, int>();
            }

            var ids = new HashSet<int>(employeeIds.Select(x => x.Value));

            using var ctx = this.dbFactory();
            var hours = await ctx.Value.ChangeRequests
                .Where(x => ids.Contains(x.EmployeeId))
                .Where(x => x.Approvals.Any())
                .Select(x => new
                {
                    x.EmployeeId,
                    Multiplier = x.ChangeType == WorkHoursChangeType.Workout ? 1 : -1,
                    Hours = x.DayPart == DayPart.Full ? 8 : 4
                })
                .GroupBy(x => x.EmployeeId)
                .Select(x => new { EmployeeId = x.Key, Hours = x.Sum(ch => ch.Multiplier * ch.Hours) })
                .ToDictionaryAsync(x => new EmployeeId(x.EmployeeId), x => x.Hours, cancellationToken);

            foreach (var employeeId in employeeIds)
            {
                if (!hours.ContainsKey(employeeId))
                {
                    hours[employeeId] = 0;
                }
            }

            return hours;
        }

        public async Task<WorkHoursChange[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
            var events = await this.QueryCalendarEvents(ctx.Value.ChangeRequests, x => x.EmployeeId == employeeId.Value)
                .ToArrayAsync(cancellationToken);

            return events;
        }

        public async Task<Dictionary<EmployeeId, WorkHoursChange[]>> GetCalendarEventsByEmployeeMapAsync(EmployeeId[] employeeIds, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
            var events = await this.QueryCalendarEvents(ctx.Value.ChangeRequests, x => employeeIds.Any(id => id.Value == x.EmployeeId))
                .ToArrayAsync(cancellationToken);

            return events.GroupBy(x=> x.EmployeeId).ToDictionary(x => x.Key, x=> x.ToArray());
        }

        public async Task<WorkHoursChange?> GetCalendarEventAsync(EmployeeId employeeId, Guid eventId, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
            var calendarEvent = await this.QueryCalendarEvents(
                    ctx.Value.ChangeRequests, 
                    x => x.EmployeeId == employeeId.Value && x.ChangeRequestId == eventId)
                .FirstOrDefaultAsync(cancellationToken);

            return calendarEvent;
        }

        public async Task<Dictionary<EmployeeId, WorkHoursChange[]>> GetActiveRequestsAsync(EmployeeId[] employeeIds, CancellationToken cancellationToken)
        {
            if (employeeIds.Length == 0)
            {
                return new Dictionary<EmployeeId, WorkHoursChange[]>();
            }

            var ids = new HashSet<int>(employeeIds.Select(x => x.Value));

            using var ctx = this.dbFactory();
            var events = await this.QueryCalendarEvents(
                    ctx.Value.ChangeRequests,
                    x => ids.Contains(x.EmployeeId))
                .Where(x => x.Status != ChangeRequestStatus.Cancelled && x.Status != ChangeRequestStatus.Approved)
                .ToListAsync(cancellationToken);

            var groupedEvents = events.GroupBy(x => x.EmployeeId).ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var employeeId in employeeIds)
            {
                if (!groupedEvents.ContainsKey(employeeId))
                {
                    groupedEvents[employeeId] = new WorkHoursChange[0];
                }
            }

            return groupedEvents;
        }

        public async Task<WorkHoursChange> RequestChangeAsync(EmployeeId employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart)
        {
            using var ctx = this.dbFactory();
            var entity = new ChangeRequest
            {
                ChangeRequestId = Guid.NewGuid(),
                EmployeeId = employeeId.Value,
                ChangeType = changeType,
                Date = date,
                DayPart = dayPart,
                Timestamp = DateTimeOffset.Now
            };

            ctx.Value.ChangeRequests.Add(entity);
            await ctx.Value.SaveChangesAsync();

            return new WorkHoursChange()
            {
                ChangeId = entity.ChangeRequestId,
                ChangeType = changeType,
                DayPart = dayPart,
                Date = date,
                Status = ChangeRequestStatus.Requested
            };
        }

        public async Task<ChangeRequestApproval[]> GetApprovalsAsync(EmployeeId employeeId, Guid eventId, CancellationToken cancellationToken)
        {
            using var ctx = this.dbFactory();
            var approvals = await ctx.Value
                .ChangeRequests
                .Where(x => x.EmployeeId == employeeId.Value && x.ChangeRequestId == eventId)
                .Select(x => x
                    .Approvals
                    .Select(y => new ChangeRequestApproval() { ApproverId = new EmployeeId(y.ChangedByEmployeeId), Timestamp = y.Timestamp })
                    .ToArray()
                )
                .FirstOrDefaultAsync(cancellationToken);

            return approvals;
        }

        public async Task ApproveRequestAsync(EmployeeId employeeId, Guid requestId, EmployeeId approvedBy)
        {
            using var ctx = this.dbFactory();
            var entity = new Approval
            {
                EmployeeId = employeeId.Value,
                ChangeRequestId = requestId,
                Timestamp = DateTimeOffset.Now,
                ChangedByEmployeeId = approvedBy.Value
            };

            ctx.Value.Approvals.Add(entity);
            await ctx.Value.SaveChangesAsync();
        }

        public async Task RejectRequestAsync(EmployeeId employeeId, Guid requestId, string? rejectionReason, EmployeeId rejectedBy)
        {
            using var ctx = this.dbFactory();
            var entity = new Rejection
            {
                EmployeeId = employeeId.Value,
                ChangeRequestId = requestId,
                Timestamp = DateTimeOffset.Now,
                ChangedByEmployeeId = rejectedBy.Value,
                Comment = rejectionReason
            };

            ctx.Value.Rejections.Add(entity);
            await ctx.Value.SaveChangesAsync();
        }

        public async Task CancelRequestAsync(EmployeeId employeeId, Guid requestId, string? rejectionReason, EmployeeId cancelledBy)
        {
            using var ctx = this.dbFactory();
            var entity = new Cancellation()
            {
                EmployeeId = employeeId.Value,
                ChangeRequestId = requestId,
                Timestamp = DateTimeOffset.Now,
                ChangedByEmployeeId = cancelledBy.Value,
                Comment = rejectionReason
            };

            ctx.Value.Cancellations.Add(entity);
            await ctx.Value.SaveChangesAsync();
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        private IQueryable<WorkHoursChange> QueryCalendarEvents(IQueryable<ChangeRequest> changeRequests, Expression<Func<ChangeRequest, bool>> predicate)
        {
            var events = changeRequests
                .AsNoTracking()
                .Where(predicate)
                .Select(x => new WorkHoursChange
                {
                    EmployeeId = new EmployeeId(x.EmployeeId),
                    ChangeType = x.ChangeType,
                    Date = x.Date,
                    DayPart = x.DayPart,
                    ChangeId = x.ChangeRequestId,
                    Status =
                        x.Cancellations.Any() ? ChangeRequestStatus.Cancelled
                        : x.Rejections.Any() ? ChangeRequestStatus.Rejected
                        : x.Approvals.Any() ? ChangeRequestStatus.Approved
                        : ChangeRequestStatus.Requested
                });

            return events;
        }
    }
}