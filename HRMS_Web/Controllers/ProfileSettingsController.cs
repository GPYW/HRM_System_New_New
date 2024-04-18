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

        // Now you have the userId, you can use it to retrieve user details from the database
        var userDetails = _context.ApplicationUser.FirstOrDefault(u => u.Id == userId);

        if (userDetails == null)
        {
            return NotFound(); // Return 404 if user not found
        }

        return View(userDetails);
    }

    // Action to update user details
    public IActionResult Update()
    {
        // Get the ID of the currently logged-in user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return NotFound();
        }

        // Find the ApplicationUser based on the retrieved ID
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
            // Find the user object from the database
            ApplicationUser obj = _context.ApplicationUser.Find(userId);
            if (obj == null)
            {
                return NotFound();
            }

            // Update properties of the ApplicationUser object
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
                // Handle concurrency conflicts if needed
                return RedirectToAction("ConcurrencyError", "Error");
            }
        }
        // If ModelState is not valid, return to the Edit view with the model to show validation errors
        return View(model);
    }
    
}
