using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using HRMS_Web.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var totalEmployees = await _context.ApplicationUser.CountAsync();
            var todayAttendanceCount = await _context.AttendanceTimeTable.CountAsync(a => a.Date == today && a.IsPresent);
            var totalAttendanceCount = await _context.AttendanceTimeTable.CountAsync(a => a.IsPresent);
            var totalAttendancePercentage = totalAttendanceCount * 100 / totalEmployees;
            var totalLeavesTaken = await _context.LeaveRequests.CountAsync();
            var pendingLeaveRequests = await _context.LeaveRequests.CountAsync(lr => lr.Status == "Pending");
            var totalDepartments = await _context.Departments.CountAsync();

            ViewData["TotalEmployees"] = totalEmployees;
            ViewData["TodayAttendanceCount"] = todayAttendanceCount;
            ViewData["TotalAttendancePercentage"] = totalAttendancePercentage;
            ViewData["TotalLeavesTaken"] = totalLeavesTaken;
            ViewData["PendingLeaveRequests"] = pendingLeaveRequests;
            ViewData["TotalDepartments"] = totalDepartments;

            return View();
        }
    }
}
