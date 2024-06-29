using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class AttendanceManagementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AttendanceManagementController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
            new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "AttendanceManagement") },

        };
            return View();
        }
        public IActionResult Reports()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
                new BreadcrumbItem { Title = "Reports", Url = Url.Action("Reports", "AttendanceManagement") },
            };
            return View();
        }
        public IActionResult MarkAttendance()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
            new BreadcrumbItem { Title = "Mark Attendance", Url = Url.Action("MarkAttendance", "AttendanceManagement") },

        };
            return View();  
        }
        public IActionResult ViewHistory()
        {
            List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable.ToList();
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
            new BreadcrumbItem { Title = "View History", Url = Url.Action("ViewHistory", "AttendanceManagement") },

        };
            return View(objAttendanceManagemetList);
        }
        public IActionResult AddAttendance()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
            new BreadcrumbItem { Title = "Mark Attendance", Url = Url.Action("MarkAttendance", "AttendanceManagement") },
            new BreadcrumbItem { Title = "Add Attendance", Url = Url.Action("AddAttendance", "AttendanceManagement") },


        };
            return View();
        }
        [HttpPost]
        public IActionResult AddAttendance(AttendanceManagement obj)
        {
            if (ModelState.IsValid)
            {
                _db.AttendanceTimeTable.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("ViewHistory");
            }
            return View(obj);
        }
    }
}
