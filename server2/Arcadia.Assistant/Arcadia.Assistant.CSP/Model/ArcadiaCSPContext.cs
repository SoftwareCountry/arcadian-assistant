using Microsoft.EntityFrameworkCore;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class ArcadiaCspContext : DbContext
    {
        public ArcadiaCspContext()
        {
        }

        public ArcadiaCspContext(DbContextOptions<ArcadiaCspContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CertificationDatum> CertificationData { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyHistory> CompanyHistories { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Cspalert> Cspalerts { get; set; }
        public virtual DbSet<CspalertType> CspalertTypes { get; set; }
        public virtual DbSet<Csprole> Csproles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DepartmentHistory> DepartmentHistories { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeCertification> EmployeeCertifications { get; set; }
        public virtual DbSet<EmployeeCertificationHistory> EmployeeCertificationHistories { get; set; }
        public virtual DbSet<EmployeeCspalert> EmployeeCspalerts { get; set; }
        public virtual DbSet<EmployeeHistory> EmployeeHistories { get; set; }
        public virtual DbSet<EmployeePosition> EmployeePositions { get; set; }
        public virtual DbSet<EmployeePositionHistory> EmployeePositionHistories { get; set; }
        public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public virtual DbSet<EmployeeTeam> EmployeeTeams { get; set; }
        public virtual DbSet<EmployeeTeamHistory> EmployeeTeamHistories { get; set; }
        public virtual DbSet<ForeignPassport> ForeignPassports { get; set; }
        public virtual DbSet<ForeignPassportHistory> ForeignPassportHistories { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<SickLeave> SickLeaves { get; set; }
        public virtual DbSet<SickLeaveCancellation> SickLeaveCancellations { get; set; }
        public virtual DbSet<SickLeaveComplete> SickLeaveCompletes { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamHistory> TeamHistories { get; set; }
        public virtual DbSet<Vacation> Vacations { get; set; }
        public virtual DbSet<VacationApproval> VacationApprovals { get; set; }
        public virtual DbSet<VacationCancellation> VacationCancellations { get; set; }
        public virtual DbSet<VacationProcess> VacationProcesses { get; set; }
        public virtual DbSet<VacationReady> VacationReadies { get; set; }
        public virtual DbSet<VacationRemain> VacationRemains { get; set; }
        public virtual DbSet<Visa> Visas { get; set; }
        public virtual DbSet<VisaHistory> VisaHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<CertificationDatum>(entity =>
            {
                entity.HasIndex(e => e.CreatedBy)
                    .HasName("IX_CreatedBy");

                entity.HasIndex(e => e.EmployeeCertificationId)
                    .HasName("IX_EmployeeCertificationId");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CertificationData)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_dbo.CertificationData_dbo.Employee_CreatedBy");

                entity.HasOne(d => d.EmployeeCertification)
                    .WithMany(p => p.CertificationData)
                    .HasForeignKey(d => d.EmployeeCertificationId)
                    .HasConstraintName("FK_dbo.CertificationData_dbo.EmployeeCertifications_EmployeeCertificationId");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Company")
                    .IsUnique();
            });

            modelBuilder.Entity<CompanyHistory>(entity =>
            {
                entity.HasIndex(e => new { e.CompanyId, e.Modified })
                    .HasName("IX_CompanyHistory")
                    .IsUnique();

                entity.Property(e => e.BusinessCity).IsUnicode(false);

                entity.Property(e => e.BusinessCountry).IsUnicode(false);

                entity.Property(e => e.BusinessZip).IsUnicode(false);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyHistories)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyHistory_Company");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.CompanyHistories)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyHistory_Modified_Employee");
            });

            modelBuilder.Entity<Cspalert>(entity =>
            {
                entity.HasOne(d => d.AlertType)
                    .WithMany(p => p.Cspalerts)
                    .HasForeignKey(d => d.AlertTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CSPAlert_CSPAlertType");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Department")
                    .IsUnique();

                entity.HasOne(d => d.Chief)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.ChiefId)
                    .HasConstraintName("FK_Department_Employee");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Department_Company");

                entity.HasOne(d => d.ParentDepartment)
                    .WithMany(p => p.InverseParentDepartment)
                    .HasForeignKey(d => d.ParentDepartmentId)
                    .HasConstraintName("FK_Department_Department");
            });

            modelBuilder.Entity<DepartmentHistory>(entity =>
            {
                entity.HasIndex(e => new { e.DepartmentId, e.Modified })
                    .HasName("IX_DepartmentHistory")
                    .IsUnique();

                entity.HasOne(d => d.Chief)
                    .WithMany(p => p.DepartmentHistories)
                    .HasForeignKey(d => d.ChiefId)
                    .HasConstraintName("FK_DepartmentHistory_EmployeeHistory");

                entity.HasOne(d => d.CompanyHistory)
                    .WithMany(p => p.DepartmentHistories)
                    .HasForeignKey(d => d.CompanyHistoryId)
                    .HasConstraintName("FK_DepartmentHistory_CompanyHistory");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.DepartmentHistories)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DepartmentHistory_Department");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.DepartmentHistories)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DepartmentHistory_Modified_Employee");

                entity.HasOne(d => d.ParentDepartmentHistory)
                    .WithMany(p => p.InverseParentDepartmentHistory)
                    .HasForeignKey(d => d.ParentDepartmentHistoryId)
                    .HasConstraintName("FK_DepartmentHistory_Parent_DepartmentHistory");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.ClockNumber)
                    .HasName("IX_ClockNumber")
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .HasName("IX_Email");

                entity.HasIndex(e => e.Id)
                    .HasName("IX_Employee")
                    .IsUnique();

                entity.HasIndex(e => e.LoginName)
                    .HasName("IX_LoginName");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Employee_Company");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Employee_Department");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_Employee_Employeeosition");
            });

            modelBuilder.Entity<EmployeeCertification>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeCertifications)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_dbo.EmployeeCertifications_dbo.Employee_EmployeeId");
            });

            modelBuilder.Entity<EmployeeCertificationHistory>(entity =>
            {
                entity.HasIndex(e => e.EmployeeCertificationId)
                    .HasName("IX_EmployeeCertificationId");

                entity.HasIndex(e => e.EmployeeModifiedById)
                    .HasName("IX_EmployeeModifiedBy_Id");

                entity.HasOne(d => d.EmployeeCertification)
                    .WithMany(p => p.EmployeeCertificationHistories)
                    .HasForeignKey(d => d.EmployeeCertificationId)
                    .HasConstraintName("FK_dbo.EmployeeCertificationHistory_dbo.EmployeeCertifications_EmployeeCertificationId");

                entity.HasOne(d => d.EmployeeModifiedBy)
                    .WithMany(p => p.EmployeeCertificationHistories)
                    .HasForeignKey(d => d.EmployeeModifiedById)
                    .HasConstraintName("FK_dbo.EmployeeCertificationHistory_dbo.Employee_EmployeeModifiedBy_Id");
            });

            modelBuilder.Entity<EmployeeCspalert>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.CspalertId });

                entity.HasOne(d => d.Cspalert)
                    .WithMany(p => p.EmployeeCspalerts)
                    .HasForeignKey(d => d.CspalertId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeCSPAlert_CSPAlertType");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeCspalerts)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeCSPAlert_Employee");
            });

            modelBuilder.Entity<EmployeeHistory>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("IX_Email");

                entity.HasIndex(e => e.LoginName)
                    .HasName("IX_LoginName");

                entity.HasIndex(e => new { e.EmployeeId, e.Modified })
                    .HasName("IX_EmployeeHistory")
                    .IsUnique();

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeHistories)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_EmployeeHistory_CompanyHistory");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeHistories)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_EmployeeHistory_DepartmentHistory");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeHistoryEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeHistory_Employee");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.EmployeeHistoryModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeHistory_Modified_Employee");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.EmployeeHistories)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_EmployeeHistory_PositionHistory");
            });

            modelBuilder.Entity<EmployeePosition>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_EmployeePosition")
                    .IsUnique();
            });

            modelBuilder.Entity<EmployeePositionHistory>(entity =>
            {
                entity.HasIndex(e => new { e.EmployeePositionId, e.Modified })
                    .HasName("IX_EmployeePositionHistory")
                    .IsUnique();

                entity.HasOne(d => d.EmployeePosition)
                    .WithMany(p => p.EmployeePositionHistories)
                    .HasForeignKey(d => d.EmployeePositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeePositionHistory_EmployeePosition");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.EmployeePositionHistories)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeePositionHistory_Employee");
            });

            modelBuilder.Entity<EmployeeRole>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.RoleId });

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeRoles)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeRoles_Employee");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.EmployeeRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeRoles_CSPRoles");
            });

            modelBuilder.Entity<EmployeeTeam>(entity =>
            {
                entity.HasKey(e => new { e.EmpolyeeId, e.TeamId });

                entity.HasOne(d => d.Empolyee)
                    .WithMany(p => p.EmployeeTeams)
                    .HasForeignKey(d => d.EmpolyeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeam_Employee");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.EmployeeTeams)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeam_Team");
            });

            modelBuilder.Entity<EmployeeTeamHistory>(entity =>
            {
                entity.HasKey(e => new { e.EmpolyeeHistoryId, e.TeamHistoryId });

                entity.HasOne(d => d.EmpolyeeHistory)
                    .WithMany(p => p.EmployeeTeamHistories)
                    .HasForeignKey(d => d.EmpolyeeHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeamHistory_EmployeeHistory");

                entity.HasOne(d => d.TeamHistory)
                    .WithMany(p => p.EmployeeTeamHistories)
                    .HasForeignKey(d => d.TeamHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeamHistory_TeamHistory");
            });

            modelBuilder.Entity<ForeignPassport>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ForeignPassports)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ForeignPassport_dbo.Employee_EmployeeId");
            });

            modelBuilder.Entity<ForeignPassportHistory>(entity =>
            {
                entity.HasIndex(e => e.EmployeeHistoryId)
                    .HasName("IX_EmployeeHistoryId");

                entity.HasIndex(e => e.ModifiedBy)
                    .HasName("IX_ModifiedBy");

                entity.HasIndex(e => e.OriginId)
                    .HasName("IX_OriginId");

                entity.HasOne(d => d.EmployeeHistory)
                    .WithMany(p => p.ForeignPassportHistories)
                    .HasForeignKey(d => d.EmployeeHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ForeignPassportHistory_dbo.EmployeeHistory_EmployeeHistoryId");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ForeignPassportHistories)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ForeignPassportHistory_dbo.Employee_ModifiedBy");

                entity.HasOne(d => d.Origin)
                    .WithMany(p => p.ForeignPassportHistories)
                    .HasForeignKey(d => d.OriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ForeignPassportHistory_dbo.ForeignPassport_OriginId");
            });

            modelBuilder.Entity<SickLeave>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.SickLeaves)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SickLeaves_dbo.Employee_EmployeeId");
            });

            modelBuilder.Entity<SickLeaveCancellation>(entity =>
            {
                entity.HasIndex(e => e.ById)
                    .HasName("IX_ById");

                entity.HasIndex(e => e.SickLeaveId)
                    .HasName("IX_SickLeaveId");

                entity.HasOne(d => d.By)
                    .WithMany(p => p.SickLeaveCancellations)
                    .HasForeignKey(d => d.ById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SickLeaveCancellations_dbo.Employee_ById");

                entity.HasOne(d => d.SickLeave)
                    .WithMany(p => p.SickLeaveCancellations)
                    .HasForeignKey(d => d.SickLeaveId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SickLeaveCancellations_dbo.SickLeaves_SickLeaveId");
            });

            modelBuilder.Entity<SickLeaveComplete>(entity =>
            {
                entity.HasIndex(e => e.ById)
                    .HasName("IX_ById");

                entity.HasIndex(e => e.SickLeaveId)
                    .HasName("IX_SickLeaveId");

                entity.HasOne(d => d.By)
                    .WithMany(p => p.SickLeaveCompletes)
                    .HasForeignKey(d => d.ById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SickLeaveCompletes_dbo.Employee_ById");

                entity.HasOne(d => d.SickLeave)
                    .WithMany(p => p.SickLeaveCompletes)
                    .HasForeignKey(d => d.SickLeaveId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SickLeaveCompletes_dbo.SickLeaves_SickLeaveId");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Team")
                    .IsUnique();

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_Team_Employee");
            });

            modelBuilder.Entity<TeamHistory>(entity =>
            {
                entity.HasIndex(e => new { e.TeamId, e.Modified })
                    .HasName("IX_TeamHistory");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.LeadHistory)
                    .WithMany(p => p.TeamHistories)
                    .HasForeignKey(d => d.LeadHistoryId)
                    .HasConstraintName("FK_TeamHistory_EmployeeHistory");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.TeamHistories)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamHistory_Modified_Person");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamHistories)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamHistory_Team");
            });

            modelBuilder.Entity<Vacation>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.HasIndex(e => e.EmployeeId1)
                    .HasName("IX_Employee_Id");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.VacationEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Vacations_dbo.Employee_EmployeeId");

                entity.HasOne(d => d.EmployeeId1Navigation)
                    .WithMany(p => p.VacationEmployeeId1Navigations)
                    .HasForeignKey(d => d.EmployeeId1)
                    .HasConstraintName("FK_dbo.Vacations_dbo.Employee_Employee_Id");
            });

            modelBuilder.Entity<VacationApproval>(entity =>
            {
                entity.HasIndex(e => e.ApproverId)
                    .HasName("IX_ApproverId");

                entity.HasIndex(e => e.VacationId)
                    .HasName("IX_VacationId");

                entity.HasOne(d => d.Approver)
                    .WithMany(p => p.VacationApprovals)
                    .HasForeignKey(d => d.ApproverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VacationApprovals_dbo.Employee_ApproverId");

                entity.HasOne(d => d.Vacation)
                    .WithMany(p => p.VacationApprovals)
                    .HasForeignKey(d => d.VacationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VacationApprovals_dbo.Vacations_VacationId");
            });

            modelBuilder.Entity<VacationCancellation>(entity =>
            {
                entity.HasIndex(e => e.CancelledById)
                    .HasName("IX_CancelledById");

                entity.HasIndex(e => e.VacationId)
                    .HasName("IX_VacationId");

                entity.HasOne(d => d.CancelledBy)
                    .WithMany(p => p.VacationCancellations)
                    .HasForeignKey(d => d.CancelledById)
                    .HasConstraintName("FK_dbo.VacationCancellations_dbo.Employee_CancelledById");

                entity.HasOne(d => d.Vacation)
                    .WithMany(p => p.VacationCancellations)
                    .HasForeignKey(d => d.VacationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VacationCancellations_dbo.Vacations_VacationId");
            });

            modelBuilder.Entity<VacationProcess>(entity =>
            {
                entity.HasIndex(e => e.ProcessById)
                    .HasName("IX_ProcessById");

                entity.HasIndex(e => e.VacationId)
                    .HasName("IX_VacationId");

                entity.HasOne(d => d.ProcessBy)
                    .WithMany(p => p.VacationProcesses)
                    .HasForeignKey(d => d.ProcessById)
                    .HasConstraintName("FK_dbo.VacationProcesses_dbo.Employee_ProcessById");

                entity.HasOne(d => d.Vacation)
                    .WithMany(p => p.VacationProcesses)
                    .HasForeignKey(d => d.VacationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VacationProcesses_dbo.Vacations_VacationId");
            });

            modelBuilder.Entity<VacationReady>(entity =>
            {
                entity.HasIndex(e => e.ReadyById)
                    .HasName("IX_ReadyById");

                entity.HasIndex(e => e.VacationId)
                    .HasName("IX_VacationId");

                entity.HasOne(d => d.ReadyBy)
                    .WithMany(p => p.VacationReadies)
                    .HasForeignKey(d => d.ReadyById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VacationReadies_dbo.Employee_ReadyById");

                entity.HasOne(d => d.Vacation)
                    .WithMany(p => p.VacationReadies)
                    .HasForeignKey(d => d.VacationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VacationReadies_dbo.Vacations_VacationId");
            });

            modelBuilder.Entity<VacationRemain>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Id");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.VacationRemain)
                    .HasForeignKey<VacationRemain>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VacationRemain_dbo.Employee_Id");
            });

            modelBuilder.Entity<Visa>(entity =>
            {
                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.HasIndex(e => e.ForeignPassportId)
                    .HasName("IX_ForeignPassportId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Visas)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Visa_dbo.Country_CountryId");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Visas)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Visa_dbo.Employee_EmployeeId");

                entity.HasOne(d => d.ForeignPassport)
                    .WithMany(p => p.Visas)
                    .HasForeignKey(d => d.ForeignPassportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Visa_dbo.ForeignPassport_ForeignPassportId");
            });

            modelBuilder.Entity<VisaHistory>(entity =>
            {
                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasIndex(e => e.EmployeeHistoryId)
                    .HasName("IX_EmployeeHistoryId");

                entity.HasIndex(e => e.ForeignPassportHistoryId)
                    .HasName("IX_ForeignPassportHistoryId");

                entity.HasIndex(e => e.ModifiedBy)
                    .HasName("IX_ModifiedBy");

                entity.HasIndex(e => e.OriginId)
                    .HasName("IX_OriginId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.VisaHistories)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.Country_CountryId");

                entity.HasOne(d => d.EmployeeHistory)
                    .WithMany(p => p.VisaHistories)
                    .HasForeignKey(d => d.EmployeeHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.EmployeeHistory_EmployeeHistoryId");

                entity.HasOne(d => d.ForeignPassportHistory)
                    .WithMany(p => p.VisaHistories)
                    .HasForeignKey(d => d.ForeignPassportHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.ForeignPassportHistory_ForeignPassportHistoryId");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.VisaHistories)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.Employee_ModifiedBy");

                entity.HasOne(d => d.Origin)
                    .WithMany(p => p.VisaHistories)
                    .HasForeignKey(d => d.OriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.Visa_OriginId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}