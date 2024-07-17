using HRMS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS_Web.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HRMS_Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ConfigurationController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Configuration", Url = Url.Action("Index", "Configuration") },
            };

            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveManagement.ToListAsync(),
                NewLeaveType = new LeaveManagement(),
                Roles = await _roleManager.Roles.ToListAsync(),
                NewRole = new IdentityRole(),
                Departments = await _context.Departments.ToListAsync(),
                NewDepartment = new Department()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLeaveType([Bind("LeaveId,LeaveType,NoOfLeaves_Year")] LeaveManagement leaveManagement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveManagement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Configuration", Url = Url.Action("Index", "Configuration") },
            };

            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveManagement.ToListAsync(),
                NewLeaveType = leaveManagement,
                Roles = await _roleManager.Roles.ToListAsync(),
                NewRole = new IdentityRole(),
                Departments = await _context.Departments.ToListAsync(),
                NewDepartment = new Department()
            };
            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole([Bind("Id,Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                await _roleManager.CreateAsync(role);
                return RedirectToAction(nameof(Index));
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Configuration", Url = Url.Action("Index", "Configuration") },
            };

            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveManagement.ToListAsync(),
                NewLeaveType = new LeaveManagement(),
                Roles = await _roleManager.Roles.ToListAsync(),
                NewRole = role,
                Departments = await _context.Departments.ToListAsync(),
                NewDepartment = new Department()
            };
            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAdminDetails(Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Admins.Add(admin);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, re-render the view with appropriate data
            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = _context.LeaveManagement.ToList(),
                NewLeaveType = new LeaveManagement(),
                Roles = _roleManager.Roles.ToList(),
                NewRole = new IdentityRole(),
                Departments = _context.Departments.ToList(),
                NewDepartment = new Department(),
                Admins = _context.Admins.ToList()  // Retrieve admins again to ensure they are passed to the view
            };

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
    {
        new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
        new BreadcrumbItem { Title = "Configuration", Url = Url.Action("Index", "Configuration") },
    };

            return View("Index", viewModel);
        }

    }
}



