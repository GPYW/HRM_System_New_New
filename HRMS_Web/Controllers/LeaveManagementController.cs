﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Web.Controllers
{
    [Authorize]
    public class LeaveManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LeaveManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var leaveManagements = _db.LeaveManagement
                .Include(lm => lm.RemainingLeaves)
                .ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Leave Management", Url = Url.Action("Index", "LeaveManagement") }
            };

            return View(leaveManagements);
        }

        public IActionResult Reports()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Reports", Url = Url.Action("Reports", "LeaveManagement") }
            };

            return View();
        }

        public async Task<IActionResult> ViewLeaveHistory(int? month, int? year, string leaveType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the current month's leave requests
            var currentMonthLeaveRequests = await _db.LeaveRequests
                .Include(lr => lr.LeaveManagement)
                .Where(lr => lr.Id == userId && lr.StartDate.Month == DateTime.Now.Month && lr.StartDate.Year == DateTime.Now.Year)
                .ToListAsync();

            // Fetch leave history based on search criteria
            var leaveHistoryRequests = _db.LeaveRequests
                .Include(lr => lr.LeaveManagement)
                .Where(lr => lr.Id == userId);

            if (month.HasValue && year.HasValue)
            {
                leaveHistoryRequests = leaveHistoryRequests.Where(lr => lr.StartDate.Month == month.Value && lr.StartDate.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(leaveType))
            {
                leaveHistoryRequests = leaveHistoryRequests.Where(lr => lr.LeaveManagement.LeaveType == leaveType);
            }

            var leaveHistory = await leaveHistoryRequests.ToListAsync();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "View Leave History", Url = Url.Action("ViewLeaveHistory", "LeaveManagement") }
            };

            ViewBag.CurrentMonthLeaveRequests = currentMonthLeaveRequests;
            ViewBag.LeaveTypes = await GetLeaveTypesAsync();

            return View(leaveHistory);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LeaveRequestAsync()
        {
            var pendingRequests = await _db.LeaveRequests
                                  .Include(l => l.LeaveManagement)
                                  .Include(l => l.User) // Include the ApplicationUser
                                  .Where(l => l.Status == "Pending")
                                  .ToListAsync();

            var nonPendingRequests = await _db.LeaveRequests
                                  .Include(l => l.LeaveManagement)
                                  .Include(l => l.User) // Include the ApplicationUser
                                  .Where(l => l.Status != "Pending")
                                  .ToListAsync();

            ViewBag.NonPendingRequests = nonPendingRequests;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Leave Requests", Url = Url.Action("LeaveRequestAsync", "LeaveManagement") }
            };

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

            return RedirectToAction("LeaveRequest");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeclineRequest(int id)
        {
            var leaveRequest = await _db.LeaveRequests
                .Include(lr => lr.LeaveManagement)
                .FirstOrDefaultAsync(lr => lr.RequestId == id);

            if (leaveRequest != null)
            {
                // Find the RemainingLeaves record
                var remainingLeaves = await _db.RemainingLeaves
                    .FirstOrDefaultAsync(rl => rl.Id == leaveRequest.Id && rl.LeaveId == leaveRequest.LeaveId);

                if (remainingLeaves != null)
                {
                    // Restore the NoOfRemainingLeave property
                    if (leaveRequest.LeaveType == "Half Day")
                    {
                        remainingLeaves.NoOfRemainingLeave += 1;
                    }
                    else
                    {
                        remainingLeaves.NoOfRemainingLeave += leaveRequest.NumberOfLeaveDays;
                    }

                    // Save the changes to the RemainingLeaves instance
                    _db.RemainingLeaves.Update(remainingLeaves);
                }

                leaveRequest.Status = "Declined";
                _db.SaveChanges();
            }

            return RedirectToAction("LeaveRequest");
        }

        public IActionResult LeaveForm()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Title = "Leave Form", Url = Url.Action("LeaveForm", "LeaveManagement") }
            };

            ViewBag.LeaveTypes = GetLeaveTypesAsync().Result;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveForm(LeaveRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }

                ViewBag.LeaveTypes = await GetLeaveTypesAsync();
                return View(model);
            }

            model.Status = "Pending";
            model.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var leaveManagement = await _db.LeaveManagement.FirstOrDefaultAsync(lm => lm.LeaveType == model.LeaveType);
            if (leaveManagement != null)
            {
                model.LeaveId = leaveManagement.LeaveId;
            }
            else
            {
                ModelState.AddModelError("LeaveType", "Invalid leave type selected.");
                ViewBag.LeaveTypes = await GetLeaveTypesAsync();
                return View(model);
            }

            // Set the NumberOfLeaveDays property
            model.NumberOfLeaveDays = (model.EndDate - model.StartDate).Days + 1;

            if (model.LeaveType == "Half Day")
            {
                model.StartDate = model.EndDate;
                // Set the NumberOfLeaveDays property
                model.NumberOfLeaveDays = 0;
            }

            // Fetch the RemainingLeaves instance for the user and the selected leave type
            var remainingLeaves = await _db.RemainingLeaves
                .FirstOrDefaultAsync(rl => rl.Id == model.Id && rl.LeaveId == leaveManagement.LeaveId);

            if (remainingLeaves != null)
            {
                if (remainingLeaves.NoOfRemainingLeave == 0)
                {
                    ModelState.AddModelError("LeaveType", "You don't have any leave balance for this leave type.");
                    ViewBag.LeaveTypes = await GetLeaveTypesAsync();
                    return View(model);
                }

                // Check if the user has enough remaining leaves
                if (model.NumberOfLeaveDays > remainingLeaves.NoOfRemainingLeave)
                {
                    ModelState.AddModelError("LeaveType", "You don't have enough leave balance to apply for this leave.");
                    ViewBag.LeaveTypes = await GetLeaveTypesAsync();
                    return View(model);
                }

                if (model.LeaveType == "Half Day")
                {
                    // Update the NoOfRemainingLeave property
                    remainingLeaves.NoOfRemainingLeave = remainingLeaves.NoOfRemainingLeave - 1;
                    // Save the changes to the RemainingLeaves instance
                    _db.RemainingLeaves.Update(remainingLeaves);
                }

                // Update the NoOfRemainingLeave property
                remainingLeaves.NoOfRemainingLeave -= model.NumberOfLeaveDays;

                // Save the changes to the RemainingLeaves instance
                _db.RemainingLeaves.Update(remainingLeaves);
            }
            else
            {
                ModelState.AddModelError("LeaveType", "Remaining leave record not found.");
                ViewBag.LeaveTypes = await GetLeaveTypesAsync();
                return View(model);
            }

            _db.LeaveRequests.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction("ViewLeaveHistory");
        }

        [HttpGet]
        public async Task<IActionResult> GetRemainingLeaves(string leaveType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var leaveManagement = await _db.LeaveManagement.FirstOrDefaultAsync(lm => lm.LeaveType == leaveType);
            if (leaveManagement == null)
            {
                return Json(new { success = false, message = "Invalid leave type selected." });
            }

            var remainingLeaves = await _db.RemainingLeaves
                .FirstOrDefaultAsync(rl => rl.Id == userId && rl.LeaveId == leaveManagement.LeaveId);

            if (remainingLeaves == null)
            {
                return Json(new { success = false, message = "Remaining leave record not found." });
            }
            if (remainingLeaves.NoOfRemainingLeave == 0)
            {
                return Json(new { success = false, message = "You don't have any leave balance for this leave type." });
            }

            return Json(new { success = true, remainingLeaves = remainingLeaves.NoOfRemainingLeave });
        }

        private async Task<List<string>> GetLeaveTypesAsync()
        {
            return await _db.LeaveManagement.Select(lm => lm.LeaveType).Distinct().ToListAsync();
        }

        [HttpPost]
        public IActionResult GetTotalRemainingLeaves()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<object> data = new List<object>();

            // Fetch leave types
            List<string> labels = _db.LeaveManagement.Select(l => l.LeaveType).ToList();

            // Fetch remaining leaves for the logged-in user
            List<int> total = _db.RemainingLeaves
                                 .Where(r => r.Id == userId)
                                 .Select(r => r.NoOfRemainingLeave ?? 0)
                                 .ToList();

            data.Add(labels);
            data.Add(total);

            return Json(data);
        }

        [HttpPost]
        public IActionResult GetTotalAnnualLeaves()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<object> data = new List<object>();

            // Fetch leave types
            List<string> labels = _db.LeaveManagement.Select(l => l.LeaveType).ToList();

            // Fetch annual leaves for the logged-in user
            List<int> total = _db.LeaveManagement.Select(l => l.NoOfLeaves_Year).ToList();

            data.Add(labels);
            data.Add(total);

            return Json(data);
        }

    }
}
