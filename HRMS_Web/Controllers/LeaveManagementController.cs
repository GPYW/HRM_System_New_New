using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
            var leaveHistory = _db.LeaveRequests.Where(l => l.Status == "Approved" || l.Status == "Declined").ToList();
            return View(leaveHistory);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult LeaveRequest()
        {
            var pendingRequests = _db.LeaveRequests.Where(l => l.Status == "Pending").ToList();
            return View(pendingRequests);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ApproveRequest(int id)
        {
            var leaveRequest = _db.LeaveRequests.Find(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = "Approved";
                _db.SaveChanges();
            }
            return RedirectToAction("RequestedLeaves");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeclineRequest(int id)
        {
            var leaveRequest = _db.LeaveRequests.Find(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = "Declined";
                _db.SaveChanges();
            }
            return RedirectToAction("RequestedLeaves");
        }

        public IActionResult LeaveForm()
        {
            ViewBag.LeaveTypes = GetLeaveTypes();
            return View();
        }

        [HttpPost]
        public IActionResult LeaveForm(LeaveRequestModel model)
        {
            if (ModelState.IsValid)
            {
                model.Status = "Pending"; // Set default status to Pending
                _db.LeaveRequests.Add(model);
                _db.SaveChanges();
                return RedirectToAction("ViewLeaveHistory");
            }
            ViewBag.LeaveTypes = GetLeaveTypes();
            return View(model);
        }


        public JsonResult GetRemainingLeaves(string leaveType)
        {
            int remaining = 0;

            switch (leaveType)
            {
                case "Sick Leave":
                    remaining = 7;
                    break;
                case "Casual Leave":
                    remaining = 7;
                    break;
                case "Maternity Leave":
                    // Example: fetch from DB or set a default value
                    remaining = 0; // Assuming it's dynamically calculated
                    break;
                case "Annual Leave":
                    remaining = 12;
                    break;
            }

            return Json(remaining);
        }

        private List<string> GetLeaveTypes()
        {
            return new List<string> { "Sick Leave", "Casual Leave", "Maternity Leave", "Annual Leave" };
        }
    }
 
}
