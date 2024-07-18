using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    public class PerformanceController : Controller
    {
        private readonly IPerformanceService _performanceService;

        public PerformanceController(IPerformanceService performanceService)
        {
            _performanceService = performanceService;
        }

        public IActionResult Dashboard()
        {
            var model = _performanceService.GetDashboardData(User.Identity.Name);
            return View(model);
        }
    }
}
