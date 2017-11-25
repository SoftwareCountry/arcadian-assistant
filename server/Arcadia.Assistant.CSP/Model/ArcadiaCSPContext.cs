using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Arcadia.Assistant.CSP.Model
{
    public partial class ArcadiaCspContext : DbContext
    {
        public ArcadiaCspContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyHistory> CompanyHistory { get; set; }
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
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<TeamHistory> TeamHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.\;Database=ArcadiaCSP;Trusted_Connection=True;");
            }
        }

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
                    .HasColumnType("nchar(6)");

                entity.Property(e => e.ContactName).HasMaxLength(80);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

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
                    .HasColumnType("char(6)");

                entity.Property(e => e.ContactName).HasMaxLength(80);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.HistoryFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

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
                    .HasColumnType("nchar(6)");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

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
                    .HasColumnType("nchar(6)");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.HistoryFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

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
                    .HasColumnType("nchar(6)");

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
                    .HasColumnType("nchar(1)");

                entity.Property(e => e.HiringDate).HasColumnType("date");

                entity.Property(e => e.HomeCity).HasMaxLength(100);

                entity.Property(e => e.HomeCountry).HasMaxLength(80);

                entity.Property(e => e.HomePhone).HasMaxLength(50);

                entity.Property(e => e.HomeStreet).HasMaxLength(255);

                entity.Property(e => e.HomeStreet2).HasMaxLength(255);

                entity.Property(e => e.HomeStreet3).HasMaxLength(255);

                entity.Property(e => e.HomeZip)
                    .HasColumnName("HomeZIP")
                    .HasColumnType("nchar(6)");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

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

                entity.Property(e => e.PartTime).HasComputedColumnSql("([WeekHours]/(40))");

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
                    .HasColumnType("nchar(6)");

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
                    .HasColumnType("nchar(1)");

                entity.Property(e => e.HiringDate).HasColumnType("date");

                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.HistoryFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e.HomeCity).HasMaxLength(100);

                entity.Property(e => e.HomeCountry).HasMaxLength(80);

                entity.Property(e => e.HomePhone).HasMaxLength(50);

                entity.Property(e => e.HomeStreet).HasMaxLength(255);

                entity.Property(e => e.HomeStreet2).HasMaxLength(255);

                entity.Property(e => e.HomeStreet3).HasMaxLength(255);

                entity.Property(e => e.HomeZip)
                    .HasColumnName("HomeZIP")
                    .HasColumnType("nchar(6)");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.LastNameRus)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LoginName).HasMaxLength(40);

                entity.Property(e => e.MiddleName).HasMaxLength(20);

                entity.Property(e => e.MiddleNameRus)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.MobilePhone).HasMaxLength(20);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.PartTime).HasComputedColumnSql("([WeekHours]/(40))");

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

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.Title)
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

                entity.Property(e => e.HistoryFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Title)
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

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Team")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

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

                entity.Property(e => e.HistoryFlag).HasDefaultValueSql("((0))");

                entity.Property(e => e.IntrabaseId).HasColumnType("nchar(12)");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

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
        }
    }
}
