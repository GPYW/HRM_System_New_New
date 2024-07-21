using HRMS_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Web.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<AttendanceManagement> AttendanceTimeTable { get; set; }
        public DbSet<AttendanceManagement> Attendances { get; set; }
        public DbSet<LeaveRequestModel> LeaveRequests { get; set; }
        public DbSet<LeaveManagement> LeaveManagement { get; set; }
        public DbSet<RemainingLeaves> RemainingLeaves { get; set; }
        public DbSet<LeaveManagement> LeaveTypes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Goals> Goals { get; set; }
        public DbSet<Appraisal> Appraisals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser to Department relationship
            builder.Entity<ApplicationUser>()
                .HasOne(c => c.Department)
                .WithMany(u => u.ApplicationUsers)
                .HasForeignKey(c => c.DepartmentID);

            // LeaveRequestModel relationships
            builder.Entity<LeaveRequestModel>()
                .HasOne(lr => lr.LeaveManagement)
                .WithMany(lm => lm.LeaveRequests)
                .HasForeignKey(lr => lr.LeaveId)
                .IsRequired();

            builder.Entity<LeaveRequestModel>()
                .HasOne(l => l.User)
                .WithMany(u => u.LeaveRequests)
                .HasForeignKey(l => l.Id)
                .IsRequired();

            // RemainingLeaves relationships
            builder.Entity<RemainingLeaves>()
                .Property(rl => rl.RemainingLeaveId)
                .ValueGeneratedOnAdd();

            builder.Entity<RemainingLeaves>()
                .HasOne(lr => lr.LeaveManagement)
                .WithMany(lm => lm.RemainingLeaves)
                .HasForeignKey(lr => lr.LeaveId)
                .IsRequired();

            builder.Entity<RemainingLeaves>()
                .HasOne(l => l.User)
                .WithMany(u => u.RemainingLeaves)
                .HasForeignKey(l => l.Id)
                .IsRequired();

            // AttendanceManagement relationship
            builder.Entity<AttendanceManagement>()
                .HasOne(u => u.ApplicationUser)
                .WithMany(a => a.AttendanceTimeTable)
                .HasForeignKey(u => u.Id)
                .IsRequired();

            // Admin relationship
            builder.Entity<Admin>()
                .HasOne(c => c.User)
                .WithMany(u => u.Admins)
                .HasForeignKey(c => c.AdminId);

            // Goals relationship
            builder.Entity<Goals>()
                .HasOne(g => g.User)
                .WithMany(e => e.Goals)
                .HasForeignKey(g => g.Id);

            // Appraisal relationship
            builder.Entity<Appraisal>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appraisals)
                .HasForeignKey(a => a.Id)
                .IsRequired();
        }
    }
}
