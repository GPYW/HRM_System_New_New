using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class EmployeeManagementController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

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

        public IActionResult Edit(int ? id)
        {
            if (id == null || id == 0 ){
                return NotFound();
            }
            ApplicationUser? ApplicationUserFromDb =_db.ApplicationUser.Find(id);

            if(ApplicationUserFromDb == null)
            {
                return NotFound();
            }
            return View(ApplicationUserFromDb);
        }

        [HttpPost]
        public IActionResult Edit(ApplicationUser obj)
        {
            if (ModelState.IsValid)
            {
                _db.ApplicationUser.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
           
        }


    }
}
