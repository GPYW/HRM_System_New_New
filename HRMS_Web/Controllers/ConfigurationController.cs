using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using System.Threading.Tasks;

namespace HRMS_Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConfigurationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ConfigurationViewModel
            {
                LeaveTypes = await _context.LeaveTypes.ToListAsync(),
                NewLeaveType = new LeaveType()
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
                NewLeaveType = leaveType
            };
            return View("Index", viewModel);
        }
    }
}
