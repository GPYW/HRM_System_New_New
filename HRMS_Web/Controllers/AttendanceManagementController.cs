using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OfficeOpenXml;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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

        [Authorize(Roles = "Admin")]
        public IActionResult MarkAttendance
        {
            get
            {
                List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable.ToList();
                ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
            new BreadcrumbItem { Title = "Mark Attendance", Url = Url.Action("MarkAttendance", "AttendanceManagement") },

        };
                return View(objAttendanceManagemetList);
            }
        }

        public IActionResult ViewHistory()
        {
            // Get the logged-in user's ID
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // Fetch the applicationUser record using the logged-in user's ID
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            // Check if applicationUser is not null
            if (applicationUser == null)
            {
                return NotFound("User not found");
            }

            // Retrieve the CompanyId from the applicationUser record
            var companyId = applicationUser.CompanyID;

            // Query the AttendanceManagement table where EmpID equals the CompanyId
            List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable
                .Where(a => a.Id == companyId)
                .ToList();

            // Set breadcrumb data
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
{
    new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
    new BreadcrumbItem { Title = "View History", Url = Url.Action("ViewHistory", "AttendanceManagement") },
};

            // Return the filtered list to the view
            return View(objAttendanceManagemetList);
        }

        // File path: Controllers/AttendanceManagementController.cs

        [HttpPost]
        public IActionResult SearchAttendance(DateTime date)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == loggedInUserId);

            if (applicationUser == null)
            {
                return NotFound("User not found");
            }

            var companyId = applicationUser.CompanyID;
            List<AttendanceManagement> objAttendanceManagemetList = _db.AttendanceTimeTable
                .Where(a => a.Id == companyId && a.Date == date)
                .ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
{
    new BreadcrumbItem { Title = "Attendance Management", Url = Url.Action("Index", "AttendanceManagement") },
    new BreadcrumbItem { Title = "View History", Url = Url.Action("ViewHistory", "AttendanceManagement") },
};

            return View("ViewHistory", objAttendanceManagemetList);
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

        ////To handle upload files

        //[HttpPost]
        //public async Task<IActionResult> UploadAttendance(IFormFile attendanceFile, DateTime attendanceDate)
        //{
        //    if (attendanceFile == null || attendanceFile.Length == 0)
        //    {
        //        return Content("File not selected");
        //    }

        //    var filePath = Path.GetTempFileName();

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await attendanceFile.CopyToAsync(stream);
        //    }

        //    var attendanceList = new List<AttendanceManagement>();

        //    using (var package = new ExcelPackage(new FileInfo(filePath)))
        //    {
        //        var worksheet = package.Workbook.Worksheets[0];
        //        var rowCount = worksheet.Dimension.Rows;

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            if (!int.TryParse(worksheet.Cells[row, 1].Value?.ToString().Trim(), out int RecordId))
        //            {
        //                // Handle parse failure for RecordId (e.g., skip this row or log an error)
        //                continue;
        //            }

        //            //var RecordId = worksheet.Cells[row, 1].Value.ToString().Trim();
        //            var CompanyId = worksheet.Cells[row, 6].Value.ToString().Trim();
        //            var CheckIn = TimeSpan.Parse(worksheet.Cells[row, 2].Value.ToString().Trim());
        //            var CheckOut = TimeSpan.Parse(worksheet.Cells[row, 3].Value.ToString().Trim());
        //            var breakTime = worksheet.Cells[row, 4].Value.ToString().Trim() ?? "1 hrs";
        //            var overTime = worksheet.Cells[row, 5].Value.ToString().Trim();

        //            var attendance = new AttendanceManagement
        //            {
        //                RecordId = RecordId,
        //                Id = CompanyId,
        //                Date = attendanceDate,
        //                CheckIn = CheckIn,
        //                CheckOut = CheckOut,
        //                Break = breakTime,
        //                OverTime = overTime
        //            };

        //            attendanceList.Add(attendance);
        //        }
        //    }

        //    _db.AttendanceTimeTable.AddRange(attendanceList);
        //    await _db.SaveChangesAsync();

        //    return RedirectToAction("MarkAttendance");
        //}



        ////[HttpPost]
        ////public async Task<IActionResult> UploadAttendance(IFormFile attendanceFile, DateTime attendanceDate)
        ////{
        ////    if (attendanceFile == null || attendanceFile.Length == 0)
        ////    {
        ////        return Content("File not selected");
        ////    }

        ////    var filePath = Path.GetTempFileName();

        ////    using (var stream = new FileStream(filePath, FileMode.Create))
        ////    {
        ////        await attendanceFile.CopyToAsync(stream);
        ////    }

        ////    var attendanceList = new List<AttendanceManagement>();

        ////    using (var package = new ExcelPackage(new FileInfo(filePath)))
        ////    {
        ////        var worksheet = package.Workbook.Worksheets[0];
        ////        var rowCount = worksheet.Dimension.Rows;

        ////        for (int row = 2; row <= rowCount; row++)
        ////        {
        ////            if (!int.TryParse(worksheet.Cells[row, 1].Value?.ToString().Trim(), out int RecordId))
        ////            {
        ////                // Handle parse failure for RecordId (e.g., skip this row or log an error)
        ////                continue;
        ////            }

        ////            var CompanyIDString = worksheet.Cells[row, 6].Value?.ToString().Trim();
        ////            if (string.IsNullOrEmpty(CompanyIDString))
        ////            {
        ////                // Handle missing CompanyId
        ////                continue;
        ////            }

        ////            if (!int.TryParse(CompanyIDString, out int CompanyId))
        ////            {
        ////                // Handle parse failure for CompanyId
        ////                continue;
        ////            }

        ////            var userId =Id;
        ////            // Retrieve the relevant Id from the database
        ////            var user = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
        ////            if (user == null)
        ////            {
        ////                // Handle case where company is not found
        ////                continue;
        ////            }

        ////            var checkIn = TimeSpan.Parse(worksheet.Cells[row, 2].Value?.ToString().Trim() ?? "00:00:00");
        ////            var checkOut = TimeSpan.Parse(worksheet.Cells[row, 3].Value?.ToString().Trim() ?? "00:00:00");
        ////            var breakTime = worksheet.Cells[row, 4]?.Value?.ToString().Trim() ?? "1 hrs";
        ////            var overTime = worksheet.Cells[row, 5]?.Value?.ToString().Trim() ?? "00:00:00";

        ////            var attendance = new AttendanceManagement
        ////            {
        ////                RecordId = RecordId,
        ////                Id = CompanyID, // Use the Id from the retrieved company
        ////                Date = attendanceDate,
        ////                CheckIn = checkIn,
        ////                CheckOut = checkOut,
        ////                Break = breakTime,
        ////                OverTime = overTime
        ////            };

        ////            attendanceList.Add(attendance);
        ////        }
        ////    }

        ////    _db.AttendanceTimeTable.AddRange(attendanceList);
        ////    await _db.SaveChangesAsync();

        ////    return RedirectToAction("MarkAttendance");
        ////}
        ///


        //[HttpPost]
        //public async Task<IActionResult> UploadAttendance(IFormFile attendanceFile, DateTime attendanceDate)
        //{
        //    if (attendanceFile == null || attendanceFile.Length == 0)
        //    {
        //        return Content("File not selected");
        //    }

        //    var filePath = Path.GetTempFileName();

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await attendanceFile.CopyToAsync(stream);
        //    }

        //    var attendanceList = new List<AttendanceManagement>();

        //    using (var package = new ExcelPackage(new FileInfo(filePath)))
        //    {
        //        var worksheet = package.Workbook.Worksheets[0];
        //        var rowCount = worksheet.Dimension.Rows;

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            if (!int.TryParse(worksheet.Cells[row, 1].Value?.ToString().Trim(), out int recordId))
        //            {
        //                // Handle parse failure for RecordId (e.g., skip this row or log an error)
        //                continue;
        //            }

        //            var userIdString = worksheet.Cells[row, 6].Value?.ToString().Trim();
        //            if (string.IsNullOrEmpty(userIdString))
        //            {
        //                // Handle missing UserId
        //                continue;
        //            }

        //            if (!int.TryParse(userIdString, out int userId))
        //            {
        //                // Handle parse failure for UserId
        //                continue;
        //            }

        //            // Retrieve the relevant user from the database
        //            var user = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId.ToString());
        //            if (user == null)
        //            {
        //                // Handle case where user is not found
        //                continue;
        //            }

        //            var companyId = user.CompanyID; // Get the CompanyID from the retrieved user

        //            var checkIn = TimeSpan.Parse(worksheet.Cells[row, 2].Value?.ToString().Trim() ?? "00:00:00");
        //            var checkOut = TimeSpan.Parse(worksheet.Cells[row, 3].Value?.ToString().Trim() ?? "00:00:00");
        //            var breakTime = worksheet.Cells[row, 4]?.Value?.ToString().Trim() ?? "1 hrs";
        //            var overTime = worksheet.Cells[row, 5]?.Value?.ToString().Trim() ?? "00:00:00";

        //            var attendance = new AttendanceManagement
        //            {
        //                RecordId = recordId,
        //                Id = companyId, // Use the CompanyID from the retrieved user
        //                Date = attendanceDate,
        //                CheckIn = checkIn,
        //                CheckOut = checkOut,
        //                Break = breakTime,
        //                OverTime = overTime
        //            };

        //            attendanceList.Add(attendance);
        //        }
        //    }

        //    _db.AttendanceTimeTable.AddRange(attendanceList);
        //    await _db.SaveChangesAsync();

        //    return RedirectToAction("MarkAttendance");
        //}


        ///444
        ///


        //[HttpPost]
        //[ValidateAntiForgeryToken] // Prevent CSRF attacks
        //public async Task<IActionResult> AddAttendance(DateTime attendanceDate, IFormFile attendanceFile)
        //{
        //    if (attendanceFile == null || attendanceFile.Length == 0)
        //    {
        //        ModelState.AddModelError("attendanceFile", "Please select an attendance file.");
        //        return View("MarkAttendance"); // Return the MarkAttendance view with error message
        //    }

        //    // Process uploaded Excel file (implementation details in Step 3)
        //    var attendanceData = await ProcessAttendanceExcel(attendanceFile);

        //    if (attendanceData.Any(data => !ModelState.IsValid))
        //    {
        //        // Display validation errors for each invalid data entry
        //        foreach (var data in attendanceData.Where(data => !ModelState.IsValid))
        //        {
        //            ModelState.AddModelError(string.Empty, $"Error processing row: {data.Error}");
        //        }
        //        return View("MarkAttendance"); // Return the MarkAttendance view with validation errors
        //    }

        //    // Save attendance data to database (implementation details in Step 4)
        //    await SaveAttendanceData(attendanceData, attendanceDate);

        //    return RedirectToAction("Index", "Home"); // Redirect to a success page or Home page
        //}



        //// Helper method to process Excel file (implementation details in Step 3)
        //private async Task<List<AttendanceData>> ProcessAttendanceExcel(IFormFile attendanceFile)
        //{
        //    var attendanceData = new List<AttendanceData>();

        //    using (var stream = attendanceFile.OpenReadStream())
        //    {
        //        // Use a suitable library like OfficeOpenXml or ClosedXML to read the Excel file
        //        // Assuming you're using OfficeOpenXml:
        //        using (var excelPackage = new ExcelPackage(stream))
        //        {
        //            var worksheet = excelPackage.Workbook.Worksheets.First();
        //            int rowCount = worksheet.Dimension.End.Row;

        //            for (int row = 2; row <= rowCount; row++) // Skip header row (row 1)
        //            {
        //                try
        //                {
        //                    var employeeId = worksheet.Cells[row, 1].Value?.ToString(); // Assuming employee ID is in column 1
        //                    var checkInTime = worksheet.Cells[row, 2].Value?.ToString(); // Assuming check-in time is in column 2
        //                                                                                 // ... (extract other relevant data from Excel cells)

        //                    // Validate extracted data (example for employee ID)
        //                    if (string.IsNullOrEmpty(employeeId))
        //                    {
        //                        ModelState.AddModelError(string.Empty, $"Missing employee ID in row {row}.");
        //                        continue;
        //                    }

        //                    // Find user (CompanyID can be retrieved from user object)
        //                    var user = await _userManager.FindByIdAsync(employeeId);
        //                    if (user == null)
        //                    {


        //                        ModelState.AddModelError(string.Empty, $"Employee

        //// Helper method to save attendance data to database (implementation details in Step 4)
        //private async Task SaveAttendanceData(List<AttendanceData> attendanceData, DateTime attendanceDate)
        //                        {
        //                            // ... (implementation details)
        //                        }

        //                    }
        //                }
        //                }
        //        }
        //    }

        //}


        ////5555555555555555555555555555555555555



    }
}
                       
