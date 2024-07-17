using HRMS_Web.DataAccess.Data;
using HRMS_Web.IService;
using HRMS_Web.Models;
using HRMS_Web.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

public class ProfileSettingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUserService _userService = null;

    public ProfileSettingsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IUserService userService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public IActionResult Index(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userDetails = _context.ApplicationUser
            .Include(u => u.Department)
            .FirstOrDefault(u => u.Id == userId);

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
            new BreadcrumbItem { Title = "Edit", Url = Url.Action("Edit", "ProfileSettings") }
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

        return View();
    }

    [HttpPost]
    public IActionResult SaveFile(IFormFile ProfileImage)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = _context.ApplicationUser.Find(userId);

        if (user == null)
        {
            return NotFound();
        }

        if (ProfileImage != null && ProfileImage.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                ProfileImage.CopyTo(ms);
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
        user.Photo = this.GetImage(Convert.ToBase64String(user.Photo));

        return Json(user);
    }

    public byte[] GetImage(string sBase64String)
    {
        byte[] bytes = null;
        if (!string.IsNullOrEmpty(sBase64String))
        {
            bytes = Convert.FromBase64String(sBase64String);
        }
        return bytes;
    }
}
