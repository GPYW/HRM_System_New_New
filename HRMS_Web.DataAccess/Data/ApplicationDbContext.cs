﻿using HRMS_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
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
        public DbSet<Department> Department { get; set; }

        public DbSet<AttendanceManagement> AttendanceTimeTable { get; set; }
       

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(c => c.Department)
                .WithMany(u => u.ApplicationUsers)
                .HasForeignKey(c => c.DepartmentID);


            builder.Entity<AttendanceManagement>()
               .HasOne(u => u.ApplicationUser)
               .WithMany(a => a.AttendanceTimeTable)
               .HasForeignKey( u=> u.Id)
               .IsRequired();
        }

    
    }
}
