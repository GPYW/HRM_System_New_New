using HRMS_Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class PerformanceController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Appraisal()
        {
            return View();
        } 
        public IActionResult AppraisalForm()
        {
            return View();
        }

    }


    //private string GetBreadcrumbTitle(string action)
    //{
    //    return action switch
    //    {
    //        "Dashboard" => "Home",
    //        //"Reports" => "Reports",
    //        //"MarkAttendance" => "Mark Attendance",
    //        //"ViewHistory" => "View History",
    //        //"AddAttendance" => "Add Attendance",
    //        _ => action,
    //    };
    //}
}
