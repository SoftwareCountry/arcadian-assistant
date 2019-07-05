namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using System;

    using Contracts;

    using Microsoft.EntityFrameworkCore;

    public class WorkHoursCreditContext : DbContext
    {
        public DbSet<ChangeRequest> ChangeRequests { get; set; }

        public DbSet<Approval> Approvals { get; set; }

        public DbSet<Cancellation> Cancellations { get; set; }

        public DbSet<Rejection> Rejections { get; set; }

        public WorkHoursCreditContext()
        {
        }

        public WorkHoursCreditContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ChangeRequest>()
                .HasIndex(x => new { x.EmployeeId, ChangeId = x.ChangeRequestId });

            modelBuilder
                .Entity<ChangeRequest>()
                .Property(e => e.ChangeType)
                .HasConversion(x => x.ToString(), x => Enum.Parse<WorkHoursChangeType>(x, true));

            modelBuilder
                .Entity<ChangeRequest>()
                .Property(e => e.DayPart)
                .HasConversion(x => x.ToString(), x => Enum.Parse<DayPart>(x, true));

            modelBuilder
                .Entity<Approval>()
                .HasIndex(x => new { x.ChangeRequestId });


            modelBuilder
                .Entity<Cancellation>()
                .HasIndex(x => new { x.ChangeRequestId });


            modelBuilder
                .Entity<Rejection>()
                .HasIndex(x => new { x.ChangeRequestId });
        }
    }
}