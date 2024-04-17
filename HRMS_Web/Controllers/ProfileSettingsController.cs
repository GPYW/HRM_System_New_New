using HRMS_Web.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HRMS_Web.Models;
using System.Security.Claims;

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
    [HttpPost]
    public IActionResult Update(ApplicationUser model)
    {
        if (ModelState.IsValid)
        {
            // Update user details in the database
            _context.ApplicationUser.Update(model);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redirect to user details page
        }

        return View(model); // Return to the update form with validation errors
    }



    // Action to delete user details
    public IActionResult Delete(String id)
    {
        // Retrieve user details from the database
        var userDetails = _context.ApplicationUser.FirstOrDefault(u => u.Id == id);

        if (userDetails == null)
        {
            return NotFound(); // Return 404 if user not found
        }

        _context.ApplicationUser.Remove(userDetails);
        _context.SaveChanges();

        return RedirectToAction("Index"); // Redirect to user details page
    }

    
}
