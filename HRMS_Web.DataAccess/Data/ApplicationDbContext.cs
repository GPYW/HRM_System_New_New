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
        public DbSet<AttendanceManagement> AttendanceTimeTable { get; set; }
        public DbSet<LeaveRequestModel> LeaveRequests { get; set; }
        public DbSet<LeaveManagement> LeaveManagement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LeaveRequestModel>()
                .HasOne(lr => lr.LeaveManagement)
                .WithMany(lm => lm.LeaveRequests)
                .HasForeignKey(lr => lr.LeaveId)
                .IsRequired();

            modelBuilder.Entity<LeaveRequestModel>()
               .HasOne(l => l.User)
               .WithMany(u => u.LeaveRequests)
               .HasForeignKey(l => l.Id)
                .IsRequired();
        }
    }
}