using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using HRMS_Web.DataAccess.Data;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.InkML;
using System;
using Mono.TextTemplating;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS_Web.Controllers
{
    public class PerformanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;

        public PerformanceController(ApplicationDbContext context)
        {
            _context = context;
            //_userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AdminView()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            // Filter appraisals by department of the admin
            var appraisals = await _context.Appraisals
                .Where(a => a.User.DepartmentID == admin.DepartmentID)
                .ToListAsync();

            // Filter goals by department of the admin
            var goals = await _context.Goals
                .Where(g => g.User.DepartmentID == admin.DepartmentID)
                .Include(g => g.User)
                .ToListAsync();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Performance", Url = Url.Action("Dashboard", "Performance") },
                new BreadcrumbItem { Title = "Admin View", Url = Url.Action("AdminView", "Performance") }
            };

            var model = new Tuple<IEnumerable<Appraisal>, IEnumerable<Goals>>(appraisals, goals);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateGoal()
        {
            ViewBag.Users = GetUsersAsync().Result;
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Appraisal()
        {
            var appraisals = await _context.Appraisals.ToListAsync();
            return View(appraisals);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> AppraisalForm()
        {
            //var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var admin = _context.ApplicationUser.FirstOrDefault(u => u.Id == adminId);

            //if (admin == null)
            //{
            //    return NotFound();
            //}

            //// Filter names by department of the admin
            //List<ApplicationUser> objAttendanceManagementList = _context.Appraisals
            //    .Where(a => a.ApplicationUser.DepartmentID == admin.DepartmentID)
            //    .ToList();

            ViewBag.Users = GetUsersAsync().Result;
            return View();
        }

        private async Task<List<ApplicationUser>> GetUsersAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null)

            {
                return new List<ApplicationUser>();
            }

            string departmentID = currentUser?.DepartmentID;

            return await _context.ApplicationUser
                .Where(u => u.DepartmentID == departmentID && u.Id != userId)
                .ToListAsync();
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppraisalForm([Bind("AppraisalDate,Status,Id,Designation,Employee")] Appraisal appraisal)
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

                ViewBag.Users = GetUsersAsync().Result;
                ModelState.AddModelError(string.Empty, "User not found");
                //return View(appraisal); //to display the validation errors
            }

            
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
            var user = await _context.ApplicationUser
               .Where(u => u.Id == appraisal.Id)
               .Select(u => new { u.FirstName, u.LastName, u.DepartmentID })
               .FirstOrDefaultAsync();

            if (user != null)
            {

                appraisal.Employee = $"{user.FirstName} {user.LastName}";

                var department = await _context.Department
                    .Where(u => u.DepartmentID == user.DepartmentID)
                    .Select(u => u.DepartmentName)
                    .FirstOrDefaultAsync();

                appraisal.Title = "Orgamizational";
                appraisal.Department = department;
                _context.Add(appraisal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Appraisal));
            }


            ViewBag.Users = await GetUsersAsync();
            ModelState.AddModelError(string.Empty, "User not found");
            return View(appraisal);
        }


        //[HttpGet]
        //public async Task<IActionResult> AppraisalEdit(int AppraisalId)
        //{
        //    if (AppraisalId == null)
        //    {
        //        return NotFound();
        //    }
        //    Appraisal AppraisalFromDb = _context.ApplicationUser.Include(u => u.Department).FirstOrDefault(u => u.Id == AppraisalId);

        //    if (AppraisalFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    //ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        //    //{
        //    //    new BreadcrumbItem { Title = "Employee Management", Url = Url.Action("Index", "EmployeeManagement") },
        //    //    new BreadcrumbItem { Title = "Edit", Url = Url.Action("Edit", "EmployeeManagement") },

        //    //};
        //    return View(AppraisalFromDb);
        //}



        //Edit Appraisal


        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> AppraisalEdit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var appraisal = await _context.Appraisals.FindAsync(id);
            if (appraisal == null)
            {
                return NotFound();
            }

            var users = await GetUsersAsync();
            ViewBag.Users = users;

            return View(appraisal);
        }



        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AppraisalEdit([Bind("AppraisalId,AppraisalDate,Status,Id,Designation,Employee")] Appraisal appraisal, string selectedEmployeeId)
        {

            //if (id != appraisal.AppraisalId)
            //{
            //    return NotFound();
            //}

            if (!ModelState.IsValid)
            {
                Appraisal? obj = _context.Appraisals.Find(appraisal.AppraisalId);
                if (obj == null)
                {
                    return NotFound();
                }


                var user = await _context.ApplicationUser
                    .Where(u => u.Id == selectedEmployeeId)
                    .Select(u => new { u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    obj.Employee = $"{user.FirstName} {user.LastName}";
                    //  obj.Id = selectedEmployeeId; // Set the selected employee ID
                    obj.AppraisalDate = appraisal.AppraisalDate;
                    obj.Status = appraisal.Status;
                    obj.Designation = appraisal.Designation;


                    //_context.Add(appraisal);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Appraisal));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            ViewBag.Users = await GetUsersAsync();
            return View(appraisal);
        }




        //// Edit Action
        //[Authorize(Roles = SD.Role_Admin)]
        //[HttpGet]
        //public IActionResult AppraisalEdit(int? AppraisalId)
        //{
        //    if (AppraisalId == null || AppraisalId == 0)
        //    {
        //        return NotFound();
        //    }
        //    Appraisal? AppraisalFromDb = _context.Appraisals.Find(AppraisalId);

        //    if (AppraisalFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(AppraisalFromDb);
        //}

        //[HttpPost, ActionName("AppraisalEdit")]
        //public IActionResult AppraisalEdit(Appraisal model)
        //{
        //    Appraisal? obj = _context.Appraisals.Find(model.AppraisalId);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    obj.AppraisalDate = model.AppraisalDate;
        //    obj.Status = model.Status;
        //    //  obj.Employee = model.Employee;
        //    obj.Designation = model.Designation;

        //    _context.SaveChanges();
        //    return RedirectToAction("Appraisal");
        //}

        //Delete Appraisal details


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAppraisalConfirmed(int AppraisalId)
        {
            try
            {
                var appraisal = _context.Appraisals.Find(AppraisalId);
                if (appraisal != null)
                {
                    _context.Appraisals.Remove(appraisal);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Appraisal not found" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = ex.Message });
            }
        }


        //Goals create

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Goals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
            {
                return NotFound("Current user not found");
            }

            var departmentID = currentUser.DepartmentID;
            var employees = await _context.ApplicationUser
                .Where(u => u.DepartmentID == departmentID && u.Id != userId)
                .ToListAsync();

            var goals = await _context.Goals.Include(g => g.User).ToListAsync();
            //var model = new Tuple<IEnumerable<Goals>, Goals>(goals, new Goals());

            //ViewBag.Users = employees;
            return View(goals);
        }


        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGoal([Bind("Title,Description,StartDate,EndDate,Status")] Goals goal, string selectedEmployeeId)
        {
            if (!ModelState.IsValid)
            {
                var user = await _context.ApplicationUser
                    .Where(u => u.Id == selectedEmployeeId)
                    .Select(u => new { u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    goal.Employee = $"{user.FirstName} {user.LastName}";
                    goal.Id = selectedEmployeeId;

                    _context.Add(goal);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Goals));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            ViewBag.Users = await GetUsersAsync();
            var goals = await _context.Goals.Include(g => g.User).ToListAsync();
            //var model = new Tuple<IEnumerable<Goals>, Goals>(goals, goal);
            return View(goals);
        }

        //Edit Goal

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> EditGoal(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return NotFound();
            }

            var users = await GetUsersAsync();
            ViewBag.Users = users;

            return View(goal);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGoal(int id, [Bind("GoalId,Title,Description,StartDate,EndDate,Status")] Goals goal, string selectedEmployeeId)
        {
            if (id != goal.GoalId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var user = await _context.ApplicationUser
                    .Where(u => u.Id == selectedEmployeeId)
                    .Select(u => new { u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    goal.Employee = $"{user.FirstName} {user.LastName}";
                    goal.Id = selectedEmployeeId; // Set the selected employee ID

                    _context.Update(goal);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Goals));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            ViewBag.Users = await GetUsersAsync();
            return View(goal);
        }


        //DeleteGoal

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return NotFound();
            }

            return View(goal);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost, ActionName("DeleteGoal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoalConfirmed(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                _context.Goals.Remove(goal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Goals));
        }



    }
}
