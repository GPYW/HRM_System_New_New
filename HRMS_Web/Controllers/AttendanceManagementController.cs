using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using ExcelDataReader;
using OfficeOpenXml.Style;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;

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
                EmpID=user.CompanyID

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





    }
}
