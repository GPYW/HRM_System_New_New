using HRMS_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(c => c.Department)
                .WithMany(u => u.ApplicationUsers)
                .HasForeignKey(c => c.DepartmentID);
           
        }
    }
}