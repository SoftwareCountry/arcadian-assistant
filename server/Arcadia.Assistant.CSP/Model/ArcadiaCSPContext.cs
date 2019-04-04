using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyHistory> CompanyHistory { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Cspalert> Cspalert { get; set; }
        public virtual DbSet<CspalertType> CspalertType { get; set; }
        public virtual DbSet<Csproles> Csproles { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DepartmentHistory> DepartmentHistory { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeCspalert> EmployeeCspalert { get; set; }
        public virtual DbSet<EmployeeHistory> EmployeeHistory { get; set; }
        public virtual DbSet<EmployeePosition> EmployeePosition { get; set; }
        public virtual DbSet<EmployeePositionHistory> EmployeePositionHistory { get; set; }
        public virtual DbSet<EmployeeRoles> EmployeeRoles { get; set; }
        public virtual DbSet<EmployeeTeam> EmployeeTeam { get; set; }
        public virtual DbSet<EmployeeTeamHistory> EmployeeTeamHistory { get; set; }
        public virtual DbSet<ForeignPassport> ForeignPassport { get; set; }
        public virtual DbSet<ForeignPassportHistory> ForeignPassportHistory { get; set; }
        public virtual DbSet<Holidays> Holidays { get; set; }
        public virtual DbSet<MigrationHistory> MigrationHistory { get; set; }
        public virtual DbSet<NetwrixAuditErrors> NetwrixAuditErrors { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<SickLeaves> SickLeaves { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<TeamHistory> TeamHistory { get; set; }
        public virtual DbSet<VacationApprovals> VacationApprovals { get; set; }
        public virtual DbSet<VacationCancellations> VacationCancellations { get; set; }
        public virtual DbSet<VacationProcesses> VacationProcesses { get; set; }
        public virtual DbSet<VacationReadies> VacationReadies { get; set; }
        public virtual DbSet<Vacations> Vacations { get; set; }
        public virtual DbSet<Visa> Visa { get; set; }
        public virtual DbSet<VisaHistory> VisaHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Company")
                    .IsUnique();

                entity.Property(e => e.BusinessCity).HasMaxLength(100);

                entity.Property(e => e.BusinessCountry).HasMaxLength(80);

                entity.Property(e => e.BusinessFax).HasMaxLength(40);

                entity.Property(e => e.BusinessPhone).HasMaxLength(80);

                entity.Property(e => e.BusinessPhone2).HasMaxLength(80);

                entity.Property(e => e.BusinessStreet).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet2).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet3).HasMaxLength(255);

                entity.Property(e => e.BusinessZip)
                    .HasColumnName("BusinessZIP")
                    .HasMaxLength(6);

                entity.Property(e => e.ContactName).HasMaxLength(80);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Name).HasMaxLength(120);

                entity.Property(e => e.PostAddress).HasMaxLength(255);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Web).HasMaxLength(255);
            });

            modelBuilder.Entity<CompanyHistory>(entity =>
            {
                entity.HasIndex(e => new { e.CompanyId, e.Modified })
                    .HasName("IX_CompanyHistory")
                    .IsUnique();

                entity.Property(e => e.BusinessCity)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessCountry)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessFax).HasMaxLength(40);

                entity.Property(e => e.BusinessPhone).HasMaxLength(80);

                entity.Property(e => e.BusinessPhone2).HasMaxLength(80);

                entity.Property(e => e.BusinessStreet).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet2).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet3).HasMaxLength(255);

                entity.Property(e => e.BusinessZip)
                    .HasColumnName("BusinessZIP")
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.ContactName).HasMaxLength(80);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(120);

                entity.Property(e => e.PostAddress).HasMaxLength(255);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Web).HasMaxLength(255);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyHistory)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyHistory_Company");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.CompanyHistory)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyHistory_Modified_Employee");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Code2)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.Code3)
                    .IsRequired()
                    .HasMaxLength(3);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameRus)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Cspalert>(entity =>
            {
                entity.ToTable("CSPAlert");

                entity.Property(e => e.AlertDate).HasColumnType("datetime");

                entity.Property(e => e.AlertText).IsRequired();

                entity.HasOne(d => d.AlertType)
                    .WithMany(p => p.Cspalert)
                    .HasForeignKey(d => d.AlertTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CSPAlert_CSPAlertType");
            });

            modelBuilder.Entity<CspalertType>(entity =>
            {
                entity.ToTable("CSPAlertType");

                entity.Property(e => e.AlertType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.DisplayName).HasMaxLength(50);
            });

            modelBuilder.Entity<Csproles>(entity =>
            {
                entity.ToTable("CSPRoles");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Department")
                    .IsUnique();

                entity.Property(e => e.Abbreviation).HasMaxLength(40);

                entity.Property(e => e.BusinessCity).HasMaxLength(100);

                entity.Property(e => e.BusinessCountry).HasMaxLength(80);

                entity.Property(e => e.BusinessFax).HasMaxLength(40);

                entity.Property(e => e.BusinessPhone).HasMaxLength(80);

                entity.Property(e => e.BusinessStreet).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet2).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet3).HasMaxLength(255);

                entity.Property(e => e.BusinessZip)
                    .HasColumnName("BusinessZIP")
                    .HasMaxLength(6);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Chief)
                    .WithMany(p => p.Department)
                    .HasForeignKey(d => d.ChiefId)
                    .HasConstraintName("FK_Department_Employee");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Department)
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

                entity.Property(e => e.Abbreviation).HasMaxLength(40);

                entity.Property(e => e.BusinessCity).HasMaxLength(100);

                entity.Property(e => e.BusinessCountry).HasMaxLength(80);

                entity.Property(e => e.BusinessFax).HasMaxLength(40);

                entity.Property(e => e.BusinessPhone).HasMaxLength(80);

                entity.Property(e => e.BusinessStreet).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet2).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet3).HasMaxLength(255);

                entity.Property(e => e.BusinessZip)
                    .HasColumnName("BusinessZIP")
                    .HasMaxLength(6);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Chief)
                    .WithMany(p => p.DepartmentHistory)
                    .HasForeignKey(d => d.ChiefId)
                    .HasConstraintName("FK_DepartmentHistory_EmployeeHistory");

                entity.HasOne(d => d.CompanyHistory)
                    .WithMany(p => p.DepartmentHistory)
                    .HasForeignKey(d => d.CompanyHistoryId)
                    .HasConstraintName("FK_DepartmentHistory_CompanyHistory");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.DepartmentHistory)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DepartmentHistory_Department");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.DepartmentHistory)
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

                entity.HasIndex(e => e.Id)
                    .HasName("IX_Employee")
                    .IsUnique();

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.BusinessCity).HasMaxLength(100);

                entity.Property(e => e.BusinessCountry).HasMaxLength(80);

                entity.Property(e => e.BusinessPhone).HasMaxLength(20);

                entity.Property(e => e.BusinessPhone2).HasMaxLength(20);

                entity.Property(e => e.BusinessStreet).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet2).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet3).HasMaxLength(255);

                entity.Property(e => e.BusinessZip)
                    .HasColumnName("BusinessZIP")
                    .HasMaxLength(6);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.FiringDate).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.FirstNameRus)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.HiringDate).HasColumnType("date");

                entity.Property(e => e.HomeCity).HasMaxLength(100);

                entity.Property(e => e.HomeCountry).HasMaxLength(80);

                entity.Property(e => e.HomePhone).HasMaxLength(50);

                entity.Property(e => e.HomeStreet).HasMaxLength(255);

                entity.Property(e => e.HomeStreet2).HasMaxLength(255);

                entity.Property(e => e.HomeStreet3).HasMaxLength(255);

                entity.Property(e => e.HomeZip)
                    .HasColumnName("HomeZIP")
                    .HasMaxLength(6);

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LastNameRus)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LoginName).HasMaxLength(40);

                entity.Property(e => e.MiddleName).HasMaxLength(20);

                entity.Property(e => e.MiddleNameRus).HasMaxLength(20);

                entity.Property(e => e.MobilePhone).HasMaxLength(20);

                entity.Property(e => e.ProbationEnd).HasColumnType("date");

                entity.Property(e => e.RoomNumber).HasMaxLength(20);

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Employee_Company");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Employee_Department");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_Employee_Employeeosition");
            });

            modelBuilder.Entity<EmployeeCspalert>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.CspalertId });

                entity.ToTable("EmployeeCSPAlert");

                entity.Property(e => e.CspalertId).HasColumnName("CSPAlertId");

                entity.HasOne(d => d.Cspalert)
                    .WithMany(p => p.EmployeeCspalert)
                    .HasForeignKey(d => d.CspalertId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeCSPAlert_CSPAlertType");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeCspalert)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeCSPAlert_Employee");
            });

            modelBuilder.Entity<EmployeeHistory>(entity =>
            {
                entity.HasIndex(e => new { e.EmployeeId, e.Modified })
                    .HasName("IX_EmployeeHistory")
                    .IsUnique();

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.BusinessCity).HasMaxLength(100);

                entity.Property(e => e.BusinessCountry).HasMaxLength(80);

                entity.Property(e => e.BusinessPhone).HasMaxLength(20);

                entity.Property(e => e.BusinessPhone2).HasMaxLength(20);

                entity.Property(e => e.BusinessStreet).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet2).HasMaxLength(255);

                entity.Property(e => e.BusinessStreet3).HasMaxLength(255);

                entity.Property(e => e.BusinessZip)
                    .HasColumnName("BusinessZIP")
                    .HasMaxLength(6);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.FiringDate).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.FirstNameRus)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.HiringDate).HasColumnType("date");

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.HomeCity).HasMaxLength(100);

                entity.Property(e => e.HomeCountry).HasMaxLength(80);

                entity.Property(e => e.HomePhone).HasMaxLength(50);

                entity.Property(e => e.HomeStreet).HasMaxLength(255);

                entity.Property(e => e.HomeStreet2).HasMaxLength(255);

                entity.Property(e => e.HomeStreet3).HasMaxLength(255);

                entity.Property(e => e.HomeZip)
                    .HasColumnName("HomeZIP")
                    .HasMaxLength(6);

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.LastNameRus)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LoginName).HasMaxLength(40);

                entity.Property(e => e.MiddleName).HasMaxLength(20);

                entity.Property(e => e.MiddleNameRus).HasMaxLength(20);

                entity.Property(e => e.MobilePhone).HasMaxLength(20);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.ProbationEnd).HasColumnType("date");

                entity.Property(e => e.RoomNumber).HasMaxLength(20);

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeHistory)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_EmployeeHistory_CompanyHistory");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeHistory)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_EmployeeHistory_DepartmentHistory");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeHistoryEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeHistory_Employee");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.EmployeeHistoryModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeHistory_Modified_Employee");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.EmployeeHistory)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_EmployeeHistory_PositionHistory");
            });

            modelBuilder.Entity<EmployeePosition>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_EmployeePosition")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TitleRus)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TitleShort)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EmployeePositionHistory>(entity =>
            {
                entity.HasIndex(e => new { e.EmployeePositionId, e.Modified })
                    .HasName("IX_EmployeePositionHistory")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TitleRus)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TitleShort)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.EmployeePosition)
                    .WithMany(p => p.EmployeePositionHistory)
                    .HasForeignKey(d => d.EmployeePositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeePositionHistory_EmployeePosition");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.EmployeePositionHistory)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeePositionHistory_Employee");
            });

            modelBuilder.Entity<EmployeeRoles>(entity =>
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
                    .WithMany(p => p.EmployeeTeam)
                    .HasForeignKey(d => d.EmpolyeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeam_Employee");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.EmployeeTeam)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeam_Team");
            });

            modelBuilder.Entity<EmployeeTeamHistory>(entity =>
            {
                entity.HasKey(e => new { e.EmpolyeeHistoryId, e.TeamHistoryId });

                entity.HasOne(d => d.EmpolyeeHistory)
                    .WithMany(p => p.EmployeeTeamHistory)
                    .HasForeignKey(d => d.EmpolyeeHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeamHistory_EmployeeHistory");

                entity.HasOne(d => d.TeamHistory)
                    .WithMany(p => p.EmployeeTeamHistory)
                    .HasForeignKey(d => d.TeamHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTeamHistory_TeamHistory");
            });

            modelBuilder.Entity<ForeignPassport>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.Property(e => e.DateOfExpiry).HasColumnType("datetime");

                entity.Property(e => e.DateOfIssue).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.IssuedBy)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.PassportNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ForeignPassport)
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

                entity.Property(e => e.DateOfExpiry).HasColumnType("datetime");

                entity.Property(e => e.DateOfIssue).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.IssuedBy)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.PassportNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.EmployeeHistory)
                    .WithMany(p => p.ForeignPassportHistory)
                    .HasForeignKey(d => d.EmployeeHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ForeignPassportHistory_dbo.EmployeeHistory_EmployeeHistoryId");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ForeignPassportHistory)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ForeignPassportHistory_dbo.Employee_ModifiedBy");

                entity.HasOne(d => d.Origin)
                    .WithMany(p => p.ForeignPassportHistory)
                    .HasForeignKey(d => d.OriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ForeignPassportHistory_dbo.ForeignPassport_OriginId");
            });

            modelBuilder.Entity<Holidays>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<MigrationHistory>(entity =>
            {
                entity.HasKey(e => new { e.MigrationId, e.ContextKey });

                entity.ToTable("__MigrationHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ContextKey).HasMaxLength(300);

                entity.Property(e => e.Model).IsRequired();

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<NetwrixAuditErrors>(entity =>
            {
                entity.HasKey(e => e.ErrorId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("Netwrix_audit_errors");

                entity.HasIndex(e => e.ErrorTime)
                    .HasName("NetwrixErrorTimeClustered")
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => e.MessageId)
                    .HasName("NetwrixMessageIDNonClustered");

                entity.Property(e => e.ErrorId)
                    .HasColumnName("ErrorID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Application).IsRequired();

                entity.Property(e => e.DataBaseName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.ErrorTime).HasColumnType("datetime");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Workstation).IsRequired();
            });

            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.RoomNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<SickLeaves>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.SickLeaves)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SickLeaves_dbo.Employee_EmployeeId");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Team")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.Lead)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.LeadId)
                    .HasConstraintName("FK_Team_Employee");
            });

            modelBuilder.Entity<TeamHistory>(entity =>
            {
                entity.HasIndex(e => new { e.TeamId, e.Modified })
                    .HasName("IX_TeamHistory");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.IntrabaseId).HasMaxLength(12);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.LeadHistory)
                    .WithMany(p => p.TeamHistory)
                    .HasForeignKey(d => d.LeadHistoryId)
                    .HasConstraintName("FK_TeamHistory_EmployeeHistory");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.TeamHistory)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamHistory_Modified_Person");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamHistory)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamHistory_Team");
            });

            modelBuilder.Entity<VacationApprovals>(entity =>
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

            modelBuilder.Entity<VacationCancellations>(entity =>
            {
                entity.HasIndex(e => e.CancelledById)
                    .HasName("IX_CancelledById");

                entity.HasIndex(e => e.VacationId)
                    .HasName("IX_VacationId");

                entity.Property(e => e.Reason).HasMaxLength(1000);

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

            modelBuilder.Entity<VacationProcesses>(entity =>
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

            modelBuilder.Entity<VacationReadies>(entity =>
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

            modelBuilder.Entity<Vacations>(entity =>
            {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.HasIndex(e => e.EmployeeId1)
                    .HasName("IX_Employee_Id");

                entity.Property(e => e.EmployeeId1).HasColumnName("Employee_Id");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.VacationsEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Vacations_dbo.Employee_EmployeeId");

                entity.HasOne(d => d.EmployeeId1Navigation)
                    .WithMany(p => p.VacationsEmployeeId1Navigation)
                    .HasForeignKey(d => d.EmployeeId1)
                    .HasConstraintName("FK_dbo.Vacations_dbo.Employee_Employee_Id");
            });

            modelBuilder.Entity<Visa>(entity =>
            {
                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IX_EmployeeId");

                entity.HasIndex(e => e.ForeignPassportId)
                    .HasName("IX_ForeignPassportId");

                entity.Property(e => e.DateOfExpiry).HasColumnType("datetime");

                entity.Property(e => e.DateOfIssue).HasColumnType("datetime");

                entity.Property(e => e.Days).HasMaxLength(50);

                entity.Property(e => e.InsuranceEnd).HasColumnType("datetime");

                entity.Property(e => e.InsuranceStart).HasColumnType("datetime");

                entity.Property(e => e.IssuedBy).HasMaxLength(200);

                entity.Property(e => e.MultiTime).HasMaxLength(50);

                entity.Property(e => e.VisaNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Visa)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Visa_dbo.Country_CountryId");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Visa)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Visa_dbo.Employee_EmployeeId");

                entity.HasOne(d => d.ForeignPassport)
                    .WithMany(p => p.Visa)
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

                entity.Property(e => e.DateOfExpiry).HasColumnType("datetime");

                entity.Property(e => e.DateOfIssue).HasColumnType("datetime");

                entity.Property(e => e.Days).HasMaxLength(50);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.InsuranceEnd).HasColumnType("datetime");

                entity.Property(e => e.InsuranceStart).HasColumnType("datetime");

                entity.Property(e => e.IssuedBy).HasMaxLength(200);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.MultiTime).HasMaxLength(50);

                entity.Property(e => e.VisaNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.VisaHistory)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.Country_CountryId");

                entity.HasOne(d => d.EmployeeHistory)
                    .WithMany(p => p.VisaHistory)
                    .HasForeignKey(d => d.EmployeeHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.EmployeeHistory_EmployeeHistoryId");

                entity.HasOne(d => d.ForeignPassportHistory)
                    .WithMany(p => p.VisaHistory)
                    .HasForeignKey(d => d.ForeignPassportHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.ForeignPassportHistory_ForeignPassportHistoryId");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.VisaHistory)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.Employee_ModifiedBy");

                entity.HasOne(d => d.Origin)
                    .WithMany(p => p.VisaHistory)
                    .HasForeignKey(d => d.OriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.VisaHistory_dbo.Visa_OriginId");
            });
        }
    }
}
