using HRMS_Web.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HRMS_Web.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using HRMS_Web.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;
using HRMS_Web.iService;
using Microsoft.AspNetCore.Http;

public class ProfileSettingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly iUserService _userService;

    public ProfileSettingsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, iUserService userService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userDetails = _context.ApplicationUser
            .Include(u => u.Department)
            .FirstOrDefault(u => u.Id == userId);

        if (userDetails == null)
        {
            return NotFound();
        }

        if (userDetails.Photo != null)
        {
            string imageBase64Data = Convert.ToBase64String(userDetails.Photo);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewData["ProfileImage"] = imageDataURL;
        }

        ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Profile Settings", Url = Url.Action("Index", "ProfileSettings") }
        };

        return View(userDetails);
    }

    public IActionResult Update()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return NotFound();
        }

        var applicationUserFromDb = _context.ApplicationUser
            .Include(u => u.Department)
            .FirstOrDefault(u => u.Id == userId);

        if (applicationUserFromDb == null)
        {
            return NotFound();
        }

        ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Profile Settings", Url = Url.Action("Index", "ProfileSettings") },
            new BreadcrumbItem { Title = "Edit", Url = Url.Action("Update", "ProfileSettings") }
        };

        return View(applicationUserFromDb);
    }

    [HttpPost]
    public IActionResult Update(ApplicationUser model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var obj = _context.ApplicationUser.Find(userId);
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
        catch (DbUpdateConcurrencyException)
        {
            return RedirectToAction("ConcurrencyError", "Error");
        }
    }

    public ActionResult UploadPhoto()
    {
        ViewData["Breadcrumb"] = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Title = "Profile Settings", Url = Url.Action("Index", "ProfileSettings") },
            new BreadcrumbItem { Title = "Upload Profile Picture", Url = Url.Action("UploadPhoto", "ProfileSettings") }
        };

        return View(new EmployeeViewModel());
    }

    [HttpPost]
    public IActionResult SaveFile(EmployeeViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = _context.ApplicationUser.Find(userId);
        if (user == null)
        {
            return NotFound();
        }

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                model.ProfileImage.CopyTo(ms);
                var fileBytes = ms.ToArray();
                user.Photo = fileBytes;

                _context.SaveChanges();
            }
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public JsonResult GetSavedUser()
    {
        var user = _userService.GetSavedUser();
        user.Photo = GetImage(Convert.ToBase64String(user.Photo));
        return Json(user);
    }

    private byte[] GetImage(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            return null;
        }
        return Convert.FromBase64String(base64String);
    }
}
