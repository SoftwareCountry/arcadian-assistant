namespace Arcadia.Assistant.WorkHoursCredit.Model
{
    using Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class WorkHoursCreditContext : DbContext
    {
        public DbSet<ChangeRequest> ChangeRequests { get; set; }

        public DbSet<Approval> Approvals { get; set; }

        public DbSet<Cancellation> Cancellations { get; set; }

        public DbSet<Rejection> Rejections { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public WorkHoursCreditContext()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public WorkHoursCreditContext(DbContextOptions options) : base(options)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ChangeRequest>()
                .HasIndex(x => new { x.EmployeeId, ChangeId = x.ChangeRequestId });

            modelBuilder
                .Entity<ChangeRequest>()
                .HasKey(x => new { x.EmployeeId, x.ChangeRequestId });

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
                .HasIndex(x => new { x.EmployeeId, x.ChangeRequestId });


            modelBuilder
                .Entity<Cancellation>()
                .HasIndex(x => new { x.EmployeeId, x.ChangeRequestId });


            modelBuilder
                .Entity<Rejection>()
                .HasIndex(x => new { x.EmployeeId, x.ChangeRequestId });
        }
    }
}