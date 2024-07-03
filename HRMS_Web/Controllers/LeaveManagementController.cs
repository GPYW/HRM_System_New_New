﻿using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HRMS_Web.Controllers
{
    [Authorize] // Add general authorization to the controller
    public class LeaveManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LeaveManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public async Task<IActionResult> ViewLeaveHistory()
        {
            var leaveManagementList = await _db.LeaveManagement.ToListAsync();
            return View(leaveManagementList);
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
        public async Task<IActionResult> LeaveForm(LeaveManagement obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.LeaveManagement.Add(obj);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("ViewLeaveHistory");
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            ViewBag.LeaveTypes = GetLeaveTypes();
            return View(obj);
        }

        private List<string> GetLeaveTypes()
        {
            return new List<string> { "Sick Leave", "Casual Leave", "Maternity Leave", "Annual Leave" };
        }
    }
}