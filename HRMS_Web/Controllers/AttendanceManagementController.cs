using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

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
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            if (applicationUser == null)
            {
                return NotFound("User not found");
            }

            var companyId = applicationUser.CompanyID;

            List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable
                .Where(a => a.EmpID == companyId)
                .OrderByDescending(a => a.Date)
                .ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
                new BreadcrumbItem { Title = "View History", Url = Url.Action("ViewHistory", "AttendanceManagement") },
            };

            return View(objAttendanceManagemetList);
        }

        [HttpPost]
        public IActionResult SearchAttendance(DateTime FromDate, DateTime ToDate)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            if (applicationUser == null)
            {
                return NotFound("User not found");
            }

            var companyId = applicationUser.CompanyID;

            List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable
                .Where(a => a.EmpID == companyId && a.Date >= FromDate && a.Date <= ToDate)
                .OrderByDescending(a => a.Date)
                .ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
                new BreadcrumbItem { Title = "View History", Url = Url.Action("ViewHistory", "AttendanceManagement") },
            };

            return View("ViewHistory", objAttendanceManagemetList);
        }

        [HttpPost]
        public IActionResult ShowAllAttendance()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            if (applicationUser == null)
            {
                return NotFound("User not found");
            }

            var companyId = applicationUser.CompanyID;

            List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable
                .Where(a => a.EmpID == companyId)
                .OrderByDescending(a => a.Date)
                .ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
                new BreadcrumbItem { Title = "View History", Url = Url.Action("ViewHistory", "AttendanceManagement") },
            };

            return View("ViewHistory", objAttendanceManagemetList);
        }

        [HttpPost]
        public IActionResult SearchFromAll(string EmployeeId, DateTime? FromDate, DateTime? ToDate)
        {
            var query = _db.Attendances.AsQueryable();

            if (!string.IsNullOrEmpty(EmployeeId))
            {
                query = query.Where(a => a.EmpID == EmployeeId);
            }

            if (FromDate.HasValue && ToDate.HasValue)
            {
                query = query.Where(a => a.Date >= FromDate && a.Date <= ToDate);
            }
            else if (FromDate.HasValue)
            {
                query = query.Where(a => a.Date >= FromDate);
            }
            else if (ToDate.HasValue)
            {
                query = query.Where(a => a.Date <= ToDate);
            }

            var results = query.ToList();

            return View("MarkAttendance", results);
        }

        [HttpPost]
        public IActionResult SubmitAttendance(string EmployeeId, DateTime? FromDate, DateTime? ToDate)
        {
            var attendanceQuery = _db.AttendanceTimeTable.AsQueryable();

            if (!string.IsNullOrEmpty(EmployeeId))
            {
                attendanceQuery = attendanceQuery.Where(a => a.EmpID == EmployeeId);
            }

            if (FromDate.HasValue && ToDate.HasValue)
            {
                attendanceQuery = attendanceQuery.Where(a => a.Date >= FromDate.Value && a.Date <= ToDate.Value);
            }

            var attendanceRecords = attendanceQuery.OrderByDescending(a => a.Date).ToList();

            return View("AttendanceRecords", attendanceRecords);
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
            _db.AttendanceTimeTable.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("MarkAttendance");
        }
    }
}
