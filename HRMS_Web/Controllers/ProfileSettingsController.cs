using HRMS_Web.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HRMS_Web.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using HRMS_Web.ViewModel;
using Microsoft.AspNetCore.Hosting;  // Ensure this using statement is present
using System.IO;  // Ensure this using statement is present
using System;

public class ProfileSettingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    // Constructor to inject ApplicationDbContext and IWebHostEnvironment
    public ProfileSettingsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
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

        ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Profile Settings", Url = Url.Action("Index", "ProfileSettings") }
        };

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

        ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Profile Settings", Url = Url.Action("Index", "ProfileSettings") },
            new BreadcrumbItem { Title = "Edit", Url = Url.Action("Edit", "ProfileSettings") }
        };

        return View(applicationUserFromDb);
    }

    [HttpPost]
    public IActionResult Update(ApplicationUser model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            obj.UserName = model.UserName;

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
    }

    public ActionResult UploadPhoto()
    {
        ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Profile Settings", Url = Url.Action("Index", "ProfileSettings") },
            new BreadcrumbItem { Title = "Upload Profile Picture", Url = Url.Action("UploadPhoto", "ProfileSettings") }
        };

        return View();
    }

    [HttpPost]
    public ActionResult UploadPhoto(EmployeeViewModel vm, ApplicationUser model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        string stringFileName = UploadFile(vm);
        {
            ApplicationUser obj = _context.ApplicationUser.Find(userId);

            if (obj == null)
            {
                return NotFound();
            }

            obj.ProfileImage = stringFileName;

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
        
        }

    private string UploadFile(EmployeeViewModel vm)
    {
        string fileName = null;
        if (vm.ProfileImage != null)
        {
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            fileName = Guid.NewGuid().ToString() + "_" + vm.ProfileImage.FileName;
            string filePath = Path.Combine(uploadDir, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.ProfileImage.CopyTo(fileStream);
            }
        }

        return fileName;
    }
}
