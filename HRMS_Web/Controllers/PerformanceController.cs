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
using Microsoft.AspNetCore.Identity;


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
            var tasks = _db.Tasks.ToList();
            return View(tasks);
        }

        public IActionResult CreateTask()
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

        //edit project details

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
            obj.StartDate = model.StartDate;
            obj.EndDate = model.EndDate;
            obj.P_Status = model.P_Status;
            obj.P_Priority = model.P_Priority;
            obj.Rate = model.Rate;

            _db.SaveChanges();
            return RedirectToAction("Assign");
        }

        //edit task details

        public IActionResult EditTask(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Tasks? TasksFromDb = _db.Tasks.Find(id);

            if (TasksFromDb == null)
            {
                return NotFound();
            }
            return View(TasksFromDb);
        }

        [HttpPost]
        public IActionResult EditTask(Tasks model)
        {
            Tasks? obj = _db.Tasks.Find(model.TaskID);
            if (obj == null)
            {
                return NotFound();
            }
            obj.Task_Name = model.Task_Name;
            obj.Task_Description = model.Task_Description;
            obj.Due_Date = model.Due_Date;
            obj.T_Priority = model.T_Priority;
            obj.T_Status = model.T_Status;

            _db.SaveChanges();
            return RedirectToAction("Tasks");
        }

        //create project

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

            // Split LeaderFullName into first name and last name
            var names = project.Project_Manager.Split(' ');
            if (names.Length < 2)
            {
                // Handle error if name is not in expected format
                ModelState.AddModelError("LeaderFullName", "Please select a valid leader.");
                return View(project);
            }

            var firstName = names[0];
            var lastName = names[1];

            // Query the ApplicationUser table to get the LeaderID
            var user = await _db.ApplicationUser
                .FirstOrDefaultAsync(u => u.FirstName == firstName && u.LastName == lastName);

            if (user == null)
            {
                // Handle error if leader is not found
                ModelState.AddModelError("LeaderFullName", "Leader not found.");
                return View(project);
            }

            // Set the LeaderID for the project
            project.LeaderID = user.CompanyID;

            _db.Projects.Add(project);
                await _db.SaveChangesAsync();

            return RedirectToAction("Projects");
        }

        //create task

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public async Task<IActionResult> CreateTask(Tasks task)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var state in ModelState)
        //        {
        //            foreach (var error in state.Value.Errors)
        //            {
        //                Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
        //            }
        //        }

        //        //return View(project);
        //    }


        //    _db.Tasks.Add(task);
        //    await _db.SaveChangesAsync();

        //    return RedirectToAction("Tasks");
        //}


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTask(Tasks task)
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
               //eturn View(task);
            }

            // Split TeamMember into first name and last name
            var names = task.TeamMember.Split(' ');
            if (names.Length < 2)
            {
                ModelState.AddModelError("TeamMember", "Please enter both first name and last name for the team member.");
                return View(task);
            }

            var firstName = names[0];
            var lastName = names[1];

            // Query the ApplicationUser table to get the User
            var user = await _db.ApplicationUser
                .FirstOrDefaultAsync(u => u.FirstName == firstName && u.LastName == lastName);

            if (user == null)
            {
                ModelState.AddModelError("TeamMember", "Team member not found.");
                return View(task);
            }

            // Retrieve the ProjectID based on the assigned project name
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Project_Name == task.Projects.Project_Name);

            if (project == null)
            {
                ModelState.AddModelError("ProjectID", "Project not found.");
                return View(task);
            }

            // Set the UserId and ProjectID in the task
            task.UserId = user.Id;
            task.ProjectID = project.ProjectID;

            // Add and save the task to the database
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Performance");
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

        //Delete task details

        public IActionResult DeleteTask(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Tasks? TasksFromDb = _db.Tasks.Find(id);

            if (TasksFromDb == null)
            {
                return NotFound();
            }
            return View(TasksFromDb);
        }
        [HttpPost, ActionName("DeleteTask")]
        public IActionResult DeleteTaskPOST(int? id)
        {
            Tasks? obj = _db.Tasks.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Tasks.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Tasks");
        }


    }
}
