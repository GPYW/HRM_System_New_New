using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using HRMS_Web.DataAccess.Data;
using DocumentFormat.OpenXml.Bibliography;

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
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Goals()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Appraisal()
        {
            var appraisals = await _context.Appraisals.ToListAsync();
            return View(appraisals);
        }

        [HttpGet]
        public async Task<IActionResult> AppraisalForm()
        {
            
            ViewBag.Users = GetUsersAsync().Result;
            return View();
        }

        private async Task<List<ApplicationUser>> GetUsersAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
            string departmentID = currentUser?.DepartmentID;

            return await _context.ApplicationUser
                .Where(u => u.DepartmentID == departmentID)
                .ToListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppraisalForm([Bind("AppraisalDate,Status,Employee,Designation,Department")] Appraisal appraisal)
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
            }


            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var user = await _context.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);
            var user = await _context.ApplicationUser
               .Where(u => u.Id == appraisal.Id)
               .Select(u => new { u.FirstName, u.LastName , u.DepartmentID})
               .FirstOrDefaultAsync();

            if (user != null)
                {
                   
                     appraisal.Employee = $"{user.FirstName} {user.LastName}";

                var department = await _context.Department
                    .Where(u => u.DepartmentID == user.DepartmentID)
                    .Select(u=>u.DepartmentName)
                    .FirstOrDefaultAsync();

                    appraisal.Department = department;
                    _context.Add(appraisal);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Appraisal));
                }
             
            
            ViewBag.Users = await GetUsersAsync();
            return View(appraisal);
        }

    }
}