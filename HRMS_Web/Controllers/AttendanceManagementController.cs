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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using ExcelDataReader;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;


namespace HRMS_Web.Controllers
{
    public class AttendanceManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AttendanceManagementController(ApplicationDbContext db)
        {
            _db = db;

        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
                new BreadcrumbItem { Title = "Home", Url = Url.Action("Index", "AttendanceManagement") },
            };
            return View();
        }

        [HttpGet]
        public IActionResult Reports()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
                new BreadcrumbItem { Title = "Reports", Url = Url.Action("Reports", "AttendanceManagement") },
            };
            return View();
        }

        //    public IActionResult ViewHistory(DateTime? date)
        //    {
        //        // Get the logged-in user's ID
        //        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //        // Fetch the applicationUser record using the logged-in user's ID
        //        var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

        //        // Check if applicationUser is not null
        //        if (applicationUser == null)
        //        {
        //            return NotFound("User not found");
        //        }

        //        // Query the AttendanceManagement table where EmpID equals the logged-in user's ID
        //        IQueryable<AttendanceManagement> query = _db.AttendanceTimeTable
        //            .Where(a => a.Id == loggedInUserId);

        //        // Filter by date if a date is provided
        //        if (date.HasValue)
        //        {
        //            query = query.Where(a => a.Date == date.Value);
        //            ViewData["SelectedDate"] = date.Value.ToString("yyyy-MM-dd");
        //        }

        //        // Execute the query and get the results
        //        List<AttendanceManagement> objAttendanceManagemetList = query.ToList();

        //        //// Set breadcrumb data
        //        ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        //{
        //    new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
        //    new BreadcrumbItem { Title = "View History", Url = Url.Action("ViewHistory", "AttendanceManagement") },
        //};

        //        // Return the filtered list to the view
        //       /return View(objAttendanceManagemetList);
        //    }


        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public IActionResult MarkAttendance()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var admin = _db.ApplicationUser.FirstOrDefault(u => u.Id == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            // Filter attendance records by department of the admin
            List<AttendanceManagement> objAttendanceManagementList = _db.AttendanceTimeTable
                .Where(a => a.ApplicationUser.DepartmentID == admin.DepartmentID)
                .ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
    {
        new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
        new BreadcrumbItem { Title = "Mark Attendance", Url = Url.Action("MarkAttendance", "AttendanceManagement") },
    };

            return View(objAttendanceManagementList);
        }



        [HttpGet]
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
            // Fetch the ApplicationUser based on the EmpID which refers to CompanyID
            var user = _db.ApplicationUser.SingleOrDefault(u => u.CompanyID == obj.EmpID);

            // Check if user is found
            if (user != null)
            {
                // Assign the Id of the fetched user to the AttendanceManagement object
                obj.Id = user.Id;
            }
            else
            {
                // Handle the case where the user is not found
                // You can return an error message or take appropriate action
                return BadRequest("Employee not found");
            }

            obj.IsPresent = true;
            obj.Break = "1 hrs";

            _db.AttendanceTimeTable.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("MarkAttendance");
        }


        [HttpGet]
        public IActionResult AddDailyAttendance()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var admin = _db.ApplicationUser.FirstOrDefault(u => u.Id == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            var employees = _db.ApplicationUser.Where(u => u.DepartmentID == admin.DepartmentID && u.Id != adminId).Select(user => new AttendanceManagement
            {
                ApplicationUser = user,
                Date = DateTime.Now,
                CheckIn = DateTime.Now.TimeOfDay,
                CheckOut = DateTime.Now.TimeOfDay,
                OverTime = "0 hrs",
                Id = user.Id,
                EmpID = user.CompanyID

            }).ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
                new BreadcrumbItem { Title = "Mark Attendance", Url = Url.Action("MarkAttendance", "AttendanceManagement") },
                new BreadcrumbItem { Title = "Add Daily Attendance", Url = Url.Action("AddDailyAttendance", "AttendanceManagement") }
            };

            return View(employees);
        }


        [HttpGet]
        public IActionResult GetEmployeeList(DateTime date)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _db.ApplicationUser.FirstOrDefault(u => u.Id == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            // Get attendance records for the selected date
            var attendanceRecords = _db.AttendanceTimeTable
                 .Include(a => a.ApplicationUser)
                 .Where(a => a.Date.Date == date.Date && a.ApplicationUser.DepartmentID == admin.DepartmentID)
                 .ToList();

            var employees = attendanceRecords.Select(a => new
            {
                id = a.Id,
                companyID = a.ApplicationUser.CompanyID,
                firstName = a.ApplicationUser.FirstName,
                lastName = a.ApplicationUser.LastName,
                //isPresent = attendanceRecords.Any(record => record.Id == a.Id && record.IsPresent) 
                isPresent = a.IsPresent // Check the specific attendance record for the selected date

            })
            .ToList();

            return Json(employees);
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



        //To Handle Daiy Attendance

        //[HttpPost]
        //public IActionResult SaveDailyAttendance([FromBody] List<AttendanceManagement> attendances)
        //{
        //    if (attendances == null || !attendances.Any())
        //    {
        //        return BadRequest(new { success = false, message = "No attendance data received." });
        //    }

        //    try
        //    {
        //        foreach (var attendance in attendances)
        //        {
        //            // Fetch the ApplicationUser based on the EmpID which refers to CompanyID
        //        //    var user = _db.ApplicationUser.SingleOrDefault(u => u.CompanyID == attendance.EmpID);

        //            //if (user == null)
        //            //{
        //            //    // Handle the case where the user is not found
        //            //    return BadRequest(new { success = false, message = $"User with CompanyID '{attendance.EmpID}' not found." });
        //            //}

        //            // Assign the Id of the fetched user to the AttendanceManagement object
        //           // attendance.Id = user.CompanyID;

        //            // Check if the attendance record already exists for the employee on the given date
        //            var existingRecord = _db.AttendanceTimeTable
        //                .FirstOrDefault(a => a.EmpID == attendance.EmpID && a.Date.Date == attendance.Date.Date);

        //            if (existingRecord != null)
        //            {
        //                // Update existing record if needed (for now, assuming it's handled elsewhere)
        //                continue;
        //            }
        //            else
        //            {
        //                // Add new record
        //                var newAttendance = new AttendanceManagement
        //                {
        //                    Id = attendance.Id,
        //                    EmpID = attendance.EmpID,
        //                    Date = attendance.Date,
        //                    IsPresent = attendance.IsPresent,
        //                    CheckIn = attendance.CheckIn,
        //                    CheckOut = attendance.CheckOut,
        //                    OverTime = attendance.OverTime,
        //                    Break = attendance.Break
        //                };
        //                _db.AttendanceTimeTable.Add(newAttendance);
        //            }
        //        }

        //        _db.SaveChanges();

        //        return Ok(new { success = true, message = "Attendance saved successfully!" });

        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if necessary
        //        return StatusCode(500, new { success = false, message = "An error occurred while saving the attendance." });
        //    }
        //}



        //public IActionResult SaveDailyAttendance([FromBody] List<AttendanceManagement> attendances)
        //{
        //    if (attendances == null || !attendances.Any())
        //    {
        //        return BadRequest(new { success = false, message = "No attendance data received." });
        //    }

        //    try
        //    {
        //        foreach (var attendance in attendances)
        //        {
        //            // Fetch the ApplicationUser based on the Id property (assuming it's populated correctly on the client-side)
        //            var user = _db.ApplicationUser.SingleOrDefault(u => u.Id == attendance.Id);

        //            // Handle the case where the user is not found (optional, based on your business logic)
        //            if (user == null)
        //            {
        //                // You can return an error message, create a new user record, or handle it differently
        //                return BadRequest(new { success = false, message = $"User with Id '{attendance.Id}' not found." });
        //            }

        //            // Assign the fetched user to the attendance object
        //            attendance.ApplicationUser = user;

        //            var existingRecord = _db.AttendanceTimeTable
        //               .FirstOrDefault(a => a.Id == attendance.Id && a.Date.Date == attendance.Date.Date);


        //            // Check for existing attendance records and handle them appropriately (update or create new)
        //            // ... (rest of your existing code to check and handle existing records)

        //            if (existingRecord != null)
        //            {
        //                // Update existing record if needed (for now, assuming it's handled elsewhere)
        //                continue;
        //            }
        //            else
        //            {
        //                // Add new record
        //                var newAttendance = new AttendanceManagement
        //                {
        //                    ApplicationUser = attendance.ApplicationUser, // Set the foreign key relationship
        //                    EmpID = attendance.EmpID,
        //                    Date = attendance.Date,
        //                    IsPresent = attendance.IsPresent,
        //                    CheckIn = attendance.CheckIn,
        //                    CheckOut = attendance.CheckOut,
        //                    OverTime = attendance.OverTime,
        //                    Break = attendance.Break
        //                };
        //                _db.AttendanceTimeTable.Add(newAttendance);
        //            }
        //        }

        //                 _db.SaveChanges();

        //        return Ok(new { success = true, message = "Attendance saved successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if necessary
        //        return StatusCode(500, new { success = false, message = "An error occurred while saving the attendance.", error = ex.Message });
        //    }
        //}


        [HttpPost]
        public IActionResult SaveDailyAttendance([FromBody] List<AttendanceManagement> attendances)
        {
            if (attendances == null || !attendances.Any())
            {
                return Json(new { success = false, message = "No attendance data provided." });
            }

            try
            {
                foreach (var attendance in attendances)
                {
                    var existingRecord = _db.AttendanceTimeTable
                        .FirstOrDefault(a => a.Date == attendance.Date && a.EmpID == attendance.EmpID);

                    if (existingRecord != null)
                    {
                        // Update existing record
                        existingRecord.CheckIn = attendance.CheckIn;
                        existingRecord.CheckOut = attendance.CheckOut;
                        existingRecord.Break = attendance.Break;
                        existingRecord.OverTime = attendance.OverTime;
                        existingRecord.IsPresent = attendance.IsPresent;
                    }
                    else
                    {
                        // Add new record
                        _db.AttendanceTimeTable.Add(new AttendanceManagement
                        {
                            EmpID = attendance.EmpID,
                            Date = attendance.Date,
                            CheckIn = attendance.CheckIn,
                            CheckOut = attendance.CheckOut,
                            Break = attendance.Break,
                            OverTime = attendance.OverTime,
                            IsPresent = attendance.IsPresent,
                            Id = attendance.Id

                        });
                    }
                }

                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        //To generate and download the excel file
        public IActionResult ExportAttendanceToExcel(DateTime selectedDate)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _db.ApplicationUser.FirstOrDefault(u => u.Id == adminId);
            if (admin == null)
            {
                return NotFound();
            }

            var attendanceRecords = _db.AttendanceTimeTable
                .Include(a => a.ApplicationUser)
                .Where(a => a.Date.Date == selectedDate.Date && a.ApplicationUser.DepartmentID == admin.DepartmentID)
                .ToList();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Attendance");

                // Add header
                worksheet.Cells[1, 1].Value = "Employee ID";
                worksheet.Cells[1, 2].Value = "Employee Name";
                worksheet.Cells[1, 3].Value = "Status";
                worksheet.Cells[1, 4].Value = "Check In";
                worksheet.Cells[1, 5].Value = "Check Out";
                worksheet.Cells[1, 6].Value = "Overtime";

                // Add data
                for (int i = 0; i < attendanceRecords.Count; i++)
                {
                    var record = attendanceRecords[i];
                    worksheet.Cells[i + 2, 1].Value = record.ApplicationUser.CompanyID.ToString(); // Convert int to string
                    worksheet.Cells[i + 2, 2].Value = $"{record.ApplicationUser.FirstName} {record.ApplicationUser.LastName}";
                    worksheet.Cells[i + 2, 3].Value = record.IsPresent ? "Present" : "Absent";
                    worksheet.Cells[i + 2, 4].Value = record.IsPresent ? record.CheckIn?.ToString(@"hh\:mm") : "-";
                    worksheet.Cells[i + 2, 5].Value = record.IsPresent ? record.CheckOut?.ToString(@"hh\:mm") : "-";
                    worksheet.Cells[i + 2, 6].Value = record.IsPresent ? record.OverTime : "-"; // Convert TimeSpan? to string

                }

                package.Save();
            }

            stream.Position = 0;
            var fileName = $"Attendance_{selectedDate:yyyy-MM-dd}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        //To upload and process the excel file
        [HttpPost]
        public async Task<IActionResult> ImportAttendanceFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not selected.");
            }

            try
            {
                var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    string fileName = file.FileName;

                    // Extract date from file name (assuming format "Attendance_yyyy-MM-dd.xlsx")
                    string dateString = fileName.Split(new char[] { '_', '.' })[1];
                    if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        return BadRequest("Invalid date format in file name.");
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var employeeId = worksheet.Cells[row, 1].Text;
                        var status = worksheet.Cells[row, 3].Text;
                        var checkIn = worksheet.Cells[row, 4].Text;
                        var checkOut = worksheet.Cells[row, 5].Text;
                        var overTime = worksheet.Cells[row, 6].Text;

                        if (string.IsNullOrEmpty(employeeId))
                        {
                            // Log or handle the case where employeeId is empty
                            continue;
                        }

                        var attendanceRecord = _db.AttendanceTimeTable.FirstOrDefault(a =>
                            a.ApplicationUser.CompanyID == employeeId && a.Date.Date == date.Date);

                        if (attendanceRecord != null)
                        {
                            // Update existing record
                            attendanceRecord.IsPresent = status == "Present";
                            attendanceRecord.CheckIn = checkIn != "-" && TimeSpan.TryParse(checkIn, out var checkInTime) ? (TimeSpan?)checkInTime : null;
                            attendanceRecord.CheckOut = checkOut != "-" && TimeSpan.TryParse(checkOut, out var checkOutTime) ? (TimeSpan?)checkOutTime : null;
                            attendanceRecord.OverTime = overTime != "-" ? overTime : null;
                        }
                        else
                        {
                            // Create new record
                            var user = _db.ApplicationUser.FirstOrDefault(u => u.CompanyID == employeeId);
                            if (user != null)
                            {

                                attendanceRecord = new AttendanceManagement
                                {
                                    Id = user.Id,
                                    Date = date,
                                    IsPresent = status == "Present",
                                    CheckIn = checkIn != "-" && TimeSpan.TryParse(checkIn, out var checkInTime) ? (TimeSpan?)checkInTime : null,
                                    CheckOut = checkOut != "-" && TimeSpan.TryParse(checkOut, out var checkOutTime) ? (TimeSpan?)checkOutTime : null,
                                    OverTime = overTime != "-" ? overTime : null
                                };
                                _db.AttendanceTimeTable.Add(attendanceRecord);
                            }
                        }
                    }

                    await _db.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Attendance imported successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) details
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
            var employeeName = $"{applicationUser.FirstName} {applicationUser.LastName}";

            List<AttendanceManagement> attendanceList = _db.AttendanceTimeTable
                .Where(a => a.EmpID == employeeId && a.Date.Month == month && a.Date.Year == year)
                .OrderByDescending(a => a.Date)
                .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4, 25, 25, 30, 30))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    var image = Image.GetInstance("wwwroot/images/LetterHead.png");
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

                    PdfPTable table = new PdfPTable(6)
                    {
                        WidthPercentage = 100,
                        DefaultCell = { Border = PdfPCell.NO_BORDER }
                    };

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
                        cell.BackgroundColor = isOdd ? BaseColor.WHITE : new BaseColor(185, 212, 217);
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
                        AddBodyCell(table, attendance.CheckIn.HasValue ? attendance.CheckIn.Value.ToString(@"hh\:mm") : "", isOddRow);
                        AddBodyCell(table, attendance.CheckOut.HasValue ? attendance.CheckOut.Value.ToString(@"hh\:mm") : "", isOddRow);
                        AddBodyCell(table, attendance.OverTime ?? "", isOddRow);
                        AddBodyCell(table, attendance.Break ?? "", isOddRow);

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
                using (Document document = new Document(PageSize.A4, 25, 25, 30, 30))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    var image = Image.GetInstance("wwwroot/images/LetterHead.png");
                    float pageWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                    image.ScaleToFit(pageWidth, image.Height * (pageWidth / image.Width));
                    image.Alignment = Element.ALIGN_CENTER;
                    document.Add(image);

                    var footer = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                    var subTitleFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    var regularFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                    document.Add(new Paragraph($"Attendance Report - {date:yyyy-MM-dd}", subTitleFont));
                    document.Add(new Paragraph(" "));
                    document.Add(new Paragraph($"Department: {department.DepartmentName}", regularFont));
                    document.Add(new Paragraph(" "));

                    PdfPTable table = new PdfPTable(5)
                    {
                        WidthPercentage = 100,
                        DefaultCell = { Border = PdfPCell.NO_BORDER }
                    };

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
                        cell.BackgroundColor = isOdd ? BaseColor.WHITE : new BaseColor(185, 212, 217);
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
                        AddBodyCell(table, attendance.CheckIn.HasValue ? attendance.CheckIn.Value.ToString(@"hh\:mm") : "", isOddRow);
                        AddBodyCell(table, attendance.CheckOut.HasValue ? attendance.CheckOut.Value.ToString(@"hh\:mm") : "", isOddRow);
                        AddBodyCell(table, attendance.OverTime ?? "", isOddRow);

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

        private List<BreadcrumbItem> GetDefaultBreadcrumb(string mainTitle, string currentAction, string currentController = "AttendanceManagement")
        {
            return new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = mainTitle, Url = Url.Action("Index", currentController) },
            new BreadcrumbItem { Title = GetBreadcrumbTitle(currentAction), Url = Url.Action(currentAction, currentController) },
        };
        }

        private string GetBreadcrumbTitle(string action)
        {
            return action switch
            {
                "Index" => "Home",
                "Reports" => "Reports",
                "MarkAttendance" => "Mark Attendance",
                "ViewHistory" => "View History",
                "AddAttendance" => "Add Attendance",
                _ => action,
            };
        }
    }
}
