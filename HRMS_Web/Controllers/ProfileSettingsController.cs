using Microsoft.AspNetCore.Mvc;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace HRMS_Web.Controllers
{
    public class ProfileSettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
