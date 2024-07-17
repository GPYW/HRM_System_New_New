using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using HRMS_Web.DataAccess.Data;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;


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

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Projects? ProjectsFromDb = _db.Projects.Find(id);

            if (ProjectsFromDb == null)
            {
                return NotFound();
            }
            return View(ProjectsFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Projects model)
        {
            Projects? obj = _db.Projects.Find(model.ProjectID);
            if (obj == null)
            {
                return NotFound();
            }
            obj.Project_Name = model.Project_Name;
            obj.Project_Description = model.Project_Description;
            obj.Client = model.Client;
            obj.Project_Manager = model.Project_Manager;
            obj.Project_Team = model.Project_Team;
            obj.StartDate = model.StartDate;
            obj.EndDate = model.EndDate;
            obj.P_Status = model.P_Status;
            obj.P_Priority = model.P_Priority;
            obj.Rate = model.Rate;

            _db.SaveChanges();
            return RedirectToAction("Assign");
        }


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


        //Delete project details

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Projects? ProjectsFromDb = _db.Projects.Find(id);

            if (ProjectsFromDb == null)
            {
                return NotFound();
            }
            return View(ProjectsFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Projects? obj = _db.Projects.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Projects.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Assign");
        }


    }
}
