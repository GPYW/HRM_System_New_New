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

        //public DbSet<Employee> Employee { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<AttendanceManagement> AttendanceTimeTable { get; set; }


    }
}
