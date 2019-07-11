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

    using Microsoft.EntityFrameworkCore;
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

        public WorkHoursCredit(StatelessServiceContext context, Func<Owned<WorkHoursCreditContext>> dbFactory)
            : base(context)
        {
            this.dbFactory = dbFactory;
        }

        public Task<int> GetAvailableHoursAsync(string employeeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<WorkHoursChange[]> GetCalendarEventsAsync(string employeeId, CancellationToken cancellationToken)
        {
            using (var ctx = this.dbFactory())
            {
                var events = await this.GetCalendarEvents(ctx.Value.ChangeRequests, x => x.EmployeeId == employeeId)
                    .ToArrayAsync(cancellationToken);

                return events;
            }
        }

        public async Task<WorkHoursChange> GetCalendarEventAsync(string employeeId, Guid eventId, CancellationToken cancellationToken)
        {
            using (var ctx = this.dbFactory())
            {
                var calendarEvent = await this.GetCalendarEvents(
                        ctx.Value.ChangeRequests, 
                        x => x.EmployeeId == employeeId && x.ChangeRequestId == eventId)
                    .FirstOrDefaultAsync(cancellationToken);

                return calendarEvent;
            }
        }

        public async Task<WorkHoursChange> RequestChangeAsync(string employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart)
        {
            using (var ctx = this.dbFactory())
            {
                var entity = new ChangeRequest
                {
                    ChangeRequestId = Guid.NewGuid(),
                    EmployeeId = employeeId,
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
        }

        public async Task<ChangeRequestApproval[]> GetApprovalsAsync(string employeeId, Guid eventId, CancellationToken cancellationToken)
        {
            using (var ctx = this.dbFactory())
            {
                var approvals = await ctx.Value
                    .ChangeRequests
                    .Where(x => x.EmployeeId == employeeId && x.ChangeRequestId == eventId)
                    .Select(x => x
                        .Approvals
                        .Select(y => new ChangeRequestApproval() { ApproverId = y.ChangedByEmployeeId, Timestamp = y.Timestamp })
                        .ToArray()
                    )
                    .FirstOrDefaultAsync(cancellationToken);

                return approvals;
            }
        }

        public async Task ApproveRequestAsync(string employeeId, Guid requestId, string approvedBy)
        {
            using (var ctx = this.dbFactory())
            {
                var entity = new Approval
                {
                    EmployeeId = employeeId,
                    ChangeRequestId = requestId,
                    Timestamp = DateTimeOffset.Now,
                    ChangedByEmployeeId = approvedBy
                };

                ctx.Value.Approvals.Add(entity);
                await ctx.Value.SaveChangesAsync();
            }
        }

        public async Task RejectRequestAsync(string employeeId, Guid requestId, string rejectionReason, string rejectedBy)
        {
            using (var ctx = this.dbFactory())
            {
                var entity = new Rejection
                {
                    EmployeeId = employeeId,
                    ChangeRequestId = requestId,
                    Timestamp = DateTimeOffset.Now,
                    ChangedByEmployeeId = rejectedBy,
                    Comment = rejectionReason
                };

                ctx.Value.Rejections.Add(entity);
                await ctx.Value.SaveChangesAsync();
            }
        }

        public async Task CancelRequestAsync(string employeeId, Guid requestId, string rejectionReason, string cancelledBy)
        {
            using (var ctx = this.dbFactory())
            {
                var entity = new Cancellation()
                {
                    EmployeeId = employeeId,
                    ChangeRequestId = requestId,
                    Timestamp = DateTimeOffset.Now,
                    ChangedByEmployeeId = cancelledBy,
                    Comment = rejectionReason
                };

                ctx.Value.Cancellations.Add(entity);
                await ctx.Value.SaveChangesAsync();
            }
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
        }

        private IQueryable<WorkHoursChange> GetCalendarEvents(IQueryable<ChangeRequest> changeRequests, Expression<Func<ChangeRequest, bool>> predicate)
        {
            var events = changeRequests
                .AsNoTracking()
                .Where(predicate)
                .Select(x => new WorkHoursChange
                {
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