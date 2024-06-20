using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class EmployeeManagementController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;
        private object dbContext;

        //Register
        public IActionResult Index()
        {
            var applicationUsers = _db.ApplicationUser.Include(u => u.Department).ToList();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Employee Management", Url = Url.Action("Index", "EmployeeManagement") },
        };
            return View(applicationUsers);
        }

        public IActionResult Register()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Employee Management", Url = Url.Action("Index", "EmployeeManagement") },
            //new BreadcrumbItem { Title = "Add a New Employee", Url = Url.Action("Register","EmployeeManagement") }
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
