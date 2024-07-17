using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using HRMS_Web.DataAccess.Data;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace HRMS_Web.Controllers
{
    public class PerformanceController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private object upload;

        public PerformanceController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult CreateProject()
        {
            return View();
        }

        public IActionResult Projects()
        {
            var projects = _db.Projects.ToList();
            return View(projects);
        }

        public IActionResult Tasks()
        {
            return View();
        }

        public IActionResult ViewHistory()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Assign()
        {
            var projects = _db.Projects.ToList();
            return View(projects);
        }

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public async Task<IActionResult> CreateProject(Projects project)
        //{
        //    if (!projectState.IsValid)
        //    {
        //        foreach (var state in projectState)
        //        {
        //            foreach (var error in state.Value.Errors)
        //            {
        //                Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
        //            }
        //        }


        //        //return Json(new { success = false, message = "project validation failed.", errors = projectState.Values.SelectMany(v => v.Errors) });
        //    }
        //    //Handle file upload
        //    string uniqueFileName = null;
        //    if (project.FileUpload != null)
        //    {
        //        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
        //        uniqueFileName = Guid.NewGuid().ToString() + "_" + project.FileUpload.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await project.FileUpload.CopyToAsync(fileStream);
        //        }
        //        project.UploadFile = uniqueFileName;
        //    }

        //    _dbContext.Projects.Add(project);
        //    await _dbContext.SaveChangesAsync();

        //    return Json(new { success = true, message = "Project created successfully!" });



        //}

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProject(Projects project)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }

                //return View(project);
            }

            // Handle file upload
            string uniqueFileName = null;
            if (project.FileUpload != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

                // Check if directory exists, if not, create it
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + project.FileUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await project.FileUpload.CopyToAsync(fileStream);
                }
                project.UploadFile = uniqueFileName;
            }

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return RedirectToAction("Projects");
        }



    }
}
