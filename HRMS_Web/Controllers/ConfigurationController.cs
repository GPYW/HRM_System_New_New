using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

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
            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveTypes.ToListAsync(),
                NewLeaveType = new LeaveType(),
                Roles = await _roleManager.Roles.ToListAsync(),
                NewRole = new IdentityRole(),
                Departments = await _context.Departments.ToListAsync(),
                NewDepartment = new Department()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLeaveType([Bind("Id,Name")] LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveTypes.ToListAsync(),
                NewLeaveType = leaveType,
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
            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveTypes.ToListAsync(),
                NewLeaveType = new LeaveType(),
                Roles = await _roleManager.Roles.ToListAsync(),
                NewRole = role,
                Departments = await _context.Departments.ToListAsync(),
                NewDepartment = new Department()
            };
            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDepartment([Bind("DepartmentID,DepartmentName,NoOfEmployees")] Department department)
        {

                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveTypes.ToListAsync(),
                NewLeaveType = new LeaveType(),
                Roles = await _roleManager.Roles.ToListAsync(),
                NewRole = new IdentityRole(),
                Departments = await _context.Departments.ToListAsync(),
                NewDepartment = department
            };
            return View("Index", viewModel);
        }
    }
}
