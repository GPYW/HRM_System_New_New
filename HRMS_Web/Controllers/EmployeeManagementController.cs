using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRMS_Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class EmployeeManagementController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;
        private object dbContext;

        public async Task<IActionResult> Index()
        {
            // Step 1: Get the logged-in user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Step 2: If user ID is null, redirect to the login page
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Step 3: Fetch the logged-in user's department ID
            var user = await _db.ApplicationUser
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == userId);

            // Step 4: If department ID is null, handle the error appropriately
            if (user?.DepartmentID == null)
            {
                // Handle the error appropriately, e.g., show an error message or redirect
                return RedirectToAction("Error", "Home"); // Adjust as needed
            }

            var userDepId = user.DepartmentID;

            // Step 5: Fetch all users who belong to the same department
            var applicationUsers = await _db.ApplicationUser
                .Include(u => u.Department)
                .Where(u => u.DepartmentID == userDepId)
                .ToListAsync();

            // Set the breadcrumb
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Employee Management", Url = Url.Action("Index", "EmployeeManagement") },
        };

            // Step 6: Return the view with the filtered list of users
            return View(applicationUsers);
        }

        public IActionResult Register()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
    {
        new BreadcrumbItem { Title = "Employee Management", Url = Url.Action("Index", "EmployeeManagement") },
        new BreadcrumbItem { Title = "Register Employee", Url = "/Identity/Account/Register" }
    };

            return View();
        }


        [HttpPost]
        public IActionResult Register(ApplicationUser obj)
        {
            _db.ApplicationUser.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(string? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            ApplicationUser applicationUserFromDb = _db.ApplicationUser.Include(u => u.Department).FirstOrDefault(u => u.Id == Id);

            if (applicationUserFromDb == null)
            {
                return NotFound();
            }
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Employee Management", Url = Url.Action("Index", "EmployeeManagement") },
                new BreadcrumbItem { Title = "Edit", Url = Url.Action("Edit", "EmployeeManagement") },

            };
            return View(applicationUserFromDb);
        }

        [HttpPost]
        public IActionResult Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser? obj = _db.ApplicationUser.Find(model.Id);
                if (obj == null)
                {
                    return NotFound();
                }

                // Update properties of the ApplicationUser object
                obj.FirstName = model.FirstName;
                obj.LastName = model.LastName;
                obj.Address = model.Address;
                //obj.Email = model.Email;
                obj.DOB = model.DOB;
                obj.PhoneNumber = model.PhoneNumber;
                obj.join_date = model.join_date;
                obj.DepartmentID = model.DepartmentID;
                // Update other properties as needed

                try
                {
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Handle concurrency conflicts if needed
                    return RedirectToAction("ConcurrencyError", "Error");
                }
            }
            // If ModelState is not valid, return to the Edit view with the model to show validation errors
            return View(model);
        }




        //Delete

        public IActionResult Delete(string? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            ApplicationUser? ApplicationUserFromDb = _db.ApplicationUser.Find(Id);

            if (ApplicationUserFromDb == null)
            {
                return NotFound();
            }
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Employee Management", Url = Url.Action("Index", "EmployeeManagement") },
                new BreadcrumbItem { Title = "Delete", Url = Url.Action("Delete", "EmployeeManagement") },

            };
            return View(ApplicationUserFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(string? Id)
        {
            ApplicationUser? obj = _db.ApplicationUser.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }

            _db.ApplicationUser.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");


        }


    }
}
