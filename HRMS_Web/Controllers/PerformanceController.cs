using Microsoft.AspNetCore.Mvc;
using HRMS_Web.Services;

namespace HRMS_Web.Controllers
{
    public class PerformanceController : Controller
    {
        private readonly IPerformanceService _performanceService;

        public PerformanceController(IPerformanceService performanceService)
        {
            _performanceService = performanceService;
        }

        public IActionResult Dashboard()
        {
            var model = _performanceService.GetDashboardData(User.Identity.Name);
            return View(model);
        }
    }


    //private string GetBreadcrumbTitle(string action)
    //{
    //    return action switch
    //    {
    //        "Dashboard" => "Home",
    //        //"Reports" => "Reports",
    //        //"MarkAttendance" => "Mark Attendance",
    //        //"ViewHistory" => "View History",
    //        //"AddAttendance" => "Add Attendance",
    //        _ => action,
    //    };
    //}
}
