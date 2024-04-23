using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class AttendanceManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Reports()
        {
            return View();
        }
        public IActionResult MarkAttendance()
        {
            return View();
        }
        public IActionResult ViewHistory()
        {
            return View();
        }
    }
}
