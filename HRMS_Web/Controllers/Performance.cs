using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class Performance : Controller
    {
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Performance", Url = Url.Action("Index", "Performance") },
                new BreadcrumbItem { Title = "Dashboard", Url = Url.Action("Index", "Performance") },
            };
            return View();
        }

        public IActionResult Projects()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Performance", Url = Url.Action("Index", "Performance") },
                new BreadcrumbItem { Title = "Projects", Url = Url.Action("Projects", "Performance") },
            };
            return View();
        }

        //public IActionResult ProjectDetails(int id)
        //{
        //    ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        //    {
        //        new BreadcrumbItem { Title = "Performance", Url = Url.Action("Index", "Performance") },
        //        new BreadcrumbItem { Title = "ProjectDetails", Url = Url.Action("Projects", "Performance") },
        //    };
        //    // Retrieve project details by id from the database or service
        //    var project = _projectService.GetProjectById(id); // Assuming you have a service to get project details
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(project);
        //}

        public IActionResult Tasks()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Performance", Url = Url.Action("Index", "Performance") },
                new BreadcrumbItem { Title = "Tasks", Url = Url.Action("Tasks", "Performance") },
            };
            return View();
        }
        public IActionResult ViewHistory()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Performance", Url = Url.Action("Index", "Performance") },
                new BreadcrumbItem { Title = "ViewHistory", Url = Url.Action("ViewHistory", "Performance") },
            };
            return View();
        }
        //public IActionResult ViewHistory()
        //{
        //    List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable.ToList();
        //    return View(objAttendanceManagemetList);
        //}

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Assign()
        {

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Performance", Url = Url.Action("Index", "Performance") },
                new BreadcrumbItem { Title = "Assign", Url = Url.Action("Assign", "Performance") },
            };
            return View();
        }
        //[HttpPost]
        //public IActionResult AddAttendance(AttendanceManagement obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.AttendanceTimeTable.Add(obj);
        //        _db.SaveChanges();
        //        return RedirectToAction("ViewHistory");
        //    }
        //    return View(obj);
        //}
    }
}



