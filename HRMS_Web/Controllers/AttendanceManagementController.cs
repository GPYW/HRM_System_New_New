using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class AttendanceManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
