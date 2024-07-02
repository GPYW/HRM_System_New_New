using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class LeaveManagementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LeaveManagementController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            //{
            //    new BreadcrumbItem { Title = "Leave Management", Url = Url.Action("Index", "LeaveManagement") },
            //};

            //ViewBag.LeaveTypes = GetLeaveTypes();
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult ViewLeaveHistory()
        {
            List<LeaveManagement> objLeaveManagemetList = _db.LeaveRequestTable.ToList();
            return View(objLeaveManagemetList);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult LeaveRequest()
        {
            return View();
        }

        public IActionResult LeaveForm()
        {
            ViewBag.LeaveTypes = GetLeaveTypes();
            return View();
        }

        [HttpPost]
        public IActionResult LeaveForm(LeaveManagement obj)
        {
            if (ModelState.IsValid)
            {
                _db.LeaveRequestTable.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("ViewLeaveHistory");
            }
            return View(obj);
        }

        private List<string> GetLeaveTypes()
        {
            return new List<string> { "Sick Leave", "Casual Leave", "Maternity Leave", "Paternity Leave" };
        }
    }

    //public class BreadcrumbItem
    //{
    //    public string? Title { get; set; }
    //    public string? Url { get; set; }
    //}
}
