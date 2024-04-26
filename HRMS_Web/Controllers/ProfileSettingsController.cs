using HRMS_Web.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HRMS_Web.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class ProfileSettingsController : Controller
{
    private readonly ApplicationDbContext _context;

    // Constructor to inject ApplicationDbContext
    public ProfileSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }



    // Action to display all user details
    public IActionResult Index(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userDetails = _context.ApplicationUser.FirstOrDefault(u => u.Id == userId);

        if (userDetails == null)
        {
            return NotFound();
        }

        return View(userDetails);
    }

    // Action to update user details
    public IActionResult Update()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return NotFound();
        }

        ApplicationUser applicationUserFromDb = _context.ApplicationUser.Find(userId);

        if (applicationUserFromDb == null)
        {
            return NotFound();
        }

        return View(applicationUserFromDb);
    }


    [HttpPost]
    public IActionResult Update(ApplicationUser model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        if (ModelState.IsValid)
        {
            ApplicationUser obj = _context.ApplicationUser.Find(userId);
            if (obj == null)
            {
                return NotFound();
            }

            obj.FirstName = model.FirstName;
            obj.LastName = model.LastName;
            obj.Address = model.Address;
            obj.DOB = model.DOB;
            obj.PhoneNumber = model.PhoneNumber;
            obj.join_date = model.join_date;

            try
            {
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return RedirectToAction("ConcurrencyError", "Error");
            }
        }
        return View(model);
    }
    
}
