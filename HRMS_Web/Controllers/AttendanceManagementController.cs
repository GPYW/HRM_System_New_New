using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;
using System.Diagnostics.Metrics;

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
            ViewData["Breadcrumb"] = GetDefaultBreadcrumb("Attendance Management", "Index");
            return View();
        }

        public IActionResult Reports()
        {
            ViewData["Breadcrumb"] = GetDefaultBreadcrumb("Attendance Management", "Reports");
            return View();
        }

        public IActionResult MarkAttendance()
        {
            ViewData["Breadcrumb"] = GetDefaultBreadcrumb("Attendance Management", "MarkAttendance");
            return View();
        }

        public IActionResult ViewHistory()
        {
            var objAttendanceManagemetList = GetAttendanceList();

            ViewData["Breadcrumb"] = GetDefaultBreadcrumb("Attendance Management", "ViewHistory");
            return View("ViewHistory", objAttendanceManagemetList);
        }

        [HttpPost]
        public IActionResult SearchAttendance(DateTime FromDate, DateTime ToDate)
        {
            var objAttendanceManagemetList = GetAttendanceList(FromDate, ToDate);

            ViewData["Breadcrumb"] = GetDefaultBreadcrumb("Attendance Management", "ViewHistory");
            return View("ViewHistory", objAttendanceManagemetList);
        }

        [HttpPost]
        public IActionResult ShowAllAttendance()
        {
            var objAttendanceManagemetList = GetAttendanceList();

            ViewData["Breadcrumb"] = GetDefaultBreadcrumb("Attendance Management", "ViewHistory");
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
            ViewData["Breadcrumb"] = GetDefaultBreadcrumb("Attendance Management", "MarkAttendance", "AddAttendance");
            return View();
        }

        [HttpPost]
        public IActionResult AddAttendance(AttendanceManagement obj)
        {
            _db.AttendanceTimeTable.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("MarkAttendance");
        }

        [HttpPost]
        public IActionResult PDF(int month, int year)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            if (applicationUser == null)
            {
                return NotFound("User not found");
            }

            var employeeId = applicationUser.CompanyID;
            var employeeName = applicationUser.FirstName + " " + applicationUser.LastName;

            List<AttendanceManagement> attendanceList = _db.AttendanceTimeTable
                .Where(a => a.EmpID == employeeId && a.Date.Month == month && a.Date.Year == year)
                .OrderByDescending(a => a.Date)
                .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                using (iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 25, 25, 30, 30))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    var image = iTextSharp.text.Image.GetInstance("wwwroot/images/LetterHead.png");
                    float pageWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                    image.ScaleToFit(pageWidth, image.Height * (pageWidth / image.Width));
                    image.Alignment = Element.ALIGN_CENTER;
                    document.Add(image);

                    var footer = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                    var subTitleFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    var regularFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                    document.Add(new Paragraph($"Attendance Report - {year} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}", subTitleFont));
                    document.Add(new Paragraph(" "));
                    document.Add(new Paragraph($"Employee ID: {employeeId}", regularFont));
                    document.Add(new Paragraph($"Employee Name: {employeeName}", regularFont));
                    document.Add(new Paragraph($"Month : {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}", regularFont));
                    document.Add(new Paragraph(" "));

                    PdfPTable table = new PdfPTable(6);
                    table.WidthPercentage = 100;
                    table.DefaultCell.Border = PdfPCell.NO_BORDER;

                    void AddHeaderCell(PdfPTable t, string text)
                    {
                        var font = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.WHITE);
                        PdfPCell cell = new PdfPCell(new Phrase(text, font))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BackgroundColor = new BaseColor(25, 161, 184),
                            Border = PdfPCell.NO_BORDER
                        };
                        t.AddCell(cell);
                    }

                    void AddBodyCell(PdfPTable t, string text, bool isOdd)
                    {
                        var font = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        PdfPCell cell = new PdfPCell(new Phrase(text, font))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = PdfPCell.NO_BORDER
                        };
                        if (isOdd)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = new BaseColor(185, 212, 217);
                        }
                        t.AddCell(cell);
                    }

                    AddHeaderCell(table, "No");
                    AddHeaderCell(table, "Date");
                    AddHeaderCell(table, "Check-In");
                    AddHeaderCell(table, "Check-Out");
                    AddHeaderCell(table, "Overtime");
                    AddHeaderCell(table, "Break");

                    bool isOddRow = true;
                    int counter = 1;
                    foreach (var attendance in attendanceList)
                    {
                        AddBodyCell(table, counter.ToString(), isOddRow);
                        AddBodyCell(table, attendance.Date.ToString("yyyy-MM-dd"), isOddRow);
                        AddBodyCell(table, attendance.CheckIn.ToString(@"hh\:mm"), isOddRow);
                        AddBodyCell(table, attendance.CheckOut.ToString(@"hh\:mm"), isOddRow);
                        AddBodyCell(table, attendance.OverTime, isOddRow);
                        AddBodyCell(table, attendance.Break, isOddRow);

                        isOddRow = !isOddRow;
                        counter++;
                    }

                    document.Add(table);
                    document.Add(new Paragraph(" "));

                    document.Add(new Paragraph("----End of the document----", footer));

                    document.Add(new Paragraph("*This document is system-generated and does not require a signature.", footer));
                    document.Add(new Paragraph(" "));

                    document.Close();

                    byte[] byteArray = ms.ToArray();
                    return File(byteArray, "application/pdf", $"Report_{year}_{month}.pdf");
                }
            }
        }

        public IActionResult GenerateReport(DateTime date)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            if (applicationUser == null)
            {
                return NotFound("User not found");
            }

            var department = _db.Department.FirstOrDefault(d => d.DepartmentID == applicationUser.DepartmentID);

            if (department == null)
            {
                return NotFound("Department not found");
            }

            var employees = _db.ApplicationUser
                .Where(u => u.DepartmentID == applicationUser.DepartmentID && u.Id != loggedInUserId)
                .ToList();

            var attendanceList = _db.AttendanceTimeTable
                .Where(a => a.Date.Date == date.Date && employees.Select(e => e.CompanyID).Contains(a.EmpID))
                .OrderBy(a => a.EmpID)
                .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                using (iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 25, 25, 30, 30))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    var image = iTextSharp.text.Image.GetInstance("wwwroot/images/LetterHead.png");
                    float pageWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                    image.ScaleToFit(pageWidth, image.Height * (pageWidth / image.Width));
                    image.Alignment = Element.ALIGN_CENTER;
                    document.Add(image);

                    var footer = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                    var subTitleFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    var regularFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                    document.Add(new Paragraph($"Attendance Report - {date.ToString("yyyy-MM-dd")}", subTitleFont));
                    document.Add(new Paragraph(" "));
                    document.Add(new Paragraph($"Department: {department.DepartmentName}", regularFont));
                    document.Add(new Paragraph(" "));

                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.DefaultCell.Border = PdfPCell.NO_BORDER;

                    void AddHeaderCell(PdfPTable t, string text)
                    {
                        var font = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.WHITE);
                        PdfPCell cell = new PdfPCell(new Phrase(text, font))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BackgroundColor = new BaseColor(25, 161, 184),
                            Border = PdfPCell.NO_BORDER
                        };
                        t.AddCell(cell);
                    }

                    void AddBodyCell(PdfPTable t, string text, bool isOdd)
                    {
                        var font = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                        PdfPCell cell = new PdfPCell(new Phrase(text, font))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Border = PdfPCell.NO_BORDER
                        };
                        if (isOdd)
                        {
                            cell.BackgroundColor = BaseColor.WHITE;
                        }
                        else
                        {
                            cell.BackgroundColor = new BaseColor(185, 212, 217);
                        }
                        t.AddCell(cell);
                    }

                    AddHeaderCell(table, "No");
                    AddHeaderCell(table, "EmployeeID");
                    AddHeaderCell(table, "Check-In");
                    AddHeaderCell(table, "Check-Out");
                    AddHeaderCell(table, "Overtime");

                    bool isOddRow = true;
                    int counter = 1;

                    foreach (var attendance in attendanceList)
                    {
                        AddBodyCell(table, counter.ToString(), isOddRow);
                        AddBodyCell(table, attendance.EmpID, isOddRow);
                        AddBodyCell(table, attendance.CheckIn.ToString(@"hh\:mm"), isOddRow);
                        AddBodyCell(table, attendance.CheckOut.ToString(@"hh\:mm"), isOddRow);
                        AddBodyCell(table, attendance.OverTime, isOddRow);

                        isOddRow = !isOddRow;
                        counter++;
                    }

                    document.Add(table);
                    document.Add(new Paragraph(" "));

                    document.Add(new Paragraph("----End of the document----", footer));

                    document.Add(new Paragraph("*This document is system-generated and does not require a signature.", footer));
                    document.Add(new Paragraph(" "));

                    document.Close();

                    byte[] byteArray = ms.ToArray();
                    return File(byteArray, "application/pdf", $"Report_{date:yyyy-MM-dd}.pdf");
                }
            }
        }
        // Helper method to fetch attendance list based on filters
        private List<AttendanceManagement> GetAttendanceList(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            if (applicationUser == null)
            {
                return new List<AttendanceManagement>(); // Or handle appropriately
            }

            var companyId = applicationUser.CompanyID;

            var query = _db.AttendanceTimeTable.Where(a => a.EmpID == companyId);

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(a => a.Date >= fromDate && a.Date <= toDate);
            }

            var objAttendanceManagemetList = query.OrderByDescending(a => a.Date).ToList();

            return objAttendanceManagemetList;
        }

        // Helper method to get default breadcrumb items
        private List<BreadcrumbItem> GetDefaultBreadcrumb(string mainTitle, string currentAction, string currentController = "AttendanceManagement")
        {
            return new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = mainTitle, Url = Url.Action("Index", currentController) },
                new BreadcrumbItem { Title = GetBreadcrumbTitle(currentAction), Url = Url.Action(currentAction, currentController) },
            };
        }

        // Helper method to get breadcrumb title based on action
        private string GetBreadcrumbTitle(string action)
        {
            switch (action)
            {
                case "Index":
                    return "Home";
                case "Reports":
                    return "Reports";
                case "MarkAttendance":
                    return "Mark Attendance";
                case "ViewHistory":
                    return "View History";
                case "AddAttendance":
                    return "Add Attendance";
                default:
                    return action;
            }
        }
    }
}
