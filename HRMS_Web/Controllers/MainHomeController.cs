using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class MainHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
