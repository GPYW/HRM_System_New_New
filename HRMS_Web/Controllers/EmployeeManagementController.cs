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
            List<ApplicationUser> applicationUsers = _db.ApplicationUser.ToList();
            return View(applicationUsers);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(ApplicationUser obj)
        {
            _db.ApplicationUser.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Edit Employee Details


        public IActionResult Edit(string? Id)
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

        [HttpPost]
        public async Task<IActionResult> EditAsync(ApplicationUser obj)
        {

            if(ModelState.IsValid)
            {
                try
                {
                    // Your code to update the entity
                    _db.ApplicationUser.Update(obj);
                    _db.SaveChanges();
                   
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Get the entity that caused the conflict
                    var entry = ex.Entries.Single();
                    var databaseEntry = entry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        // The entity has been deleted from the database
                        // Handle this situation as needed
                    }
                    else
                    {
                        // The entity has been modified in the database
                        // Update the entity with the database values
                        entry.OriginalValues.SetValues(databaseEntry);

                        // Optionally, let the user know about the conflict and ask how to resolve it
                        // Then, try saving changes again or handle it based on your application's logic
                    }
                }
                return RedirectToAction("Index");

            }
            return View(obj);


        }
            //public IActionResult Edit(string? Id)
            //{
            //    if (Id == null)
            //    {
            //        return NotFound();
            //    }
            //    ApplicationUser? ApplicationUserFromDb = _db.ApplicationUser.Find(Id);

            //    if (ApplicationUserFromDb == null)
            //    {
            //        return NotFound();
            //    }
            //    return View(ApplicationUserFromDb);
            //}

            //[HttpPost]
            //public IActionResult EditPOST(ApplicationUser obj)
            //{
            //    if (ModelState.IsValid)
            //    {
            //        _db.ApplicationUser.Update(obj);
            //        _db.SaveChanges();
            //        return RedirectToAction("Index");
            //    }
            //    return View(obj);

            //}



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


