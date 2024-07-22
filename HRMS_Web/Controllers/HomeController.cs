using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HRMS_Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var totalEmployees = _context.Users.Count();
            var totalAttendance = _context.Attendances.Count(a => a.IsPresent);
            var totalLeaves = _context.LeaveRequests.Count();
            var totalGoals = _context.Goals.Count(g => g.Status == GoalStatus.Completed);
            var totalDepartments = _context.Departments.Count();
            var totalRoles = _context.Roles.Count();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Dashboard", Url = Url.Action("Index", "Home") },
            };

            ViewData["TotalEmployees"] = totalEmployees;
            ViewData["TotalAttendance"] = totalAttendance;
            ViewData["TotalLeaves"] = totalLeaves;
            ViewData["TotalGoals"] = totalGoals;
            ViewData["TotalDepartments"] = totalDepartments;
            ViewData["TotalRoles"] = totalRoles;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
