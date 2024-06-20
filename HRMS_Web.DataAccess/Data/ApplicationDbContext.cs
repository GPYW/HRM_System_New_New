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

        //public DbSet<Employee> Employee { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<AttendanceManagement> AttendanceTimeTable { get; set; }

        //public DbSet<Projects> Projects { get; set; }

        //public DbSet<Task> Tasks { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<Task>()
        //        .HasOne(p => p.Project)
        //        .WithMany(p => p.Tasks)
        //        .HasForeignKey(p => p.ProjectID)
               
        //        ;
        //}


    }
}
